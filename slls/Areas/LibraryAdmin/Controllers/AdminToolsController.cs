using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoCat.ViewModels;
using slls.DAO;
using slls.Models;
using slls.Utils.Helpers;
using slls.ViewModels;
using Westwind.Globalization;

namespace slls.Areas.LibraryAdmin
{
    public class AdminToolsController : AdminBaseController
    {
        private readonly DbEntities _db = new DbEntities();
        
        public ActionResult UpdateMergeAuthorityList(string list = "")
        {
            var authorityLists = new Dictionary<string, string>();

            if (Roles.IsUserInRole("Catalogue Admin") || Roles.IsUserInRole("Bailey Admin"))
            {
                authorityLists.Add("Authors", DbRes.T("Authors","EntityType"));
                authorityLists.Add("Classmarks", DbRes.T("Classmarks","EntityType"));
                authorityLists.Add("Frequencies", DbRes.T("Frequencies","EntityType"));
                authorityLists.Add("Keywords", DbRes.T("Keywords","EntityType"));
                authorityLists.Add("Languages", DbRes.T("Languages","EntityType"));
                authorityLists.Add("MediaTypes", DbRes.T("MediaTypes","EntityType"));
                authorityLists.Add("Publishers", DbRes.T("Publishers","EntityType"));
                authorityLists.Add("StatusTypes", DbRes.T("StatusTypes","EntityType"));
            }

            if (Roles.IsUserInRole("Finance Admin") || Roles.IsUserInRole("Bailey Admin"))
            {
                authorityLists.Add("AccountYears", DbRes.T("AccountYears", "EntityType"));
                authorityLists.Add("ActivityTypes", DbRes.T("ActivityTypes", "EntityType"));
                authorityLists.Add("BudgetCodes", DbRes.T("BudgetCodes", "EntityType"));
                authorityLists.Add("CommMethodTypes", DbRes.T("CommMethodTypes", "EntityType"));
                authorityLists.Add("OrderCategories", DbRes.T("OrderCategories", "EntityType"));
                authorityLists.Add("Suppliers", DbRes.T("Suppliers", "EntityType"));
            }

            if (Roles.IsUserInRole("Loans Admin") || Roles.IsUserInRole("Bailey Admin"))
            {
                authorityLists.Add("LoanTypes", DbRes.T("LoanTypes","EntityType"));
            }

            if (Roles.IsUserInRole("Users Admin") || Roles.IsUserInRole("Bailey Admin"))
            {
                authorityLists.Add("Departments", DbRes.T("Departments", "EntityType"));
            }

            if (Roles.IsUserInRole("Users Admin") || Roles.IsUserInRole("Catalogue Admin") || Roles.IsUserInRole("Bailey Admin"))
            {
                authorityLists.Add("Locations", DbRes.T("Locations","EntityType"));
            }
            
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
            ViewData["AuthorityLists"] = authorityLists.OrderBy(pair => pair.Value);
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
        
        public ActionResult PostUpdateMergeAuthorityList(MergeAuthorityListViewModel viewModel)
        {
            var authorityList = viewModel.AuthorityList;
            var selectedIds = viewModel.SelectedIds;
            var newId = viewModel.NewId;
            var newValue = viewModel.NewValue;
            var sql = "";

            switch (authorityList)
            {
                case "AccountYears":
                {
                    if (newId == 0 && string.IsNullOrEmpty(newValue) == false)
                    {
                        var existingAccountYear = _db.AccountYears.Where(x => x.AccountYear1 == newValue);
                        if (existingAccountYear != null)
                        {
                            newId = existingAccountYear.FirstOrDefault().AccountYearID;
                        }
                        var newAccountYear = new AccountYear
                        {
                            AccountYear1 = newValue,
                            YearStartDate = DateTime.Today,
                            YearEndDate = DateTime.Today.AddYears(1),
                            CanUpdate = true,
                            CanDelete = true,
                            InputDate = DateTime.Now
                        };
                        _db.AccountYears.Add(newAccountYear);
                        _db.SaveChanges();
                        newId = newAccountYear.AccountYearID;
                        CacheProvider.RemoveCache("accountyears");
                    }

                    if (newId != 0)
                    {
                        foreach (var accountYearId in selectedIds)
                        {
                            sql = "UPDATE dbo.Orders SET AccountYearID = " + newId + " WHERE AccountYearID = " +
                                  accountYearId;
                            _db.Database.ExecuteSqlCommand(sql); 
                        }
                    }

                    break;
                }
                case "ActivityTypes":
                {
                    if (newId == 0 && string.IsNullOrEmpty(newValue) == false)
                    {
                        var newActivityType = new ActivityType()
                        {
                            Activity = newValue,
                            CanUpdate = true,
                            CanDelete = true,
                            InputDate = DateTime.Now
                        };
                        _db.ActivityTypes.Add(newActivityType);
                        _db.SaveChanges();
                        newId = newActivityType.ActivityCode;
                        CacheProvider.RemoveCache("activitytypes");
                    }

                    if (newId != 0)
                    {
                        foreach (var activityTypeId in selectedIds)
                        {
                            sql = "UPDATE dbo.SupplierAddresses SET ActivityCode = " + newId + " WHERE ActivityCode = " +
                                  activityTypeId;
                            _db.Database.ExecuteSqlCommand(sql);
                        }
                    }

                    break;
                }
                case "Authors":
                {
                    if (newId == 0 && string.IsNullOrEmpty(newValue) == false)
                    {
                        var newAuthor = new Author()
                        {
                            DisplayName = newValue,
                            AuthType = "P",
                            InputDate = DateTime.Now
                        };
                        _db.Authors.Add(newAuthor);
                        _db.SaveChanges();
                        newId = newAuthor.AuthorID;
                        CacheProvider.RemoveCache("authors");
                    }

                    if (newId != 0)
                    {
                        foreach (var authorId in selectedIds)
                        {
                            sql = "UPDATE dbo.TitleAuthors SET AuthorID = " + newId + " WHERE AuthorID = " +
                                  authorId;
                            _db.Database.ExecuteSqlCommand(sql);
                            sql = "UPDATE dbo.TitleEditors SET AuthorID = " + newId + " WHERE AuthorID = " +
                                  authorId;
                            _db.Database.ExecuteSqlCommand(sql);
                        }
                    }

                    break;
                }
                case "BudgetCodes":
                {
                    if (newId == 0 && string.IsNullOrEmpty(newValue) == false)
                    {
                        var newBudgetCode = new BudgetCode()
                        {
                            BudgetCode1 = newValue,
                            CanUpdate = true,
                            CanDelete = true,
                            InputDate = DateTime.Now
                        };
                        _db.BudgetCodes.Add(newBudgetCode);
                        _db.SaveChanges();
                        newId = newBudgetCode.BudgetCodeID;
                        CacheProvider.RemoveCache("budgetcodes");
                    }

                    if (newId != 0)
                    {
                        foreach (var budgetCodeId in selectedIds)
                        {
                            sql = "UPDATE dbo.Orders SET BudgetCodeID = " + newId + " WHERE BudgetCodeID = " +
                                  budgetCodeId;
                            _db.Database.ExecuteSqlCommand(sql);
                        }
                    }

                    break;
                }
                case "Classmarks":
                {
                    if (newId == 0 && string.IsNullOrEmpty(newValue) == false)
                    {
                        var existingValue = from c in _db.Classmarks where c.Classmark1 == newValue select c.ClassmarkID;
                        newId = existingValue.FirstOrDefault();

                        if (newId == 0)
                        {
                            var newClassmark = new Classmark()
                            {
                                Classmark1 = newValue,
                                CanUpdate = true,
                                CanDelete = true,
                                InputDate = DateTime.Now
                            };
                            _db.Classmarks.Add(newClassmark);
                            _db.SaveChanges();
                            newId = newClassmark.ClassmarkID;
                            CacheProvider.RemoveCache("classmarks");
                        }
                    }

                    if (newId != 0)
                    {
                        foreach (var classmarkId in selectedIds)
                        {
                            sql = "UPDATE dbo.Titles SET ClassmarkID = " + newId + " WHERE ClassmarkID = " +
                                  classmarkId;
                            _db.Database.ExecuteSqlCommand(sql);
                        }
                    }

                    break;
                }
                case "CommMethodTypes":
                {
                    if (newId == 0 && string.IsNullOrEmpty(newValue) == false)
                    {
                        var existingValue = from c in _db.CommMethodTypes where c.Method == newValue select c.MethodID;
                        newId = existingValue.FirstOrDefault();

                        if (newId == 0)
                        {
                            var newMethod = new CommMethodType()
                            {
                                Method = newValue,
                                CanUpdate = true,
                                CanDelete = true,
                                InputDate = DateTime.Now
                            };
                            _db.CommMethodTypes.Add(newMethod);
                            _db.SaveChanges();
                            newId = newMethod.MethodID;
                        }
                    }

                    if (newId != 0)
                    {
                        foreach (var methodId in selectedIds)
                        {
                            sql = "UPDATE dbo.SupplierPeopleComms SET MethodID = " + newId + " WHERE MethodID = " +
                                  methodId;
                            _db.Database.ExecuteSqlCommand(sql);
                        }
                    }

                    break;
                }
                case "Departments":
                {
                    if (newId == 0 && string.IsNullOrEmpty(newValue) == false)
                    {
                        var existingValue = from c in _db.Departments where c.Department1 == newValue select c.DepartmentID;
                        newId = existingValue.FirstOrDefault();

                        if (newId == 0)
                        {
                            var newDepartment = new Department()
                            {
                                Department1 = newValue,
                                CanUpdate = true,
                                CanDelete = true,
                                InputDate = DateTime.Now
                            };
                            _db.Departments.Add(newDepartment);
                            _db.SaveChanges();
                            newId = newDepartment.DepartmentID;
                            CacheProvider.RemoveCache("departments");
                        }
                    }

                    if (newId != 0)
                    {
                        foreach (var departmentId in selectedIds)
                        {
                            sql = "UPDATE dbo.AspNetUsers SET DepartmentId = " + newId + " WHERE DepartmentId = " +
                                  departmentId;
                            _db.Database.ExecuteSqlCommand(sql);
                        }
                    }

                    break;
                }
                case "Frequencies":
                {
                    if (newId == 0 && string.IsNullOrEmpty(newValue) == false)
                    {
                        var existingValue = from c in _db.Frequencies where c.Frequency1 == newValue select c.FrequencyID;
                        newId = existingValue.FirstOrDefault();

                        if (newId == 0)
                        {
                            var newFrequency = new Frequency()
                            {
                                Frequency1 = newValue,
                                CanUpdate = true,
                                CanDelete = true,
                                InputDate = DateTime.Now
                            };
                            _db.Frequencies.Add(newFrequency);
                            _db.SaveChanges();
                            newId = newFrequency.FrequencyID;
                            CacheProvider.RemoveCache("frequencies");
                        }
                    }

                    if (newId != 0)
                    {
                        foreach (var frequencyId in selectedIds)
                        {
                            sql = "UPDATE dbo.Titles SET FrequencyID = " + newId + " WHERE FrequencyID = " +
                                  frequencyId;
                            _db.Database.ExecuteSqlCommand(sql);
                        }
                    }

                    break;
                }
                case "Keywords":
                {
                    if (newId == 0 && string.IsNullOrEmpty(newValue) == false)
                    {
                        var existingValue = from c in _db.Keywords where c.KeywordTerm == newValue && c.ParentKeywordID == -1 select c.KeywordID;
                        newId = existingValue.FirstOrDefault();

                        if (newId == 0)
                        {
                            var newKeyword = new Keyword()
                            {
                                KeywordTerm = newValue,
                                ParentKeywordID = -1,
                                CanUpdate = true,
                                CanDelete = true,
                                InputDate = DateTime.Now
                            };
                            _db.Keywords.Add(newKeyword);
                            _db.SaveChanges();
                            newId = newKeyword.KeywordID;
                            CacheProvider.RemoveCache("keywords");
                        }
                    }

                    if (newId != 0)
                    {
                        foreach (var keywordId in selectedIds)
                        {
                            sql = "UPDATE dbo.SubjectIndex SET KeywordID = " + newId + " WHERE KeywordID = " +
                                  keywordId;
                            _db.Database.ExecuteSqlCommand(sql);
                        }
                    }

                    break;
                }
                case "Languages":
                {
                    if (newId == 0 && string.IsNullOrEmpty(newValue) == false)
                    {
                        var existingValue = from x in _db.Languages where x.Language1 == newValue select x.LanguageID;
                        newId = existingValue.FirstOrDefault();

                        if (newId == 0)
                        {
                            var newLanguage = new Language()
                            {
                                Language1 = newValue,
                                CanUpdate = true,
                                CanDelete = true,
                                InputDate = DateTime.Now
                            };
                            _db.Languages.Add(newLanguage);
                            _db.SaveChanges();
                            newId = newLanguage.LanguageID;
                            CacheProvider.RemoveCache("languages");
                        }
                    }

                    if (newId != 0)
                    {
                        foreach (var languageId in selectedIds)
                        {
                            sql = "UPDATE dbo.Titles SET LanguageID = " + newId + " WHERE LanguageID = " +
                                  languageId;
                            _db.Database.ExecuteSqlCommand(sql);
                        }
                    }

                    break;
                }
                case "Locations":
                {
                    if (newId == 0 && string.IsNullOrEmpty(newValue) == false)
                    {
                        var existingValue = from x in _db.Locations where x.Location1 == newValue && x.ParentLocationID == null select x.LocationID;
                        newId = existingValue.FirstOrDefault();

                        if (newId == 0)
                        {
                            var newLocation = new Location()
                            {
                                Location1 = newValue,
                                ParentLocationID = null,
                                CanUpdate = true,
                                CanDelete = true,
                                InputDate = DateTime.Now
                            };
                            _db.Locations.Add(newLocation);
                            _db.SaveChanges();
                            newId = newLocation.LocationID;
                            CacheProvider.RemoveCache("locations");
                        }
                    }

                    if (newId != 0)
                    {
                        foreach (var locationId in selectedIds)
                        {
                            sql = "UPDATE dbo.Copies SET LocationID = " + newId + " WHERE LocationID = " +
                                  locationId;
                            _db.Database.ExecuteSqlCommand(sql);
                            sql = "UPDATE dbo.AspNetUsers SET LocationID = " + newId + " WHERE LocationID = " +
                                  locationId;
                            _db.Database.ExecuteSqlCommand(sql);
                        }
                    }

                    break;
                }
                case "LoanTypes":
                {
                    if (newId == 0 && string.IsNullOrEmpty(newValue) == false)
                    {
                        var existingValue = from x in _db.LoanTypes where x.LoanTypeName == newValue select x.LoanTypeID;
                        newId = existingValue.FirstOrDefault();

                        if (newId == 0)
                        {
                            var newLoanType = new LoanType()
                            {
                                LoanTypeName = newValue,
                                RefOnly = false,
                                DailyFine = 0,
                                LengthDays = 9999,
                                MaxItems = 0,
                                CanUpdate = true,
                                CanDelete = true,
                                InputDate = DateTime.Now
                            };
                            _db.LoanTypes.Add(newLoanType);
                            _db.SaveChanges();
                            newId = newLoanType.LoanTypeID;
                            CacheProvider.RemoveCache("loantypes");
                        }
                    }

                    if (newId != 0)
                    {
                        foreach (var loantypeId in selectedIds)
                        {
                            sql = "UPDATE dbo.MediaTypes SET LoanTypeID = " + newId + " WHERE LoanTypeID = " +
                                  loantypeId;
                            _db.Database.ExecuteSqlCommand(sql);
                            sql = "UPDATE dbo.Volumes SET LoanTypeID = " + newId + " WHERE LoanTypeID = " +
                                  loantypeId;
                            _db.Database.ExecuteSqlCommand(sql);
                        }
                    }

                    break;
                }
                case "MediaTypes":
                {
                    if (newId == 0 && string.IsNullOrEmpty(newValue) == false)
                    {
                        var existingValue = from x in _db.MediaTypes where x.Media == newValue select x.MediaID;
                        newId = existingValue.FirstOrDefault();

                        if (newId == 0)
                        {
                            var newMediaType = new MediaType()
                            {
                                Media = newValue,
                                LoanTypeID = Utils.PublicFunctions.GetDefaultValue("MediaTypes", "LoanTypeID"),
                                CanUpdate = true,
                                CanDelete = true,
                                InputDate = DateTime.Now
                            };
                            _db.MediaTypes.Add(newMediaType);
                            _db.SaveChanges();
                            newId = newMediaType.MediaID;
                            CacheProvider.RemoveCache("mediatypes");
                        }
                    }

                    if (newId != 0)
                    {
                        foreach (var mediaId in selectedIds)
                        {
                            sql = "UPDATE dbo.Titles SET MediaID = " + newId + " WHERE MediaID = " +
                                  mediaId;
                            _db.Database.ExecuteSqlCommand(sql);
                        }
                    }

                    break;
                }
                case "OrderCategories":
                {
                    if (newId == 0 && string.IsNullOrEmpty(newValue) == false)
                    {
                        var existingValue = from x in _db.OrderCategories where x.OrderCategory1 == newValue select x.OrderCategoryID;
                        newId = existingValue.FirstOrDefault();

                        if (newId == 0)
                        {
                            var newOrderCategory = new OrderCategory()
                            {
                                OrderCategory1 = newValue,
                                Annual = false,
                                Sub = false,
                                CanUpdate = true,
                                CanDelete = true,
                                InputDate = DateTime.Now
                            };
                            _db.OrderCategories.Add(newOrderCategory);
                            _db.SaveChanges();
                            newId = newOrderCategory.OrderCategoryID;
                            CacheProvider.RemoveCache("ordercategories");
                        }
                    }

                    if (newId != 0)
                    {
                        foreach (var orderCategoryId in selectedIds)
                        {
                            sql = "UPDATE dbo.Orders SET OrderCategoryID = " + newId + " WHERE OrderCategoryID = " +
                                  orderCategoryId;
                            _db.Database.ExecuteSqlCommand(sql);
                        }
                    }

                    break;
                }
                case "Publishers":
                {
                    if (newId == 0 && string.IsNullOrEmpty(newValue) == false)
                    {
                        var existingValue = from x in _db.Publishers where x.PublisherName == newValue select x.PublisherID;
                        newId = existingValue.FirstOrDefault();

                        if (newId == 0)
                        {
                            var newPublisher = new Publisher()
                            {
                                PublisherName = newValue,
                                CanUpdate = true,
                                CanDelete = true,
                                InputDate = DateTime.Now
                            };
                            _db.Publishers.Add(newPublisher);
                            _db.SaveChanges();
                            newId = newPublisher.PublisherID;
                            CacheProvider.RemoveCache("publishers");
                        }
                    }

                    if (newId != 0)
                    {
                        foreach (var publisherId in selectedIds)
                        {
                            sql = "UPDATE dbo.Titles SET PublisherID = " + newId + " WHERE PublisherID = " +
                                  publisherId;
                            _db.Database.ExecuteSqlCommand(sql);
                        }
                    }

                    break;
                }
                case "StatusTypes":
                {
                    if (newId == 0 && string.IsNullOrEmpty(newValue) == false)
                    {
                        var existingValue = from x in _db.StatusTypes where x.Status == newValue select x.StatusID;
                        newId = existingValue.FirstOrDefault();

                        if (newId == 0)
                        {
                            var newStatusType = new StatusType()
                            {
                                Status = newValue,
                                Opac = true,
                                CanUpdate = true,
                                CanDelete = true,
                                InputDate = DateTime.Now
                            };
                            _db.StatusTypes.Add(newStatusType);
                            _db.SaveChanges();
                            newId = newStatusType.StatusID;
                            CacheProvider.RemoveCache("statustypes");
                        }
                    }

                    if (newId != 0)
                    {
                        foreach (var statustypeId in selectedIds)
                        {
                            sql = "UPDATE dbo.Copies SET StatusID = " + newId + " WHERE StatusID = " +
                                  statustypeId;
                            _db.Database.ExecuteSqlCommand(sql);
                        }
                    }

                    break;
                }
                case "Suppliers":
                {
                    if (newId == 0 && string.IsNullOrEmpty(newValue) == false)
                    {
                        var existingValue = from x in _db.Suppliers where x.SupplierName == newValue select x.SupplierID;
                        newId = existingValue.FirstOrDefault();

                        if (newId == 0)
                        {
                            var newSupplier = new Supplier()
                            {
                                SupplierName = newValue,
                                CanUpdate = true,
                                CanDelete = true,
                                InputDate = DateTime.Now
                            };
                            _db.Suppliers.Add(newSupplier);
                            _db.SaveChanges();
                            newId = newSupplier.SupplierID;
                            CacheProvider.RemoveCache("statustypes");
                        }
                    }

                    if (newId != 0)
                    {
                        foreach (var supplierId in selectedIds)
                        {
                            sql = "UPDATE dbo.Orders SET SupplierID = " + newId + " WHERE SupplierID = " +
                                  supplierId;
                            _db.Database.ExecuteSqlCommand(sql);
                            sql = "UPDATE dbo.SupplierAddresses SET SupplierID = " + newId + " WHERE SupplierID = " +
                                  supplierId;
                            _db.Database.ExecuteSqlCommand(sql);
                        }
                    }

                    break;
                }
            }

            TempData["SuccessDialogMsg"] = "The values in the " + viewModel.AuthorityList + " authority list have been successfully moved/merged.";
            return RedirectToAction("Index", viewModel.AuthorityList);
        }
    }
}