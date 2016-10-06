using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using slls.Models;
using slls.Utils.Helpers;
using slls.ViewModels;
using Westwind.Globalization;

namespace slls.Areas.LibraryAdmin
{
    public class CommMethodTypesController : AdminBaseController
    {
        private readonly DbEntities _db = new DbEntities();
        private readonly string _entityName = DbRes.T("CommunicationTypes.Method", "FieldDisplayName");

        // GET: LibraryAdmin/CommMethodTypes
        public ActionResult Index()
        {
            ViewBag.Title = DbRes.T("CommunicationTypes", "EntityType");
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("authlistsSeeAlso", ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["controller"].ToString());
            return View(_db.CommMethodTypes.ToList());
        }
        
        // GET: LibraryAdmin/CommMethodTypes/Create
        public ActionResult Create()
        {
            ViewBag.Title = "Add " + _entityName;
            return PartialView();
        }

        // POST: LibraryAdmin/CommMethodTypes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MethodID,Method")] CommMethodType commMethodType)
        {
            if (ModelState.IsValid)
            {
                commMethodType.InputDate = DateTime.Now;
                commMethodType.CanDelete = true;
                commMethodType.CanUpdate = true;
                _db.CommMethodTypes.Add(commMethodType);
                _db.SaveChanges();
                //return RedirectToAction("Index");
                return Json(new { success = true });
            }

            ViewBag.Title = "Add " + _entityName;
            return PartialView(commMethodType);
        }

        public ActionResult _add()
        {
            return PartialView();
        }

        // POST: Classmarks/_add
        [HttpPost]
        public JsonResult _add(CommMethodType commMethod)
        {
            if (ModelState.IsValid)
            {
                var newCommMethod = _db.CommMethodTypes.FirstOrDefault(x => x.Method == commMethod.Method);

                if (newCommMethod == null)
                {
                    newCommMethod = new CommMethodType
                    {
                        Method = commMethod.Method,
                        CanUpdate = true,
                        CanDelete = true,
                        InputDate = DateTime.Now
                    };

                    _db.CommMethodTypes.Add(newCommMethod);
                    _db.SaveChanges();

                    return Json(new
                    {
                        success = true,
                        newData = newCommMethod
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


        // GET: LibraryAdmin/CommMethodTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CommMethodType commMethodType = _db.CommMethodTypes.Find(id);
            if (commMethodType == null)
            {
                return HttpNotFound();
            }
            ViewBag.Title = "Edit " + _entityName;
            return PartialView(commMethodType);
        }

        // POST: LibraryAdmin/CommMethodTypes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MethodID,Method,CanUpdate,CanDelete,ListPos")] CommMethodType commMethodType)
        {
            if (ModelState.IsValid)
            {
                commMethodType.LastModified = DateTime.Now;
                _db.Entry(commMethodType).State = EntityState.Modified;
                _db.SaveChanges();
                return Json(new { success = true });
            }
            ViewBag.Title = "Edit " + _entityName;
            return PartialView(commMethodType);
        }

        [HttpGet]
        public ActionResult Delete(int id = 0)
        {
            var commType = _db.CommMethodTypes.Find(id);
            if (commType == null)
            {
                return HttpNotFound();
            }
            if (commType.Deleted)
            {
                return HttpNotFound();
            }

            //Check if we can delete this item ...
            if (commType.CanDelete == false)
            {
                return RedirectToAction("Index");
            }

            // Create new instance of DeleteConfirmationViewModel and pass it to _DeleteConfirmation Dialog (Reusable)
            DeleteConfirmationViewModel deleteConfirmationViewModel = new DeleteConfirmationViewModel
            {
                DeleteEntityId = id,
                HeaderText = _entityName,
                PostDeleteAction = "Delete",
                PostDeleteController = "CommMethodTypes",
                DetailsText = commType.Method
            };
            return PartialView("_DeleteConfirmation", deleteConfirmationViewModel);
        }

        [HttpPost]
        public ActionResult Delete(DeleteConfirmationViewModel dcvm)
        {
            // Get item which we need to delete from EntityFramework 
            var item = _db.CommMethodTypes.Find(dcvm.DeleteEntityId);

            if (item == null)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _db.CommMethodTypes.Remove(item);
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
            }
            base.Dispose(disposing);
        }
    }
}
