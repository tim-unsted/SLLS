using System;
using System.EnterpriseServices;
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
    public class ActivityTypesController : FinanceBaseController
    {
        private readonly DbEntities _db = new DbEntities();
        private readonly string _entityName = DbRes.T("Activities.Activity_Type", "FieldDisplayName");
        private readonly GenericRepository _repository;

        public ActivityTypesController()
        {
            ViewBag.Title = DbRes.T("Activities", "EntityType");
            _repository = new GenericRepository(typeof(ActivityType));
        }
        
        // GET: ActivityTypes
        public ActionResult Index()
        {
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("authlistsSeeAlso", ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["controller"].ToString());
            return View(_repository.GetAll<ActivityType>().Where(a => a.Deleted == false).OrderBy(a => a.ListPos).ThenBy(a => a.Activity));
        }
        
        // GET: ActivityTypes/Create
        public ActionResult Create()
        {
            var viewModel = new ActivityTypesAddViewModel();
            ViewBag.Title = "Add New " + _entityName;
            return PartialView(viewModel);
        }

        // POST: ActivityTypes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
            [Bind(Include = "ActivityCode,Activity")] ActivityTypesAddViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var activityType = new ActivityType
                {
                    Activity = viewModel.Activity,
                    CanUpdate = true,
                    CanDelete = true,
                    InputDate = DateTime.Now
                };
                _repository.Insert(activityType);
                CacheProvider.RemoveCache("activitytypes");
                return Json(new { success = true });
            }

            return PartialView(viewModel);
        }

        public ActionResult _add()
        {
            return PartialView();
        }

        // POST: ActivityTypes/_add
        [HttpPost]
        public JsonResult _add(ActivityType activitytype)
        {
            if (ModelState.IsValid)
            {
                var newActivity = _db.ActivityTypes.FirstOrDefault(x => x.Activity == activitytype.Activity);

                if (newActivity == null)
                {
                    newActivity = new ActivityType()
                    {
                        Activity = activitytype.Activity,
                        CanUpdate = true,
                        CanDelete = true,
                        InputDate = DateTime.Now
                    };

                    _db.ActivityTypes.Add(newActivity);
                    _db.SaveChanges();
                    CacheProvider.RemoveCache("activitytypes");

                    return Json(new
                    {
                        success = true,
                        newData = newActivity
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        errMsg = "This Activity Type already exists!"
                    });
                }
            }
            return null;
        }

        // GET: ActivityTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var activityType = _repository.GetById<ActivityType>(id.Value);
            if (activityType == null)
            {
                return HttpNotFound();
            }
            if (activityType.Deleted)
            {
                return HttpNotFound();
            }

            var viewModel = new ActivityTypesEditViewModel
            {
                ActivityCode = activityType.ActivityCode,
                Activity = activityType.Activity,
                CanUpdate = activityType.CanUpdate,
                CanDelete = activityType.CanDelete
            };
            ViewBag.Title = "Edit " + _entityName;
            return PartialView(viewModel);
        }

        // POST: ActivityTypes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
            [Bind(Include = "ActivityCode,Activity")] ActivityTypesEditViewModel viewModel)
        {
            if (!ModelState.IsValid) return PartialView(viewModel);
            var activityType = _repository.GetById<ActivityType>(viewModel.ActivityCode);
            if (activityType == null)
            {
                return HttpNotFound();
            }
            if (activityType.Deleted)
            {
                return HttpNotFound();
            }
            //activityType.ActivityCode = viewModel.ActivityCode;
            activityType.Activity = viewModel.Activity;
            activityType.LastModified = DateTime.Now;
            _repository.Update(activityType);;
            //return RedirectToAction("Index");
            CacheProvider.RemoveCache("activitytypes");
            return Json(new { success = true });
        }
        
        [HttpGet]
        public ActionResult Delete(int id = 0)
        {
            var at = _db.ActivityTypes.Find(id);
            if (at == null)
            {
                return HttpNotFound();
            }
            if (at.Deleted)
            {
                return HttpNotFound();
            }
            //Check if we can delete this item ...
            if (at.CanDelete == false)
            {
                return RedirectToAction("Index");
            }

            // Create new instance of DeleteConfirmationViewModel and pass it to _DeleteConfirmation Dialog (Reusable)
            DeleteConfirmationViewModel deleteConfirmationViewModel = new DeleteConfirmationViewModel
            {
                DeleteEntityId = id,
                HeaderText = _entityName,
                PostDeleteAction = "Delete",
                PostDeleteController = "ActivityTypes",
                DetailsText = at.Activity
            };
            return PartialView("_DeleteConfirmation", deleteConfirmationViewModel);
        }

        [HttpPost]
        public ActionResult Delete(DeleteConfirmationViewModel dcvm)
        {
            // Get item which we need to delete from EntityFramework 
            var item = _db.ActivityTypes.Find(dcvm.DeleteEntityId);

            if (item == null)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _db.ActivityTypes.Remove(item);
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