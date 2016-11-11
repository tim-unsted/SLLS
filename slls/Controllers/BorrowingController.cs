using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
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
        public ActionResult NewLoan(string userId = "", bool success = false)
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
                UserID = userId,
                Borrower = fullName,
                Volumes = new SelectList(volumes, "VolumeID", "Barcode"),
                Copies = new SelectList(copies, "CopyID", "CopyNumber"),
                Titles = new SelectList(titles, "TitleID", "Title"),
                SeeAlso = MenuHelper.SeeAlso("SelfLoansSeeAlso", ControllerContext.RouteData.Values["action"].ToString(), null, "SortOrder")
            };

            ViewData["SeeAlso"] = viewModel.SeeAlso; //MenuHelper.SeeAlso("SelfLoansSeeAlso", ControllerContext.RouteData.Values["action"].ToString(), null, "SortOrder");
            if (success)
            {
                TempData["SuccessMsg"] = "Item loaned successfully. Add another?";
            }
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

            ViewData["SeeAlso"] = viewModel.SeeAlso;
            ViewBag.Title = "Borrow an Item";
            return View("NewLoan", viewModel);
        }

        //Return Loan
        public ActionResult ReturnLoan(bool success = false)
        {
            var userId = Utils.PublicFunctions.GetUserId();
            var currentLoans = _db.Borrowings.Where(b => b.BorrowerUser.Id == userId && b.Returned == null).Select(b => b.VolumeID).Distinct();
            
            var borrowedVolumes = from t in _db.Titles join c in _db.Copies on t.TitleID equals c.TitleID join v in _db.Volumes on c.CopyID equals v.CopyID where currentLoans.Contains(v.VolumeID) select new {volumeId = v.VolumeID, title = t.Title1};
            var volumes = borrowedVolumes
               .ToList()
               .Select(v => new
               {
                   v.volumeId,
                   v.title
               });
            
            var viewModel = new ReturnLoanViewModel()
            {
                Volumes = new SelectList(volumes, "volumeId", "title"),
                SeeAlso = MenuHelper.SeeAlso("SelfLoansSeeAlso", ControllerContext.RouteData.Values["action"].ToString(), null, "SortOrder")
            };

            if (success)
            {
                if (currentLoans.Any())
                {
                    TempData["SuccessMsg"] = "Item renewed successfully. Renew another?";
                }
                else
                {
                    TempData["SuccessMsg"] = "Item renewed successfully. You have no more " + DbRes.T("Borrowing.Items_On_Loan", "FieldDisplayName").ToLower() + ".";
                }

            }

            if (!currentLoans.Any() && !success)
            {
                TempData["ErrorMsg"] = "You don't currently have any " + DbRes.T("Borrowing.Items_On_Loan", "FieldDisplayName").ToLower() + "!";
                TempData["InfoDialogMsg"] = "You don't currently have any " + DbRes.T("Borrowing.Items_On_Loan", "FieldDisplayName").ToLower() + "!";
            }

            ViewData["SeeAlso"] = viewModel.SeeAlso;
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
                ViewData["SeeAlso"] = viewModel.SeeAlso;
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
                ViewData["SeeAlso"] = viewModel.SeeAlso;
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
            var borrowedVolumes = from t in _db.Titles join c in _db.Copies on t.TitleID equals c.TitleID join v in _db.Volumes on c.CopyID equals v.CopyID where currentLoans.Contains(v.VolumeID) select new { volumeId = v.VolumeID, title = t.Title1 };
            var volumes = borrowedVolumes
               .ToList()
               .Select(v => new
               {
                   v.volumeId,
                   v.title
               });

            var viewModel = new RenewLoanViewModel()
            {
                Volumes = new SelectList(volumes, "volumeId", "title"),
                ReturnDue = DateTime.Today.AddDays(21),
                SeeAlso = MenuHelper.SeeAlso("SelfLoansSeeAlso", ControllerContext.RouteData.Values["action"].ToString(), null, "SortOrder")
            };

            if (success)
            {
                if (currentLoans.Any())
                {
                    TempData["SuccessMsg"] = "Item renewed successfully. Renew another?";
                }
                else
                {
                    TempData["SuccessMsg"] = "Item renewed successfully. You have no more " + DbRes.T("Borrowing.Items_On_Loan", "FieldDisplayName").ToLower() + "."; 
                }

            }

            if (!currentLoans.Any() && !success)
            {
                TempData["ErrorMsg"] = "You don't currently have any " + DbRes.T("Borrowing.Items_On_Loan", "FieldDisplayName").ToLower() + "!";
                TempData["InfoDialogMsg"] = "You don't currently have any " + DbRes.T("Borrowing.Items_On_Loan", "FieldDisplayName").ToLower() + "!";
            }

            ViewData["SeeAlso"] = viewModel.SeeAlso;
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
                ViewData["SeeAlso"] = viewModel.SeeAlso;
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
                ViewData["SeeAlso"] = viewModel.SeeAlso;
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

            var title = volume.Copy.Title.Title1;
            var copy = volume.Copy.CopyNumber;
            return Json(new
            {
                success = true,
                BarcodeDetails = title + " - Copy " + copy,
                ReturnDue = DateTime.Today.AddDays(volume.LoanType.LengthDays).ToShortDateString()
            });
        }

        //Return a JSON object containing details for the selected volumeId ...
        public JsonResult GetBorrowedItemDetails(int volumeId = 0)
        {
            //barcode = barcode.Trim();
            var volume = _db.Volumes.Find(volumeId);
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

            var barcode = volume.Barcode;
            var title = volume.Copy.Title.Title1;
            var copy = volume.Copy.CopyNumber;
            var origReturnDue = DateTime.Today;
            var newReturnDue = DateTime.Today.AddDays(21);
            var borrowedBy = "";
            var borrowing = _db.Borrowings.FirstOrDefault(b => b.VolumeID == volume.VolumeID && b.Returned == null);
            if (borrowing != null)
            {
                origReturnDue = borrowing.ReturnDue.Value;
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
                origReturnDue = origReturnDue.ToShortDateString(),
                newReturnDue = newReturnDue.ToShortDateString()
            });

        }
        
        [HttpPost]
        public JsonResult BarcodeExists(string barcode)
        {
            barcode = barcode.Trim();
            var volume = _db.Volumes.FirstOrDefault(v => v.Barcode == barcode);
            return Json(volume != null);
        }

        public ActionResult SelectBorrower()
        {
            var viewModel = new SelectPopupViewModel
            {
                PostSelectController = "Borrowing",
                PostSelectAction = "PostSelectBorrower",
                SelectedItem = "0",
                HeaderText = "Select a Borrower",
                DetailsText = "Choose a different borrower from the drop-down list below:",
                SelectLabel = "",
                OkButtonText = "Select Borrower"
            };

            viewModel.AvailableItems = _db.Users.Where(u => u.IsLive && u.CanDelete && u.Lastname != null)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Lastname + ", " + x.Firstname
                }).OrderBy(c => c.Text)
                .ToList();

            ViewBag.Title = "Select a Borrower";
            return PartialView("_SelectPopup", viewModel);
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