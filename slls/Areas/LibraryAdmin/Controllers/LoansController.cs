using System;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using slls.Models;
using slls.Utils.Helpers;
using slls.ViewModels;
using Westwind.Globalization;

namespace slls.Areas.LibraryAdmin
{
    public class LoansController : AdminBaseController
    {
        private readonly DbEntities _db = new DbEntities();
        private ApplicationUserManager _userManager;

        private ApplicationUserManager UserManager
        {
            get { return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            set { _userManager = value; }
        }

        public LoansController()
        {
            ViewBag.Title = "Loans";
        }


        // All loand (Historical)
        public ActionResult Index()
        {
            var allloans = _db.Borrowings;
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("BorrowingSeeAlso", ControllerContext.RouteData.Values["action"].ToString(), null, "SortOrder");
            ViewBag.Title = "All Loans (Historical)";
            return View(allloans.ToList());
        }

        // Current loans (i.e. Returned IS NULL)
        public ActionResult ItemsOnLoan()
        {
            var borrowings = _db.Borrowings.Where(b => b.Returned == null);
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("BorrowingSeeAlso", ControllerContext.RouteData.Values["action"].ToString(), null, "SortOrder");
            ViewBag.Title = "Items Currently On Loan";
            return View(borrowings.ToList());
        }

        // Overdue loand (i.e. Returned IS NULL and ReturnDue has passed ...
        public ActionResult OverdueLoans()
        {
            var borrowings = _db.Borrowings.Where(b => b.Returned == null && b.ReturnDue < DateTime.Today);
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("BorrowingSeeAlso", ControllerContext.RouteData.Values["action"].ToString(), null, "SortOrder");
            if (!borrowings.Any())
            {
                TempData["NoData"] = "There are currently no overdue loans!";
            }
            ViewBag.Title = "Overdue Loans";
            return View(borrowings.ToList());
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var loanDetails = _db.Borrowings.Find(id);
            if (loanDetails == null)
            {
                return HttpNotFound();
            }
            if (loanDetails.Borrowed != null)
            {
                var viewModel = new EditLoanViewModel()
                {
                    BorrowID = id.Value,

                    Title = loanDetails.Volume.Copy.Title.Title1,
                    CopyNumber = loanDetails.Volume.Copy.CopyNumber,
                    Barcode = loanDetails.Volume.Barcode,
                    BorrowerUserId = loanDetails.BorrowerUser.Id,
                    //TitleId = loanDetails.Volume.Copy.TitleID,
                    //CopyId = loanDetails.Volume.CopyID,
                    Borrowed = loanDetails.Borrowed,
                    ReturnDue = loanDetails.ReturnDue,
                    VolumeId = loanDetails.VolumeID,
                    Renewal = loanDetails.Renewal
                };

                var users = UserManager.Users.OrderBy(u => u.Lastname).ThenBy(u => u.Firstname)
                .ToList()
                .Select(u => new
                {
                    u.Id,
                    Fullname = u.FullnameRev,
                    viewModel.BorrowerUserId
                });

                ViewBag.UserID = new SelectList(users, "Id", "Fullname", viewModel.BorrowerUserId);
                ViewBag.Title = "Edit Loan";
                return PartialView(viewModel);
            }
            return null;
        }

        [HttpPost]
        public ActionResult Edit(EditLoanViewModel viewModel)
        {
            var loanDetails = _db.Borrowings.Find(viewModel.BorrowID);
            if (loanDetails == null)
            {
                return HttpNotFound();
            }
            loanDetails.BorrowerUser = _db.Users.Find(viewModel.UserID);
            loanDetails.ReturnDue = viewModel.ReturnDue;
            loanDetails.Renewal = viewModel.Renewal;

            _db.Entry(loanDetails).State = EntityState.Modified;
            _db.SaveChanges();

            return RedirectToAction("ItemsOnLoan");
        }

        // GET: Loans/New
        public ActionResult NewLoan(string userId = "", bool success = false)
        {
            // Get a list of users for a drop-down list
            var users = UserManager.Users.Where(u => u.IsLive && u.CanDelete).OrderBy(u => u.Lastname).ThenBy(u => u.Firstname)
                .ToList()
                .Select(u => new
                {
                    u.Id,
                    Fullname = u.FullnameRev
                });
            
            // Get a list of barcodes for a drop-down list
            var currentLoans = _db.Borrowings.Where(b => b.Returned == null).Select(b => b.VolumeID);
            var availableVolumes = _db.Volumes.Where(v => v.Deleted == false && v.LoanType.RefOnly == false && v.LoanType.LengthDays > 0 && !currentLoans.Contains(v.VolumeID));
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
                Users = new SelectList(users, "Id", "Fullname", userId),
                Volumes = new SelectList(volumes, "VolumeID", "Barcode"),
                Copies = new SelectList(copies, "CopyID", "CopyNumber"),
                Titles = new SelectList(titles, "TitleID", "Title"),
                SeeAlso = MenuHelper.SeeAlso("BorrowingSeeAlso", ControllerContext.RouteData.Values["action"].ToString(), null, "SortOrder")
            };

            ViewData["SeeAlso"] = viewModel.SeeAlso; //MenuHelper.SeeAlso("BorrowingSeeAlso", ControllerContext.RouteData.Values["action"].ToString(), null, "SortOrder");
            if (success)
            {
                TempData["SuccessMsg"] = "Item loaned successfully. Add another?";
            }
            ViewBag.Title = "Enter New Loan";
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

            ViewData["SeeAlso"] = MenuHelper.SeeAlso("BorrowingSeeAlso", ControllerContext.RouteData.Values["action"].ToString(), null, "SortOrder");
            ViewBag.Title = "New Loan";
            return View("NewLoan", viewModel);
        }

        //Return Loan
        public ActionResult ReturnLoan(bool success = false)
        {
            var currentLoans = _db.Borrowings.Where(b => b.Returned == null).Select(b => b.VolumeID).Distinct();
            var availableVolumes = _db.Volumes.Where(v => v.Deleted == false && currentLoans.Contains(v.VolumeID));
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

            var viewModel = new ReturnLoanViewModel()
            {
                Volumes = new SelectList(volumes, "VolumeID", "Barcode"),
                Copies = new SelectList(copies, "CopyID", "CopyNumber"),
                Titles = new SelectList(titles, "TitleID", "Title"),
            };

            if (success)
            {
                TempData["SuccessMsg"] = "Loan returned successfully. Return another?";
            }
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("BorrowingSeeAlso", ControllerContext.RouteData.Values["action"].ToString(), null, "SortOrder");
            ViewBag.Title = "Return Loan";
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
            
            var viewModel = new ConfirmationRenewReturnViewModel
            {
                PostConfirmController = "Loans",
                PostConfirmAction = "DoQuickReturn",
                ConfirmationText = "<p>Do you want to continue?</p>",
                DetailsText = "<p>You are about to return the following loan: </p>",
                ConfirmButtonText = "Return Loan",
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
            
            var viewModel = new ConfirmationRenewReturnViewModel
            {
                PostConfirmController = "Loans",
                PostConfirmAction = "DoQuickRenew",
                ConfirmationText = "<p>Do you want to continue?</p>",
                DetailsText = "<p>You are about to renew the following loan:</p>",
                ConfirmButtonText = "Renew Loan",
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
                return View("ReturnLoan", viewModel);
            }
            
            return RedirectToAction("ReturnLoan", new {success = true});
        }

        public ActionResult DoQuickReturn(ConfirmationRenewReturnViewModel viewModel)
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
            var currentLoans = _db.Borrowings.Where(b => b.Returned == null).Select(b => b.VolumeID).Distinct();
            var availableVolumes = _db.Volumes.Where(v => v.Deleted == false && currentLoans.Contains(v.VolumeID));
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

            var viewModel = new RenewLoanViewModel()
            {
                Volumes = new SelectList(volumes, "VolumeID", "Barcode"),
                Copies = new SelectList(copies, "CopyID", "CopyNumber"),
                Titles = new SelectList(titles, "TitleID", "Title"),
                ReturnDue = DateTime.Today.AddDays(21),
            };

            if (success)
            {
                TempData["SuccessMsg"] = "Loan renewed successfully. Renew another?";
            }
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("BorrowingSeeAlso", ControllerContext.RouteData.Values["action"].ToString(), null, "SortOrder");
            ViewBag.Title = "Renew Loan";
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
                return View("RenewLoan", viewModel);
            }

            try
            {
                var newLoan = new Borrowing
                {
                    VolumeID = volume.VolumeID,
                    BorrowerUser = loan.BorrowerUser,
                    Borrowed = DateTime.Today,
                    ReturnDue = viewModel.ReturnDue,
                    InputDate = DateTime.Now,
                    Renewal = true
                };

                _db.Borrowings.Add(newLoan);
                _db.SaveChanges();
                
            }
            catch (Exception)
            {
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
        public ActionResult DoQuickRenew(ConfirmationRenewReturnViewModel viewModel)
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

        //Return a JSON object containing details for the selected or entered barcode ...
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
            string origReturnDue = DateTime.Today.ToString("dd-MM-yyyy");
            DateTime newReturnDue = DateTime.Today.AddDays(21);
            string borrowedBy = "";
            var borrowing = _db.Borrowings.FirstOrDefault(b => b.VolumeID == volume.VolumeID && b.Returned == null);
            if (borrowing != null)
            {
                origReturnDue = borrowing.ReturnDue.ToString();
                borrowedBy = borrowing.BorrowerUser.Fullname;
            }

            if(volume.LoanType.LengthDays > 0)
            {
                newReturnDue = DateTime.Today.AddDays(volume.LoanType.LengthDays);  //.Date.ToString("dd/MM/yyyy")
            }

            return Json(new
            {
                success = true,
                BarcodeDetails = title + " - Copy " + copy,
                BorrowedBy = borrowedBy,
                origReturnDue = origReturnDue,
                newReturnDue = newReturnDue.Date.ToString("dd/MM/yyyy")
            });

        }
        
        [HttpPost]
        public JsonResult BarcodeExists(string barcode)
        {
            barcode = barcode.Trim();
            var volume = _db.Volumes.FirstOrDefault(v => v.Barcode == barcode);
            return Json(volume != null);
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