using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using slls.DAO;
using slls.Models;
using slls.Utils.Helpers;
using slls.ViewModels;
using Westwind.Globalization;

namespace slls.Areas.LibraryAdmin
{
    public class OrderCategoriesController : FinanceBaseController
    {
        private readonly DbEntities _db = new DbEntities();
        private readonly string _entityName = DbRes.T("OrderCategories.Order_Category", "FieldDisplayName");
        private readonly GenericRepository _repository;


        public OrderCategoriesController()
        {
            ViewBag.Title = DbRes.T("OrderCategories", "EntityType");
            _repository = new GenericRepository(typeof(OrderCategory));
        }

        // GET: LibraryAdmin/OrderCategories
        public ActionResult Index()
        {
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("authlistsSeeAlso", ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["controller"].ToString());
            return View(_db.OrderCategories.ToList());
        }

        // GET: LibraryAdmin/OrderCategories/Create
        public ActionResult Create()
        {
            ViewBag.Title = "Add New " + _entityName;
            return PartialView();
        }

        // POST: LibraryAdmin/OrderCategories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OrderCategoryID,OrderCategory1,Annual,Sub")] OrderCategory orderCategory)
        {
            if (ModelState.IsValid)
            {
                orderCategory.InputDate = DateTime.Now;
                orderCategory.CanDelete = true;
                orderCategory.CanUpdate = true;
                _db.OrderCategories.Add(orderCategory);
                _db.SaveChanges();
                return Json(new { success = true });
                //return RedirectToAction("Index");
            }

            ViewBag.Title = "Add New " + _entityName;
            return PartialView(orderCategory);
        }

        // GET: LibraryAdmin/OrderCategories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderCategory orderCategory = _db.OrderCategories.Find(id);
            if (orderCategory == null)
            {
                return HttpNotFound();
            }

            ViewBag.Title = "Edit " + _entityName;
            return PartialView(orderCategory);
        }

        // POST: LibraryAdmin/OrderCategories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OrderCategoryID,OrderCategory1,Annual,Sub")] OrderCategory orderCategory)
        {
            if (ModelState.IsValid)
            {
                orderCategory.LastModified = DateTime.Now;
                _db.Entry(orderCategory).State = EntityState.Modified;
                _db.SaveChanges();
                return Json(new { success = true });
                //return RedirectToAction("Index");
            }

            ViewBag.Title = "Edit " + _entityName;
            return PartialView(orderCategory);
        }

        [HttpGet]
        public ActionResult Delete(int id = 0)
        {
            var oc = _db.OrderCategories.Find(id);
            if (oc == null)
            {
                return HttpNotFound();
            }
            if (oc.Deleted)
            {
                return HttpNotFound();
            }
            //Check if we can delete this item ...
            if (oc.CanDelete == false)
            {
                return RedirectToAction("Index");
            }

            // Create new instance of DeleteConfirmationViewModel and pass it to _DeleteConfirmation Dialog (Reusable)
            DeleteConfirmationViewModel deleteConfirmationViewModel = new DeleteConfirmationViewModel
            {
                DeleteEntityId = id,
                HeaderText = _entityName,
                PostDeleteAction = "Delete",
                PostDeleteController = "OrderCategories",
                DetailsText = oc.OrderCategory1
            };
            return PartialView("_DeleteConfirmation", deleteConfirmationViewModel);
        }

        [HttpPost]
        public ActionResult Delete(DeleteConfirmationViewModel dcvm)
        {
            // Get item which we need to delete from EntityFramework 
            var item = _db.OrderCategories.Find(dcvm.DeleteEntityId);

            if (item == null)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _db.OrderCategories.Remove(item);
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
