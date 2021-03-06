﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using slls.App_Settings;
using slls.Controllers;
using slls.DAO;
using slls.Models;
using slls.Utils.Helpers;
using slls.ViewModels;

namespace slls.Areas.LibraryAdmin
{
    public class ParametersController : AdminBaseController
    {
        private readonly DbEntities _db = new DbEntities();
        private readonly GenericRepository _repository;

        public ParametersController()
        {
            _repository = new GenericRepository(typeof(Parameter));
        }

        // GET: LibraryAdmin/Parameters
        public ActionResult Index(string parameterArea = "")
        {
            var userRoles = Roles.GetUserRoles();
            var allParameters = CacheProvider.GetAll<Parameter>("parameters").ToList();

            //Only show options that match the logged-in user's roles ...
            var userParameters = (from p in allParameters
                                  where p.Roles.Split(new String[] { ";" }, StringSplitOptions.RemoveEmptyEntries)
                                      .Any(x => userRoles.Contains(x))
                                  select p).ToList();

            var areas = userParameters.Select(x => x.ParameterArea).ToList();

            var viewModel = new ParameterIndexViewModel
            {
                Parameters =
                    !string.IsNullOrEmpty(parameterArea)
                        ? userParameters.Where(x => x.ParameterID.StartsWith(parameterArea + "."))
                        : userParameters
            };

            ViewBag.Message = "Parameters";
            ViewData["Areas"] = new SelectList(areas.Distinct(), "");
            ViewBag.Title = "System Parameters (Settings)";
            return View(viewModel);
        }
        
        // GET: LibraryAdmin/Parameters/Create
        [AuthorizeRoles(Roles.BsAdmin)]
        public ActionResult Create()
        {
            var viewModel = new ParametersAddEditViewModel();
            ViewBag.Title = "Add New System Parameter";
            return PartialView(viewModel);
        }

        // POST: LibraryAdmin/Parameters/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRoles(Roles.BsAdmin)]
        public ActionResult Create([Bind(Include = "ParameterID,ParameterValue,ParamUsage")] ParametersAddEditViewModel viewmodel)
        {
            if (ModelState.IsValid)
            {
                var parameter = new Parameter()
                {
                    ParameterID = viewmodel.ParameterID,
                    ParameterValue = viewmodel.ParameterValue,
                    InputDate = DateTime.Now,
                    ParamUsage = viewmodel.ParamUsage,
                    Roles = Roles.IsLibraryStaff() ? "Admin;Bailey Admin;Library Staff" : "Admin;Bailey Admin"
                };
                _db.Parameters.Add(parameter);
                _db.SaveChanges();
                CacheProvider.RemoveCache("parameters");
                return Json(new { success = true });
                //return RedirectToAction("Index");
            }

            ViewBag.Title = "Add New System Parameter";
            return PartialView(viewmodel);
        }

        // GET: LibraryAdmin/Parameters/Edit/5
        public ActionResult Edit(int id = 0)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var parameter = _db.Parameters.Find(id);
            if (parameter == null)
            {
                return HttpNotFound();
            }
            var viewModel = new ParametersAddEditViewModel()
            {
                RecID = parameter.RecID,
                ParameterID = parameter.ParameterID,
                ParameterValue = parameter.ParameterValue,
                ParamUsage = parameter.ParamUsage,
                ParameterName = parameter.ParameterName,
                ParameterArea = parameter.ParameterArea
            };

            ViewBag.Title = "Edit System Parameter";
            return PartialView(viewModel);
        }

        // POST: LibraryAdmin/Parameters/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "RecID,ParameterID,ParameterValue,ParamUsage")] ParametersAddEditViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var parameter = _db.Parameters.Find(viewModel.RecID);
                if (parameter == null)
                {
                    return HttpNotFound();
                }
                parameter.ParameterID = viewModel.ParameterID;
                parameter.ParameterValue = viewModel.ParameterValue;
                parameter.ParamUsage = viewModel.ParamUsage;
                parameter.LastModified = DateTime.Now;

                _db.Entry(parameter).State = EntityState.Modified;
                _db.SaveChanges();
                CacheProvider.RemoveCache("parameters");

                //Reload all of the global styling variables ...
                if (parameter.ParameterID.StartsWith("Styling."))
                {
                    CssManager.LoadCss();
                }

                return Json(new { success = true });
                //return RedirectToAction("Index");
            }

            ViewBag.Title = "Edit System Parameter";
            return PartialView(viewModel);
        }

        // GET: LibraryAdmin/Parameters/Delete/5
        [HttpGet]
        public ActionResult Delete(int id = 0)
        {
            var parameter = _db.Parameters.Find(id);
            if (parameter == null)
            {
                return HttpNotFound();
            }
            if (parameter.Deleted)
            {
                return HttpNotFound();
            }
            if (parameter.CanDelete == false)
            {
                return RedirectToAction("Index");
            }
            // Create new instance of DeleteConfirmationViewModel and pass it to _DeleteConfirmation Dialog (Reusable)
            DeleteConfirmationViewModel deleteConfirmationViewModel = new DeleteConfirmationViewModel
            {
                //DeleteEntityId = id,
                DeleteEntityId = id,
                HeaderText = "System Parameter",
                PostDeleteAction = "Delete",
                PostDeleteController = "Parameters",
                DetailsText = parameter.ParameterID
            };
            return PartialView("_DeleteConfirmation", deleteConfirmationViewModel);
        }

        [HttpPost]
        public ActionResult Delete(DeleteConfirmationViewModel dcvm)
        {
            // Get item which we need to delete from EntityFramework
            var parameter = _db.Parameters.Find(dcvm.DeleteEntityIdString);

            if (parameter == null)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _repository.Delete(parameter);
                    CacheProvider.RemoveCache("parameters");
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
