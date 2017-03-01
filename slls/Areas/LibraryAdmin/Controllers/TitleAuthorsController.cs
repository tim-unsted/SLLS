using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using slls.DAO;
using slls.Models;
using slls.Utils.Helpers;
using slls.ViewModels;
using Westwind.Globalization;

namespace slls.Areas.LibraryAdmin
{
    public class TitleAuthorsController : CatalogueBaseController
    {
        private readonly DbEntities _db = new DbEntities();
        private readonly GenericRepository _repository;
        private readonly string _entityName = DbRes.T("Authors.Author", "FieldDisplayName");

        public TitleAuthorsController()
        {
            ViewBag.Title = DbRes.T("TitleAuthors", "EntityType");
            _repository = new GenericRepository(typeof(TitleAuthor));
        }

        // GET: LibraryAdmin/TitleAuthors
        public ActionResult List(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var titleAuthors = _db.TitleAuthors
                .Where(t => t.TitleId == id)
                .Include(t => t.Author)
                .Include(t => t.Title)
                .OrderBy(t => t.OrderSeq)
                .ThenBy(x => x.Author.DisplayName);

            if (titleAuthors.Any())
            {
                return View(titleAuthors.ToList());
            }
            return RedirectToAction("Edit", "Titles", new { id });
        }

        // GET: LibraryAdmin/TitleAuthors/Add  -- for adding a new author to a specified title
        public ActionResult Add(int id = 0)
        {
            //Get the Title of the item we're editing ...
            var title = from t in _db.Titles
                        where t.TitleID == id
                        select t.Title1;

            TitleAuthorAddViewModel tavm = new TitleAuthorAddViewModel
            {
                TitleId = id,
                Title = title.SingleOrDefault(),
                SelectedAuthors = null,
                AvailableAuthors = SelectListHelper.AuthorsList(addNew:false)
            };

            //tavm.OrderSeq = 1;
            //ViewData["AuthorID"] = SelectListHelper.SelectAuthorsList();
            ViewBag.Title = "Add " + DbRes.T("Authors.Author", "FieldDisplayName") + " to " + DbRes.T("Titles.Title", "FieldDisplayName");
            ViewBag.Msg = "Add " + DbRes.T("Authors.Author", "FieldDisplayName") + " to " + DbRes.T("Titles.Title", "FieldDisplayName");
            return PartialView(tavm);

        }

        // POST: LibraryAdmin/TitleAuthors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TitleAuthorAddViewModel tavm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (tavm.SelectedAuthors != null)
                    {
                        foreach (var authorId in tavm.SelectedAuthors)
                        {
                            if (authorId > 0)
                            {
                                //Check if the author has already been added to the title ...
                                bool exists = _db.TitleAuthors.Any(a => a.AuthorId == authorId && a.TitleId == tavm.TitleId);

                                //If not, proceed ...
                                if (exists == false)
                                {
                                    var ta = new TitleAuthor
                                    {
                                        TitleId = tavm.TitleId,
                                        AuthorId = authorId,
                                        InputDate = DateTime.Now
                                    };
                                    _repository.Insert(ta);
                                }
                            }
                        }
                        //return RedirectToAction("Edit", "Titles", new { id = tavm.TitleId });
                        return Json(new { success = true });
                    }
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", e.Message);
                }

            }

            return PartialView("Add", tavm);
        }


        // GET: LibraryAdmin/TitleAuthors/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var titleAuthor = _repository.GetById<TitleAuthor>(id.Value);
            if (titleAuthor == null)
            {
                return HttpNotFound();
            }

            var tavm = new TitleAuthorEditViewModel
            {
                TitleAuthorId = titleAuthor.TitleAuthorId,
                TitleId = titleAuthor.TitleId,
                AuthorId = titleAuthor.AuthorId,
                OrderSeq = titleAuthor.OrderSeq,
                Title = titleAuthor.Title.Title1
            };
            ViewData["AuthorID"] = SelectListHelper.AuthorsList(titleAuthor.AuthorId);
            return PartialView(tavm);
        }

        // POST: LibraryAdmin/TitleAuthors/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, TitleAuthorEditViewModel tavm)
        {
            if (ModelState.IsValid)
            {
                var titleid = tavm.TitleId;
                var titleAuthor = _repository.GetById<TitleAuthor>(tavm.TitleAuthorId);
                if (titleAuthor == null)
                {
                    return HttpNotFound();
                }
                titleAuthor.AuthorId = tavm.AuthorId;
                titleAuthor.TitleId = tavm.TitleId;
                titleAuthor.OrderSeq = tavm.OrderSeq;
                titleAuthor.LastModified = DateTime.Now;
                _repository.Update(titleAuthor);
                return Json(new { success = true });
                //return RedirectToAction("Edit", "Titles", new { id = titleid });
            }
            ViewData["AuthorID"] = SelectListHelper.AuthorsList(tavm.AuthorId);
            return PartialView(tavm);
        }

        [HttpGet]
        public ActionResult Delete(int id = 0)
        {
            var titleAuthor = _db.TitleAuthors.Find(id);
            if (titleAuthor == null)
            {
                return HttpNotFound();
            }
            // Create new instance of DeleteConfirmationViewModel and pass it to _DeleteConfirmation Dialog (Reusable)
            DeleteConfirmationViewModel deleteConfirmationViewModel = new DeleteConfirmationViewModel
            {
                DeleteEntityId = id,
                HeaderText = DbRes.T("Authors.Author", "FieldDisplayName") + " from " + DbRes.T("Titles.Title", "FieldDisplayName"),
                PostDeleteAction = "Delete",
                PostDeleteController = "TitleAuthors",
                FunctionText = "Remove",
                ButtonText = "Remove",
                ConfirmationHeaderText = "You are about to remove the following",
                DetailsText = titleAuthor.Author.DisplayName
            };
            return PartialView("_DeleteConfirmation", deleteConfirmationViewModel);
        }

        [HttpPost]
        public ActionResult Delete(DeleteConfirmationViewModel dcvm)
        {
            // Get item which we need to delete from EntityFramework 
            var item = _db.TitleAuthors.Find(dcvm.DeleteEntityId);

            if (item == null)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _db.TitleAuthors.Remove(item);
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