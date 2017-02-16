using System;
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
            var allParameters = _db.Parameters.Where(p => p.Deleted == false).ToList();

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
        public ActionResult Edit(int id = 0, string parameterId = "")
        {
            if (id == 0 && string.IsNullOrEmpty(parameterId))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Parameter parameter = new Parameter();

            if (id > 0)
            {
                parameter = _db.Parameters.Find(id);
            }
            if (!string.IsNullOrEmpty(parameterId))
            {
                parameter = _db.Parameters.FirstOrDefault(p => p.ParameterID == parameterId);
            }
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
                ParameterArea = parameter.ParameterArea,
                DataType1 = parameter.DataType1
            };

            switch (parameter.DataType1)
            {
                case "text":
                    {
                        viewModel.ParameterValueText = parameter.ParameterValue;
                        break;
                    }
                case "longtext":
                    {
                        viewModel.ParameterValueLongText = parameter.ParameterValue;
                        break;
                    }
                case "int":
                    {
                        viewModel.ParameterValueInteger = int.Parse(parameter.ParameterValue);
                        break;
                    }
                case "double":
                    {
                        viewModel.ParameterValueDouble = double.Parse(parameter.ParameterValue);
                        break;
                    }
                case "bool":
                    {
                        viewModel.ParameterValueBoolean = parameter.ParameterValue == "true";
                        break;
                    }
                default:
                    {
                        viewModel.ParameterValue = parameter.ParameterValue;
                        break;
                    }
            }

            ViewBag.Title = "Edit System Parameter";
            return PartialView(viewModel);
        }

        // POST: LibraryAdmin/Parameters/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PostEdit(ParametersAddEditViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var parameter = _db.Parameters.Find(viewModel.RecID);
                if (parameter == null)
                {
                    return HttpNotFound();
                }

                parameter.ParameterID = viewModel.ParameterID;
                parameter.ParamUsage = viewModel.ParamUsage;
                parameter.DataType1 = viewModel.DataType1;
                parameter.LastModified = DateTime.Now;

                switch (viewModel.DataType1)
                {
                    case "text":
                        {
                            parameter.ParameterValue = viewModel.ParameterValueText;
                            break;
                        }
                    case "longtext":
                        {
                            parameter.ParameterValue = viewModel.ParameterValueLongText;
                            break;
                        }
                    case "int":
                        {
                            parameter.ParameterValue = viewModel.ParameterValueInteger.ToString();
                            break;
                        }
                    case "double":
                        {
                            parameter.ParameterValue = viewModel.ParameterValueDouble.ToString();
                            break;
                        }
                    case "bool":
                        {
                            parameter.ParameterValue = viewModel.ParameterValueBoolean == true ? "true" : "false";
                            break;
                        }
                    default:
                        {
                            parameter.ParameterValue = viewModel.ParameterValue;
                            break;
                        }
                }

                _db.Entry(parameter).State = EntityState.Modified;
                _db.SaveChanges();
                CacheProvider.RemoveCache("parameters");

                //Reload all of the global styling variables ...
                if (parameter.ParameterID.StartsWith("Styling."))
                {
                    CssManager.LoadCss();
                }
                if (parameter.ParameterID == "General.PopupTimeout")
                {
                    GlobalVariables.PopupTimeout = parameter.ParameterValue;
                }
                if (parameter.ParameterID == "General.DatepickerDateFormat")
                {
                    GlobalVariables.DateFormat = parameter.ParameterValue;
                }
                if (parameter.ParameterID == "OPAC.OpacName")
                {
                    GlobalVariables.OpacName = parameter.ParameterValue;
                }
                if (parameter.ParameterID == "OPAC.SiteName")
                {
                    GlobalVariables.SiteName = parameter.ParameterValue;
                }
                if (parameter.ParameterID == "Security.IpFilteringOn")
                {
                    GlobalVariables.IpFilteringOn = parameter.ParameterValue == "true";
                }

                return Json(new { success = true });
            }

            ViewBag.Title = "Edit System Parameter";
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
