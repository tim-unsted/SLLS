using System;
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
    public class StatusTypesController : CatalogueBaseController
    {
        private readonly DbEntities _db = new DbEntities();
        private readonly string _entityName = DbRes.T("StatusTypes.Status_Type", "FieldDisplayName");
        private readonly GenericRepository _repository;

        public StatusTypesController()
        {
            ViewBag.Title = DbRes.T("StatusTypes", "EntityType");
            _repository = new GenericRepository(typeof(StatusType));
        }
        
        // GET: StatusTypes
        public ActionResult Index()
        {
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("authlistsSeeAlso", ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["controller"].ToString());
            return View(_repository.GetAll<StatusType>().Where(s => s.Deleted == false));
        }

        
        // GET: StatusTypes/Create
        public ActionResult Create()
        {
            ViewBag.Title = "Add new " + _entityName;
            var viewModel = new StatusTypesAddViewModel()
            {
                Opac = true
            };
            return PartialView(viewModel);
        }

        // POST: StatusTypes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
            [Bind(Include = "Status,Opac")] StatusTypesAddViewModel stvm)
        {
            if (ModelState.IsValid)
            {
                var statusType = new StatusType
                {
                    Status = stvm.Status,
                    Opac = stvm.Opac,
                    CanUpdate = true,
                    CanDelete = true,
                    InputDate = DateTime.Now
                };
                _repository.Insert(statusType);
                CacheProvider.RemoveCache("statustypes");
                return Json(new { success = true });
                //return RedirectToAction("Index");
            }

            return PartialView(stvm);
        }

        public ActionResult _add()
        {
            return PartialView();
        }

        // POST: Classmarks/_add
        [HttpPost]
        public JsonResult _add(StatusType statusType)
        {
            if (ModelState.IsValid)
            {
                var newStatusType = _db.StatusTypes.FirstOrDefault(x => x.Status == statusType.Status);

                if (newStatusType == null)
                {
                    newStatusType = new StatusType
                    {
                        Status = statusType.Status,
                        Opac = true,
                        CanUpdate = true,
                        CanDelete = true,
                        InputDate = DateTime.Now
                    };

                    _db.StatusTypes.Add(newStatusType);
                    _db.SaveChanges();
                    CacheProvider.RemoveCache("statustypes");

                    return Json(new
                    {
                        success = true,
                        newData = newStatusType
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        errMsg = "This " + _entityName + " already exists!"
                    });
                }
            }
            return null;
        }

        // GET: StatusTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var statusType = _repository.GetById<StatusType>(id.Value);
            if (statusType == null)
            {
                return HttpNotFound();
            }
            if (statusType.Deleted)
            {
                return HttpNotFound();
            }
            var stvm = new StatusTypesEditViewModel
            {
                StatusID = statusType.StatusID,
                Status = statusType.Status,
                Opac = statusType.Opac,
                CanDelete = statusType.CanDelete,
                CanUpdate = statusType.CanUpdate
            };
            ViewBag.Title = "Edit " + _entityName;
            return PartialView(stvm);
        }

        // POST: StatusTypes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
            [Bind(Include = "StatusID,Status,Opac,CanUpdate,CanDelete")] StatusTypesEditViewModel viewModel)
        {
            if (!ModelState.IsValid) return PartialView(viewModel);
            var statusType = _repository.GetById<StatusType>(viewModel.StatusID);
            if (statusType == null)
            {
                return HttpNotFound();
            }
            if (statusType.Deleted)
            {
                return HttpNotFound();
            }
            //Catch the current (i.e. before updated) status of the 'Copy.Status.Opac' boolean;
            var opacStatus = statusType.Opac;

            statusType.StatusID = viewModel.StatusID;
            statusType.Status = viewModel.Status;
            statusType.Opac = viewModel.Opac;
            statusType.LastModified = DateTime.Now;
            _repository.Update(statusType);
            CacheProvider.RemoveCache("statustypes");
            
            return Json(new { success = true });
        }
        
        [HttpGet]
        public ActionResult Delete(int id = 0)
        {
            var statusType = _repository.GetById<StatusType>(id);
            if (statusType == null)
            {
                return HttpNotFound();
            }
            if (statusType.Deleted)
            {
                return HttpNotFound();
            }
            if (statusType.CanDelete == false)
            {
                return RedirectToAction("Index");
            }
            // Create new instance of DeleteConfirmationViewModel and pass it to _DeleteConfirmation Dialog (Reusable)
            DeleteConfirmationViewModel deleteConfirmationViewModel = new DeleteConfirmationViewModel
            {
                DeleteEntityId = id,
                HeaderText = _entityName,
                PostDeleteAction = "Delete",
                PostDeleteController = "StatusTypes",
                DetailsText = statusType.Status
            };
            return PartialView("_DeleteConfirmation", deleteConfirmationViewModel);
        }

        [HttpPost]
        public ActionResult Delete(DeleteConfirmationViewModel dcvm)
        {
            // Get item which we need to delete from EntityFramework
            var statusType = _db.StatusTypes.Find(dcvm.DeleteEntityId);

            if (statusType == null)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _db.StatusTypes.Remove(statusType);
                    _db.SaveChanges();
                    CacheProvider.RemoveCache("statustypes");
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