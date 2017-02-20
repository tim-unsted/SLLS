using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using AutoCat.ViewModels;
using slls.App_Settings;
using slls.DAO;
using slls.Hubs;
using slls.Models;
using slls.Utils.Helpers;
using slls.ViewModels;
using Westwind.Globalization;

namespace slls.Areas.LibraryAdmin
{
    [RouteArea("LibraryAdmin", AreaPrefix = "Admin")]
    [RoutePrefix("Titles")]
    [Route("{action=index}")]
    public class TitlesController : CatalogueBaseController
    {
        private readonly DbEntities _db = new DbEntities();
        private readonly GenericRepository _repository;
        private readonly string _entityName = DbRes.T("Titles.Title", "FieldDisplayName");


        public TitlesController()
        {
            _repository = new GenericRepository(typeof(Title));
            ViewBag.Title = DbRes.T("Titles", "EntityType");
        }


        [Route("~/LibraryAdmin/Titles/GetTitles")]
        public JsonResult GetTitles()
        {
            var result = _db.Titles.ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [Route("~/LibraryAdmin/Titles/Find")]
        public ActionResult Find()
        {
            return View();
        }


        //GET: Select a title ...
        public ActionResult Select(string callingAction = "edit", string filter = "")
        {
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("titlesSeeAlso", ControllerContext.RouteData.Values["action"].ToString());

            var viewModel = new SelectTitleViewmodel();

            switch (callingAction)
            {
                case "edit":
                {
                    ViewBag.Title = "View/Edit " + _entityName;
                    viewModel.BtnText = "View/Edit " + _entityName;
                    viewModel.Message = "Select an " + _entityName.ToLower() + " to view/edit";
                    viewModel.HelpText = "Select the " + _entityName.ToLower() +
                                         " you wish to view/edit from the dropdown list of available " +
                                         DbRes.T("Titles", "EntityType").ToLower() + " below.";
                    viewModel.ReturnAction = "Edit";
                    viewModel.Titles = SelectListHelper.TitlesList();
                    break;
                }

                case "duplicate":
                {
                    ViewBag.Title = "Duplicate Existing " + _entityName;
                    viewModel.BtnText = "Duplicate Existing " + _entityName;
                    viewModel.Message = "Select an existing " + _entityName.ToLower() + " to duplicate";
                    viewModel.HelpText = "Select the existing " + _entityName.ToLower() +
                                         " you wish to duplicate from the dropdown list of available " +
                                         DbRes.T("Titles", "EntityType").ToLower() + " below.";
                    viewModel.ReturnAction = "DuplicateTitle";
                    viewModel.Titles = SelectListHelper.TitlesList();
                    return View("SelectDuplicate", viewModel);
                    break;
                }

                case "printdetails":
                {
                    ViewBag.Title = "Print " + _entityName;
                    viewModel.BtnText = "Print " + _entityName;
                    viewModel.Message = "Select an " + _entityName.ToLower() + " to print";
                    viewModel.HelpText = "Select the " + _entityName.ToLower() +
                                         " you wish to print from the dropdown list of available " +
                                         DbRes.T("Titles", "EntityType").ToLower() + " below.";
                    viewModel.ReturnAction = "PrintDetails";
                    viewModel.Titles = SelectListHelper.TitlesList();
                    break;
                }
            }

            return View(viewModel);
        }

        //POST: Select a title ...
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Select(SelectTitleViewmodel viewModel)
        {
            if (viewModel.TitleID == 0)
            {
                return null;
            }
            //TempData["GoToTab"] = viewModel.Tab;
            return RedirectToAction(viewModel.ReturnAction, new { id = viewModel.TitleID });
        }

        //POST: Select a title ...
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PostSelectDuplicate(SelectTitleViewmodel viewModel)
        {
            if (viewModel.TitleID == 0)
            {
                return null;
            }
            //TempData["GoToTab"] = viewModel.Tab;
            return RedirectToAction(viewModel.ReturnAction, new { id = viewModel.TitleID });
        }

        // GET: ALL Titles
        [Route("Index")]
        [Route("All")]
        [Route("ViewAll")]
        [Route("~/LibraryAdmin/Titles/Index")]
        public ActionResult Index(string selectedLetter = "A")
        {
            var viewModel = new TitlesListViewModel { SelectedLetter = selectedLetter };

            //Fill a list with the first letters of all suppliers names ...
            viewModel.FirstLetters = _db.Titles
                .GroupBy(t => t.Title1.Substring(0, 1))
                .Select(x => x.Key.ToUpper())
                .ToList();

            if (string.IsNullOrEmpty(selectedLetter) || selectedLetter == "All")
            {
                viewModel.Titles = _db.Titles.OrderBy(t => t.Title1.Substring(t.NonFilingChars)).ToList();
            }
            else
            {
                if (selectedLetter == "0-9")
                {
                    var numbers = Enumerable.Range(0, 10).Select(i => i.ToString());
                    viewModel.Titles = _db.Titles
                        .Where(t => numbers.Contains(t.Title1.Substring(0, 1)))
                        .ToList();
                }
                else if (selectedLetter == "non alpha")
                {
                    //Get a list 
                    var nonalpha1 = Enumerable.Range(32, 16).Select(i => ((char)i).ToString()).ToList();
                    var nonalpha2 = Enumerable.Range(91, 6).Select(i => ((char)i).ToString()).ToList();
                    var nonalpha3 = Enumerable.Range(123, 4).Select(i => ((char)i).ToString()).ToList();
                    IEnumerable<string> nonalpha = nonalpha1.Concat(nonalpha2).Concat(nonalpha3);

                    viewModel.Titles = _db.Titles
                        .Where(t => nonalpha.Contains(t.Title1.Substring(0, 1)))
                        .ToList();
                }
                else
                {
                    viewModel.Titles = _db.Titles
                        .Where(t => t.Title1.Substring(t.NonFilingChars).StartsWith(selectedLetter))
                        .OrderBy(t => t.Title1.Substring(t.NonFilingChars))
                        .ToList();
                }
            }


            ViewData["SeeAlso"] = MenuHelper.SeeAlso("titlesSeeAlso", "index");
            return View(viewModel);
        }


        // GET: BriefRecordList
        [Route("BriefRecordList")]
        [Route("~/LibraryAdmin/Titles/BriefRecordList")]
        public ActionResult BriefRecordList(string selectedLetter = "A")
        {
            //Fill a list with the first letters of all suppliers names ...
            var viewModel = new TitlesListViewModel
            {
                SelectedLetter = selectedLetter,
                FirstLetters = _db.Titles
                    .GroupBy(t => t.Title1.Substring(0, 1))
                    .Select(x => x.Key.ToUpper())
                    .ToList()
            };

            if (string.IsNullOrEmpty(selectedLetter) || selectedLetter == "All")
            {
                viewModel.Titles = _db.Titles.OrderBy(t => t.Title1.Substring(t.NonFilingChars)).ToList();
            }
            else
            {
                switch (selectedLetter)
                {
                    case "0-9":
                        var numbers = Enumerable.Range(0, 10).Select(i => i.ToString());
                        viewModel.Titles = _db.Titles
                            .Where(t => numbers.Contains(t.Title1.Substring(0, 1)))
                            .ToList();
                        break;
                    case "non alpha":
                        //Get a list 
                        var nonalpha1 = Enumerable.Range(32, 16).Select(i => ((char)i).ToString()).ToList();
                        var nonalpha2 = Enumerable.Range(91, 6).Select(i => ((char)i).ToString()).ToList();
                        var nonalpha3 = Enumerable.Range(123, 4).Select(i => ((char)i).ToString()).ToList();
                        var nonalpha = nonalpha1.Concat(nonalpha2).Concat(nonalpha3);
                        viewModel.Titles = _db.Titles
                            .Where(t => nonalpha.Contains(t.Title1.Substring(0, 1)))
                            .ToList();
                        break;
                    default:
                        viewModel.Titles = _db.Titles
                            .Where(t => t.Title1.Substring(t.NonFilingChars).StartsWith(selectedLetter))
                            .OrderBy(t => t.Title1.Substring(t.NonFilingChars))
                            .ToList();
                        break;
                }
            }

            ViewData["SeeAlso"] = MenuHelper.SeeAlso("titlesSeeAlso", "index");
            ViewBag.Title = "Brief Title List";
            return View(viewModel);
        }

        public ActionResult ByAuthor(int id = 0)
        {
            var viewModel = new TitlesListViewModel
            {
                Titles = (from t in
                              _db.Titles
                          join ta in _db.TitleAuthors on t.TitleID equals ta.TitleId
                          where ta.AuthorId == id
                          select t).ToList()
            };

            if (viewModel.Titles.Count() == 1)
            {
                var firstOrDefault = viewModel.Titles.FirstOrDefault();
                if (firstOrDefault != null)
                {
                    var titleId = firstOrDefault.TitleID;
                    return RedirectToAction("Edit", new { id = titleId });
                }
            }

            var author = from a in _db.Authors
                         where a.AuthorID == id
                         select a.DisplayName;

            ViewData["SeeAlso"] = MenuHelper.SeeAlso("titlesSeeAlso", ControllerContext.RouteData.Values["action"].ToString());
            ViewBag.Title = ViewBag.Title + " By " + DbRes.T("Authors.Author", "FieldDisplayName") + ": " + author.SingleOrDefault();
            return View(viewModel);
        }

        [Route("ByClassmark/{id}")]
        [Route("TitlesByClassmark/{id}")]
        [Route("CatalogueByClassmark/{id}")]
        ////[Route("~/Admin/Catalogue/ByClassmark/{id}")]
        [Route("~/LibraryAdmin/Titles/ByClassmark/{id}")]
        public ActionResult ByClassmark(int id)
        {
            var viewModel = new TitlesListViewModel
            {
                Titles = (from t in
                              _db.Titles
                          where t.Classmark.ClassmarkID == id
                          select t).ToList()
            };

            if (viewModel.Titles.Count() == 1)
            {
                var firstOrDefault = viewModel.Titles.FirstOrDefault();
                if (firstOrDefault != null)
                {
                    var titleId = firstOrDefault.TitleID;
                    return RedirectToAction("Edit", new { id = titleId });
                }
            }

            var classmarks = CacheProvider.GetAll<Classmark>("classmarks");
            var classmark = from c in classmarks
                            where c.ClassmarkID == id
                            select c.Classmark1;

            ViewData["SeeAlso"] = MenuHelper.SeeAlso("titlesSeeAlso", ControllerContext.RouteData.Values["action"].ToString());
            ViewBag.Title = ViewBag.Title + " By " + DbRes.T("Classmarks.Classmark", "FieldDisplayName") + ": " + classmark.FirstOrDefault();
            return View(viewModel);
        }

        public ActionResult ByLinkedFile(int id)
        {
            var viewModel = new TitlesListViewModel
            {
                Titles = (from t in _db.Titles
                          join l in _db.TitleLinks on t.TitleID equals l.TitleID
                          where l.FileId == id
                          select t).ToList()
            };

            if (viewModel.Titles.Count() == 1)
            {
                var firstOrDefault = viewModel.Titles.FirstOrDefault();
                if (firstOrDefault != null)
                {
                    var titleId = firstOrDefault.TitleID;
                    return RedirectToAction("Edit", new { id = titleId });
                }
            }

            var linkedFiles = from f in _db.HostedFiles
                        where f.FileId == id
                        select f.FileName;

            ViewData["SeeAlso"] = MenuHelper.SeeAlso("titlesSeeAlso", ControllerContext.RouteData.Values["action"].ToString());
            ViewBag.Title = ViewBag.Title + " By " + DbRes.T("Links.Linked_File", "FieldDisplayName") + ": " + linkedFiles.FirstOrDefault();
            return View(viewModel);
        }

        public ActionResult ByCoverImage(int id)
        {
            var viewModel = new TitlesListViewModel
            {
                Titles = (from t in _db.Titles
                          join i in _db.TitleImages on t.TitleID equals i.TitleId
                          where i.ImageId == id
                          select t).ToList()
            };

            if (viewModel.Titles.Count() == 1)
            {
                var firstOrDefault = viewModel.Titles.FirstOrDefault();
                if (firstOrDefault != null)
                {
                    var titleId = firstOrDefault.TitleID;
                    return RedirectToAction("Edit", new { id = titleId });
                }
            }

            var coverImages = from i in _db.Images
                              where i.ImageId == id
                              select i.Source;

            ViewData["SeeAlso"] = MenuHelper.SeeAlso("titlesSeeAlso", ControllerContext.RouteData.Values["action"].ToString());
            ViewBag.Title = ViewBag.Title + " By Cover Image: " + coverImages.FirstOrDefault();
            return View(viewModel);
        }

        public ActionResult ByMedia(int id)
        {
            var viewModel = new TitlesListViewModel
            {
                Titles = (from t in
                              _db.Titles
                          where t.MediaType.MediaID == id
                          select t).ToList()
            };

            if (viewModel.Titles.Count() == 1)
            {
                var firstOrDefault = viewModel.Titles.FirstOrDefault();
                if (firstOrDefault != null)
                {
                    var titleId = firstOrDefault.TitleID;
                    return RedirectToAction("Edit", new { id = titleId });
                }
            }

            var media = from m in CacheProvider.GetAll<MediaType>("mediatypes")
                        where m.MediaID == id
                        select m.Media;

            ViewData["SeeAlso"] = MenuHelper.SeeAlso("titlesSeeAlso", ControllerContext.RouteData.Values["action"].ToString());
            ViewBag.Title = ViewBag.Title + " By " + DbRes.T("MediaTypes.Media_Type", "FieldDisplayName") + ": " + media.FirstOrDefault();
            return View(viewModel);
        }

        public ActionResult ByPublisher(int id)
        {
            var viewModel = new TitlesListViewModel
            {
                Titles = (from t in
                              _db.Titles
                          where t.Publisher.PublisherID == id
                          select t).ToList()
            };

            if (viewModel.Titles.Count() == 1)
            {
                var firstOrDefault = viewModel.Titles.FirstOrDefault();
                if (firstOrDefault != null)
                {
                    var titleId = firstOrDefault.TitleID;
                    return RedirectToAction("Edit", new { id = titleId });
                }
            }

            var publisher = from p in CacheProvider.GetAll<Publisher>("publishers")
                            where p.PublisherID == id
                            select p.PublisherName;

            ViewData["SeeAlso"] = MenuHelper.SeeAlso("titlesSeeAlso", ControllerContext.RouteData.Values["action"].ToString());
            ViewBag.Title = ViewBag.Title + " By " + DbRes.T("Publishers.Publisher", "FieldDisplayName") + ": " + publisher.FirstOrDefault();
            return View(viewModel);
        }

        public ActionResult ByLanguage(int id)
        {
            var viewModel = new TitlesListViewModel
            {
                Titles = (from t in
                              _db.Titles
                          where t.LanguageID == id
                          select t).ToList()
            };

            if (viewModel.Titles.Count() == 1)
            {
                var firstOrDefault = viewModel.Titles.FirstOrDefault();
                if (firstOrDefault != null)
                {
                    var titleId = firstOrDefault.TitleID;
                    return RedirectToAction("Edit", new { id = titleId });
                }
            }

            var language = from l in CacheProvider.GetAll<Language>("languages")
                           where l.LanguageID == id
                           select l.Language1;

            ViewData["SeeAlso"] = MenuHelper.SeeAlso("titlesSeeAlso", ControllerContext.RouteData.Values["action"].ToString());
            ViewBag.Title = ViewBag.Title + " By " + DbRes.T("Languages.Language", "FieldDisplayName") + ": " + language.FirstOrDefault();
            return View(viewModel);
        }

        public ActionResult BySubject(int id)
        {
            var viewModel = new TitlesListViewModel
            {
                Titles = (from t in
                              _db.Titles
                          join ta in _db.SubjectIndexes on t.TitleID equals ta.TitleID
                          where ta.KeywordID == id
                          select t).ToList()
            };

            //If only one title, go direct to the Edit view ...
            if (viewModel.Titles.Count() == 1)
            {
                var firstOrDefault = viewModel.Titles.FirstOrDefault();
                if (firstOrDefault != null)
                {
                    var titleId = firstOrDefault.TitleID;
                    return RedirectToAction("Edit", new { id = titleId });
                }
            }

            // ... otherwise show a list of titles.
            var keyword = from k in _db.Keywords
                          where k.KeywordID == id
                          select k.KeywordTerm;

            ViewData["SeeAlso"] = MenuHelper.SeeAlso("titlesSeeAlso", ControllerContext.RouteData.Values["action"].ToString());
            ViewBag.Title = ViewBag.Title + " By " + DbRes.T("Keywords.Keyword", "FieldDisplayName") + ": " + keyword.FirstOrDefault();
            return View(viewModel);
        }

        [Route("BrowseByAuthor")]
        ////[Route("~/Admin/Catalogue/BrowseByAuthor")]
        [Route("~/LibraryAdmin/Titles/BrowseByAuthor")]
        public ActionResult BrowseByAuthor(int listAuthors = 0)
        {
            var viewModel = new TitlesListViewModel();

            //Get a list of all authors in use
            var authors = (from a in _db.Authors
                           join t in _db.TitleAuthors on a.AuthorID equals t.AuthorId
                           where t.AuthorId != 0
                           select new { a.AuthorID, DisplayName = a.DisplayName.TrimStart() + " (" + a.TitleAuthors.Count + ")" }).Distinct().OrderBy(x => x.DisplayName);

            //Start a new list selectlist items ...
            List<SelectListItem> authorList = authors.Select(item => new SelectListItem
            {
                Text = item.DisplayName,
                Value = item.AuthorID.ToString()
            }).ToList();

            //Add the authors ...

            ViewData["ListAuthors"] = authorList;
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("titlesSeeAlso", ControllerContext.RouteData.Values["action"].ToString());
            ViewBag.Title = ViewBag.Title + " By " +
                            DbRes.T("Authors.Author", "FieldDisplayName");

            //Get the actual results if the user has selected anything ...
            viewModel.Titles =
                (from t in
                     _db.Titles
                 join a in _db.TitleAuthors on t.TitleID equals a.TitleId
                 where a.AuthorId == listAuthors && a.AuthorId != 0
                 select t).ToList();
            return View(viewModel);
        }


        [Route("BrowseByClassmark")]
        ////[Route("~/Admin/Catalogue/BrowseByClassmark")]
        [Route("~/LibraryAdmin/Titles/BrowseByClassmark")]
        public ActionResult BrowseByClassmark(int listClassmarks = 0)
        {
            //Get the list of classmarks in use ...
            var classmarks = (from c in CacheProvider.GetAll<Classmark>("classmarks")
                              where c.Titles.Count > 0
                              orderby c.Classmark1
                              select
                                  new { c.ClassmarkID, Classmark = c.Classmark1 + " (" + c.Titles.Count + ")" })
                .Distinct();

            //Start a new list selectlist items ...
            var classmarkList = new List<SelectListItem>
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
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("titlesSeeAlso", ControllerContext.RouteData.Values["action"].ToString());
            ViewBag.Title = ViewBag.Title + " By " +
                            DbRes.T("Classmarks.Classmark", "FieldDisplayName");

            //Get the actual results if the user has selected anything ...
            var viewModel = new TitlesListViewModel
            {
                Titles = (from t in
                              _db.Titles
                          where t.Classmark.ClassmarkID == listClassmarks
                          select t).ToList()
            };
            return View(viewModel);
        }


        [Route("BrowseByMedia")]
        ////[Route("~/Admin/Catalogue/BrowseByMedia")]
        [Route("~/LibraryAdmin/Titles/BrowseByMedia")]
        public ActionResult BrowseByMedia(int listMedia = 0)
        {
            //Get the list of media types in use ...
            var media = (from m in CacheProvider.GetAll<MediaType>("mediatypes")
                         where m.Titles.Count > 0
                         select
                             new { m.MediaID, Media = m.Media + " (" + m.Titles.Count + ")" })
                .Distinct().OrderBy(x => x.Media);

            //Start a new list selectlist items ...
            var mediaList = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select a " + DbRes.T("MediaTypes.Media_Type", "FieldDisplayName"),
                    Value = "0"
                }
            };

            //Add a default item ...

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
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("titlesSeeAlso", ControllerContext.RouteData.Values["action"].ToString());
            ViewBag.Title = ViewBag.Title + " By " +
                            DbRes.T("MediaTypes.Media_Type", "FieldDisplayName");

            //Get the actual results if the user has selected anything ...
            var viewModel = new TitlesListViewModel();
            viewModel.Titles =
                (from t in
                     _db.Titles
                 where t.MediaType.MediaID == listMedia
                 select t).ToList();
            return View(viewModel);
        }


        [Route("BrowseByPublisher")]
        ////[Route("~/Admin/Catalogue/BrowseByPublisher")]
        [Route("~/LibraryAdmin/Titles/BrowseByPublisher")]
        public ActionResult BrowseByPublisher(int listPublishers = 0)
        {
            //Get the list of publishers in use ...
            var publishers = (from c in CacheProvider.GetAll<Publisher>("publishers")
                              where c.Titles.Count > 0
                              select
                                  new { c.PublisherID, PublisherName = c.PublisherName + " (" + c.Titles.Count + ")" })
                .Distinct().OrderBy(x => x.PublisherName);

            //Start a new list selectlist items ...
            var publisherList = new List<SelectListItem>
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

            //Add a default item ...

            //Add the actual publishers ...

            ViewData["ListPublishers"] = publisherList;
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("titlesSeeAlso", ControllerContext.RouteData.Values["action"].ToString());
            ViewBag.Title = ViewBag.Title + " By " +
                            DbRes.T("Publishers.Publisher", "FieldDisplayName");

            //Get the actual results if the user has selected anything ...
            var viewModel = new TitlesListViewModel();
            viewModel.Titles =
                (from t in
                     _db.Titles
                 where t.Publisher.PublisherID == listPublishers
                 select t).ToList();
            return View(viewModel);
        }


        [Route("BrowseByLanguage")]
        //[Route("~/Admin/Catalogue/BrowseByLanguage")]
        [Route("~/LibraryAdmin/Titles/BrowseByLanguage")]
        public ActionResult BrowseByLanguage(int listLanguage = 0)
        {
            //Get the list of media types in use ...
            var language = (from l in CacheProvider.GetAll<Language>("languages")
                            where l.Titles.Count > 0
                            select
                                new { l.LanguageID, Language = l.Language1 + " (" + l.Titles.Count + ")" })
                .Distinct().OrderBy(x => x.Language);

            //Start a new list selectlist items ...
            var languageList = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select a " + DbRes.T("Languages.Language", "FieldDisplayName"),
                    Value = "0"
                }
            };

            //Add a default item ...

            //Add the actual media types ...
            foreach (var item in language)
            {
                languageList.Add(new SelectListItem
                {
                    Text = item.Language,
                    Value = item.LanguageID.ToString()
                });
            }

            ViewData["ListLanguage"] = languageList;
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("titlesSeeAlso", ControllerContext.RouteData.Values["action"].ToString());
            ViewBag.Title = ViewBag.Title + " By " +
                            DbRes.T("Languages.Language", "FieldDisplayName");

            //Get the actual results if the user has selected anything ...
            var viewModel = new TitlesListViewModel();
            viewModel.Titles =
                (from t in
                     _db.Titles
                 where t.LanguageID == listLanguage
                 select t).ToList();
            return View(viewModel);
        }


        [Route("BrowseBySubject")]
        //[Route("~/Admin/Catalogue/BrowseBySubject")]
        [Route("~/LibraryAdmin/Titles/BrowseBySubject")]
        public ActionResult BrowseBySubject(int listSubjects = 0)
        {
            //Get a list of all currently used keywords. this cuts the down the list a bit!
            var keywords = (from k in _db.Keywords
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
            var kwdList = new List<SelectListItem>
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
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("titlesSeeAlso", ControllerContext.RouteData.Values["action"].ToString());
            ViewBag.Title = ViewBag.Title + " By " +
                            DbRes.T("Keywords.Keyword", "FieldDisplayName");

            //Get a list of all items linked to the selected keyword
            var viewModel = new TitlesListViewModel();
            viewModel.Titles =
                (from t in
                     _db.Titles
                 join x in _db.SubjectIndexes on t.TitleID equals x.TitleID
                 where x.KeywordID == listSubjects
                 select t).ToList();
            return View(viewModel);
        }


        // GET: Titles/Details/5
        [Route("Details")]
        //[Route("~/Admin/Catalogue/Details")]
        [Route("~/LibraryAdmin/Titles/Details")]
        public ActionResult Details(int? id)
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

            var authorlist = title.TitleAuthors.Select(t => t.Author.DisplayName).ToList();
            var editorlist = title.TitleEditors.Select(t => t.Author.DisplayName).ToList();
            var keywordslist = title.SubjectIndexes.Select(x => x.Keyword.KeywordTerm).ToList();

            ViewBag.Authors = authorlist;
            ViewBag.Editors = editorlist;
            ViewBag.Keywords = keywordslist;
            ViewBag.Title = _entityName + " Details";
            return View(title);
        }

        // GET: Titles/Add
        [Route("Add")]
        [Route("~/LibraryAdmin/Titles/Add")]
        public ActionResult Add()
        {
            return RedirectToAction("Create");
        }

        // GET: Titles/Create
        [Route("Create")]
        [Route("~/LibraryAdmin/Titles/Create")]
        public ActionResult Create(string isbn = "", bool notFound = false, int step = 1)
        {
            ViewData["ClassmarkID"] = SelectListHelper.ClassmarkList(Utils.PublicFunctions.GetDefaultValue("Titles", "ClassmarkID"));
            ViewData["publisherID"] = SelectListHelper.PublisherList(Utils.PublicFunctions.GetDefaultValue("Titles", "PublisherID"));
            ViewData["FrequencyID"] = SelectListHelper.FrequencyList(Utils.PublicFunctions.GetDefaultValue("Titles", "FrequencyID"));
            ViewData["LanguageID"] = SelectListHelper.LanguageList(Utils.PublicFunctions.GetDefaultValue("Titles", "LanguageID"));
            ViewData["MediaID"] = SelectListHelper.MediaTypeList(Utils.PublicFunctions.GetDefaultValue("Titles", "MediaID"));
            ViewData["AuthorID"] = SelectListHelper.AuthorsList(addDefault: true, addNew: false);
            //ViewData["EditorID"] = SelectListHelper.SelectEditorsList(addDefault: true, addNew: false);

            var viewModel = new TitleAddViewModel()
            {
                Year = DateTime.Now.ToString("yyyy"),
                Step = step
            };

            viewModel.Authors.Add(new Author());
            viewModel.Editors.Add(new Author());
            ViewData["Isbn"] = isbn;
            ViewData["NotFound"] = notFound;
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("titlesSeeAlso", "Add");
            ViewBag.Title = step != 0 ? "Step " + step + ": Add New " + _entityName : "Add New " + _entityName;
            ViewBag.BtnText = "Next >";
            ViewBag.BtnTip = "Save new " + _entityName + " and add a " + DbRes.T("Copies.Copy", "EntityType");
            return View(viewModel);
        }

        // POST: Titles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DoNewTitle(TitleAddViewModel viewModel)
        {
            if (viewModel.Authors.Any())
            {
                viewModel.Authors.RemoveAll(a => a.AuthorID == 0);
            }

            if (ModelState.IsValid)
            {
                var newTitle = new Title
                {
                    Title1 = viewModel.Title1,
                    Citation = viewModel.Citation,
                    Description = viewModel.Description,
                    ClassmarkID = viewModel.ClassmarkID <= 0 ? Utils.PublicFunctions.GetDefaultValue("Titles", "ClassmarkID") : viewModel.ClassmarkID,
                    MediaID = viewModel.MediaID <= 0 ? Utils.PublicFunctions.GetDefaultValue("Titles", "MediaID") : viewModel.MediaID,
                    PublisherID = viewModel.PublisherID <= 0 ? Utils.PublicFunctions.GetDefaultValue("Titles", "PublisherID") : viewModel.PublisherID,
                    FrequencyID = viewModel.FrequencyID <= 0 ? Utils.PublicFunctions.GetDefaultValue("Titles", "FrequencyID") : viewModel.FrequencyID,
                    LanguageID = viewModel.LanguageID <= 0 ? Utils.PublicFunctions.GetDefaultValue("Titles", "LanguageID") : viewModel.LanguageID,
                    Source = viewModel.Source,
                    Series = viewModel.Series,
                    ISBN10 = viewModel.ISBN10,
                    ISBN13 = viewModel.ISBN13,
                    NonFilingChars = viewModel.NonFilingChars == 0 ? GetNonFilingChars(viewModel.Title1) : viewModel.NonFilingChars,
                    Edition = viewModel.Edition,
                    PlaceofPublication = viewModel.PlaceofPublication,
                    Year = viewModel.Year,
                    Price = viewModel.Price,
                    Notes = viewModel.Notes,
                    DateCatalogued = DateTime.Now,
                    CataloguedBy = Utils.PublicFunctions.GetCurrentUserName()
                };

                _repository.Insert(newTitle);
                var titleId = newTitle.TitleID;

                //Do any Authors ...
                if (viewModel.Authors.Any())
                {
                    foreach (var author in viewModel.Authors.Where(a => a.AuthorID > 0))
                    {
                        var titleAuthor = new TitleAuthor()
                        {
                            TitleId = titleId,
                            AuthorId = author.AuthorID
                        };
                        _db.TitleAuthors.Add(titleAuthor);
                        _db.SaveChanges();
                    }
                }

                var step = viewModel.Step != 0 ? viewModel.Step + 1 : 0;
                return RedirectToAction("Add", "Copies", new { id = titleId, step = step, returnAction = "Edit", returnController = "Titles" });
            }
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("titlesSeeAlso", "Add");
            ViewData["ClassmarkID"] = SelectListHelper.ClassmarkList(viewModel.ClassmarkID);
            ViewData["publisherID"] = SelectListHelper.PublisherList(viewModel.PublisherID);
            ViewData["FrequencyID"] = SelectListHelper.FrequencyList(viewModel.FrequencyID);
            ViewData["LanguageID"] = SelectListHelper.LanguageList(viewModel.LanguageID);
            ViewData["MediaID"] = SelectListHelper.MediaTypeList(viewModel.MediaID);
            ViewData["AuthorID"] = SelectListHelper.AuthorsList(addDefault: true, addNew: false);
            ViewBag.Title = viewModel.Step != 0 ? "Step " + viewModel.Step + ": Add New " + _entityName : "Add New " + _entityName;
            ViewBag.BtnText = "Next >";
            ViewBag.BtnTip = "Save new " + _entityName + " and add a " + DbRes.T("Copies.Copy", "EntityType");
            return View("Create", viewModel);
        }

        //Create a default copy and volume ...
        public void AddUnmannedCopy(int titleId)
        {
            if (titleId == 0)
            {
                return;
            }
            var title = _db.Titles.Find(titleId);
            if (title == null)
            {
                return;
            }
            var copyNumber = title.Copies == null ? 1 : title.Copies.Count + 1;
            var newCopy = new Copy
            {
                TitleID = titleId,
                CopyNumber = copyNumber,
                AcquisitionsNo = titleId.ToString() + "." + copyNumber.ToString(),
                LocationID = Utils.PublicFunctions.GetDefaultValue("Copies", "LocationID"),
                StatusID = Utils.PublicFunctions.GetDefaultValue("Copies", "StatusID"),
                CirculationMsgID = Utils.PublicFunctions.GetDefaultValue("Copies", "CirculationMsgID"),
                PrintLabel = true,
                Commenced = DateTime.Now,
                InputDate = DateTime.Now
            };

            _db.Copies.Add(newCopy);
            _db.SaveChanges();
            var copyId = newCopy.CopyID;

            //Do default volume/label ...
            var newVolume = new Volume
            {
                CopyID = copyId,
                Barcode = Utils.PublicFunctions.NewBarcode(),
                RefOnly = false,
                PrintLabel = true,
                LoanTypeID = Utils.PublicFunctions.GetDefaultLoanType(title.MediaID),
                InputDate = DateTime.Now
            };

            _db.Volumes.Add(newVolume);
            _db.SaveChanges();
        }

        public ActionResult _autoCat(string isbn = "", bool notFound = false)
        {
            //Get a list of all data soures (i.e. Amazon;Hammick;Wildys; etc.)
            var allSources = ConfigurationManager.AppSettings["AutoCatDataSources"];
            var sources = allSources.Split(',').ToList();

            var viewModel = new AddTitleWithAutoCatViewModel
            {
                Sources = sources,
                Isbn = isbn,
                EntityName = DbRes.T("Titles.Title", "FieldDisplayName")
            };

            if (notFound)
            {
                ModelState.AddModelError("NotFound", "This ISBN could not be found at the selected source. Please check or try an alternative source.");
            }

            return PartialView(viewModel);
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult AddAuthor()
        {
            var title = new TitleAddViewModel();
            ViewData["AuthorID"] = SelectListHelper.AuthorsList(addDefault: true, addNew: false);
            title.Authors.Add(new Author());
            return View(title);
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult AddEditor()
        {
            var title = new TitleAddViewModel();
            ViewData["EditorID"] = SelectListHelper.EditorsList(addDefault: true, addNew: false);
            title.Editors.Add(new Author());
            return View(title);
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
        public ActionResult _autoCat(AddTitleWithAutoCatViewModel viewModel, string source)
        {
            var isbn = viewModel.Isbn;
            if (isbn == null)
            {
                return null;
            }

            isbn = CleanIsbn(viewModel.Isbn);
            var autoCat = new AutoCat.AutoCat();

            //Start a list of any ISBNs that cannot be found, or that return an error
            var errorList = new List<string>();
            var titleId = 0;

            if (isbn.Length == 10 || isbn.Length == 13)
            {
                //Get the data from the autoCat plug-in, passing the ISBN and the preferred source
                var newTitle = autoCat.GetIsbnData(source, isbn);

                if (newTitle != null && newTitle.ErrorMessage == null)
                {
                    titleId = AddNewAutoCatTitle(newTitle);
                }
                else
                {
                    //Collect any ISBNs that cannot be found or return an error ...
                    errorList.Add(isbn);
                }
            }
            else
            {
                errorList.Add(isbn);
            }

            //Take the user to the appropriate page ...
            return errorList.Count > 0 ? RedirectToAction("Create", new { isbn, notFound = true }) : RedirectToAction("Edit", new { id = titleId });
        }


        //Code to create a new Title from a passed AutoCat record. This is called from 3 places  ....
        public int AddNewAutoCatTitle(AutoCatNewTitle newTitle)
        {
            var doKeywords = Settings.GetParameterValue("AutoCat.IncludeKeywords", "false", "Add new keywords when using AutoCat", dataType: "bool") == "true";
            var doReviews = Settings.GetParameterValue("AutoCat.IncludeReviews", "true", "Add reviews when using AutoCat", dataType: "bool") == "true";
            var doContents = Settings.GetParameterValue("AutoCat.IncludeContents", "true", "Add contents when using AutoCat", dataType: "bool") == "true";

            var titleId = 0;

            var languageId = string.IsNullOrEmpty(newTitle.Language) ? Utils.PublicFunctions.GetDefaultValue("Titles", "LanguageID") : LanguagesController.GetLanguageId(newTitle.Language);
            var publisherId = string.IsNullOrEmpty(newTitle.Publisher) ? Utils.PublicFunctions.GetDefaultValue("Titles", "PublisherID") : PublishersController.GetPublisherId(newTitle.Publisher);
            var frequencyId = string.IsNullOrEmpty(newTitle.Frequency) ? Utils.PublicFunctions.GetDefaultValue("Titles", "FrequencyID") : FrequenciesController.GetFrequencyId(newTitle.Frequency);
            var mediaId = string.IsNullOrEmpty(newTitle.Media) ? Utils.PublicFunctions.GetDefaultValue("Titles", "MediaID") : MediaTypesController.GetMediaId(newTitle.Media);
            var classmarkId = string.IsNullOrEmpty(newTitle.Classmark) ? Utils.PublicFunctions.GetDefaultValue("Titles", "ClassmarkID") : ClassmarksController.GetClassmarkId(newTitle.Classmark);

            //Check for a long 'Description'
            var longDescription = newTitle.Description;
            if (longDescription != null)
            {
                if (longDescription.Length > 255)
                {
                    newTitle.Description = "";
                }
                else
                {
                    longDescription = null;
                }
            }

            var title = new Title
            {
                Title1 = newTitle.Title,
                Description = newTitle.Description,
                Edition = newTitle.Edition,
                ISBN10 = newTitle.ISBN10,
                ISBN13 = newTitle.ISBN13,
                Series = newTitle.Series,
                Citation = newTitle.Citation,
                Source = newTitle.Source,
                PlaceofPublication = newTitle.PlaceofPublication,
                Year = newTitle.Year,
                MediaID = mediaId,
                ClassmarkID = classmarkId,
                LanguageID = languageId,
                PublisherID = publisherId,
                FrequencyID = frequencyId,
                NonFilingChars = GetNonFilingChars(newTitle.Title),
                DateCatalogued = DateTime.Now,
                CataloguedBy = Utils.PublicFunctions.GetCurrentUserName()
            };

            //Save the main title details as a new Title ...
            _db.Titles.Add(title);
            _db.SaveChanges();

            //Establish the new Title ID ...
            titleId = title.TitleID;

            //Do authors ...
            if (newTitle.Author != null)
            {
                if (newTitle.Author.Any())
                {
                    foreach (var author in newTitle.Author.ToList())
                    {
                        //get the author id ...
                        var authorId = AuthorsController.GetAuthorId(author);

                        if (authorId != 0)
                        {
                            //insert into TitleAuthors ...
                            var ta = new TitleAuthor
                            {
                                TitleId = titleId,
                                AuthorId = authorId,
                                InputDate = DateTime.Now
                            };
                            _db.TitleAuthors.Add(ta);
                            _db.SaveChanges();
                        }
                    }
                }
            }

            //Do keywords ...
            if (doKeywords)
            {
                if (newTitle.Keywords != null)
                {
                    if (newTitle.Keywords.Any())
                    {
                        foreach (var keyword in newTitle.Keywords.ToList())
                        {
                            var keywordId = KeywordsController.GetKeywordId(keyword);

                            if (keywordId != 0)
                            {
                                var subjectIndex = new SubjectIndex
                                {
                                    KeywordID = keywordId,
                                    TitleID = titleId,
                                    InputDate = DateTime.Now
                                };
                                _db.SubjectIndexes.Add(subjectIndex);
                                _db.SaveChanges();
                            }
                        }
                    }
                }
            }

            //Do title texts (e.g. Long Description, content, reviews, etc.)
            if (longDescription != null)
            {
                var labelId = TitleAdditionalFieldDefsController.GetLongTextLabelId("Description");
                var data = new TitleAdditionalFieldData
                {
                    TitleID = titleId,
                    FieldID = labelId,
                    FieldData = longDescription,
                    InputDate = DateTime.Now
                };
                _db.TitleAdditionalFieldDatas.Add(data);
                _db.SaveChanges();
            }

            if (doContents)
            {
                if (!string.IsNullOrEmpty(newTitle.Contents))
                {
                    var labelId = TitleAdditionalFieldDefsController.GetLongTextLabelId("Contents");
                    var data = new TitleAdditionalFieldData
                    {
                        TitleID = titleId,
                        FieldID = labelId,
                        FieldData = newTitle.Contents,
                        InputDate = DateTime.Now
                    };
                    _db.TitleAdditionalFieldDatas.Add(data);
                    _db.SaveChanges();
                }
            }

            if (doReviews)
            {
                if (!string.IsNullOrEmpty(newTitle.Reviews))
                {
                    var labelId = TitleAdditionalFieldDefsController.GetLongTextLabelId("Reviews");
                    var data = new TitleAdditionalFieldData
                    {
                        TitleID = titleId,
                        FieldID = labelId,
                        FieldData = newTitle.Reviews,
                        InputDate = DateTime.Now
                    };
                    _db.TitleAdditionalFieldDatas.Add(data);
                    _db.SaveChanges();
                }
            }

            //Do links ...
            if (newTitle.Links != null)
            {
                if (newTitle.Links.Any())
                {
                    foreach (var link in newTitle.Links)
                    {
                        var newTitleLink = new TitleLink()
                        {
                            TitleID = titleId,
                            URL = link.Value,
                            DisplayText = link.Key,
                            HoverTip = link.Key,
                            InputDate = DateTime.Now,
                            IsValid = true
                        };
                        _db.TitleLinks.Add(newTitleLink);
                        _db.SaveChanges();
                    }
                }
            }

            //Do image ...
            if (!string.IsNullOrEmpty(newTitle.ImageUrl))
            {
                using (var client = new WebClient())
                {
                    var image = client.DownloadData(newTitle.ImageUrl);
                    if (image != null)
                    {
                        var img = new CoverImage
                        {
                            Image = image,
                            Source = newTitle.ImageUrl,
                            Size = image.Length,
                            Type = image.GetType().ToString(),
                            InputDate = DateTime.Now
                        };
                        _db.Images.Add(img);
                        _db.SaveChanges();

                        var imageId = img.ImageId;

                        var titleImage = new TitleImage
                        {
                            ImageId = imageId,
                            TitleId = titleId,
                            Alt = newTitle.Title,
                            HoverText = newTitle.Title,
                            IsPrimary = true,
                            InputDate = DateTime.Now
                        };
                        _db.TitleImages.Add(titleImage);
                        _db.SaveChanges();
                    }
                }
            }

            //Do default copy and volume ...
            AddUnmannedCopy(titleId);

            return titleId;
        }


        public ActionResult _copacSearch(bool notSelected = false)
        {
            var viewModel = (CopacSearchCriteria)TempData["SearchCriteria"];
            var searchResults = (CopacSearchResults)TempData["SearchResults"];
            ViewData["NotSelected"] = notSelected;
            ViewData["Language"] = SelectListHelper.CopacLanguageList(viewModel == null ? null : viewModel.Language);
            ViewData["Library"] = SelectListHelper.CopacLibraryList(viewModel == null ? null : viewModel.Library);

            if (searchResults == null) return PartialView(viewModel);
            var countResults = searchResults.ResultsCount;

            if (notSelected == false)
            {
                if (searchResults.HasErrors)
                {
                    ModelState.AddModelError("SearchError", "COPAC encountered an error whist running your search. The most likely cause is because your search is too broad and timed-out. Try to be more specific or use more fields to improve your search.");
                }
                else
                {
                    if (countResults == 0)
                    {
                        ModelState.AddModelError("SearchError", "COPAC doesn't have any information about your search criteria. To improve your search results, try any of the following:"
                                                                + Environment.NewLine + "Check the spellings."
                                                                + Environment.NewLine + "Check that you are using the right criteria."
                                                                + Environment.NewLine + "Replace author full name with initials."
                                                                + Environment.NewLine + "For multiple authors, enter one author name.");
                    }
                    if (countResults > 100)
                    {
                        ModelState.AddModelError("SearchError",
                            "Your search will return too may results to handle efficiently. To limit search results, try to be more specific or use more search fields.");
                    }
                }
            }

            return PartialView(viewModel);
        }


        [HttpPost]
        public ActionResult _copacSearch(CopacSearchCriteria viewModel)
        {
            var autoCat = new AutoCat.AutoCat();
            TempData["SearchCriteria"] = viewModel;

            var searchString = "";

            if (!string.IsNullOrEmpty(viewModel.Title))
            {
                searchString = searchString + "&ti=" + viewModel.Title.Replace((char)32, (char)43);
            }

            if (!string.IsNullOrEmpty(viewModel.Publisher))
            {
                searchString = searchString + "&pub=" + viewModel.Publisher.Replace((char)32, (char)43);
            }

            if (!string.IsNullOrEmpty(viewModel.Author))
            {
                searchString = searchString + "&au=" + viewModel.Author.Replace((char)32, (char)43);
            }

            if (!string.IsNullOrEmpty(viewModel.PubYear))
            {
                searchString = searchString + "&date=" + viewModel.PubYear.Replace((char)32, (char)43);
            }

            if (!string.IsNullOrEmpty(viewModel.CopacIsbn))
            {
                searchString = searchString + "&isn=" + viewModel.CopacIsbn.Replace((char)32, (char)43);
            }

            if (!string.IsNullOrEmpty(viewModel.Language))
            {
                searchString = searchString + "&lang=" + viewModel.Language.Replace((char)32, (char)43);
            }

            if (!string.IsNullOrEmpty(viewModel.Library))
            {
                searchString = searchString + "&lib=" + viewModel.Library.Replace((char)32, (char)43);
            }

            //Get the data from the autoCat plug-in, passing the ISBN and the preferred source
            var copacSearchResults = autoCat.GetCopacSearchResults(searchString);

            if (copacSearchResults == null)
            {
                return RedirectToAction("AddWithAutoCat", new { notFound = true });
            }
            TempData["SearchResults"] = copacSearchResults;
            return RedirectToAction("AddWithAutoCat");
        }

        public ActionResult _CopacResults(bool notSelected = false)
        {
            var viewModel = (CopacSearchResults)TempData["SearchResults"];

            //Try to keep the temp data alive ...
            TempData["SearchCriteria"] = TempData["SearchCriteria"];

            if (notSelected)
            {
                ModelState.AddModelError("NotSelected", "Nothing selected! Please tick at least one title to add to the catalogue");
            }

            return PartialView(viewModel);
        }

        [HttpPost]
        public ActionResult _CopacResults(CopacSearchResults viewModel)
        {
            var titleId = 0;
            var count = 0;
            TempData["SearchResults"] = viewModel;

            foreach (var copacRecord in viewModel.CopacRecords.Where(r => r.AddTitle))
            {
                var newTitle = new AutoCatNewTitle
                {
                    Title = copacRecord.Title,
                    Year = copacRecord.PubYear,
                    ISBN13 = copacRecord.Isbn13,
                    ISBN10 = copacRecord.Isbn10,
                    Author = copacRecord.Author != null ? copacRecord.Author.Split(';').ToList() : null,
                    Edition = copacRecord.Edition,
                    PlaceofPublication = copacRecord.Place,
                    Publisher = copacRecord.Publisher
                };

                //Try adding this new title ...
                titleId = AddNewAutoCatTitle(newTitle);
                count++;
            }

            if (count == 0)
            {
                return RedirectToAction("AddWithAutoCat", new { notSelected = true });
            }

            if (count > 1)
            {
                TempData["SearchResults"] = null;
                return RedirectToAction("RecentlyAdded", new { days = 1 });
            }

            TempData["SearchResults"] = null;
            return RedirectToAction("Edit", new { id = titleId });
        }


        // GET: Titles/Edit/5
        [Route("Edit/{id}")]
        [Route("~/LibraryAdmin/Titles/Edit/{id}")]
        public ActionResult Edit(int id = 0)
        {
            if (id == 0)
            {
                return RedirectToAction("Select");
            }
            var title = _repository.GetById<Title>(id);
            if (title == null)
            {
                return RedirectToAction("Select");
            }

            var viewModel = new TitleEditViewModel
            {
                TitleID = title.TitleID,
                Copies = title.Copies,
                HasCopies = title.Copies.Any(),
                OrderDetails = title.OrderDetails,
                TitleImages = title.TitleImages,
                TitleLinks = title.TitleLinks,
                TitleAdditionalFieldDatas = title.TitleAdditionalFieldDatas, 
                SubjectIndexes = title.SubjectIndexes
            };

            ViewData["TitleId"] = SelectListHelper.TitlesList(id);
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("titlesSeeAlso", ControllerContext.RouteData.Values["action"].ToString());
            ViewBag.SubjectCount = viewModel.SubjectIndexes.Count();
            ViewBag.CopiesCount = viewModel.Copies.Count();
            ViewBag.ImagesCount = viewModel.TitleImages.Count();
            ViewBag.LinksCount = viewModel.TitleLinks.Count();
            ViewBag.LongTextsCount = viewModel.TitleAdditionalFieldDatas.Count;
            ViewBag.OrdersCount = viewModel.OrderDetails.Count();
            ViewBag.Message = _entityName + " to edit:";
            ViewBag.Title = _entityName + " Details (View/Edit)";
            return View(viewModel);
        }

        
        // POST: Titles/Update/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(TitleDetailsViewModel editedtitle)
        {
            var titleId = editedtitle.TitleID;
            var title = _repository.GetById<Title>(titleId);

            if (ModelState.IsValid)
            {
                title.Title1 = editedtitle.Title1;
                title.MediaID = editedtitle.MediaID;
                title.ClassmarkID = editedtitle.ClassmarkID;
                title.Citation = editedtitle.Citation;
                title.Description = editedtitle.Description;
                title.Edition = editedtitle.Edition;
                title.FrequencyID = editedtitle.FrequencyID;
                title.ISBN13 = editedtitle.ISBN13;
                title.ISBN10 = editedtitle.ISBN10;
                title.LanguageID = editedtitle.LanguageID;
                title.LastModified = DateTime.Now;
                title.NonFilingChars = editedtitle.NonFilingChars;
                title.Notes = editedtitle.Notes;
                title.Series = editedtitle.Series;
                title.Source = editedtitle.Source;
                title.PlaceofPublication = editedtitle.PlaceofPublication;
                title.Year = editedtitle.Year;
                title.PublisherID = editedtitle.PublisherID;
                title.ModifiedBy = Utils.PublicFunctions.GetCurrentUserName();
                _repository.Update(title);

                return RedirectToAction("Edit", new { id = titleId });
            }
            return RedirectToAction("Edit", new { id = titleId });
        }

        // GET: Titles/_Details/5
        public ActionResult _Details(int id = 0)
        {
            {
                var title = _repository.GetById<Title>(id);
                if (title == null)
                {
                    return RedirectToAction("Select");
                }

                ViewData["ClassmarkID"] = SelectListHelper.ClassmarkList(title.ClassmarkID, null, false);
                ViewData["FrequencyID"] = SelectListHelper.FrequencyList(title.FrequencyID, null, false);
                ViewData["publisherID"] = SelectListHelper.PublisherList(title.PublisherID, null, false);
                ViewData["LanguageID"] = SelectListHelper.LanguageList(title.LanguageID, null, false);
                ViewData["MediaID"] = SelectListHelper.MediaTypeList(title.MediaID, null, false);

                var viewModel = new TitleDetailsViewModel()
                {
                    TitleID = title.TitleID,
                    Title1 = title.Title1,
                    ClassmarkID = title.ClassmarkID,
                    PublisherID = title.PublisherID,
                    Description = title.Description,
                    Edition = title.Edition,
                    FrequencyID = title.FrequencyID,
                    ISBN10 = title.ISBN10,
                    ISBN13 = title.ISBN13,
                    MediaID = title.MediaID,
                    Series = title.Series,
                    Source = title.Source,
                    NonFilingChars = title.NonFilingChars,
                    LanguageID = title.LanguageID,
                    PlaceofPublication = title.PlaceofPublication,
                    Notes = title.Notes,
                    Year = title.Year,
                    TitleAuthors = title.TitleAuthors, 
                    TitleEditors = title.TitleEditors
                };
                return PartialView(viewModel);
            }
        }

        // POST: Titles/_Details/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _Details(int id, TitleDetailsViewModel editedTitle)
        {
            if (ModelState.IsValid)
            {
                var item = _repository.GetById<Title>(id);

                item.Title1 = editedTitle.Title1;
                item.ISBN10 = editedTitle.ISBN10;
                item.ISBN13 = editedTitle.ISBN13;
                item.LanguageID = editedTitle.LanguageID;
                item.MediaID = editedTitle.MediaID;
                item.PlaceofPublication = editedTitle.PlaceofPublication;
                item.Series = editedTitle.Series;
                item.Year = editedTitle.Year;
                item.FrequencyID = editedTitle.FrequencyID;
                item.PublisherID = editedTitle.PublisherID;
                item.Description = editedTitle.Description;
                item.Edition = editedTitle.Edition;
                item.Source = editedTitle.Source;
                item.Citation = editedTitle.Citation;
                item.NonFilingChars = editedTitle.NonFilingChars;
                item.Notes = editedTitle.Notes;
                item.LastModified = DateTime.Now;

                _repository.Update(item);
                return RedirectToAction("Edit", new { id });
            }
            //In case things go wrong ...
            ViewData["ClassmarkID"] = new SelectList(CacheProvider.GetAll<Classmark>("classmarks").Where(c => c.Deleted == false), "ClassmarkID", "Classmark1", editedTitle.ClassmarkID);
            ViewData["publisherID"] = new SelectList(CacheProvider.GetAll<Publisher>("publishers").Where(p => p.Deleted == false), "publisherID", "publisherName", editedTitle.PublisherID);
            ViewData["FrequencyID"] = new SelectList(CacheProvider.GetAll<Frequency>("frequencies").Where(f => f.Deleted == false), "FrequencyID", "Frequency1", editedTitle.FrequencyID);
            ViewData["LanguageID"] = new SelectList(CacheProvider.GetAll<Language>("languages").Where(l => l.Deleted = false), "LanguageID", "Language1", editedTitle.LanguageID);
            ViewData["MediaID"] = new SelectList(CacheProvider.GetAll<MediaType>("mediatypes").Where(m => m.Deleted == false), "MediaID", "Media", editedTitle.MediaID);
            ViewBag.Title = "View/Edit " + _entityName;
            return PartialView(editedTitle);
        }


        // GET: Titles/Delete/5
        [HttpGet]
        [Route("Delete/{id}")]
        [Route("~/LibraryAdmin/Titles/Delete/{id}")]
        public ActionResult Delete(int? id, string view = "")
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var title = _repository.GetById<Title>(id.Value);
            if (title == null)
            {
                return HttpNotFound();
            }

            var viewModel = new TitleDeleteViewModel
            {
                TitleId = title.TitleID,
                Title1 = title.Title1,
                Edition = title.Edition,
                Isbn = title.ISBN13 ?? title.ISBN10,
                Year = title.Year,
                Description = title.Description,
                Notes = title.Notes,
                CurrentViewName = view
            };

            ViewBag.Title = "Delete " + _entityName;
            return PartialView(viewModel);
        }

        // POST: Titles/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult ConfirmDelete(int? id)
        public ActionResult ConfirmDelete(TitleDeleteViewModel viewModel)
        {
            //if (id == null)
            if (viewModel.TitleId == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var title = _repository.GetById<Title>(viewModel.TitleId);
            _repository.Delete(title);

            //var returnView = viewModel.CurrentViewName;
            //if (string.IsNullOrEmpty(returnView))
            //{
            //    returnView = "Index";
            //}

            ////return RedirectToAction("Index");
            //return RedirectToAction(returnView);
            return Json(new
            {
                success = true
            });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult Autocomplete(string term)
        {
            var titles = (from t in _db.Titles
                          where t.Title1.Contains(term)
                          orderby t.Title1
                          select new { t.Title1, t.TitleID }).Take(10);

            IList<SelectListItem> list = new List<SelectListItem>();

            //list.Add(new SelectListItem { Text = "[Show All]", Value = "-1" });

            foreach (var t in titles)
            {
                list.Add(new SelectListItem { Text = t.Title1, Value = t.TitleID.ToString() });
            }

            var result = list.Select(item => new KeyValuePair<string, string>(item.Value.ToString(), item.Text)).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        //Method used to supply a JSON list of copies when selecting a title (Ajax stuf)
        public JsonResult GetTitleCopies(int titleId = 0)
        {
            var title = _db.Titles.Find(titleId);
            if (title == null)
            {
                return Json(new { success = true });
            }

            var copies = new SelectList(_db.Copies.Where(x => x.TitleID == titleId).ToList(), "CopyID", "CopyNumber");

            return Json(new
            {
                success = true,
                TitleCopyData = copies,
                DefaultloanType = Utils.PublicFunctions.GetDefaultLoanType(title.MediaID)
            });
        }

        [HttpGet]
        public ActionResult DeleteItem(int id = 0)
        {
            var title = _repository.GetById<Title>(id);
            if (title == null)
            {
                return HttpNotFound();
            }
            // Create new instance of DeleteConfirmationViewModel and pass it to _DeleteConfirmation Dialog (Reusable)
            DeleteConfirmationViewModel deleteConfirmationViewModel = new DeleteConfirmationViewModel
            {
                DeleteEntityId = id,
                HeaderText = _entityName,
                PostDeleteAction = "Delete",
                PostDeleteController = "Titles",
                DetailsText = title.Title1
            };
            return PartialView("_DeleteConfirmation", deleteConfirmationViewModel);
        }

        [HttpPost]
        public ActionResult DeleteItem(DeleteConfirmationViewModel dcvm)
        {
            // Get item which we need to delete from EntityFramework
            var title = _repository.GetById<Title>(dcvm.DeleteEntityId);

            if (title == null)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _repository.Delete(title);
                    return Json(new { success = true });
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", e.Message);
                }
            }
            return PartialView("_DeleteConfirmation", dcvm);
        }

        [Route("TitlesNoCopies")]
        [Route("TitlesWithoutCopies")]
        [Route("~/LibraryAdmin/Titles/TitlesNoCopies")]
        public ActionResult TitlesNoCopies()
        {
            var viewModel = new TitlesListViewModel
            {
                Titles = _db.Titles.Where(t => t.Copies.Count == 0).ToList()
            };
            if (!viewModel.Titles.Any())
            {
                TempData["NoData"] = "You have no Titles without Copies!";
            }
            ViewBag.Title = "Titles Without Copies";
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("titlesSeeAlso", ControllerContext.RouteData.Values["action"].ToString());
            return View(viewModel);
        }

        [Route("TitlesNoImage")]
        [Route("~/LibraryAdmin/Titles/TitlesNoImage")]
        public ActionResult TitlesNoImage(bool Isbn = false)
        {
            var titles = _db.Titles.Where(t => t.TitleImages.Count == 0);
            if (Isbn)
            {
                titles = from t in titles where (t.ISBN10 != null) || (t.ISBN13 != null) select t;
            }

            var viewModel = new TitlesListViewModel { Titles = titles.ToList() };
            ViewBag.Title = "Titles Without Cover Image";
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("titlesSeeAlso", ControllerContext.RouteData.Values["action"].ToString());
            return View(viewModel);
        }

        [Route("TitlesNoIsbn")]
        [Route("~/LibraryAdmin/Titles/TitlesNoIsbn")]
        public ActionResult TitlesNoIsbn()
        {
            var viewModel = new TitlesListViewModel { Titles = _db.Titles.Where(t => t.ISBN10 == null && t.ISBN13 == null).ToList() };
            ViewBag.Title = "Titles Without an ISBN";
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("titlesSeeAlso", ControllerContext.RouteData.Values["action"].ToString());
            return View(viewModel);
        }


        [Route("RecentlyAdded")]
        [Route("~/LibraryAdmin/Titles/RecentlyAdded")]
        public ActionResult RecentlyAdded(int days = 21)
        {
            var viewModel = new TitlesListViewModel { Titles = _db.Titles.Where(t => DbFunctions.DiffDays(t.DateCatalogued, DateTime.Now) < days).ToList() };
            ViewBag.Title = "Recently Catalogued Titles";
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("titlesSeeAlso", ControllerContext.RouteData.Values["action"].ToString());
            ViewData["SelectDays"] = new Dictionary<string, string>
            {
                {"1", "Last day"},
                {"7", "Last week"},
                {"14", "Last two weeks"},
                {"21", "Last three weeks"},
                {"28", "Last month"},
                {"56", "Last two months"},
                {"84", "Last three months"}
            };
            return View(viewModel);
        }

        [Route("NewTitles")]
        [Route("NewAcquisitions")]
        [Route("~/LibraryAdmin/Titles/NewTitles")]
        public ActionResult NewTitles()
        {
            //Create a new list for us to add stuff to later ...
            var newTitlesList = new List<NewTitlesListViewModel>();

            //Get a collection of items on the 'New Titles' list ...
            var allnewtitles = (from t in _db.Titles
                                join c in _db.Copies on t.TitleID equals c.TitleID
                                where c.AcquisitionsList
                                select new { t.TitleID, t.Title1, t.Publisher.PublisherName, t.Edition, Isbn = string.IsNullOrEmpty(t.ISBN13) ? t.ISBN10 : t.ISBN13, t.Year, c.CopyNumber, c.CopyID, c.Location.Location1, c.AddedToAcquisitions }).ToList();

            //Now loop through the collection and add each record to out newTitles list, created above ...
            foreach (var row in allnewtitles)
            {
                var newTitle = new NewTitlesListViewModel
                {
                    TitleId = row.TitleID,
                    CopyId = row.CopyID,
                    Title = row.Title1 ?? "",
                    Publisher = row.PublisherName ?? "",
                    Edition = row.Edition ?? "",
                    ISBN = row.Isbn ?? "",
                    Year = row.Year ?? "",
                    Copy = row.CopyNumber,
                    Location = row.Location1 ?? "",
                    DateAdded = row.AddedToAcquisitions ?? DateTime.Now
                };
                var authorString = _db.Titles.Find(row.TitleID).AuthorString;
                newTitle.Author = authorString ?? "";
                newTitlesList.Add(newTitle);
            }

            ViewBag.Title = DbRes.T("NewTitlesList", "EntityType"); ;
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("titlesSeeAlso", ControllerContext.RouteData.Values["action"].ToString());
            return View(newTitlesList);
        }

        // Create a simple dictionary that we can used to fill a dropdown list in views
        public Dictionary<string, string> GetAvailableItemsTypes()
        {
            return new Dictionary<string, string>
            {
                {"a2z", "A-Z By Title"},
                {"dateadded", "Recently Added"}
            };
        }

        //Add to NewTitles list
        [Route("AddToNewTitlesList")]
        [Route("AddToAcquisitionsList")]
        [Route("AddToBinding")]
        [Route("~/LibraryAdmin/Titles/AddToNewTitlesList")]
        public ActionResult AddToNewTitlesList()
        {
            var newTitlesList = DbRes.T("NewTitlesList", "EntityType");
            ListBoxViewModel viewModel = new ListBoxViewModel
            {
                PostSelectController = "Titles",
                PostSelectAction = "PostAddToNewTitlesList",
                SelectedItems = null,
                HeaderText = "Add items to '" + newTitlesList + "'",
                DetailsText = "Select the items you wish to add to the '" + newTitlesList + "'.",
                SelectLabel = "Select Items",
                AvailableItems =
                    _db.Copies.Where(c => c.AcquisitionsList == false)
                        .OrderBy(c => c.Title.Title1.Substring(c.Title.NonFilingChars))
                        .ThenBy(c => c.CopyNumber)
                        .Select(x => new SelectListItem
                        {
                            Value = x.CopyID.ToString(),
                            Text = x.Title.Title1 + " - Copy: " + x.CopyNumber.ToString()
                        })
                        .ToList()
            };

            ViewBag.AvailableItemsTypes = GetAvailableItemsTypes();
            ViewBag.Title = "Add items to '" + newTitlesList + "'";
            return PartialView("_MultiSelectListBox", viewModel);
        }

        //Method used to supply a JSON list of copies when selecting a title (Ajax stuf)
        public JsonResult GetListRows(string listType = "a2z")
        {
            IEnumerable<SelectListItem> availableItems;

            if (listType == "a2z")
            {
                availableItems = _db.Copies.Where(c => c.AcquisitionsList == false)
                  .OrderBy(c => c.Title.Title1.Substring(c.Title.NonFilingChars))
                  .ThenBy(c => c.CopyNumber)
                  .Select(x => new SelectListItem
                  {
                      Value = x.CopyID.ToString(),
                      Text = x.Title.Title1 + " - Copy: " + x.CopyNumber.ToString()
                  })
                  .ToList();
            }
            else
            {
                availableItems = _db.Copies.Where(c => c.AcquisitionsList == false)
                  .OrderByDescending(c => c.Title.TitleID)
                  .ThenBy(c => c.CopyNumber)
                  .Select(x => new SelectListItem
                  {
                      Value = x.CopyID.ToString(),
                      Text = x.Title.Title1 + " - Copy: " + x.CopyNumber.ToString()
                  })
                  .ToList();
            }

            return Json(new
            {
                success = true,
                AvailableItems = availableItems
            });
        }

        //Add to NewTitles list
        [HttpPost]
        public ActionResult PostAddToNewTitlesList(ListBoxViewModel viewModel)
        {
            foreach (var copyid in viewModel.SelectedItems)
            {
                var copy = _db.Copies.Find(int.Parse(copyid));
                if (copy != null)
                {
                    copy.AcquisitionsList = true;
                    copy.LastModified = DateTime.Now;
                    copy.AddedToAcquisitions = DateTime.Now;
                    if (ModelState.IsValid)
                    {
                        try
                        {
                            _db.Entry(copy).State = EntityState.Modified;
                            _db.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            ModelState.AddModelError("", e.Message);
                        }
                        CacheProvider.RemoveCache("newtitles");
                    }
                }
            }
            //return RedirectToAction("NewTitles");
            return Json(new { success = true }); 
        }


        //Method to confirm item is to be remove from New Titles list
        public ActionResult RemoveFromNewTitlesList(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("NewTitles");
            }
            var item = _repository.GetById<Copy>(id.Value);
            if (item == null)
            {
                return RedirectToAction("NewTitles");
            }
            var newTitlesList = DbRes.T("NewTitlesList", "EntityType");
            // Create new instance of DeleteConfirmationViewModel and pass it to _DeleteConfirmation Dialog (Reusable)
            DeleteConfirmationViewModel deleteConfirmationViewModel = new DeleteConfirmationViewModel
            {
                DeleteEntityId = id.Value,
                // HeaderText = "New " + ViewBag.Title,
                FunctionText = "Remove item from '" + newTitlesList + "'",
                PostDeleteAction = "PostRemoveFromNewTitlesList",
                ButtonText = "Remove",
                ButtonClass = "btn-success",
                ButtonGlyphicon = "glyphicon-remove",
                ConfirmationHeaderText = "You are about to remove the following item from the '" + newTitlesList + "'?",
                PostDeleteController = "Titles",
                DetailsText = item.Title.Title1 + " - Copy: " + item.CopyNumber
            };
            return PartialView("_DeleteConfirmation", deleteConfirmationViewModel);
        }

        //Method called after confirming removal from New Titles list
        [HttpPost]
        public ActionResult PostRemoveFromNewTitlesList(DeleteConfirmationViewModel dcvm)
        {
            var item = _repository.GetById<Copy>(dcvm.DeleteEntityId);
            if (item == null)
            {
                return RedirectToAction("NewTitles");
            }
            item.AcquisitionsList = false;
            item.AddedToAcquisitions = null;
            item.LastModified = DateTime.Now;
            _repository.Update(item);
            CacheProvider.RemoveCache("newtitles");
            return Json(new { success = true });
        }


        [Route("ClearNewTitles")]
        [Route("ClearAcquisitionsLIst")]
        [Route("~/LibraryAdmin/Titles/ClearNewTitles")]
        public ActionResult ClearNewTitles()
        {
            var newTitlesList = DbRes.T("NewTitlesList", "EntityType");
            var gcvm = new GenericConfirmationViewModel
            {
                PostConfirmController = "Titles",
                PostConfirmAction = "PostClearNewTitles",
                ConfirmationText = "Are you sure you want to continue?",
                DetailsText = "You are about to remove all items from the '" + newTitlesList + "'.",
                ConfirmButtonText = "Clear",
                ConfirmButtonClass = "btn-danger",
                CancelButtonText = "Cancel",
                HeaderText = "Clear '" + newTitlesList + "'?",
                Glyphicon = "glyphicon-remove"
            };
            return PartialView("_GenericConfirmation", gcvm);
        }

        [HttpPost]
        public ActionResult PostClearNewTitles(GenericConfirmationViewModel model)
        {
            var newTitlesList = from c in _db.Copies
                                join t in _db.Titles on c.TitleID equals t.TitleID
                                where c.AcquisitionsList
                                select c.CopyID;

            try
            {
                foreach (var copyid in newTitlesList)
                {
                    var copy = _repository.GetById<Copy>(copyid);

                    copy.AcquisitionsList = false;
                    copy.AddedToAcquisitions = null;
                    copy.LastModified = DateTime.Now;
                    _repository.Update(copy);
                }
                CacheProvider.RemoveCache("newtitles");
                TempData["SuccessDialogMsg"] = DbRes.T("NewTitlesList", "EntityType") + " has been cleared.";
                return Json(new { success = true });
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message.ToString());
                return Json(new { success = false });
            }
        }

        [HttpPost]
        public JsonResult IsbnUnique(string isbn)
        {
            var titleId = 0;
            isbn = CleanIsbn(isbn);
            var title = _db.Titles.FirstOrDefault(t => t.ISBN10 == isbn || t.ISBN13 == isbn);
            if (title != null)
            {
                titleId = title.TitleID;
            }
            return Json(new { titleId = titleId });
        }


        [Route("QuickFind")]
        [Route("~/LibraryAdmin/Titles/QuickFind")]
        public ActionResult QuickFind(string searchTerm = "")
        {
            return RedirectToAction("SimpleSearch", "Home", new { q = searchTerm });
        }

        [Route("ReportGenerator")]
        [Route("~/LibraryAdmin/Titles/ReportGenerator")]
        public ActionResult ReportGenerator()
        {
            var notSelected = new[]
            {
                "Destroyed",
                "Missing",
                "Exclude"
            };

            var viewModel = new ReportsGeneratorViewModel
            {
                Reports = SelectListHelper.CatalogueReportsList(),//listReports,
                Classmarks = SelectListHelper.ClassmarkList(addDefault: false),
                Keywords = SelectListHelper.KeywordList(addDefault: false),
                MediaTypes = SelectListHelper.MediaTypeList(addDefault: false),
                Locations = SelectListHelper.OfficeLocationList(addDefault: false),
                Offices = SelectListHelper.OfficeList(addDefault: false),
                Publishers = SelectListHelper.PublisherList(addDefault: false),
                StatusTypes = _db.StatusTypes.ToList().Select(x => new SelectListItem
                {
                    Selected = notSelected.Contains(x.Status) == false,
                    Text = string.IsNullOrEmpty(x.Status) ? "<no name>" : x.Status,
                    Value = x.StatusID.ToString()
                }),
                StartDate = DateTime.Now.AddYears(-1),
                EndDate = DateTime.Now.Date
            };

            ViewBag.Title = "Report Generator";
            return View(viewModel);
        }


        [HttpPost]
        public ActionResult PostReportGenerator(ReportsGeneratorViewModel viewModel)
        {
            var reportId = 0;
            var reportName = "";
            var friendlyName = "";
            var classmarkId = "";
            var keywordId = "";
            var mediaId = "";
            var locationId = "";
            var statusIdString = "";
            var publisherId = "";
            var officeId = "";
            var startDate = DateTime.Now.AddYears(-1);
            var endDate = DateTime.Now;

            foreach (int r in viewModel.SelectedReport)
            {
                reportId = r;
            };

            if (viewModel.SelectedClassmark != null)
            {
                classmarkId = string.Join(",", viewModel.SelectedClassmark);
            }
            if (viewModel.SelectedPublisher != null)
            {
                publisherId = string.Join(",", viewModel.SelectedPublisher);
            }
            if (viewModel.SelectedKeyword != null)
            {
                keywordId = string.Join(",", viewModel.SelectedKeyword);
            }
            if (viewModel.SelectedMediaType != null)
            {
                mediaId = string.Join(",", viewModel.SelectedMediaType);
            }
            if (viewModel.SelectedOffice != null)
            {
                officeId = string.Join(",", viewModel.SelectedOffice);
            }
            if (viewModel.SelectedLocations != null)
            {
                locationId = string.Join(",", viewModel.SelectedLocations);
            }
            if (viewModel.SelectedStatusTypes != null)
            {
                statusIdString = string.Join(",", viewModel.SelectedStatusTypes);
            }
            if (viewModel.StartDate != null)
            {
                startDate = viewModel.StartDate.Date;
            }
            if (viewModel.EndDate != null)
            {
                endDate = viewModel.EndDate.Date;
            }

            if (reportId == 0)
            {
                ViewBag.Title = "Reports Generator";
                return View("ReportGenerator", viewModel);
            }
            var selectedReport = _db.ReportTypes.Find(reportId);
            reportName = selectedReport.ReportName;
            friendlyName = selectedReport.FriendlyName;

            switch (reportName)
            {
                case "NewAcquisitions":
                    {
                        return RedirectToAction("NewAcquisitions_Report", new { caption = friendlyName, statusIdString });
                    }
                case "NewAcquisitionsFull":
                    {
                        return RedirectToAction("NewAcquisitionsFull_Report", new { caption = friendlyName, statusIdString });
                    }
                case "AuthorTitleList":
                    {
                        return RedirectToAction("AuthorTitleList_Report", new { caption = friendlyName, statusIdString });
                    }
                case "CatalogueByAuthorTitle":
                    {
                        return RedirectToAction("CatalogueByAuthorTitle_Report", new { caption = friendlyName, statusIdString });
                    }
                case "CatalogueByTitle":
                    {
                        return RedirectToAction("CatalogueByTitle_Report", new { caption = friendlyName, statusIdString });
                    }
                case "TitleCatSubject":
                    {
                        return RedirectToAction("TitleCatSubject_Report", new { caption = friendlyName, statusIdString });
                    }
                case "ISBNCatalogue":
                    {
                        return RedirectToAction("ISBNCatalogue_Report", new { caption = friendlyName, statusIdString });
                    }
                case "ISBNCatalogueByMedia":
                    {
                        return RedirectToAction("ISBNCatalogueByMedia_Report", new { caption = friendlyName, statusIdString });
                    }
                case "SubjectCatalogueByAuthorTitle":
                    {
                        return RedirectToAction("SubjectCatalogueByAuthorTitle_Report", new { caption = friendlyName, statusIdString });
                    }
                case "SubjectCatalogueByTitle":
                    {
                        return RedirectToAction("SubjectCatalogueByTitle_Report", new { caption = friendlyName, statusIdString });
                    }
                case "ClassmarkCatalogueByAuthorTitle":
                    {
                        return RedirectToAction("ClassmarkCatalogueByAuthorTitle_Report", new { caption = friendlyName, statusIdString });
                    }
                case "ClassmarkCatalogueByTitle":
                    {
                        return RedirectToAction("ClassmarkCatalogueByTitle_Report", new { caption = friendlyName, statusIdString });
                    }
                case "SubjectCatalogueChosenSubjectByAuthorTitle":
                    {
                        return RedirectToAction("SubjectCatalogueChosenSubjectByAuthorTitle_Report", new { caption = friendlyName, statusIdString, keywordId });
                    }
                case "SubjectCatalogueChosenSubjectByTitle":
                    {
                        return RedirectToAction("SubjectCatalogueChosenSubjectByTitle_Report", new { caption = friendlyName, statusIdString, keywordId });
                    }
                case "ClassmarkCatalogueChosenClassmarkByAuthorTitle":
                    {
                        return RedirectToAction("ClassmarkCatalogueChosenClassmarkByAuthorTitle_Report", new { caption = friendlyName, statusIdString, classmarkIdString = classmarkId });
                    }
                case "ClassmarkCatalogueChosenClassmarkByTitle":
                    {
                        return RedirectToAction("ClassmarkCatalogueChosenClassmarkByTitle_Report", new { caption = friendlyName, statusIdString, classmarkIdString = classmarkId });
                    }
                case "CatalogueSelectedMediaByAuthorTitle":
                    {
                        return RedirectToAction("CatalogueSelectedMediaByAuthorTitle_Report", new { caption = friendlyName, statusIdString, mediaTypeString = mediaId });
                    }
                case "ClassmarkCatalogueSelectedMediaByAuthorTitle":
                    {
                        return RedirectToAction("ClassmarkCatalogueSelectedMediaByAuthorTitle_Report", new { caption = friendlyName, statusIdString, mediaTypeString = mediaId });
                    }
                case "SubjectCatalogueSelectedMediaByAuthorTitle":
                    {
                        return RedirectToAction("SubjectCatalogueSelectedMediaByAuthorTitle_Report", new { caption = friendlyName, statusIdString, mediaTypeString = mediaId });
                    }
                case "CatalogueChosenOfficeByAuthorTitle":
                    {
                        return RedirectToAction("CatalogueChosenOfficeByAuthorTitle_Report", new { caption = friendlyName, statusIdString, officeId });
                    }
                case "ClassmarkCatalogueChosenOfficeByAuthorTitle":
                    {
                        return RedirectToAction("ClassmarkCatalogueChosenOfficeByAuthorTitle_Report", new { caption = friendlyName, statusIdString, officeId });
                    }
                case "SubjectCatalogueChosenOfficeByAuthorTitle":
                    {
                        return RedirectToAction("SubjectCatalogueChosenOfficeByAuthorTitle_Report", new { caption = friendlyName, statusIdString, officeId });
                    }
                case "CatalogueChosenLocationByAuthorTitle":
                    {
                        return RedirectToAction("CatalogueChosenLocationByAuthorTitle_Report", new { caption = friendlyName, statusIdString, locationId });
                    }
                case "ClassmarkCatalogueChosenLocationByAuthorTitle":
                    {
                        return RedirectToAction("ClassmarkCatalogueChosenLocationByAuthorTitle_Report", new { caption = friendlyName, statusIdString, locationId });
                    }
                case "SubjectCatalogueChosenLocationByAuthorTitle":
                    {
                        return RedirectToAction("SubjectCatalogueChosenLocationByAuthorTitle_Report", new { caption = friendlyName, statusIdString, locationId });
                    }
                case "CatalogueChosenPublisherByAuthorTitle":
                    {
                        return RedirectToAction("CatalogueChosenPublisherByAuthorTitle_Report", new { caption = friendlyName, statusIdString, publisherId });
                    }
                case "ClassmarkCatalogueChosenPublisherByAuthorTitle":
                    {
                        return RedirectToAction("ClassmarkCatalogueChosenPublisherByAuthorTitle_Report", new { caption = friendlyName, statusIdString, publisherId });
                    }
                case "SubjectCatalogueChosenPublisherByAuthorTitle":
                    {
                        return RedirectToAction("SubjectCatalogueChosenPublisherByAuthorTitle_Report", new { caption = friendlyName, statusIdString, publisherId });
                    }
                case "CatalogueSelectedLocationsByAuthorTitle":
                    {
                        return RedirectToAction("CatalogueSelectedLocationsByAuthorTitle_Report", new { caption = friendlyName, statusIdString, locationIdString = locationId });
                    }
                case "CatalogueStocktake":
                    {
                        return RedirectToAction("CatalogueStocktake_Report", new { caption = friendlyName, statusIdString, officeId });
                    }
                case "CopiesCataloguedBetweenDates":
                    {
                        return RedirectToAction("CopiesCataloguedBetweenDates_Report", new { caption = friendlyName, statusIdString, startDate, endDate });
                    }
                default:
                    {
                        return null;
                    }
            }
        }


        public ActionResult AuthorTitleList_Report(string caption = "", string statusIdString = "")
        {
            var allcopies = from c in _db.Copies where statusIdString.Contains(c.StatusID.ToString()) select c;
            var alltitles = from t in _db.Titles select t;

            var titles = from title in alltitles
                         join copy in allcopies on title.TitleID equals copy.TitleID into gj
                         from subtitles in gj.DefaultIfEmpty()
                         select title;

            var viewModel = new TitlesReportsViewModel
            {
                Titles = titles,
                HasData = titles.Any()
            };

            ViewBag.Title = caption;
            return View("Reports/AuthorTitleList", viewModel);
        }


        public ActionResult CatalogueByAuthorTitle_Report(string caption = "", string statusIdString = "")
        {
            int[] statusTypes = statusIdString.Split(',').Select(n => Convert.ToInt32(n)).ToArray();

            var titles = (from c in _db.Copies
                          where statusTypes.Contains(c.StatusID.Value) && c.Deleted == false && c.Title.Deleted == false
                          select c.Title).Distinct();

            var viewModel = new TitlesReportsViewModel
            {
                Titles = titles,
                StatusTypes = statusTypes,
                HasData = titles.Any()
            };

            ViewBag.Title = caption;
            return View("Reports/CatalogueByAuthorTitle", viewModel);
        }

        public ActionResult CatalogueByTitle_Report(string caption = "", string statusIdString = "")
        {
            int[] statusTypes = statusIdString.Split(',').Select(n => Convert.ToInt32(n)).ToArray();

            var titles = (from c in _db.Copies
                          where statusTypes.Contains(c.StatusID.Value)
                          select c.Title).Distinct();

            var viewModel = new TitlesReportsViewModel
            {
                Titles = titles,
                StatusTypes = statusTypes,
                HasData = titles.Any()
            };

            ViewBag.Title = caption;
            return View("Reports/CatalogueByTitle", viewModel);
        }

        public ActionResult CatalogueChosenLocationByAuthorTitle_Report(string caption = "", string statusIdString = "", int locationId = 0)
        {
            int[] statusTypes = statusIdString.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
            var location = _db.Locations.Find(locationId);

            var titles = (from c in _db.Copies
                          where statusTypes.Contains(c.StatusID.Value) && c.LocationID == locationId
                          select c.Title).Distinct();

            var viewModel = new TitlesReportsViewModel
            {
                Titles = titles,
                Location = location,
                StatusTypes = statusTypes,
                HasData = titles.Any()
            };

            ViewBag.Title = caption;
            return View("Reports/CatalogueChosenLocationByAuthorTitle", viewModel);
        }

        public ActionResult CatalogueChosenOfficeByAuthorTitle_Report(string caption = "", string statusIdString = "", int officeId = 0)
        {
            int[] statusTypes = statusIdString.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
            var office = _db.Locations.Find(officeId);

            var titles = (from c in _db.Copies
                          where statusTypes.Contains(c.StatusID.Value) && (c.Location.ParentLocation.LocationID == officeId || c.LocationID == officeId)
                          select c.Title).Distinct();

            var viewModel = new TitlesReportsViewModel
            {
                Titles = titles,
                Office = office,
                StatusTypes = statusTypes,
                HasData = titles.Any()
            };

            ViewBag.Title = caption;
            return View("Reports/CatalogueChosenOfficeByAuthorTitle", viewModel);
        }

        public ActionResult CatalogueChosenPublisherByAuthorTitle_Report(string caption = "", string statusIdString = "", int publisherId = 0)
        {
            int[] statusTypes = statusIdString.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
            var publisher = _db.Publishers.FirstOrDefault(p => p.PublisherID == publisherId);

            var titles = (from t in _db.Titles
                          join c in _db.Copies on t.TitleID equals c.TitleID
                          where statusTypes.Contains(c.StatusID.Value) && t.PublisherID == publisher.PublisherID
                          select t).Distinct();

            var viewModel = new TitlesReportsViewModel
            {
                Titles = titles,
                Publisher = publisher,
                StatusTypes = statusTypes,
                HasData = titles.Any()
            };

            ViewBag.Title = caption;
            return View("Reports/CatalogueChosenPublisherByAuthorTitle", viewModel);
        }

        public ActionResult CatalogueSelectedLocationsByAuthorTitle_Report(string caption = "", string statusIdString = "", string locationIdString = "")
        {
            int[] statusTypes = statusIdString.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
            int[] locationIds = locationIdString.Split(',').Select(n => Convert.ToInt32(n)).ToArray();

            var locations = (from c in _db.Copies
                             where statusTypes.Contains(c.StatusID.Value) && locationIds.Contains(c.LocationID.Value)
                             select c.Location).Distinct();

            var viewModel = new TitlesReportsViewModel
            {
                Locations = locations,
                StatusTypes = statusTypes,
                HasData = locations.Any()
            };

            ViewBag.Title = caption;
            return View("Reports/CatalogueSelectedLocationsByAuthorTitle", viewModel);
        }

        public ActionResult CatalogueSelectedMediaByAuthorTitle_Report(string caption = "", string statusIdString = "", string mediaTypeString = "")
        {
            int[] statusTypes = statusIdString.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
            int[] mediaTypeIds = mediaTypeString.Split(',').Select(n => Convert.ToInt32(n)).ToArray();

            var mediaTypes = (from t in _db.Titles
                              join c in _db.Copies on t.TitleID equals c.TitleID
                              where mediaTypeIds.Contains(t.MediaID) && statusTypes.Contains(c.StatusID.Value)
                              select t.MediaType).Distinct();

            var viewModel = new TitlesReportsViewModel
            {
                MediaTypes = mediaTypes,
                StatusTypes = statusTypes,
                HasData = mediaTypes.Any()
            };

            ViewBag.Title = caption;
            return View("Reports/CatalogueSelectedMediaByAuthorTitle", viewModel);
        }

        public ActionResult CatalogueStocktake_Report(string caption = "", string statusIdString = "", int officeId = 0)
        {
            var office = _db.Locations.Find(officeId);
            int[] statusTypes = statusIdString.Split(',').Select(n => Convert.ToInt32(n)).ToArray();

            var classmarks = (from t in _db.Titles
                              join c in _db.Copies on t.TitleID equals c.TitleID
                              where statusTypes.Contains(c.StatusID.Value) && c.Location.ParentLocation.LocationID == office.LocationID
                              select t.Classmark).Distinct();

            var viewModel = new TitlesReportsViewModel
            {
                Office = office,
                Classmarks = classmarks,
                StatusTypes = statusTypes,
                HasData = classmarks.Any()
            };

            ViewBag.Title = caption;
            return View("Reports/CatalogueStocktake", viewModel);
        }

        public ActionResult ClassmarkCatalogueByAuthorTitle_Report(string caption = "", string statusIdString = "")
        {
            int[] statusTypes = statusIdString.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
            var classmarks = (from s in _db.Classmarks
                              join t in _db.Titles on s.ClassmarkID equals t.ClassmarkID
                              join c in _db.Copies on t.TitleID equals c.TitleID
                              where statusTypes.Contains(c.StatusID.Value)
                              select s).Distinct();

            var viewModel = new TitlesReportsViewModel
            {
                Classmarks = classmarks,
                StatusTypes = statusTypes,
                HasData = classmarks.Any()
            };

            ViewBag.Title = caption;
            return View("Reports/ClassmarkCatalogueByAuthorTitle", viewModel);
        }

        public ActionResult ClassmarkCatalogueByTitle_Report(string caption = "", string statusIdString = "")
        {
            int[] statusTypes = statusIdString.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
            var classmarks = (from s in _db.Classmarks
                              join t in _db.Titles on s.ClassmarkID equals t.ClassmarkID
                              join c in _db.Copies on t.TitleID equals c.TitleID
                              where statusTypes.Contains(c.StatusID.Value)
                              select s).Distinct();

            var viewModel = new TitlesReportsViewModel
            {
                Classmarks = classmarks,
                StatusTypes = statusTypes,
                HasData = classmarks.Any()
            };

            ViewBag.Title = caption;
            return View("Reports/ClassmarkCatalogueByTitle", viewModel);
        }

        public ActionResult ClassmarkCatalogueChosenClassmarkByAuthorTitle_Report(string caption = "", string statusIdString = "", string classmarkIdString = "")
        {
            int[] statusTypes = statusIdString.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
            int[] classmarkIds = classmarkIdString.Split(',').Select(n => Convert.ToInt32(n)).ToArray();

            var classmarks = (from s in _db.Classmarks
                              join t in _db.Titles on s.ClassmarkID equals t.ClassmarkID
                              join c in _db.Copies on t.TitleID equals c.TitleID
                              where statusTypes.Contains(c.StatusID.Value) && classmarkIds.Contains(s.ClassmarkID)
                              select s).Distinct();

            var viewModel = new TitlesReportsViewModel
            {
                Classmarks = classmarks,
                StatusTypes = statusTypes,
                HasData = classmarks.Any()
            };

            ViewBag.Title = caption;
            return View("Reports/ClassmarkCatalogueChosenClassmarkByAuthorTitle", viewModel);
        }

        public ActionResult ClassmarkCatalogueChosenClassmarkByTitle_Report(string caption = "", string statusIdString = "", string classmarkIdString = "")
        {
            int[] statusTypes = statusIdString.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
            int[] classmarkIds = classmarkIdString.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
            var classmarks = (from s in _db.Classmarks
                              join t in _db.Titles on s.ClassmarkID equals t.ClassmarkID
                              join c in _db.Copies on t.TitleID equals c.TitleID
                              where statusTypes.Contains(c.StatusID.Value) && classmarkIds.Contains(s.ClassmarkID)
                              select s).Distinct();

            var viewModel = new TitlesReportsViewModel
            {
                Classmarks = classmarks,
                StatusTypes = statusTypes,
                HasData = classmarks.Any()
            };

            ViewBag.Title = caption;
            return View("Reports/ClassmarkCatalogueChosenClassmarkByTitle", viewModel);
        }

        public ActionResult ClassmarkCatalogueChosenLocationByAuthorTitle_Report(string caption = "", string statusIdString = "", int locationId = 0)
        {
            int[] statusTypes = statusIdString.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
            var location = _db.Locations.Find(locationId);
            var classmarks = (from t in _db.Titles
                              join s in _db.Classmarks on t.ClassmarkID equals s.ClassmarkID
                              join c in _db.Copies on t.TitleID equals c.TitleID
                              where c.Location.LocationID == location.LocationID //&& statusTypes.Contains(c.StatusID.Value)
                              select s).Distinct();

            var viewModel = new TitlesReportsViewModel
            {
                Location = location,
                Classmarks = classmarks,
                StatusTypes = statusTypes,
                HasData = classmarks.Any()
            };

            ViewBag.Title = caption;
            return View("Reports/ClassmarkCatalogueChosenLocationByAuthorTitle", viewModel);
        }

        public ActionResult ClassmarkCatalogueChosenOfficeByAuthorTitle_Report(string caption = "", string statusIdString = "", int officeId = 0)
        {
            int[] statusTypes = statusIdString.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
            var office = _db.Locations.Find(officeId);
            var classmarks = (from t in _db.Titles
                              join s in _db.Classmarks on t.ClassmarkID equals s.ClassmarkID
                              join c in _db.Copies on t.TitleID equals c.TitleID
                              where statusTypes.Contains(c.StatusID.Value) && (c.Location.ParentLocation.LocationID == office.LocationID || c.LocationID == office.LocationID)
                              select s).Distinct();

            var viewModel = new TitlesReportsViewModel
            {
                Office = office,
                Classmarks = classmarks,
                StatusTypes = statusTypes,
                HasData = classmarks.Any()
            };

            ViewBag.Title = caption;
            return View("Reports/ClassmarkCatalogueChosenOfficeByAuthorTitle", viewModel);
        }

        public ActionResult ClassmarkCatalogueChosenPublisherByAuthorTitle_Report(string caption = "", string statusIdString = "", int publisherId = 0)
        {
            int[] statusTypes = statusIdString.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
            var publisher = _db.Publishers.Find(publisherId);
            var classmarks = (from t in _db.Titles
                              join s in _db.Classmarks on t.ClassmarkID equals s.ClassmarkID
                              join c in _db.Copies on t.TitleID equals c.TitleID
                              where t.PublisherID == publisher.PublisherID && statusTypes.Contains(c.StatusID.Value)
                              select s).Distinct();

            var viewModel = new TitlesReportsViewModel
            {
                Publisher = publisher,
                Classmarks = classmarks,
                StatusTypes = statusTypes,
                HasData = classmarks.Any()
            };

            ViewBag.Title = caption;
            return View("Reports/ClassmarkCatalogueChosenPublisherByAuthorTitle", viewModel);
        }

        public ActionResult ClassmarkCatalogueSelectedMediaByAuthorTitle_Report(string caption = "", string statusIdString = "", string mediaTypeString = "")
        {
            int[] statusTypes = statusIdString.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
            int[] mediaTypeIds = mediaTypeString.Split(',').Select(n => Convert.ToInt32(n)).ToArray();

            var mediaTypes = (from m in _db.MediaTypes
                              join t in _db.Titles on m.MediaID equals t.MediaID
                              join c in _db.Copies on t.TitleID equals c.TitleID
                              where mediaTypeIds.Contains(m.MediaID) && statusTypes.Contains(c.StatusID.Value)
                              select m).Distinct();

            var viewModel = new TitlesReportsViewModel
            {
                MediaTypes = mediaTypes,
                StatusTypes = statusTypes,
                HasData = mediaTypes.Any()
            };

            ViewBag.Title = caption;
            return View("Reports/ClassmarkCatalogueSelectedMediaByAuthorTitle", viewModel);
        }

        public ActionResult ISBNCatalogue_Report(string caption = "", string statusIdString = "")
        {
            int[] statusTypes = statusIdString.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
            var titles = from t in _db.Titles
                         join c in _db.Copies on t.TitleID equals c.TitleID
                         where statusTypes.Contains(c.StatusID.Value)
                         select t;

            var viewModel = new TitlesReportsViewModel
            {
                Titles = titles,
                StatusTypes = statusTypes,
                HasData = titles.Any()
            };

            ViewBag.Title = caption;
            return View("Reports/ISBNCatalogue", viewModel);
        }

        public ActionResult ISBNCatalogueByMedia_Report(string caption = "", string statusIdString = "")
        {
            int[] statusTypes = statusIdString.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
            var mediaTypes = (from t in _db.Titles
                              join c in _db.Copies on t.TitleID equals c.TitleID
                              where statusTypes.Contains(c.StatusID.Value)
                              select t.MediaType).Distinct();

            var viewModel = new TitlesReportsViewModel
            {
                MediaTypes = mediaTypes,
                StatusTypes = statusTypes,
                HasData = mediaTypes.Any()
            };

            ViewBag.Title = caption;
            return View("Reports/ISBNCatalogueByMedia", viewModel);
        }

        public ActionResult NewAcquisitions_Report(string caption = "", string statusIdString = "")
        {
            int[] statusTypes = statusIdString.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
            var titles = from t in _db.Titles
                         join c in _db.Copies on t.TitleID equals c.TitleID
                         where statusTypes.Contains(c.StatusID.Value) && c.AcquisitionsList
                         select t;

            var classmarks = (from t in titles
                              select t.Classmark).Distinct();

            var viewModel = new TitlesReportsViewModel
            {
                //NewTitles = newTitles,
                Classmarks = classmarks,
                StatusTypes = statusTypes,
                HasData = titles.Any()
            };

            ViewBag.Title = caption;
            return View("Reports/NewAcquisitionsReport", viewModel);
        }

        public ActionResult NewAcquisitionsFull_Report(string caption = "", string statusIdString = "")
        {
            int[] statusTypes = statusIdString.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
            var titles = from t in _db.Titles
                         join c in _db.Copies on t.TitleID equals c.TitleID
                         where statusTypes.Contains(c.StatusID.Value) && c.AcquisitionsList
                         select t;

            var classmarks = (from t in titles
                              select t.Classmark).Distinct();

            var viewModel = new TitlesReportsViewModel
            {
                //NewTitles = newTitles,
                Classmarks = classmarks,
                StatusTypes = statusTypes,
                HasData = titles.Any()
            };

            ViewBag.Title = caption;
            return View("Reports/NewAcquisitionsFullReport", viewModel);
        }

        public ActionResult SubjectCatalogueByAuthorTitle_Report(string caption = "", string statusIdString = "")
        {
            int[] statusTypes = statusIdString.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
            var keywords = (from x in _db.SubjectIndexes
                            join c in _db.Copies on x.TitleID equals c.TitleID
                            where c.StatusID != null && statusTypes.Contains(c.StatusID.Value)
                            select x.Keyword).Distinct();

            var viewModel = new TitlesReportsViewModel
            {
                Keywords = keywords,
                StatusTypes = statusTypes,
                HasData = keywords.Any()
            };

            ViewBag.Title = caption;
            return View("Reports/SubjectCatalogueByAuthorTitle", viewModel);
        }

        public ActionResult SubjectCatalogueByTitle_Report(string caption = "", string statusIdString = "")
        {
            int[] statusTypes = statusIdString.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
            var keywords = (from x in _db.SubjectIndexes
                            join c in _db.Copies on x.TitleID equals c.TitleID
                            where c.StatusID != null && statusTypes.Contains(c.StatusID.Value)
                            select x.Keyword).Distinct();

            var viewModel = new TitlesReportsViewModel
            {
                Keywords = keywords,
                StatusTypes = statusTypes,
                HasData = keywords.Any()
            };

            ViewBag.Title = caption;
            return View("Reports/SubjectCatalogueByTitle", viewModel);
        }

        public ActionResult SubjectCatalogueChosenLocationByAuthorTitle_Report(string caption = "", string statusIdString = "", int locationId = 0)
        {
            int[] statusTypes = statusIdString.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
            var location = _db.Locations.Find(locationId);
            var keywords = (from x in _db.SubjectIndexes
                            join c in _db.Copies on x.TitleID equals c.TitleID
                            where c.LocationID == location.LocationID && statusTypes.Contains(c.StatusID.Value)
                            select x.Keyword).Distinct();

            var viewModel = new TitlesReportsViewModel
            {
                Location = location,
                Keywords = keywords,
                StatusTypes = statusTypes,
                HasData = keywords.Any()
            };

            ViewBag.Title = caption;
            return View("Reports/SubjectCatalogueChosenLocationByAuthorTitle", viewModel);
        }

        public ActionResult SubjectCatalogueChosenOfficeByAuthorTitle_Report(string caption = "", string statusIdString = "", int officeId = 0)
        {
            int[] statusTypes = statusIdString.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
            var office = _db.Locations.Find(officeId);
            var keywords = (from x in _db.SubjectIndexes
                            join c in _db.Copies on x.TitleID equals c.TitleID
                            where statusTypes.Contains(c.StatusID.Value) && (c.Location.ParentLocation.LocationID == office.LocationID || c.LocationID == office.LocationID)
                            select x.Keyword).Distinct();

            var viewModel = new TitlesReportsViewModel
            {
                Office = office,
                Keywords = keywords,
                StatusTypes = statusTypes,
                HasData = keywords.Any()
            };

            ViewBag.Title = caption;
            return View("Reports/SubjectCatalogueChosenOfficeByAuthorTitle", viewModel);
        }

        public ActionResult SubjectCatalogueChosenPublisherByAuthorTitle_Report(string caption = "", string statusIdString = "", int publisherId = 0)
        {
            int[] statusTypes = statusIdString.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
            var publisher = _db.Publishers.Find(publisherId);
            var keywords = (from x in _db.SubjectIndexes
                            join t in _db.Titles on x.TitleID equals t.TitleID
                            join c in _db.Copies on t.TitleID equals c.TitleID
                            where t.PublisherID == publisher.PublisherID && statusTypes.Contains(c.StatusID.Value)
                            select x.Keyword).Distinct();

            var viewModel = new TitlesReportsViewModel
            {
                Publisher = publisher,
                Keywords = keywords,
                StatusTypes = statusTypes,
                HasData = keywords.Any()
            };

            ViewBag.Title = caption;
            return View("Reports/SubjectCatalogueChosenPublisherByAuthorTitle", viewModel);
        }

        public ActionResult SubjectCatalogueChosenSubjectByAuthorTitle_Report(string caption = "", string statusIdString = "", int keywordId = 0)
        {
            int[] statusTypes = statusIdString.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
            var keyword = _db.Keywords.Find(keywordId);
            var titles = (from c in _db.Copies
                          join x in _db.SubjectIndexes on c.TitleID equals x.TitleID
                          where x.KeywordID == keyword.KeywordID && statusTypes.Contains(c.StatusID.Value)
                          select c.Title).Distinct();

            var viewModel = new TitlesReportsViewModel
            {
                Titles = titles,
                Keyword = keyword,
                StatusTypes = statusTypes,
                HasData = titles.Any()
            };

            ViewBag.Title = caption;
            return View("Reports/SubjectCatalogueChosenSubjectByAuthorTitle", viewModel);
        }

        public ActionResult SubjectCatalogueChosenSubjectByTitle_Report(string caption = "", string statusIdString = "", int keywordId = 0)
        {
            int[] statusTypes = statusIdString.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
            var keyword = _db.Keywords.Find(keywordId);
            var titles = (from c in _db.Copies
                          join x in _db.SubjectIndexes on c.TitleID equals x.TitleID
                          where x.KeywordID == keyword.KeywordID && statusTypes.Contains(c.StatusID.Value)
                          select c.Title).Distinct();

            var viewModel = new TitlesReportsViewModel
            {
                Titles = titles,
                Keyword = keyword,
                StatusTypes = statusTypes,
                HasData = titles.Any()
            };

            ViewBag.Title = caption;
            return View("Reports/SubjectCatalogueChosenSubjectByAuthorTitle", viewModel);
        }

        public ActionResult SubjectCatalogueSelectedMediaByAuthorTitle_Report(string caption = "", string statusIdString = "", string mediaTypeString = "")
        {
            int[] statusTypes = statusIdString.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
            int[] mediaTypeIds = mediaTypeString.Split(',').Select(n => Convert.ToInt32(n)).ToArray();

            var mediaTypes = (from m in _db.MediaTypes
                              join t in _db.Titles on m.MediaID equals t.MediaID
                              join c in _db.Copies on t.TitleID equals c.TitleID
                              where mediaTypeIds.Contains(m.MediaID) && t.SubjectIndexes.Any() && statusTypes.Contains(c.StatusID.Value)
                              select m).Distinct();

            var viewModel = new TitlesReportsViewModel
            {
                MediaTypes = mediaTypes,
                StatusTypes = statusTypes,
                HasData = mediaTypes.Any()
            };

            ViewBag.Title = caption;
            return View("Reports/SubjectCatalogueSelectedMediaByAuthorTitle", viewModel);
        }

        public ActionResult TitleCatSubject_Report(string caption = "", string statusIdString = "")
        {
            int[] statusTypes = statusIdString.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
            var titles = (from c in _db.Copies
                          where statusTypes.Contains(c.StatusID.Value)
                          select c.Title).Distinct();

            var viewModel = new TitlesReportsViewModel
            {
                Titles = titles,
                StatusTypes = statusTypes,
                HasData = titles.Any()
            };

            ViewBag.Title = caption;
            return View("Reports/TitleCatSubject", viewModel);
        }

        public ActionResult CatalogueStatistics()
        {
            var offices = _db.Locations.Where(l => l.ParentLocationID == null && l.SubLocations.Any());
            var locations = _db.Locations.Where(l => l.ParentLocationID != null);

            var viewModel = new TitlesReportsViewModel
            {
                TitlesCount = _db.Titles.Count(t => t.Deleted == false),
                TitlesNoMediaCount = _db.Titles.Count(t => t.Deleted == false && t.MediaID == null),
                CopiesCount = _db.Copies.Count(c => c.Deleted == false),
                CopiesNoStatusCount = _db.Copies.Count(c => c.Deleted == false && c.StatusID == null),
                CopiesNoLocationCount = _db.Copies.Count(c => c.Deleted == false && c.LocationID == null),
                CopiesNoOfficeCount = _db.Copies.Count(c => c.Deleted == false && c.LocationID == null),
                VolumesCount = _db.Volumes.Count(v => v.Deleted == false),
                RefOnlyCount = _db.Volumes.Count(v => v.Deleted == false && v.RefOnly),
                VolumesOnLoanCount = _db.Volumes.Count(v => v.Deleted == false && v.OnLoan),
                VolumesAvailableCount = _db.Volumes.Count(v => v.Deleted == false && v.RefOnly == false && v.OnLoan == false),
                MediaTypes = _db.MediaTypes.Where(m => m.Titles.Any()),
                StatusTypesList = _db.StatusTypes.Where(s => s.Copies.Any()),
                Offices = offices,
                Locations = locations,
                HasData = _db.Titles.Any()
            };

            ViewBag.Title = "Catalogue Statistics";
            return View("Reports/CatalogueStatistics", viewModel);
        }


        public ActionResult SubjectAZ_Report()
        {
            var keywords = _db.Keywords.Where(k => k.Deleted == false && k.ParentKeywordID != null);
            var viewModel = new TitlesReportsViewModel
            {
                Keywords = keywords,
                HasData = keywords.Any()
            };

            ViewBag.Title = "Subjects A-Z List";
            return View("Reports/SubjectsAZList", viewModel);
        }


        public ActionResult SubjectStatistics_Report()
        {
            var keywords = _db.Keywords.Where(k => k.SubjectIndexes.Any() && k.Deleted == false);
            var viewModel = new TitlesReportsViewModel
            {
                Keywords = keywords,
                HasData = keywords.Any()
            };

            ViewBag.Title = "Subjects Statistics";
            return View("Reports/SubjectStatistics", viewModel);
        }


        public ActionResult SubjectsAssignedToTitles_Report()
        {
            var keywords = from k in _db.Keywords
                           where k.SubjectIndexes.Any() && k.Deleted == false
                           select k;
            var viewModel = new TitlesReportsViewModel
            {
                Keywords = keywords,
                HasData = keywords.Any()
            };

            ViewBag.Title = "Subjects Assigned To Titles";
            return View("Reports/SubjectsAssignedToTitles", viewModel);
        }

        public ActionResult CopiesCataloguedBetweenDates_Report(string caption = "", string statusIdString = "", DateTime? startDate = null, DateTime? endDate = null)
        {
            if (startDate == null)
            {
                startDate = DateTime.Today.AddYears(-1).Date;
            }
            if (endDate == null)
            {
                endDate = DateTime.Today.Date;
            }
            int[] statusTypes = statusIdString.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
            var titles = (from c in _db.Copies
                          where c.Commenced >= startDate && c.InputDate <= endDate && statusTypes.Contains(c.StatusID.Value) && c.Deleted == false
                          select c.Title).Distinct();
            var displayStartDate = startDate.Value.ToShortDateString();
            var displayEndDate = endDate.Value.ToShortDateString();

            var viewModel = new TitlesReportsViewModel
            {
                Titles = titles,
                StatusTypes = statusTypes,
                HasData = titles.Any()
            };

            ViewBag.Title = "Copies Catalogue Between " + displayStartDate + " and " + displayEndDate;
            return View("Reports/CopiesCataloguedBetweenDates", viewModel);
        }

        [HttpPost]
        public JsonResult CountCopiesAdded(string statusIdString = "", DateTime? startDate = null, DateTime? endDate = null)
        {
            int[] statusTypes = statusIdString.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
            var copiesAdded =
                _db.Copies
                    .Count(c => c.Commenced >= startDate && c.InputDate <= endDate && c.Deleted == false && statusTypes.Contains(c.StatusID.Value));
            return Json(new
            {
                success = true,
                Count = copiesAdded
            });
        }


        [Route("~/LibraryAdmin/Titles/CLAAuditReport")]
        [HttpGet]
        public ActionResult CLAAuditReport()
        {
            List<SelectListItem> expenditureTypes = new List<SelectListItem>
            {
                new SelectListItem {Text="All Titles", Value="0"},
                new SelectListItem {Text="One-off Purchases", Value="1"},
                new SelectListItem {Text="Subscriptions", Value="2"}
            };

            List<SelectListItem> reportTypes = new List<SelectListItem>
            {
                new SelectListItem {Text="Copies Held", Value="1"},
                new SelectListItem {Text="Copies Purchased", Value="2"}
            };

            var notSelected = new[]
            {
                "destroyed",
                "cancelled order",
                "on approval",
                "on order"
            };

            var lstMediaTypes = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "[All Media]",
                    Value = "-1"
                }
            };
            lstMediaTypes.AddRange(_db.MediaTypes.OrderBy(m => m.Media).Select(item => new SelectListItem
            {
                Text = string.IsNullOrEmpty(item.Media) ? "<no name>" : item.Media,
                Value = item.MediaID.ToString()
            }));

            var viewModel = new CLAAuditReportViewModel
            {
                ExpenditureTypes = expenditureTypes,
                ReportTypes = reportTypes,
                MediaTypes = lstMediaTypes, //slls.Utils.Helpers.SelectListHelper.SelectMediaTypeList(addDefault: false),
                StartDate = DateTime.Today.AddYears(-1),
                EndDate = DateTime.Today,
                StatusTypes = _db.StatusTypes.Where(s => s.Status != null).ToList().Select(x => new SelectListItem
                {
                    Selected = notSelected.Contains(x.Status.ToLower()) == false,
                    Text = string.IsNullOrEmpty(x.Status) ? "<no name>" : x.Status,
                    Value = x.StatusID.ToString()
                })
            };

            ViewBag.Title = "CLA Audit Reports";
            return View(viewModel);
        }


        [HttpPost]
        [Route("~/LibraryAdmin/Titles/CLAAuditReport")]
        public ActionResult CLAAuditReport(CLAAuditReportViewModel viewModel)
        {
            var reportType = viewModel.ReportType;
            var expenditureType = viewModel.ExpenditureType;
            var heldOrPurch = "held";
            var boolByDates = viewModel.UseDates;
            var labelBetween = "";
            var startDate = viewModel.StartDate;
            var endDate = viewModel.EndDate;
            var labelReportType = "All Titles";
            var labelNoCopies = "No. Copies Held";
            var friendlyName = "CLA Audit";
            int[] mediaTypes = null;

            if (viewModel.SelectedMediaTypes != null)
            {
                var mediaIdString = string.Join(",", viewModel.SelectedMediaTypes);
                mediaTypes = mediaIdString.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
            }
            var statusIdString = string.Join(",", viewModel.SelectedStatusTypes);

            int[] statusTypes = statusIdString.Split(',').Select(n => Convert.ToInt32(n)).ToArray();

            IEnumerable<Title> titles = from t in _db.Titles
                                        where t.Deleted == false
                                        select t;

            IEnumerable<OrderDetail> orders = from o in _db.OrderDetails
                                              where
                                                  o.ReceivedDate != null && o.ReturnedDate == null && o.NumCopies > 0 && o.OnApproval == false
                                              select o;

            IEnumerable<Title> results;

            if (reportType == 1)
            {
                heldOrPurch = "held";
                labelNoCopies = "No. Copies Held";
                results = (from t in titles
                           join c in _db.Copies on t.TitleID equals c.TitleID
                           where c.StatusID != null && statusTypes.Contains(c.StatusID.Value)
                           select t).Distinct();
            }
            else
            {
                heldOrPurch = "purch";
                labelNoCopies = "No. Copies Purchased";
                results = (from t in titles
                           join o in orders on t.TitleID equals o.TitleID
                           select t).Distinct();
            }

            if (mediaTypes == null || mediaTypes.Length == 0 || mediaTypes[0] == -1)
            {
                friendlyName = friendlyName + " - All Media";
                //mediaTypeParam = "all";
            }
            else
            {
                var media = from m in _db.MediaTypes where mediaTypes.Contains(m.MediaID) select m.Media;
                var mediaTypeParam = string.Join(", ", media);
                friendlyName = friendlyName + " - " + mediaTypeParam;
                results = from r in results
                          where mediaTypes.Contains(r.MediaID)
                          select r;
            }

            if (boolByDates)
            {
                labelBetween = "Between " + startDate.ToString("d") + " and " + endDate.ToString("d");
                friendlyName = friendlyName + " - " + startDate.ToString("d") + " and " + endDate.ToString("d");
                orders = from o in orders
                         where o.ReceivedDate >= startDate && o.ReceivedDate <= endDate && o.ReturnedDate == null
                         select o;
                results = from r in results
                          join o in orders on r.TitleID equals o.TitleID
                          select r;
            }
            else
            {
                friendlyName = friendlyName + " - All Dates";
            }

            switch (expenditureType)
            {
                case 0:
                    labelReportType = "All Titles";
                    friendlyName = friendlyName + " - All Titles";
                    break;
                case 1:
                    labelReportType = "One-off Purchases";
                    friendlyName = friendlyName + " - One-off Purchases";
                    orders = from o in orders
                             where o.OrderCategory.Sub == false
                             select o;
                    results = from r in results
                              join o in orders on r.TitleID equals o.TitleID
                              select r;
                    break;
                case 2:
                    labelReportType = "Subscriptions";
                    friendlyName = friendlyName + " - Subscriptions";
                    orders = from o in orders
                             where o.OrderCategory.Sub
                             select o;
                    results = from r in results
                              join o in orders on r.TitleID equals o.TitleID
                              select r;
                    break;
            }



            viewModel.Titles = results.Distinct();
            viewModel.HasData = results.Any();
            viewModel.FriendlyName = friendlyName;
            viewModel.LabelReportType = labelReportType;
            viewModel.LabelNoCopies = labelNoCopies;
            viewModel.LabelBetween = labelBetween;
            viewModel.StartDate = startDate;
            viewModel.EndDate = endDate;
            viewModel.ReportStatusTypes = statusTypes;
            viewModel.HeldOrPurch = heldOrPurch;
            ViewBag.Title = "CLA Audit Report";
            return View("Reports/CLAAuditReport", viewModel);
        }


        [Route("~/LibraryAdmin/Titles/AddWithAutoCat")]
        [Route("~/LibraryAdmin/Titles/AutoCat")]
        public ActionResult AddWithAutoCat(bool notFound = false, bool notSelected = false)
        {
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("titlesSeeAlso", "AddWithAutoCat");
            ViewData["NotFound"] = notFound;
            ViewData["NotSelected"] = notSelected;
            ViewBag.Title = "AutoCat - Automatic Cataloguing";
            return View();
        }


        public ActionResult _IsbnLookup()
        {
            //Get a list of all data sources (i.e. Amazon;Hammick;Wildys; etc.)
            var allSources = ConfigurationManager.AppSettings["AutoCatDataSources"];
            var sources = allSources.Split(',').ToList();

            var viewModel = new AddTitleWithAutoCatViewModel
            {
                Sources = sources,
                Who = System.Web.HttpContext.Current.User.Identity.Name
            };

            //Get list of any ISBN that cannot be found or returned an error
            var errorList = TempData["badIsbnList"] as List<String>;

            if (errorList != null)
            {
                viewModel.HasErrors = true;
                viewModel.ErrorList = errorList.Select(x =>
                    new SelectListItem
                    {
                        Text = x.ToString()
                    });
            }
            else
            {
                viewModel.HasErrors = false;
                viewModel.ErrorList = Enumerable.Empty<SelectListItem>();
            }

            return PartialView(viewModel);
        }


        [HttpPost]
        public ActionResult PostIsbnLookup(AddTitleWithAutoCatViewModel viewModel, string source, IEnumerable<string> Isbnlist)
        {
            var autoCat = new AutoCat.AutoCat();

            //Start a list of any ISBNs that cannot be found, or that return an error
            var errorList = new List<string>();
            var successCount = 0;
            var titleId = 0;
            double startCount = Isbnlist.Count();
            double progress = 0;

            ProgressHub.NotifyStart("AutoCat has started looking for data to download ...", viewModel.Who);

            //Work through the list of passed ISBNs - there may be only one!
            foreach (var isbn in Isbnlist)
            {
                progress++;
                if (isbn.Length == 10 || isbn.Length == 13)
                {
                    //Get the data from the autoCat plug-in, passing the ISBN and the preferred source
                    var newTitle = autoCat.GetIsbnData(source, isbn);

                    if (newTitle != null && newTitle.ErrorMessage == null)
                    {
                        titleId = AddNewAutoCatTitle(newTitle);

                        successCount++;

                        // Send the current ISBN back to the client. This will remove the processed ISBN from the listbox ...
                        ProgressHub.SendCurrentValue(isbn, viewModel.Who);
                    }
                    else
                    {
                        //Collect any ISBNs that cannot be found or return an error ...
                        errorList.Add(isbn);
                    }
                }

                var progressPercent = (progress / startCount) * 100;
                ProgressHub.NotifyProgress((int)progressPercent + "%", (int)progressPercent, viewModel.Who);
                if (successCount > 0)
                {
                    ProgressHub.SendMessage(successCount + " titles successfully added to the database.", viewModel.Who);
                }
                else
                {
                    ProgressHub.SendMessage("", viewModel.Who);
                }
            }

            //Take the user to the appropriate page ...
            if (errorList.Count > 0)
            {
                // Send a message to the client that the process has finished and there are x number of errors: Stay on the same page.
                ProgressHub.Stop("Oops, there have been errors!", 0, 0, viewModel.Who);
                return null;
            }
            if (successCount > 1)
            {
                // Send a message to the client that the process has finished and there are x number of success: Show the 'Recently Added' page
                ProgressHub.Stop("", successCount, 0, viewModel.Who);
                return null;
            }
            // Send a message to the client that the process has finished and there is just success and no failures: Show the 'Edit Title' page
            ProgressHub.Stop("", 1, titleId, viewModel.Who);
            return null;

        }

        private static int GetNonFilingChars(string title)
        {
            //Get a list of words to look for - dafault is 'A,An,The'
            if (title == null) return 0;
            var param = ConfigurationManager.AppSettings["NonFilingWords"];
            var nonFilingWords = param.Split(',').ToList();
            return (from word in nonFilingWords where title.IndexOf(word + " ", StringComparison.Ordinal) == 0 select word.Length + 1).FirstOrDefault();
        }

        public ActionResult DuplicateTitle(int id = 0)
        {
            if (id == 0)
            {
                return RedirectToAction("Select", new { callingAction = "duplicate" });
            }
            var title = _repository.GetById<Title>(id);
            if (title == null)
            {
                return RedirectToAction("Select", new { callingAction = "duplicate" });
            }

            var viewModel = new DuplicateTitleViewModel
            {
                TitleId = title.TitleID,
                Title1 = title.Title1,
                Edition = title.Edition,
                Isbn = title.ISBN13 ?? title.ISBN10,
                Year = title.Year,
                Description = title.Description,
                Source = "EditTitle"
            };

            ViewBag.Title = "Duplicate " + _entityName;
            return PartialView(viewModel);

        }

        public ActionResult PostDuplicateTitle(DuplicateTitleViewModel viewModel)
        {
            if (viewModel.TitleId == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //Get the exisiting title details that we want to duplicate ...
            var existingTitle = _repository.GetById<Title>(viewModel.TitleId);

            var duplicateTitle = new Title
            {
                Title1 = existingTitle.Title1 + " (Duplicate)",
                Description = existingTitle.Description,
                Edition = existingTitle.Edition,
                ISBN10 = existingTitle.ISBN10,
                ISBN13 = existingTitle.ISBN13,
                Series = existingTitle.Series,
                Citation = existingTitle.Citation,
                Source = existingTitle.Source,
                PlaceofPublication = existingTitle.PlaceofPublication,
                Year = existingTitle.Year,
                MediaID = existingTitle.MediaID,
                ClassmarkID = existingTitle.ClassmarkID,
                LanguageID = existingTitle.LanguageID,
                PublisherID = existingTitle.PublisherID,
                FrequencyID = existingTitle.FrequencyID,
                NonFilingChars = GetNonFilingChars(existingTitle.Title1),
                DateCatalogued = DateTime.Now,
                CataloguedBy = Utils.PublicFunctions.GetCurrentUserName()
            };

            //Save the main title details as a new Title ...
            _db.Titles.Add(duplicateTitle);
            _db.SaveChanges();

            //Establish the new Title ID ...
            var newTitleId = duplicateTitle.TitleID;

            //Do authors ...
            if (existingTitle.TitleAuthors.Any())
            {
                foreach (var author in existingTitle.TitleAuthors)
                {
                    //get the author id ...
                    var authorId = author.AuthorId;

                    if (authorId != 0)
                    {
                        //insert into TitleAuthors ...
                        var ta = new TitleAuthor
                        {
                            TitleId = newTitleId,
                            AuthorId = authorId,
                            InputDate = DateTime.Now
                        };
                        _db.TitleAuthors.Add(ta);
                        _db.SaveChanges();
                    }
                }
            }

            //Do editors ...
            if (existingTitle.TitleEditors.Any())
            {
                foreach (var editor in existingTitle.TitleEditors)
                {
                    //get the editor id ...
                    var editorId = editor.AuthorID;

                    if (editorId != 0)
                    {
                        //insert into TitleEditors ...
                        var ta = new TitleEditor()
                        {
                            TitleID = newTitleId,
                            AuthorID = editorId,
                            InputDate = DateTime.Now
                        };
                        _db.TitleEditors.Add(ta);
                        _db.SaveChanges();
                    }
                }
            }

            //Do keywords ...
            if (existingTitle.SubjectIndexes.Any())
            {
                foreach (var keyword in existingTitle.SubjectIndexes)
                {
                    var keywordId = keyword.KeywordID;

                    if (keywordId != 0)
                    {
                        var subjectIndex = new SubjectIndex
                        {
                            KeywordID = keywordId,
                            TitleID = newTitleId,
                            InputDate = DateTime.Now
                        };
                        _db.SubjectIndexes.Add(subjectIndex);
                        _db.SaveChanges();
                    }
                }
            }

            //Do title texts (e.g. Long Description, content, reviews, etc.)
            if (existingTitle.TitleAdditionalFieldDatas.Any())
            {
                foreach (var text in existingTitle.TitleAdditionalFieldDatas)
                {
                    var fieldId = text.FieldID;
                    var textString = text.FieldData;

                    if (fieldId != 0)
                    {
                        var titleText = new TitleAdditionalFieldData()
                        {
                            TitleID = newTitleId,
                            FieldID = fieldId,
                            FieldData = textString
                        };
                        _db.TitleAdditionalFieldDatas.Add(titleText);
                        _db.SaveChanges();
                    }
                }
            }


            //Do links ...
            if (existingTitle.TitleLinks.Any())
            {
                foreach (var link in existingTitle.TitleLinks)
                {
                    var existingTitleLink = new TitleLink()
                    {
                        TitleID = newTitleId,
                        URL = link.URL,
                        DisplayText = link.DisplayText,
                        HoverTip = link.HoverTip,
                        LinkStatus = link.LinkStatus,
                        Password = link.Password,
                        Login = link.Login,
                        InputDate = DateTime.Now,
                        IsValid = true
                    };
                    _db.TitleLinks.Add(existingTitleLink);
                    _db.SaveChanges();
                }
            }

            //Do image ...
            if (existingTitle.TitleImages.Any())
            {
                foreach (var image in existingTitle.TitleImages)
                {
                    var imageId = image.ImageId;

                    var titleImage = new TitleImage
                        {
                            ImageId = imageId,
                            TitleId = newTitleId,
                            Alt = image.Alt,
                            HoverText = image.HoverText,
                            IsPrimary = image.IsPrimary,
                            InputDate = DateTime.Now
                        };
                    _db.TitleImages.Add(titleImage);
                    _db.SaveChanges();
                }
            }

            //Add a default copy and volume ...
            AddUnmannedCopy(newTitleId);

            if (viewModel.Source == "EditTitle")
            {
                if (newTitleId > 0)
                {
                    UrlHelper urlHelper = new UrlHelper(HttpContext.Request.RequestContext);
                    string actionUrl = urlHelper.Action("Edit", "Titles", new {id = newTitleId});
                    TempData["SuccessMsg"] = "The title '" + existingTitle.Title1 +
                                             "' has been successfully duplicated.";
                    return Json(new {success = true, redirectTo = actionUrl});
                }
                return Json(new {success = false});
            }
            if (newTitleId > 0)
            {
                TempData["SuccessMsg"] = "The title '" + existingTitle.Title1 +
                                         "' has been successfully duplicated.";
                return RedirectToAction("Edit", "Titles", new {id = newTitleId});
            }

            return RedirectToAction("Select", "Titles", new { callingAction = "duplicate" });
        }

        public JsonResult GetTitleDetails(int titleId = 0)
        {
            var selectedTitle = _db.Titles.Find(titleId);
            if (selectedTitle == null)
            {
                return Json(new
                {
                    success = false,
                    Title = "The item you have selected does not exist. Please check and try again."
                });
            }

            var title = selectedTitle.Title1;
            var edition = selectedTitle.Edition;
            var isbn = selectedTitle.Isbn;
            var year = selectedTitle.Year;
            var description = selectedTitle.Description;

            return Json(new
            {
                success = true,
                Title = title,
                Edition = edition,
                Isbn = isbn,
                Year = year,
                Description = description
            });
        }

        public ActionResult PrintDetails(int id = 0)
        {
            if (id == 0)
            {
                return RedirectToAction("Select", new { callingAction = "printdetails" });
            }
            var title = _repository.GetById<Title>(id);
            if (title == null)
            {
                return RedirectToAction("Select", new { callingAction = "printdetails" });
            }

            return RedirectToAction("NotFound", "Home");
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