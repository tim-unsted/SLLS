using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls.Expressions;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using slls.DAO;
using slls.Models;
using Westwind.Globalization;

namespace slls.Utils.Helpers
{
    public class SelectListHelper
    {
        
        public static IEnumerable<SelectListItem> CatalogueReportsList(int id = 0, string msg = "Select a Report ", bool addDefault = true)
        {
            DbEntities db = new DbEntities();
            var catalogueReportsList = new List<SelectListItem>();

            //Add a default value if required ...
            if (addDefault)
            {
                catalogueReportsList.Add(new SelectListItem
                {
                    Text = msg,
                    Value = "0"
                });
            };                      

            //Add the actual report types ...
            foreach (var report in db.ReportTypes.Where(r => r.ReportArea == "Catalogue").OrderBy(r => r.FriendlyName))
            {
                catalogueReportsList.Add(new SelectListItem
                {
                    Text = report.FriendlyName,
                    Value = report.ReportID.ToString()
                });
            }

            return catalogueReportsList.Select(l => new SelectListItem { Selected = (l.Value == id.ToString()), Text = l.Text, Value = l.Value });
        }

        public static IEnumerable<SelectListItem> FinanceReportsList(int id = 0, string msg = "Select a Report ", bool addDefault = true)
        {
            DbEntities db = new DbEntities();
            var financeReportsList = new List<SelectListItem>();

            //Add a default value if required ...
            if (addDefault)
            {
                financeReportsList.Add(new SelectListItem
                {
                    Text = msg,
                    Value = "0"
                });
            };

            //Add the actual report types ...
            foreach (var report in db.ReportTypes.Where(r => r.ReportArea == "Finance").OrderBy(r => r.FriendlyName))
            {
                financeReportsList.Add(new SelectListItem
                {
                    Text = report.FriendlyName,
                    Value = report.ReportID.ToString()
                });
            }

            return financeReportsList.Select(l => new SelectListItem { Selected = (l.Value == id.ToString()), Text = l.Text, Value = l.Value });
        }
        
        public static IEnumerable<SelectListItem> AccountYearsList(int id = 0, string msg = "Select an ", bool addDefault = true, bool addNew = true)
        {
            DbEntities db = new DbEntities();
            var accountYearsList = new List<SelectListItem>();

            //Add a default value if required ...
            if (addDefault)
            {
                accountYearsList.Add(new SelectListItem
                {
                    Text = msg + DbRes.T("AccountYears.Account_Year", "FieldDisplayName"),
                    Value = "0"
                });
            };

            //Add New
            if (addNew)
            {
                accountYearsList.Add(new SelectListItem
                {
                    Text = "[Add New " + DbRes.T("AccountYears.Account_Year", "FieldDisplayName") + "]",
                    Value = "-1"
                });
            }

            //Add the actual account years ...
            accountYearsList.AddRange(CacheProvider.GetAll<AccountYear>("accountyears").OrderBy(a => a.AccountYear1).Select(item => new SelectListItem
            {
                Text = string.IsNullOrEmpty(item.AccountYear1) ? "<No name>" : item.AccountYear1, Value = item.AccountYearID.ToString()
            }));

            return accountYearsList.Select(l => new SelectListItem { Selected = (l.Value == id.ToString()), Text = l.Text, Value = l.Value });
        }

        public static IEnumerable<SelectListItem> ActivityTypesList(int id = 0, string msg = "Select an ", bool addDefault = true, bool addNew = true)
        {
            DbEntities db = new DbEntities();
            var activityTypesList = new List<SelectListItem>();

            //Add a default value if required ...
            if (addDefault)
            {
                activityTypesList.Add(new SelectListItem
                {
                    Text = msg + DbRes.T("ActivityTypes.Activity_Type", "FieldDisplayName"),
                    Value = "0"
                });
            };

            //Add New
            if (addNew)
            {
                activityTypesList.Add(new SelectListItem
                {
                    Text = "[Add New " + DbRes.T("ActivityTypes.Activity_Type", "FieldDisplayName") + "]",
                    Value = "-1"
                });
            }

            //Add the actual activity types ...
            foreach (var item in CacheProvider.GetAll<ActivityType>("activitytypes").OrderBy(a => a.Activity))
            {
                activityTypesList.Add(new SelectListItem
                {
                    Text = string.IsNullOrEmpty(item.Activity) ? "<No name>" : item.Activity,
                    Value = item.ActivityCode.ToString()
                });
            }

            return activityTypesList.Select(l => new SelectListItem { Selected = (l.Value == id.ToString()), Text = l.Text, Value = l.Value });
        }

        public static IEnumerable<SelectListItem> BudgetCodesList(int id = 0, string msg = "Select a ", bool addDefault = true, bool addNew = true)
        {
            DbEntities db = new DbEntities();
            var budgetCodesList = new List<SelectListItem>();

            //Add a default value if required ...
            if (addDefault)
            {
                budgetCodesList.Add(new SelectListItem
                {
                    Text = msg + DbRes.T("BudgetCode.Budget_Code", "FieldDisplayName"),
                    Value = "0"
                });
            };

            //Add New
            if (addNew)
            {
                budgetCodesList.Add(new SelectListItem
                {
                    Text = "[Add New " + DbRes.T("BudgetCode.Budget_Code", "FieldDisplayName") + "]",
                    Value = "-1"
                });
            }

            //Add the actual budget codes ...
            foreach (var item in CacheProvider.GetAll<BudgetCode>("budgetcodes").OrderBy(x => x.BudgetCode1))
            {
                budgetCodesList.Add(new SelectListItem
                {
                    Text = item.BudgetCode1 ?? "",
                    Value = item.BudgetCodeID.ToString()
                });
            }

            return budgetCodesList.Select(l => new SelectListItem { Selected = (l.Value == id.ToString()), Text = l.Text, Value = l.Value });
        }


        public static IEnumerable<SelectListItem> OrderCategoriesList(int id = 0, string msg = "Select a ", bool addDefault = true, bool addNew = true)
        {
            DbEntities db = new DbEntities();
            var expenditureTypesList = new List<SelectListItem>();

            //Add a default value if required ...
            if (addDefault)
            {
                expenditureTypesList.Add(new SelectListItem
                {
                    Text = msg, // + DbRes.T("OrderCategories.Expenditure_Type", "FieldDisplayName"),
                    Value = "0"
                });
            };

            //Add New
            if (addNew)
            {
                expenditureTypesList.Add(new SelectListItem
                {
                    Text = "[Add New " + DbRes.T("OrderCategories.Expenditure_Type", "FieldDisplayName") + "]",
                    Value = "-1"
                });
            }

            //Add the actual Order Categories ...
            foreach (var item in CacheProvider.GetAll<OrderCategory>("ordercategories").OrderBy(x => x.OrderCategory1))
            {
                expenditureTypesList.Add(new SelectListItem
                {
                    Text = item.OrderCategory1 ?? "",
                    Value = item.OrderCategoryID.ToString()
                });
            }

            return expenditureTypesList.Select(l => new SelectListItem { Selected = (l.Value == id.ToString()), Text = l.Text, Value = l.Value });
        }

        
        public static IEnumerable<SelectListItem> AuthorsList(int id = 0, string msg = "Select an ", bool addDefault = true, bool addNew = true)
        {
            DbEntities db = new DbEntities();
            var authorList = new List<SelectListItem>();

            //Add a default value if required ...
            if (addDefault)
            {
                authorList.Add(new SelectListItem
                {
                    Text = msg + DbRes.T("Authors.Author", "FieldDisplayName"),
                    Value = "-1"
                });
            };

            //Add New
            if (addNew)
            {
                authorList.Add(new SelectListItem
                {
                    Text = "[Add New " + DbRes.T("Authors.Author", "FieldDisplayName") + "]",
                    Value = "-1"
                });
            }

            //Add the actual author names ...
            var authors = CacheProvider.GetAll<Author>("authors");
            authorList.AddRange(authors.OrderBy(a => a.DisplayName).Select(item => new SelectListItem
            {
                Text = item.DisplayName ?? "", Value = item.AuthorID.ToString()
            }));

            return authorList.Select(l => new SelectListItem { Selected = (l.Value == id.ToString()), Text = l.Text, Value = l.Value });
        }

        public static IEnumerable<SelectListItem> EditorsList(int id = 0, string msg = "Select an ", bool addDefault = true, bool addNew = true)
        {
            DbEntities db = new DbEntities();
            var authorList = new List<SelectListItem>();

            //Add a default value if required ...
            if (addDefault)
            {
                authorList.Add(new SelectListItem
                {
                    Text = msg + DbRes.T("Authors.Editor", "FieldDisplayName"),
                    Value = "0"
                });
            };

            //Add New
            if (addNew)
            {
                authorList.Add(new SelectListItem
                {
                    Text = "[Add New " + DbRes.T("Authors.Editor", "FieldDisplayName") + "]",
                    Value = "-1"
                });
            }

            //Add the actual author names ...
            var authors = CacheProvider.GetAll<Author>("authors");
            authorList.AddRange(authors.OrderBy(a => a.DisplayName).Select(item => new SelectListItem
            {
                Text = item.DisplayName ?? "", 
                Value = item.AuthorID.ToString()
            }));

            return authorList.Select(l => new SelectListItem { Selected = (l.Value == id.ToString()), Text = l.Text, Value = l.Value });
        }

        //[OutputCache(Duration = 7200, VaryByParam = "*")]
        public static IEnumerable<SelectListItem> TitlesList(int id = 0, bool addDefault = true, string msg = "Select a ")
        {
            DbEntities db = new DbEntities();
            var titlesList = new List<SelectListItem>();

            //Add a default item ...
            if (addDefault)
            {
                titlesList.Add(new SelectListItem
                {
                    Text = msg + DbRes.T("Titles.Title", "FieldDisplayName"),
                    Value = "0"
                });
            }

            //Add the actual author names ...
            foreach (var item in db.Titles.Where(t => t.Deleted == false).OrderBy(t => t.Title1.Substring(t.NonFilingChars)))
            {
                titlesList.Add(new SelectListItem
                {
                    Text = string.IsNullOrEmpty(item.Title1) ? "<empty title>" : StringHelper.Truncate(item.Title1, 100),
                    Value = item.TitleID.ToString()
                });
            }

            return titlesList.Select(l => new SelectListItem { Selected = (l.Value == id.ToString()), Text = l.Text, Value = l.Value });
        }

        public static IEnumerable<SelectListItem> TitlesWithCopies(int id = 0, bool addDefault = true, string msg = "Select a ")
        {
            DbEntities db = new DbEntities();
            var titlesList = new List<SelectListItem>();

            //Add a default item ...
            if (addDefault)
            {
                titlesList.Add(new SelectListItem
                {
                    Text = msg + DbRes.T("Titles.Title", "FieldDisplayName"),
                    Value = "0"
                });
            }

            foreach (var item in db.Titles.Where(t => t.Copies.Any()).OrderBy(t => t.Title1.Substring(t.NonFilingChars)))
            {
                titlesList.Add(new SelectListItem
                {
                    Text = string.IsNullOrEmpty(item.Title1) ? "<empty title>" : StringHelper.Truncate(item.Title1, 100),
                    Value = item.TitleID.ToString()
                });
            }

            return titlesList.Select(l => new SelectListItem { Selected = (l.Value == id.ToString()), Text = l.Text, Value = l.Value });
        }

        public static IEnumerable<SelectListItem> TitlesWithVolumes(int id = 0, bool addDefault = true, string msg = "Select a ")
        {
            DbEntities db = new DbEntities();
            var titlesList = new List<SelectListItem>();

            //Add a default item ...
            if (addDefault)
            {
                titlesList.Add(new SelectListItem
                {
                    Text = msg + DbRes.T("Titles.Title", "FieldDisplayName"),
                    Value = "0"
                });
            }

            var copiesWithVolumes = db.Copies.Where(c => c.Volumes.Any()).Select(c => c.TitleID);
            var titlesWithVolumes = db.Titles.Where(t => copiesWithVolumes.Contains(t.TitleID)).OrderBy(t => t.Title1.Substring(t.NonFilingChars));
            foreach (var item in titlesWithVolumes)
            {
                titlesList.Add(new SelectListItem
                {
                    Text = string.IsNullOrEmpty(item.Title1) ? "<empty title>" : StringHelper.Truncate(item.Title1, 100),
                    Value = item.TitleID.ToString()
                });
            }

            return titlesList.Select(l => new SelectListItem { Selected = (l.Value == id.ToString()), Text = l.Text, Value = l.Value });
        }

        [OutputCache(Duration = 7200, VaryByParam = "*")]
        public static IEnumerable<SelectListItem> AllCopiesList(int id = 0, bool addDefault = true, bool addAll = false, string msg = "Select a Copy")
        {
            DbEntities db = new DbEntities();
            var copiesList = new List<SelectListItem>();

            //Add a default item ...
            if (addDefault)
            {
                copiesList.Add(new SelectListItem
                {
                    Text = msg,
                    Value = "0"
                });
            }

            //Add a default item ...
            if (addAll)
            {
                copiesList.Add(new SelectListItem
                {
                    Text = "[Show All]",
                    Value = "-1"
                });
            }

            foreach (var item in db.Copies.OrderBy(c => c.Title.Title1.Substring(c.Title.NonFilingChars)).ThenBy(c => c.CopyNumber))
            {
                copiesList.Add(new SelectListItem
                {
                    Text = string.IsNullOrEmpty(item.Title.Title1) ? "<empty title>" : StringHelper.Truncate(item.Title.Title1, 100) + " - Copy: " + item.CopyNumber,
                    Value = item.CopyID.ToString()
                });
            }

            return copiesList.Select(l => new SelectListItem { Selected = (l.Value == id.ToString()), Text = l.Text, Value = l.Value });
        }


        public static IEnumerable<SelectListItem> MediaTypeList(int id = 0, string msg = "Select a ", bool addDefault = true, bool addNew = true)
        {
            DbEntities db = new DbEntities();
            var mediaList = new List<SelectListItem>();

            //Add a default value if required ...
            if (addDefault)
            {
                mediaList.Add(new SelectListItem
                {
                    Text = msg + DbRes.T("MediaTypes.Media_Type", "FieldDisplayName"),
                    Value = "0"
                });
            };

            //Add New
            if (addNew)
            {
                mediaList.Add(new SelectListItem
                {
                    Text = "[Add New " + DbRes.T("MediaTypes.Media_Type", "FieldDisplayName") + "]",
                    Value = "-1"
                });
            }

            //Add the actual media types ...
            foreach (var item in CacheProvider.GetAll<MediaType>("mediatypes").Where(m => m.Deleted == false).OrderBy(m => m.Media))
            {
                mediaList.Add(new SelectListItem
                {
                    //Text = string.IsNullOrEmpty(item.Media) ? "<no name>" : item.Media,
                    Text = item.Media ?? "",
                    Value = item.MediaID.ToString()
                });
            }

            return mediaList.Select(l => new SelectListItem { Selected = (l.Value == id.ToString()), Text = l.Text, Value = l.Value });
        }

        public static IEnumerable<SelectListItem> ClassmarkList(int id = 0, string msg = "Select a ", bool addDefault = true, bool addNew = true)
        {
            DbEntities db = new DbEntities();
            var classmarkList = new List<SelectListItem>();

            //Add a default value if required ...
            if (addDefault)
            {
                classmarkList.Add(new SelectListItem
                {
                    Text = msg + DbRes.T("Classmarks.Classmark", "FieldDisplayName"),
                    Value = "0"
                });
            };

            //Add New
            if (addNew)
            {
                classmarkList.Add(new SelectListItem
                {
                    Text = "[Add New " + DbRes.T("Classmarks.Classmark", "FieldDisplayName") + "]",
                    Value = "-1"
                });
            }

            //Add the actual classmarks ...
            foreach (var item in CacheProvider.GetAll<Classmark>("classmarks").Where(c => c.Deleted == false).OrderBy(c => c.Classmark1))
            {
                classmarkList.Add(new SelectListItem
                {
                    //Text = string.IsNullOrEmpty(item.Classmark1) ? "<no name>" : item.Classmark1,
                    Text = item.Classmark1 ?? "",
                    Value = item.ClassmarkID.ToString()
                });
            }

            return classmarkList.Select(l => new SelectListItem { Selected = (l.Value == id.ToString()), Text = l.Text, Value = l.Value });
        }

        public static IEnumerable<SelectListItem> PublisherList(int id = 0, string msg = "Select a ", bool addDefault = true, bool addNew = true)
        {
            DbEntities db = new DbEntities();
            var publisherList = new List<SelectListItem>();

           //Add a default value if required ...
            if (addDefault)
            {
                publisherList.Add(new SelectListItem
                {
                    Text = msg + DbRes.T("Publishers.Publisher", "FieldDisplayName"),
                    Value = "0"
                });
            };

            //Add New
            if (addNew)
            {
                publisherList.Add(new SelectListItem
                {
                    Text = "[Add New " + DbRes.T("Publishers.Publisher", "FieldDisplayName") + "]",
                    Value = "-1"
                });
            }

            //Add the actual Publishers ...
            foreach (var item in CacheProvider.GetAll<Publisher>("publishers").Where(p => p.Deleted == false).OrderBy(x => x.PublisherName))
            {
                publisherList.Add(new SelectListItem
                {
                    //Text = string.IsNullOrEmpty(item.PublisherName) ? "<no name>" : item.PublisherName,
                    Text = item.PublisherName ?? "",
                    Value = item.PublisherID.ToString()
                });
            }

            return publisherList.Select(l => new SelectListItem { Selected = (l.Value == id.ToString()), Text = l.Text, Value = l.Value });
        }

        public static IEnumerable<SelectListItem> FrequencyList(int id = 0, string msg = "Select a ", bool addDefault = true, bool addNew = true)
        {
            DbEntities db = new DbEntities();
            var frequencyList = new List<SelectListItem>();

            if(addDefault)
            {
                frequencyList.Add(new SelectListItem
                {
                    Text = msg + DbRes.T("Frequency.Frequency", "FieldDisplayName"),
                    Value = "0"
                });
            }

            //Add New
            if (addNew)
            {
                frequencyList.Add(new SelectListItem
                {
                    Text = "[Add New " + DbRes.T("Frequency.Frequency", "FieldDisplayName") + "]",
                    Value = "-1"
                });
            }

            //Add the actual frequencies ...
            foreach (var item in CacheProvider.GetAll<Frequency>("frequencies").Where(f => f.Deleted == false).OrderBy(x => x.Frequency1))
            {
                frequencyList.Add(new SelectListItem
                {
                    //Text = string.IsNullOrEmpty(item.Frequency1) ? "<no name>" : item.Frequency1,
                    Text = item.Frequency1 ?? "",
                    Value = item.FrequencyID.ToString()
                });
            }

            return frequencyList.Select(l => new SelectListItem { Selected = (l.Value == id.ToString()), Text = l.Text, Value = l.Value });
        }

        public static IEnumerable<SelectListItem> LanguageList(int id = 0, string msg = "Select a ", bool addDefault = true, bool addNew = true)
        {
            DbEntities db = new DbEntities();
            var languageList = new List<SelectListItem>();

            //Add a default value if required ...
            if (addDefault)
            {
                languageList.Add(new SelectListItem
                {
                    Text = msg + DbRes.T("Languages.Language", "FieldDisplayName"),
                    Value = "0"
                });
            };

            //Add New
            if (addNew)
            {
                languageList.Add(new SelectListItem
                {
                    Text = "[Add New " + DbRes.T("Languages.Language", "FieldDisplayName") + "]",
                    Value = "-1"
                });
            }

            //Add the actual languages ...
            foreach (var item in CacheProvider.GetAll<Language>("languages").Where(l => l.Deleted == false).OrderBy(x => x.Language1))
            {
                languageList.Add(new SelectListItem
                {
                    //Text = string.IsNullOrEmpty(item.Language1) ? "<no name>" : item.Language1,
                    Text = item.Language1 ?? "",
                    Value = item.LanguageID.ToString()
                });
            }

            return languageList.Select(l => new SelectListItem { Selected = (l.Value == id.ToString()), Text = l.Text, Value = l.Value });
        }


        public static IEnumerable<SelectListItem> CopacLanguageList(string id = "")
        {
            DbEntities db = new DbEntities();
            var languageList = new List<SelectListItem>();

            //Add a default value ...
            languageList.Add(new SelectListItem
                {
                    Text = "",
                    Value = ""
                });
            languageList.Add(new SelectListItem
            {
                Text = "English",
                Value = "English"
            });

            //Add the actual languages ...
            foreach (var item in db.CopacLanguages.OrderBy(x => x.Language))
            {
                languageList.Add(new SelectListItem
                {
                    Text = item.Language, //string.IsNullOrEmpty(item.Language) ? "" : item.Language,
                    Value = item.Language //string.IsNullOrEmpty(item.Language) ? "" : item.Language
                });
            }

            return languageList.Select(l => new SelectListItem { Selected = (l.Value == id), Text = l.Text, Value = l.Value });
        }


        public static IEnumerable<SelectListItem> CopacLibraryList(string id = "")
        {
            DbEntities db = new DbEntities();
            var libraryList = new List<SelectListItem>();

            //Add a default value ...
            libraryList.Add(new SelectListItem
            {
                Text = "",
                Value = ""
            });
            
            //Add the actual libraries ...
            foreach (var item in db.CopacLibraries.OrderBy(x => x.Library))
            {
                libraryList.Add(new SelectListItem
                {
                    Text = item.Library,
                    Value = item.Library
                });
            }

            return libraryList.Select(l => new SelectListItem { Selected = (l.Value == id), Text = l.Text, Value = l.Value });
        }



        public static IEnumerable<SelectListItem> KeywordList(int selected = 0, string msg = "Select a ", bool addDefault = true, bool addNew = true, int avoid = 0)
        {
            DbEntities db = new DbEntities();
            var keywordList = new List<SelectListItem>();
            var allKeywords = CacheProvider.GetAll<Keyword>("keywords")
                .Where(k => k.KeywordID != avoid).ToList();
            
            //Add a default value if required ...
            if (allKeywords.Count > 1)
            {
                if (addDefault)
                {
                    keywordList.Add(new SelectListItem
                    {
                        Text = msg + DbRes.T("Keywords.Keyword", "FieldDisplayName"),
                        Value = "0"
                    });
                }
            }
            else
            {
                selected = allKeywords.FirstOrDefault().KeywordID;
            }

            //Add New
            if (addNew)
            {
                keywordList.Add(new SelectListItem
                {
                    Text = "[Add New " + DbRes.T("Keywords.Keyword", "FieldDisplayName") + "]",
                    Value = "-1"
                });
            }

            //Add the actual keywords ...
            foreach (var item in allKeywords.OrderBy(x => x.KeywordTerm))
            {
                keywordList.Add(new SelectListItem
                {
                    Text = item.KeywordTerm ?? "",
                    Value = item.KeywordID.ToString()
                });
            }

            return keywordList.Select(l => new SelectListItem { Selected = (l.Value == selected.ToString()), Text = l.Text, Value = l.Value });
        }

        public static IEnumerable<SelectListItem> CirculationMessageList(int id = 0, string msg = "Select a ", bool addDefault = true, bool addNew = true)
        {
            DbEntities db = new DbEntities();
            var circulationMsgList = new List<SelectListItem>();

            //Add a default value if required ...
            if (addDefault)
            {
                circulationMsgList.Add(new SelectListItem
                {
                    Text = msg + DbRes.T("Circulation.Circulation_Slip_Message", "FieldDisplayName"),
                    Value = "0"
                });
            };

            //Add New
            if (addNew)
            {
                circulationMsgList.Add(new SelectListItem
                {
                    Text = "[Add New " + DbRes.T("Circulation.Circulation_Slip_Message", "FieldDisplayName") + "]",
                    Value = "-1"
                });
            }

            //Add the actual Circulation slip messages ...
            circulationMsgList.AddRange(db.CirculationMessages.OrderBy(x => x.CirculationMsg).Select(item => new SelectListItem
            {
                Text = item.CirculationMsg ?? "",
                Value = item.CirculationMsgID.ToString()
            }));

            return circulationMsgList.Select(l => new SelectListItem { Selected = (l.Value == id.ToString()), Text = l.Text, Value = l.Value });
        }

        public static IEnumerable<SelectListItem> LocationList(int id = 0, string msg = "Select a ", bool addDefault = true, bool addNew = true)
        {
            DbEntities db = new DbEntities();
            var locationList = new List<SelectListItem>();

            //Add a default value if required ...
            if (addDefault)
            {
                locationList.Add(new SelectListItem
                {
                    Text = msg + DbRes.T("Locations.Location", "FieldDisplayName"),
                    Value = "0"
                });
            };

            //Add New
            if (addNew)
            {
                locationList.Add(new SelectListItem
                {
                    Text = "[Add New " + DbRes.T("Locations.Location", "FieldDisplayName") + "]",
                    Value = "-1"
                });
            }

            //Add the actual locations ...
            var locations = CacheProvider.GetAll<Location>("locations");
            locationList.AddRange(locations.OrderBy(x => x.Location1).Select(item => new SelectListItem
            {
                Text = item.Location1 ?? "",
                Value = item.LocationID.ToString()
            }));

            return locationList.Select(l => new SelectListItem { Selected = (l.Value == id.ToString()), Text = l.Text, Value = l.Value });
        }

        public static IEnumerable<SelectListItem> ParentLocationList(int id = -1)
        {
            DbEntities db = new DbEntities();
            var parentLocationList = new List<SelectListItem>();

            //Add an empty (To parent) location ...
            parentLocationList.Add(new SelectListItem
            {
                Text = "[All locations]",
                Value = "-1"
            });

            parentLocationList.Add(new SelectListItem
            {
                Text = "[Top-level locations only]",
                Value = "0"
            });

            //Add the actual locations ...
            var locations = CacheProvider.GetAll<Location>("locations");
            parentLocationList.AddRange(locations.Where(l => l.SubLocations.Any()).OrderBy(x => x.Location1).Select(item => new SelectListItem
            {
                Text = item.Location1 ?? "",
                Value = item.LocationID.ToString()
            }));

            return parentLocationList.Select(l => new SelectListItem { Selected = (l.Value == id.ToString()), Text = l.Text, Value = l.Value });
        }


        public static IEnumerable<SelectListItem> OfficeLocationList(int id = 0, string msg = "Select a ", bool addDefault = true, string separator = ": ")
        {
            DbEntities db = new DbEntities();
            var locationList = new List<SelectListItem>();

            //Add a default value if required ...
            if (addDefault)
            {
                locationList.Add(new SelectListItem
                {
                    Text = msg + DbRes.T("Locations.Location", "FieldDisplayName"),
                    Value = "0"
                });
            };

            //Add the actual locations ...
            var locations = CacheProvider.GetAll<Location>("locations");
            locationList.AddRange(locations.OrderBy(x => x.ParentLocationID == null ? x.Location1 : x.ParentLocation.Location1 + x.Location1).Select(item => new SelectListItem
            {
                //Text = (item.ParentLocation == null ? item.Location1 : "\x230A" + "\xA0" + "\xA0" + "\xA0" + "\xA0" + item.Location1),
                //Text = (item.ParentLocation == null ? item.Location1 : "\xA0" + item.ParentLocation.Location1 + " \x2192 " + item.Location1),
                Text = (item.ParentLocation == null ? item.Location1 : item.ParentLocation.Location1 + separator + item.Location1),
                Value = item.LocationID.ToString()
            }));

            return locationList.Select(l => new SelectListItem { Selected = (l.Value == id.ToString()), Text = l.Text, Value = l.Value });
        }


        public static IEnumerable<SelectListItem> OfficeList(int id = 0, string msg = "Select an ", bool addDefault = true)
        {
            DbEntities db = new DbEntities();
            var officeList = new List<SelectListItem>();
            
            //Add a default value if required ...
            if (addDefault)
            {
                officeList.Add(new SelectListItem
                {
                    Text = msg + DbRes.T("Office.Office", "FieldDisplayName"),
                    Value = "0"
                });
            };

            var locations = CacheProvider.GetAll<Location>("locations");
            var offices =locations.Where(l => l.ParentLocationID == null && l.SubLocations.Any());

            ////Add the actual offices ...
            foreach (var item in offices.OrderBy(o => o.Location1))
            {
                officeList.Add(new SelectListItem
                {
                    //Text = string.IsNullOrEmpty(item.Location1) ? "<no name>" : txtInfo.ToTitleCase(item.Location1.ToLower),
                    Text = string.IsNullOrEmpty(item.Location1) ? "<no name>" : item.Location1,
                    Value = item.LocationID.ToString()
                });
            }

            //Add the actual locations ...
            //officeList.AddRange(db.Locations.Where(l => l.ParentLocation.Any())).Select(item => new SelectListItem
            //{
            //    Text = string.IsNullOrEmpty(item.Location1) ? "<no name>" : item.Location1,
            //    Value = item.LocationID.ToString()
            //});

            return officeList.Select(l => new SelectListItem { Selected = (l.Value == id.ToString()), Text = l.Text, Value = l.Value });
        }


        [OutputCache(Duration = 7200, VaryByParam = "*")]
        public static IEnumerable<SelectListItem> StatusList(int id = 0, string msg = "Select a ", bool addDefault = true, bool addNew = true)
        {
            DbEntities db = new DbEntities();
            var statusList = new List<SelectListItem>();

            //Add a default value if required ...
            if (addDefault)
            {
                statusList.Add(new SelectListItem
                {
                    Text = msg + DbRes.T("StatusTypes.Status_Type", "FieldDisplayName"),
                    Value = "0"
                });
            };

            //Add New
            if (addNew)
            {
                statusList.Add(new SelectListItem
                {
                    Text = "[Add New " + DbRes.T("StatusTypes.Status_Type", "FieldDisplayName") + "]",
                    Value = "-1"
                });
            }
            
            //Add the actual status types ...
            var statusTypes = CacheProvider.GetAll<StatusType>("statustypes");
            statusList.AddRange(statusTypes.OrderBy(x => x.Status).Select(item => new SelectListItem
            {
                //Text = string.IsNullOrEmpty(item.Status) ? "<no name>" : item.Status,
                Text = item.Status ?? "",
                Value = item.StatusID.ToString()
            }));

            return statusList.Select(l => new SelectListItem { Selected = (l.Value == id.ToString()), Text = l.Text, Value = l.Value });
        }

        public static IEnumerable<SelectListItem> SupplierList(int id = 0, string msg = "Select a ")
        {
            DbEntities db = new DbEntities();
            var supplierList = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = msg + DbRes.T("Suppliers.Supplier", "FieldDisplayName"),
                    Value = "0"
                }
            };

            //Add the actual Publishers ...
            var suppliers = CacheProvider.GetAll<Supplier>("suppliers");
            foreach (var item in suppliers.OrderBy(x => x.SupplierName))
            {
                supplierList.Add(new SelectListItem
                {
                    Text = string.IsNullOrEmpty(item.SupplierName) ? "<no name>" : item.SupplierName,
                    Value = item.SupplierID.ToString()
                });
            }

            return supplierList.Select(x => new SelectListItem { Selected = (x.Value == id.ToString()), Text = x.Text, Value = x.Value });
        }

        public static IEnumerable<SelectListItem> OrderCategoryList(int id = 0, string msg = "Select an ")
        {
            DbEntities db = new DbEntities();
            var orderCategoryList = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = msg + DbRes.T("Orders.Category", "FieldDisplayName"),
                    Value = "0"
                }
            };

            //Add the actual Publishers ...
            var orderCategories = CacheProvider.GetAll<OrderCategory>("ordercategories");
            foreach (var item in orderCategories.OrderBy(x => x.OrderCategory1))
            {
                orderCategoryList.Add(new SelectListItem
                {
                    Text = string.IsNullOrEmpty(item.OrderCategory1) ? "<no name>" : item.OrderCategory1,
                    Value = item.OrderCategoryID.ToString()
                });
            }

            return orderCategoryList.Select(x => new SelectListItem { Selected = (x.Value == id.ToString()), Text = x.Text, Value = x.Value });
        }

        public static IEnumerable<SelectListItem> LibraryUserList(string id = "", bool addDefault = true, string msg = "Select a ")
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var libraryUserList = new List<SelectListItem>();

            //Add a default user if required ...
            if (addDefault)
            {
                libraryUserList.Add(new SelectListItem
                {
                    Text = msg + DbRes.T("Users.User", "FieldDisplayName"),
                    Value = "0"
                });
            }

            //Add the actual users ...
            foreach (var item in db.Users.Where(u => u.CanDelete && u.IsLive && u.Lastname != null).OrderBy(x => x.Lastname).ThenBy(x => x.Firstname))
            {
                libraryUserList.Add(new SelectListItem
                {
                    Text = item.FullnameRev,
                    Value = item.Id.ToString()
                });
            }

            return libraryUserList.Select(x => new SelectListItem { Selected = (x.Value == id), Text = x.Text, Value = x.Value });
        }

        public static IEnumerable<SelectListItem> OrdersList(int id = 0, string msg = "Select an ", string filter = "")
        {
            DbEntities db = new DbEntities();
            var ordersList = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = msg + DbRes.T("Orders.Order", "FieldDisplayName"),
                    Value = "0"
                }
            };

            var title = "Item:";
            var supplier = "Supplier:";
            var orderNo = "Order:";
            var orderDate = "Order Date:";
            title = title.PadRight(50, '\u00A0');
            supplier = supplier.PadRight(40, '\u00A0');
            orderNo = orderNo.PadRight(20, '\u00A0');

            ordersList.Add(new SelectListItem
            {
                Text = string.Format("{0} {1} {2} {3}", title, supplier, orderNo, orderDate),
                Value = "-1"
            });

            //Add the actual orders ...
            var validOrders = db.OrderDetails.Where(o => o.Title.Title1 != null);

            if (filter == "outstanding")
            {
                validOrders = validOrders.Where(o => o.ReceivedDate == null);
            }
            
            if (filter == "noinvoice")
            {
                validOrders = validOrders.Where(o => o.InvoiceDate == null && o.InvoiceRef == null);
            }
            
            foreach (var item in validOrders.OrderBy(x => x.Title.Title1.Substring(x.Title.NonFilingChars)).ThenByDescending(x => x.OrderID))
            {
                ordersList.Add(new SelectListItem
                {
                    Text = item.SelectOrder,
                    Value = item.OrderID.ToString()
                });
            }

            return ordersList.Select(l => new SelectListItem { Selected = (l.Value == id.ToString()), Text = l.Text, Value = l.Value });
        }


        //Provides a list of suppliers based on the data provided ...
        public static IEnumerable<SelectListItem> SuppliersListCustom(IOrderedQueryable<SupplierList> suppliers, bool addDefault = true, bool showAll = true )
        {
            var supplierList = new List<SelectListItem>();

            if (addDefault)
            {
                supplierList.Add(new SelectListItem
                {
                    Text = "Select a " + DbRes.T("Suppliers.Supplier", "FieldDisplayName"),
                    Value = "0"
                });
            }
            if (showAll)
            {
                supplierList.Add(new SelectListItem
                {
                    Text = "All " + DbRes.T("Suppliers", "EntityType"),
                    Value = "-1"
                });
            }

            supplierList.AddRange(suppliers.Select(item => new SelectListItem
            {
                Text = item.SupplierName + " (" + item.Count.ToString() + ")",
                Value = item.SupplierId.ToString()
            }));

            return supplierList;
        }

        public static IEnumerable<SelectListItem> CommTypesList(int id = 0)
        {
            DbEntities db = new DbEntities();
            var commTypesList = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select a " + DbRes.T("SupplierCommTypes.Communication_Type", "FieldDisplayName"),
                    Value = "0"
                }
            };

            //Add the actual author names ...
            foreach (var item in db.CommMethodTypes.OrderBy(x => x.Method))
            {
                commTypesList.Add(new SelectListItem
                {
                    Text = string.IsNullOrEmpty(item.Method) ? "<no name>" : item.Method,
                    Value = item.MethodID.ToString()
                });
            }

            return commTypesList.Select(l => new SelectListItem { Selected = (l.Value == id.ToString()), Text = l.Text, Value = l.Value });
        }
        
        public static IEnumerable<SelectListItem> LoanTypes(int id = 0, string msg = "", bool addDefault = true, bool addNew = true)
        {
            DbEntities db = new DbEntities();
            var list = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select a " + DbRes.T("LoanTypes.Loan_Type", "FieldDisplayName"),
                    Value = "0"
                }
            };

            //Add New ?
            if (addNew)
            {
                list.Add(new SelectListItem
                {
                    Text = "[Add New " + DbRes.T("LoanTypes.Loan_Type", "FieldDisplayName") + "]",
                    Value = "-1"
                });
            }

            //Add the actual loan types ...
            var loanTypes = CacheProvider.GetAll<LoanType>("loantypes");
            foreach (var item in loanTypes.OrderBy(x => x.LoanTypeName))
            {
                list.Add(new SelectListItem
                {
                    Text = string.IsNullOrEmpty(item.LoanTypeName) ? "<no name>" : item.LoanTypeName,
                    Value = item.LoanTypeID.ToString()
                });
            }

            return list.Select(l => new SelectListItem { Selected = (l.Value == id.ToString()), Text = l.Text, Value = l.Value });
        }


        public static IEnumerable<SelectListItem> ContactsList(int id = 0, string msg = "Select a ")
        {
            DbEntities db = new DbEntities();
            var contactList = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = msg + DbRes.T("SupplierPeople.Contact", "FieldDisplayName"),
                    Value = "0"
                }
            };

            //Add the actual contacts ...
            foreach (var item in db.SupplierPeoples.OrderBy(x => x.Surname).ThenBy(x => x.Firstname))
            {
                contactList.Add(new SelectListItem
                {
                    Text = string.Format("{0}, {1}", item.Surname, item.Firstname),
                    Value = item.ContactID.ToString()
                });
            }

            return contactList.Select(l => new SelectListItem { Selected = (l.Value == id.ToString()), Text = l.Text, Value = l.Value });
        }


        public static IEnumerable<SelectListItem> MenuList(int id = 0, bool addDefault = true, string msg = "Select a Menu to Edit ...")
        {
            DbEntities db = new DbEntities();
            var menusList = new List<SelectListItem>();
            var userRoles = Roles.GetUserRoles();

            //Create a list of menu areas the current use has access to ...
            var allowedAreas = new List<string> { "Menu", "", "OPAC", "MyLibrary", "Home" };

            //Add extra roles and menu area if allowed ...
            if (userRoles.Contains("Bailey Admin") || userRoles.Contains("Bailey Developer"))
            {
                allowedAreas.Add("LibraryAdmin");
                allowedAreas.Add("Config");
                allowedAreas.Add("BS");
            }

            //Add a default item ...
            if (addDefault)
            {
                menusList.Add(new SelectListItem
                {
                    Text = msg,
                    Value = "0"
                });
            }

            //Add the actual author names ...
            foreach (var item in db.Menus.Where(m => m.ParentID == -999 && allowedAreas.Contains(m.MenuArea)).OrderBy(m => m.Title))
            {
                menusList.Add(new SelectListItem
                {
                    Text = item.Title,
                    Value = item.Title
                });
            }

            return menusList.Select(l => new SelectListItem { Selected = (l.Value == id.ToString()), Text = l.Text, Value = l.Value });
        }

        public static IEnumerable<SelectListItem> SearchFieldsList(string id = "all", string msg = "Any available field", bool addDefault = true, string scope = "opac")
        {
            DbEntities db = new DbEntities();
            var searchFieldsList = new List<SelectListItem>();

            //Add a default value if required ...
            if (addDefault)
            {
                searchFieldsList.Add(new SelectListItem
                {
                    Text = msg,
                    Value = "all"
                });
            };
            
            //Add the actual search fields ...
            var allSearchFields = CacheProvider.GetAll<SearchField>("searchfields");
            foreach (var item in allSearchFields.Where(x => x.Enabled && x.Scope.Contains(scope)).OrderBy(x => x.Position).ThenBy(x => x.FieldName))
            {
                searchFieldsList.Add(new SelectListItem
                {
                    Text = item.DisplayName ?? "",
                    Value = item.FieldName
                });
            }

            return searchFieldsList.Select(l => new SelectListItem { Selected = (l.Value == id), Text = l.Text, Value = l.Value });
        }

        
        public static IEnumerable<SelectListItem> OpacResultsOrderBy(string selected = "title.asc")
        {
            DbEntities db = new DbEntities();
            var orderByList = new List<SelectListItem>();
            
            //Add the actual search order types ...
            var allSearchOrderTypes = CacheProvider.GetAll<SearchOrderType>("searchordertypes");
            foreach (var item in allSearchOrderTypes.Where(x => x.Scope.Contains('o')).OrderBy(x => x.Display).ThenBy(x => x.OrderTypeFriendly))
            {
                orderByList.Add(new SelectListItem
                {
                    Text = item.OrderTypeFriendly ?? "",
                    Value = item.OrderType
                });
            }

            return orderByList.Select(list => new SelectListItem { Selected = (list.Value == selected), Text = list.Text, Value = list.Value });
        }


        public static IEnumerable<SelectListItem> NewTitlesOrderBy(string selected = "commenced.desc")
        {
            DbEntities db = new DbEntities();
            var orderByList = new List<SelectListItem>();

            //Add the actual 'New Titles' order types ...
            var allSearchOrderTypes = CacheProvider.GetAll<SearchOrderType>("searchordertypes");
            foreach (var item in allSearchOrderTypes.Where(x => x.Scope.Contains('n')).OrderBy(x => x.Display).ThenBy(x => x.OrderTypeFriendly))
            {
                orderByList.Add(new SelectListItem
                {
                    Text = item.OrderTypeFriendly ?? "",
                    Value = item.OrderType
                });
            }

            return orderByList.Select(list => new SelectListItem { Selected = (list.Value == selected), Text = list.Text, Value = list.Value });
        }


        public static IEnumerable<SelectListItem> VersionReleases(int id = 0, bool addDefault = true, string msg = "Select a Release ...")
        {
            DbEntities db = new DbEntities();
            var list = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = msg,
                    Value = "0"
                }
            };
            
            //Add the releases ...
            var releases = db.ReleaseHeaders;
            foreach (var item in releases.OrderBy(r => r.ReleaseId))
            {
                list.Add(new SelectListItem
                {
                    Text = string.IsNullOrEmpty(item.ReleaseName) ? item.ReleaseNumber + " - " + item.ReleaseDate : item.ReleaseName + " (" + item.ReleaseNumber + ") - " + item.ReleaseDate,
                    Value = item.ReleaseId.ToString()
                });
            }

            return list.Select(l => new SelectListItem { Selected = (l.Value == id.ToString()), Text = l.Text, Value = l.Value });
        }
    }
}