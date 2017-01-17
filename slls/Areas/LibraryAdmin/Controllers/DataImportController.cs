using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Windows.Forms.Design;
using System.Xml;
using LumenWorks.Framework.IO.Csv;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;
using slls.Models;
using slls.Utils.Helpers;
using slls.ViewModels;

namespace slls.Areas.LibraryAdmin
{
    public class DataImportController : AdminBaseController
    {
        private ApplicationUserManager _userManager;

        private ApplicationUserManager UserManager
        {
            get { return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            set { _userManager = value; }
        }
        
        public ActionResult ImportUsers()
        {
            var dataVm = new DataImportViewModel();
            var usersVm = new UserImportViewModel();
            List<ApplicationUser> newUsers = new List<ApplicationUser>();

            if (TempData["tempDataImportViewModel"] != null)
            {
                dataVm = (DataImportViewModel) TempData["tempDataImportViewModel"];
            }

            if (dataVm.File != null)
            {
                dataVm.FilePath = dataVm.File.FileName;
                dataVm.FileName = Path.GetFileName(dataVm.File.FileName);

                var defaultPasswordPart = App_Settings.Settings.GetParameterValue(
                "Security.Passwords.DefaultPassweordPart", "6174",
                "The numeric part of a default password when new users are added automatically, or via an import script or tool. A default password is constructed from a user's firstname, the default numeric part and the first letter of their surname (e.g. tim1234u)", dataType: "int");
                
                if (Path.GetExtension(dataVm.File.FileName).ToLower() == ".xml")
                {
                    var doc = new XmlDocument();
                    var count = 0;
                    doc.Load(dataVm.File.InputStream);

                    XmlNode root = doc.DocumentElement;

                    var nodeList = root.SelectNodes("//users/user");

                    foreach (XmlNode userNode in nodeList)
                    {
                        count++;
                        var user = new ApplicationUser()
                        {
                            UserName = userNode.SelectSingleNode("sAMAccountName").InnerText ?? "",
                            AdObjectGuid = userNode.SelectSingleNode("objectGuid").InnerText ?? "",
                            Firstname = userNode.SelectSingleNode("givenname").InnerText ?? "",
                            Lastname = userNode.SelectSingleNode("sn").InnerText ?? "",
                            Position = userNode.SelectSingleNode("title").InnerText ?? "",
                            Email = userNode.SelectSingleNode("mail").InnerText ?? "",
                            PhoneNumber = userNode.SelectSingleNode("telephoneNumber").InnerText ?? "",
                            IsLive = true,
                            FoundInAd = true,
                            IgnoreAd = false,
                            SelfLoansAllowed = true,
                            InputDate = DateTime.Now,
                            TempPassword = string.Format("{0}{1}{2}", userNode.SelectSingleNode("givenname").InnerText, defaultPasswordPart, userNode.SelectSingleNode("sn").InnerText.Substring(0,1)).ToLower(), 
                            CanDelete = true
                        };
                        //Only add department is there's one specified ...
                        if(!string.IsNullOrEmpty(userNode.SelectSingleNode("department").InnerText))
                        {
                            user.DepartmentId =
                                DepartmentsController.GetDepartmentId(userNode.SelectSingleNode("department").InnerText);
                        }
                        //Only add location (office) is there's one specified ...
                        if (!string.IsNullOrEmpty(userNode.SelectSingleNode("physicaldeliveryofficename").InnerText))
                        {
                            user.LocationID =
                                LocationsController.GetOfficeId(
                                    userNode.SelectSingleNode("physicaldeliveryofficename").InnerText);
                        }
                        newUsers.Add(user);
                    }
                    usersVm.Users = newUsers;
                    dataVm.Rows = count;
                }
                else //CSV!!!
                {
                    var count = 0;
                    if (dataVm.File != null && dataVm.File.ContentLength > 0)
                    {
                        if (dataVm.File.FileName.EndsWith(".csv"))
                        {
                            Stream stream = dataVm.File.InputStream;
                            DataTable csvTable = new DataTable();
                            using (CsvReader csvReader =
                                new CsvReader(new StreamReader(stream), true))
                            {
                                csvTable.Load(csvReader);

                                foreach (DataRow userRow in csvTable.Rows)
                                {
                                    count++;
                                    var user = new ApplicationUser();
                                    foreach (DataColumn field in csvTable.Columns)
                                    {
                                        switch (field.ColumnName)
                                        {
                                            case "objectGuid":
                                                user.AdObjectGuid = userRow[field.ColumnName].ToString();
                                                break;
                                            case "sAMAccountName":
                                                user.UserName = userRow[field.ColumnName].ToString();
                                                break;
                                            case "title":
                                                user.Position = userRow[field.ColumnName].ToString();
                                                break;
                                            case "givenname":
                                                user.Firstname = userRow[field.ColumnName].ToString();
                                                break;
                                            case "sn":
                                                user.Lastname = userRow[field.ColumnName].ToString();
                                                break;
                                            case "mail":
                                                user.Email = userRow[field.ColumnName].ToString();
                                                break;
                                            case "telephoneNumber":
                                                user.PhoneNumber = userRow[field.ColumnName].ToString();
                                                break;
                                            case "department":
                                                //Only add department is there's one specified ...
                                                if (!string.IsNullOrEmpty(userRow[field.ColumnName].ToString()))
                                                {
                                                    user.DepartmentId =
                                                        DepartmentsController.GetDepartmentId(userRow[field.ColumnName].ToString());
                                                }
                                                break;
                                            case "physicaldeliveryofficename":
                                                //Only add location (office) is there's one specified ...
                                                if (!string.IsNullOrEmpty(userRow[field.ColumnName].ToString()))
                                                {
                                                    user.LocationID =
                                                        LocationsController.GetOfficeId(userRow[field.ColumnName].ToString());
                                                }
                                                break;
                                            default:
                                                if (field.ColumnName == "objectGuid")
                                                {
                                                    user.AdObjectGuid = userRow[field.ColumnName].ToString();
                                                }
                                                break;
                                        }
                                    }
                                    user.TempPassword =
                                        string.Format("{0}{1}{2}", user.Firstname, defaultPasswordPart,
                                            user.Lastname.Substring(0, 1)).ToLower();

                                    newUsers.Add(user);
                                }
                                usersVm.Users = newUsers;
                                dataVm.Rows = count;
                            }
                        }
                    }
                }

                //TempData["tempDataImportViewModel"] = dataVm;
                TempData["tempUserImportViewModel"] = usersVm;
            }

            dataVm.AcceptedFileTypes = ".xml, .XML, .csv, .CSV, .xls, .XLS, .xlsx, .XLSX";
            dataVm.Tip = "Browse for the file containing your user data, then click the 'Open' button to load it.";
            TempData["tempDataImportViewModel"] = dataVm;
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("usersSeeAlso", ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["controller"].ToString());
            ViewBag.Title = "Import Users From File";
            return View(dataVm);
        }

        public async Task<ActionResult> DoUserImport()
        {
            var dataVm = new DataImportViewModel();
            int countInserted = 0;
            int countUpdated = 0;
            int countFailed = 0;
            
            if (TempData["tempUserImportViewModel"] != null)
            {
                var usersVm = (UserImportViewModel)TempData["tempUserImportViewModel"];

                foreach (var user in usersVm.Users)
                {
                    var existingUser = UserManager.Users.FirstOrDefault(u => u.UserName == user.UserName);
                    if (existingUser == null)
                    {
                        //Insert a new user ...
                        var insertResult = await UserManager.CreateAsync(user, user.TempPassword);
                        if (insertResult.Succeeded)
                        {
                            countInserted++;
                        }
                        else
                        {
                            countFailed++;
                        }

                        //Add the "OPAC User" role ...
                        await UserManager.AddToRolesAsync(user.Id, "OPAC User");
                        
                        //Remove the temp password ...
                        user.TempPassword = null;
                        await UserManager.UpdateAsync(user);

                    }
                    else
                    {
                        existingUser.AdObjectGuid = user.AdObjectGuid;
                        existingUser.FoundInAd = true;
                        existingUser.IsLive = true;
                        existingUser.TempPassword = null;
                        if(existingUser.Firstname != user.Firstname)
                        {
                            existingUser.Firstname = user.Firstname;
                        }
                        if (existingUser.Lastname != user.Lastname)
                        {
                            existingUser.Lastname = user.Lastname;
                        }
                        if (existingUser.Position != user.Position)
                        {
                            existingUser.Position = user.Position;
                        }
                        if (existingUser.Email != user.Email)
                        {
                            existingUser.Email = user.Email;
                        }
                        if (existingUser.PhoneNumber != user.PhoneNumber)
                        {
                            existingUser.PhoneNumber = user.PhoneNumber;
                        }
                        if (user.LocationID > 0)
                        {
                            existingUser.LocationID = user.LocationID;
                        }
                        if (user.DepartmentId > 0)
                        {
                            existingUser.DepartmentId = user.DepartmentId;
                        }

                        //Update the existing user ...
                        var updateResult = await UserManager.UpdateAsync(existingUser);
                        if (updateResult.Succeeded)
                        {
                            countUpdated++;
                        }
                    }
                }
            }

            TempData["SuccessDialogMsg"] = countInserted + " new users inserted. ";
            if (countUpdated > 0)
            {
                TempData["SuccessDialogMsg"] += countUpdated + " existing users updated. ";
            }
            if (countFailed > 0)
            {
                TempData["SuccessDialogMsg"] += countFailed + " records could not be inserted or updated. ";
            }
            
            return RedirectToAction("Index", "LibraryUsers");
        }

        public ActionResult _BrowseForFile(string title = "Select File ...", string buttonText = "Open", string glyphicon = "glyphicon-folder-open", string tip = "Browse for and select the file and then click the 'Open' button.")
        {
            var viewModel = new DataImportViewModel();
            if (TempData["tempDataImportViewModel"] != null)
            {
                viewModel = (DataImportViewModel)TempData["tempDataImportViewModel"];
            }
            else
            {
                viewModel.Title = title;
                viewModel.ButtonText = buttonText;
                viewModel.Glyphicon = "glyphicon " + glyphicon;
                viewModel.Tip = tip;
            }

            ViewBag.Title = title;
            return PartialView(viewModel);
        }

        [HttpPost]
        public ActionResult _BrowseForFile(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0 && (file.ContentType == "text/xml" || file.ContentType == "text/plain"))
            {
                var viewModel = new DataImportViewModel()
                {
                    File = file,
                    FilePath = file.FileName,
                    FileName = Path.GetFileName(file.FileName)
                };
                TempData["tempDataImportViewModel"] = viewModel;
                return RedirectToAction("ImportUsers");
            }
            return RedirectToAction("ImportUsers");
        }

        public FileResult ViewFile()
        {
            var dataVm = new DataImportViewModel();

            if (TempData["tempDataImportViewModel"] != null)
            {
                dataVm = (DataImportViewModel)TempData["tempDataImportViewModel"];
                Response.AppendHeader("Content-Disposition", "inline; filename=" + dataVm.FileName + ";");
                ViewBag.Title = dataVm.FileName;
                return File(dataVm.FilePath, "text/xml", dataVm.FileName);
            }
            return null;
        }
    }
}