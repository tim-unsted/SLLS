using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Web;
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
using System.Web.Script.Serialization;
using System.Web.UI.WebControls.Expressions;
using slls.Utils;

namespace slls.Controllers
{
    public class HomeController : sllsBaseController
    {
        private readonly DbEntities _db = new DbEntities();
        private readonly GenericRepository _repository;

        //Get the available gadgets from the database or cache ...
        private readonly IEnumerable<DashboardGadget> _allGadgets = CacheProvider.DashboardGadgets().Where(x => x.Area == "Home");

        public HomeController()
        {
            _repository = new GenericRepository(typeof(Title));
        }

        public ActionResult Index()
        {
            var viewModel = new OPACHomePageViewModel()
            {
                ShowWelcomeMessage = Settings.GetParameterValue("OPAC.ShowWelcomeMessage", "true", dataType: "bool") == "true",
                WelcomeHeader = Settings.GetParameterValue("OPAC.WelcomeHeader", "Library OPAC", dataType: "longtext"),
                WelcomeMessage = Settings.GetParameterValue("OPAC.WelcomeMessage", "Welcome to our new on-line Library", dataType: "longtext")
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

        public ActionResult Contact(bool success = false)
        {
            var userId = PublicFunctions.GetUserId();
            var emailFrom = "";
            if (userId != null)
            {
                emailFrom = _db.Users.Find(userId).Email;
            }

            var viewModel = new NewEmailViewModel
            {
                From = emailFrom,
                RedirectAction = "Contact",
                RedirectController = "Home",
                Title = "Contact Us"
            };

            var enquiryTypes = new Dictionary<string, string>
            {
                {"info@baileysolutions.co.uk", "General Enquiry"},
                {"support@baileysolutions.co.uk", "Support Request"}
            };

            ViewBag.EnquiryTypes = enquiryTypes;
            ViewBag.Title = "Contact Us";
            return View(viewModel);
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
            var viewModel = new SimpleSearchingViewModel();
            viewModel.LibraryStaff = Roles.IsLibraryStaff();
            viewModel.OrderBy = Settings.GetParameterValue("Searching.DefaultNewTitlesSortOrder", "commenced.desc", "Sets the default sort order for the 'New Titles' list.", dataType: "text");
            ViewData["OrderBy"] = SelectListHelper.NewTitlesOrderBy(viewModel.OrderBy);

            //Get a collection of items on the 'New Titles' list ...
            var newtitles = (from t in _db.Titles
                             join c in _db.Copies on t.TitleID equals c.TitleID
                             where t.Deleted == false && c.Deleted == false && c.AcquisitionsList && c.StatusType.Opac && c.Volumes.Any()
                             select t).Distinct();

            viewModel.Results = newtitles.ToList();
            ViewBag.Title = "New Titles";
            return View(viewModel);
        }

        public ActionResult RenderGadget(int col = 0, int row = 0, bool hasTitles = false)
        {
            //Get the current user's ID ...
            var userId = Utils.PublicFunctions.GetUserId(); //User.Identity.GetUserId();
            var gadgetAction =
                _allGadgets.Where(x => x.Row == row && x.Column == col).Select(g => g.Name).FirstOrDefault();

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
                case "askthelibrary":
                    {
                        var emailFrom = "";
                        var emailTo = Settings.GetParameterValue("EmailSettings.EmailToAddress", "", "Sets the email address for ''Ask the Librarian'', etc.", dataType: "text");

                        if (string.IsNullOrEmpty(emailTo))
                        {
                            return Null();
                        }

                        if (userId != null)
                        {
                            emailFrom = _db.Users.Find(userId).Email;
                        }

                        //if (string.IsNullOrEmpty(emailFrom))
                        //{
                        //    return null;
                        //}

                        var newEmailViewModel = new NewEmailViewModel
                        {
                            To = emailTo,
                            From = emailFrom,
                            RedirectAction = "Index",
                            RedirectController = "Home",
                            Title = "Ask a Question",
                            InternalMsg = true,
                            ShowCaptcha = string.IsNullOrEmpty(emailFrom)
                        };

                        var enquiryTypes = new Dictionary<string, string>
                    {
                        {"info@baileysolutions.co.uk", "General Enquiry"},
                        {"support@baileysolutions.co.uk", "Support Request"}
                    };

                        ViewBag.EnquiryTypes = enquiryTypes;
                        ViewBag.Title = "Ask a Question";
                        return PartialView("Dashboard/_AskTheLibrary", newEmailViewModel);
                    }

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

                        ViewData["SearchField"] = SelectListHelper.SearchFieldsList(id: "title");
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
                        var opacNotifications = allNotifications.Where(n => n.Scope == "O" && n.Visible && (n.ExpireDate == null || n.ExpireDate > DateTime.Today)).OrderBy(n => n.Position).Take(5).ToList();

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

                        var newTitlesSortOrder = Settings.GetParameterValue("Searching.DefaultNewTitlesSortOrder",
                            "commenced.desc", "Sets the default sort order for the 'New Titles' list.", dataType: "text");
                        if (newTitlesSortOrder == "title.asc")
                        {
                            var newTitles = allnewtitles.OrderBy(t => t.Title.Substring(t.NonFilingChars)).GroupBy(x => x.TitleId).Select(t => t.First()).ToList();
                            viewModel.NewTitles = newTitles.Take(10).ToList();
                            ViewBag.Count = newTitles.Count();
                        }
                        else
                        {
                            var newTitles = allnewtitles.OrderByDescending(t => t.Commenced).GroupBy(x => x.TitleId).Select(t => t.First()).ToList();
                            viewModel.NewTitles = newTitles.Take(10).ToList();
                            ViewBag.Count = newTitles.Count();
                        }

                        ViewBag.Title = DbRes.T("Titles.New_Titles", "FieldDisplayName");
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

        public ActionResult BrowseByAuthor(int id = 0)
        {
            var viewModel = new SimpleSearchingViewModel();

            if (id > 0)
            {
                var author = _db.Authors.Find(id);
                if (author != null)
                {
                    viewModel.SelectItem = author.DisplayName;
                }
            }
            
            //Get the actual results if the user has selected anything ...
            viewModel.Results =
                (from t in
                     _db.Titles
                 join c in _db.Copies on t.TitleID equals c.TitleID
                 join a in _db.TitleAuthors on t.TitleID equals a.TitleId
                 where
                     t.Copies.Any() && c.StatusType.Opac && c.Volumes.Any() && a.AuthorId == id && a.AuthorId != 0
                 select t).Distinct().ToList();
            
            viewModel.LibraryStaff = Roles.IsLibraryStaff();
            viewModel.IsActualSearch = false;
            viewModel.OrderBy = Settings.GetParameterValue("Searching.DefaultSortOrder", "title.asc", "Sets the default sort order for search results.", dataType: "text");

            //ViewData["ListAuthors"] = authorList;
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("browseBySeeAlso", "");
            ViewBag.Title = "Browse By " + DbRes.T("Authors.Author", "FieldDisplayName");
            ViewData["OrderBy"] = SelectListHelper.OpacResultsOrderBy(viewModel.OrderBy);
            return View(viewModel);
        }

        public ActionResult BrowseByClassmark(int listClassmarks = 0)
        {
            //Get the list of classmarks in use ...
            var classmarks = (from c in _db.Classmarks
                              where c.Titles.Count > 0
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
            foreach (var item in classmarks.OrderBy(c => c.Classmark))
            {
                classmarkList.Add(new SelectListItem
                {
                    Text = item.Classmark,
                    Value = item.ClassmarkID.ToString()
                });
            }

            //Get the actual results if the user has selected anything ...
            var viewModel = new SimpleSearchingViewModel
            {
                Results = (from t in
                               _db.Titles
                           join c in _db.Copies on t.TitleID equals c.TitleID
                           where
                               t.Copies.Any() && c.StatusType.Opac && c.Volumes.Any() &&
                               t.ClassmarkID == listClassmarks
                           select t).Distinct().ToList(),
                LibraryStaff = Roles.IsLibraryStaff(),
                IsActualSearch = false,
                OrderBy = Settings.GetParameterValue("Searching.DefaultSortOrder", "title.asc", "Sets the default sort order for search results.", dataType: "text")
            };

            ViewData["ListClassmarks"] = classmarkList;
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("browseBySeeAlso", ControllerContext.RouteData.Values["action"].ToString());
            ViewBag.Title = "Browse By " + DbRes.T("Classmarks.Classmark", "FieldDisplayName");
            ViewData["OrderBy"] = SelectListHelper.OpacResultsOrderBy(viewModel.OrderBy);
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

            //Get the actual results if the user has selected anything ...
            var viewModel = new SimpleSearchingViewModel
            {
                Results = (from t in
                               _db.Titles
                           join c in _db.Copies on t.TitleID equals c.TitleID
                           where
                               t.Copies.Any() && c.StatusType.Opac && c.Volumes.Any() &&
                               t.MediaID == listMedia
                           select t).Distinct().ToList(),
                LibraryStaff = Roles.IsLibraryStaff(),
                IsActualSearch = false,
                OrderBy = Settings.GetParameterValue("Searching.DefaultSortOrder", "title.asc", "Sets the default sort order for search results.", dataType: "text")
            };

            ViewData["ListMedia"] = mediaList;
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("browseBySeeAlso", ControllerContext.RouteData.Values["action"].ToString());
            ViewBag.Title = "Browse By " + DbRes.T("MediaTypes.Media_Type", "FieldDisplayName");
            ViewData["OrderBy"] = SelectListHelper.OpacResultsOrderBy(viewModel.OrderBy);
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

            //Get the actual results if the user has selected anything ...
            var viewModel = new SimpleSearchingViewModel
            {
                Results = (from t in
                               _db.Titles
                           join c in _db.Copies on t.TitleID equals c.TitleID
                           where
                               t.Copies.Any() && c.StatusType.Opac && c.Volumes.Any() &&
                               t.PublisherID == listPublishers
                           select t).Distinct().ToList(),
                LibraryStaff = Roles.IsLibraryStaff(),
                IsActualSearch = false,
                OrderBy = Settings.GetParameterValue("Searching.DefaultSortOrder", "title.asc", "Sets the default sort order for search results.", dataType: "text")
            };

            ViewData["ListPublishers"] = publisherList;
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("browseBySeeAlso", ControllerContext.RouteData.Values["action"].ToString());
            ViewBag.Title = "Browse By " + DbRes.T("Publishers.Publisher", "FieldDisplayName");
            ViewData["OrderBy"] = SelectListHelper.OpacResultsOrderBy(viewModel.OrderBy);
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

            //Get the actual results if the user has selected anything ...
            var viewModel = new SimpleSearchingViewModel
            {
                Results = (from t in
                               _db.Titles
                           join c in _db.Copies on t.TitleID equals c.TitleID
                           where
                               t.Copies.Any() && c.StatusType.Opac && c.Volumes.Any() &&
                               t.LanguageID == listLanguage
                           select t).Distinct().ToList(),
                LibraryStaff = Roles.IsLibraryStaff(),
                IsActualSearch = false,
                OrderBy = Settings.GetParameterValue("Searching.DefaultSortOrder", "title.asc", "Sets the default sort order for search results.", dataType: "text")
            };

            ViewData["ListLanguage"] = languageList;
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("browseBySeeAlso", ControllerContext.RouteData.Values["action"].ToString());
            ViewBag.Title = "Browse By " + DbRes.T("Languages.Language", "FieldDisplayName");
            ViewData["OrderBy"] = SelectListHelper.OpacResultsOrderBy(viewModel.OrderBy);
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

            ////Start a new list selectlist items ...
            //List<SelectListItem> locationList = new List<SelectListItem>
            //{
            //    new SelectListItem
            //    {
            //        Text = "Select a " + DbRes.T("Locations.Location", "FieldDisplayName"),
            //        Value = "0"
            //    }
            //};

            ////Add the actual locations ...
            //foreach (var item in location)
            //{
            //    locationList.Add(new SelectListItem
            //    {
            //        Text = item.location,
            //        Value = item.locationID.ToString()
            //    });
            //}

            //Get the actual results if the user has selected anything ...
            var viewModel = new SimpleSearchingViewModel
            {
                Results = (from t in
                               _db.Titles
                           join c in _db.Copies on t.TitleID equals c.TitleID
                           where
                               t.Copies.Any() && c.StatusType.Opac && c.Volumes.Any() &&
                               c.LocationID == listLocation
                           select t).Distinct().ToList(),
                LibraryStaff = Roles.IsLibraryStaff(),
                IsActualSearch = false,
                OrderBy = Settings.GetParameterValue("Searching.DefaultSortOrder", "title.asc", "Sets the default sort order for search results.", dataType: "text")
            };

            ViewData["Listlocation"] = SelectListHelper.OfficeLocationList(); //locationList;
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("browseBySeeAlso", ControllerContext.RouteData.Values["action"].ToString());
            ViewBag.Title = "Browse By " + DbRes.T("Locations.Location", "FieldDisplayName");
            ViewData["OrderBy"] = SelectListHelper.OpacResultsOrderBy(viewModel.OrderBy);
            return View(viewModel);
        }

        public ActionResult BrowseBySubject(int id = 0)
        {
            var results = new List<Title>();
            var viewModel = new SimpleSearchingViewModel
            {
                LibraryStaff = Roles.IsLibraryStaff(),
                IsActualSearch = false,
                OrderBy = Settings.GetParameterValue("Searching.DefaultSortOrder", "title.asc", "Sets the default sort order for search results.", dataType: "text"),
                Results = results
            };

            if (id > 0)
            {
                var keyword = _db.vwSelectKeywordsUsed.Find(id);
                viewModel.SelectItem = keyword.KeywordTerm;
                viewModel.Results = (from t in
                    _db.Titles
                    join c in _db.Copies on t.TitleID equals c.TitleID
                    join x in _db.SubjectIndexes on t.TitleID equals x.TitleID
                    where
                        t.Copies.Any() && c.StatusType.Opac && c.Volumes.Any() && x.KeywordID == id && x.KeywordID != 0
                    select t).Distinct().ToList();
            }

            ViewData["SeeAlso"] = MenuHelper.SeeAlso("browseBySeeAlso", ControllerContext.RouteData.Values["action"].ToString());
            ViewBag.Title = "Browse By " + DbRes.T("Keywords.Keyword", "FieldDisplayName");
            ViewData["OrderBy"] = SelectListHelper.OpacResultsOrderBy(viewModel.OrderBy);
            return View(viewModel);
        }

        public ActionResult _BarcodeEnquiry()
        {
            var viewModel = new OpacBarcodeEnquiryViewModel();
            ViewBag.Title = DbRes.T("CopyItems.Barcode", "FieldDisplayName") + " Search";
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
        public ActionResult SimpleSearch(string q = "")
        {
            var viewModel = new SimpleSearchingViewModel()
            {
                //LibraryStaff = Roles.IsUserInRole("Catalogue Admin"),
                Area = "opac",
                OrderBy = "title",
                IsActualSearch = true,
                SearchString = q
            };

            ViewData["SearchField"] = SelectListHelper.SearchFieldsList(scope: "opac");
            ViewBag.SearchTips = HelpTextHelper.GetHelpText("searchingtips");
            ViewBag.Title = Settings.GetParameterValue("Searching.SearchPageWelcome", "Search the Library", dataType: "text");
            return View(viewModel);
        }

        //[HttpPost]
        public ActionResult SimpleSearchResults(SimpleSearchingViewModel viewModel)
        {
            viewModel.IsActualSearch = true;
            viewModel.OrderBy = Settings.GetParameterValue("Searching.DefaultSortOrder", "title.asc", "Sets the default sort order for search results.", dataType: "text");
            ViewData["OrderBy"] = SelectListHelper.OpacResultsOrderBy(viewModel.OrderBy);
            var take = viewModel.NarrowByDefaultRecordCount;

            if (TempData["SearchTerm"] != null)
            {
                if (TempData["SearchTerm"].ToString() != viewModel.SearchString)
                {
                    TempData["ClassmarksFilter"] = null;
                    TempData["AuthorFilter"] = null;
                    TempData["KeywordFilter"] = null;
                    TempData["MediaFilter"] = null;
                    TempData["LanguageFilter"] = null;
                    TempData["PublisherFilter"] = null;
                    viewModel.ClassmarksFilter.Clear();
                    viewModel.AuthorFilter.Clear();
                    viewModel.KeywordFilter.Clear();
                    viewModel.MediaFilter.Clear();
                    viewModel.LanguageFilter.Clear();
                    viewModel.ClassmarksFilter.Clear();
                }
            }

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

            //Do some work on the passed search string ...

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
                var results = SearchService.DoFullTextSearch(q, qIgnore.Trim(), viewModel.SearchField == "all" ? "*" : viewModel.SearchField);
                viewModel.ResultsBeforeFilter = results;

                //Order the results ...
                switch (viewModel.OrderBy)
                {
                    case "title":
                        {
                            results = results.OrderBy(r => r.Title1.Substring(r.NonFilingChars)).Distinct().ToList();
                            break;
                        }
                    case "author":
                        {
                            results = results.OrderBy(r => r.AuthorString).Distinct().ToList();
                            break;
                        }
                    case "classmark":
                        {
                            results = results.OrderBy(r => r.Classmark).Distinct().ToList();
                            break;
                        }
                    case "pubyear":
                        {
                            results = results.OrderBy(r => r.Year).Distinct().ToList();
                            break;
                        }
                }

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
                viewModel.JsonData = Json(results);

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

            viewModel.LibraryStaff = Roles.IsLibraryStaff();
            ViewData["SearchField"] = SelectListHelper.SearchFieldsList(viewModel.SearchField);
            ViewBag.SearchTips = HelpTextHelper.GetHelpText("searchingtips");
            TempData["SearchTerm"] = viewModel.SearchString;
            ViewBag.Title = !string.IsNullOrEmpty(q) ? "Search Results" : "Search the Library";
            TempData["simpleSearchingViewModel"] = viewModel;

            return View("SimpleSearch", viewModel);
        }


        [HttpGet]
        public ActionResult ShowAllClassmarksFilter()
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
        public ActionResult ShowAllMediaFilter()
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
        public ActionResult ShowAllPublishersFilter()
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
        public ActionResult ShowAllLanguagesFilter()
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
        public ActionResult ShowAllKeywordsFilter()
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
        public ActionResult ShowAllAuthorsFilter()
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
                UserId = PublicFunctions.GetUserId(), //User.Identity.GetUserId(),
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

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult KeywordsUsed(string term)
        {
            term = " " + term;
            var keywords = new List<vwSelectKeywordUsed>();
            if (term.Length < 3)
            {
                keywords = (from k in _db.vwSelectKeywordsUsed
                            where k.KeywordTerm.StartsWith(term)
                            orderby k.KeywordTerm
                            select k).Take(100).ToList();
            }
            else
            {
                keywords = (from k in _db.vwSelectKeywordsUsed
                            where k.KeywordTerm.Contains(term)
                            orderby k.KeywordTerm
                            select k).Take(100).ToList();
            }

            IList<SelectListItem> list = new List<SelectListItem>();

            foreach (var x in keywords)
            {
                list.Add(new SelectListItem { Text = x.KeywordTerm, Value = x.KeywordId.ToString() });
            }

            var result = list.Select(item => new KeyValuePair<string, string>(item.Value.ToString(), item.Text)).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult AutoCompleteAuthors(string term)
        {
            //term = " " + term;
            var authors = SearchService.SelectAuthors(term);
            
            IList<SelectListItem> list = new List<SelectListItem>();

            foreach (var x in authors)
            {
                list.Add(new SelectListItem { Text = x.DisplayName, Value = x.AuthorID.ToString() });
            }

            var result = list.Select(item => new KeyValuePair<string, string>(item.Value.ToString(), item.Text)).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        
    }
}