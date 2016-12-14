using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.WebPages;
using OfficeOpenXml.FormulaParsing.Utilities;
using slls.App_Settings;
using slls.DAO;
using slls.Models;
using slls.Utils.Helpers;
using slls.ViewModels;

namespace slls.Areas.Config
{
    public class ParametersController : ConfigBaseController
    {
        private readonly DbEntities _db = new DbEntities();
        private readonly GenericRepository _repository;
        private readonly string _customerPackage = GlobalVariables.Package;

        public ParametersController()
        {
            _repository = new GenericRepository(typeof(Parameter));
        }

        // GET: LibraryAdmin/Parameters
        public ActionResult Index(string parameterArea = "")
        {
            var allParameters = CacheProvider.GetAll<Parameter>("parameters").ToList();
            List<Parameter> userParameters;

            //If the user is a Bailey Admin then grant them access to everything!
            if (User.IsInRole("Bailey Admin"))
            {
                userParameters = allParameters;
            }
            else
            {
                //This area is for a System Admins only, so they should see all parameters matching their package ...
                userParameters = (from p in allParameters
                                  where p.Roles.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Any(role => role == "System Admin") &&
                                        p.Packages.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Any(package => package == _customerPackage)
                                  select p).ToList();
            }

            var areas = userParameters.Select(x => x.ParameterArea).Distinct().ToList();

            var viewModel = new ParameterIndexViewModel
            {
                Parameters =
                    !string.IsNullOrEmpty(parameterArea)
                        ? userParameters.Where(x => x.ParameterID.StartsWith(parameterArea + "."))
                        : userParameters
            };

            ViewBag.Message = "Parameters";
            ViewData["Areas"] = new SelectList(areas, "");
            ViewBag.Title = "System Parameters (Settings)";
            return View(viewModel);
        }
        
        // GET: LibraryAdmin/Parameters/Create
        [AuthorizeRoles(Roles.BaileyAdmin)]
        public ActionResult Create()
        {
            var viewModel = new ParametersAddEditViewModel();
            ViewBag.Title = "Add New System Parameter";
            return PartialView(viewModel);
        }

        // POST: LibraryAdmin/Parameters/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRoles(Roles.BaileyAdmin)]
        public ActionResult Create([Bind(Include = "ParameterID,ParameterValue,ParamUsage")] ParametersAddEditViewModel viewmodel)
        {
            if (ModelState.IsValid)
            {
                var packages = "";
                var customerPackage = GlobalVariables.Package;
                switch (customerPackage)
                {
                    case "collectors" :
                    {
                        packages = "collectors;";
                        break;
                    }
                    case "sharing":
                    {
                        packages = "collectors;sharing;";
                        break;
                    }
                    case "expert":
                    {
                        packages = "collectors;sharing;expert;";
                        break;
                    }
                    case "super":
                    {
                        packages = "collectors;sharing;expert;super;";
                        break;
                    }
                    default:
                    {
                        packages = "collectors;sharing;expert;";
                        break;
                    }
                }
                
                var parameter = new Parameter()
                {
                    ParameterID = viewmodel.ParameterID,
                    ParameterValue = viewmodel.ParameterValue,
                    ParamUsage = viewmodel.ParamUsage,
                    Roles = "System Admin;Bailey Admin;",
                    Packages = packages,
                    InputDate = DateTime.Now
                };
                _db.Parameters.Add(parameter);
                _db.SaveChanges();
                CacheProvider.RemoveCache("parameters");
                return Json(new { success = true });
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

            if (parameter.ParameterValue.Length <= 10)
            {
                viewModel.Class = "small";
            }
            if (parameter.ParameterValue == "true" || parameter.ParameterValue == "false")
            {
                viewModel.Class = "small";
            }
            if (parameter.ParameterValue.StartsWith("#"))
            {
                viewModel.Class = "small";
            }
            if (parameter.ParameterValue.EndsWith("px"))
            {
                viewModel.Class = "small";
            }

            ViewBag.Title = "Edit System Parameter";
            return PartialView(viewModel);
        }

        // POST: LibraryAdmin/Parameters/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PostEdit([Bind(Include = "RecID,ParameterID,ParameterValue,ParamUsage")] ParametersAddEditViewModel viewModel)
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
            return PartialView("Edit", viewModel);
        }

        // GET: LibraryAdmin/Parameters
        public ActionResult Styling()
        {
            var userRoles = Roles.GetUserRoles();
            var allParameters = CacheProvider.GetAll<Parameter>("parameters").ToList();
            
            var viewModel = new ParameterIndexViewModel
            {
                Parameters = allParameters.Where(x => x.ParameterID.StartsWith("Styling."))
            };

            ViewBag.Title = "Style Settings (Css)";
            return View(viewModel);
        }

        // GET: LibraryAdmin/Parameters/EditStyle/5
        public ActionResult EditStyle(int id = 0)
        {
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

            ViewBag.Title = "Edit Style Setting";
            return PartialView("Edit", viewModel);
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
