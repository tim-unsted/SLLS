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
    public class LocationsController : CatalogueBaseController
    {
        private readonly DbEntities _db = new DbEntities();
        private readonly GenericRepository _repository;
        private readonly string _entityName = DbRes.T("Locations.Location", "FieldDisplayName");

        public LocationsController()
        {
            _repository = new GenericRepository(typeof(Location));
            ViewBag.Title = DbRes.T("Locations", "EntityType");
        }

        // GET: Locations
        public ActionResult Index(int? parentId)
        {
            var locations = _db.Locations.Where(l => l.Deleted == false).OrderBy(l => l.ParentLocation.Location1).ToList();
            
            if(parentId > 0)
            {
                locations = locations.Where(l => l.ParentLocationID == parentId).ToList();
            }

            if (parentId == 0)
            {
                locations = locations.Where(l => l.ParentLocationID == null).ToList();
            }

            ViewData["Parents"] = Utils.Helpers.SelectListHelper.ParentLocationList();
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("authlistsSeeAlso", ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["controller"].ToString());
            return View(locations);
        }

        
        // GET: Locations/Create
        public ActionResult Create()
        {
            //ViewData["ParentLocationID"] = new SelectList(_db.Locations.Where(l => l.Deleted == false).OrderBy(l => l.Location1), "LocationID", "Location1");
            ViewData["ParentLocationID"] = SelectListHelper.OfficeLocationList(addDefault: false, separator: " \x2192 ");
            var viewModel = new LocationsAddViewModel();
            ViewBag.Title = "Add New " + _entityName;
            return PartialView(viewModel);
        }

        // POST: Locations/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
            [Bind(Include = "ParentLocationID,Location,LocationHier")] LocationsAddViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var location = new Location
                {
                    Location1 = viewModel.Location,
                    ParentLocationID = viewModel.ParentLocationID,
                    CanUpdate = true,
                    CanDelete = true,
                    InputDate = DateTime.Now
                };
                _repository.Insert(location);
                CacheProvider.RemoveCache("locations");
                return Json(new { success = true });
                //return RedirectToAction("Index");
            }

            ViewData["ParentLocationID"] = new SelectList(_db.Locations.Where(l => l.Deleted == false).OrderBy(l => l.Location1), "LocationID", "Location1",
                viewModel.ParentLocationID);
            return PartialView(viewModel);
        }

        // GET: Locations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var location = _repository.GetById<Location>(id.Value);
            if (location == null)
            {
                return HttpNotFound();
            }
            if (location.Deleted)
            {
                return HttpNotFound();
            }

            var lvm = new LocationsEditViewModel
            {
                Location = location.Location1,
                LocationID = location.LocationID,
                ParentLocationID = location.ParentLocationID,
                CanUpdate = location.CanUpdate,
                CanDelete = location.CanDelete
            };
            ViewData["ParentLocationID"] = new SelectList(_db.Locations.Where(l => l.Deleted == false).OrderBy(l => l.Location1), "LocationID", "Location1",
                location.ParentLocationID);
            ViewBag.Title = "Edit " + _entityName;
            return PartialView(lvm);
        }

        // POST: Locations/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
            [Bind(Include = "LocationID,ParentLocationID,Location,LocationHier,CanUpdate,CanDelete")] LocationsEditViewModel lvm)
        {
            if (ModelState.IsValid)
            {
                var location = _repository.GetById<Location>(lvm.LocationID);
                if (location == null)
                {
                    return HttpNotFound();
                }
                if (location.Deleted)
                {
                    return HttpNotFound();
                }
                //location.LocationID = lvm.LocationID;
                location.Location1 = lvm.Location;
                location.ParentLocationID = lvm.ParentLocationID;
                location.CanDelete = lvm.CanDelete;
                location.CanUpdate = lvm.CanUpdate;
                location.LastModified = DateTime.Now;

                _repository.Update(location);
                CacheProvider.RemoveCache("locations");
                return Json(new { success = true });
                //return RedirectToAction("Index");
            }
            ViewData["ParentLocationID"] = new SelectList(_db.Locations.Where(l => l.Deleted == false).OrderBy(l => l.Location1), "LocationID", "Location1",
                lvm.ParentLocationID);
            return PartialView(lvm);
        }

        public static int GetLocationId(string location, int parentLocationId)
        {
            location = location.Trim();
            var db = new DbEntities();
            var allLocations = CacheProvider.GetAll<Location>("locations");
            var model = allLocations.FirstOrDefault(x => string.Equals(x.Location1, location, StringComparison.OrdinalIgnoreCase) && x.ParentLocationID == parentLocationId);
            if (model != null) return model.LocationID;
            //insert new Location now ...
            var newLocation = new Location
            {
                Location1 = location,
                ParentLocationID = parentLocationId,
                CanUpdate = true,
                CanDelete = true,
                InputDate = DateTime.Now
            };
            db.Locations.Add(newLocation);
            db.SaveChanges();
            CacheProvider.RemoveCache("locations");
            return newLocation.LocationID;
        }

        public static int GetOfficeId(string location)
        {
            location = location.Trim();
            var db = new DbEntities();
            var allLocations = CacheProvider.GetAll<Location>("locations");
            var model = allLocations.FirstOrDefault(x => string.Equals(x.Location1, location, StringComparison.OrdinalIgnoreCase) && x.ParentLocationID == null);
            if (model != null) return model.LocationID;
            //insert new Parent Location now ...
            var newLocation = new Location
            {
                Location1 = location,
                ParentLocationID = null,
                CanUpdate = true,
                CanDelete = true,
                InputDate = DateTime.Now
            };
            db.Locations.Add(newLocation);
            db.SaveChanges();
            CacheProvider.RemoveCache("locations");
            return newLocation.LocationID;
        }

        [HttpGet]
        public ActionResult Delete(int id = 0)
        {
            var location = _repository.GetById<Location>(id);
            if (location == null)
            {
                return HttpNotFound();
            }
            if (location.Deleted)
            {
                return HttpNotFound();
            }
            if (location.CanDelete == false)
            {
                return RedirectToAction("Index");
            }

            // Create new instance of DeleteConfirmationViewModel and pass it to _DeleteConfirmation Dialog (Reusable)
            DeleteConfirmationViewModel deleteConfirmationViewModel = new DeleteConfirmationViewModel
            {
                DeleteEntityId = id,
                HeaderText = _entityName,
                PostDeleteAction = "Delete",
                PostDeleteController = "Locations",
                DetailsText = location.Location1
            };
            return PartialView("_DeleteConfirmation", deleteConfirmationViewModel);
        }

        [HttpPost]
        public ActionResult Delete(DeleteConfirmationViewModel dcvm)
        {
            // Get item which we need to delete from EntityFramework
            var location = _repository.GetById<Location>(dcvm.DeleteEntityId);

            if (location == null)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _repository.Delete(location);
                    CacheProvider.RemoveCache("locations");
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