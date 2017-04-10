using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using slls.DAO;
using slls.Models;
using slls.Utils;
using slls.Utils.Helpers;
using slls.ViewModels;
using Westwind.Globalization;

namespace slls.Areas.LibraryAdmin
{
    public class SearchingController : AdminBaseController
    {
        private readonly DbEntities _db = new DbEntities();
        private readonly GenericRepository _repository;

        public SearchingController()
        {
            _repository = new GenericRepository(typeof (Title));
        }

        public Dictionary<string, string> GetSearchStyles()
        {
            return new Dictionary<string, string>
            {
                {"prefix", "Begins With (Prefix)"},
                {"suffix", "Ends With (Suffix)"},
                {"wholeword", "Whole Word (or Phrase) only"}
            };
        }

        // GET: LibraryAdmin/Searching
        [HttpGet]
        public ActionResult AdminSearch()
        {
            var viewModel = new SimpleSearchingViewModel()
            {
                LibraryStaff = Roles.IsLibraryStaff(),
                Area = "admin"
            };

            ViewData["SearchField"] = SelectListHelper.SearchFieldsList(scope: "catalogue");
            ViewBag.SearchTips = HelpTextHelper.GetHelpText("searchingtips");
            ViewBag.Title = "Search the Catalogue";
            return View(viewModel);
        }

        // GET: LibraryAdmin/FinanceSearch
        [HttpGet]
        public ActionResult FinanceSearch()
        {
            var viewModel = new FinanceSearchingViewModel()
            {
                LibraryStaff = Roles.IsLibraryStaff(),
                Area = "admin"
            };

            ViewData["SearchField"] = SelectListHelper.SearchFieldsList(scope: "finance");
            ViewBag.Title = "Search Finance Records";
            return View(viewModel);
        }

        //[HttpPost]
        public ActionResult AdminSearchResults(SimpleSearchingViewModel viewModel)
        {
            //Classmarks filter ...
            if (!viewModel.ClassmarksFilter.Any())
            {
                if (TempData["ClassmarksFilter"] != null)
                {
                    viewModel.ClassmarksFilter = (List<SelectClassmarkEditorViewModel>) TempData["ClassmarksFilter"];
                }
            }
            var selectedClassmarkIds = viewModel.GetSelectedClassmarkIds().ToList();

            //Media filter ...
            if (!viewModel.MediaFilter.Any())
            {
                if (TempData["MediaFilter"] != null)
                {
                    viewModel.MediaFilter = (List<SelectMediaEditorViewModel>) TempData["MediaFilter"];
                }
            }
            var selectedMediaIds = viewModel.GetSelectedMediaIds().ToList();

            //Publisher filter ...
            if (!viewModel.PublisherFilter.Any())
            {
                if (TempData["PublisherFilter"] != null)
                {
                    viewModel.PublisherFilter = (List<SelectPublisherEditorViewModel>) TempData["PublisherFilter"];
                }
            }
            var selectedPublisherIds = viewModel.GetSelectedPublisherIds().ToList();

            //Language filter ...
            if (!viewModel.LanguageFilter.Any())
            {
                if (TempData["LanguageFilter"] != null)
                {
                    viewModel.LanguageFilter = (List<SelectLanguageEditorViewModel>) TempData["LanguageFilter"];
                }
            }
            var selectedLanguageIds = viewModel.GetSelectedLanguageIds().ToList();

            //Keyword filter ...
            if (!viewModel.KeywordFilter.Any())
            {
                if (TempData["KeywordFilter"] != null)
                {
                    viewModel.KeywordFilter = (List<SelectKeywordEditorViewModel>) TempData["KeywordFilter"];
                }
            }
            var selectedKeywordIds = viewModel.GetSelectedKeywordIds().ToList();

            //Author filter ...
            if (!viewModel.AuthorFilter.Any())
            {
                if (TempData["AuthorFilter"] != null)
                {
                    viewModel.AuthorFilter = (List<SelectAuthorEditorViewModel>) TempData["AuthorFilter"];
                }
            }
            var selectedAuthorIds = viewModel.GetSelectedAuthorIds().ToList();

            //Do some work on the passes search string ...

            //1. Check if there is a hyphen before a word - this indicates an "AND NOT" search
            var stringSplitter = new string[] { " -", " not " };
            var searchString = viewModel.SearchString.Split(stringSplitter, StringSplitOptions.None);
            var q = searchString[0].Trim();
            var qIgnore = "";
            if (searchString.Length > 1)
            {
                for (int i = 1; i < searchString.Length; i++)
                {
                    qIgnore = qIgnore + " " + searchString[i];
                }
            }

            if (!string.IsNullOrEmpty(q))
            {
                var results = SearchService.DoFullTextSearch(q, qIgnore.Trim(), viewModel.SearchField == "all" ? "*" : viewModel.SearchField, "AdminFullTextSearch");
                viewModel.ResultsBeforeFilter = results;

                //Order the results ...
                results = results.OrderBy(r => r.Title1.Substring(r.NonFilingChars)).Distinct().ToList();

                if (selectedMediaIds.Any())
                {
                    viewModel.ResultsBeforeFilter = results;

                    results = results.Where(r => selectedMediaIds.Contains(r.MediaID)).ToList();
                    foreach (var mediaId in selectedMediaIds)
                    {
                        viewModel.Filters.Add(
                            CacheProvider.GetAll<MediaType>("mediatypes")
                                .FirstOrDefault(x => x.MediaID == mediaId)
                                .Media);
                    }
                }

                if (selectedClassmarkIds.Any())
                {
                    viewModel.ResultsBeforeFilter = results;

                    results = results.Where(r => selectedClassmarkIds.Contains(r.ClassmarkID)).ToList();
                    foreach (var classmarkId in selectedClassmarkIds)
                    {
                        viewModel.Filters.Add(
                            CacheProvider.GetAll<Classmark>("classmarks")
                                .FirstOrDefault(x => x.ClassmarkID == classmarkId)
                                .Classmark1);
                    }
                }

                if (selectedPublisherIds.Any())
                {
                    viewModel.ResultsBeforeFilter = results;

                    results = results.Where(r => selectedPublisherIds.Contains(r.PublisherID)).ToList();
                    foreach (var publisherId in selectedPublisherIds)
                    {
                        viewModel.Filters.Add(
                            CacheProvider.GetAll<Publisher>("publishers")
                                .FirstOrDefault(x => x.PublisherID == publisherId)
                                .PublisherName);
                    }
                }

                if (selectedLanguageIds.Any())
                {
                    viewModel.ResultsBeforeFilter = results;

                    results = results.Where(r => selectedLanguageIds.Contains(r.LanguageID)).ToList();
                    foreach (var languageId in selectedLanguageIds)
                    {
                        viewModel.Filters.Add(
                            CacheProvider.GetAll<Language>("languages")
                                .FirstOrDefault(x => x.LanguageID == languageId)
                                .Language1);
                    }
                }

                if (selectedKeywordIds.Any())
                {
                    viewModel.ResultsBeforeFilter = results;

                    results = (from r in results
                        join x in _db.SubjectIndexes on r.TitleID equals x.TitleID
                        where selectedKeywordIds.Contains(x.KeywordID)
                        select r).ToList();

                    foreach (var keywordId in selectedKeywordIds)
                    {
                        viewModel.Filters.Add(
                            CacheProvider.GetAll<Keyword>("keywords")
                                .FirstOrDefault(k => k.KeywordID == keywordId)
                                .KeywordTerm);
                    }
                }

                if (selectedAuthorIds.Any())
                {
                    viewModel.ResultsBeforeFilter = results;

                    results = (from r in results
                        join x in _db.TitleAuthors on r.TitleID equals x.TitleId
                        where selectedAuthorIds.Contains(x.AuthorId)
                        select r).ToList();

                    foreach (var authorId in selectedAuthorIds)
                    {
                        viewModel.Filters.Add(
                            CacheProvider.GetAll<Author>("authors")
                                .FirstOrDefault(a => a.AuthorID == authorId)
                                .DisplayName);
                    }
                }

                viewModel.Results = results;

                var take = viewModel.NarrowByDefaultRecordCount;

                //1. Get the media types to narrow by ...
                //Get a list of any media types associated with titles in the search results ...
                viewModel.MediaFilter.Clear();
                ModelState.Clear();

                List<int> mediaIds;
                if (selectedMediaIds.Any())
                {
                    mediaIds = selectedMediaIds;
                }
                else
                {
                    mediaIds = new List<int>();
                    foreach (var media in results.GroupBy(r => r.MediaID)
                        .Select(group => new
                        {
                            MediaID = group.Key,
                            Count = group.Count()
                        })
                        .OrderByDescending(x => x.Count).Take(take))
                    {
                        mediaIds.Add(media.MediaID);
                    }
                }

                if (mediaIds.Any())
                {
                    var mediatypes = (from mediatype in CacheProvider.GetAll<MediaType>("mediatypes")
                                      where mediaIds.Contains(mediatype.MediaID)
                                      select mediatype).ToList();

                    viewModel.MediaFilter.Clear();

                    foreach (var item in mediatypes)
                    {
                        var editorViewModel = new SelectMediaEditorViewModel()
                        {
                            Id = item.MediaID,
                            Name = item.Media,
                            Selected = selectedMediaIds.Contains(item.MediaID),
                            TitleCount = results.Count(r => r.MediaID == item.MediaID)
                        };
                        viewModel.MediaFilter.Add(editorViewModel);
                    }
                }
                viewModel.MediaFilter = viewModel.MediaFilter.OrderByDescending(x => x.TitleCount).ToList();
                TempData["MediaFilter"] = viewModel.MediaFilter;

                //2. Get the classmarks to narrow by ...
                viewModel.ClassmarksFilter.Clear();
                ModelState.Clear();

                List<int> classmarkIds;
                if (selectedClassmarkIds.Any())
                {
                    classmarkIds = selectedClassmarkIds;
                }
                else
                {
                    classmarkIds = new List<int>();
                    foreach (var classmark in results.GroupBy(r => r.ClassmarkID)
                        .Select(group => new
                        {
                            ClassmarkID = group.Key,
                            Count = group.Count()
                        })
                        .OrderByDescending(x => x.Count).Take(take))
                    {
                        classmarkIds.Add(classmark.ClassmarkID);
                    }
                }

                if (classmarkIds.Any())
                {
                    var classmarks = (from classmark in CacheProvider.GetAll<Classmark>("classmarks")
                                      where classmarkIds.Contains(classmark.ClassmarkID)
                                      select classmark).ToList();

                    viewModel.ClassmarksFilter.Clear();

                    foreach (var item in classmarks)
                    {
                        var editorViewModel = new SelectClassmarkEditorViewModel()
                        {
                            Id = item.ClassmarkID,
                            Name = item.Classmark1,
                            Selected = selectedClassmarkIds.Contains(item.ClassmarkID),
                            TitleCount = results.Count(r => r.ClassmarkID == item.ClassmarkID)
                        };
                        viewModel.ClassmarksFilter.Add(editorViewModel);
                    }
                }
                viewModel.ClassmarksFilter = viewModel.ClassmarksFilter.OrderByDescending(x => x.TitleCount).ToList();
                TempData["ClassmarksFilter"] = viewModel.ClassmarksFilter;

                //3. Get the publishers to narrow by ...
                //Get a list of any publishers associated with titles in the search results ...
                viewModel.PublisherFilter.Clear();
                ModelState.Clear();

                List<int> publisherIds;
                if (selectedPublisherIds.Any())
                {
                    publisherIds = selectedPublisherIds;
                }
                else
                {
                    publisherIds = new List<int>();
                    foreach (var publisher in results.GroupBy(r => r.PublisherID)
                        .Select(group => new
                        {
                            PublisherID = group.Key,
                            Count = group.Count()
                        })
                        .OrderByDescending(x => x.Count).Take(take))
                    {
                        publisherIds.Add(publisher.PublisherID);
                    }
                }

                if (publisherIds.Any())
                {
                    var publishers = (from publisher in CacheProvider.GetAll<Publisher>("publishers")
                                      where publisherIds.Contains(publisher.PublisherID)
                                      select publisher).ToList();

                    viewModel.PublisherFilter.Clear();

                    foreach (var item in publishers)
                    {
                        var editorViewModel = new SelectPublisherEditorViewModel()
                        {
                            Id = item.PublisherID,
                            Name = item.PublisherName,
                            Selected = selectedPublisherIds.Contains(item.PublisherID),
                            TitleCount = results.Count(r => r.PublisherID == item.PublisherID)
                        };
                        viewModel.PublisherFilter.Add(editorViewModel);
                    }
                }
                viewModel.PublisherFilter = viewModel.PublisherFilter.OrderByDescending(x => x.TitleCount).ToList();
                TempData["PublisherFilter"] = viewModel.PublisherFilter;

                //4. Get the languages to narrow by ...
                //Get a list of any languages associated with titles in the search results ...
                viewModel.LanguageFilter.Clear();
                ModelState.Clear();

                //var languageIds = selectedLanguageIds.Any() ? selectedLanguageIds : results.Select(item => item.LanguageID).Distinct().ToList();
                List<int> languageIds;
                if (selectedLanguageIds.Any())
                {
                    languageIds = selectedLanguageIds;
                }
                else
                {
                    languageIds = new List<int>();
                    foreach (var language in results.GroupBy(r => r.PublisherID)
                        .Select(group => new
                        {
                            LanguageID = group.Key,
                            Count = group.Count()
                        })
                        .OrderByDescending(x => x.Count).Take(take))
                    {
                        languageIds.Add(language.LanguageID);
                    }
                }

                if (languageIds.Any())
                {
                    var languages = (from language in CacheProvider.GetAll<Language>("languages")
                                     where languageIds.Contains(language.LanguageID)
                                     select language).ToList();

                    viewModel.LanguageFilter.Clear();

                    foreach (var item in languages)
                    {
                        var editorViewModel = new SelectLanguageEditorViewModel()
                        {
                            Id = item.LanguageID,
                            Name = item.Language1,
                            Selected = selectedLanguageIds.Contains(item.LanguageID),
                            TitleCount = results.Count(r => r.LanguageID == item.LanguageID)
                        };
                        viewModel.LanguageFilter.Add(editorViewModel);
                    }
                }
                viewModel.LanguageFilter = viewModel.LanguageFilter.OrderByDescending(x => x.TitleCount).ToList();
                TempData["LangaugeFilter"] = viewModel.LanguageFilter;

                //5. Get the keywords to narrow by ... 
                //Get a list of any keywords associated with titles in the search results ...
                viewModel.KeywordFilter.Clear();
                ModelState.Clear();

                //List<int> keywordIds;
                var resultsKeywordIds = new List<int>();

                // From the subject indexes of each result, get a list of any Keywords (KeywordID) 
                // Note: this list may contain duplicates which we'll sort out in the next step...
                var subjectIndexIds = results.Select(r => r.SubjectIndexes).Distinct().ToList();
                foreach (var resultTitle in subjectIndexIds.Where(x => x.Count > 0))
                {
                    resultsKeywordIds.AddRange(resultTitle.Select(t => t.KeywordID));
                }

                //Now get a list of distinct Keyword IDs from the list above and a count of how many times they occur in the search results ...
                var distinctKeywords = new Dictionary<int, int>(); ;
                foreach (var distinctTerm in resultsKeywordIds.GroupBy(i => i)
                        .Select(group => new
                        {
                            KeywordID = group.Key,
                            Count = group.Count()
                        })
                        .OrderByDescending(x => x.Count).Take(take))
                {
                    distinctKeywords.Add(distinctTerm.KeywordID, distinctTerm.Count);
                }

                //Now get the actual Keyword details (we've only been dealing with thier ID's up until now) ...
                if (distinctKeywords.Any())
                {
                    var keywords = (from k in CacheProvider.GetAll<Keyword>("keywords")
                                    where resultsKeywordIds.Contains(k.KeywordID)
                                    select k).ToList();

                    // Loop through the rows in the 'distinctKeywords' list and get the KeywordTerm ...
                    foreach (var item in distinctKeywords.OrderByDescending(k => k.Value))
                    {
                        var keyword = keywords.FirstOrDefault(k => k.KeywordID == item.Key);
                        if (keyword == null) continue;
                        var editorViewModel = new SelectKeywordEditorViewModel()
                        {
                            Id = keyword.KeywordID,
                            Name = keyword.KeywordTerm,
                            Selected = selectedKeywordIds.Contains(keyword.KeywordID),
                            TitleCount = item.Value
                        };
                        viewModel.KeywordFilter.Add(editorViewModel);
                    }
                }
                TempData["KeywordFilter"] = viewModel.KeywordFilter;


                //6. Get the authors to narrow by ...
                //Get a list of any authors associated with titles in the search results ...
                viewModel.AuthorFilter.Clear();
                ModelState.Clear();

                var resultsAuthorIds = new List<int>();

                // From the Title Authors of each result, get a list of any Authors (AuthorID) 
                // Note: this list may contain duplicates which we'll sort out in the next step...
                var titleAuthorIds = results.Select(r => r.TitleAuthors).Distinct().ToList();
                foreach (var resultTitle in titleAuthorIds.Where(x => x.Count > 0))
                {
                    resultsAuthorIds.AddRange(resultTitle.Select(t => t.AuthorId));
                }

                //Now get a list of distinct Keyword IDs from the list above and a count of how many times they occur in the search results ...
                var distinctAuthors = new Dictionary<int, int>(); ;
                foreach (var distinctAuthor in resultsAuthorIds.GroupBy(i => i)
                        .Select(group => new
                        {
                            AuthorID = group.Key,
                            Count = group.Count()
                        })
                        .OrderByDescending(x => x.Count).Take(take))
                {
                    distinctAuthors.Add(distinctAuthor.AuthorID, distinctAuthor.Count);
                }

                //Now get the actual Author details (we've only been dealing with their ID's up until now) ...
                if (distinctAuthors.Any())
                {
                    var authors = (from a in CacheProvider.GetAll<Author>("authors")
                                   where resultsAuthorIds.Contains(a.AuthorID)
                                   select a).ToList();

                    // Loop through the rows in the 'distinctAuthors' list and get the DisplayName ...
                    foreach (var item in distinctAuthors.OrderByDescending(a => a.Value))
                    {
                        var author = authors.FirstOrDefault(a => a.AuthorID == item.Key);
                        if (author == null) continue;
                        var editorViewModel = new SelectAuthorEditorViewModel()
                        {
                            Id = author.AuthorID,
                            Name = author.DisplayName,
                            Selected = selectedAuthorIds.Contains(author.AuthorID),
                            TitleCount = item.Value
                        };
                        viewModel.AuthorFilter.Add(editorViewModel);
                    }
                }
                TempData["AuthorFilter"] = viewModel.AuthorFilter;
            }

            if (!viewModel.Results.Any())
            {
                TempData["NoData"] = "Sorry, your search did not find any results. Please try again.";
            }

            ViewData["SearchField"] = SelectListHelper.SearchFieldsList(viewModel.SearchField);
            //ViewData["SearchStyles"] = GetSearchStyles();
            ViewBag.SearchTips = HelpTextHelper.GetHelpText("searchingtips");
            ViewBag.Title = !string.IsNullOrEmpty(q) ? "Catalogue Search Results" : "Search the Catalogue";
            TempData["simpleSearchingViewModel"] = viewModel;

            return View("AdminSearch", viewModel);
        }

        //[HttpPost]
        public ActionResult FinanceSearchResults(FinanceSearchingViewModel viewModel)
        {
            //Account Years filter ...
            if (!viewModel.AccountYearsFilter.Any())
            {
                if (TempData["AccountYearsFilter"] != null)
                {
                    viewModel.AccountYearsFilter =
                        (List<SelectAccountYearEditorViewModel>) TempData["AccountYearsFilter"];
                }
            }
            var selectedAccountYearIds = viewModel.GetSelectedAccountYearIds().ToList();

            //Budget Codes filter ...
            if (!viewModel.BudgetCodesFilter.Any())
            {
                if (TempData["BudgetCodesFilter"] != null)
                {
                    viewModel.BudgetCodesFilter = (List<SelectBudgetCodeEditorViewModel>) TempData["BudgetCodesFilter"];
                }
            }
            var selectedBudgetCodeIds = viewModel.GetSelectedBudgetCodeIds().ToList();

            //Order Categories filter ...
            if (!viewModel.OrderCategoriesFilter.Any())
            {
                if (TempData["OrderCategoriesFilter"] != null)
                {
                    viewModel.OrderCategoriesFilter =
                        (List<SelectOrderCategoryEditorViewModel>) TempData["OrderCategoriesFilter"];
                }
            }
            var selectedOrderCategoryIds = viewModel.GetSelectedOrderCategoryIds().ToList();

            //Requesters filter ...
            if (!viewModel.RequestersFilter.Any())
            {
                if (TempData["RequestersFilter"] != null)
                {
                    viewModel.RequestersFilter = (List<SelectRequesterEditorViewModel>) TempData["RequestersFilter"];
                }
            }
            var selectedRequestersIds = viewModel.GetSelectedRequesterIds().ToList();

            //Suppliers filter ...
            if (!viewModel.SuppliersFilter.Any())
            {
                if (TempData["SuppliersFilter"] != null)
                {
                    viewModel.SuppliersFilter = (List<SelectSupplierEditorViewModel>) TempData["SuppliersFilter"];
                }
            }
            var selectedSupplierIds = viewModel.GetSelectedSupplierIds().ToList();

            //local variable to make it easier to work with
            var q = viewModel.SearchString;

            if (!string.IsNullOrEmpty(q))
            {
                List<OrderDetail> orders;
                List<OrderDetail> results;

                q = q.ToLower();

                if (TempData["AllOrders"] == null)
                {
                    orders = (from o in _db.OrderDetails select o).Distinct().ToList();

                    TempData["AllOrders"] = orders;
                }
                else
                {
                    orders = (List<OrderDetail>) TempData["AllOrders"];
                    TempData["AllOrders"] = orders;
                }

                switch (viewModel.SearchField) //field to search in
                {
                    case "orderedtitle":
                    {
                        results = (from o in orders
                            where (o.Title.Title1 ?? "").ToLower().Contains(q)
                            orderby (o.Title.Title1 ?? "").Substring(o.Title.NonFilingChars)
                            select o).ToList();
                        break;
                    }
                    case "monthsubdue":
                    {
                        results = (from o in orders
                            where o.MonthSubDue != null && o.MonthSubDue == int.Parse(q)
                            orderby (o.Title.Title1 ?? "").Substring(o.Title.NonFilingChars)
                            select o).ToList();
                        break;
                    }
                    case "invoiceref":
                    {
                        results = (from o in orders
                            where o.InvoiceRef != null && o.InvoiceRef.ToLower().Contains(q)
                            orderby (o.Title.Title1 ?? "").Substring(o.Title.NonFilingChars)
                            select o).ToList();
                        break;
                    }
                    case "notes":
                    {
                        results = (from o in orders
                            where o.Notes != null && o.Notes.ToLower().Contains(q)
                            orderby (o.Title.Title1 ?? "").Substring(o.Title.NonFilingChars)
                            select o).ToList();
                        break;
                    }
                    case "ordernumber":
                    {
                        results = (from o in orders
                            where o.OrderNo != null && o.OrderNo.ToLower().Contains(q)
                            orderby (o.Title.Title1 ?? "").Substring(o.Title.NonFilingChars)
                            select o).ToList();
                        break;
                    }
                    case "price":
                    {
                        results = (from o in orders
                            where o.Price != null && o.Price == decimal.Parse(q)
                            orderby (o.Title.Title1 ?? "").Substring(o.Title.NonFilingChars)
                            select o).ToList();
                        break;
                    }
                    case "report":
                    {
                        results = (from o in orders
                            where o.Report != null && o.Report.ToLower().Contains(q)
                            orderby (o.Title.Title1 ?? "").Substring(o.Title.NonFilingChars)
                            select o).ToList();
                        break;
                    }
                    case "requestedby":
                    {
                        results = (from o in orders
                            where o.RequesterUser != null && o.RequesterUser.Lastname.Contains(q)
                            orderby (o.Title.Title1 ?? "").Substring(o.Title.NonFilingChars)
                            select o).ToList();
                        break;
                    }
                    case "requirement":
                    {
                        results = (from o in orders
                            where o.Item != null && o.Item.Contains(q)
                            orderby (o.Title.Title1 ?? "").Substring(o.Title.NonFilingChars)
                            select o).ToList();
                        break;
                    }
                    case "supplier":
                    {
                        results = (from o in orders
                            where o.Supplier != null && o.Supplier.SupplierName.Contains(q)
                            orderby (o.Title.Title1 ?? "").Substring(o.Title.NonFilingChars)
                            select o).ToList();
                        break;
                    }
                    case "all":
                    {
                        results = (from o in orders
                            where
                                (o.Title.Title1 ?? "").ToLower().Contains(q) ||
                                //(o.MonthSubDue ?? 0) == int.Parse(q) ||
                                (o.Notes ?? "").ToLower().Contains(q) ||
                                (o.OrderNo ?? "").ToLower().Contains(q) ||
                                //(o.Price ?? 0) == Decimal.Parse(q) ||
                                (o.Report ?? "").ToLower().Contains(q) ||
                                //(o.RequesterUser != null && (o.RequesterUser.Lastname ?? "").ToLower().Contains(q)) ||
                                (o.Item ?? "").ToLower().Contains(q)
                            orderby (o.Title.Title1 ?? "").Substring(o.Title.NonFilingChars)
                            select o).ToList();
                        break;
                    }
                    default:
                    {
                        results = orders;
                        break;
                    }
                }

                if (selectedAccountYearIds.Any())
                {
                    viewModel.ResultsBeforeFilter = results;

                    results = results.Where(r => selectedAccountYearIds.Contains(r.AccountYearID ?? 1900)).ToList();
                    foreach (var item in selectedAccountYearIds)
                    {
                        viewModel.Filters.Add(
                            CacheProvider.GetAll<AccountYear>("accountyears")
                                .FirstOrDefault(x => x.AccountYearID == item)
                                .AccountYear1);
                    }
                }

                if (selectedBudgetCodeIds.Any())
                {
                    viewModel.ResultsBeforeFilter = results;

                    results = results.Where(r => selectedBudgetCodeIds.Contains(r.BudgetCodeID ?? 0)).ToList();
                    foreach (var item in selectedBudgetCodeIds)
                    {
                        viewModel.Filters.Add(
                            CacheProvider.GetAll<BudgetCode>("budgetcodes")
                                .FirstOrDefault(x => x.BudgetCodeID == item)
                                .BudgetCode1);
                    }
                }

                if (selectedOrderCategoryIds.Any())
                {
                    viewModel.ResultsBeforeFilter = results;

                    results = results.Where(r => selectedOrderCategoryIds.Contains(r.OrderCategoryID ?? 0)).ToList();
                    foreach (var item in selectedOrderCategoryIds)
                    {
                        viewModel.Filters.Add(
                            CacheProvider.GetAll<OrderCategory>("ordercategories")
                                .FirstOrDefault(x => x.OrderCategoryID == item)
                                .OrderCategory1);
                    }
                }

                if (selectedRequestersIds.Any())
                {
                    viewModel.ResultsBeforeFilter = results;

                    results =
                        results.Where(
                            r => r.RequesterUser != null && selectedRequestersIds.Contains(r.RequesterUser.Id ?? ""))
                            .ToList();
                    foreach (var item in selectedRequestersIds)
                    {
                        viewModel.Filters.Add(
                            _db.Users.Where(u => u.RequestedOrders.Any() && u.CanDelete)
                                .FirstOrDefault(x => x.Id == item)
                                .FullnameRev);
                    }
                }

                if (selectedSupplierIds.Any())
                {
                    viewModel.ResultsBeforeFilter = results;

                    results =
                        results.Where(r => r.SupplierID != null && selectedSupplierIds.Contains(r.SupplierID ?? 0))
                            .ToList();
                    foreach (var item in selectedSupplierIds)
                    {
                        viewModel.Filters.Add(
                            CacheProvider.GetAll<Supplier>("suppliers")
                                .FirstOrDefault(x => x.SupplierID == item)
                                .SupplierName);
                    }
                }

                viewModel.Results = results;

                var take = viewModel.NarrowByDefaultRecordCount;

                //1. Get the account years to narrow by ...
                //Get a list of any account years associated with orders in the search results ...
                viewModel.AccountYearsFilter.Clear();
                ModelState.Clear();

                List<int> accountYearIds;
                if (selectedAccountYearIds.Any())
                {
                    accountYearIds = selectedAccountYearIds;
                }
                else
                {
                    accountYearIds = new List<int>();
                    foreach (var item in results.Where(r => r.AccountYearID != null).GroupBy(r => r.AccountYearID)
                        .Select(group => new
                        {
                            AccountYearID = group.Key,
                            Count = group.Count()
                        })
                        .OrderByDescending(x => x.Count).Take(take))
                    {
                        accountYearIds.Add(item.AccountYearID ?? 0);
                    }
                }

                if (accountYearIds.Any())
                {
                    var accountyears = (from accountyear in CacheProvider.GetAll<AccountYear>("accountyears")
                        where accountYearIds.Contains(accountyear.AccountYearID)
                        select accountyear).ToList();

                    viewModel.AccountYearsFilter.Clear();

                    foreach (var item in accountyears)
                    {
                        var editorViewModel = new SelectAccountYearEditorViewModel()
                        {
                            Id = item.AccountYearID,
                            Name = item.AccountYear1,
                            Selected = selectedAccountYearIds.Contains(item.AccountYearID),
                            OrderCount = results.Count(r => r.AccountYearID == item.AccountYearID)
                        };
                        viewModel.AccountYearsFilter.Add(editorViewModel);
                    }
                }
                TempData["AccountYearsFilter"] = viewModel.AccountYearsFilter;

                //2. Get the budget codes to narrow by ...
                viewModel.BudgetCodesFilter.Clear();
                ModelState.Clear();

                List<int> budgetCodeIds;
                if (selectedBudgetCodeIds.Any())
                {
                    budgetCodeIds = selectedBudgetCodeIds;
                }
                else
                {
                    budgetCodeIds = new List<int>();
                    foreach (var budgetcode in results.Where(r => r.BudgetCodeID != null).GroupBy(r => r.BudgetCodeID)
                        .Select(group => new
                        {
                            BudgetCodeID = group.Key,
                            Count = group.Count()
                        })
                        .OrderByDescending(x => x.Count).Take(take))
                    {
                        budgetCodeIds.Add(budgetcode.BudgetCodeID ?? 0);
                    }
                }

                if (budgetCodeIds.Any())
                {
                    var budgetCodes = (from b in CacheProvider.GetAll<BudgetCode>("budgetcodes")
                        where budgetCodeIds.Contains(b.BudgetCodeID)
                        select b).ToList();

                    viewModel.BudgetCodesFilter.Clear();

                    foreach (var item in budgetCodes)
                    {
                        var editorViewModel = new SelectBudgetCodeEditorViewModel()
                        {
                            Id = item.BudgetCodeID,
                            Name = item.BudgetCode1,
                            Selected = selectedBudgetCodeIds.Contains(item.BudgetCodeID),
                            OrderCount = results.Count(r => r.BudgetCodeID == item.BudgetCodeID)
                        };
                        viewModel.BudgetCodesFilter.Add(editorViewModel);
                    }
                }
                TempData["BudgetCodesFilter"] = viewModel.BudgetCodesFilter;

                //3. Get the order categories to narrow by ...
                //Get a list of any order categories associated with orders in the search results ...
                viewModel.OrderCategoriesFilter.Clear();
                ModelState.Clear();

                List<int> orderCategoryIds;
                if (selectedOrderCategoryIds.Any())
                {
                    orderCategoryIds = selectedOrderCategoryIds;
                }
                else
                {
                    orderCategoryIds = new List<int>();
                    foreach (
                        var orderCategory in
                            results.Where(r => r.OrderCategoryID != null).GroupBy(r => r.OrderCategoryID)
                                .Select(group => new
                                {
                                    OrderCategoryID = group.Key,
                                    Count = group.Count()
                                })
                                .OrderByDescending(x => x.Count).Take(take))
                    {
                        orderCategoryIds.Add(orderCategory.OrderCategoryID ?? 0);
                    }
                }

                if (orderCategoryIds.Any())
                {
                    var orderCategories = (from oc in CacheProvider.GetAll<OrderCategory>("ordercategories")
                        where orderCategoryIds.Contains(oc.OrderCategoryID)
                        select oc).ToList();

                    viewModel.OrderCategoriesFilter.Clear();

                    foreach (var item in orderCategories)
                    {
                        var editorViewModel = new SelectOrderCategoryEditorViewModel()
                        {
                            Id = item.OrderCategoryID,
                            Name = item.OrderCategory1,
                            Selected = selectedOrderCategoryIds.Contains(item.OrderCategoryID),
                            OrderCount = results.Count(r => r.OrderCategoryID == item.OrderCategoryID)
                        };
                        viewModel.OrderCategoriesFilter.Add(editorViewModel);
                    }
                }
                TempData["OrderCategoriesFilter"] = viewModel.OrderCategoriesFilter;


                //4. Get the requesters to narrow by ...
                viewModel.RequestersFilter.Clear();
                ModelState.Clear();

                List<string> requesterIds;
                if (selectedRequestersIds.Any())
                {
                    requesterIds = selectedRequestersIds;
                }
                else
                {
                    requesterIds = new List<string>();
                    foreach (
                        var requester in
                            results.Where(r => r.RequesterUser != null).GroupBy(r => r.RequesterUser.Id)
                                .Select(group => new
                                {
                                    RequesterID = group.Key,
                                    Count = group.Count()
                                })
                                .OrderByDescending(x => x.Count).Take(take))
                    {
                        requesterIds.Add(requester.RequesterID ?? "");
                    }
                }

                if (requesterIds.Any())
                {
                    var requesters = (from users in _db.Users.Where(u => u.RequestedOrders.Any())
                        where requesterIds.Contains(users.Id)
                        select users).ToList();

                    viewModel.RequestersFilter.Clear();

                    foreach (var user in requesters)
                    {
                        var editorViewModel = new SelectRequesterEditorViewModel()
                        {
                            Id = user.Id,
                            Name = user.FullnameRev,
                            Selected = selectedRequestersIds.Contains(user.Id),
                            OrderCount = results.Count(r => r.RequesterUser == user)
                        };
                        viewModel.RequestersFilter.Add(editorViewModel);
                    }
                }
                TempData["RequestersFilter"] = viewModel.RequestersFilter;

                //5. Get the suppliers to narrow by ...
                viewModel.SuppliersFilter.Clear();
                ModelState.Clear();

                List<int> supplierIds;
                if (selectedSupplierIds.Any())
                {
                    supplierIds = selectedSupplierIds;
                }
                else
                {
                    supplierIds = new List<int>();
                    foreach (
                        var supplier in
                            results.Where(r => r.SupplierID != null).GroupBy(r => r.SupplierID)
                                .Select(group => new
                                {
                                    SupplierID = group.Key,
                                    Count = group.Count()
                                })
                                .OrderByDescending(x => x.Count).Take(take))
                    {
                        supplierIds.Add(supplier.SupplierID ?? 0);
                    }
                }

                if (supplierIds.Any())
                {
                    var suppliers = (from s in CacheProvider.GetAll<Supplier>("suppliers")
                        where supplierIds.Contains(s.SupplierID)
                        select s).ToList();

                    viewModel.SuppliersFilter.Clear();

                    foreach (var item in suppliers)
                    {
                        var editorViewModel = new SelectSupplierEditorViewModel()
                        {
                            Id = item.SupplierID,
                            Name = item.SupplierName,
                            Selected = selectedSupplierIds.Contains(item.SupplierID),
                            OrderCount = results.Count(r => r.SupplierID == item.SupplierID)
                        };
                        viewModel.SuppliersFilter.Add(editorViewModel);
                    }
                }
                TempData["SuppliersFilter"] = viewModel.SuppliersFilter;
            }

            ViewData["SearchField"] = SelectListHelper.SearchFieldsList(viewModel.SearchField, scope: "finance");
            ViewBag.Title = string.IsNullOrEmpty(q) ? "Search Orders & Finance" : "Orders & Finance Search Results";
            TempData["financeSearchingViewModel"] = viewModel;

            return View("FinanceSearch", viewModel);
        }


        [HttpGet]
        public ActionResult AdminShowAllClassmarksFilter()
        {
            var viewModel = (SimpleSearchingViewModel)TempData["simpleSearchingViewModel"];
            var selectedClassmarkIds = viewModel.GetSelectedClassmarkIds().ToList();
            viewModel.ClassmarksFilter.Clear();

            var classmarks = (from c in CacheProvider.GetAll<Classmark>("classmarks")
                              join r in viewModel.ResultsBeforeFilter on c.ClassmarkID equals r.ClassmarkID
                              select c).Distinct().ToList();

            foreach (var item in classmarks.OrderBy(x => x.Classmark1))
            {
                var editorViewModel = new SelectClassmarkEditorViewModel()
                {
                    Id = item.ClassmarkID,
                    Name = item.Classmark1,
                    Selected = selectedClassmarkIds.Contains(item.ClassmarkID),
                    TitleCount = viewModel.ResultsBeforeFilter.Any() ? viewModel.ResultsBeforeFilter.Count(r => r.ClassmarkID == item.ClassmarkID) : viewModel.Results.Count(r => r.ClassmarkID == item.ClassmarkID)
                };
                viewModel.ClassmarksFilter.Add(editorViewModel);
            }

            TempData["simpleSearchingViewModel"] = viewModel;
            ViewBag.Title = "Narrow by " + @DbRes.T("Classmarks.Classmark", "FieldDisplayName");
            return PartialView(viewModel);
        }

        [HttpGet]
        public ActionResult AdminShowAllMediaFilter()
        {
            var viewModel = (SimpleSearchingViewModel)TempData["simpleSearchingViewModel"];
            var selectedMediaIds = viewModel.GetSelectedMediaIds().ToList();
            viewModel.MediaFilter.Clear();

            var mediatypes = (from m in CacheProvider.GetAll<MediaType>("mediatypes")
                              join r in viewModel.ResultsBeforeFilter on m.MediaID equals r.MediaID
                              select m).Distinct().ToList();

            foreach (var item in mediatypes.OrderBy(x => x.Media))
            {
                var editorViewModel = new SelectMediaEditorViewModel()
                {
                    Id = item.MediaID,
                    Name = item.Media,
                    Selected = selectedMediaIds.Contains(item.MediaID),
                    TitleCount = viewModel.ResultsBeforeFilter.Any() ? viewModel.ResultsBeforeFilter.Count(r => r.MediaID == item.MediaID) : viewModel.Results.Count(r => r.MediaID == item.MediaID)
                };
                viewModel.MediaFilter.Add(editorViewModel);
            }

            TempData["simpleSearchingViewModel"] = viewModel;
            ViewBag.Title = "Narrow by " + @DbRes.T("MediaTypes.Media_Type", "FieldDisplayName");
            return PartialView(viewModel);
        }

        [HttpGet]
        public ActionResult AdminShowAllPublishersFilter()
        {
            var viewModel = (SimpleSearchingViewModel)TempData["simpleSearchingViewModel"];
            var selectedPublisherIds = viewModel.GetSelectedPublisherIds().ToList();
            viewModel.PublisherFilter.Clear();

            var publishers = (from p in CacheProvider.GetAll<Publisher>("publishers")
                              join r in viewModel.ResultsBeforeFilter on p.PublisherID equals r.PublisherID
                              select p).Distinct().ToList();

            foreach (var item in publishers.OrderBy(x => x.PublisherName))
            {
                var editorViewModel = new SelectPublisherEditorViewModel()
                {
                    Id = item.PublisherID,
                    Name = item.PublisherName,
                    Selected = selectedPublisherIds.Contains(item.PublisherID),
                    TitleCount = viewModel.ResultsBeforeFilter.Any() ? viewModel.ResultsBeforeFilter.Count(r => r.PublisherID == item.PublisherID) : viewModel.Results.Count(r => r.PublisherID == item.PublisherID)
                };
                viewModel.PublisherFilter.Add(editorViewModel);
            }

            TempData["simpleSearchingViewModel"] = viewModel;
            ViewBag.Title = "Narrow by " + @DbRes.T("Publishers.Publisher", "FieldDisplayName");
            return PartialView(viewModel);
        }

        [HttpGet]
        public ActionResult AdminShowAllLanguagesFilter()
        {
            var viewModel = (SimpleSearchingViewModel)TempData["simpleSearchingViewModel"];
            var selectedLanguageIds = viewModel.GetSelectedLanguageIds().ToList();
            viewModel.LanguageFilter.Clear();

            var languages = (from l in CacheProvider.GetAll<Language>("languages")
                             join r in viewModel.ResultsBeforeFilter on l.LanguageID equals r.LanguageID
                             select l).Distinct().ToList();

            foreach (var item in languages.OrderBy(x => x.Language1))
            {
                var editorViewModel = new SelectLanguageEditorViewModel()
                {
                    Id = item.LanguageID,
                    Name = item.Language1,
                    Selected = selectedLanguageIds.Contains(item.LanguageID),
                    TitleCount = viewModel.ResultsBeforeFilter.Any() ? viewModel.ResultsBeforeFilter.Count(r => r.LanguageID == item.LanguageID) : viewModel.Results.Count(r => r.LanguageID == item.LanguageID)
                };
                viewModel.LanguageFilter.Add(editorViewModel);
            }

            TempData["simpleSearchingViewModel"] = viewModel;
            ViewBag.Title = "Narrow by " + @DbRes.T("Languages.Language", "FieldDisplayName");
            return PartialView(viewModel);
        }

        [HttpGet]
        public ActionResult AdminShowAllKeywordsFilter()
        {
            var viewModel = (SimpleSearchingViewModel)TempData["simpleSearchingViewModel"];
            var selectedKeywordIds = viewModel.GetSelectedKeywordIds().ToList();
            var resultsKeywordIds = new List<int>();

            viewModel.KeywordFilter.Clear();

            // From the subject indexes of each result, get a list of any Keywords (KeywordID) 
            // Note: this list may contain duplicates which we'll sort out in the next step...
            var subjectIndexIds = viewModel.Results.Select(r => r.SubjectIndexes).Distinct().ToList();
            foreach (var resultTitle in subjectIndexIds.Where(x => x.Count > 0))
            {
                resultsKeywordIds.AddRange(resultTitle.Select(t => t.KeywordID));
            }

            //Now get a list of distinct Keyword IDs from the list above and a count of how many times they occur in the search results ...
            var distinctKeywords = new Dictionary<int, int>();
            foreach (var distinctKeyword in resultsKeywordIds.GroupBy(i => i)
                    .Select(group => new
                    {
                        KeywordID = group.Key,
                        Count = group.Count()
                    })
                )
            {
                distinctKeywords.Add(distinctKeyword.KeywordID, distinctKeyword.Count);
            }

            //Now get the actual Keyword details (we've only been dealing with their ID's up until now) ...
            if (distinctKeywords.Any())
            {
                // Loop through the rows in the 'distinctKeywords' list and get the KeywordTerm ...
                foreach (var item in distinctKeywords)
                {
                    var keyword = _db.Keywords.Find(item.Key);
                    if (keyword == null) continue;
                    var editorViewModel = new SelectKeywordEditorViewModel()
                    {
                        Id = keyword.KeywordID,
                        Name = keyword.KeywordTerm,
                        Selected = selectedKeywordIds.Contains(keyword.KeywordID),
                        TitleCount = item.Value
                    };
                    viewModel.KeywordFilter.Add(editorViewModel);
                }
            }
            viewModel.KeywordFilter = viewModel.KeywordFilter.OrderBy(f => f.Name).ToList();
            TempData["simpleSearchingViewModel"] = viewModel;
            ViewBag.Title = "Narrow by " + @DbRes.T("Keywords.Keyword", "FieldDisplayName");
            return PartialView(viewModel);
        }

        [HttpGet]
        public ActionResult AdminShowAllAuthorsFilter()
        {
            var viewModel = (SimpleSearchingViewModel)TempData["simpleSearchingViewModel"];
            var selectedAuthorIds = viewModel.GetSelectedAuthorIds().ToList();
            var resultsAuthorIds = new List<int>();

            viewModel.AuthorFilter.Clear();

            // From the title authors of each result, get a list of any Authors (AuthorID) 
            // Note: this list may contain duplicates which we'll sort out in the next step...
            var titleAuthorIds = viewModel.ResultsBeforeFilter.Select(r => r.TitleAuthors).Distinct().ToList();
            foreach (var resultTitle in titleAuthorIds.Where(x => x.Count > 0))
            {
                resultsAuthorIds.AddRange(resultTitle.Select(t => t.AuthorId));
            }

            //Now get a list of distinct Author IDs from the list above and a count of how many times they occur in the search results ...
            var distinctAuthors = new Dictionary<int, int>(); ;
            foreach (var distinctAuthor in resultsAuthorIds.GroupBy(i => i)
                    .Select(group => new
                    {
                        AuthorID = group.Key,
                        Count = group.Count()
                    })
                )
            {
                distinctAuthors.Add(distinctAuthor.AuthorID, distinctAuthor.Count);
            }

            //Now get the actual Author details (we've only been dealing with thier ID's up until now) ...
            if (distinctAuthors.Any())
            {
                // Loop through the rows in the 'distinctAuthors' list and get the DisplayName ...
                foreach (var item in distinctAuthors)
                {
                    var author = _db.Authors.Find(item.Key);
                    if (author == null) continue;
                    var editorViewModel = new SelectAuthorEditorViewModel()
                    {
                        Id = author.AuthorID,
                        Name = author.DisplayName,
                        Selected = selectedAuthorIds.Contains(author.AuthorID),
                        TitleCount = item.Value
                    };
                    viewModel.AuthorFilter.Add(editorViewModel);
                }
            }

            viewModel.AuthorFilter = viewModel.AuthorFilter.OrderBy(f => f.Name).ToList();
            TempData["simpleSearchingViewModel"] = viewModel;
            ViewBag.Title = "Narrow by " + @DbRes.T("Authors.Author", "FieldDisplayName");
            return PartialView(viewModel);
        }

        [HttpGet]
        public ActionResult AdminShowAllAccountYearsFilter()
        {
            var viewModel = (FinanceSearchingViewModel) TempData["financeSearchingViewModel"];
            var accountYears = CacheProvider.GetAll<AccountYear>("accountyears").ToList();
            var selectedAccountYearIds = viewModel.GetSelectedAccountYearIds().ToList();

            viewModel.AccountYearsFilter.Clear();

            foreach (var item in accountYears.OrderBy(x => x.YearStartDate))
            {
                var editorViewModel = new SelectAccountYearEditorViewModel()
                {
                    Id = item.AccountYearID,
                    Name = item.AccountYear1,
                    Selected = selectedAccountYearIds.Contains(item.AccountYearID),
                    OrderCount =
                        viewModel.ResultsBeforeFilter.Any()
                            ? viewModel.ResultsBeforeFilter.Count(r => r.AccountYearID == item.AccountYearID)
                            : viewModel.Results.Count(r => r.AccountYearID == item.AccountYearID)
                };
                viewModel.AccountYearsFilter.Add(editorViewModel);
            }

            TempData["financeSearchingViewModel"] = viewModel;
            ViewBag.Title = "Narrow by " + @DbRes.T("AccountYears.Account_Year", "FieldDisplayName");
            return PartialView(viewModel);
        }

        [HttpGet]
        public ActionResult AdminShowAllBudgetCodesFilter()
        {
            var viewModel = (FinanceSearchingViewModel) TempData["financeSearchingViewModel"];
            var budgetCodes = CacheProvider.GetAll<BudgetCode>("budgetcodes").ToList();
            var selectedBudgetCodeIds = viewModel.GetSelectedBudgetCodeIds().ToList();

            viewModel.BudgetCodesFilter.Clear();

            foreach (var item in budgetCodes.OrderBy(x => x.BudgetCode1))
            {
                var editorViewModel = new SelectBudgetCodeEditorViewModel()
                {
                    Id = item.BudgetCodeID,
                    Name = item.BudgetCode1,
                    Selected = selectedBudgetCodeIds.Contains(item.BudgetCodeID),
                    OrderCount =
                        viewModel.ResultsBeforeFilter.Any()
                            ? viewModel.ResultsBeforeFilter.Count(r => r.BudgetCodeID == item.BudgetCodeID)
                            : viewModel.Results.Count(r => r.BudgetCodeID == item.BudgetCodeID)
                };
                viewModel.BudgetCodesFilter.Add(editorViewModel);
            }

            TempData["financeSearchingViewModel"] = viewModel;
            ViewBag.Title = "Narrow by " + @DbRes.T("BudgetCode.Budget_Code", "FieldDisplayName");
            return PartialView(viewModel);
        }

        [HttpGet]
        public ActionResult AdminShowAllRequestersFilter()
        {
            var viewModel = (FinanceSearchingViewModel) TempData["financeSearchingViewModel"];
            var requesters = _db.Users.Where(u => u.RequestedOrders.Any()).ToList();
                //CacheProvider.GetAll<BudgetCode>("budgetcodes").ToList();
            var selectedRequesterIds = viewModel.GetSelectedRequesterIds().ToList();

            viewModel.RequestersFilter.Clear();

            foreach (var user in requesters.OrderBy(x => x.Lastname).ThenBy(x => x.Firstname))
            {
                var editorViewModel = new SelectRequesterEditorViewModel()
                {
                    Id = user.Id,
                    Name = string.Format("{0}, {1}", user.Lastname, user.Firstname),
                    Selected = selectedRequesterIds.Contains(user.Id),
                    OrderCount =
                        viewModel.ResultsBeforeFilter.Any()
                            ? viewModel.ResultsBeforeFilter.Count(r => r.RequesterUser == user)
                            : viewModel.Results.Count(r => r.RequesterUser == user)
                };
                viewModel.RequestersFilter.Add(editorViewModel);
            }

            TempData["financeSearchingViewModel"] = viewModel;
            ViewBag.Title = "Narrow by " + @DbRes.T("Orders.Requested_By", "FieldDisplayName");
            return PartialView(viewModel);
        }

        [HttpGet]
        public ActionResult AdminShowAllOrderCategoriesFilter()
        {
            var viewModel = (FinanceSearchingViewModel) TempData["financeSearchingViewModel"];
            var orderCategories = CacheProvider.GetAll<OrderCategory>("ordercategories").ToList();
            var selectedOrderCategoryIds = viewModel.GetSelectedOrderCategoryIds().ToList();

            viewModel.OrderCategoriesFilter.Clear();

            foreach (var item in orderCategories.OrderBy(x => x.OrderCategory1))
            {
                var editorViewModel = new SelectOrderCategoryEditorViewModel()
                {
                    Id = item.OrderCategoryID,
                    Name = item.OrderCategory1,
                    Selected = selectedOrderCategoryIds.Contains(item.OrderCategoryID),
                    OrderCount =
                        viewModel.ResultsBeforeFilter.Any()
                            ? viewModel.ResultsBeforeFilter.Count(r => r.BudgetCodeID == item.OrderCategoryID)
                            : viewModel.Results.Count(r => r.BudgetCodeID == item.OrderCategoryID)
                };
                viewModel.OrderCategoriesFilter.Add(editorViewModel);
            }

            TempData["financeSearchingViewModel"] = viewModel;
            ViewBag.Title = "Narrow by " + @DbRes.T("OrderCategories.Order_Category", "FieldDisplayName");
            return PartialView(viewModel);
        }

        [HttpGet]
        public ActionResult AdminShowAllSuppliersFilter()
        {
            var viewModel = (FinanceSearchingViewModel) TempData["financeSearchingViewModel"];
            var suppliers = CacheProvider.GetAll<Supplier>("suppliers").ToList();
            var selectedSupplierIds = viewModel.GetSelectedSupplierIds().ToList();

            viewModel.SuppliersFilter.Clear();

            foreach (var item in suppliers.OrderBy(x => x.SupplierName))
            {
                var editorViewModel = new SelectSupplierEditorViewModel()
                {
                    Id = item.SupplierID,
                    Name = item.SupplierName,
                    Selected = selectedSupplierIds.Contains(item.SupplierID),
                    OrderCount =
                        viewModel.ResultsBeforeFilter.Any()
                            ? viewModel.ResultsBeforeFilter.Count(r => r.SupplierID == item.SupplierID)
                            : viewModel.Results.Count(r => r.SupplierID == item.SupplierID)
                };
                viewModel.SuppliersFilter.Add(editorViewModel);
            }

            TempData["financeSearchingViewModel"] = viewModel;
            ViewBag.Title = "Narrow by " + @DbRes.T("Orders.Supplier", "FieldDisplayName");
            return PartialView(viewModel);
        }

        [HttpPost]
        public ActionResult PostShowAllEntityFilter(SimpleSearchingViewModel viewModel)
        {
            //TempData["simpleSearchingViewModel"] = viewModel;
            TempData["ClassmarksFilter"] = viewModel.ClassmarksFilter;
            TempData["MediaFilter"] = viewModel.MediaFilter;
            TempData["PublisherFilter"] = viewModel.PublisherFilter;
            TempData["LanguageFilter"] = viewModel.LanguageFilter;
            TempData["KeywordFilter"] = viewModel.KeywordFilter;
            TempData["AuthorFilter"] = viewModel.AuthorFilter;

            return RedirectToAction("AdminSearchResults", viewModel);
        }

        [HttpPost]
        public ActionResult PostShowAllOrderEntityFilter(FinanceSearchingViewModel viewModel)
        {
            //TempData["simpleSearchingViewModel"] = viewModel;
            TempData["AccountYearsFilter"] = viewModel.AccountYearsFilter;
            TempData["BudgetCodesFilter"] = viewModel.BudgetCodesFilter;
            TempData["OrderCategoriesFilter"] = viewModel.OrderCategoriesFilter;
            TempData["RequestersFilter"] = viewModel.RequestersFilter;
            TempData["SuppliersFilter"] = viewModel.SuppliersFilter;
            return RedirectToAction("FinanceSearchResults", viewModel);
        }

        public ActionResult ClearAllFilters()
        {
            var viewModel = (SimpleSearchingViewModel) TempData["simpleSearchingViewModel"];
            if (viewModel != null)
            {
                viewModel.ClassmarksFilter.Clear();
                viewModel.MediaFilter.Clear();
                viewModel.PublisherFilter.Clear();
                viewModel.LanguageFilter.Clear();
                viewModel.KeywordFilter.Clear();
                viewModel.AuthorFilter.Clear();
            }
            return RedirectToAction("AdminSearchResults", viewModel);
        }

        public ActionResult ClearAllFinanceFilters()
        {
            var viewModel = (FinanceSearchingViewModel) TempData["financeSearchingViewModel"];
            if (viewModel != null)
            {
                viewModel.AccountYearsFilter.Clear();
                viewModel.BudgetCodesFilter.Clear();
                viewModel.OrderCategoriesFilter.Clear();
                viewModel.RequestersFilter.Clear();
                viewModel.SuppliersFilter.Clear();
            }
            return RedirectToAction("FinanceSearchResults", viewModel);
        }

        public ActionResult BarcodeEnquiry()
        {
            var viewModel = new BarcodeEnquiryViewModel();
            ViewBag.Title = "Barcode Enquiry";
            return PartialView(viewModel);
        }

        [HttpPost]
        public JsonResult BarcodeExists(string barcode)
        {
            barcode = barcode.Trim();
            var volume = _db.Volumes.FirstOrDefault(v => v.Barcode == barcode);
            return Json(volume != null);
        }

        [HttpPost]
        public ActionResult BarcodeLookup(BarcodeEnquiryViewModel viewModel)
        {
            if(string.IsNullOrEmpty(viewModel.Barcode))
            {
                return Json( new { success = false} );
            }
            
            var copyId = 0;
            var titleId = 0;
            var barcode = viewModel.Barcode.Trim();

            copyId = (from v in _db.Volumes
                where v.Barcode == barcode
                select v.CopyID).FirstOrDefault();

            if (copyId != 0)
            {
                titleId = (from c in _db.Copies
                    where c.CopyID == copyId
                    select c.TitleID).FirstOrDefault();
            }

            if (titleId != 0)
            {
                UrlHelper urlHelper = new UrlHelper(HttpContext.Request.RequestContext);
                string actionUrl = urlHelper.Action("Edit", "Titles", new { id = titleId });
                return Json(new { success = true, redirectTo = actionUrl });
            }

            return Json( new { success = false} );
        }

        public ActionResult IsbnEnquiry()
        {
            var viewModel = new IsbnLookupViewModel();
            ViewBag.Title = "ISBN/ISSN Enquiry";
            return PartialView(viewModel);
        }

        [HttpPost]
        public JsonResult IsbnExists(string isbn)
        {
            isbn = CleanIsbn(isbn);
            var title = _db.Titles.FirstOrDefault(t => t.ISBN10 == isbn || t.ISBN13 == isbn);
            return Json(title != null);
        }

        //Simple function to remove hyphens and spaces from passed ISBNs ...
        public string CleanIsbn(string isbn)
        {
            if (isbn != null)
            {
                var cleanedIsbn = isbn.Replace("-", "");
                cleanedIsbn = cleanedIsbn.Replace(".", "");
                cleanedIsbn = cleanedIsbn.Replace(" ", "");
                cleanedIsbn = cleanedIsbn.Trim();
                return cleanedIsbn;
            }
            return null;
        }

        [HttpPost]
        public ActionResult PostIsbnEnquiry(IsbnLookupViewModel viewModel)
        {
            var titles = from t in _db.Titles
                where t.ISBN10 == viewModel.Isbn || t.ISBN13 == viewModel.Isbn
                select t;

            if (!titles.Any())
            {
                return Json(new {success = false});
            }

            if (titles.Count() > 1)
            {
                UrlHelper urlHelper = new UrlHelper(HttpContext.Request.RequestContext);
                string actionUrl = urlHelper.Action("AdminSearch", new { q = viewModel.Isbn, f = "isbn" });
                return Json(new { success = true, redirectTo = actionUrl });
            }
            else
            {
                UrlHelper urlHelper = new UrlHelper(HttpContext.Request.RequestContext);
                string actionUrl = urlHelper.Action("Edit", "Titles", new { id = titles.FirstOrDefault().TitleID });
                return Json(new { success = true, redirectTo = actionUrl });
            }
        }

        public ActionResult EditTitle(int id)
        {
            return RedirectToAction("Edit", "Titles", new {id});
        }

        public void ExportCatalogueResults(string exportType = "titles")
        {
            var viewModel = (SimpleSearchingViewModel) TempData["simpleSearchingViewModel"];

            switch (exportType)
            {
                case "titles":
                {
                    var titlesOnlyList = new List<ExportJustTitlesViewModel>();
                    foreach (var title in viewModel.Results.ToList())
                    {
                        var anotherTitle = new ExportJustTitlesViewModel
                        {
                            TitleId = title.TitleID,
                            Title = title.Title1 ?? "",
                            Author = title.AuthorString ?? "",
                            Edition = title.Edition ?? "",
                            ISBN = title.Isbn ?? "",
                            Publisher = title.Publisher.PublisherName ?? "",
                            Year = title.Year ?? "",
                            PlaceofPublication = title.PlaceofPublication ?? "",
                            Media = title.MediaType.Media ?? "",
                            Classmark = title.Classmark.Classmark1 ?? "",
                            Language = title.Language.Language1 ?? "",
                            Series = title.Series ?? "",
                            Copies = title.Copies.Count(c => c.Deleted == false),
                            DateCatalogued = string.Format("{0:yyyy-MM-dd}", title.DateCatalogued),
                            Links = title.LinkString ?? "",
                            Keywords = title.KeywordString ?? "",
                            TitleTexts = title.TitleTextString ?? ""
                        };
                        titlesOnlyList.Add(anotherTitle);
                    }
                    DataExport.ExportExcel2007(titlesOnlyList);
                    break;
                }
                case "titlescopies":
                {
                    var titlesCopiesList = new List<ExportTitlesCopiesViewModel>();
                    var copies = from t in viewModel.Results
                        join c in _db.Copies on t.TitleID equals c.TitleID
                        select c;

                    foreach (var copy in copies.ToList())
                    {
                        var anotherCopy = new ExportTitlesCopiesViewModel
                        {
                            TitleId = copy.TitleID,
                            Title = copy.Title.Title1 ?? "",
                            Author = copy.Title.AuthorString ?? "",
                            Edition = copy.Title.Edition ?? "",
                            ISBN = copy.Title.Isbn ?? "",
                            Publisher = copy.Title.Publisher.PublisherName ?? "",
                            Year = copy.Title.Year ?? "",
                            PlaceofPublication = copy.Title.PlaceofPublication ?? "",
                            Media = copy.Title.MediaType.Media ?? "",
                            Classmark = copy.Title.Classmark.Classmark1 ?? "",
                            Language = copy.Title.Language.Language1 ?? "",
                            Series = copy.Title.Series ?? "",
                            Keywords = copy.Title.KeywordString ?? "",
                            Links = copy.Title.LinkString ?? "",
                            TitleTexts = copy.Title.TitleTextString ?? "",
                            Copy = copy.CopyNumber,
                            Location = copy.Location.LocationString ?? "",
                            Status = copy.StatusType.Status,
                            Holdings = copy.Holdings ?? "",
                            Volumes = copy.Volumes.Count(volume => volume.Deleted == false),
                            DateCatalogued = string.Format("{0:yyyy-MM-dd}", copy.Title.DateCatalogued)
                        };
                        titlesCopiesList.Add(anotherCopy);
                    }
                    DataExport.ExportExcel2007(titlesCopiesList);
                    break;
                }
                case "titlescopiesvolumes":
                {
                    var titlesCopiesVolumesList = new List<ExportTitlesCopiesVolumesViewModel>();
                    var allVolumes = from t in viewModel.Results
                        join c in _db.Copies on t.TitleID equals c.TitleID
                        join v in _db.Volumes on c.CopyID equals v.CopyID
                        where t.Deleted == false && c.Deleted == false && v.Deleted == false
                        select v;

                    foreach (var volume in allVolumes)
                    {
                        var anotherVolume = new ExportTitlesCopiesVolumesViewModel()
                        {
                            TitleId = volume.Copy.TitleID,
                            Title = volume.Copy.Title.Title1 ?? "",
                            Author = volume.Copy.Title.AuthorString ?? "",
                            Edition = volume.Copy.Title.Edition ?? "",
                            ISBN = volume.Copy.Title.Isbn ?? "",
                            Publisher = volume.Copy.Title.Publisher.PublisherName ?? "",
                            Year = volume.Copy.Title.Year ?? "",
                            PlaceofPublication = volume.Copy.Title.PlaceofPublication ?? "",
                            Media = volume.Copy.Title.MediaType.Media ?? "",
                            Classmark = volume.Copy.Title.Classmark.Classmark1 ?? "",
                            Language = volume.Copy.Title.Language.Language1 ?? "",
                            Series = volume.Copy.Title.Series ?? "",
                            Keywords = volume.Copy.Title.KeywordString ?? "",
                            Links = volume.Copy.Title.LinkString ?? "",
                            TitleTexts = volume.Copy.Title.TitleTextString ?? "",
                            Copy = volume.Copy.CopyNumber,
                            Location = volume.Copy.Location.Location1 ?? "",
                            Status = volume.Copy.StatusType.Status,
                            Holdings = volume.Copy.Holdings ?? "",
                            Barcode = volume.Barcode ?? "",
                            LabelText = volume.LabelText ?? "",
                            LoanType = volume.LoanType.LoanTypeName ?? "",
                            OnLoan = volume.OnLoan,
                            DateCatalogued = string.Format("{0:yyyy-MM-dd}", volume.Copy.Title.DateCatalogued)
                        };
                        titlesCopiesVolumesList.Add(anotherVolume);
                    }
                    DataExport.ExportExcel2007(titlesCopiesVolumesList);
                    break;
                }
            }
            TempData["simpleSearchingViewModel"] = viewModel;
        }

        public async Task<ActionResult> ExportCatalogueResultsAsync(string exportType = "titles")
        {
            var viewModel = (SimpleSearchingViewModel)TempData["simpleSearchingViewModel"];

            await Task.Run(() =>
            {
                switch (exportType)
                {
                    case "titles":
                    {
                        var titlesOnlyList = new List<ExportJustTitlesViewModel>();
                        foreach (var title in viewModel.Results.ToList())
                        {
                            var anotherTitle = new ExportJustTitlesViewModel
                            {
                                TitleId = title.TitleID,
                                Title = title.Title1 ?? "",
                                Author = title.AuthorString ?? "",
                                Edition = title.Edition ?? "",
                                ISBN = title.Isbn ?? "",
                                Publisher = title.Publisher.PublisherName ?? "",
                                Year = title.Year ?? "",
                                PlaceofPublication = title.PlaceofPublication ?? "",
                                Media = title.MediaType.Media ?? "",
                                Classmark = title.Classmark.Classmark1 ?? "",
                                Language = title.Language.Language1 ?? "",
                                Series = title.Series ?? "",
                                Copies = title.Copies.Count(c => c.Deleted == false),
                                DateCatalogued = string.Format("{0:yyyy-MM-dd}", title.DateCatalogued),
                                Links = title.LinkString ?? "",
                                Keywords = title.KeywordString ?? "",
                                TitleTexts = title.TitleTextString ?? ""
                            };
                            titlesOnlyList.Add(anotherTitle);
                        }
                        DataExport.ExportExcel2007(titlesOnlyList);
                        break;
                    }
                    case "titlescopies":
                    {
                        var titlesCopiesList = new List<ExportTitlesCopiesViewModel>();
                        var copies = from t in viewModel.Results
                            join c in _db.Copies on t.TitleID equals c.TitleID
                            select c;

                        foreach (var copy in copies.ToList())
                        {
                            var anotherCopy = new ExportTitlesCopiesViewModel
                            {
                                TitleId = copy.TitleID,
                                Title = copy.Title.Title1 ?? "",
                                Author = copy.Title.AuthorString ?? "",
                                Edition = copy.Title.Edition ?? "",
                                ISBN = copy.Title.Isbn ?? "",
                                Publisher = copy.Title.Publisher.PublisherName ?? "",
                                Year = copy.Title.Year ?? "",
                                PlaceofPublication = copy.Title.PlaceofPublication ?? "",
                                Media = copy.Title.MediaType.Media ?? "",
                                Classmark = copy.Title.Classmark.Classmark1 ?? "",
                                Language = copy.Title.Language.Language1 ?? "",
                                Series = copy.Title.Series ?? "",
                                Keywords = copy.Title.KeywordString ?? "",
                                Links = copy.Title.LinkString ?? "",
                                TitleTexts = copy.Title.TitleTextString ?? "",
                                Copy = copy.CopyNumber,
                                Location = copy.Location.LocationString ?? "",
                                Status = copy.StatusType.Status,
                                Holdings = copy.Holdings ?? "",
                                Volumes = copy.Volumes.Count(volume => volume.Deleted == false),
                                DateCatalogued = string.Format("{0:yyyy-MM-dd}", copy.Title.DateCatalogued)
                            };
                            titlesCopiesList.Add(anotherCopy);
                        }
                        DataExport.ExportExcel2007(titlesCopiesList);
                        break;
                    }
                    case "titlescopiesvolumes":
                    {
                        var titlesCopiesVolumesList = new List<ExportTitlesCopiesVolumesViewModel>();
                        var allVolumes = from t in viewModel.Results
                            join c in _db.Copies on t.TitleID equals c.TitleID
                            join v in _db.Volumes on c.CopyID equals v.CopyID
                            where t.Deleted == false && c.Deleted == false && v.Deleted == false
                            select v;

                        foreach (var volume in allVolumes)
                        {
                            var anotherVolume = new ExportTitlesCopiesVolumesViewModel()
                            {
                                TitleId = volume.Copy.TitleID,
                                Title = volume.Copy.Title.Title1 ?? "",
                                Author = volume.Copy.Title.AuthorString ?? "",
                                Edition = volume.Copy.Title.Edition ?? "",
                                ISBN = volume.Copy.Title.Isbn ?? "",
                                Publisher = volume.Copy.Title.Publisher.PublisherName ?? "",
                                Year = volume.Copy.Title.Year ?? "",
                                PlaceofPublication = volume.Copy.Title.PlaceofPublication ?? "",
                                Media = volume.Copy.Title.MediaType.Media ?? "",
                                Classmark = volume.Copy.Title.Classmark.Classmark1 ?? "",
                                Language = volume.Copy.Title.Language.Language1 ?? "",
                                Series = volume.Copy.Title.Series ?? "",
                                Keywords = volume.Copy.Title.KeywordString ?? "",
                                Links = volume.Copy.Title.LinkString ?? "",
                                TitleTexts = volume.Copy.Title.TitleTextString ?? "",
                                Copy = volume.Copy.CopyNumber,
                                Location = volume.Copy.Location.Location1 ?? "",
                                Status = volume.Copy.StatusType.Status,
                                Holdings = volume.Copy.Holdings ?? "",
                                Barcode = volume.Barcode ?? "",
                                LabelText = volume.LabelText ?? "",
                                LoanType = volume.LoanType.LoanTypeName ?? "",
                                OnLoan = volume.OnLoan,
                                DateCatalogued = string.Format("{0:yyyy-MM-dd}", volume.Copy.Title.DateCatalogued)
                            };
                            titlesCopiesVolumesList.Add(anotherVolume);
                        }
                        DataExport.ExportExcel2007(titlesCopiesVolumesList);
                        break;
                    }
                }
                
            });

            TempData["simpleSearchingViewModel"] = viewModel;
            return null;
        }

        public ActionResult SaveSearch()
        {
            var currentSearch = (SimpleSearchingViewModel)TempData["simpleSearchingViewModel"];

            var description = "";
            if (!string.IsNullOrEmpty(currentSearch.SearchString))
            {
                description = currentSearch.SearchString;
            }

            var viewModel = new LibraryUserSavedSearchViewModel()
            {
                UserId = PublicFunctions.GetUserId(), //User.Identity.GetUserId(),
                Description = description,
                SearchString = currentSearch.SearchString,
                SearchField = currentSearch.SearchField,
                Scope = "catalogue",
                AuthorFilter = string.Join(",", currentSearch.GetSelectedAuthorIds()),
                ClassmarksFilter = String.Join(",", currentSearch.GetSelectedClassmarkIds()),
                LanguageFilter = String.Join(",", currentSearch.GetSelectedLanguageIds()),
                KeywordFilter = String.Join(",", currentSearch.GetSelectedKeywordIds()),
                MediaFilter = String.Join(",", currentSearch.GetSelectedMediaIds()),
                PublisherFilter = String.Join(",", currentSearch.GetSelectedPublisherIds())
            };

            ViewBag.Title = "Save Search As ...";
            TempData["simpleSearchingViewModel"] = viewModel;
            return PartialView("SaveSearch", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PostSaveSearch(LibraryUserSavedSearchViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var newSavedSearch = new LibraryUserSavedSearch()
                    {
                        UserID = viewModel.UserId,
                        Description = viewModel.Description,
                        SearchString = viewModel.SearchString,
                        SearchField = viewModel.SearchField,
                        Scope = viewModel.Scope,
                        AuthorFilter = viewModel.AuthorFilter,
                        ClassmarksFilter = viewModel.ClassmarksFilter,
                        LanguageFilter = viewModel.LanguageFilter,
                        KeywordFilter = viewModel.KeywordFilter,
                        MediaFilter = viewModel.MediaFilter,
                        PublisherFilter = viewModel.PublisherFilter,
                        InputDate = DateTime.Now
                    };
                    _db.LibraryUserSavedSearches.Add(newSavedSearch);
                    _db.SaveChanges();
                    TempData["SuccessDialogMsg"] = "Your search has been saved.";
                    return Json(new { success = true });
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", e.Message);
                }
            }
            return Json(new { success = true });
        }
    }
}