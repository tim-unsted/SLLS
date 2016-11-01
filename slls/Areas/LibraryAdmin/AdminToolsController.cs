using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using slls.DAO;
using slls.Models;
using slls.Utils.Helpers;
using slls.ViewModels;
using Westwind.Globalization;

namespace slls.Areas.LibraryAdmin
{
    public class AdminToolsController : AdminBaseController
    {
        public ActionResult UpdateMergeAuthorityList(string list = "")
        {
            var authorityLists = new Dictionary<string, string>()
            {
                {"AccountYears", DbRes.T("AccountYears","EntityType")},
                {"ActivityTypes", DbRes.T("ActivityTypes","EntityType")},
                {"Authors", DbRes.T("Authors","EntityType")},
                {"BudgetCodes", DbRes.T("BudgetCodes","EntityType")},
                {"Classmarks", DbRes.T("Classmarks","EntityType")},
                {"CommMethodTypes", DbRes.T("CommMethodTypes","EntityType")},
                {"Departments", DbRes.T("Departments","EntityType")},
                {"Frequencies", DbRes.T("Frequencies","EntityType")},
                {"Keywords", DbRes.T("Keywords","EntityType")},
                {"Languages", DbRes.T("Languages","EntityType")},
                {"LoanTypes", DbRes.T("LoanTypes","EntityType")},
                {"Locations", DbRes.T("Locations","EntityType")},
                {"MediaTypes", DbRes.T("MediaTypes","EntityType")},
                {"OrderCategories", DbRes.T("OrderCategories","EntityType")},
                {"Publishers", DbRes.T("Publishers","EntityType")},
                {"StatusTypes", DbRes.T("StatusTypes","EntityType")},
                {"Suppliers", DbRes.T("Suppliers","EntityType")}
            };

            var listValues = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select an Authority List",
                    Value = "0"
                }
            };

            var viewModel = new MergeAuthorityListViewModel()
            {
                AvailableItems = listValues
            };
            ViewData["AuthorityLists"] = authorityLists;
            ViewBag.Title = "Update/Merge Authority List Values";
            return View(viewModel);
        }

        //Method used to supply a JSON list of authority list values when selecting an authority list (Ajax stuf)
        public JsonResult GetAuthorityValues(string AuthList = "")
        {
            var db = new DbEntities();
            var listValues = new List<SelectListItem>();
            switch (AuthList)
            {
                case "AccountYears":
                {
                    listValues.AddRange(db.AccountYears.OrderBy(a => a.AccountYear1).Select(item => new SelectListItem
                    {
                        Text = string.IsNullOrEmpty(item.AccountYear1) ? "<No name>" : item.AccountYear1,
                        Value = item.AccountYearID.ToString()
                    }));
                    break;
                }
                case "ActivityTypes":
                {
                    listValues.AddRange(db.ActivityTypes.OrderBy(a => a.Activity).Select(item => new SelectListItem
                    {
                        Text = string.IsNullOrEmpty(item.Activity) ? "<No name>" : item.Activity, Value = item.ActivityCode.ToString()
                    }));
                    break;
                }
                case "Authors":
                {
                    listValues.AddRange(db.Authors.OrderBy(a => a.DisplayName).Select(item => new SelectListItem
                    {
                        Text = item.DisplayName ?? "",
                        Value = item.AuthorID.ToString()
                    }));
                    break;
                }
                case "BudgetCodes":
                {
                    listValues.AddRange(db.BudgetCodes.OrderBy(a => a.BudgetCode1).Select(item => new SelectListItem
                    {
                        Text = string.IsNullOrEmpty(item.BudgetCode1) ? "<No name>" : item.BudgetCode1,
                        Value = item.BudgetCodeID.ToString()
                    }));
                    break;
                }
                case "Classmarks":
                {
                    listValues.AddRange(db.Classmarks.OrderBy(a => a.Classmark1).Select(item => new SelectListItem
                    {
                        Text = string.IsNullOrEmpty(item.Classmark1) ? "<No name>" : item.Classmark1,
                        Value = item.ClassmarkID.ToString()
                    }));
                    break;
                }
                case "CommMethodTypes":
                {
                    listValues.AddRange(db.CommMethodTypes.OrderBy(a => a.Method).Select(item => new SelectListItem
                    {
                        Text = string.IsNullOrEmpty(item.Method) ? "<No name>" : item.Method,
                        Value = item.MethodID.ToString()
                    }));
                    break;
                }
                case "Departments":
                {
                    listValues.AddRange(db.Departments.OrderBy(a => a.Department1).Select(item => new SelectListItem
                    {
                        Text = string.IsNullOrEmpty(item.Department1) ? "<No name>" : item.Department1,
                        Value = item.DepartmentID.ToString()
                    }));
                    break;
                }
                case "Frequencies":
                {
                    listValues.AddRange(db.Frequencies.OrderBy(a => a.Frequency1).Select(item => new SelectListItem
                    {
                        Text = string.IsNullOrEmpty(item.Frequency1) ? "<No name>" : item.Frequency1,
                        Value = item.FrequencyID.ToString()
                    }));
                    break;
                }
                case "Keywords":
                {
                    listValues.AddRange(db.Keywords.OrderBy(a => a.KeywordTerm).Select(item => new SelectListItem
                    {
                        Text = string.IsNullOrEmpty(item.KeywordTerm) ? "<No name>" : item.KeywordTerm,
                        Value = item.KeywordID.ToString()
                    }));
                    break;
                }
                case "Languages":
                {
                    listValues.AddRange(db.Languages.OrderBy(a => a.Language1).Select(item => new SelectListItem
                    {
                        Text = string.IsNullOrEmpty(item.Language1) ? "<No name>" : item.Language1,
                        Value = item.LanguageID.ToString()
                    }));
                    break;
                }
                case "Locations":
                {
                    listValues.AddRange(db.Locations.OrderBy(x => x.ParentLocationID == null ? x.Location1 : x.ParentLocation.Location1 + x.Location1).Select(item => new SelectListItem
                    {
                        Text = (item.ParentLocation == null ? item.Location1 : item.ParentLocation.Location1 + ": " + item.Location1),
                        Value = item.LocationID.ToString()
                    }));
                    break;
                }
                case "LoanTypes":
                {
                    listValues.AddRange(db.LoanTypes.OrderBy(a => a.LoanTypeName).Select(item => new SelectListItem
                    {
                        Text = string.IsNullOrEmpty(item.LoanTypeName) ? "<No name>" : item.LoanTypeName,
                        Value = item.LoanTypeID.ToString()
                    }));
                    break;
                }
                case "MediaTypes":
                {
                    listValues.AddRange(db.MediaTypes.OrderBy(a => a.Media).Select(item => new SelectListItem
                    {
                        Text = string.IsNullOrEmpty(item.Media) ? "<No name>" : item.Media,
                        Value = item.MediaID.ToString()
                    }));
                    break;
                }
                case "OrderCategories":
                {
                    listValues.AddRange(db.OrderCategories.OrderBy(a => a.OrderCategory1).Select(item => new SelectListItem
                    {
                        Text = string.IsNullOrEmpty(item.OrderCategory1) ? "<No name>" : item.OrderCategory1,
                        Value = item.OrderCategoryID.ToString()
                    }));
                    break;
                }
                case "Publishers":
                {
                    listValues.AddRange(db.Publishers.OrderBy(a => a.PublisherName).Select(item => new SelectListItem
                    {
                        Text = string.IsNullOrEmpty(item.PublisherName) ? "<No name>" : item.PublisherName,
                        Value = item.PublisherID.ToString()
                    }));
                    break;
                }
                case "StatusTypes":
                {
                    listValues.AddRange(db.StatusTypes.OrderBy(a => a.Status).Select(item => new SelectListItem
                    {
                        Text = string.IsNullOrEmpty(item.Status) ? "<No name>" : item.Status,
                        Value = item.StatusID.ToString()
                    }));
                    break;
                }
                case "Suppliers":
                {
                    listValues.AddRange(db.Suppliers.OrderBy(a => a.SupplierName).Select(item => new SelectListItem
                    {
                        Text = string.IsNullOrEmpty(item.SupplierName) ? "<No name>" : item.SupplierName,
                        Value = item.SupplierID.ToString()
                    }));
                    break;
                }
            }
            
            return Json(new
            {
                success = true,
                AuthorityListData = listValues
            });
        }

        public ActionResult ConfirmAuthListMerger(MergeAuthorityListViewModel viewModel)
        {
            var authorityList = viewModel.AuthorityList;
            if (authorityList == null)
            {
                return HttpNotFound();
            }

            TempData["mergeAuthorityListViewModel"] = viewModel;

            var gcvm = new GenericConfirmationViewModel
            {
                PostConfirmController = "AdminTools",
                PostConfirmAction = "PostUpdateMergeAuthorityList",
                ConfirmationText = "You are about to merge Authority List values. Are you sure you want to continue?",
                ConfirmButtonText = "Do merger",
                ConfirmButtonClass = "btn-danger",
                CancelButtonText = "Cancel",
                HeaderText = "Update/Merge Authority List Values?",
                Glyphicon = "glyphicon-ok"
            };
            return PartialView("_GenericConfirmation", gcvm);
        }

        public ActionResult PostUpdateMergeAuthorityList(MergeAuthorityListViewModel viewModel)
        {
            //MergeAuthorityListViewModel viewModel = (MergeAuthorityListViewModel)TempData["mergeAuthorityListViewModel"];

            return RedirectToAction("Index", viewModel.AuthorityList);
        }
    }
}