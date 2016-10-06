using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using slls.Controllers;
using slls.DAO;
using slls.Models;
using slls.Utils.Helpers;
using slls.ViewModels;

namespace slls.Areas.LibraryAdmin
{
    public class LoanTypesController : AdminBaseController
    {
        private readonly DbEntities _db = new DbEntities();

        // GET: LibraryAdmin/LoanTypes
        public ActionResult Index()
        {
            ViewBag.Title = "Loan Types";
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("authlistsSeeAlso", ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["controller"].ToString());
            return View(_db.LoanTypes.ToList());
        }
        
        // GET: LibraryAdmin/LoanTypes/Create
        public ActionResult Create()
        {
            var viewModel = new LoanTypesAddEditViewModel()
            {
                DailyFine = 0,
                LengthDays = 21,
                MaxItems = 0
            };
            ViewBag.Title = "Add New Loan Type";
            return PartialView(viewModel);
        }

        // POST: LibraryAdmin/LoanTypes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LoanTypeName,RefOnly,DailyFine,LengthDays,MaxItems")] LoanTypesAddEditViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var loanType = new LoanType();
                loanType.LoanTypeName = viewModel.LoanTypeName;
                loanType.DailyFine = viewModel.DailyFine ?? 0;
                loanType.MaxItems = viewModel.MaxItems;
                loanType.LengthDays = viewModel.LengthDays;
                loanType.RefOnly = viewModel.RefOnly;
                loanType.InputDate = DateTime.Now;
                loanType.CanDelete = true;
                loanType.CanUpdate = true;
                _db.LoanTypes.Add(loanType);
                _db.SaveChanges();
                return Json(new { success = true });
            }
            ViewBag.Title = "Add New Loan Type";
            return PartialView(viewModel);
        }

        // GET: LibraryAdmin/LoanTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var loanType = _db.LoanTypes.Find(id);
            if (loanType == null)
            {
                return HttpNotFound();
            }
            var viewModel = new LoanTypesAddEditViewModel()
            {
                LoanTypeID = loanType.LoanTypeID,
                LoanTypeName = loanType.LoanTypeName,
                RefOnly = loanType.RefOnly,
                MaxItems = loanType.MaxItems,
                LengthDays = loanType.LengthDays,
                DailyFine = loanType.DailyFine
            };
            ViewBag.Title = "Edit Loan Type";
            return PartialView(viewModel);
        }

        // POST: LibraryAdmin/LoanTypes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "LoanTypeID,LoanTypeName,RefOnly,DailyFine,LengthDays,MaxItems")] LoanTypesAddEditViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var loanType = _db.LoanTypes.Find(viewModel.LoanTypeID);
                if (loanType == null)
                {
                    return HttpNotFound();
                }
                loanType.LoanTypeName = viewModel.LoanTypeName;
                loanType.DailyFine = viewModel.DailyFine ?? 0 ;
                loanType.LengthDays = viewModel.LengthDays;
                loanType.MaxItems = viewModel.MaxItems;
                loanType.RefOnly = viewModel.RefOnly;
                loanType.LastModified = DateTime.Now;
                _db.Entry(loanType).State = EntityState.Modified;
                _db.SaveChanges();
                return Json(new { success = true });
                //return RedirectToAction("Index");
            }
            ViewBag.Title = "Edit Loan Type";
            return PartialView(viewModel);
        }

        // GET: LibraryAdmin/LoanTypes/Delete/5
        [HttpGet]
        public ActionResult Delete(int id = 0)
        {
            var loanType = _db.LoanTypes.Find(id);
            if (loanType == null)
            {
                return HttpNotFound();
            }
            if (loanType.CanDelete == false)
            {
                return RedirectToAction("Index");
            }
            // Create new instance of DeleteConfirmationViewModel and pass it to _DeleteConfirmation Dialog (Reusable)
            DeleteConfirmationViewModel deleteConfirmationViewModel = new DeleteConfirmationViewModel
            {
                DeleteEntityId = id,
                HeaderText = "Loan Type",
                PostDeleteAction = "Delete",
                PostDeleteController = "LoanTypes",
                DetailsText = loanType.LoanTypeName
            };
            return PartialView("_DeleteConfirmation", deleteConfirmationViewModel);
        }

        [HttpPost]
        public ActionResult Delete(DeleteConfirmationViewModel dcvm)
        {
            // Get item which we need to delete from EntityFramework
            var loanType = _db.LoanTypes.Find(dcvm.DeleteEntityId);

            if (loanType == null)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _db.LoanTypes.Remove(loanType);
                    _db.SaveChanges();
                    CacheProvider.RemoveCache("loantypes");
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
