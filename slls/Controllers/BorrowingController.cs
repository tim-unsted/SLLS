using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using slls.App_Settings;
using slls.Models;
using slls.Utils.Helpers;
using slls.ViewModels;
using Westwind.Globalization;

namespace slls.Controllers
{
    public class BorrowingController : sllsBaseController
    {
        private readonly DbEntities _db = new DbEntities();
        private ApplicationUserManager _userManager;

        private ApplicationUserManager UserManager
        {
            get { return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            set { _userManager = value; }
        }

        public BorrowingController()
        {
            ViewBag.Title = "Loans";
        }
        
        
        // GET: Loans/New
        public ActionResult NewLoan(string userId = "", bool success = false, bool invalidUser = false)
        {
            //Find the user record in th database: AspNetUsers ...
            if (string.IsNullOrEmpty(userId))
            {
                userId = Utils.PublicFunctions.GetUserId();
            }
            var fullName = "";
            var user = _db.Users.Find(userId);

            if (user != null)
            {
                fullName = string.Concat(new[] { user.Firstname, " ", user.Lastname });
            }
            
            var viewModel = new NewLoanViewModel()
            {
                Borrowed = DateTime.Today,
                ReturnDue = DateTime.Today.AddDays(21),
                UserID = userId,
                Borrower = fullName,
                Volumes = new SelectList(""),
                Copies = new SelectList(""),
                Titles = new SelectList(""),
                //SeeAlso = MenuHelper.SeeAlso("SelfLoansSeeAlso", ControllerContext.RouteData.Values["action"].ToString(), null, "SortOrder")
            };
            
            if (success)
            {
                TempData["SuccessMsg"] = "Item loaned successfully. Add another?";
            }
            if (invalidUser)
            {
                TempData["ErrorMsg"] = "The " + DbRes.T("Users.Username", "FieldDisplayName") + " or " + DbRes.T("Users.Barcode", "FieldDisplayName") + " you entered cannot be found. Please check and try again.";
            }
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("SelfLoansSeeAlso", ControllerContext.RouteData.Values["action"].ToString(), null, "SortOrder");
            ViewBag.Title = "Borrow an Item";
            return View(viewModel);
        }

        // POST: Loans/New
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult DoNewLoan(NewLoanViewModel viewModel)
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

                return RedirectToAction("NewLoan", new { userId = viewModel.UserID, success = true });
            }

            ViewData["SeeAlso"] = MenuHelper.SeeAlso("SelfLoansSeeAlso", ControllerContext.RouteData.Values["action"].ToString(), null, "SortOrder");
            ViewBag.Title = "Borrow an Item";
            return View("NewLoan", viewModel);
        }

        //Return Loan
        public ActionResult ReturnLoan(bool success = false)
        {
            var userId = Utils.PublicFunctions.GetUserId();
            var currentLoans = _db.Borrowings.Where(b => b.BorrowerUser.Id == userId && b.Returned == null).Select(b => b.VolumeID).Distinct();
            
            var borrowedVolumes = from t in _db.Titles join c in _db.Copies on t.TitleID equals c.TitleID join v in _db.Volumes on c.CopyID equals v.CopyID where currentLoans.Contains(v.VolumeID) select new {barcode = v.Barcode, title = t.Title1};
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
                //SeeAlso = MenuHelper.SeeAlso("SelfLoansSeeAlso", ControllerContext.RouteData.Values["action"].ToString(), null, "SortOrder")
            };

            if (success)
            {
                if (currentLoans.Any())
                {
                    TempData["SuccessMsg"] = "Item returned successfully. Return another?";
                }
                else
                {
                    TempData["SuccessMsg"] = "Item returned successfully. You have no more " + DbRes.T("Borrowing.Items_On_Loan", "FieldDisplayName").ToLower() + " to return.";
                }

            }

            if (!currentLoans.Any() && !success)
            {
                TempData["ErrorMsg"] = "You don't currently have any " + DbRes.T("Borrowing.Items_On_Loan", "FieldDisplayName").ToLower() + "!";
                TempData["InfoDialogMsg"] = "You don't currently have any " + DbRes.T("Borrowing.Items_On_Loan", "FieldDisplayName").ToLower() + "!";
            }

            ViewData["SeeAlso"] = MenuHelper.SeeAlso("SelfLoansSeeAlso", ControllerContext.RouteData.Values["action"].ToString(), null, "SortOrder");
            ViewBag.Title = "Return an Item";
            return View(viewModel);
        }
        
        public ActionResult ConfirmReturnLoan(int id = 0)
        {
            var loan = _db.Borrowings.Find(id);
            if (loan == null)
            {
                return HttpNotFound();
            }
            var volume = loan.Volume;
            var copy = volume.Copy;
            var title = copy.Title.Title1;
            DateTime borrowed = loan.Borrowed.Value;
            DateTime returnDue = loan.ReturnDue.Value;
            
            var viewModel = new ConfirmNewLoanRenewReturnViewModel
            {
                PostConfirmController = "Borrowing",
                PostConfirmAction = "DoQuickReturn",
                ConfirmationText = "<p>Do you want to continue?</p>",
                DetailsText = "<p>You are about to return the following loan: </p>",
                ConfirmButtonText = "Continue",
                ConfirmButtonClass = "btn-success",
                CancelButtonText = "Cancel",
                HeaderText = "Return Loan?",
                Glyphicon = "glyphicon-ok",
                BorrowID = id,
                Title = copy.Title.Title1,
                Borrower = loan.BorrowerUser.Fullname,
                Borrowed = borrowed.ToShortDateString(),
                ReturnDue = returnDue.ToShortDateString()
            };
            return PartialView("ConfirmRenewReturn", viewModel);
        }

        public ActionResult ConfirmNewLoan(int volumeId = 0, string userId = "")
        {

            var volume = _db.Volumes.Find(volumeId);
            if (volume == null)
            {
                return HttpNotFound();
            }
            var borrowerUser = _db.Users.Find(userId);
            if (borrowerUser == null)
            {
                return HttpNotFound();
            }

            var copy = volume.Copy;
            var title = copy.Title.Title1;
            DateTime borrowed = DateTime.Today;
            DateTime returnDue = DateTime.Today.AddDays(volume.LoanType.LengthDays);

            var viewModel = new ConfirmNewLoanRenewReturnViewModel
            {
                PostConfirmController = "Borrowing",
                PostConfirmAction = "DoQuickLoan",
                ConfirmationText = "<p>Do you want to continue?</p>",
                DetailsText = "<p>You are about to borrow the following item:</p>",
                ConfirmButtonText = "Continue",
                ConfirmButtonClass = "btn-success",
                CancelButtonText = "Cancel",
                HeaderText = "Borrow Item?",
                Glyphicon = "glyphicon-ok",
                VolumeID = volumeId,
                Title = copy.Title.Title1,
                Borrower = borrowerUser.Fullname,
                BorrowerId = borrowerUser.Id,
                Borrowed = borrowed.ToShortDateString(),
                ReturnDue = returnDue.ToShortDateString()
            };
            return PartialView("ConfirmRenewReturn", viewModel);
        }
        
        public ActionResult ConfirmRenewLoan(int id = 0)
        {
            var loan = _db.Borrowings.Find(id);
            if (loan == null)
            {
                return HttpNotFound();
            }
            var volume = loan.Volume;
            var copy = volume.Copy;
            var title = copy.Title.Title1;
            DateTime borrowed = DateTime.Today;
            DateTime returnDue = DateTime.Today.AddDays(volume.LoanType.LengthDays);
            
            var viewModel = new ConfirmNewLoanRenewReturnViewModel
            {
                PostConfirmController = "Borrowing",
                PostConfirmAction = "DoQuickRenew",
                ConfirmationText = "<p>Do you want to continue?</p>",
                DetailsText = "<p>You are about to renew the following loan:</p>",
                ConfirmButtonText = "Continue",
                ConfirmButtonClass = "btn-success",
                CancelButtonText = "Cancel",
                HeaderText = "Renew Loan?",
                Glyphicon = "glyphicon-ok",
                BorrowID = id,
                Title = copy.Title.Title1,
                Borrower = loan.BorrowerUser.Fullname,
                Borrowed = borrowed.ToShortDateString(),
                ReturnDue = returnDue.ToShortDateString()
            };
            return PartialView("ConfirmRenewReturn", viewModel);
        }
        
        [HttpPost]
        public ActionResult DoQuickLoan(ConfirmNewLoanRenewReturnViewModel viewModel)
        {
            var volume = _db.Volumes.Find(viewModel.VolumeID);
            if(volume == null)
            {
                return Json(new { success = false });
            }
            var borrowerUser = _db.Users.Find(viewModel.BorrowerId);
            if (borrowerUser == null)
            {
                return Json(new { success = false });
            }

            //Add a new loan ...
            var daysToAdd = volume.LoanType.LengthDays == 0 ? 21 : volume.LoanType.LengthDays;

            try
            {
                var newLoan = new Borrowing
                {
                    VolumeID = viewModel.VolumeID,
                    BorrowerUser = borrowerUser,
                    Borrowed = DateTime.Today,
                    ReturnDue = DateTime.Today.AddDays(daysToAdd),
                    InputDate = DateTime.Now,
                    Renewal = true
                };
                _db.Borrowings.Add(newLoan);
                _db.SaveChanges();
            }
            catch (Exception)
            {
                return Json(new { success = false });
            }

            //Mark the volume as 'On Loan'
            if (volume == null) return Json(new { success = true });
            try
            {
                volume.OnLoan = true;
                _db.Entry(volume).State = EntityState.Modified;
                _db.SaveChanges();
            }
            catch (Exception)
            {
                return Json(new { success = false });
            }

            return Json(new { success = true });
        }

        //Return Loan
        [HttpPost]
        public ActionResult DoReturnLoan(ReturnLoanViewModel viewModel)
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
                //ViewData["SeeAlso"] = viewModel.SeeAlso;
                MenuHelper.SeeAlso("SelfLoansSeeAlso", ControllerContext.RouteData.Values["action"].ToString(), null,
                    "SortOrder");
                ViewBag.Title = "Return Item";
                return View("ReturnLoan", viewModel);
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
                //ViewData["SeeAlso"] = viewModel.SeeAlso;
                MenuHelper.SeeAlso("SelfLoansSeeAlso", ControllerContext.RouteData.Values["action"].ToString(), null,
                    "SortOrder");
                ViewBag.Title = "Return Item";
                return View("ReturnLoan", viewModel);
            }
            
            return RedirectToAction("ReturnLoan", new {success = true});
        }

        public ActionResult DoQuickReturn(ConfirmNewLoanRenewReturnViewModel viewModel)
        {
            var loan = _db.Borrowings.Find(viewModel.BorrowID);
            if (loan == null)
            {
                return Json(new { success = false });
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
                return Json(new { success = false });
            }

            var volume = _db.Volumes.Find(loan.VolumeID);
            if (volume == null)
            {
                return Json(new { success = false });
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
                return Json(new { success = false });
            }
            return Json(new { success = true });
        }

        //Renew Loan
        public ActionResult RenewLoan(bool success = false)
        {
            var userId = Utils.PublicFunctions.GetUserId();
            var currentLoans = _db.Borrowings.Where(b => b.BorrowerUser.Id == userId && b.Returned == null).Select(b => b.VolumeID).Distinct();
            var borrowedVolumes = from t in _db.Titles join c in _db.Copies on t.TitleID equals c.TitleID join v in _db.Volumes on c.CopyID equals v.CopyID where currentLoans.Contains(v.VolumeID) select new { barcode = v.Barcode, title = t.Title1 };
            var volumes = borrowedVolumes
               .ToList()
               .Select(v => new
               {
                   v.barcode,
                   v.title
               });

            var viewModel = new RenewLoanViewModel()
            {
                Volumes = new SelectList(volumes, "barcode", "title"),
                ReturnDue = DateTime.Today.AddDays(21),
                //SeeAlso = MenuHelper.SeeAlso("SelfLoansSeeAlso", ControllerContext.RouteData.Values["action"].ToString(), null, "SortOrder")
            };

            if (success)
            {
                if (currentLoans.Any())
                {
                    TempData["SuccessMsg"] = "Item renewed successfully. Renew another?";
                }
                else
                {
                    TempData["SuccessMsg"] = "Item renewed successfully. You have no more " + DbRes.T("Borrowing.Items_On_Loan", "FieldDisplayName").ToLower() + " to renew."; 
                }

            }

            if (!currentLoans.Any() && !success)
            {
                TempData["ErrorMsg"] = "You don't currently have any " + DbRes.T("Borrowing.Items_On_Loan", "FieldDisplayName").ToLower() + "!";
                TempData["InfoDialogMsg"] = "You don't currently have any " + DbRes.T("Borrowing.Items_On_Loan", "FieldDisplayName").ToLower() + "!";
            }

            ViewData["SeeAlso"] = MenuHelper.SeeAlso("SelfLoansSeeAlso",
                ControllerContext.RouteData.Values["action"].ToString(), null, "SortOrder");
            ViewBag.Title = "Renew an Item";
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult DoRenewLoan(RenewLoanViewModel viewModel)
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

            //First, we'll return the existing loan before issuing a new loan
            try
            {
                loan.Returned = DateTime.Today;
                loan.LastModified = DateTime.Now;
                _db.Entry(loan).State = EntityState.Modified;
                _db.SaveChanges();
            }
            catch (Exception)
            {
                ViewData["SeeAlso"] = MenuHelper.SeeAlso("SelfLoansSeeAlso", ControllerContext.RouteData.Values["action"].ToString(), null, "SortOrder");
                ViewBag.Title = "Renew Loan";
                return View("RenewLoan", viewModel);
            }

            try
            {
                //Calculate actual return date based on lantype ...
                var daysToAdd = volume.LoanType.LengthDays;
                if (daysToAdd == 0) daysToAdd = 28;

                var newLoan = new Borrowing
                {
                    VolumeID = volume.VolumeID,
                    BorrowerUser = loan.BorrowerUser,
                    Borrowed = DateTime.Today,
                    ReturnDue = DateTime.Today.AddDays(daysToAdd),
                    InputDate = DateTime.Now,
                    Renewal = true
                };

                _db.Borrowings.Add(newLoan);
                _db.SaveChanges();
                
            }
            catch (Exception)
            {
                ViewData["SeeAlso"] = MenuHelper.SeeAlso("SelfLoansSeeAlso", ControllerContext.RouteData.Values["action"].ToString(), null, "SortOrder");
                ViewBag.Title = "Renew Loan";
                return View("RenewLoan", viewModel);
            }

            try
            {
                //Mark the volume as 'On Loan'
                volume.OnLoan = true;
                _db.Entry(volume).State = EntityState.Modified;
                _db.SaveChanges();
            }
            catch (Exception)
            {
                //throw;
            }
            
            return RedirectToAction("RenewLoan", new { success = true });
        }

        [HttpPost]
        public ActionResult DoQuickRenew(ConfirmNewLoanRenewReturnViewModel viewModel)
        {
            var loan = _db.Borrowings.Find(viewModel.BorrowID);
            if (loan == null)
            {
                return HttpNotFound();
            }
            var volume = _db.Volumes.Find(loan.VolumeID);

            //First, we'll return the existing loan before issuing a new loan
            try
            {
                loan.Returned = DateTime.Today;
                loan.LastModified = DateTime.Now;
                _db.Entry(loan).State = EntityState.Modified;
                _db.SaveChanges();
            }
            catch (Exception)
            {
                return Json(new { success = false });
            }

            //Then create a new loan ...
            var daysToAdd = volume.LoanType.LengthDays == 0 ? 21 : volume.LoanType.LengthDays;

            try
            {
                var newLoan = new Borrowing
                {
                    VolumeID = loan.VolumeID,
                    BorrowerUser = loan.BorrowerUser,
                    Borrowed = DateTime.Today,
                    ReturnDue = DateTime.Today.AddDays(daysToAdd),
                    InputDate = DateTime.Now,
                    Renewal = true
                };
                _db.Borrowings.Add(newLoan);
                _db.SaveChanges();
            }
            catch (Exception)
            {
                return Json(new { success = false });
            }

            //Mark the volume as 'On Loan'
            if (volume == null) return Json(new {success = true});
            try
            {
                volume.OnLoan = true;
                _db.Entry(volume).State = EntityState.Modified;
                _db.SaveChanges();
            }
            catch (Exception)
            {
                return Json(new { success = false });
            }

            return Json(new { success = true });
        }

        //Get a list of available titles (Ajax stuff) ...
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult GetAvailableTitles(string term)
        {
            term = " " + term;
            var titles = (from t in _db.VwSelectTitlesToBorrow
                          where t.Title.Contains(term)
                          orderby t.Title.Substring(t.NonFilingChars)
                          select new { Title = t.Title, TitleId = t.TitleId, Year = t.Year, Edition = t.Edition, Authors = t.AuthorString }).Take(250);

            return Json(titles, JsonRequestBehavior.AllowGet);
        }

        //Get a list of titles on-loan (Ajax stuff) ...
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult GetBorrowedTitles(string term)
        {
            term = " " + term;
            var titles = (from t in _db.VwSelectTitlesToRenewReturn
                          where t.Title.Contains(term)
                          orderby t.Title.Substring(t.NonFilingChars)
                          select new { Title = t.Title, TitleId = t.TitleId, Year = t.Year, Edition = t.Edition, Authors = t.AuthorString }).Take(250);

            return Json(titles, JsonRequestBehavior.AllowGet);
        }

        //Method used to supply a JSON list of Copies when selecting a Title (Ajax stuf)
        public JsonResult GetAvailableCopies(int titleId = 0)
        {
            var availableCopies = (from c in _db.Copies
                join v in _db.Volumes on c.CopyID equals v.CopyID
                where c.TitleID == titleId && v.OnLoan == false && v.LoanType.RefOnly == false && v.LoanType.LengthDays > 0
                select c).Distinct();

            var copies = new SelectList(availableCopies.ToList(), "CopyID", "CopyNumber");
            
            return Json(new
            {
                success = true,
                AvailableCopies = copies
            });
        }

        //Method used to supply a JSON list of Volumes when selecting a Copy (Ajax stuf)
        public JsonResult GetAvailableVolumes(int copyId = 0)
        {
            var availableVolumes = from v in _db.Volumes where v.CopyID == copyId
                                  where v.OnLoan == false && v.LoanType.RefOnly == false && v.LoanType.LengthDays > 0
                                  select v;

            var volumes = new SelectList(availableVolumes.ToList(), "VolumeID", "Barcode");

            return Json(new
            {
                success = true,
                AvailableVolumes = volumes
            });
        }

        [HttpPost]
        public JsonResult VolumeAvailable(int volumeId)
        {
            var volume = _db.Volumes.Find(volumeId);
            return Json(volume != null);
        }

        //Method used to supply a JSON list of Copies on loan when selecting a Title (Ajax stuf)
        public JsonResult GetBorrowedCopies(int titleId = 0)
        {
            var borrowedCopies = (from c in _db.Copies
                                   join v in _db.Volumes on c.CopyID equals v.CopyID
                                   where c.TitleID == titleId && v.OnLoan
                                   select c).Distinct();

            var copies = new SelectList(borrowedCopies.ToList(), "CopyID", "CopyNumber");

            return Json(new
            {
                success = true,
                BorrowedCopies = copies
            });
        }

        //Method used to supply a JSON list of Volumes on loan when selecting a Copy (Ajax stuf)
        public JsonResult GetBorrowedVolumes(int copyId = 0)
        {
            var borrowedVolumes = from v in _db.Volumes
                                   where v.CopyID == copyId
                                   where v.OnLoan
                                   select v;

            var volumes = new SelectList(borrowedVolumes.ToList(), "VolumeID", "Barcode");

            return Json(new
            {
                success = true,
                BorrowedVolumes = volumes
            });
        }


        //Return a JSON object containing details for the selected or entered barcode ...
        public JsonResult GetBarcodeDetails(string barcode = "")
        {
            barcode = barcode.Trim();
            var volume = _db.Volumes.FirstOrDefault(v => v.Barcode == barcode);
            if (volume == null)
            {
                return Json(new
                {
                    success = false,
                    BarcodeDetails = "The item you have entered is not available for lending or does not exist."
                });
            }
            if (volume.OnLoan)
            {
                return Json(new
                {
                    success = false,
                    BarcodeDetails = "The item you have entered is currently on-loan to another borrower."
                });
            }
            if (volume.LoanType.RefOnly)
            {
                return Json(new
                {
                    success = false,
                    BarcodeDetails = "The item you have entered is not ''Reference-Only' and is not available for lending."
                });
            }
            if (volume.LoanType.LengthDays == 0)
            {
                return Json(new
                {
                    success = false,
                    BarcodeDetails = "The item you have entered is not available for lending because its loan type length (days) is zero."
                });
            }

            var title = volume.Copy.Title;
            var copy = volume.Copy;
            return Json(new
            {
                success = true,
                //BarcodeDetails = title + " - Copy " + copy,
                BarcodeDetails = title.Title1 + " - Copy " + copy.CopyNumber,
                Author = title.AuthorString,
                Edition = title.Edition,
                ReturnDue = DateTime.Today.AddDays(volume.LoanType.LengthDays).ToShortDateString()
            });
        }

        //Return a JSON object containing details for the selected selected or entered barcode ...
        public JsonResult GetBorrowedItemDetails(string barcode = "")
        {
            barcode = barcode.Trim();
            var volume = _db.Volumes.FirstOrDefault(v => v.Barcode == barcode);
            if (volume == null)
            {
                return Json(new
                {
                    success = false,
                    BarcodeDetails = "The item you have entered does not exist. Please check and try again."
                });
            }
            if (volume.OnLoan == false)
            {
                return Json(new
                {
                    success = false,
                    BarcodeDetails = "The item you have entered is not currently on-loan."
                });
            }

            var title = volume.Copy.Title.Title1;
            var copy = volume.Copy.CopyNumber;
            DateTime? origReturnDue = DateTime.Today;
            var newReturnDue = DateTime.Today.AddDays(21);
            var borrowedBy = "";
            var borrowing = _db.Borrowings.FirstOrDefault(b => b.VolumeID == volume.VolumeID && b.Returned == null);
            if (borrowing != null)
            {
                origReturnDue = borrowing.ReturnDue;
                borrowedBy = borrowing.BorrowerUser.Fullname;
            }

            if (volume.LoanType.LengthDays > 0)
            {
                newReturnDue = DateTime.Today.AddDays(volume.LoanType.LengthDays);  //.Date.ToString("dd/MM/yyyy")
            }

            return Json(new
            {
                success = true,
                Barcode = barcode,
                BarcodeDetails = title + " - Copy " + copy,
                BorrowedBy = borrowedBy,
                origReturnDue = origReturnDue == null ? "" : origReturnDue.Value.ToShortDateString(),
                newReturnDue = newReturnDue.ToShortDateString()
            });
        }

        //Method used to supply JSON data for a user (Ajax stuff)
        public JsonResult GetBorrowerDetails(string userIdNameSwipe = "")
        {
            ApplicationUser borrower = new ApplicationUser();
            if (!string.IsNullOrEmpty(userIdNameSwipe))
            {
                borrower = _db.Users.FirstOrDefault(u => u.Id == userIdNameSwipe || u.UserName == userIdNameSwipe || u.UserBarcode == userIdNameSwipe);
            }
            
            if (borrower == null)
            {
                return Json(new
                {
                    Fullname = "The username/barcode that you've entered is invalid. Please check and try again.",
                    success = false
                });
            }

            return Json(new
            {
                success = true,
                UserId = borrower.Id,
                Fullname = borrower.Fullname
            });
        }
        
        [HttpPost]
        public JsonResult BarcodeExists(string barcode)
        {
            barcode = barcode.Trim();
            var volume = _db.Volumes.FirstOrDefault(v => v.Barcode == barcode);
            return Json(volume != null);
        }

        [HttpPost]
        public JsonResult UserNameExists(string username)
        {
            username = username.Trim();
            var user = _db.Users.FirstOrDefault(u => u.UserName == username);
            return Json(user != null);
        }

        [HttpPost]
        public JsonResult UserSwipeExists(string barcode)
        {
            barcode = barcode.Trim();
            var user = _db.Users.FirstOrDefault(u => u.UserBarcode == barcode);
            return Json(user != null);
        }

        public ActionResult SelectBorrower()
        {
            var selectBorrowerOption = Settings.GetParameterValue("Borrowing.SelectBorrowerMethod", "dropdownlist",
                "Sets how borrowers can identify themselves in the loans screens when challenged. Valid options are: 'dropdownlist', 'swipecard', or 'username'.", dataType: "text");
            //var viewModel = new SelectPopupViewModel();

            switch (selectBorrowerOption)
            {
                case "dropdownlist":
                {
                    var viewModel = new SelectPopupViewModel
                    {
                        PostSelectController = "Borrowing",
                        PostSelectAction = "PostSelectBorrower",
                        SelectedItem = "0",
                        HeaderText = "Select a " + DbRes.T("Borrowing.Borrower", "FieldDisplayName"),
                        DetailsText = "Select a different " + DbRes.T("Borrowing.Borrower", "FieldDisplayName") + " from the drop-down list below:",
                        SelectLabel = "",
                        OkButtonText = "Select " + DbRes.T("Borrowing.Borrower", "FieldDisplayName")
                    };

                    viewModel.AvailableItems = _db.Users.Where(u => u.IsLive && u.CanDelete && u.Lastname != null && u.SelfLoansAllowed)
                        .Select(x => new SelectListItem
                        {
                            Value = x.Id.ToString(),
                            Text = x.Lastname + ", " + x.Firstname
                        }).OrderBy(c => c.Text)
                        .ToList();

                    ViewBag.Title = "Change " + DbRes.T("Borrowing.Borrower", "FieldDisplayName");
                    return PartialView("_SelectPopup", viewModel);
                }
                case "swipecard":
                {
                    var viewModel = new EnterTextValuePopupViewModel()
                    {
                        DetailsText = "To change the " + DbRes.T("Borrowing.Borrower", "FieldDisplayName") + ", enter or swipe a valid " + DbRes.T("Users.Barcode", "FieldDisplayName") + " in the box below.",
                        PostSelectAction = "PostEnterUserSwipe"
                    };
                    ViewBag.Title = "Change " + DbRes.T("Borrowing.Borrower", "FieldDisplayName");
                    return PartialView("_SwipeBorrower ", viewModel);
                }
                case "username":
                {
                    var viewModel = new EnterTextValuePopupViewModel()
                    {
                        DetailsText = "To change the " + DbRes.T("Borrowing.Borrower", "FieldDisplayName") + ", enter a valid " + DbRes.T("Users.Username", "FieldDisplayName") + " in the box below.",
                        PostSelectAction = "PostEnterUsername"
                    };
                    ViewBag.Title = "Change " + DbRes.T("Borrowing.Borrower", "FieldDisplayName");
                    return PartialView("_EnterBorrower", viewModel);
                }
                default:
                {
                    //Use username option as this is probably the most robust ...
                    var viewModel = new EnterTextValuePopupViewModel()
                    {
                        DetailsText = "To change the " + DbRes.T("Borrowing.Borrower", "FieldDisplayName") + ", enter a valid " + DbRes.T("Users.Username", "FieldDisplayName") + " in the box below.",
                        PostSelectAction = "PostEnterUsername"
                    };
                    return PartialView("_EnterBorrower", viewModel);
                }
            }
        }

        [HttpPost]
        public ActionResult PostSelectBorrower(SelectPopupViewModel viewModel)
        {
            var user = _db.Users.Find(viewModel.SelectedItem);
            if (user != null)
            {
                UrlHelper urlHelper = new UrlHelper(HttpContext.Request.RequestContext);
                string actionUrl = urlHelper.Action("NewLoan", "Borrowing", new {userId = user.Id});
                return Json(new { success = true, redirectTo = actionUrl});
            }
            RedirectToAction("NewLoan");
            return Json(new { success = true });
        }

        [HttpPost]
        public ActionResult PostEnterUsername(EnterTextValuePopupViewModel viewModel)
        {
            var user = _db.Users.FirstOrDefault(u => u.UserName == viewModel.UserName);
            UrlHelper urlHelper;
            string actionUrl = "";

            if (user != null)
            {
                urlHelper = new UrlHelper(HttpContext.Request.RequestContext);
                actionUrl = urlHelper.Action("NewLoan", "Borrowing", new { userId = user.Id });
                return Json(new { success = true, redirectTo = actionUrl });
            }

            urlHelper = new UrlHelper(HttpContext.Request.RequestContext);
            actionUrl = urlHelper.Action("NewLoan", "Borrowing", new { invalidUser = true });
            return Json(new { success = false, redirectTo = actionUrl });
        }

        [HttpPost]
        public ActionResult PostEnterUserSwipe(EnterTextValuePopupViewModel viewModel)
        {
            var user = _db.Users.FirstOrDefault(u => u.UserBarcode == viewModel.UserSwipe);
            UrlHelper urlHelper;
            string actionUrl = "";

            if (user != null)
            {
                urlHelper = new UrlHelper(HttpContext.Request.RequestContext);
                actionUrl = urlHelper.Action("NewLoan", "Borrowing", new { userId = user.Id });
                return Json(new { success = true, redirectTo = actionUrl });
            }

            urlHelper = new UrlHelper(HttpContext.Request.RequestContext);
            actionUrl = urlHelper.Action("NewLoan", "Borrowing", new { invalidUser = true });
            return Json(new { success = false, redirectTo = actionUrl });
        }

        [HttpGet]
        public ActionResult Delete(int id = 0)
        {
            var loan = _db.Borrowings.Find(id);
            if (loan == null)
            {
                return HttpNotFound();
            }
            if (loan.Deleted)
            {
                return HttpNotFound();
            }
            var volume = loan.Volume;
            var copy = volume.Copy;
            var title = copy.Title.Title1;

            // Create new instance of DeleteConfirmationViewModel and pass it to _DeleteConfirmation Dialog (Reusable)
            DeleteConfirmationViewModel deleteConfirmationViewModel = new DeleteConfirmationViewModel
            {
                DeleteEntityId = id,
                HeaderText = "Loan",
                PostDeleteAction = "Delete",
                PostDeleteController = "Loans",
                DetailsText = "Title: " + title + " - Copy: " + copy.CopyNumber + "<br>Borrowed By: " + loan.BorrowerUser.Fullname 
            };
            return PartialView("_DeleteConfirmation", deleteConfirmationViewModel);
        }

        [HttpPost]
        public ActionResult Delete(DeleteConfirmationViewModel dcvm)
        {
            // Get item which we need to delete from EntityFramework
            var loan = _db.Borrowings.Find(dcvm.DeleteEntityId);

            if (loan == null)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _db.Borrowings.Remove(loan);
                    _db.SaveChanges();
                    return Json(new { success = true });
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", e.Message);
                    return Json(new { success = false });
                }
            }
            return Json(new { success = false });
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