using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.AspNet.Identity;
using slls.App_Settings;
using slls.DAO;
using slls.Models;
using slls.Utils.Helpers;
using slls.ViewModels;
using Westwind.Globalization;
using System.Web.Mvc.Expressions;

namespace slls.Controllers
{
    public class HomeController : sllsBaseController
    {
        private readonly DbEntities _db = new DbEntities();
        private readonly GenericRepository _repository;

        public HomeController()
        {
            _repository = new GenericRepository(typeof (Title));
        }
        
        public ActionResult Index()
        {
            var viewModel = new OPACHomePageViewModel()
            {
                ShowWelcomeMessage = Settings.GetParameterValue("OPAC.ShowWelcomeMessage", "true") == "true",
                WelcomeHeader = Settings.GetParameterValue("OPAC.WelcomeHeader", "Library OPAC"),
                WelcomeMessage = Settings.GetParameterValue("OPAC.WelcomeMessage", "Welcome to our new on-line Library")
            };
            using (var db = new DbEntities())
            {
                viewModel.HasTitles = db.Titles.Any();
            }
            
            ViewBag.Title = "Home";
            return View(viewModel);
        }

        public ActionResult ReleaseNotes(int releaseId = 0)
        {
            var releaseNotes = new List<ReleaseNote>();

            if (releaseId == 0)
            {
                releaseId = _db.ReleaseHeaders.Select(r => r.ReleaseId).Max();
            }

            if (releaseId > 0)
            {
                releaseNotes = _db.ReleaseNotes.Include(r => r.ReleaseHeader).Where(r => r.ReleaseId == releaseId).ToList();
            }

            ViewBag.Title = "Release Notes";
            ViewBag.ReleaseId = SelectListHelper.VersionReleases(releaseId);
            return View(releaseNotes);
        }

        public ActionResult About()
        {
            ViewBag.Title = "About";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Bailey Solutions Ltd";
            return View();
        }

        public ActionResult NotFound()
        {
            return View();
        }

        public ActionResult _MenuSearchTool()
        {
            var viewModel = new SimpleSearchingViewModel();
            return PartialView(viewModel);
        }
        
        public ActionResult NewTitles()
        {
            var viewModel = new TitlesListViewModel();

            //Get a collection of items on the 'New Titles' list ...
            var newtitles = (from t in _db.Titles
                                join c in _db.Copies on t.TitleID equals c.TitleID
                                where c.AcquisitionsList
                                orderby t.Title1.Substring(t.NonFilingChars)
                                select t).Distinct();

            viewModel.Titles = newtitles.ToList();
            ViewBag.Title = "New Titles";
            return View(viewModel);
        }

        public ActionResult RenderGadget(int col = 0, int row = 0, bool hasTitles = false)
        {
            //Get the current user's ID ...
            var userId = User.Identity.GetUserId();
            var allGadgets = CacheProvider.GetAll<DashboardGadget>("dashboardgadgets").ToList();
            var gadgetAction =
                allGadgets.Where(x => x.Row == row && x.Column == col && x.Area == "Home").Select(g => g.Name).FirstOrDefault();

            if (gadgetAction == null)
            {
                return null;
            }

            var viewModel = new OPACHomePageViewModel()
            {
                LibraryStaff = Roles.IsLibraryStaff()
            };

            switch (gadgetAction.ToLower())
            {
                case "usefullinks":
                {
                    var allLinks = CacheProvider.GetAll<UsefulLink>("usefullinks").ToList();
                    var links = from l in allLinks
                                where string.IsNullOrEmpty(l.DisplayText) == false
                                orderby l.DisplayText
                                select l;

                    if (!links.Any())
                    {
                        return null;
                    }

                    viewModel.UsefulLinks = links.ToList();
                    ViewBag.Title = DbRes.T("Useful_Links", "EntityType"); 
                    return PartialView("Dashboard/_UsefulLinks", viewModel);
                }
                case "searchgadget":
                {
                    if (!hasTitles)
                    {
                        return null;
                    }

                    ViewData["SearchField"] = SelectListHelper.SearchFieldsList();
                    ViewBag.Title = DbRes.T("Search", "Terminology");
                    return PartialView("Dashboard/_SearchGadget");
                }
                case "barcodesearch":
                {
                    if (!hasTitles)
                    {
                        return null;
                    }

                    ViewBag.Title = DbRes.T("CopyItems.Barcode", "FieldDisplayName") + " Look-Up";
                    return PartialView("Dashboard/_BarcodeSearch");
                }
                case "notifications":
                {
                    var allNotifications = CacheProvider.GetAll<Notification>("notifications").ToList();
                    var opacNotifications = allNotifications.Where(n => n.Scope == "O").OrderByDescending(n => n.InputDate).Take(5).ToList();

                    if (!opacNotifications.Any())
                    {
                        return null;
                    }

                    viewModel.Notifications = opacNotifications;
                    var count = opacNotifications.Count();
                    var countLabel = DbRes.T("Notifications.Notification", "FieldDisplayName");
                    if (count > 1)
                    {
                        countLabel = countLabel + "s";
                    }
                    ViewBag.Title = DbRes.T("Notifications", "EntityType");
                    ViewBag.Count = count;
                    ViewBag.CountLabel = countLabel;
                    ViewBag.Showing = opacNotifications.Count() < 5 ? opacNotifications.Count() : 5;
                    return PartialView("Dashboard/_Notifications", viewModel);
                }

                case "newtitles":
                {
                    if (!hasTitles)
                    {
                        return null;
                    }
                    
                    //Get a collection of items on the 'New Titles' list ...
                    var allnewtitles = CacheProvider.NewTitles().ToList();

                    if (!allnewtitles.Any())
                    {
                        return null;
                    }

                    viewModel.NewTitles = allnewtitles.OrderBy(t => t.Title1.Substring(t.NonFilingChars)).Take(10).ToList();
                    ViewBag.Title = DbRes.T("Titles.New_Titles", "FieldDisplayName");
                    ViewBag.Count = allnewtitles.Count();
                    ViewBag.Showing = allnewtitles.Count() < 10 ? allnewtitles.Count() : 10;
                    return PartialView("Dashboard/_NewTitles", viewModel);
                }

                case "usersavedsearches":
                {
                    //Bomb out now if the user isn't logged in
                    if (userId == null)
                    {
                        return null;
                    }

                    var allSavedSearches = CacheProvider.GetAll<LibraryUserSavedSearch>("savedsearches").ToList();
                    var savedSearches = from s in allSavedSearches
                                        where s.UserID == userId && string.IsNullOrEmpty(s.Description) == false
                                        orderby s.Description
                                        select s;

                    if (!savedSearches.Any())
                    {
                        return null;
                    }
                    viewModel.SavedSearches = savedSearches.Take(10).ToList();
                    ViewBag.Title = "My " + DbRes.T("Saved_Searches", "EntityType");
                    ViewBag.Count = savedSearches.Count();
                    ViewBag.Showing = savedSearches.Count() < 10 ? savedSearches.Count() : 10;
                    return PartialView("Dashboard/_UserSavedSearches", viewModel);
                }
                
                case "userbookmarks":
                {
                    if (userId == null)
                    {
                        return null;
                    }

                    var allUserBookmarks = CacheProvider.GetAll<LibraryUserBookmark>("bookmarks").ToList();
                    var userBookmarks = from b in allUserBookmarks
                                         where b.UserID == userId
                                         orderby b.Description
                                         select b;

                    if (!userBookmarks.Any())
                    {
                        return null;
                    }

                    viewModel.UserBookmarks = userBookmarks.Take(10).ToList();
                    ViewBag.Title = "My " + DbRes.T("Bookmarks", "EntityType");
                    ViewBag.Count = userBookmarks.Count();
                    ViewBag.Showing = userBookmarks.Count() < 10 ? userBookmarks.Count() : 10;
                    return PartialView("Dashboard/_UserBookmarks", viewModel);
                }
            }
            
            return null;
        }
        
        public ActionResult Notifications()
        {
            var notifications = _db.Notifications.Where(n => n.Scope == "O").OrderByDescending(n => n.InputDate);
            ViewBag.Title = DbRes.T("Notifications", "EntityType");
            return View(notifications);
        }

        public ActionResult BrowseByAuthor(int listAuthors = 0)
        {
            var viewModel = new SimpleSearchingViewModel();

            //Get a list of all authors in use
            var authors = (from a in _db.Authors
                           join t in _db.TitleAuthors on a.AuthorID equals t.AuthorId
                           select new { a.AuthorID, DisplayName = a.DisplayName.TrimStart() + " (" + a.TitleAuthors.Count + ")" }).Distinct().OrderBy(x => x.DisplayName);

            //Start a new list selectlist items ...
            List<SelectListItem> authorList = new List<SelectListItem>();

            //Add the authors ...
            foreach (var item in authors)
            {
                authorList.Add(new SelectListItem
                {
                    Text = item.DisplayName,
                    Value = item.AuthorID.ToString()
                });
            }

            ViewData["ListAuthors"] = authorList;
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("browseBySeeAlso", "");
            ViewBag.Title = "Browse By " + DbRes.T("Authors.Author", "FieldDisplayName");

            //Get the actual results if the user has selected anything ...
            viewModel.Results =
                (from t in
                    _db.Titles
                join a in _db.TitleAuthors on t.TitleID equals a.TitleId
                where a.AuthorId == listAuthors
                select t).ToList();
            return View(viewModel);
        }

        public ActionResult BrowseByClassmark(int listClassmarks = 0)
        {
            //Get the list of classmarks in use ...
            var classmarks = (from c in _db.Classmarks
                              where c.Titles.Count > 0
                              orderby c.Classmark1
                              select
                                  new { c.ClassmarkID, Classmark = c.Classmark1 + " (" + c.Titles.Count + ")" })
                .Distinct();

            //Start a new list selectlist items ...
            List<SelectListItem> classmarkList = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select a " + DbRes.T("Classmarks.Classmark", "FieldDisplayName"),
                    Value = "0"
                }
            };

            //Add the actual classmarks ...
            foreach (var item in classmarks)
            {
                classmarkList.Add(new SelectListItem
                {
                    Text = item.Classmark,
                    Value = item.ClassmarkID.ToString()
                });
            }

            ViewData["ListClassmarks"] = classmarkList;
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("browseBySeeAlso", ControllerContext.RouteData.Values["action"].ToString());
            ViewBag.Title = "Browse By " + DbRes.T("Classmarks.Classmark", "FieldDisplayName");

            //Get the actual results if the user has selected anything ...
            var viewModel = new SimpleSearchingViewModel();
            viewModel.Results =
                (from t in
                    _db.Titles
                where t.Classmark.ClassmarkID == listClassmarks
                select t).ToList();
            return View(viewModel);
        }

        public ActionResult BrowseByMedia(int listMedia = 0)
        {
            //Get the list of media types in use ...
            var media = (from m in CacheProvider.GetAll<MediaType>("mediatypes")
                         where m.Deleted == false && m.Titles.Count > 0
                         select
                             new { m.MediaID, Media = m.Media + " (" + m.Titles.Count + ")" })
                .Distinct().OrderBy(x => x.Media);

            //Start a new list selectlist items ...
            List<SelectListItem> mediaList = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select a " + DbRes.T("MediaTypes.Media_Type", "FieldDisplayName"),
                    Value = "0"
                }
            };

            //Add the actual media types ...
            foreach (var item in media)
            {
                mediaList.Add(new SelectListItem
                {
                    Text = item.Media,
                    Value = item.MediaID.ToString()
                });
            }

            ViewData["ListMedia"] = mediaList;
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("browseBySeeAlso", ControllerContext.RouteData.Values["action"].ToString());
            ViewBag.Title = "Browse By " + DbRes.T("MediaTypes.Media_Type", "FieldDisplayName");

            //Get the actual results if the user has selected anything ...
            var viewModel = new SimpleSearchingViewModel();
            viewModel.Results =
                (from t in
                    _db.Titles
                where t.MediaType.MediaID == listMedia
                select t).ToList();
            return View(viewModel);
        }

        public ActionResult BrowseByPublisher(int listPublishers = 0)
        {
            //Get the list of publishers in use ...
            var publishers = (from c in CacheProvider.GetAll<Publisher>("publishers")
                              where c.Titles.Count > 0
                              select
                                  new { c.PublisherID, PublisherName = c.PublisherName + " (" + c.Titles.Count + ")" })
                .Distinct().OrderBy(x => x.PublisherName);

            //Start a new list selectlist items ...
            List<SelectListItem> publisherList = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select a " + DbRes.T("Publishers.Publisher", "FieldDisplayName"),
                    Value = "0"
                }
            };
            publisherList.AddRange(publishers.Select(item => new SelectListItem
            {
                Text = item.PublisherName,
                Value = item.PublisherID.ToString()
            }));
            
            ViewData["ListPublishers"] = publisherList;
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("browseBySeeAlso", ControllerContext.RouteData.Values["action"].ToString());
            ViewBag.Title = "Browse By " + DbRes.T("Publishers.Publisher", "FieldDisplayName");

            //Get the actual results if the user has selected anything ...
            var viewModel = new SimpleSearchingViewModel();
            viewModel.Results =
                (from t in
                    _db.Titles
                where t.Publisher.PublisherID == listPublishers
                select t).ToList();
            return View(viewModel);
        }

        public ActionResult BrowseByLanguage(int listLanguage = 0)
        {
            //Get the list of languages in use ...
            var language = (from l in CacheProvider.GetAll<Language>("languages")
                            where l.Titles.Count > 0
                            select
                                new { l.LanguageID, Language = l.Language1 + " (" + l.Titles.Count + ")" })
                .Distinct().OrderBy(x => x.Language);

            //Start a new list selectlist items ...
            List<SelectListItem> languageList = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select a " + DbRes.T("Languages.Language", "FieldDisplayName"),
                    Value = "0"
                }
            };
            
            //Add the actual languages ...
            foreach (var item in language)
            {
                languageList.Add(new SelectListItem
                {
                    Text = item.Language,
                    Value = item.LanguageID.ToString()
                });
            }

            ViewData["ListLanguage"] = languageList;
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("browseBySeeAlso", ControllerContext.RouteData.Values["action"].ToString());
            ViewBag.Title = "Browse By " + DbRes.T("Languages.Language", "FieldDisplayName");

            //Get the actual results if the user has selected anything ...
            var viewModel = new SimpleSearchingViewModel();
            viewModel.Results =
                (from t in
                    _db.Titles
                where t.LanguageID == listLanguage
                select t).ToList();
            return View(viewModel);
        }

        public ActionResult BrowseByLocation(int listLocation = 0)
        {
            //Get the list of locations in use ...
            var location = (from l in _db.Locations
                            where l.Copies.Count > 0
                            select
                                new { locationID = l.LocationID, location = l.Location1 })
                .Distinct().OrderBy(x => x.location);

            //Start a new list selectlist items ...
            List<SelectListItem> locationList = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select a " + DbRes.T("Locations.Location", "FieldDisplayName"),
                    Value = "0"
                }
            };

            //Add the actual locations ...
            foreach (var item in location)
            {
                locationList.Add(new SelectListItem
                {
                    Text = item.location,
                    Value = item.locationID.ToString()
                });
            }

            ViewData["Listlocation"] = locationList;
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("browseBySeeAlso", ControllerContext.RouteData.Values["action"].ToString());
            ViewBag.Title = "Browse By " + DbRes.T("Locations.Location", "FieldDisplayName");

            //Get the actual results if the user has selected anything ...
            var viewModel = new SimpleSearchingViewModel();
            viewModel.Results =
                (from t in
                    _db.Titles join c in _db.Copies on t.TitleID equals c.CopyID
                where c.LocationID == listLocation
                select t).Distinct().ToList();
            return View(viewModel);
        }

        public ActionResult BrowseBySubject(int listSubjects = 0)
        {
            //Get a list of all currently used keywords. this cuts the down the list a bit!
            var keywords = (from k in CacheProvider.GetAll<Keyword>("keywords")
                            join x in _db.SubjectIndexes on k.KeywordID equals x.KeywordID
                            group k by new
                            {
                                Id = k.KeywordID,
                                Term = k.KeywordTerm
                            } into grouped
                            select new
                            {
                                keywordid = grouped.Key.Id,
                                keywordterm = grouped.Key.Term + " (" + grouped.Count() + ")"
                            }).Distinct().OrderBy(x => x.keywordterm);

            //Start a new list selectlist items ...
            List<SelectListItem> kwdList = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select a " + DbRes.T("Keywords.Keyword", "FieldDisplayName"),
                    Value = "0"
                }
            };

            //Add a default item ...

            //Add the actual keywords ...
            foreach (var item in keywords)
            {
                kwdList.Add(new SelectListItem
                {
                    Text = item.keywordterm,
                    Value = item.keywordid.ToString()
                });
            }

            ViewData["ListSubjects"] = kwdList;
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("browseBySeeAlso", ControllerContext.RouteData.Values["action"].ToString());
            ViewBag.Title = "Browse By " + DbRes.T("Keywords.Keyword", "FieldDisplayName");

            //Get a list of all items linked to the selected keyword
            var viewModel = new SimpleSearchingViewModel();
            viewModel.Results =
                (from t in
                    _db.Titles
                join x in _db.SubjectIndexes on t.TitleID equals x.TitleID
                where x.KeywordID == listSubjects
                select t).ToList();
            return View(viewModel);
        }

        public ActionResult _BarcodeEnquiry()
        {
            var viewModel = new OpacBarcodeEnquiryViewModel();
            ViewBag.Title = DbRes.T("CopyItems.Barcode","FieldDisplayName") + " Search";
            return PartialView(viewModel);
        }

        [HttpPost]
        public ActionResult BarcodeLookup(OpacBarcodeEnquiryViewModel viewModel)
        {
            var copyId = 0;
            var titleId = 0;

            copyId = (from v in _db.Volumes
                      where v.Barcode == viewModel.Barcode
                      select v.CopyID).FirstOrDefault();

            if (copyId != 0)
            {
                titleId = (from c in _db.Copies
                           where c.CopyID == copyId
                           select c.TitleID).FirstOrDefault();
            }

            if (titleId != 0)
            {
                return RedirectToAction("BookDetails", "Home", new { id = titleId });
                //return Json(new { success = true });
            }
            //ViewBag.Title = DbRes.T("CopyItems.Barcode", "FieldDisplayName") + " Search";
            //return PartialView("_BarcodeEnquiry", viewModel);
            return Json(new { success = false });
        }

        [HttpGet]
        public ActionResult SimpleSearch()
        {
            var viewModel = new SimpleSearchingViewModel()
            {
                LibraryStaff = Roles.IsLibraryStaff(),
                Area = "admin"
            };

            ViewData["SearchField"] = SelectListHelper.SearchFieldsList(scope: "opac");
            ViewBag.Title = Settings.GetParameterValue("Searching.SearchPageWelcome", "Search the Library");
            return View(viewModel);
        }

        //[HttpPost]
        public ActionResult SimpleSearchResults(SimpleSearchingViewModel viewModel)
        {
            //Classmarks filter ...
            if (!viewModel.ClassmarksFilter.Any())
            {
                if (TempData["ClassmarksFilter"] != null)
                {
                    viewModel.ClassmarksFilter = (List<SelectClassmarkEditorViewModel>)TempData["ClassmarksFilter"];
                }
            }
            var selectedClassmarkIds = viewModel.GetSelectedClassmarkIds().ToList();

            //Media filter ...
            if (!viewModel.MediaFilter.Any())
            {
                if (TempData["MediaFilter"] != null)
                {
                    viewModel.MediaFilter = (List<SelectMediaEditorViewModel>)TempData["MediaFilter"];
                }
            }
            var selectedMediaIds = viewModel.GetSelectedMediaIds().ToList();

            //Publisher filter ...
            if (!viewModel.PublisherFilter.Any())
            {
                if (TempData["PublisherFilter"] != null)
                {
                    viewModel.PublisherFilter = (List<SelectPublisherEditorViewModel>)TempData["PublisherFilter"];
                }
            }
            var selectedPublisherIds = viewModel.GetSelectedPublisherIds().ToList();

            //Language filter ...
            if (!viewModel.LanguageFilter.Any())
            {
                if (TempData["LanguageFilter"] != null)
                {
                    viewModel.LanguageFilter = (List<SelectLanguageEditorViewModel>)TempData["LanguageFilter"];
                }
            }
            var selectedLanguageIds = viewModel.GetSelectedLanguageIds().ToList();

            //Keyword filter ...
            if (!viewModel.KeywordFilter.Any())
            {
                if (TempData["KeywordFilter"] != null)
                {
                    viewModel.KeywordFilter = (List<SelectKeywordEditorViewModel>)TempData["KeywordFilter"];
                }
            }
            var selectedKeywordIds = viewModel.GetSelectedKeywordIds().ToList();

            //Author filter ...
            if (!viewModel.AuthorFilter.Any())
            {
                if (TempData["AuthorFilter"] != null)
                {
                    viewModel.AuthorFilter = (List<SelectAuthorEditorViewModel>)TempData["AuthorFilter"];
                }
            }
            var selectedAuthorIds = viewModel.GetSelectedAuthorIds().ToList();

            //Do some work on the passes search string ...

            //1. Check if there is a hyphen before a word - this indicates an "AND NOT" search
            var stringSplitter = new string[] { " -" };
            var searchString = viewModel.SearchString.Split(stringSplitter, StringSplitOptions.None);
            var q = searchString[0].Trim();
            var qIgnore = "";
            if (searchString.Length > 1)
            {
                for (int i = 1; i < searchString.Length; i++)
                {
                    qIgnore = qIgnore + searchString[i];
                }
            }

            //Check for some other 'Google-type' advanced search characters ...
            bool suffixSearch = q.StartsWith("*");
            bool wholeWordOnly = q.StartsWith("\u0022"); // i.e. quote mark (chr(34)
            bool prefixSearch = !suffixSearch && !wholeWordOnly;
            bool orSearch = q.Contains(" OR ");
            q = q.Replace("*", string.Empty);
            q = q.Replace("\u0022", string.Empty);
            q = q.ToLower();

            //Generate a list of words to ignore - at some point we might want to consider putting this into a table so that the user can control it.
            List<string> wordsToRemove = "and or the a an".Split(' ').ToList();

            if (!string.IsNullOrEmpty(q))
            {
                List<Title> opacTitles;
                List<Title> results;

                q = q.ToLower();

                if (TempData["OpacTitles"] == null)
                {
                    opacTitles = (from titles in _db.Titles
                                  join copies in _db.Copies on titles.TitleID equals copies.TitleID
                                  where !titles.Deleted && !copies.Deleted && copies.StatusType.Opac && copies.Volumes.Any()
                                  select titles).Distinct().ToList();

                    TempData["OpacTitles"] = opacTitles;
                }
                else
                {
                    opacTitles = (List<Title>)TempData["OpacTitles"];
                    TempData["OpacTitles"] = opacTitles;
                }

                //Initialize our results ...
                if (orSearch)
                {
                    results = _db.Titles.Where(x => 1 == 2).ToList();
                }
                else
                {
                    results = opacTitles;
                }

                switch (viewModel.SearchField) // i.e. field to search in
                {
                    case "title":
                        {
                            if (wholeWordOnly)
                            {
                                results = (from t in opacTitles
                                           where (t.Title1 ?? "").ToLower() == q || (t.Title1 ?? "").ToLower().Contains(" " + q + " ") || (t.Title1 ?? "").ToLower().StartsWith(q + " ") || (t.Title1 ?? "").ToLower().EndsWith(" " + q)
                                           select t).ToList();
                            }
                            else if (prefixSearch) // the default
                            {
                                var qWords = q.Split(' ').Except(wordsToRemove);
                                foreach (var word in qWords)
                                {
                                    if (orSearch)
                                    {
                                        results = results.Concat(from t in opacTitles
                                                                 where
                                                                     (t.Title1 ?? "").ToLower().Contains(" " + word) ||
                                                                     (t.Title1 ?? "").ToLower().StartsWith(word)
                                                                 select t).ToList();
                                    }
                                    else
                                    {
                                        results = (from t in results
                                                   where
                                                       (t.Title1 ?? "").ToLower().Contains(" " + word) ||
                                                       (t.Title1 ?? "").ToLower().StartsWith(word)
                                                   select t).ToList();
                                    }
                                }
                            }
                            else if (suffixSearch)
                            {
                                var qWords = q.Split(' ').Except(wordsToRemove);
                                foreach (var word in qWords)
                                {
                                    if (orSearch)
                                    {
                                        results = results.Concat(from t in opacTitles
                                                                 where
                                                                     (t.Title1 ?? "").ToLower().Contains(word + " ") ||
                                                                     (t.Title1 ?? "").ToLower().EndsWith(word)
                                                                 select t).ToList();
                                    }
                                    else
                                    {
                                        results = (from t in results
                                                   where
                                                       (t.Title1 ?? "").ToLower().Contains(word + " ") ||
                                                       (t.Title1 ?? "").ToLower().EndsWith(word)
                                                   select t).ToList();
                                    }
                                }
                            }
                            else
                            {
                                var qWords = q.Split(' ').Except(wordsToRemove);
                                foreach (var word in qWords)
                                {
                                    if (orSearch)
                                    {
                                        results = results.Concat(from t in opacTitles
                                                                 where
                                                                     (t.Title1 ?? "").ToLower().Contains(word)
                                                                 select t).ToList();
                                    }
                                    else
                                    {
                                        results = (from t in results
                                                   where
                                                       (t.Title1 ?? "").ToLower().Contains(word)
                                                   select t).ToList();
                                    }
                                }
                            }
                            if (qIgnore.Length > 0)
                            {
                                results = (from t in results
                                           where
                                               !(t.Title1 ?? "").ToLower().Contains(qIgnore)
                                           select t).ToList();
                            }
                            break;
                        }
                    case "author":
                        {
                            if (wholeWordOnly)
                            {
                                results = (from t in opacTitles
                                           where (t.AuthorString ?? "").ToLower() == q || (t.AuthorString ?? "").ToLower().Contains(" " + q + " ") || (t.AuthorString ?? "").ToLower().StartsWith(q + " ") || (t.AuthorString ?? "").ToLower().EndsWith(" " + q)
                                           select t).ToList();
                            }
                            else if (prefixSearch) // the default
                            {
                                var qWords = q.Split(' ').Except(wordsToRemove);
                                foreach (var word in qWords)
                                {
                                    if (orSearch)
                                    {
                                        results = results.Concat(from t in opacTitles
                                                                 where
                                                                     (t.AuthorString ?? "").ToLower().Contains(" " + word) ||
                                                                     (t.AuthorString ?? "").ToLower().StartsWith(word)
                                                                 select t).ToList();
                                    }
                                    else
                                    {
                                        results = (from t in results
                                                   where
                                                       (t.AuthorString ?? "").ToLower().Contains(" " + word) ||
                                                       (t.AuthorString ?? "").ToLower().StartsWith(word)
                                                   select t).ToList();
                                    }
                                }
                            }
                            else if (suffixSearch)
                            {
                                var qWords = q.Split(' ').Except(wordsToRemove);
                                foreach (var word in qWords)
                                {
                                    if (orSearch)
                                    {
                                        results = results.Concat(from t in opacTitles
                                                                 where
                                                                     (t.AuthorString ?? "").ToLower().Contains(word + " ") ||
                                                                     (t.AuthorString ?? "").ToLower().EndsWith(word)
                                                                 select t).ToList();
                                    }
                                    else
                                    {
                                        results = (from t in results
                                                   where
                                                       (t.AuthorString ?? "").ToLower().Contains(word + " ") ||
                                                       (t.AuthorString ?? "").ToLower().EndsWith(word)
                                                   select t).ToList();
                                    }
                                }
                            }
                            else
                            {
                                var qWords = q.Split(' ').Except(wordsToRemove);
                                foreach (var word in qWords)
                                {
                                    if (orSearch)
                                    {
                                        results = results.Concat(from t in opacTitles
                                                                 where
                                                                     (t.AuthorString ?? "").ToLower().Contains(word)
                                                                 select t).ToList();
                                    }
                                    else
                                    {
                                        results = (from t in results
                                                   where
                                                       (t.AuthorString ?? "").ToLower().Contains(word)
                                                   select t).ToList();
                                    }
                                }
                            }
                            if (qIgnore.Length > 0)
                            {
                                results = (from t in results
                                           where
                                               !(t.AuthorString ?? "").ToLower().Contains(qIgnore)
                                           select t).ToList();
                            }
                            break;
                        }
                    case "editor":
                        {
                            if (wholeWordOnly)
                            {
                                results = (from t in opacTitles
                                           where (t.EditorString ?? "").ToLower() == q || (t.EditorString ?? "").ToLower().Contains(" " + q + " ") || (t.EditorString ?? "").ToLower().StartsWith(q + " ") || (t.EditorString ?? "").ToLower().EndsWith(" " + q)
                                           select t).ToList();
                            }
                            else if (prefixSearch) // the default
                            {
                                var qWords = q.Split(' ').Except(wordsToRemove);
                                foreach (var word in qWords)
                                {
                                    if (orSearch)
                                    {
                                        results = results.Concat(from t in opacTitles
                                                                 where
                                                                     (t.EditorString ?? "").ToLower().Contains(" " + word) ||
                                                                     (t.EditorString ?? "").ToLower().StartsWith(word)
                                                                 select t).ToList();
                                    }
                                    else
                                    {
                                        results = (from t in results
                                                   where
                                                       (t.EditorString ?? "").ToLower().Contains(" " + word) ||
                                                       (t.EditorString ?? "").ToLower().StartsWith(word)
                                                   select t).ToList();
                                    }
                                }
                            }
                            else if (suffixSearch)
                            {
                                var qWords = q.Split(' ').Except(wordsToRemove);
                                foreach (var word in qWords)
                                {
                                    if (orSearch)
                                    {
                                        results = results.Concat(from t in opacTitles
                                                                 where
                                                                     (t.EditorString ?? "").ToLower().Contains(word + " ") ||
                                                                     (t.EditorString ?? "").ToLower().EndsWith(word)
                                                                 select t).ToList();
                                    }
                                    else
                                    {
                                        results = (from t in results
                                                   where
                                                       (t.EditorString ?? "").ToLower().Contains(word + " ") ||
                                                       (t.EditorString ?? "").ToLower().EndsWith(word)
                                                   select t).ToList();
                                    }
                                }
                            }
                            else
                            {
                                var qWords = q.Split(' ').Except(wordsToRemove);
                                foreach (var word in qWords)
                                {
                                    if (orSearch)
                                    {
                                        results = results.Concat(from t in opacTitles
                                                                 where
                                                                     (t.EditorString ?? "").ToLower().Contains(word)
                                                                 select t).ToList();
                                    }
                                    else
                                    {
                                        results = (from t in results
                                                   where
                                                       (t.EditorString ?? "").ToLower().Contains(word)
                                                   select t).ToList();
                                    }
                                }
                            }
                            if (qIgnore.Length > 0)
                            {
                                results = (from t in results
                                           where
                                               !(t.EditorString ?? "").ToLower().Contains(qIgnore)
                                           select t).ToList();
                            }
                            break;
                        }
                    case "publisher":
                        {
                            if (wholeWordOnly)
                            {
                                results = (from t in opacTitles
                                           where (t.Publisher.PublisherName ?? "").ToLower() == q || (t.Publisher.PublisherName ?? "").ToLower().Contains(" " + q + " ") || (t.Publisher.PublisherName ?? "").ToLower().StartsWith(q + " ") || (t.Publisher.PublisherName ?? "").ToLower().EndsWith(" " + q)
                                           select t).ToList();
                            }
                            else if (prefixSearch) // the default
                            {
                                var qWords = q.Split(' ').Except(wordsToRemove);
                                foreach (var word in qWords)
                                {
                                    if (orSearch)
                                    {
                                        results = results.Concat(from t in opacTitles
                                                                 where
                                                                     (t.Publisher.PublisherName ?? "").ToLower().Contains(" " + word) ||
                                                                     (t.Publisher.PublisherName ?? "").ToLower().StartsWith(word)
                                                                 select t).ToList();
                                    }
                                    else
                                    {
                                        results = (from t in results
                                                   where
                                                       (t.Publisher.PublisherName ?? "").ToLower().Contains(" " + word) ||
                                                       (t.Publisher.PublisherName ?? "").ToLower().StartsWith(word)
                                                   select t).ToList();
                                    }
                                }
                            }
                            else if (suffixSearch)
                            {
                                var qWords = q.Split(' ').Except(wordsToRemove);
                                foreach (var word in qWords)
                                {
                                    if (orSearch)
                                    {
                                        results = results.Concat(from t in opacTitles
                                                                 where
                                                                     (t.Publisher.PublisherName ?? "").ToLower().Contains(word + " ") ||
                                                                     (t.Publisher.PublisherName ?? "").ToLower().EndsWith(word)
                                                                 select t).ToList();
                                    }
                                    else
                                    {
                                        results = (from t in results
                                                   where
                                                       (t.Publisher.PublisherName ?? "").ToLower().Contains(word + " ") ||
                                                       (t.Publisher.PublisherName ?? "").ToLower().EndsWith(word)
                                                   select t).ToList();
                                    }
                                }
                            }
                            else
                            {
                                var qWords = q.Split(' ').Except(wordsToRemove);
                                foreach (var word in qWords)
                                {
                                    if (orSearch)
                                    {
                                        results = results.Concat(from t in opacTitles
                                                                 where
                                                                     (t.Publisher.PublisherName ?? "").ToLower().Contains(word)
                                                                 select t).ToList();
                                    }
                                    else
                                    {
                                        results = (from t in results
                                                   where
                                                       (t.Publisher.PublisherName ?? "").ToLower().Contains(word)
                                                   select t).ToList();
                                    }
                                }
                            }
                            if (qIgnore.Length > 0)
                            {
                                results = (from t in results
                                           where
                                               !(t.Publisher.PublisherName ?? "").ToLower().Contains(qIgnore)
                                           select t).ToList();
                            }
                            break;
                        }
                    case "citation":
                        {
                            if (wholeWordOnly)
                            {
                                results = (from t in opacTitles
                                           where (t.Citation ?? "").ToLower() == q || (t.Citation ?? "").ToLower().Contains(" " + q + " ") || (t.Citation ?? "").ToLower().StartsWith(q + " ") || (t.Citation ?? "").ToLower().EndsWith(" " + q)
                                           select t).ToList();
                            }
                            else if (prefixSearch) // the default
                            {
                                var qWords = q.Split(' ').Except(wordsToRemove);
                                foreach (var word in qWords)
                                {
                                    if (orSearch)
                                    {
                                        results = results.Concat(from t in opacTitles
                                                                 where
                                                                     (t.Citation ?? "").ToLower().Contains(" " + word) ||
                                                                     (t.Citation ?? "").ToLower().StartsWith(word)
                                                                 select t).ToList();
                                    }
                                    else
                                    {
                                        results = (from t in results
                                                   where
                                                       (t.Citation ?? "").ToLower().Contains(" " + word) ||
                                                       (t.Citation ?? "").ToLower().StartsWith(word)
                                                   select t).ToList();
                                    }
                                }
                            }
                            else if (suffixSearch)
                            {
                                var qWords = q.Split(' ').Except(wordsToRemove);
                                foreach (var word in qWords)
                                {
                                    if (orSearch)
                                    {
                                        results = results.Concat(from t in opacTitles
                                                                 where
                                                                     (t.Citation ?? "").ToLower().Contains(word + " ") ||
                                                                     (t.Citation ?? "").ToLower().EndsWith(word)
                                                                 select t).ToList();
                                    }
                                    else
                                    {
                                        results = (from t in results
                                                   where
                                                       (t.Citation ?? "").ToLower().Contains(word + " ") ||
                                                       (t.Citation ?? "").ToLower().EndsWith(word)
                                                   select t).ToList();
                                    }
                                }
                            }
                            else
                            {
                                var qWords = q.Split(' ').Except(wordsToRemove);
                                foreach (var word in qWords)
                                {
                                    if (orSearch)
                                    {
                                        results = results.Concat(from t in opacTitles
                                                                 where
                                                                     (t.Citation ?? "").ToLower().Contains(word)
                                                                 select t).ToList();
                                    }
                                    else
                                    {
                                        results = (from t in results
                                                   where
                                                       (t.Citation ?? "").ToLower().Contains(word)
                                                   select t).ToList();
                                    }
                                }
                            }
                            if (qIgnore.Length > 0)
                            {
                                results = (from t in results
                                           where
                                               !(t.Citation ?? "").ToLower().Contains(qIgnore)
                                           select t).ToList();
                            }
                            break;
                        }
                    case "source":
                        {
                            if (wholeWordOnly)
                            {
                                results = (from t in opacTitles
                                           where (t.Source ?? "").ToLower() == q || (t.Source ?? "").ToLower().Contains(" " + q + " ") || (t.Source ?? "").ToLower().StartsWith(q + " ") || (t.Source ?? "").ToLower().EndsWith(" " + q)
                                           select t).ToList();
                            }
                            else if (prefixSearch) // the default
                            {
                                var qWords = q.Split(' ').Except(wordsToRemove);
                                foreach (var word in qWords)
                                {
                                    if (orSearch)
                                    {
                                        results = results.Concat(from t in opacTitles
                                                                 where
                                                                     (t.Source ?? "").ToLower().Contains(" " + word) ||
                                                                     (t.Source ?? "").ToLower().StartsWith(word)
                                                                 select t).ToList();
                                    }
                                    else
                                    {
                                        results = (from t in results
                                                   where
                                                       (t.Source ?? "").ToLower().Contains(" " + word) ||
                                                       (t.Source ?? "").ToLower().StartsWith(word)
                                                   select t).ToList();
                                    }
                                }
                            }
                            else if (suffixSearch)
                            {
                                var qWords = q.Split(' ').Except(wordsToRemove);
                                foreach (var word in qWords)
                                {
                                    if (orSearch)
                                    {
                                        results = results.Concat(from t in opacTitles
                                                                 where
                                                                     (t.Source ?? "").ToLower().Contains(word + " ") ||
                                                                     (t.Source ?? "").ToLower().EndsWith(word)
                                                                 select t).ToList();
                                    }
                                    else
                                    {
                                        results = (from t in results
                                                   where
                                                       (t.Source ?? "").ToLower().Contains(word + " ") ||
                                                       (t.Source ?? "").ToLower().EndsWith(word)
                                                   select t).ToList();
                                    }
                                }
                            }
                            else
                            {
                                var qWords = q.Split(' ').Except(wordsToRemove);
                                foreach (var word in qWords)
                                {
                                    if (orSearch)
                                    {
                                        results = results.Concat(from t in opacTitles
                                                                 where
                                                                     (t.Source ?? "").ToLower().Contains(word)
                                                                 select t).ToList();
                                    }
                                    else
                                    {
                                        results = (from t in results
                                                   where
                                                       (t.Source ?? "").ToLower().Contains(word)
                                                   select t).ToList();
                                    }
                                }
                            }
                            if (qIgnore.Length > 0)
                            {
                                results = (from t in results
                                           where
                                               !(t.Source ?? "").ToLower().Contains(qIgnore)
                                           select t).ToList();
                            }
                            break;
                        }
                    case "description":
                        {
                            if (wholeWordOnly)
                            {
                                results = (from t in opacTitles
                                           where (t.Description ?? "").ToLower() == q || (t.Description ?? "").ToLower().Contains(" " + q + " ") || (t.Description ?? "").ToLower().StartsWith(q + " ") || (t.Description ?? "").ToLower().EndsWith(" " + q)
                                           select t).ToList();
                            }
                            else if (prefixSearch) // the default
                            {
                                var qWords = q.Split(' ').Except(wordsToRemove);
                                foreach (var word in qWords)
                                {
                                    if (orSearch)
                                    {
                                        results = results.Concat(from t in opacTitles
                                                                 where
                                                                     (t.Description ?? "").ToLower().Contains(" " + word) ||
                                                                     (t.Description ?? "").ToLower().StartsWith(word)
                                                                 select t).ToList();
                                    }
                                    else
                                    {
                                        results = (from t in results
                                                   where
                                                       (t.Description ?? "").ToLower().Contains(" " + word) ||
                                                       (t.Description ?? "").ToLower().StartsWith(word)
                                                   select t).ToList();
                                    }
                                }
                            }
                            else if (suffixSearch)
                            {
                                var qWords = q.Split(' ').Except(wordsToRemove);
                                foreach (var word in qWords)
                                {
                                    if (orSearch)
                                    {
                                        results = results.Concat(from t in opacTitles
                                                                 where
                                                                     (t.Description ?? "").ToLower().Contains(word + " ") ||
                                                                     (t.Description ?? "").ToLower().EndsWith(word)
                                                                 select t).ToList();
                                    }
                                    else
                                    {
                                        results = (from t in results
                                                   where
                                                       (t.Description ?? "").ToLower().Contains(word + " ") ||
                                                       (t.Description ?? "").ToLower().EndsWith(word)
                                                   select t).ToList();
                                    }
                                }
                            }
                            else
                            {
                                var qWords = q.Split(' ').Except(wordsToRemove);
                                foreach (var word in qWords)
                                {
                                    if (orSearch)
                                    {
                                        results = results.Concat(from t in opacTitles
                                                                 where
                                                                     (t.Description ?? "").ToLower().Contains(word)
                                                                 select t).ToList();
                                    }
                                    else
                                    {
                                        results = (from t in results
                                                   where
                                                       (t.Description ?? "").ToLower().Contains(word)
                                                   select t).ToList();
                                    }
                                }
                            }
                            if (qIgnore.Length > 0)
                            {
                                results = (from t in results
                                           where
                                               !(t.Description ?? "").ToLower().Contains(qIgnore)
                                           select t).ToList();
                            }
                            break;
                        }
                    case "series":
                        {
                            if (wholeWordOnly)
                            {
                                results = (from t in opacTitles
                                           where (t.Series ?? "").ToLower() == q || (t.Series ?? "").ToLower().Contains(" " + q + " ") || (t.Series ?? "").ToLower().StartsWith(q + " ") || (t.Series ?? "").ToLower().EndsWith(" " + q)
                                           select t).ToList();
                            }
                            else if (prefixSearch) // the default
                            {
                                var qWords = q.Split(' ').Except(wordsToRemove);
                                foreach (var word in qWords)
                                {
                                    if (orSearch)
                                    {
                                        results = results.Concat(from t in opacTitles
                                                                 where
                                                                     (t.Series ?? "").ToLower().Contains(" " + word) ||
                                                                     (t.Series ?? "").ToLower().StartsWith(word)
                                                                 select t).ToList();
                                    }
                                    else
                                    {
                                        results = (from t in results
                                                   where
                                                       (t.Series ?? "").ToLower().Contains(" " + word) ||
                                                       (t.Series ?? "").ToLower().StartsWith(word)
                                                   select t).ToList();
                                    }
                                }
                            }
                            else if (suffixSearch)
                            {
                                var qWords = q.Split(' ').Except(wordsToRemove);
                                foreach (var word in qWords)
                                {
                                    if (orSearch)
                                    {
                                        results = results.Concat(from t in opacTitles
                                                                 where
                                                                     (t.Series ?? "").ToLower().Contains(word + " ") ||
                                                                     (t.Series ?? "").ToLower().EndsWith(word)
                                                                 select t).ToList();
                                    }
                                    else
                                    {
                                        results = (from t in results
                                                   where
                                                       (t.Series ?? "").ToLower().Contains(word + " ") ||
                                                       (t.Series ?? "").ToLower().EndsWith(word)
                                                   select t).ToList();
                                    }
                                }
                            }
                            else
                            {
                                var qWords = q.Split(' ').Except(wordsToRemove);
                                foreach (var word in qWords)
                                {
                                    if (orSearch)
                                    {
                                        results = results.Concat(from t in opacTitles
                                                                 where
                                                                     (t.Series ?? "").ToLower().Contains(word)
                                                                 select t).ToList();
                                    }
                                    else
                                    {
                                        results = (from t in results
                                                   where
                                                       (t.Series ?? "").ToLower().Contains(word)
                                                   select t).ToList();
                                    }
                                }
                            }
                            if (qIgnore.Length > 0)
                            {
                                results = (from t in results
                                           where
                                               !(t.Series ?? "").ToLower().Contains(qIgnore)
                                           select t).ToList();
                            }
                            break;
                        }
                    case "edition":
                        {
                            if (wholeWordOnly)
                            {
                                results = (from t in opacTitles
                                           where (t.Edition ?? "").ToLower() == q || (t.Edition ?? "").ToLower().Contains(" " + q + " ") || (t.Edition ?? "").ToLower().StartsWith(q + " ") || (t.Edition ?? "").ToLower().EndsWith(" " + q)
                                           select t).ToList();
                            }
                            else if (prefixSearch) // the default
                            {
                                var qWords = q.Split(' ').Except(wordsToRemove);
                                foreach (var word in qWords)
                                {
                                    if (orSearch)
                                    {
                                        results = results.Concat(from t in opacTitles
                                                                 where
                                                                     (t.Edition ?? "").ToLower().Contains(" " + word) ||
                                                                     (t.Edition ?? "").ToLower().StartsWith(word)
                                                                 select t).ToList();
                                    }
                                    else
                                    {
                                        results = (from t in results
                                                   where
                                                       (t.Edition ?? "").ToLower().Contains(" " + word) ||
                                                       (t.Edition ?? "").ToLower().StartsWith(word)
                                                   select t).ToList();
                                    }
                                }
                            }
                            else if (suffixSearch)
                            {
                                var qWords = q.Split(' ').Except(wordsToRemove);
                                foreach (var word in qWords)
                                {
                                    if (orSearch)
                                    {
                                        results = results.Concat(from t in opacTitles
                                                                 where
                                                                     (t.Edition ?? "").ToLower().Contains(word + " ") ||
                                                                     (t.Edition ?? "").ToLower().EndsWith(word)
                                                                 select t).ToList();
                                    }
                                    else
                                    {
                                        results = (from t in results
                                                   where
                                                       (t.Edition ?? "").ToLower().Contains(word + " ") ||
                                                       (t.Edition ?? "").ToLower().EndsWith(word)
                                                   select t).ToList();
                                    }
                                }
                            }
                            else
                            {
                                var qWords = q.Split(' ').Except(wordsToRemove);
                                foreach (var word in qWords)
                                {
                                    if (orSearch)
                                    {
                                        results = results.Concat(from t in opacTitles
                                                                 where
                                                                     (t.Edition ?? "").ToLower().Contains(word)
                                                                 select t).ToList();
                                    }
                                    else
                                    {
                                        results = (from t in results
                                                   where
                                                       (t.Edition ?? "").ToLower().Contains(word)
                                                   select t).ToList();
                                    }
                                }
                            }
                            if (qIgnore.Length > 0)
                            {
                                results = (from t in results
                                           where
                                               !(t.Edition ?? "").ToLower().Contains(qIgnore)
                                           select t).ToList();
                            }
                            break;
                        }
                    case "isbn":
                        {
                            if (wholeWordOnly)
                            {
                                results = (from t in opacTitles
                                           where (t.Isbn ?? "").ToLower() == q || (t.Isbn ?? "").ToLower().Contains(" " + q + " ") || (t.Isbn ?? "").ToLower().StartsWith(q + " ") || (t.Isbn ?? "").ToLower().EndsWith(" " + q)
                                           select t).ToList();
                            }
                            else if (prefixSearch) // the default
                            {
                                var qWords = q.Split(' ').Except(wordsToRemove);
                                foreach (var word in qWords)
                                {
                                    if (orSearch)
                                    {
                                        results = results.Concat(from t in opacTitles
                                                                 where
                                                                     (t.Isbn ?? "").ToLower().Contains(" " + word) ||
                                                                     (t.Isbn ?? "").ToLower().StartsWith(word)
                                                                 select t).ToList();
                                    }
                                    else
                                    {
                                        results = (from t in results
                                                   where
                                                       (t.Isbn ?? "").ToLower().Contains(" " + word) ||
                                                       (t.Isbn ?? "").ToLower().StartsWith(word)
                                                   select t).ToList();
                                    }
                                }
                            }
                            else if (suffixSearch)
                            {
                                var qWords = q.Split(' ').Except(wordsToRemove);
                                foreach (var word in qWords)
                                {
                                    if (orSearch)
                                    {
                                        results = results.Concat(from t in opacTitles
                                                                 where
                                                                     (t.Isbn ?? "").ToLower().Contains(word + " ") ||
                                                                     (t.Isbn ?? "").ToLower().EndsWith(word)
                                                                 select t).ToList();
                                    }
                                    else
                                    {
                                        results = (from t in results
                                                   where
                                                       (t.Isbn ?? "").ToLower().Contains(word + " ") ||
                                                       (t.Isbn ?? "").ToLower().EndsWith(word)
                                                   select t).ToList();
                                    }
                                }
                            }
                            else
                            {
                                var qWords = q.Split(' ').Except(wordsToRemove);
                                foreach (var word in qWords)
                                {
                                    if (orSearch)
                                    {
                                        results = results.Concat(from t in opacTitles
                                                                 where
                                                                     (t.Isbn ?? "").ToLower().Contains(word)
                                                                 select t).ToList();
                                    }
                                    else
                                    {
                                        results = (from t in results
                                                   where
                                                       (t.Isbn ?? "").ToLower().Contains(word)
                                                   select t).ToList();
                                    }
                                }
                            }
                            if (qIgnore.Length > 0)
                            {
                                results = (from t in results
                                           where
                                               !(t.Isbn ?? "").ToLower().Contains(qIgnore)
                                           select t).ToList();
                            }
                            break;
                        }
                    case "keywords":
                        {
                            if (wholeWordOnly)
                            {
                                results = (from t in opacTitles
                                           where (t.KeywordString ?? "").ToLower() == q || (t.KeywordString ?? "").ToLower().Contains(" " + q + " ") || (t.KeywordString ?? "").ToLower().StartsWith(q + " ") || (t.KeywordString ?? "").ToLower().EndsWith(" " + q)
                                           select t).ToList();
                            }
                            else if (prefixSearch) // the default
                            {
                                var qWords = q.Split(' ').Except(wordsToRemove);
                                foreach (var word in qWords)
                                {
                                    if (orSearch)
                                    {
                                        results = results.Concat(from t in opacTitles
                                                                 where
                                                                     (t.KeywordString ?? "").ToLower().Contains(" " + word) ||
                                                                     (t.KeywordString ?? "").ToLower().StartsWith(word)
                                                                 select t).ToList();
                                    }
                                    else
                                    {
                                        results = (from t in results
                                                   where
                                                       (t.KeywordString ?? "").ToLower().Contains(" " + word) ||
                                                       (t.KeywordString ?? "").ToLower().StartsWith(word)
                                                   select t).ToList();
                                    }
                                }
                            }
                            else if (suffixSearch)
                            {
                                var qWords = q.Split(' ').Except(wordsToRemove);
                                foreach (var word in qWords)
                                {
                                    if (orSearch)
                                    {
                                        results = results.Concat(from t in opacTitles
                                                                 where
                                                                     (t.KeywordString ?? "").ToLower().Contains(word + " ") ||
                                                                     (t.KeywordString ?? "").ToLower().EndsWith(word)
                                                                 select t).ToList();
                                    }
                                    else
                                    {
                                        results = (from t in results
                                                   where
                                                       (t.KeywordString ?? "").ToLower().Contains(word + " ") ||
                                                       (t.KeywordString ?? "").ToLower().EndsWith(word)
                                                   select t).ToList();
                                    }
                                }
                            }
                            else
                            {
                                var qWords = q.Split(' ').Except(wordsToRemove);
                                foreach (var word in qWords)
                                {
                                    if (orSearch)
                                    {
                                        results = results.Concat(from t in opacTitles
                                                                 where
                                                                     (t.KeywordString ?? "").ToLower().Contains(word)
                                                                 select t).ToList();
                                    }
                                    else
                                    {
                                        results = (from t in results
                                                   where
                                                       (t.KeywordString ?? "").ToLower().Contains(word)
                                                   select t).ToList();
                                    }
                                }
                            }
                            if (qIgnore.Length > 0)
                            {
                                results = (from t in results
                                           where
                                               !(t.KeywordString ?? "").ToLower().Contains(qIgnore)
                                           select t).ToList();
                            }
                            break;
                        }
                    case "links":
                        {
                            if (wholeWordOnly)
                            {
                                results = (from t in opacTitles
                                           where (t.LinkString ?? "").ToLower() == q || (t.LinkString ?? "").ToLower().Contains(" " + q + " ") || (t.LinkString ?? "").ToLower().StartsWith(q + " ") || (t.LinkString ?? "").ToLower().EndsWith(" " + q)
                                           select t).ToList();
                            }
                            else if (prefixSearch) // the default
                            {
                                var qWords = q.Split(' ').Except(wordsToRemove);
                                foreach (var word in qWords)
                                {
                                    if (orSearch)
                                    {
                                        results = results.Concat(from t in opacTitles
                                                                 where
                                                                     (t.LinkString ?? "").ToLower().Contains(" " + word) ||
                                                                     (t.LinkString ?? "").ToLower().StartsWith(word)
                                                                 select t).ToList();
                                    }
                                    else
                                    {
                                        results = (from t in results
                                                   where
                                                       (t.LinkString ?? "").ToLower().Contains(" " + word) ||
                                                       (t.LinkString ?? "").ToLower().StartsWith(word)
                                                   select t).ToList();
                                    }
                                }
                            }
                            else if (suffixSearch)
                            {
                                var qWords = q.Split(' ').Except(wordsToRemove);
                                foreach (var word in qWords)
                                {
                                    if (orSearch)
                                    {
                                        results = results.Concat(from t in opacTitles
                                                                 where
                                                                     (t.LinkString ?? "").ToLower().Contains(word + " ") ||
                                                                     (t.LinkString ?? "").ToLower().EndsWith(word)
                                                                 select t).ToList();
                                    }
                                    else
                                    {
                                        results = (from t in results
                                                   where
                                                       (t.LinkString ?? "").ToLower().Contains(word + " ") ||
                                                       (t.LinkString ?? "").ToLower().EndsWith(word)
                                                   select t).ToList();
                                    }
                                }
                            }
                            else
                            {
                                var qWords = q.Split(' ').Except(wordsToRemove);
                                foreach (var word in qWords)
                                {
                                    if (orSearch)
                                    {
                                        results = results.Concat(from t in opacTitles
                                                                 where
                                                                     (t.LinkString ?? "").ToLower().Contains(word)
                                                                 select t).ToList();
                                    }
                                    else
                                    {
                                        results = (from t in results
                                                   where
                                                       (t.LinkString ?? "").ToLower().Contains(word)
                                                   select t).ToList();
                                    }
                                }
                            }
                            if (qIgnore.Length > 0)
                            {
                                results = (from t in results
                                           where
                                               !(t.LinkString ?? "").ToLower().Contains(qIgnore)
                                           select t).ToList();
                            }
                            break;
                        }
                    case "titletexts":
                        {
                            if (wholeWordOnly)
                            {
                                results = (from t in opacTitles
                                           where (t.TitleTextString ?? "").ToLower() == q || (t.TitleTextString ?? "").ToLower().Contains(" " + q + " ") || (t.TitleTextString ?? "").ToLower().StartsWith(q + " ") || (t.TitleTextString ?? "").ToLower().EndsWith(" " + q)
                                           select t).ToList();
                            }
                            else if (prefixSearch) // the default
                            {
                                var qWords = q.Split(' ').Except(wordsToRemove);
                                foreach (var word in qWords)
                                {
                                    if (orSearch)
                                    {
                                        results = results.Concat(from t in opacTitles
                                                                 where
                                                                     (t.TitleTextString ?? "").ToLower().Contains(" " + word) ||
                                                                     (t.TitleTextString ?? "").ToLower().StartsWith(word)
                                                                 select t).ToList();
                                    }
                                    else
                                    {
                                        results = (from t in results
                                                   where
                                                       (t.TitleTextString ?? "").ToLower().Contains(" " + word) ||
                                                       (t.TitleTextString ?? "").ToLower().StartsWith(word)
                                                   select t).ToList();
                                    }
                                }
                            }
                            else if (suffixSearch)
                            {
                                var qWords = q.Split(' ').Except(wordsToRemove);
                                foreach (var word in qWords)
                                {
                                    if (orSearch)
                                    {
                                        results = results.Concat(from t in opacTitles
                                                                 where
                                                                     (t.TitleTextString ?? "").ToLower().Contains(word + " ") ||
                                                                     (t.TitleTextString ?? "").ToLower().EndsWith(word)
                                                                 select t).ToList();
                                    }
                                    else
                                    {
                                        results = (from t in results
                                                   where
                                                       (t.TitleTextString ?? "").ToLower().Contains(word + " ") ||
                                                       (t.TitleTextString ?? "").ToLower().EndsWith(word)
                                                   select t).ToList();
                                    }
                                }
                            }
                            else
                            {
                                var qWords = q.Split(' ').Except(wordsToRemove);
                                foreach (var word in qWords)
                                {
                                    if (orSearch)
                                    {
                                        results = results.Concat(from t in opacTitles
                                                                 where
                                                                     (t.TitleTextString ?? "").ToLower().Contains(word)
                                                                 select t).ToList();
                                    }
                                    else
                                    {
                                        results = (from t in results
                                                   where
                                                       (t.TitleTextString ?? "").ToLower().Contains(word)
                                                   select t).ToList();
                                    }
                                }
                            }
                            if (qIgnore.Length > 0)
                            {
                                results = (from t in results
                                           where
                                               !(t.TitleTextString ?? "").ToLower().Contains(qIgnore)
                                           select t).ToList();
                            }
                            break;
                        }
                    case "customdata":
                        if (wholeWordOnly)
                        {
                            results = (from t in opacTitles
                                       where (t.CustomDataString ?? "").ToLower() == q || (t.CustomDataString ?? "").ToLower().Contains(" " + q + " ") || (t.CustomDataString ?? "").ToLower().StartsWith(q + " ") || (t.CustomDataString ?? "").ToLower().EndsWith(" " + q)
                                       select t).ToList();
                        }
                        else if (prefixSearch) // the default
                        {
                            var qWords = q.Split(' ').Except(wordsToRemove);
                            foreach (var word in qWords)
                            {
                                if (orSearch)
                                {
                                    results = results.Concat(from t in opacTitles
                                                             where
                                                                 (t.CustomDataString ?? "").ToLower().Contains(" " + word) ||
                                                                 (t.CustomDataString ?? "").ToLower().StartsWith(word)
                                                             select t).ToList();
                                }
                                else
                                {
                                    results = (from t in results
                                               where
                                                   (t.CustomDataString ?? "").ToLower().Contains(" " + word) ||
                                                   (t.CustomDataString ?? "").ToLower().StartsWith(word)
                                               select t).ToList();
                                }
                            }
                        }
                        else if (suffixSearch)
                        {
                            var qWords = q.Split(' ').Except(wordsToRemove);
                            foreach (var word in qWords)
                            {
                                if (orSearch)
                                {
                                    results = results.Concat(from t in opacTitles
                                                             where
                                                                 (t.CustomDataString ?? "").ToLower().Contains(word + " ") ||
                                                                 (t.CustomDataString ?? "").ToLower().EndsWith(word)
                                                             select t).ToList();
                                }
                                else
                                {
                                    results = (from t in results
                                               where
                                                   (t.CustomDataString ?? "").ToLower().Contains(word + " ") ||
                                                   (t.CustomDataString ?? "").ToLower().EndsWith(word)
                                               select t).ToList();
                                }
                            }
                        }
                        else
                        {
                            var qWords = q.Split(' ').Except(wordsToRemove);
                            foreach (var word in qWords)
                            {
                                if (orSearch)
                                {
                                    results = results.Concat(from t in opacTitles
                                                             where
                                                                 (t.CustomDataString ?? "").ToLower().Contains(word)
                                                             select t).ToList();
                                }
                                else
                                {
                                    results = (from t in results
                                               where
                                                   (t.CustomDataString ?? "").ToLower().Contains(word)
                                               select t).ToList();
                                }
                            }
                        }
                        if (qIgnore.Length > 0)
                        {
                            results = (from t in results
                                       where
                                           !(t.CustomDataString ?? "").ToLower().Contains(qIgnore)
                                       select t).ToList();
                        }
                        break;
                    case "notes":
                        {
                            if (wholeWordOnly)
                            {
                                results = (from t in opacTitles
                                           where (t.Notes ?? "").ToLower() == q || (t.Notes ?? "").ToLower().Contains(" " + q + " ") || (t.Notes ?? "").ToLower().StartsWith(q + " ") || (t.Notes ?? "").ToLower().EndsWith(" " + q)
                                           select t).ToList();
                            }
                            else if (prefixSearch) // the default
                            {
                                var qWords = q.Split(' ').Except(wordsToRemove);
                                foreach (var word in qWords)
                                {
                                    if (orSearch)
                                    {
                                        results = results.Concat(from t in opacTitles
                                                                 where
                                                                     (t.Notes ?? "").ToLower().Contains(" " + word) ||
                                                                     (t.Notes ?? "").ToLower().StartsWith(word)
                                                                 select t).ToList();
                                    }
                                    else
                                    {
                                        results = (from t in results
                                                   where
                                                       (t.Notes ?? "").ToLower().Contains(" " + word) ||
                                                       (t.Notes ?? "").ToLower().StartsWith(word)
                                                   select t).ToList();
                                    }
                                }
                            }
                            else if (suffixSearch)
                            {
                                var qWords = q.Split(' ').Except(wordsToRemove);
                                foreach (var word in qWords)
                                {
                                    if (orSearch)
                                    {
                                        results = results.Concat(from t in opacTitles
                                                                 where
                                                                     (t.Notes ?? "").ToLower().Contains(word + " ") ||
                                                                     (t.Notes ?? "").ToLower().EndsWith(word)
                                                                 select t).ToList();
                                    }
                                    else
                                    {
                                        results = (from t in results
                                                   where
                                                       (t.Notes ?? "").ToLower().Contains(word + " ") ||
                                                       (t.Notes ?? "").ToLower().EndsWith(word)
                                                   select t).ToList();
                                    }
                                }
                            }
                            else
                            {
                                var qWords = q.Split(' ').Except(wordsToRemove);
                                foreach (var word in qWords)
                                {
                                    if (orSearch)
                                    {
                                        results = results.Concat(from t in opacTitles
                                                                 where
                                                                     (t.Notes ?? "").ToLower().Contains(word)
                                                                 select t).ToList();
                                    }
                                    else
                                    {
                                        results = (from t in results
                                                   where
                                                       (t.Notes ?? "").ToLower().Contains(word)
                                                   select t).ToList();
                                    }
                                }
                            }
                            if (qIgnore.Length > 0)
                            {
                                results = (from t in results
                                           where
                                               !(t.Notes ?? "").ToLower().Contains(qIgnore)
                                           select t).ToList();
                            }
                            break;
                        }
                    case "all":
                        {
                            if (wholeWordOnly)
                            {
                                results = (from t in opacTitles
                                           where (t.SearchString ?? "").ToLower().Contains(" " + q + " ") || (t.SearchString ?? "").ToLower().StartsWith(q + " ") || (t.SearchString ?? "").ToLower().EndsWith(" " + q)
                                           select t).ToList();
                            }
                            else if (prefixSearch) // the default
                            {
                                var qWords = q.Split(' ').Except(wordsToRemove);
                                foreach (var word in qWords)
                                {
                                    if (orSearch)
                                    {
                                        results = results.Concat(from t in opacTitles
                                                                 where
                                                                     (t.SearchString ?? "").ToLower().Contains(" " + word) ||
                                                                     (t.SearchString ?? "").ToLower().StartsWith(word)
                                                                 select t).ToList();
                                    }
                                    else
                                    {
                                        results = (from t in results
                                                   where
                                                       (t.SearchString ?? "").ToLower().Contains(" " + word) ||
                                                       (t.SearchString ?? "").ToLower().StartsWith(word)
                                                   select t).ToList();
                                    }
                                }
                            }
                            else if (suffixSearch)
                            {
                                var qWords = q.Split(' ').Except(wordsToRemove);
                                foreach (var word in qWords)
                                {
                                    if (orSearch)
                                    {
                                        results = results.Concat(from t in opacTitles
                                                                 where
                                                                     (t.SearchString ?? "").ToLower().Contains(word + " ") ||
                                                                     (t.SearchString ?? "").ToLower().EndsWith(word)
                                                                 select t).ToList();
                                    }
                                    else
                                    {
                                        results = (from t in results
                                                   where
                                                       (t.SearchString ?? "").ToLower().Contains(word + " ") ||
                                                       (t.SearchString ?? "").ToLower().EndsWith(word)
                                                   select t).ToList();
                                    }
                                }
                            }
                            else
                            {
                                var qWords = q.Split(' ').Except(wordsToRemove);
                                foreach (var word in qWords)
                                {
                                    if (orSearch)
                                    {
                                        results = results.Concat(from t in opacTitles
                                                                 where
                                                                     (t.SearchString ?? "").ToLower().Contains(word)
                                                                 select t).ToList();
                                    }
                                    else
                                    {
                                        results = (from t in results
                                                   where
                                                       (t.SearchString ?? "").ToLower().Contains(word)
                                                   select t).ToList();
                                    }
                                }
                            }
                            if (qIgnore.Length > 0)
                            {
                                results = (from t in results
                                           where
                                               !(t.SearchString ?? "").ToLower().Contains(qIgnore)
                                           select t).ToList();
                            }
                            break;
                        }
                    default:
                        {
                            if (wholeWordOnly)
                            {
                                results = (from t in opacTitles
                                           where (t.Title1 ?? "").ToLower() == q || (t.Title1 ?? "").ToLower().Contains(" " + q + " ") || (t.Title1 ?? "").ToLower().StartsWith(q + " ") || (t.Title1 ?? "").ToLower().EndsWith(" " + q)
                                           select t).ToList();
                            }
                            else if (prefixSearch) // the default
                            {
                                var qWords = q.Split(' ').Except(wordsToRemove);
                                foreach (var word in qWords)
                                {
                                    if (orSearch)
                                    {
                                        results = results.Concat(from t in opacTitles
                                                                 where
                                                                     (t.Title1 ?? "").ToLower().Contains(" " + word) ||
                                                                     (t.Title1 ?? "").ToLower().StartsWith(word)
                                                                 select t).ToList();
                                    }
                                    else
                                    {
                                        results = (from t in results
                                                   where
                                                       (t.Title1 ?? "").ToLower().Contains(" " + word) ||
                                                       (t.Title1 ?? "").ToLower().StartsWith(word)
                                                   select t).ToList();
                                    }
                                }
                            }
                            else if (suffixSearch)
                            {
                                var qWords = q.Split(' ').Except(wordsToRemove);
                                foreach (var word in qWords)
                                {
                                    if (orSearch)
                                    {
                                        results = results.Concat(from t in opacTitles
                                                                 where
                                                                     (t.Title1 ?? "").ToLower().Contains(word + " ") ||
                                                                     (t.Title1 ?? "").ToLower().EndsWith(word)
                                                                 select t).ToList();
                                    }
                                    else
                                    {
                                        results = (from t in results
                                                   where
                                                       (t.Title1 ?? "").ToLower().Contains(word + " ") ||
                                                       (t.Title1 ?? "").ToLower().EndsWith(word)
                                                   select t).ToList();
                                    }
                                }
                            }
                            else
                            {
                                var qWords = q.Split(' ').Except(wordsToRemove);
                                foreach (var word in qWords)
                                {
                                    if (orSearch)
                                    {
                                        results = results.Concat(from t in opacTitles
                                                                 where
                                                                     (t.Title1 ?? "").ToLower().Contains(word)
                                                                 select t).ToList();
                                    }
                                    else
                                    {
                                        results = (from t in results
                                                   where
                                                       (t.Title1 ?? "").ToLower().Contains(word)
                                                   select t).ToList();
                                    }
                                }
                            }
                            if (qIgnore.Length > 0)
                            {
                                results = (from t in results
                                           where
                                               !(t.Title1 ?? "").ToLower().Contains(qIgnore)
                                           select t).ToList();
                            }
                            break;
                        }
                }

                //Order the results ...
                results = results.OrderBy(r => r.Title1.Substring(r.NonFilingChars)).Distinct().ToList();

                if (selectedMediaIds.Any())
                {
                    viewModel.ResultsBeforeFilter = results;

                    results = results.Where(r => selectedMediaIds.Contains(r.MediaID)).ToList();
                    foreach (var mediaId in selectedMediaIds)
                    {
                        viewModel.Filters.Add(CacheProvider.GetAll<MediaType>("mediatypes").FirstOrDefault(x => x.MediaID == mediaId).Media);
                    }
                }

                if (selectedClassmarkIds.Any())
                {
                    viewModel.ResultsBeforeFilter = results;

                    results = results.Where(r => selectedClassmarkIds.Contains(r.ClassmarkID)).ToList();
                    foreach (var classmarkId in selectedClassmarkIds)
                    {
                        viewModel.Filters.Add(CacheProvider.GetAll<Classmark>("classmarks").FirstOrDefault(x => x.ClassmarkID == classmarkId).Classmark1);
                    }
                }

                if (selectedPublisherIds.Any())
                {
                    viewModel.ResultsBeforeFilter = results;

                    results = results.Where(r => selectedPublisherIds.Contains(r.PublisherID)).ToList();
                    foreach (var publisherId in selectedPublisherIds)
                    {
                        viewModel.Filters.Add(CacheProvider.GetAll<Publisher>("publishers").FirstOrDefault(x => x.PublisherID == publisherId).PublisherName);
                    }
                }

                if (selectedLanguageIds.Any())
                {
                    viewModel.ResultsBeforeFilter = results;

                    results = results.Where(r => selectedLanguageIds.Contains(r.LanguageID)).ToList();
                    foreach (var languageId in selectedLanguageIds)
                    {
                        viewModel.Filters.Add(CacheProvider.GetAll<Language>("languages").FirstOrDefault(x => x.LanguageID == languageId).Language1);
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
                        viewModel.Filters.Add(CacheProvider.GetAll<Keyword>("keywords").FirstOrDefault(k => k.KeywordID == keywordId).KeywordTerm);
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
                        viewModel.Filters.Add(CacheProvider.GetAll<Author>("authors").FirstOrDefault(a => a.AuthorID == authorId).DisplayName);
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
                TempData["LangaugeFilter"] = viewModel.LanguageFilter;


                //5. Get the keywords to narrow by ... NOT CURRENTLY USED AS TOO MANY KEYWORDS !
                //Get a list of any keywords associated with titles in the search results ...
                //viewModel.KeywordFilter.Clear();
                //ModelState.Clear();

                //var subjectIndexIds = results.Select(item => item.SubjectIndexes).ToList();

                //List<int> keywordIds;

                //if (selectedKeywordIds.Any())
                //{
                //    keywordIds = selectedKeywordIds;
                //}
                //else
                //{
                //    keywordIds = subjectIndexIds.Select(item =>
                //    {
                //        var firstOrDefault = item.FirstOrDefault();
                //        return firstOrDefault != null ? firstOrDefault.KeywordID : 0;
                //    }).Distinct().ToList();
                //}

                //if (keywordIds.Any())
                //{
                //    var keywords = (from keyword in CacheProvider.GetAll<Keyword>("keywords")
                //                    where keywordIds.Contains(keyword.KeywordID)
                //                     select keyword).ToList();

                //    viewModel.KeywordFilter.Clear();
                //    //var take = viewModel.NarrowByDefaultRecordCount;
                //    if (selectedKeywordIds.Any())
                //    {
                //        take = 9999;
                //    }

                //    foreach (var item in keywords.OrderByDescending(x => x.SubjectIndexes.Count).Take(take))
                //    {
                //        item.TitleCount = results.Count(r =>
                //        {
                //            var firstOrDefault = r.SubjectIndexes.FirstOrDefault();
                //            return firstOrDefault != null && firstOrDefault.KeywordID == item.KeywordID;
                //        });

                //        var editorViewModel = new SelectKeywordEditorViewModel()
                //        {
                //            Id = item.KeywordID,
                //            Name = item.KeywordTerm,
                //            Selected = selectedLanguageIds.Contains(item.KeywordID),
                //            TitleCount = item.TitleCount
                //        };
                //        viewModel.KeywordFilter.Add(editorViewModel);
                //    }
                //}
                //TempData["KeywordFilter"] = viewModel.KeywordFilter;

                //6. Get the authors to narrow by ...
                //Get a list of any authors associated with titles in the search results ...
                viewModel.AuthorFilter.Clear();
                ModelState.Clear();

                List<int> authorIds;

                if (selectedAuthorIds.Any())
                {
                    authorIds = selectedAuthorIds;
                }
                else
                {
                    var titleAuthorIds = results.Select(item => item.TitleAuthors).Distinct().ToList();
                    authorIds = titleAuthorIds.Select(item =>
                    {
                        var firstOrDefault = item.FirstOrDefault();
                        return firstOrDefault != null ? firstOrDefault.AuthorId : 0;
                    }).Distinct().ToList();
                }
                //var authorIds = selectedAuthorIds.Any() ? selectedAuthorIds : results.Select(item => item.TitleAuthors).Distinct().ToList();

                if (authorIds.Any())
                {
                    var authors = (from author in CacheProvider.GetAll<Author>("authors")
                                   where authorIds.Contains(author.AuthorID)
                                   select author).ToList();

                    viewModel.AuthorFilter.Clear();
                    //var take = viewModel.NarrowByDefaultRecordCount;
                    if (selectedAuthorIds.Any())
                    {
                        take = 9999;
                    }

                    foreach (var item in authors.OrderByDescending(x => x.TitleAuthors.Count).Take(take))
                    {
                        item.TitleCount = results.Count(r =>
                        {
                            var firstOrDefault = r.TitleAuthors.FirstOrDefault();
                            return firstOrDefault != null && firstOrDefault.AuthorId == item.AuthorID;
                        });

                        var editorViewModel = new SelectAuthorEditorViewModel()
                        {
                            Id = item.AuthorID,
                            Name = item.DisplayName,
                            Selected = selectedLanguageIds.Contains(item.AuthorID),
                            TitleCount = item.TitleCount
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

            viewModel.LibraryStaff = Roles.IsLibraryStaff();
            ViewData["SearchField"] = SelectListHelper.SearchFieldsList(viewModel.SearchField);
            ViewBag.Title = !string.IsNullOrEmpty(q) ? "Search Results" : "Search the Library";
            TempData["simpleSearchingViewModel"] = viewModel;

            return View("SimpleSearch", viewModel);
        }


        [HttpGet]
        public ActionResult ShowAllClassmarksFilter()
        {
            var viewModel = (SimpleSearchingViewModel)TempData["simpleSearchingViewModel"];
            var classmarks = CacheProvider.GetAll<Classmark>("classmarks").ToList();
            var selectedClassmarkIds = viewModel.GetSelectedClassmarkIds().ToList();

            viewModel.ClassmarksFilter.Clear();

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
        public ActionResult ShowAllMediaFilter()
        {
            var viewModel = (SimpleSearchingViewModel)TempData["simpleSearchingViewModel"];
            var mediatypes = CacheProvider.GetAll<MediaType>("mediatypes").ToList();
            var selectedMediaIds = viewModel.GetSelectedMediaIds().ToList();

            viewModel.MediaFilter.Clear();

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
        public ActionResult ShowAllPublishersFilter()
        {
            var viewModel = (SimpleSearchingViewModel)TempData["simpleSearchingViewModel"];
            var publishers = CacheProvider.GetAll<Publisher>("publishers").ToList();
            var selectedPublisherIds = viewModel.GetSelectedPublisherIds().ToList();

            viewModel.PublisherFilter.Clear();

            foreach (var item in publishers.OrderBy(x => x.PublisherName))
            {
                var editorViewModel = new SelectPublisherEditorViewModel()
                {
                    Id = item.PublisherID,
                    Name = item.PublisherName,
                    Selected = selectedPublisherIds.Contains(item.PublisherID),
                    //TitleCount = viewModel.Results.Count(r => r.PublisherID == item.PublisherID)
                    TitleCount = viewModel.ResultsBeforeFilter.Any() ? viewModel.ResultsBeforeFilter.Count(r => r.PublisherID == item.PublisherID) : viewModel.Results.Count(r => r.PublisherID == item.PublisherID)
                };
                viewModel.PublisherFilter.Add(editorViewModel);
            }

            TempData["simpleSearchingViewModel"] = viewModel;
            ViewBag.Title = "Narrow by " + @DbRes.T("Publishers.Publisher", "FieldDisplayName");
            return PartialView(viewModel);
        }

        [HttpGet]
        public ActionResult ShowAllLanguagesFilter()
        {
            var viewModel = (SimpleSearchingViewModel)TempData["simpleSearchingViewModel"];
            var languages = CacheProvider.GetAll<Language>("languages").ToList();
            var selectedLanguageIds = viewModel.GetSelectedLanguageIds().ToList();

            viewModel.LanguageFilter.Clear();

            foreach (var item in languages.OrderBy(x => x.Language1))
            {
                var editorViewModel = new SelectLanguageEditorViewModel()
                {
                    Id = item.LanguageID,
                    Name = item.Language1,
                    Selected = selectedLanguageIds.Contains(item.LanguageID),
                    //TitleCount = viewModel.Results.Count(r => r.LanguageID == item.LanguageID)
                    TitleCount = viewModel.ResultsBeforeFilter.Any() ? viewModel.ResultsBeforeFilter.Count(r => r.LanguageID == item.LanguageID) : viewModel.Results.Count(r => r.LanguageID == item.LanguageID)
                };
                viewModel.LanguageFilter.Add(editorViewModel);
            }

            TempData["simpleSearchingViewModel"] = viewModel;
            ViewBag.Title = "Narrow by " + @DbRes.T("Languages.Language", "FieldDisplayName");
            return PartialView(viewModel);
        }

        [HttpGet]
        public ActionResult ShowAllKeywordsFilter()
        {
            var viewModel = (SimpleSearchingViewModel)TempData["simpleSearchingViewModel"];
            var keywords = CacheProvider.GetAll<Keyword>("keywords").ToList();
            var selectedKeywordIds = viewModel.GetSelectedKeywordIds().ToList();

            viewModel.KeywordFilter.Clear();

            foreach (var item in keywords.OrderBy(x => x.KeywordTerm))
            {
                var editorViewModel = new SelectKeywordEditorViewModel()
                {
                    Id = item.KeywordID,
                    Name = item.KeywordTerm,
                    Selected = selectedKeywordIds.Contains(item.KeywordID)
                    //TitleCount = viewModel.Titles.Count(r => r.SubjectIndexes. == item.LanguageID)
                };
                viewModel.KeywordFilter.Add(editorViewModel);
            }

            TempData["simpleSearchingViewModel"] = viewModel;
            ViewBag.Title = "Narrow by " + @DbRes.T("Keywords.Keyword", "FieldDisplayName");
            return PartialView(viewModel);
        }

        [HttpGet]
        public ActionResult ShowAllAuthorsFilter()
        {
            var viewModel = (SimpleSearchingViewModel)TempData["simpleSearchingViewModel"];
            var authors = CacheProvider.GetAll<Author>("authors").ToList();
            var selectedAuthorIds = viewModel.GetSelectedAuthorIds().ToList();

            viewModel.AuthorFilter.Clear();

            foreach (var item in authors.OrderBy(x => x.DisplayName))
            {
                var results = viewModel.ResultsBeforeFilter.Any() ? viewModel.ResultsBeforeFilter : viewModel.Results;

                item.TitleCount = results.Count(r =>
                {
                    var firstOrDefault = r.TitleAuthors.FirstOrDefault();
                    return firstOrDefault != null && firstOrDefault.AuthorId == item.AuthorID;
                });

                var editorViewModel = new SelectAuthorEditorViewModel()
                {
                    Id = item.AuthorID,
                    Name = item.DisplayName,
                    Selected = selectedAuthorIds.Contains(item.AuthorID),
                    TitleCount = item.TitleCount
                };
                viewModel.AuthorFilter.Add(editorViewModel);
            }

            TempData["simpleSearchingViewModel"] = viewModel;
            ViewBag.Title = "Narrow by " + @DbRes.T("Authors.Author", "FieldDisplayName");
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

            return RedirectToAction("SimpleSearchResults", viewModel);
        }

        public ActionResult ClearAllFilters()
        {
            var viewModel = (SimpleSearchingViewModel)TempData["simpleSearchingViewModel"];
            viewModel.ClassmarksFilter.Clear();
            viewModel.MediaFilter.Clear();
            viewModel.PublisherFilter.Clear();
            viewModel.LanguageFilter.Clear();
            viewModel.KeywordFilter.Clear();
            viewModel.AuthorFilter.Clear();
            return RedirectToAction("SimpleSearchResults", viewModel);
        }



        public ActionResult BookDetails(int? id = 0)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Title title = _repository.GetById<Title>(id.Value);
            if (title == null)
            {
                return HttpNotFound();
            }

            var mediaType = title.MediaType.Media;
            if (string.IsNullOrEmpty(mediaType))
            {
                mediaType = "Book";
            }

            mediaType = mediaType.TrimEnd('s');

            ViewBag.Title = mediaType + " Details";
            return View(title);
        }
        
        [HttpPost]
        public JsonResult BarcodeExists(string barcode)
        {
            var volume = _db.Volumes.FirstOrDefault(v => v.Barcode == barcode);
            return Json(volume != null);
        }
        
        public ActionResult Null(int? id = 0)
        {
            return null;
        }

        public ActionResult ShowBarcodeLookupHelp()
        {
            var viewModel = new GenericHelpViewModel()
            {
                HelpText = HelpTextHelper.GetHelpText("barcodelookup"),
                //Glyphicon = "glyphicon-ok"
            };
            ViewBag.Title = @DbRes.T("CopyItems.Barcode", "FieldDisplayName") + " Lookup";
            return PartialView("_GenericHelp", viewModel);
        }

        public ActionResult ShowSearchHelp()
        {
            var viewModel = new GenericHelpViewModel()
            {
                HelpText = HelpTextHelper.GetHelpText("searchingtips")
            };
            ViewBag.Title = "Searching Tips";
            return PartialView("_GenericHelp", viewModel);
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
                UserId = User.Identity.GetUserId(),
                Description = description,
                SearchString = currentSearch.SearchString,
                SearchField = currentSearch.SearchField,
                Scope = "opac",
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
    }
}