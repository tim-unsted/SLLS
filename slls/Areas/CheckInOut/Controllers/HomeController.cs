using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using slls.App_Settings;
using slls.Models;
using slls.Utils.Helpers;
using slls.ViewModels;
using Westwind.Globalization;

namespace slls.Areas.CheckInOut
{
    public class HomeController : CheckInOutBaseController
    {
        private readonly DbEntities _db = new DbEntities();
        private ApplicationUserManager _userManager;

        private ApplicationUserManager UserManager
        {
            get { return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            set { _userManager = value; }
        }

        // GET: CheckInOut/Home
        //Index page for Check-In/Check-Out pages ...
        public ActionResult Index()
        {
            ViewBag.Title = DbRes.T("Borrowing.CheckIn_CheckOut", "EntityType");
            ViewBag.MainHeading = Settings.GetParameterValue("Borrowing.CheckInOut.LargeHeaderText", "Check-In/Check-Out", "The main large heading on the stand-alone Check-In/Check-Out home page.", "System Admin;Loans Admin;", dataType: "longtext");
            ViewBag.SubHeading = Settings.GetParameterValue("Borrowing.CheckInOut.SubHeaderText", "Borrow and Return items here.", "The sub heading text on the stand-alone Check-In/Check-Out home page.", "System Admin;Loans Admin;", dataType: "longtext");
            return View("Index", "_CheckInOutLayout");
        }

        public ActionResult CheckOut(string checkoutUserId = "", bool success = false, bool invalidUser = false)
        {
            //Find the user record in th database: AspNetUsers ...
            var fullName = "";
            var userName = "";
            var user = _db.Users.Find(checkoutUserId);

            if (user != null)
            {
                //fullName = string.Concat(new[] { user.Firstname, " ", user.Lastname });
                fullName = user.Fullname;
                userName = user.UserName;
            }

            // Get a list of barcodes for a drop-down list
            var currentLoans = _db.Borrowings.Where(b => b.Returned == null).Select(b => b.VolumeID);
            var availableVolumes = _db.Volumes.Where(v => v.Deleted == false && v.LoanType.RefOnly == false && v.LoanType.LengthDays > 0 && v.OnLoan == false && !currentLoans.Contains(v.VolumeID));
            var availableCopies = (from c in _db.Copies join v in availableVolumes on c.CopyID equals v.CopyID select c).Distinct();
            var availableTitles = (from t in _db.Titles join c in availableCopies on t.TitleID equals c.TitleID select t).Distinct();

            var volumes = availableVolumes
                .ToList()
                .Select(v => new
                {
                    v.VolumeID,
                    v.Barcode
                });

            var copies = availableCopies.OrderBy(c => c.CopyNumber)
                .ToList()
                .Select(c => new
                {
                    c.CopyID,
                    c.CopyNumber
                });

            var titles = availableTitles.OrderBy(t => t.Title1.Substring(t.NonFilingChars))
                .ToList()
                .Select(t => new
                {
                    t.TitleID,
                    Title = t.Title1
                });

            var viewModel = new NewLoanViewModel()
            {
                Borrowed = DateTime.Today,
                ReturnDue = DateTime.Today.AddDays(21),
                UserID = checkoutUserId,
                Borrower = fullName,
                UserName = userName,
                Volumes = new SelectList(volumes, "VolumeID", "Barcode"),
                Copies = new SelectList(copies, "CopyID", "CopyNumber"),
                Titles = new SelectList(titles, "TitleID", "Title"),
                Success = success,
                ShowCheckInOutItemHelper = Settings.GetParameterValue("Borrowing.CheckInOut.ShowItemHelper", "false", "Sets whether the Check-Out/In screens display a helper tool to find the item to borrow or return.", dataType: "bool") == "true",
                SelectBorrowerOption = Settings.GetParameterValue("Borrowing.SelectBorrowerMethod", "dropdownlist", "Sets how borrowers can identify themselves in the loans screens when challenged. Valid options are: 'dropdownlist', 'swipecard', or 'username'.", dataType: "text"),
                TimeOut = int.Parse(Settings.GetParameterValue("Borrowing.CheckInOut.PageTimeOut", "10000", "The period of time the stand-alone Check-Out and Check-In pages can be left idle before they time-out and clear borrower and loan details.", dataType: "int")),
                //SeeAlso = MenuHelper.SeeAlso("checkinoutSeeAlso", ControllerContext.RouteData.Values["action"].ToString(), null, "SortOrder")
            };

            var seeAlso = new List<Menu>();
            var menu = new Menu()
            {
                Action = "CheckIn",
                Controller = "Home",
                DataTarget = "",
                DataToggle = "",
                Title = "Check In"
            };
            seeAlso.Add(menu);
            viewModel.SeeAlso = seeAlso;

            viewModel.Users = _db.Users.Where(u => u.IsLive && u.CanDelete && u.Lastname != null && u.SelfLoansAllowed)
                        .Select(x => new SelectListItem
                        {
                            Value = x.Id.ToString(),
                            Text = x.Lastname + ", " + x.Firstname
                        }).OrderBy(c => c.Text)
                        .ToList();

            ViewData["SeeAlso"] = viewModel.SeeAlso;
            if (success)
            {
                TempData["SuccessMsg"] = "Item borrowed successfully. Borrow another?";
            }
            if (invalidUser)
            {
                TempData["ErrorMsg"] = "The " + DbRes.T("Users.Username", "FieldDisplayName") + " or " + DbRes.T("Users.Barcode", "FieldDisplayName") + " you entered cannot be found. Please check and try again.";
            }
            ViewBag.Title = DbRes.T("Borrowing.Check_Out", "FieldDisplayName");
            return View("CheckOut", "_CheckInOutLayout", viewModel);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult DoCheckOut(NewLoanViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var newLoan = new Borrowing
                {
                    VolumeID = _db.Volumes.Where(v => v.Barcode == viewModel.Barcode).Select(v => v.VolumeID).FirstOrDefault(),
                    BorrowerUser = _db.Users.Find(viewModel.UserID),
                    Borrowed = viewModel.Borrowed,
                    ReturnDue = viewModel.ReturnDue,
                    InputDate = DateTime.Now,
                    Renewal = false
                };

                _db.Borrowings.Add(newLoan);
                _db.SaveChanges();

                //Mark the volume as 'On Loan'
                var volume = _db.Volumes.Find(newLoan.VolumeID);
                volume.OnLoan = true;
                _db.Entry(volume).State = EntityState.Modified;
                _db.SaveChanges();

                return RedirectToAction("CheckOut", new { checkoutUserId = viewModel.UserID, success = true });
            }

            ViewData["SeeAlso"] = viewModel.SeeAlso;
            ViewBag.Title = DbRes.T("Borrowing.Check_Out", "FieldDisplayName");
            return View("CheckOut", "_CheckInOutLayout", viewModel);
        }

        //Check-In
        public ActionResult CheckIn(bool success = false)
        {
            //var userId = Utils.PublicFunctions.GetUserId();
            var currentLoans = _db.Borrowings.Where(b => b.Returned == null);

            var borrowedVolumes = from t in _db.Titles join c in _db.Copies on t.TitleID equals c.TitleID join v in _db.Volumes on c.CopyID equals v.CopyID join l in currentLoans on v.VolumeID equals l.VolumeID select new { barcode = v.Barcode, title = t.Title1 + " : Copy " + c.CopyNumber + " (" + v.Barcode + ") - " + l.BorrowerUser.Firstname + " " + l.BorrowerUser.Lastname };
            var volumes = borrowedVolumes
               .ToList()
               .Select(v => new
               {
                   v.barcode,
                   v.title
               });

            var viewModel = new ReturnLoanViewModel()
            {
                Volumes = new SelectList(volumes, "barcode", "title"),
                Success = success,
                ShowCheckInOutItemHelper = Settings.GetParameterValue("Borrowing.CheckInOut.ShowItemHelper", "false", "Sets whether the Check-Out/In screens display a helper tool to find the item to borrow or return.", dataType: "bool") == "true",
                TimeOut = int.Parse(Settings.GetParameterValue("Borrowing.CheckInOut.PageTimeOut", "6000", "The period of time the stand-alone Check-Out and Check-In pages can be left idle before they time-out and clear borrower and loan details.", dataType: "int")),
                //SeeAlso = MenuHelper.SeeAlso("checkinoutSeeAlso", ControllerContext.RouteData.Values["action"].ToString(), null, "SortOrder")
            };

            var seeAlso = new List<Menu>();
            var menu = new Menu()
            {
                Action = "CheckOut",
                Controller = "Home",
                DataTarget = "",
                DataToggle = "",
                Title = "Check Out"
            };
            seeAlso.Add(menu);
            viewModel.SeeAlso = seeAlso;
            
            if (success)
            {
                TempData["SuccessMsg"] = "Item returned successfully. Return another?";
            }

            if (!currentLoans.Any())
            {
                TempData["ErrorMsg"] = "There are currently no " + DbRes.T("Borrowing.Items_On_Loan", "FieldDisplayName").ToLower() + "!";
                TempData["InfoDialogMsg"] = "There are currently no " + DbRes.T("Borrowing.Items_On_Loan", "FieldDisplayName").ToLower() + "!";
            }

            ViewData["SeeAlso"] = viewModel.SeeAlso;
            ViewBag.Title = DbRes.T("Borrowing.Check_In", "FieldDisplayName");
            return View("CheckIn", "_CheckInOutLayout", viewModel);
        }

        //Do Check-In
        [HttpPost]
        public ActionResult DoCheckIn(ReturnLoanViewModel viewModel)
        {
            var volume = _db.Volumes.FirstOrDefault(v => v.Barcode == viewModel.Barcode);

            if (volume == null)
            {
                return HttpNotFound();
            }

            var currentLoans = _db.Borrowings.Where(b => b.Returned == null);
            var loan = currentLoans.FirstOrDefault(b => b.VolumeID == volume.VolumeID);

            if (loan == null)
            {
                return HttpNotFound();
            }

            try
            {
                loan.Returned = DateTime.Today;
                loan.LastModified = DateTime.Now;
                _db.Entry(loan).State = EntityState.Modified;
                _db.SaveChanges();
            }
            catch (Exception)
            {
                ViewData["SeeAlso"] = viewModel.SeeAlso;
                ViewBag.Title = DbRes.T("Borrowing.Check_In", "FieldDisplayName");
                return View("CheckIn", "_CheckInOutLayout", viewModel);
            }

            try
            {
                volume.OnLoan = false;
                volume.LastModified = DateTime.Now;
                _db.Entry(volume).State = EntityState.Modified;
                _db.SaveChanges();
            }
            catch (Exception)
            {
                ViewData["SeeAlso"] = viewModel.SeeAlso;
                ViewBag.Title = DbRes.T("Borrowing.Check_In", "FieldDisplayName");
                return View("CheckIn", "_CheckInOutLayout", viewModel);
            }

            return RedirectToAction("CheckIn", new { success = true });
        }
    }
}