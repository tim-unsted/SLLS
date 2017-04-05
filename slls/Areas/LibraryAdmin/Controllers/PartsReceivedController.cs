using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using slls.DAO;
using slls.Models;
using slls.Utils.Helpers;
using slls.ViewModels;
using Westwind.Globalization;

namespace slls.Areas.LibraryAdmin
{
    public class PartsReceivedController : SerialsBaseController
    {
        private readonly DbEntities _db = new DbEntities();
        private readonly GenericRepository _repository;
        private readonly string _entityName = DbRes.T("CheckIn.Parts_Received", "FieldDisplayName");

        public PartsReceivedController()
        {
            {
                _repository = new GenericRepository(typeof(PartsReceived));
                ViewBag.Title = DbRes.T("CheckIn", "EntityType");
            }
        }

        // GET: LibraryAdmin/PartsReceived
        public ActionResult Index(int id = 0)
        {
            //Get any parts received for the selected copy, if any ...
            var partsReceivedList = id == -1
                ? _db.PartsReceiveds
                : _db.PartsReceiveds.Where(p => p.CopyID == id);

            var viewModel = new PartsReceivedIndexViewModel
            {
                PartsReceivedList = partsReceivedList
            };

            if (id > 0)
            {
                var copy = _db.Copies.Find(id);
                viewModel.SelectCopy = copy.Title.Title1 + " - Copy: " + copy.CopyNumber;
                viewModel.CopyID = id;
            }

            //ViewData["SelectedCopy"] = SelectListHelper.AllCopiesList(addAll: true, msg: "Select a " + DbRes.T("Copies.Copy", "FieldDisplayName") + " to Check-In");
            ViewData["CopyID"] = id;
            ViewBag.Title = _entityName;
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("checkInSeeAlso", ControllerContext.RouteData.Values["action"].ToString(), null, "sortOrder");
            return View(viewModel);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SelectCopiesWithCheckins(string term)
        {
            term = " " + term;

            if (term.Length < 3)
            {
                var titles = (from c in _db.Copies join t in _db.vwSelectJustTitles on c.TitleID equals t.TitleId
                    where t.Title.StartsWith(term) && c.PartsReceived.Any()
                    orderby t.Title.Substring(t.NonFilingChars + 1)
                    select
                    new
                    {
                        CopyId = c.CopyID,
                        TitleId = c.TitleID,
                        CopyNumber = c.CopyNumber,
                        Title = t.Title.Trim(),
                        Location =
                            c.Location.ParentLocation != null
                                ? c.Location.ParentLocation.Location1 + " - " + c.Location.Location1
                                : c.Location.Location1
                    }).Take(250).ToList();

                return Json(titles, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var titles = (from c in _db.Copies
                              join t in _db.vwSelectJustTitles on c.TitleID equals t.TitleId
                              where t.Title.Contains(term) && c.PartsReceived.Any()
                              orderby t.Title.Substring(t.NonFilingChars + 1)
                              select
                              new
                              {
                                  CopyId = c.CopyID,
                                  TitleId = c.TitleID,
                                  CopyNumber = c.CopyNumber,
                                  Title = t.Title.Trim(),
                                  Location =
                                      c.Location.ParentLocation != null
                                          ? c.Location.ParentLocation.Location1 + " - " + c.Location.Location1
                                          : c.Location.Location1
                              }).Take(250).ToList();

                return Json(titles, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SelectCirculatedCopies(string term)
        {
            term = " " + term;
            //var titles = new List<SelectCirculatedCopy>();

            if (term.Length < 3)
            {
                var titles = (from c in _db.Copies
                              join t in _db.vwSelectJustTitles on c.TitleID equals t.TitleId
                              where t.Title.StartsWith(term) && c.Circulated
                              orderby t.Title.Substring(t.NonFilingChars + 1)
                              select
                                  new
                                  {
                                      CopyId = c.CopyID,
                                      TitleId = c.TitleID,
                                      CopyNumber = c.CopyNumber,
                                      Title = t.Title.Trim(),
                                      Location =
                                          c.Location.ParentLocation != null
                                              ? c.Location.ParentLocation.Location1 + " - " + c.Location.Location1
                                              : c.Location.Location1
                                  }).Take(250).ToList();

                return Json(titles, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var titles = (from c in _db.Copies
                              join t in _db.vwSelectJustTitles on c.TitleID equals t.TitleId
                              where t.Title.Contains(term) && c.Circulated
                              orderby t.Title.Substring(t.NonFilingChars + 1)
                              select
                                    new
                                    {
                                        CopyId = c.CopyID,
                                        TitleId = c.TitleID,
                                        CopyNumber = c.CopyNumber,
                                        Title = t.Title.Trim(),
                                        Location = c.Location.ParentLocation != null
                                            ? c.Location.ParentLocation.Location1 + " - " + c.Location.Location1
                                            : c.Location.Location1
                                    }).Take(250).ToList();

                return Json(titles, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        public ActionResult PartsOverdue()
        {
            //Get a list of overdue items:
            var partsOverdue = _db.Copies.Include(c => c.Title).Include(c => c.PartsReceived)
                .Where(c => c.Cancellation == null && c.PartsReceived.Any() && c.Title.Frequency.Days > 0 && DbFunctions.DiffDays(c.PartsReceived.OrderByDescending(p => p.DateReceived).FirstOrDefault().DateReceived.Value, DateTime.Now) > c.Title.Frequency.Days)
                .OrderBy(c => c.Title.Title1).ThenBy(c => c.CopyNumber);

            //Calculate when the next part was expected:
            foreach (var copy in partsOverdue)
            {
                var lastOrDefault = copy.PartsReceived.LastOrDefault();
                if (lastOrDefault != null)
                    if (lastOrDefault.DateReceived != null)
                        if (copy.Title.Frequency.Days != null)
                            copy.NextPartExpected =
                                lastOrDefault.DateReceived.Value.AddDays((int) copy.Title.Frequency.Days);
            }

            var viewModel = new PartsReceivedIndexViewModel
            {
                PartsExpectedList = partsOverdue
            };

            ViewData["SeeAlso"] = MenuHelper.SeeAlso("checkInSeeAlso", ControllerContext.RouteData.Values["action"].ToString(), null, "sortOrder");
            ViewBag.Title = "Parts Overdue";
            return View(viewModel);
        }


        public ActionResult NextPartsExpected(int daysAhead = -1)
        {
            //Get a list of parts exected today or in the future ...
            var today = daysAhead == 1 ? DateTime.Today.AddDays(1) : DateTime.Today;
            var futureDay = DateTime.Today.AddDays(daysAhead == -1 ? 9999 : daysAhead);
            var partsExpected = _db.Copies.Include(c => c.Title).Include(c => c.PartsReceived)
                .Where(c => c.Cancellation == null && c.PartsReceived.Any() && c.Title.Frequency.Days > 0 && DbFunctions.AddDays(c.PartsReceived.OrderByDescending(p => p.DateReceived).FirstOrDefault().DateReceived.Value, c.Title.Frequency.Days) >= today && DbFunctions.AddDays(c.PartsReceived.OrderByDescending(p => p.DateReceived).FirstOrDefault().DateReceived.Value, c.Title.Frequency.Days) <= futureDay)
                .OrderBy(c => c.Title.Title1).ThenBy(c => c.CopyNumber);

            //Calculate the data that the next part is expected:
            foreach (var copy in partsExpected)
            {
                var lastOrDefault = copy.PartsReceived.LastOrDefault();
                if (lastOrDefault != null)
                    if (lastOrDefault.DateReceived != null)
                        if (copy.Title.Frequency.Days != null)
                            copy.NextPartExpected =
                                lastOrDefault.DateReceived.Value.AddDays((int)copy.Title.Frequency.Days);
            }

            var viewModel = new PartsReceivedIndexViewModel
            {
                PartsExpectedList = partsExpected
            };

            ViewData["DaysAhead"] = new Dictionary<int, string>
            {
                {-1, "How far ahead?"},
                {0, "Today only"},
                {1, "Tomorrow only"},
                {7, "This week"},
                {14, "Next two weeks"},
                {21, "Next three weeks"},
                {30, "This month"},
                {60, "Next two months"},
                {90, "Next three months"},
                {180, "Next six months"},
                {365, "This year"}
               
            };
            ViewBag.Title = "Next Parts Expected";
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("checkInSeeAlso", ControllerContext.RouteData.Values["action"].ToString(), null, "sortOrder");
            return View(viewModel);
        }


        public ActionResult QuickCheckIn(int id = 0, string sortOrder = "t")
        {
            var copy = _db.Copies.FirstOrDefault(c => c.CopyID == id);

            var viewModel = new CopyDetailsEditViewModel()
            {
                SelectCopyTip = "Start typing the title of the item you wish to check-in ..."
            };

            if (copy != null)
            {
                viewModel.CopyId = id;
                viewModel.SelectCopy = copy.Title.Title1 + " - Copy: " + copy.CopyNumber;
                viewModel.SelectCopyTip = "To check-in another item, start typing the item's title ...";
                viewModel.PartsReceived = copy.PartsReceived;
            }

            List<SelectListItem> listOrder = new List<SelectListItem>();
            listOrder.Add(new SelectListItem { Text = "Title", Value = "t" });
            listOrder.Add(new SelectListItem { Text = "Date next part expected", Value = "e" });
            listOrder.Add(new SelectListItem { Text = "Date last part received", Value = "r" });

            ViewData["SortOrder"] = listOrder;
            //ViewData["SelectedCopy"] = SelectListHelper.AllCopiesList(addAll: false, msg: "Select a " + DbRes.T("Copies.Copy", "FieldDisplayName") + " to Check-In");
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("checkInSeeAlso", ControllerContext.RouteData.Values["action"].ToString(), null, "sortOrder");
            ViewBag.Title = "Quick Check-In";
            ViewData["CopyID"] = id;
            return View(viewModel);
        }

        public ActionResult _PartsReceivedSubForm(int id = 0)
        {
            var partsReceived = _db.PartsReceiveds.Where(p => p.CopyID == id).Distinct();

            return PartialView(partsReceived);
        }

        // GET: LibraryAdmin/PartsReceived/Create
        public ActionResult Create(int id = 0)
        {
            if (id == 0)
            {
                return null;
            }
            var copy = _db.Copies.Find(id);
            if (copy == null)
            {
                return HttpNotFound();
            }
            
            var viewModel = new PartsReceivedAddViewModel
            {
                PrintList = true,
                DateReceived = DateTime.Now,
                CopyID = copy.CopyID,
                Title = copy.Title.Title1,
                Copy = copy.CopyLocationStatus
            };
            
            ViewBag.Title = "Check-In New " + DbRes.T("CheckIn.Part","FieldDisplayName");
            //ViewData["CopyID"] = SelectListHelper.AllCopiesList(id: id, msg: "Select a " + DbRes.T("Copies.Copy", "FieldDisplayName") + " to Check-In");
            return PartialView(viewModel);
        }

        // POST: LibraryAdmin/PartsReceived/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PartsReceivedAddViewModel viewModel)
        {
            var newPart = new PartsReceived
            {
                CopyID = viewModel.CopyID,
                PartReceived = viewModel.PartReceived,
                PrintList = viewModel.PrintList,
                Returned = viewModel.Returned,
                DateReceived = viewModel.DateReceived
            };
            if (ModelState.IsValid)
            {
                _db.PartsReceiveds.Add(newPart);
                _db.SaveChanges();
                return Json( new {success = true});

                //var partsReceived = _db.PartsReceiveds.Where(p => p.CopyID == newPart.CopyID);
                //return Json(new { success = true, data = partsReceived });
            }
            ViewBag.Title = "Check-In new " + DbRes.T("CheckIn.Part", "FieldDisplayName");
            ViewData["CopyID"] = SelectListHelper.AllCopiesList(id:viewModel.CopyID, msg: "Select a " + DbRes.T("Copies.Copy", "FieldDisplayName") + " to Check-In");
            return PartialView(viewModel);
        }

        // GET: LibraryAdmin/PartsReceived/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var part = _db.PartsReceiveds.Find(id);
            if (part == null)
            {
                return HttpNotFound();
            }

            var viewModel = new PartsReceivedEditViewModel
            {
                PartID = part.PartID,
                Title = part.Copy.Title.Title1,
                CopyNumber = part.Copy.CopyNumber,
                PrintList = part.PrintList,
                DateReceived = part.DateReceived,
                PartReceived = part.PartReceived
            };

            ViewBag.Title = "Edit " + DbRes.T("CheckIn.Part","FieldDisplayName");
            return PartialView(viewModel);
        }

        // POST: LibraryAdmin/PartsReceived/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PartID,PartReceived,DateReceived,PrintList,Returned")] PartsReceivedEditViewModel viewModel)
        {
            var partReceived = _db.PartsReceiveds.Find(viewModel.PartID);
            partReceived.DateReceived = viewModel.DateReceived;
            partReceived.PartReceived = viewModel.PartReceived;
            partReceived.PrintList = viewModel.PrintList;

            if (ModelState.IsValid)
            {
                _db.Entry(partReceived).State = EntityState.Modified;
                _db.SaveChanges();
                //return RedirectToAction("Index");
                return Json(new { success = true });
            }
            ViewBag.Title = "Edit " + DbRes.T("CheckIn.Part","FieldDisplayName");
            return View(viewModel);
        }


        public ActionResult Report_PartsOverdue()
        {
            var overdueCopies = _db.Copies
                .Where(
                    c =>
                        c.Cancellation == null && c.PartsReceived.Any() && c.Title.Frequency.Days > 0 &&
                        DbFunctions.DiffDays(
                            c.PartsReceived.OrderByDescending(p => p.DateReceived).FirstOrDefault().DateReceived.Value,
                            DateTime.Now) > c.Title.Frequency.Days);
            
            var overdueSuppliers = (from o in overdueCopies
                select o.Title.OrderDetails.OrderByDescending(x => x.OrderID).FirstOrDefault().Supplier).Distinct().OrderBy(s => s.SupplierName);
            
            var viewModel = new CirculationReportsViewModel()
            {
                Suppliers = overdueSuppliers,
                HasData = overdueSuppliers.Any()
            };

            ViewBag.Title = "Parts Overdue";
            return View("Reports/OverdueParts", viewModel);
        }

        public ActionResult OverduePartsBySupplier(int id = 0)
        {
            var supplier = _db.Suppliers.Find(id);
            if (supplier == null)
            {
                return HttpNotFound();
            }

            //Get a list of overdue items:
            var partsOverdueBySupplier = _db.Copies.Include(c => c.Title).Include(c => c.PartsReceived)
                .Where(c => c.Cancellation == null && c.PartsReceived.Any() && c.Title.Frequency.Days > 0 && DbFunctions.DiffDays(c.PartsReceived.OrderByDescending(p => p.DateReceived).FirstOrDefault().DateReceived.Value, DateTime.Now) > c.Title.Frequency.Days && c.Title.OrderDetails.OrderByDescending(o => o.OrderID).FirstOrDefault().Supplier.SupplierID == id)
                .OrderBy(c => c.Title.Title1).ThenBy(c => c.CopyNumber);

            //Calculate when the next part was expected:
            if (partsOverdueBySupplier.Any()) { 
            foreach (var copy in partsOverdueBySupplier)
                {
                    var lastOrDefault = copy.PartsReceived.LastOrDefault();
                    if (lastOrDefault != null)
                        if (lastOrDefault.DateReceived != null)
                            if (copy.Title.Frequency.Days != null)
                                copy.NextPartExpected =
                                    lastOrDefault.DateReceived.Value.AddDays((int)copy.Title.Frequency.Days);
                }
            }

            var viewModel = new PartsOverdueViewModel
            {
                PartsOverdue = partsOverdueBySupplier
            };

            return PartialView("Reports/_OverdueParts", viewModel);
        }

        
        // GET: LibraryAdmin/PartsReceived/Delete/5
        [HttpGet]
        public ActionResult Delete(int id = 0)
        {
            var part = _db.PartsReceiveds.Find(id);
            if (part == null)
            {
                return HttpNotFound();
            }
            if (part.Deleted)
            {
                return HttpNotFound();
            }
            
            // Create new instance of DeleteConfirmationViewModel and pass it to _DeleteConfirmation Dialog (Reusable)
            DeleteConfirmationViewModel deleteConfirmationViewModel = new DeleteConfirmationViewModel
            {
                DeleteEntityId = id,
                HeaderText = DbRes.T("CheckIn.Part", "FieldDisplayName"),
                PostDeleteAction = "Delete",
                PostDeleteController = "PartsReceived",
                DetailsText = part.PartReceived + " (" + part.DateReceived + ")"
            };
            return PartialView("_DeleteConfirmation", deleteConfirmationViewModel);
        }

        [HttpPost]
        public ActionResult Delete(DeleteConfirmationViewModel dcvm)
        {
            // Get item which we need to delete from EntityFramework
            var part = _db.PartsReceiveds.Find(dcvm.DeleteEntityId);

            if (part == null)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _db.PartsReceiveds.Remove(part);
                    _db.SaveChanges();
                    return Json(new { success = true });
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
                _repository.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
