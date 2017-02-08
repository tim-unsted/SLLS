using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using slls.App_Settings;
using slls.DAO;
using slls.Hubs;
using slls.Models;
using slls.Utils.Helpers;
using slls.ViewModels;
using Westwind.Globalization;

namespace slls.Areas.LibraryAdmin
{
    public class LibraryUsersController : UsersBaseController
    {
        private RoleManager<ApplicationRole> _roleManager;
        private ApplicationUserManager _userManager;
        private const int RecordsPerPage = 50;
        private readonly DbEntities _db = new DbEntities();
        private readonly string _entityName = DbRes.T("Users.LibraryUser", "FieldDisplayName");
        private readonly string _customerPackage = App_Settings.GlobalVariables.Package;

        public LibraryUsersController()
        {
            ViewBag.Title = DbRes.T("LibraryUsers", "EntityType");
            ViewBag.RecordsPerPage = RecordsPerPage;
        }

        private ApplicationUserManager UserManager
        {
            get { return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            set { _userManager = value; }
        }

        public RoleManager<ApplicationRole> RoleManager
        {
            get
            {
                return _roleManager ?? new RoleManager<ApplicationRole>(
                    new RoleStore<ApplicationRole>(new ApplicationDbContext()));

            }
            private set { _roleManager = value; }
        }

        // GET: LibraryUsers
        public ActionResult Index(string selectedLetter = "A", bool showAll = false)
        {
            var allUsers = UserManager.Users
                .Where(u => u.CanDelete && u.Lastname != null);

            if (showAll == false)
            {
                allUsers = allUsers.Where(u => u.IsLive);
            }

            IEnumerable<ApplicationUser> libraryUsers;

            if (selectedLetter == null)
            {
                var count = (from u in allUsers
                             select new { u.Id }).Count();

                selectedLetter = count > 1000 ? "A" : "All";
            }

            var viewModel = new LibraryUsersIndexViewModel
            {
                SelectedLetter = selectedLetter,
                FirstLetters = allUsers
                    .GroupBy(u => u.Lastname.Substring(0, 1))
                    .Select(x => x.Key.ToUpper())
                    .ToList(),
                ShowAll = showAll
            };

            // Get a view of users starting with the selected letter/number ...
            if (string.IsNullOrEmpty(selectedLetter) || selectedLetter == "All")
            {
                libraryUsers = allUsers; //_db.LibraryUsers.ToList();
            }
            else
            {
                if (selectedLetter == "0-9")
                {
                    var numbers = Enumerable.Range(0, 10).Select(i => i.ToString());
                    libraryUsers = allUsers.ToList()
                        .Where(u => numbers.Contains(u.Lastname.Substring(0, 1)))
                        .ToList();
                }
                else if (selectedLetter == "non alpha")
                {
                    //Get a list of non-alpha characters
                    var nonalpha1 = Enumerable.Range(32, 16).Select(i => ((char)i).ToString()).ToList();
                    var nonalpha2 = Enumerable.Range(91, 6).Select(i => ((char)i).ToString()).ToList();
                    var nonalpha3 = Enumerable.Range(123, 4).Select(i => ((char)i).ToString()).ToList();
                    var nonalpha = nonalpha1.Concat(nonalpha2).Concat(nonalpha3);

                    libraryUsers = allUsers.ToList()
                        .Where(u => nonalpha.Contains(u.Lastname.Substring(0, 1)))
                        .ToList();
                }
                else
                {
                    libraryUsers = allUsers.ToList()
                        .Where(u => u.Lastname.StartsWith(selectedLetter,StringComparison.InvariantCultureIgnoreCase))
                        .ToList();
                }

            }

            viewModel.LibraryUsers = libraryUsers;
            ViewBag.Title = showAll == false ? ViewBag.Title + " (Live Only)" : ViewBag.Title + " (All)";
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("usersSeeAlso", ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["controller"].ToString());
            return View(viewModel);
        }

        // GET: LibraryUsers/Create
        public ActionResult Create()
        {
            //Establish what roles the current user has. A user may not grant another user greater permissions than they have themselves ...
            var userRoles = Roles.GetUserRoles();

            //Only Admin users can get this far, so ensure that any lower user type is included by default ...
            userRoles.Add("OPAC User");

            //Create a list of all default roles that should be selected/ticked when the form opens ...
            var defaultMenuRoles = new List<string> { "OPAC User" };

            IEnumerable<SelectListItem> rolesList;
            if (Roles.IsBaileyAdmin())
            {
                rolesList =
                RoleManager.Roles.Where(r => r.Packages.Contains(_customerPackage))
                    .ToList()
                    .Select(x => new SelectListItem
                    {
                        Selected = defaultMenuRoles.Contains(x.Name),
                        Text = x.Name,
                        Value = x.Name
                    });
            }
            else
            {
                rolesList =
                RoleManager.Roles.Where(r => userRoles.Contains(r.Name) && r.Packages.Contains(_customerPackage))
                    .ToList()
                    .Select(x => new SelectListItem
                    {
                        Selected = defaultMenuRoles.Contains(x.Name),
                        Text = x.Name,
                        Value = x.Name
                    });
            }

            var viewModel = new LibraryUserAddViewModel()
            {
                IsLive = true,
                SelfLoansAllowed = true,
                IgnoreAd = false,
                RolesList = rolesList
            };
            
            viewModel.PasswordTip = GetPasswordTips();
            ViewBag.Title = "Add New " + _entityName;
            ViewBag.DepartmentID = new SelectList(_db.Departments, "DepartmentID", "Department1");
            ViewBag.LocationID = new SelectList(_db.Locations, "LocationID", "Location1");

            return View(viewModel);
        }

        // POST: LibraryUsers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(LibraryUserAddViewModel viewModel, params string[] selectedRoles)
        {
            if (ModelState.IsValid)
            {
                //Add a new Identity records and get the user's Id...
                var identityUser = new ApplicationUser
                {
                    UserName = viewModel.UserName,
                    LibraryUserId = 0,
                    Email = viewModel.Email,
                    AdObjectGuid = viewModel.UserName,
                    Firstname = viewModel.Firstname,
                    Lastname = viewModel.Lastname,
                    UserBarcode = viewModel.UserBarcode,
                    DepartmentId = viewModel.DepartmentId,
                    LocationID = viewModel.LocationId,
                    SelfLoansAllowed = viewModel.SelfLoansAllowed,
                    IgnoreAd = viewModel.IgnoreAd,
                    IsLive = viewModel.IsLive,
                    FoundInAd = false,
                    Notes = viewModel.Notes,
                    InputDate = DateTime.Now,
                    CanDelete = true
                };

                var result = await UserManager.CreateAsync(identityUser, viewModel.Password);

                if (!result.Succeeded)
                {
                    var exceptionText =
                        result.Errors.Aggregate(
                            "Add New " + _entityName + " Failed - The following error(s) were encountered: <br>",
                            (current, error) => current + (error + "<br>"));
                    ModelState.AddModelError("", exceptionText);
                }
                else
                {
                    if (selectedRoles != null)
                    {
                        result = await UserManager.AddToRolesAsync(identityUser.Id, selectedRoles);
                        if (!result.Succeeded)
                        {
                            ModelState.AddModelError("", result.Errors.First());
                        }
                    }
                }

                if (!ModelState.IsValid)
                {
                    ViewBag.DepartmentID = new SelectList(_db.Departments, "DepartmentID", "Department1", viewModel.DepartmentId);
                    ViewBag.LocationID = new SelectList(_db.Locations, "LocationID", "Location1", viewModel.LocationId);

                    //Establish what roles the current user has. A user may not grant another user greater permissions than they have themselves ...
                    var userRoles = Roles.GetUserRoles();

                    //Only Admin users can get this far, so ensure that any lower user type is included by default ...
                    userRoles.Add("OPAC User");

                    //Create a list of all default roles that should be selected/ticked when the form opens ...
                    var defaultMenuRoles = new List<string> { "OPAC User" };
                    viewModel.RolesList =
                        RoleManager.Roles.Where(r => userRoles.Contains(r.Name)).ToList().Select(x => new SelectListItem
                        {
                            Selected = defaultMenuRoles.Contains(x.Name),
                            Text = x.Name,
                            Value = x.Name
                        });
                    ViewBag.Title = "Add New " + _entityName;
                    return View(viewModel);
                }

                return RedirectToAction("Index");
            }

            ViewBag.DepartmentID = new SelectList(_db.Departments, "DepartmentID", "Department1", viewModel.DepartmentId);
            ViewBag.LocationID = new SelectList(_db.Locations, "LocationID", "Location1", viewModel.LocationId);
            ViewBag.Title = "Add New " + _entityName;
            return View(viewModel);
        }

        // GET: LibraryUsers/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var libraryUser = UserManager.FindById(id);
            if (libraryUser == null)
            {
                return HttpNotFound();
            }

            //Establish what roles the current user has. A user may not grant another user greater permissions than they have themselves ...
            var userRoles = Roles.GetUserRoles();

            //Only Admin users can get this far, so ensure that any lower user type is included by default ...
            userRoles.Add("OPAC User");
            
            //Create a list of all default roles that should be selected/ticked when the form opens ...
            var defaultMenuRoles = new List<string> { "OPAC User" };

            IEnumerable<SelectListItem> rolesList;
            if (Roles.IsBaileyAdmin())
            {
                rolesList =
                RoleManager.Roles.Where(r => r.Packages.Contains(_customerPackage))
                    .ToList()
                    .Select(x => new SelectListItem
                    {
                        Selected = defaultMenuRoles.Contains(x.Name),
                        Text = x.Name,
                        Value = x.Name
                    });
            }
            else
            {
                rolesList =
                RoleManager.Roles.Where(r => userRoles.Contains(r.Name) && r.Packages.Contains(_customerPackage))
                    .ToList()
                    .Select(x => new SelectListItem
                    {
                        Selected = defaultMenuRoles.Contains(x.Name),
                        Text = x.Name,
                        Value = x.Name
                    });
            }

            var viewModel = new LibraryUserEditViewModel()
            {
                Id = libraryUser.Id,
                UserName = libraryUser.UserName,
                Firstname = libraryUser.Firstname,
                Lastname = libraryUser.Lastname,
                LocationId = libraryUser.LocationID,
                DepartmentId = libraryUser.DepartmentId,
                IsLive = libraryUser.IsLive,
                IgnoreAd = libraryUser.IgnoreAd,
                SelfLoansAllowed = libraryUser.SelfLoansAllowed,
                Email = libraryUser.Email,
                Notes = libraryUser.Notes,
                RolesList = rolesList
            };

            ViewBag.Title = "Edit " + _entityName;
            ViewBag.DepartmentID = new SelectList(_db.Departments, "DepartmentID", "Department1",
                libraryUser.DepartmentId);
            ViewBag.LocationID = SelectListHelper.OfficeLocationList(id:libraryUser.LocationID ?? 0,addDefault:false);//new SelectList(_db.Locations, "LocationID", "Location1", libraryUser.LocationID);
            return PartialView(viewModel);
        }

        // POST: LibraryUsers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(LibraryUserEditViewModel viewModel, params string[] selectedRoles)
        {
            if (ModelState.IsValid)
            {
                var libraryUser = await UserManager.FindByIdAsync(viewModel.Id);
                if (libraryUser == null)
                {
                    return HttpNotFound();
                }

                libraryUser.UserName = viewModel.UserName;
                libraryUser.UserBarcode = viewModel.UserBarcode;
                libraryUser.Email = viewModel.Email;
                libraryUser.SelfLoansAllowed = viewModel.SelfLoansAllowed;
                libraryUser.IgnoreAd = viewModel.IgnoreAd;
                libraryUser.DepartmentId = viewModel.DepartmentId;
                libraryUser.Firstname = viewModel.Firstname;
                libraryUser.Lastname = viewModel.Lastname;
                libraryUser.IsLive = viewModel.IsLive;
                libraryUser.LocationID = viewModel.LocationId;
                libraryUser.Position = viewModel.Position;
                libraryUser.Notes = viewModel.Notes;
                libraryUser.LastModified = DateTime.Now;
                
                var result = await UserManager.UpdateAsync(libraryUser);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                }
                
                //Update roles/permissions ...
                var userRoles = await UserManager.GetRolesAsync(viewModel.Id);
                selectedRoles = selectedRoles ?? new string[] { };

                result = await UserManager.AddToRolesAsync(viewModel.Id, selectedRoles.Except(userRoles).ToArray());

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                }
                result = await UserManager.RemoveFromRolesAsync(viewModel.Id, userRoles.Except(selectedRoles).ToArray());

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                }

                //return RedirectToAction("Index");
                return Json(new { success = true });
            }
            ViewBag.DepartmentID = new SelectList(_db.Departments, "DepartmentID", "Department1", viewModel.DepartmentId);
            ViewBag.LocationID = new SelectList(_db.Locations, "LocationID", "Location1", viewModel.LocationId);
            ViewBag.Title = "Edit " + _entityName;
            return PartialView(viewModel);
        }


        [HttpGet]
        public ActionResult ResetUserPassword(string id = "")
        {
            var viewModel = new LibraryUserResetPasswordViewModel()
            {
                PasswordTip = GetPasswordTips()
            };
            
            if (!string.IsNullOrEmpty(id))
            {
                var libraryUser = UserManager.FindById(id);
                if (libraryUser != null)
                {
                    viewModel.UserId = libraryUser.Id;
                    viewModel.Fullname = libraryUser.Fullname;
                }
            }
            else
            {
                var users = UserManager.Users.Where(u => u.IsLive && u.CanDelete).OrderBy(u => u.Lastname).ThenBy(u => u.Firstname)
                .ToList()
                .Select(u => new
                {
                    u.Id,
                    Fullname = u.FullnameRev
                });
                viewModel.LibraryUsers = new SelectList(users, "Id", "Fullname");
            }
            
            ViewBag.Title = "Reset Password";
            return PartialView(viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> ResetUserPassword(LibraryUserResetPasswordViewModel viewModel)
        {
            var libraryUser = await UserManager.FindByIdAsync(viewModel.UserId);
            if (libraryUser == null)
            {
                return HttpNotFound();
            }
            
            var store = new UserStore<ApplicationUser>(_db);
            var newPassword = viewModel.NewPassword;
            var hashedNewPassword = UserManager.PasswordHasher.HashPassword(newPassword);
            
            await store.SetPasswordHashAsync(libraryUser, hashedNewPassword);
            var result = await UserManager.UpdateAsync(libraryUser);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", result.Errors.First());
            }

            var code = await UserManager.GeneratePasswordResetTokenAsync(libraryUser.Id);
            var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = libraryUser.Id, code = code }, protocol: Request.Url.Scheme);
            var msgSubject = App_Settings.Settings.GetParameterValue("Security.Passwords.ChangePasswordConfirmationSubject",
                "Your password for Simple Little Library System has changed", "The email 'Subject' when generating an 'Changed Password' confirmation email.", dataType: "longtext");
            var msgBody = App_Settings.Settings.GetParameterValue("Security.Passwords.ChangePasswordConfirmationBody",
                "Your password for Simple Little Library System has recently been changed. If you did not request or authorise this change, click the link below to reset it now. Otherwise, you can ignore this message.", "The email 'Body' when generating an 'Changed Password' confirmation email.", dataType: "longtext");
            await UserManager.SendEmailAsync(libraryUser.Id, msgSubject, msgBody + "<br><br><a href=\"" + callbackUrl + "\">Follow this link to reset your password now</a>");

            TempData["SuccessDialogMsg"] = "User's password has been changed. The user should receive an email alerting them of the change.";
            return Json(new { success = true });
        }

        public ActionResult EncryptPasswords()
        {
            var count = UserManager.Users.Count(u => u.PasswordHash == null && u.TempPassword != null);

            var viewModel = new GenericConfirmationViewModel
            {
                PostConfirmController = "LibraryUsers",
                PostConfirmAction = "PostEncryptPasswords",
                //DetailsText = "There are currently <strong>" + count.ToString() + "</strong> users with temporary, plain-text, passwords. Users will be unable to log in until their temporary passwords have been encypted. <br><br>This may take several minutes, depending on the number of users involved.  However, the process will run in the background so you can continue working. ",
                //ConfirmationText = "Do you want to continue?",
                //ConfirmButtonText = "Continue",
                //ConfirmButtonClass = "btn-success",
                CancelButtonText = "Cancel",
                HeaderText = "Encrypt (Hash) Temporary Passwords?",
                Glyphicon = "glyphicon-ok",
                Count = count
            };

            if (count > 0)
            {
                //viewModel.DetailsText = "There are currently <strong>" + count.ToString() +
                //                        "</strong> users with temporary, plain-text, passwords. Users will be unable to log in until their temporary passwords have been encypted. <br><br>This may take several minutes, depending on the number of users involved.  However, the process will run in the background so you can continue working. " +
                //                        "<br><br><span class='error'><strong>Note: </strong>User records require an email address for this process to work. User records without an email address will be ignored.</span>";
                //viewModel.ConfirmationText = "Do you want to continue?";
                viewModel.ConfirmButtonText = "Continue";
                viewModel.ConfirmButtonClass = "btn-success";
            }
            else
            {
                //viewModel.DetailsText = "There are currently no (0) users with temporary, plain-text, passwords. ";
                viewModel.ConfirmationText = "";
                viewModel.ConfirmButtonText = "Ok";
                viewModel.ConfirmButtonClass = "btn-success";
            }
            return PartialView("EncryptPasswords", viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> PostEncryptPasswords()
        {
            var users = UserManager.Users.Where(u => u.PasswordHash == null && u.TempPassword != null).ToList();
            var store = new UserStore<ApplicationUser>(_db);

            try
            {
                foreach (var libraryUser in users.ToList())
                {
                    var newPassword = libraryUser.TempPassword ?? Settings.GetParameterValue("Security.Passwords.DefaultTemporaryPassword", "temp@123", "The default temporary password to use when importing user data from legacy systems without passwords.", dataType: "text");
                    var hashedNewPassword = UserManager.PasswordHasher.HashPassword(newPassword);
                    await store.SetPasswordHashAsync(libraryUser, hashedNewPassword);
                    libraryUser.TempPassword = null;
                    await UserManager.UpdateAsync(libraryUser);
                }
                return Json(new { success = true });
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message.ToString());
                return Json(new { success = false });
            }
        }

        public string GetPasswordTips()
        {
            var passwordMaxLength =
                int.Parse(Settings.GetParameterValue("Security.Passwords.MaximumLength", "999",
                    "Specifies the maximum required length of a password.", dataType: "int"));
            var passwordMinLength =
                int.Parse(Settings.GetParameterValue("Security.Passwords.MinimumLength", "8",
                    "Specifies the minimum required length of a password.", dataType: "int"));
            var passwordRequireDigit =
                Settings.GetParameterValue("Security.Passwords.RequireDigit", "false",
                    "Specifies whether the password requires at least one digit (0-9).", dataType: "bool") == "true";
            var passwordRequireLowercase =
                Settings.GetParameterValue("Security.Passwords.RequireLowercase", "false",
                    "Specifies whether the password requires at least one lower-case letter.", dataType: "bool") == "true";
            var passwordRequireUppercase =
                Settings.GetParameterValue("Security.Passwords.RequireUppercase", "false",
                    "Specifies whether the password requires at least one upper-case letter", dataType: "bool") == "true";
            var passwordRequireNonLetterOrDigit =
                Settings.GetParameterValue("Security.Passwords.RequireNonLetterOrDigit", "false",
                    "Specifies whether the password requires at least one non-alphanumeric character (e.g. '$', '~', '?', etc.", dataType: "bool") ==
                "true";
                
            var passwordRequirements = new List<string>();
            if (passwordMinLength > 0)
            {
                passwordRequirements.Add(string.Format("Passwords must contain at least {0} characters", passwordMinLength));
            }
            if (passwordMaxLength < 999)
            {
                passwordRequirements.Add(string.Format("Passwords must not contain at more than {0} characters", passwordMinLength));
            }
            if (passwordRequireDigit)
            {
                passwordRequirements.Add(string.Format("Passwords must contain at least one digit ('0'-'9')"));
            }
            if (passwordRequireNonLetterOrDigit)
            {
                passwordRequirements.Add(string.Format("Passwords must contain at least one non-alphanumeric character (e.g. '$', '~', '?', etc.)"));
            }
            if (passwordRequireLowercase)
            {
                passwordRequirements.Add(string.Format("Passwords must contain at least one lower-case character"));
            }
            if (passwordRequireUppercase)
            {
                passwordRequirements.Add(string.Format("Passwords must contain at least one UPPER-case character"));
            }
            return string.Join("<br>", passwordRequirements) + "<br>";
        }

        [HttpGet]
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            // Create new instance of DeleteConfirmationViewModel and pass it to _DeleteConfirmation Dialog (Reusable)
            DeleteConfirmationViewModel deleteConfirmationViewModel = new DeleteConfirmationViewModel
            {
                DeleteEntityIdString = id,
                HeaderText = _entityName,
                PostDeleteAction = "DeleteConfirmed",
                PostDeleteController = "LibraryUsers",
                DetailsText = string.Format("{0} {1}", user.Firstname, user.Lastname) //user.Fullname
            };
            return PartialView("_DeleteConfirmation", deleteConfirmationViewModel);
        }

        [HttpPost]
        public async Task<ActionResult> DeleteConfirmed(DeleteConfirmationViewModel dcvm)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByIdAsync(dcvm.DeleteEntityIdString);
                if (user == null)
                {
                    return HttpNotFound();
                }

                try
                {
                    var result = await UserManager.DeleteAsync(user);
                    if (result.Succeeded)
                    {
                        return Json(new { success = true });
                    }
                    else
                    {
                        ModelState.AddModelError("", result.Errors.First());
                    }

                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", e.Message);
                }
            }
            return PartialView("_DeleteConfirmation", dcvm);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}