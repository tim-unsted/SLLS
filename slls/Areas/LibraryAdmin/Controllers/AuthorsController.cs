using System;
using System.Collections.Generic;
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
    public class AuthorsController : AdminBaseController
    {
        private readonly DbEntities _db = new DbEntities();
        private readonly string _entityName = DbRes.T("Authors.Author", "FieldDisplayName");
        private readonly GenericRepository _repository;

        public AuthorsController()
        {
            ViewBag.Title = DbRes.T("Authors", "EntityType");
            _repository = new GenericRepository(typeof(Author));
        }

        // Create a simple disctionary that we can used to fill a dropdown list in views
        public Dictionary<string, string> GetAuthType()
        {
            return new Dictionary<string, string>
            {
                {"C", "Corporate"},
                {"P", "Personal"}
            };
        }

        // GET: Authors
        public ActionResult Index()
        {
            var authors = CacheProvider.GetAll<Author>("authors")
                .Select(x => new AuthorIndexViewModel { AuthorID = x.AuthorID, DisplayName = x.DisplayName, AuthType = x.AuthType.Contains("P") ? "Personal" : "Corporate", TitleAuthors = x.TitleAuthors, TitleEditors = x.TitleEditors});
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("authlistsSeeAlso", ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["controller"].ToString());
            return View(authors.ToList());
        }
        

        // GET: Authors/Create
        public ActionResult Create()
        {
            ViewBag.Title = "Add New " + _entityName;
            ViewBag.AuthTypes = GetAuthType();
            var viewModel = new AuthorCreateViewModel();
            return PartialView(viewModel);
        }

        // POST: Authors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
            [Bind(
                Include =
                    "Title,DisplayName,Firstnames,Lastnames,AuthType,Notes")] AuthorCreateViewModel viewModel)
        {
            var author = new Author
            {
                DisplayName = viewModel.DisplayName,
                Title = viewModel.Title,
                Firstnames = viewModel.Firstnames,
                Lastnames = viewModel.Lastnames,
                AuthType = viewModel.AuthType,
                Notes = viewModel.Notes,
                InputDate = DateTime.Now
            };
            
            if (ModelState.IsValid)
            {
                _repository.Insert(author);
                CacheProvider.RemoveCache("authors");
                return Json(new { success = true });
                //return RedirectToAction("Index");
            }

            return PartialView(viewModel);
        }

        public ActionResult _add()
        {
            return PartialView();
        }

        // POST: Authors/_add
        [HttpPost]
        public JsonResult _add(Author author)
        {
            if (ModelState.IsValid)
            {
                var newAuthor = _db.Authors.FirstOrDefault(x => x.DisplayName == author.DisplayName);

                if (newAuthor == null)
                {
                    newAuthor = new Author
                    {
                        DisplayName = author.DisplayName,
                        InputDate = DateTime.Now
                    };

                    _db.Authors.Add(newAuthor);
                    _db.SaveChanges();
                    CacheProvider.RemoveCache("authors");

                    return Json(new
                    {
                        success = true,
                        newData = newAuthor
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        errMsg = "This Author already exists!"
                    });
                }
            }
            return null;
        }

        // GET: Authors/Edit/5
        public ActionResult Edit(int? id, int titleid = 0)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var author = _repository.GetById<Author>(id.Value);
            if (author == null)
            {
                return HttpNotFound();
            }
            if (author.Deleted)
            {
                return HttpNotFound();
            }
            ViewBag.AuthTypes = GetAuthType();
            ViewBag.Title = "Edit " + _entityName;
            ViewBag.titleid = titleid;
            return PartialView(author);
        }

        // POST: Authors/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
            [Bind(
                Include =
                    "AuthorID,Title,DisplayName,Firstnames,Lastnames,AuthType,Notes")] Author author)
        {
            if (ModelState.IsValid)
            {
                author.LastModified = DateTime.Now;
                _repository.Update(author);
                CacheProvider.RemoveCache("authors");
                return Json(new { success = true });
            }
            return View(author);
        }


        public static int GetAuthorId(string author)
        {
            var db = new DbEntities();
            var model = CacheProvider.GetAll<Author>("authors").FirstOrDefault(a => a.DisplayName == author);
            if (model != null) return model.AuthorID;
            //insert new author now ...
            var newAuthor = new Author
            {
                DisplayName = author,
                AuthType = "P",
                InputDate = DateTime.Now
            };
            db.Authors.Add(newAuthor);
            db.SaveChanges();
            return newAuthor.AuthorID;
        }
        
        [HttpGet]
        public ActionResult Delete(int id = 0)
        {
            var author = _repository.GetById<Author>(id);
            if (author == null)
            {
                return HttpNotFound();
            }
            if (author.Deleted)
            {
                return HttpNotFound();
            }
            // Create new instance of DeleteConfirmationViewModel and pass it to _DeleteConfirmation Dialog (Reusable)
            DeleteConfirmationViewModel deleteConfirmationViewModel = new DeleteConfirmationViewModel
            {
                DeleteEntityId = id,
                HeaderText = "Delete " + _entityName + "?",
                PostDeleteAction = "Delete",
                PostDeleteController = "Authors",
                DetailsText = author.DisplayName
            };
            return PartialView("_DeleteConfirmation", deleteConfirmationViewModel);
        }

        [HttpPost]
        public ActionResult Delete(DeleteConfirmationViewModel dcvm)
        {
            // Get item which we need to delete from EntityFramework 
            var item = _db.Authors.Find(dcvm.DeleteEntityId);

            if (item == null)
            {
                return HttpNotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _db.Authors.Remove(item);
                    _db.SaveChanges();
                    CacheProvider.RemoveCache("authors");
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