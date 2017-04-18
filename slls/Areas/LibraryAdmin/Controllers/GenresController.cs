using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using slls.DAO;
using slls.Models;
using slls.Utils.Helpers;
using slls.ViewModels;
using Westwind.Globalization;

namespace slls.Areas.LibraryAdmin
{
    public class GenresController : Controller
    {
        private readonly DbEntities _db = new DbEntities();

        // GET: LibraryAdmin/Genres
        public ActionResult Index()
        {
            var genres = _db.Genres.Where(x => x.GenreId != 1);
            ViewBag.Title = DbRes.T("Genres", "EntityType");
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("authlistsSeeAlso", ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["controller"].ToString());
            return View(genres.ToList());
        }

        // GET: LibraryAdmin/Genres/Create
        public ActionResult Create()
        {
            ViewBag.Title = "Add New " + DbRes.T("Genres.Genre", "FieldDisplayName");
            var viewModel = new GenresAddViewModel();
            return PartialView(viewModel);
        }

        // POST: LibraryAdmin/Genres/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Genre")] GenresAddViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var genre = new Genre()
                {
                    Genre1 = viewModel.Genre,
                    CanUpdate = true,
                    CanDelete = true,
                    InputDate = DateTime.Today
                };
                _db.Genres.Add(genre);
                _db.SaveChanges();
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }

        public ActionResult _add()
        {
            return PartialView();
        }

        // POST: Genres/_add
        [HttpPost]
        public JsonResult _add(Genre genre)
        {
            if (ModelState.IsValid)
            {
                var newGenre = _db.Genres.FirstOrDefault(x => x.Genre1 == genre.Genre1);

                if (newGenre == null)
                {
                    newGenre = new Genre
                    {
                        Genre1 = genre.Genre1,
                        CanUpdate = true,
                        CanDelete = true,
                        InputDate = DateTime.Now
                    };

                    _db.Genres.Add(newGenre);
                    _db.SaveChanges();
                    CacheProvider.RemoveCache("Genres");

                    return Json(new
                    {
                        success = true,
                        newData = newGenre
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        errMsg = "This " + DbRes.T("Genres.Genre", "FieldDisplayName") + " already exists!"
                    });
                }
            }
            return null;
        }

        // GET: LibraryAdmin/Genres/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var genre = _db.Genres.Find(id);
            if (genre == null)
            {
                return HttpNotFound();
            }
            var viewModel = new GenresEditViewModel()
            {
                Genre = genre.Genre1,
                GenreId = genre.GenreId
            };
            ViewBag.Title = "Edit " + DbRes.T("Genres.Genre", "FieldDisplayName");
            return PartialView(viewModel);
        }

        // POST: LibraryAdmin/Genres/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "GenreId,Genre")] GenresEditViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var genre = _db.Genres.Find(viewModel.GenreId);
                if (genre == null)
                {
                    return HttpNotFound();
                }
                genre.Genre1 = viewModel.Genre;
                _db.Entry(genre).State = EntityState.Modified;
                _db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpGet]
        public ActionResult Delete(int id = 0)
        {
            var genre = _db.Genres.Find(id);
            if (genre == null)
            {
                return HttpNotFound();
            }
            if (genre.Deleted)
            {
                return HttpNotFound();
            }
            if (genre.CanDelete == false)
            {
                return RedirectToAction("Index");
            }
            // Create new instance of DeleteConfirmationViewModel and pass it to _DeleteConfirmation Dialog (Reusable)
            DeleteConfirmationViewModel dcvm = new DeleteConfirmationViewModel
            {
                DeleteEntityId = id,
                HeaderText = DbRes.T("Genres.Genre", "FieldDisplayName"),
                PostDeleteAction = "Delete",
                PostDeleteController = "Genres",
                DetailsText = genre.Genre1
            };
            return PartialView("_DeleteConfirmation", dcvm);
        }

        [HttpPost]
        public ActionResult Delete(DeleteConfirmationViewModel dcvm)
        {
            // Get item which we need to delete from EntityFramework 
            var genre = _db.Genres.Find(dcvm.DeleteEntityId);

            if (genre == null)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _db.Genres.Remove(genre);
                    _db.SaveChanges();
                    CacheProvider.RemoveCache("Genres");
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
            }
            base.Dispose(disposing);
        }
    }
}
