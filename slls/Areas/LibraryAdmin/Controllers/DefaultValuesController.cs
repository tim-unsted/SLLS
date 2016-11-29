using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using slls.Controllers;
using slls.DAO;
using slls.Models;
using slls.Utils.Helpers;
using slls.ViewModels;
using Westwind.Globalization;

namespace slls.Areas.LibraryAdmin
{
    public class DefaultValuesController : AdminBaseController
    {
        private readonly DbEntities _db = new DbEntities();

        // GET: LibraryAdmin/DefaultValues
        public ActionResult Index(string tableName)
        {
            //Get only the default items that match the user's roles
            //Note: GetRoles() returns the Role.Name, not the Role.ID as expected!
            var userRoles = Roles.GetUserRoles();
            List<DefaultValue> defaultValues;

            if (Roles.IsBaileyAdmin())
            {
                defaultValues = _db.DefaultValues.ToList();
            }
            else
            {
                defaultValues = (from m in _db.DefaultValues
                                 where userRoles.Contains(m.Role)
                                 select m).ToList();
            }
            
            if (!string.IsNullOrEmpty(tableName))
            {
                defaultValues = defaultValues.Where(v => v.TableName == tableName).ToList();
            }

            var viewModel = new DefaultValueIndexViewModel {DefaultValues = new List<DefaultValueEditViewModel>()};

            foreach (var defaultValue in defaultValues)
            {
                var currentValueString = "";
                var fieldName = "";
                switch (defaultValue.ChildTableName.ToLower())
                {
                    case "classmarks":
                    {
                        var value = defaultValue;
                        fieldName = DbRes.T("Classmarks.Classmark", "FieldDisplayName");
                        var firstOrDefault = _db.Classmarks.Find(value.DefaultValueId).Classmark1;
                        currentValueString = firstOrDefault ?? value.DefaultValueId.ToString();
                        break;
                    }
                    case "frequencies":
                    {
                        var value = defaultValue;
                        fieldName = DbRes.T("Frequency.Frequency", "FieldDisplayName");
                        var firstOrDefault = _db.Frequencies.Find(value.DefaultValueId).Frequency1;
                        currentValueString = firstOrDefault ?? value.DefaultValueId.ToString();
                        break;
                    }
                    case "languages":
                    {
                        var value = defaultValue;
                        fieldName = DbRes.T("Languages.Language", "FieldDisplayName");
                        var firstOrDefault = _db.Languages.Find(value.DefaultValueId).Language1;
                        currentValueString = firstOrDefault ?? value.DefaultValueId.ToString();
                        break;
                    }
                    case "publishers":
                    {
                        var value = defaultValue;
                        fieldName = DbRes.T("Titles.Publisher", "FieldDisplayName");
                        var firstOrDefault = _db.Publishers.Find(value.DefaultValueId).PublisherName;
                        currentValueString = firstOrDefault ?? value.DefaultValueId.ToString();
                        break;
                    }
                    case "mediatypes":
                    {
                        var value = defaultValue;
                        fieldName = DbRes.T("MediaTypes.Media_Type", "FieldDisplayName");
                        var firstOrDefault = _db.MediaTypes.Find(value.DefaultValueId).Media;
                        currentValueString = firstOrDefault ?? value.DefaultValueId.ToString();
                        break;
                    }
                    case "budgetcodes":
                    {
                        var value = defaultValue;
                        fieldName = DbRes.T("Orders.Budget_Code", "FieldDisplayName");
                        var firstOrDefault = _db.BudgetCodes.Find(value.DefaultValueId).BudgetCode1;
                        currentValueString = firstOrDefault ?? value.DefaultValueId.ToString();
                        break;
                    }
                    case "suppliers":
                    {
                        var value = defaultValue;
                        fieldName = DbRes.T("Orders.Supplier", "FieldDisplayName");
                        var firstOrDefault = _db.Suppliers.Find(value.DefaultValueId).SupplierName;
                        currentValueString = firstOrDefault ?? value.DefaultValueId.ToString();
                        break;
                    }
                    case "ordercategories":
                    {
                        var value = defaultValue;
                        fieldName = DbRes.T("Orders.Category", "FieldDisplayName");
                        var firstOrDefault = _db.OrderCategories.Find(value.DefaultValueId).OrderCategory1;
                        currentValueString = firstOrDefault ?? value.DefaultValueId.ToString();
                        break;
                    }
                    case "locations":
                    {
                        var value = defaultValue;
                        fieldName = DbRes.T("Locations.Location", "FieldDisplayName");
                        var firstOrDefault = _db.Locations.Find(value.DefaultValueId).Location1;
                        currentValueString = firstOrDefault ?? value.DefaultValueId.ToString();
                        break;
                    }
                    case "statustypes":
                    {
                        var value = defaultValue;
                        fieldName = DbRes.T("Copies.Status", "FieldDisplayName");
                        var firstOrDefault = _db.StatusTypes.Find(value.DefaultValueId).Status;
                        currentValueString = firstOrDefault ?? value.DefaultValueId.ToString();
                        break;
                    }
                    case "circulationmessages":
                    {
                        var value = defaultValue;
                        fieldName = DbRes.T("Circulation.Circulation_Slip_Message", "FieldDisplayName");
                        var firstOrDefault = _db.CirculationMessages.Find(value.DefaultValueId).CirculationMsg;
                        currentValueString = firstOrDefault ?? value.DefaultValueId.ToString();
                        break;
                    }
                    case "departments":
                    {
                        var value = defaultValue;
                        fieldName = DbRes.T("Departments.Department", "FieldDisplayName");
                        var firstOrDefault = _db.Departments.Find(value.DefaultValueId).Department1;
                        currentValueString = firstOrDefault ?? value.DefaultValueId.ToString();
                        break;
                    }
                    case "loantypes":
                    {
                        var value = defaultValue;
                        fieldName = DbRes.T("MediaTypes.Loan_Type", "FieldDisplayName");
                        var firstOrDefault = _db.LoanTypes.Find(value.DefaultValueId).LoanTypeName;
                        currentValueString = firstOrDefault ?? value.DefaultValueId.ToString();
                        break;
                    }
                    default:
                    {
                        var value = defaultValue;
                        currentValueString = value.DefaultValueId.ToString();
                        break;
                    }
                }

                var row = new DefaultValueEditViewModel
                {
                    DefaultId = defaultValue.DefaultId,
                    ChildTableName = defaultValue.ChildTableName,
                    DefaultValueId = defaultValue.DefaultValueId,
                    DefaultValue = currentValueString,
                    FieldName = fieldName,
                    TableName = DbRes.T(defaultValue.TableName, "EntityType")
                };
                viewModel.DefaultValues.Add(row);
            }
            
            ViewBag.Title = "Default Values";
            return View(viewModel);
        }
        
        // GET: LibraryAdmin/DefaultValues/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DefaultValue defaultValue = _db.DefaultValues.Find(id);
            if (defaultValue == null)
            {
                return HttpNotFound();
            }

            var viewModel = new DefaultValueEditViewModel()
            {
                DefaultId = defaultValue.DefaultId,
                TableName = DbRes.T(defaultValue.TableName, "EntityType"),
                //FieldName = defaultValue.FieldName,
                ChildTableName = defaultValue.ChildTableName,
                DefaultValueId = defaultValue.DefaultValueId
            };

            var childTableName = defaultValue.ChildTableName;
            var authorityList = new Dictionary<int, string>();
            var fieldName = "";

            switch (childTableName.ToLower())
            {
                case "classmarks":
                {
                    fieldName = DbRes.T("Classmarks.Classmark", "FieldDisplayName");
                    var list = CacheProvider.GetAll<Classmark>("classmarks").OrderBy(x => x.Classmark1).ToList();
                    foreach (var item in list)
                    {
                        authorityList.Add(item.ClassmarkID, item.Classmark1);
                    }
                    break;
                }
                case "frequencies":
                {
                    fieldName = DbRes.T("Titles.Frequency", "FieldDisplayName");
                    var list = CacheProvider.GetAll<Frequency>("frequencies").OrderBy(x => x.Frequency1).ToList();
                    foreach (var item in list)
                    {
                        authorityList.Add(item.FrequencyID, item.Frequency1);
                    }
                    break;
                }
                case "languages":
                {
                    fieldName = DbRes.T("Languages.Language", "FieldDisplayName");
                    var list = CacheProvider.GetAll<Language>("languages").OrderBy(x => x.Language1).ToList();
                    foreach (var item in list)
                    {
                        authorityList.Add(item.LanguageID, item.Language1);
                    }
                    break;
                }
                case "mediatypes":
                {
                    fieldName = DbRes.T("MediaTypes.Media_Type", "FieldDisplayName");
                    var list = CacheProvider.GetAll<MediaType>("mediatypes").OrderBy(x => x.Media).ToList();
                    foreach (var item in list)
                    {
                        authorityList.Add(item.MediaID, item.Media);
                    }
                    break;
                }
                case "publishers":
                {
                    fieldName = DbRes.T("Titles.Publisher", "FieldDisplayName");
                    var list = CacheProvider.GetAll<Publisher>("publishers").OrderBy(x => x.PublisherName).ToList();
                    foreach (var item in list)
                    {
                        authorityList.Add(item.PublisherID, item.PublisherName);
                    }
                    break;
                }
                case "budgetcodes":
                {
                    fieldName = DbRes.T("Orders.Budget_Code", "FieldDisplayName");
                    var list = _db.BudgetCodes.OrderBy(x => x.BudgetCode1).ToList();
                    foreach (var item in list)
                    {
                        authorityList.Add(item.BudgetCodeID, item.BudgetCode1);
                    }
                    break;
                }
                case "suppliers":
                {
                    fieldName = DbRes.T("Orders.Supplier", "FieldDisplayName");
                    var list = _db.Suppliers.OrderBy(x => x.SupplierName).ToList();
                    foreach (var item in list)
                    {
                        authorityList.Add(item.SupplierID, item.SupplierName);
                    }
                    break;
                }
                case "ordercategories":
                {
                    fieldName = DbRes.T("Orders.Category", "FieldDisplayName");
                    var list = _db.OrderCategories.OrderBy(x => x.OrderCategory1).ToList();
                    foreach (var item in list)
                    {
                        authorityList.Add(item.OrderCategoryID, item.OrderCategory1);
                    }
                    break;
                }
                case "locations":
                {
                    fieldName = DbRes.T("Locations.Location", "FieldDisplayName");
                    var list = _db.Locations.OrderBy(x => x.Location1).ToList();
                    foreach (var item in list)
                    {
                        authorityList.Add(item.LocationID, item.Location1);
                    }
                    break;
                }
                case "statustypes":
                {
                    fieldName = DbRes.T("Copies.Status", "FieldDisplayName");
                    var list = _db.StatusTypes.OrderBy(x => x.Status).ToList();
                    foreach (var item in list)
                    {
                        authorityList.Add(item.StatusID, item.Status);
                    }
                    break;
                }
                case "circulationmessages":
                {
                    fieldName = DbRes.T("Circulation.Circulation_Slip_Message", "FieldDisplayName");
                    var list = _db.CirculationMessages.OrderBy(x => x.CirculationMsg).ToList();
                    foreach (var item in list)
                    {
                        authorityList.Add(item.CirculationMsgID, item.CirculationMsg);
                    }
                    break;
                }
                case "departments":
                {
                    fieldName = DbRes.T("Departments.Department", "FieldDisplayName");
                    var list = _db.Departments.OrderBy(x => x.Department1).ToList();
                    foreach (var item in list)
                    {
                        authorityList.Add(item.DepartmentID, item.Department1);
                    }
                    break;
                }
                case "loantypes":
                {
                    fieldName = DbRes.T("MediaTypes.Loan_Type", "FieldDisplayName");
                    var list = _db.LoanTypes.OrderBy(x => x.LoanTypeName).ToList();
                    foreach (var item in list)
                    {
                        authorityList.Add(item.LoanTypeID, item.LoanTypeName);
                    }
                    break;
                }
                default:
                {
                    var classmarks = _db.Classmarks.ToList();
                    foreach (var classmark in classmarks)
                    {
                        authorityList.Add(classmark.ClassmarkID, classmark.Classmark1);
                    }
                    break;
                }
            }

            viewModel.FieldName = fieldName.Replace("Default","");
            ViewData["AuthorityList"] = authorityList;
            ViewBag.Title = "Edit Default " + fieldName;
            return PartialView(viewModel);
        }

        // POST: LibraryAdmin/DefaultValues/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DefaultId,TableName,FieldName,ChildTableName,DefaultValueId")] DefaultValueEditViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var defaultValue = _db.DefaultValues.Find(viewModel.DefaultId);
                defaultValue.DefaultValueId = viewModel.DefaultValueId;

                _db.Entry(defaultValue).State = EntityState.Modified;
                _db.SaveChanges();
                //return RedirectToAction("Index");
                return Json(new { success = true });
            }

            ViewBag.Title = "Edit Default Value";
            return View(viewModel);
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
