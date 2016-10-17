using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using slls.App_Settings;
using slls.DAO;
using slls.Models;
using slls.Utils.Helpers;
using slls.ViewModels;
using Westwind.Globalization;

namespace slls.Controllers
{
    [AuthorizeRoles(Roles.Administrator, Roles.Staff, Roles.BsAdmin, Roles.User)]
    public class LibraryUserSavedSearchesController : sllsBaseController
    {
        private readonly DbEntities _db = new DbEntities();
        private readonly GenericRepository _repository;
        private readonly string _entityName = DbRes.T("Saved_Searches.Saved_Search", "FieldDisplayName");

        public LibraryUserSavedSearchesController()
        {
            _repository = new GenericRepository(typeof(LibraryUserSavedSearch));
            ViewBag.Title = DbRes.T("Saved_Searches", "EntityType");
        }

        // GET: LibraryUserSavedSearches
        public ActionResult Index()
        {
            var userId = Utils.PublicFunctions.GetUserId(); //User.Identity.GetUserId();
            var mySavedSearches = _db.LibraryUserSavedSearches.Where(x => x.UserID == userId);

            ViewBag.HasAlert = "false";
            ViewBag.AlertMsg = "";

            if (!mySavedSearches.Any())
            {
                //ViewBag.HasAlert = "true";
                //ViewBag.AlertMsg = "You currently have no " + ViewBag.Title + "!";
                TempData["NoData"] = "You currently have no " + ViewBag.Title + "!";
            }

            ViewBag.Title = "My " + ViewBag.Title;
            return View(mySavedSearches);
        }

        //public ActionResult Add(SimpleSearchingViewModel currentSearch)
        //{
        //    var description = "";
        //    if (!string.IsNullOrEmpty(currentSearch.SearchString))
        //    {
        //        description = currentSearch.SearchString;
        //    }

        //    var viewModel = new LibraryUserSavedSearchViewModel()
        //    {
        //        UserId = User.Identity.GetUserId(),
        //        Description = description,
        //        SearchString = currentSearch.SearchString,
        //        SearchField = currentSearch.SearchField,
        //        Scope = "opac",
        //        AuthorFilter = string.Join(",", currentSearch.AuthorFilter),
        //        ClassmarksFilter = String.Join(",", currentSearch.ClassmarksFilter),
        //        LanguageFilter = String.Join(",", currentSearch.LanguageFilter),
        //        KeywordFilter = String.Join(",", currentSearch.KeywordFilter),
        //        MediaFilter = String.Join(",", currentSearch.MediaFilter),
        //        PublisherFilter = String.Join(",", currentSearch.PublisherFilter)
        //    };

        //    ViewBag.Title = "Save Search As ...";
        //    return PartialView(viewModel);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(LibraryUserSavedSearchViewModel viewModel)
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

        // GET: LibraryUserSavedSearches/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var savedSearch = _db.LibraryUserSavedSearches.Find(id);
            if (savedSearch == null)
            {
                return HttpNotFound();
            }
            var viewModel = new LibraryUserSavedSearchViewModel()
            {
                SavedSearchId = savedSearch.SavedSearchID,
                UserId = savedSearch.UserID,
                Description = savedSearch.Description
            };

            ViewBag.Title = "Edit " + _entityName;
            return PartialView(viewModel);
        }

        // POST: LibraryUserSavedSearches/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SavedSearchID,UserID,SearchUrl,Description")] LibraryUserSavedSearchViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var savedSearch = _db.LibraryUserSavedSearches.Find(viewModel.SavedSearchId);
                if (savedSearch == null)
                {
                    return HttpNotFound();
                }
                savedSearch.UserID = viewModel.UserId;
                savedSearch.Description = viewModel.Description;

                _db.Entry(savedSearch).State = EntityState.Modified;
                _db.SaveChanges();
                //return RedirectToAction("Index");
                return Json(new { success = true });
            }
            return PartialView(viewModel);
        }

        public ActionResult Run(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var savedSearch = _repository.GetById<LibraryUserSavedSearch>(id.Value);
            if (savedSearch == null)
            {
                return HttpNotFound();
            }

            switch (savedSearch.Scope)
            {
                case "opac":
                {
                    return RedirectToAction("RunOpacSavedSearch", new { id = savedSearch.SavedSearchID });
                }
                case "catalogue":
                {
                    return RedirectToAction("RunAdminSavedSearch", new { id = savedSearch.SavedSearchID });
                }
                case "finance":
                {
                    break;
                }
            }

            return null;
        }

        public ActionResult RunOpacSavedSearch(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var savedSearch = _repository.GetById<LibraryUserSavedSearch>(id.Value);
            if (savedSearch == null)
            {
                return HttpNotFound();
            }

            var viewModel = new SimpleSearchingViewModel()
            {
                SearchString = savedSearch.SearchString,
                SearchField = savedSearch.SearchField,
                AuthorFilter = new List<SelectAuthorEditorViewModel>(),
                ClassmarksFilter = new List<SelectClassmarkEditorViewModel>(),
                LanguageFilter = new List<SelectLanguageEditorViewModel>(),
                KeywordFilter = new List<SelectKeywordEditorViewModel>(),
                MediaFilter = new List<SelectMediaEditorViewModel>(),
                PublisherFilter = new List<SelectPublisherEditorViewModel>(),
            };

            TempData["KeywordFilter"] = null;
            TempData["ClassmarksFilter"] = null;
            TempData["MediaFilter"] = null;
            TempData["PublisherFilter"] = null;
            TempData["LanguageFilter"] = null;
            TempData["AuthorFilter"] = null;

            if (savedSearch.ClassmarksFilter != null)
            {
                foreach (var item in savedSearch.ClassmarksFilter.Split(','))
                {
                    var classmark = _db.Classmarks.Find(int.Parse(item));
                    var selectedItem = new SelectClassmarkEditorViewModel()
                    {
                        Id = classmark.ClassmarkID,
                        Selected = true,
                        TitleCount = 0,
                        Name = classmark.Classmark1
                    };
                    viewModel.ClassmarksFilter.Add(selectedItem);
                }
                TempData["ClassmarksFilter"] = viewModel.ClassmarksFilter;
            }

            if (savedSearch.MediaFilter != null)
            {
                foreach (var item in savedSearch.MediaFilter.Split(','))
                {
                    var media = _db.MediaTypes.Find(int.Parse(item));
                    var selectedItem = new SelectMediaEditorViewModel()
                    {
                        Id = media.MediaID,
                        Selected = true,
                        TitleCount = 0,
                        Name = media.Media
                    };
                    viewModel.MediaFilter.Add(selectedItem);
                }
                TempData["MediaFilter"] = viewModel.MediaFilter;
            }

            if (savedSearch.PublisherFilter != null)
            {
                foreach (var item in savedSearch.PublisherFilter.Split(','))
                {
                    var publisher = _db.Publishers.Find(int.Parse(item));
                    var selectedItem = new SelectPublisherEditorViewModel()
                    {
                        Id = publisher.PublisherID,
                        Selected = true,
                        TitleCount = 0,
                        Name = publisher.PublisherName
                    };
                    viewModel.PublisherFilter.Add(selectedItem);
                }
                TempData["PublisherFilter"] = viewModel.PublisherFilter;
            }

            if (savedSearch.LanguageFilter != null)
            {
                foreach (var item in savedSearch.LanguageFilter.Split(','))
                {
                    var language = _db.Languages.Find(int.Parse(item));
                    var selectedItem = new SelectLanguageEditorViewModel()
                    {
                        Id = language.LanguageID,
                        Selected = true,
                        TitleCount = 0,
                        Name = language.Language1
                    };
                    viewModel.LanguageFilter.Add(selectedItem);
                }
                TempData["LanguageFilter"] = viewModel.LanguageFilter;
            }

            if (savedSearch.AuthorFilter != null)
            {
                foreach (var item in savedSearch.AuthorFilter.Split(','))
                {
                    var author = _db.Authors.Find(int.Parse(item));
                    var selectedItem = new SelectAuthorEditorViewModel()
                    {
                        Id = author.AuthorID,
                        Selected = true,
                        TitleCount = 0,
                        Name = author.DisplayName
                    };
                    viewModel.AuthorFilter.Add(selectedItem);
                }
                TempData["AuthorFilter"] = viewModel.AuthorFilter;
            }

            if (savedSearch.KeywordFilter != null)
            {
                foreach (var item in savedSearch.KeywordFilter.Split(','))
                {
                    var keyword = _db.Keywords.Find(int.Parse(item));
                    var selectedItem = new SelectKeywordEditorViewModel()
                    {
                        Id = keyword.KeywordID,
                        Selected = true,
                        TitleCount = 0,
                        Name = keyword.KeywordTerm
                    };
                    viewModel.KeywordFilter.Add(selectedItem);
                }
                TempData["KeywordFilter"] = viewModel.KeywordFilter;
            }

            ViewData["SearchField"] = SelectListHelper.SearchFieldsList(scope: "opac");
            ViewBag.Title = Settings.GetParameterValue("Searching.SearchPageWelcome", "Search the Library");
            return RedirectToAction("SimpleSearchResults", "Home", viewModel);
        }

        public ActionResult RunAdminSavedSearch(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var savedSearch = _repository.GetById<LibraryUserSavedSearch>(id.Value);
            if (savedSearch == null)
            {
                return HttpNotFound();
            }

            var viewModel = new SimpleSearchingViewModel()
            {
                SearchString = savedSearch.SearchString,
                SearchField = savedSearch.SearchField,
                AuthorFilter = new List<SelectAuthorEditorViewModel>(),
                ClassmarksFilter = new List<SelectClassmarkEditorViewModel>(),
                LanguageFilter = new List<SelectLanguageEditorViewModel>(),
                KeywordFilter = new List<SelectKeywordEditorViewModel>(),
                MediaFilter = new List<SelectMediaEditorViewModel>(),
                PublisherFilter = new List<SelectPublisherEditorViewModel>(),
            };

            TempData["KeywordFilter"] = null;
            TempData["ClassmarksFilter"] = null;
            TempData["MediaFilter"] = null;
            TempData["PublisherFilter"] = null;
            TempData["LanguageFilter"] = null;
            TempData["AuthorFilter"] = null;

            if (savedSearch.ClassmarksFilter != null)
            {
                foreach (var item in savedSearch.ClassmarksFilter.Split(','))
                {
                    var classmark = _db.Classmarks.Find(int.Parse(item));
                    var selectedItem = new SelectClassmarkEditorViewModel()
                    {
                        Id = classmark.ClassmarkID,
                        Selected = true,
                        TitleCount = 0,
                        Name = classmark.Classmark1
                    };
                    viewModel.ClassmarksFilter.Add(selectedItem);
                }
                TempData["ClassmarksFilter"] = viewModel.ClassmarksFilter;
            }

            if (savedSearch.MediaFilter != null)
            {
                foreach (var item in savedSearch.MediaFilter.Split(','))
                {
                    var media = _db.MediaTypes.Find(int.Parse(item));
                    var selectedItem = new SelectMediaEditorViewModel()
                    {
                        Id = media.MediaID,
                        Selected = true,
                        TitleCount = 0,
                        Name = media.Media
                    };
                    viewModel.MediaFilter.Add(selectedItem);
                }
                TempData["MediaFilter"] = viewModel.MediaFilter;
            }

            if (savedSearch.PublisherFilter != null)
            {
                foreach (var item in savedSearch.PublisherFilter.Split(','))
                {
                    var publisher = _db.Publishers.Find(int.Parse(item));
                    var selectedItem = new SelectPublisherEditorViewModel()
                    {
                        Id = publisher.PublisherID,
                        Selected = true,
                        TitleCount = 0,
                        Name = publisher.PublisherName
                    };
                    viewModel.PublisherFilter.Add(selectedItem);
                }
                TempData["PublisherFilter"] = viewModel.PublisherFilter;
            }

            if (savedSearch.LanguageFilter != null)
            {
                foreach (var item in savedSearch.LanguageFilter.Split(','))
                {
                    var language = _db.Languages.Find(int.Parse(item));
                    var selectedItem = new SelectLanguageEditorViewModel()
                    {
                        Id = language.LanguageID,
                        Selected = true,
                        TitleCount = 0,
                        Name = language.Language1
                    };
                    viewModel.LanguageFilter.Add(selectedItem);
                }
                TempData["LanguageFilter"] = viewModel.LanguageFilter;
            }

            if (savedSearch.AuthorFilter != null)
            {
                foreach (var item in savedSearch.AuthorFilter.Split(','))
                {
                    var author = _db.Authors.Find(int.Parse(item));
                    var selectedItem = new SelectAuthorEditorViewModel()
                    {
                        Id = author.AuthorID,
                        Selected = true,
                        TitleCount = 0,
                        Name = author.DisplayName
                    };
                    viewModel.AuthorFilter.Add(selectedItem);
                }
                TempData["AuthorFilter"] = viewModel.AuthorFilter;
            }

            if (savedSearch.KeywordFilter != null)
            {
                foreach (var item in savedSearch.KeywordFilter.Split(','))
                {
                    var keyword = _db.Keywords.Find(int.Parse(item));
                    var selectedItem = new SelectKeywordEditorViewModel()
                    {
                        Id = keyword.KeywordID,
                        Selected = true,
                        TitleCount = 0,
                        Name = keyword.KeywordTerm
                    };
                    viewModel.KeywordFilter.Add(selectedItem);
                }
                TempData["KeywordFilter"] = viewModel.KeywordFilter;
            }

            ViewData["SearchField"] = SelectListHelper.SearchFieldsList(scope: "opac");
            ViewBag.Title = Settings.GetParameterValue("Searching.SearchPageWelcome", "Search the Library");
            return RedirectToAction("AdminSearchResults", "Searching", viewModel);
        }


        [HttpGet]
        public ActionResult Delete(int id = 0)
        {
            var savedSearch = _db.LibraryUserSavedSearches.Find(id);
            if (savedSearch == null)
            {
                return HttpNotFound();
            }

            // Create new instance of DeleteConfirmationViewModel and pass it to _DeleteConfirmation Dialog (Reusable)
            var deleteConfirmationViewModel = new DeleteConfirmationViewModel
            {
                DeleteEntityId = id,
                HeaderText = _entityName,
                PostDeleteAction = "Delete",
                PostDeleteController = "LibraryUserSavedSearches",
                DetailsText = savedSearch.Description
            };
            return PartialView("_DeleteConfirmation", deleteConfirmationViewModel);
        }

        [HttpPost]
        public ActionResult Delete(DeleteConfirmationViewModel dcvm)
        {
            // Get item which we need to delete from EntityFramework
            var savedSearch = _db.LibraryUserSavedSearches.Find(dcvm.DeleteEntityId);

            if (savedSearch == null)
            {
                return HttpNotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _db.LibraryUserSavedSearches.Remove(savedSearch);
                    _db.SaveChanges();
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
