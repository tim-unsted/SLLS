using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using slls.DAO;
using slls.Models;
using slls.Utils.Helpers;
using slls.ViewModels;
using Westwind.Globalization;

namespace slls.Areas.LibraryAdmin
{
    public class CopiesController : CatalogueBaseController
    {
        private readonly DbEntities _db = new DbEntities();
        private readonly string _entityName = DbRes.T("Copies.Copy", "FieldDisplayName");
        private ApplicationUserManager _userManager;

        private ApplicationUserManager UserManager
        {
            get { return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            set { _userManager = value; }
        }

        public CopiesController()
        {
            ViewBag.Title = DbRes.T("Copies", "EntityType"); ;
        }

        // GET: List All Copies
        public ActionResult Index(int id = 0)
        {
            var copies =
                _db.Copies
                    .Include(c => c.Location)
                    .Include(c => c.StatusType)
                    .Include(c => c.Title)
                    .Include(c => c.Volumes)
                    .Where(c => c.TitleID == id);

            ViewBag.Title = DbRes.T("Titles.Title", "FieldDisplayName") + " " + DbRes.T("Copies", "EntityType");
            ViewData["TitleId"] = SelectListHelper.TitlesWithCopies(id, msg: "Select a ");
            return View(copies.ToList());
        }


        public ActionResult List(int id = 0)
        {
            var titleCopies = _db.Copies.Include(c => c.Volumes).Where(c => c.TitleID == id);
            ViewData["TitleId"] = SelectListHelper.TitlesWithCopies(id, msg: "Filter by ");
            return PartialView(titleCopies);
        }

        public ActionResult _ThumbnailDetails(int id = 0)
        {
            var titleCopies = _db.Copies.Include(c => c.Volumes).Where(c => c.TitleID == id);
            ViewData["TitleId"] = SelectListHelper.TitlesWithCopies(id, msg: "Filter by ");
            return PartialView(titleCopies);
        }


        // GET: List all Copies with no volumes/barcodes
        public ActionResult CopiesNoVolumes()
        {
            var allTitles = _db.Titles.Where(t => t.Deleted == false).Select(t => t.TitleID);
            var copies = _db.Copies.Where(c => allTitles.Contains(c.TitleID) && c.Volumes.Count == 0);
            if (!copies.Any())
            {
                TempData["NoData"] = "You have no Copies without " + DbRes.T("Copies.Copy_Items", "FieldDisplayName") + "!";
            }
            ViewBag.Title = "Copies Without " + DbRes.T("Copies.Copy_Items", "FieldDisplayName");
            return View(copies.ToList());
        }


        public ActionResult ByLocation(int listLocations = 0)
        {
            //Get the list of locations in use ...
            var locations = (from l in _db.Locations
                              where l.Copies.Any()
                             orderby l.ParentLocation.Location1, l.Location1
                              select
                                  new {l.LocationID, Location = l.ParentLocation.Location1 + " - " + l.Location1 + " (" + l.Copies.Count + ")" })
                .Distinct();
        

        //Start a new list selectlist items ...
        var locationList = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select a " + DbRes.T("Locations.Location", "FieldDisplayName") + " ...",
                    Value = "0"
                }
            };

        //Add the actual locations ...
            foreach (var item in locations.OrderBy(l => l.Location))
            {
                locationList.Add(new SelectListItem
                {
                    Text = item.Location,
                    Value = item.LocationID.ToString()
                });
            }

            ViewData["ListLocations"] = locationList;
            //ViewData["SeeAlso"] = MenuHelper.SeeAlso("titlesSeeAlso", this.ControllerContext.RouteData.Values["action"].ToString());
            ViewBag.Title = ViewBag.Title + " By " + DbRes.T("Locations.Location", "FieldDisplayName");

            //Get the actual results if the user has selected anything ...
            var locationCopies =
                from c in
                    _db.Copies
                where c.LocationID == listLocations
                select c;
            return View(locationCopies.ToList());
        }


        //[Route("ByStatus")]
        //[Route("~/LibraryAdmin/Copies/ByStatus")]
        public ActionResult ByStatus(int listStatus = 0)
        {
            //Get the list of locations in use ...
            var statustypes = (from s in _db.StatusTypes
                             where s.Copies.Any()
                             orderby s.Status
                             select
                                 new {s.StatusID, Status = s.Status + " (" + s.Copies.Count + ")" })
                .Distinct();


            //Start a new list selectlist items ...
            var statusList = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select a " + DbRes.T("StatusTypes.Status_Type", "FieldDisplayName"),
                    Value = "0"
                }
            };

            //Add the actual locations ...
            foreach (var item in statustypes)
            {
                statusList.Add(new SelectListItem
                {
                    Text = item.Status,
                    Value = item.StatusID.ToString()
                });
            }

            ViewData["ListStatus"] = statusList;
            ViewBag.Title = ViewBag.Title + " By " + DbRes.T("StatusTypes.Status_Type", "FieldDisplayName");

            //Get the actual results if the user has selected anything ...
            var statusCopies =
                from c in
                    _db.Copies
                where c.StatusID == listStatus
                select c;
            return View(statusCopies.ToList());
        }


        //[Route("ByTitle")]
        //[Route("~/LibraryAdmin/Copies/ByTitle")]
        public ActionResult ByTitle(int listTitle = 0)
        {
            //Get the list of locations in use ...
            var titles = (from t in _db.Titles
                               where t.Copies.Any()
                               select
                                   new {t.TitleID, Title = t.Title1 + " (" + t.Copies.Count + ")", t.NonFilingChars })
                .Distinct();


            //Start a new list selectlist items ...
            var titleList = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select a " + DbRes.T("Titles.Title", "FieldDisplayName"),
                    Value = "0"
                }
            };

            //Add the actual locations ...
            foreach (var item in titles.OrderBy(t => t.Title.Substring(t.NonFilingChars)))
            {
                titleList.Add(new SelectListItem
                {
                    Text = item.Title,
                    Value = item.TitleID.ToString()
                });
            }

            ViewData["ListTitle"] = titleList;
            ViewBag.Title = ViewBag.Title + " By " + DbRes.T("Titles.Title", "FieldDisplayName");

            //Get the actual results if the user has selected anything ...
            var titleCopies =
                from c in
                    _db.Copies
                where c.TitleID == listTitle
                select c;
            return View(titleCopies.ToList());
        }
        

        // GET: Copies/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var copy = _db.Copies.Find(id);
            if (copy == null)
            {
                return HttpNotFound();
            }
            ViewBag.Title = "Copy Details";
            return View(copy);
        }

        // GET: Copies/Create -- this is a general create copy action which is unlikely to get used.
        public ActionResult Create()
        {
            ViewData["LocationID"] = SelectListHelper.OfficeLocationList(Utils.PublicFunctions.GetDefaultValue("Copies", "LocationID")); 
            ViewData["StatusID"] = SelectListHelper.StatusList(Utils.PublicFunctions.GetDefaultValue("Copies", "StatusID"));
            ViewData["TitleID"] = SelectListHelper.TitlesList();
            int step = 1;

            var viewModel = new CopiesAddViewModel
            {
                CopyNumber = 1,
                AcquisitionsList = true,
                Step = step
            };

            ViewBag.Title = step != 0 ? "Step " + step + ": Add New " + _entityName : "Add New " + _entityName;
            ViewBag.BtnText = "Next >";
            ViewBag.BtnTip = "Save new " + _entityName + " and add " + DbRes.T("CopyItems", "EntityType");
            return View(viewModel);
        }

        // GET: Copies/Add/5  -- Add a copy to a given title
        public ActionResult Add(int? id, int step = 1)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            //Get the Title of the item we're editing ...
            var title = _db.Titles.Find(id);
            if (title == null)
            {
                return HttpNotFound();
            }
            if (title.Deleted)
            {
                return HttpNotFound();
            }

            var viewModel = new CopiesAddViewModel
            {
                TitleID = id.Value,
                Title = title.Title1,
                CopyNumber = title.Copies == null ? 1 : title.Copies.Count + 1,
                AcquisitionsList = true,
                Step = step
            };

            ViewData["LocationID"] = SelectListHelper.OfficeLocationList(Utils.PublicFunctions.GetDefaultValue("Copies", "LocationID"));
            ViewData["StatusID"] = SelectListHelper.StatusList(Utils.PublicFunctions.GetDefaultValue("Copies", "StatusID"));
            ViewBag.Title = step != 0 ? "Step " + step + ": Add New " + _entityName : "Add New " + _entityName;
            ViewBag.BtnText = "Next >";
            ViewBag.BtnTip = "Save new " + _entityName + " and add " + DbRes.T("CopyItems", "EntityType");
            return View(viewModel);
        }

        // POST: Copies/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PostCreate(CopiesAddViewModel viewModel)
        {
            var copy = new Copy
            {
                TitleID = viewModel.TitleID,
                CopyNumber = viewModel.CopyNumber,
                AcquisitionsNo = viewModel.TitleID.ToString() + "." + viewModel.CopyNumber,
                LocationID = viewModel.LocationId <= 0 ? Utils.PublicFunctions.GetDefaultValue("Copies", "LocationID") : viewModel.LocationId,
                StatusID = viewModel.StatusId <= 0 ? Utils.PublicFunctions.GetDefaultValue("Copies", "StatusID") : viewModel.StatusId,
                CirculationMsgID = Utils.PublicFunctions.GetDefaultValue("Copies", "CirculationMsgID"),
                PrintLabel = true,
                AcquisitionsList = viewModel.AcquisitionsList,
                Commenced = DateTime.Now,
                InputDate = DateTime.Now
            };
            
            if (ModelState.IsValid)
            {
                _db.Copies.Add(copy);
                _db.SaveChanges();
                var copyId = copy.CopyID;

                //Add a volume ...
                var step = viewModel.Step != 0 ? viewModel.Step + 1 : 0;
                return RedirectToAction("Add", "Volumes", new { id = copyId, step = step });
            }

            ViewData["LocationID"] = SelectListHelper.OfficeLocationList(Utils.PublicFunctions.GetDefaultValue("Copies", "LocationID"));
            ViewData["StatusID"] = SelectListHelper.StatusList(Utils.PublicFunctions.GetDefaultValue("Copies", "StatusID"));
            ViewData["TitleID"] = SelectListHelper.TitlesList();
            ViewBag.Title = viewModel.Step != 0 ? "Step " + viewModel.Step + ": Add New " + _entityName : "Add New " + _entityName;
            ViewBag.BtnText = "Next >";
            ViewBag.BtnTip = "Save new " + _entityName + " and add " + DbRes.T("CopyItems", "EntityType");
            return View("Create",viewModel);
        }

        // GET: Copies/Edit/5
        //[Route("Edit/{id}")]
        //[Route("~/LibraryAdmin/Copies/Edit/{id}")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var copy = _db.Copies
                .FirstOrDefault(c => c.CopyID == id);
            if (copy == null)
            {
                return HttpNotFound();
            }

            var viewModel = new CopyDetailsEditViewModel()
            {
                CopyId = copy.CopyID,
                AccountYearId = copy.AccountYearID,
                AcquisitionsList = copy.AcquisitionsList,
                AcquisitionsNo = copy.AcquisitionsNo,
                Bind = copy.Bind,
                Cancellation = copy.Cancellation,
                Circulated = copy.Circulated,
                CirculationMsgId = copy.CirculationMsgID,
                Commenced = copy.Commenced,
                CopyNumber = copy.CopyNumber,
                Holdings = copy.Holdings,
                LocationId = copy.LocationID,
                Notes = copy.Notes,
                PrintLabel = copy.PrintLabel,
                Saving = copy.Saving,
                StatusId = copy.StatusID,
                TitleId = copy.TitleID,
                Title = copy.Title.Title1,
                CancelledByUser = copy.CancelledByUser,
                Circulations = copy.Circulations,
                CirculationMessage = copy.CirculationMessage
            };

            //Get all copies for this title ...
            var query = from c in _db.Copies
                where c.TitleID == copy.TitleID
                orderby c.CopyNumber
                select c;
            
            //Get the index of where the current CopyID is in a list ordered by CopyNumber ASC ..
            var rowIndex = query.AsEnumerable().Select((x, index) => new {x.CopyID, index}).First(i => i.CopyID == id).index;

            //Get the first CopyID ...
            var firstId = query.AsEnumerable().Select(i => i.CopyID).First();

            //Get the last CopyID ...
            var lastId = query.AsEnumerable().Select(i => i.CopyID).Last();

            //Get the next CopyID to move forward to ...
            var nextId = query.AsEnumerable()
                .OrderBy(i => i.CopyNumber)
                .SkipWhile(i => i.CopyID != id)
                .Skip(1)
                .Select(i => i.CopyID).FirstOrDefault();

            //Get the previous CopyID to move back to ...
            var previousId = query.AsEnumerable()
                .OrderByDescending(i => i.CopyNumber)
                .SkipWhile(i => i.CopyID != id)
                .Skip(1)
                .Select(i => i.CopyID).FirstOrDefault();

            ViewBag.FirstID = firstId;
            ViewBag.LastID = lastId;
            ViewBag.NextID = nextId;
            ViewBag.PreviousID = previousId;
            ViewBag.RowIndex = rowIndex + 1;
            ViewBag.RecordCount = query.Count();
            ViewBag.RecordType = "Copy";

            ViewData["CirculationMsgID"] = SelectListHelper.CirculationMessageList(copy.CirculationMsgID ?? 0, null, false);
            ViewData["LocationID"] = SelectListHelper.OfficeLocationList(copy.LocationID ?? 0, null, false);  //new SelectList(_db.Locations, "LocationID", "Location1", copy.LocationID);
            ViewData["StatusID"] = SelectListHelper.StatusList(copy.StatusID ?? 0, null, false, true); 
            ViewData["CancelledYear"] = new SelectList(_db.AccountYears.OrderBy(y => y.AccountYear1), "AccountYearID", "AccountYear1", copy.AccountYearID);
            ViewData["CancelledBy"] = new SelectList(_db.Users.Where(u => u.IsLive).OrderBy(u => u.Lastname).ThenBy(u => u.Firstname), "Id", "FullnameRev"); //, copy.CancelledByUser.Id);
            ViewData["CopyId"] = SelectListHelper.AllCopiesList(id.Value);
            ViewBag.VolumesCount = copy.Volumes.Count();
            ViewBag.Title = "Copy Details";
            return View(viewModel);
        }

        // POST: Copies/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PostEdit(CopyDetailsEditViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var copy = _db.Copies.Find(viewModel.CopyId);
                if (copy == null)
                {
                    return HttpNotFound();
                }

                //Catch the current (i.e. before updated) status f the 'New Title' boolean;
                var newTitleList = copy.AcquisitionsList;

                copy.CopyID = viewModel.CopyId;
                copy.AccountYearID = viewModel.AccountYearId;
                copy.AcquisitionsList = viewModel.AcquisitionsList;
                copy.AcquisitionsNo = viewModel.AcquisitionsNo;
                copy.Bind = viewModel.Bind;
                copy.Cancellation = viewModel.Cancellation;
                copy.Circulated = viewModel.Circulated;
                copy.CirculationMsgID = viewModel.CirculationMsgId;
                copy.Commenced = viewModel.Commenced;
                copy.CopyNumber = viewModel.CopyNumber;
                copy.Holdings = viewModel.Holdings;
                copy.LocationID = viewModel.LocationId;
                copy.Notes = viewModel.Notes;
                copy.PrintLabel = viewModel.PrintLabel;
                copy.Saving = viewModel.Saving;
                copy.StatusID = viewModel.StatusId;
                copy.TitleID = viewModel.TitleId;
                copy.CancelledByUser = viewModel.CancelledByUser;
                copy.LastModified = DateTime.Now;
                _db.Entry(copy).State = EntityState.Modified;
                _db.SaveChanges();

                //refresh cached 'New titles' if neccessary
                if (viewModel.AcquisitionsList != newTitleList)
                {
                    CacheProvider.RemoveCache("newtitles");
                }

                return RedirectToAction("Edit", "Copies", new { id = copy.CopyID });
            }

            return RedirectToAction("Edit", "Copies", new { id = viewModel.CopyId });
        }
        
        [HttpGet]
        //[Route("Delete/{id}")]
        //[Route("~/LibraryAdmin/Copies/Delete/{id}")]
        public ActionResult Delete(int id = 0)
        {            
            var copy = _db.Copies.Find(id);
            if (copy == null)
            {
                return HttpNotFound();
            }
            // Create new instance of DeleteConfirmationViewModel and pass it to _DeleteConfirmation Dialog (Reusable)
            DeleteConfirmationViewModel deleteConfirmationViewModel = new DeleteConfirmationViewModel
            {
                DeleteEntityId = id,
                HeaderText = _entityName,
                PostDeleteAction = "Delete",
                PostDeleteController = "Copies",
                DetailsText = copy.Title.Title1 + " - Copy: " +  copy.CopyNumber
            };
            return PartialView("_DeleteConfirmation", deleteConfirmationViewModel);
        }

        [HttpPost]
        public ActionResult Delete(DeleteConfirmationViewModel dcvm)
        {
            // Get item which we need to delete from EntityFramework 
            var copy = _db.Copies.Find(dcvm.DeleteEntityId);

            if (copy == null)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _db.Copies.Remove(copy);
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

        //View bound items
        //[Route("BoundItems")]
        //[Route("~/LibraryAdmin/Copies/BoundItems")]
        public ActionResult BoundItems()
        {
            var boundItems = _db.Copies.Where(c => c.Bind);
            ViewBag.Title = "Bound Items";
            return View(boundItems.ToList());
        }
        
        //Add to Binding list
        //[Route("Bind")]
        //[Route("AddToBinding")]
        //[Route("~/LibraryAdmin/Copies/Bind")]
        public ActionResult Bind()
        {            
            var viewModel = new ListBoxViewModel
            {
                PostSelectController = "Copies",
                PostSelectAction = "PostBind",
                SelectedItems = null,
                HeaderText = "Add Copies to Binding List",
                DetailsText = "Select the copies you wish to add to the binding list.",
                SelectLabel = "Select Copies"
            };

            viewModel.AvailableItems = _db.Copies.Where(c => c.Bind == false)
                .OrderBy(c => c.Title.Title1.Substring(c.Title.NonFilingChars))
                  .ThenBy(c => c.CopyNumber)
            .Select(x => new SelectListItem
            {
                Value = x.CopyID.ToString(),
                Text = x.Title.Title1 + " - Copy: " + x.CopyNumber.ToString()
            })
            .ToList();

            ViewBag.AvailableItemsTypes = new Dictionary<string, string>
            {
                {"a2z", "A-Z By Title"},
                {"dateadded", "Recently Added"}
            }; 

            return PartialView("_MultiSelectListBox", viewModel);
        }

        //Method used to supply a JSON list of copies when selecting a title (Ajax stuf)
        public JsonResult GetListRows(string listType = "a2z")
        {
            IEnumerable<SelectListItem> availableItems;

            if (listType == "a2z")
            {
                availableItems = _db.Copies.Where(c => c.Bind == false)
                  .OrderBy(c => c.Title.Title1.Substring(c.Title.NonFilingChars))
                  .ThenBy(c => c.CopyNumber)
                  .Select(x => new SelectListItem
                  {
                      Value = x.CopyID.ToString(),
                      Text = x.Title.Title1 + " - Copy: " + x.CopyNumber.ToString()
                  })
                  .ToList();
            }
            else
            {
                availableItems = _db.Copies.Where(c => c.Bind == false)
                  .OrderByDescending(c => c.Title.TitleID)
                  .ThenBy(c => c.CopyNumber)
                  .Select(x => new SelectListItem
                  {
                      Value = x.CopyID.ToString(),
                      Text = x.Title.Title1 + " - Copy: " + x.CopyNumber.ToString()
                  })
                  .ToList();
            }

            return Json(new
            {
                success = true,
                AvailableItems = availableItems
            });
        }

        //Add to Binding list
        [HttpPost]
        public ActionResult PostBind(ListBoxViewModel viewModel)
        {
           foreach (var copyid in viewModel.SelectedItems)
           {
               var copy = _db.Copies.Find(int.Parse(copyid));
               if (copy != null)
               {
                   copy.Bind = true;
                   copy.LastModified = DateTime.Now;
                   if (ModelState.IsValid)
                    {
                        try
                        {
                            _db.Entry(copy).State = EntityState.Modified;
                            _db.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            ModelState.AddModelError("", e.Message);
                        }
                    }
                }
           }
            return RedirectToAction("BoundItems");
        }


        //Remove from binding
        //[Route("RemoveFromBinding/{id}")]
        //[Route("Unbind/{id}")]
        //[Route("~/LibraryAdmin/Copies/RemoveFromBinding/{id}")]
        public ActionResult RemoveFromBinding(int? id)
        {
            var copy = _db.Copies.Find(id);
            if (copy == null)
            {
                return HttpNotFound();
            }
            // Create new instance of DeleteConfirmationViewModel and pass it to _DeleteConfirmation Dialog (Reusable)
            DeleteConfirmationViewModel deleteConfirmationViewModel = new DeleteConfirmationViewModel
            {
                DeleteEntityId = id.Value,
                HeaderText = _entityName + " from Binding List",
                PostDeleteAction = "RemoveFromBinding",
                PostDeleteController = "Copies",
                DetailsText = copy.Title.Title1 + " - Copy: " + copy.CopyNumber,
                ButtonText = "Remove",
                ButtonClass = "btn-success",
                ButtonGlyphicon = "glyphicon-remove",
                FunctionText = "Remove",
                ConfirmationText = "Are you sure you want to remove the following"
            };
            return PartialView("_DeleteConfirmation", deleteConfirmationViewModel);
        }

        [HttpPost]
        public ActionResult RemoveFromBinding(DeleteConfirmationViewModel dcvm)
        {
            // Get item which we need to delete from EntityFramework 
            var copy = _db.Copies.Find(dcvm.DeleteEntityId);

            if (copy == null)
            {
                return HttpNotFound();
            }
            copy.Bind = false;
            copy.LastModified = DateTime.Now;
            if (ModelState.IsValid)
            {
                try
                {
                    _db.Entry(copy).State = EntityState.Modified;
                    _db.SaveChanges();
                    TempData["SuccessDialogMsg"] = "All " + _entityName + " removed from Binding List removed successfully.";
                    return Json(new { success = true });
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", e.Message);
                }
            }
            return PartialView("_DeleteConfirmation", dcvm);
        }

        public ActionResult _Summary(int id = 0)
        {
            var copy = _db.Copies.Find(id);
            
            var viewModel = new CopySummaryViewModel
            {
                CopyId = copy.CopyID,
                TitleId = copy.TitleID,
                CopyNumber = copy.CopyNumber,
                Title = copy.Title.Title1,
                Location = copy.Location.Location1,
                Media = copy.Title.MediaType.Media,
                Status = copy.StatusType.Status,
                CirculationMsgID = copy.CirculationMsgID,
                Holdings = copy.Holdings,
                Notes = copy.Notes
            };

            ViewData["CirculationMsgID"] = new SelectList(_db.CirculationMessages, "CirculationMsgID", "CirculationMsg", copy.CirculationMsgID);
            return PartialView(viewModel);
        }


        public ActionResult EditHoldings(int? id)
        {
            var copy = _db.Copies.Find(id);
            if (copy == null)
            {
                return HttpNotFound();
            }

            var viewModel = new EditHoldingsNotesViewModel
            {
                CopyId = copy.CopyID,
                CopyNumber = copy.CopyNumber,
                Title = copy.Title.Title1,
                Holdings = copy.Holdings
            };

            ViewBag.Title = "Edit " + DbRes.T("Copies.Holdings","FieldDisplayName");
            return PartialView(viewModel);
        }

        [HttpPost]
        public ActionResult EditHoldings(EditHoldingsNotesViewModel viewModel)
        {
            var copy = _db.Copies.Find(viewModel.CopyId);

            if (copy == null)
            {
                return HttpNotFound();
            }

            copy.Holdings = viewModel.Holdings;
            copy.LastModified = DateTime.Now;
            if (ModelState.IsValid)
            {
                try
                {
                    _db.Entry(copy).State = EntityState.Modified;
                    _db.SaveChanges();
                    return Json(new { success = true });
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", e.Message);
                }
            }
            ViewBag.Title = "Edit " + DbRes.T("Copies.Holdings", "FieldDisplayName");
            return PartialView(viewModel);
        }


        public ActionResult EditNotes(int? id)
        {
            var copy = _db.Copies.Find(id);
            if (copy == null)
            {
                return HttpNotFound();
            }

            var viewModel = new EditHoldingsNotesViewModel
            {
                CopyId = copy.CopyID,
                CopyNumber = copy.CopyNumber,
                Title = copy.Title.Title1,
                Notes = copy.Notes
            };

            ViewBag.Title = "Edit " + DbRes.T("Copies.Notes", "FieldDisplayName");
            return PartialView(viewModel);
        }

        [HttpPost]
        public ActionResult EditNotes(EditHoldingsNotesViewModel viewModel)
        {
            var copy = _db.Copies.Find(viewModel.CopyId);

            if (copy == null)
            {
                return HttpNotFound();
            }

            copy.Notes = viewModel.Notes;
            copy.LastModified = DateTime.Now;
            if (ModelState.IsValid)
            {
                try
                {
                    _db.Entry(copy).State = EntityState.Modified;
                    _db.SaveChanges();
                    return Json(new { success = true });
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", e.Message);
                }
            }
            ViewBag.Title = "Edit " + DbRes.T("Copies.Notes", "FieldDisplayName");
            return PartialView(viewModel);
        }

        public void UpdateCircSlipMsg(int? copyId, int? msgId)
        {
            var copy = _db.Copies.Find(copyId);
            if (copy == null)
            {
                return;
            }

            var msg = _db.CirculationMessages.Find(msgId);
            if (msg == null)
            {
                return;
            }

            copy.CirculationMsgID = msgId;
            copy.LastModified = DateTime.Now;
            if (ModelState.IsValid)
            {
                try
                {
                    _db.Entry(copy).State = EntityState.Modified;
                    _db.SaveChanges();
                    //return Json(new { success = true });
                }
                catch (Exception e)
                {
                    //ModelState.AddModelError("", e.Message);
                    ModelState.AddModelError("", e.Message);
                }
            }
        }


        public ActionResult Report_BindingList()
        {
            var boundCopies = _db.Copies.Where(c => c.Bind);
            var locations = (from c in boundCopies
                where c.Bind
                select c.Location.ParentLocation).Distinct();

            var viewModel = new TitlesReportsViewModel()
            {
                Locations = locations,
                HasData = boundCopies.Any()
            };

            ViewBag.Title = "Binding List";
            return View("Reports/BindingList", viewModel);
        }


        public ActionResult BindingListItems(int id = 0)
        {
            //var copies = _db.Copies.Where(c => c.Location.ParentLocation.LocationID == id).OrderBy(c => c.Title.Title1.Substring(c.Title.NonFilingChars)).ThenBy(c => (int)c.CopyNumber);

            var boundTitles = (from t in _db.Titles
                join c in _db.Copies on t.TitleID equals c.TitleID
                where c.Location.ParentLocation.LocationID == id && c.Bind
                orderby t.Title1.Substring(t.NonFilingChars)
                select t).Distinct();

            return PartialView("Reports/_BindingListItems", boundTitles);
        }

        //Method used to supply the next available Copy Number for a given title
        public JsonResult GetNextCopyNumber(int titleId = 0)
        {
            //var copies = new SelectList(_db.Copies.Where(x => x.TitleID == titleId).ToList(), "CopyID", "CopyNumber");
            var copyNumber = _db.Copies.Where(c => c.TitleID == titleId).Select(c => c.CopyNumber).Max() + 1;

            return Json(new
            {
                success = true,
                CopyNumber = copyNumber
            });
        }


        public JsonResult CopyNumberExists(int copyNumber, int titleId)
        {
            var copy = _db.Copies.FirstOrDefault(c => c.TitleID == titleId && c.CopyNumber == copyNumber);
            //return Json(copy != null);
            return Json(new
            {
                success = true,
                Exists = copy != null
            });
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