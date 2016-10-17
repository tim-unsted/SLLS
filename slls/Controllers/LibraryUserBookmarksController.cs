using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using slls.DAO;
using slls.Models;
using slls.Utils;
using slls.Utils.Helpers;
using slls.ViewModels;
using Westwind.Globalization;

namespace slls.Controllers
{
    //Only allow logged in users to access this functionality ...
    [AuthorizeRoles(Roles.Administrator, Roles.Staff, Roles.BsAdmin, Roles.User)]
    public class LibraryUserBookmarksController : sllsBaseController
    {
        private readonly DbEntities _db = new DbEntities();
        private readonly GenericRepository _repository;
        private readonly string _entityName = DbRes.T("Bookmarks.Bookmark", "FieldDisplayName");

        public LibraryUserBookmarksController()
        {
            _repository = new GenericRepository(typeof(LibraryUserBookmark));
            ViewBag.Title = DbRes.T("Bookmarks", "EntityType");
        }

        // GET: LibraryUserBookmarks
        public ActionResult Index()
        {
            var userId = Utils.PublicFunctions.GetUserId(); //User.Identity.GetUserId();
            var libraryUserBookmarks = _db.LibraryUserBookmarks.Where(b => b.UserID == userId);

            if (!libraryUserBookmarks.Any())
            {
                //ViewBag.AlertMsg = "You currently have no items bookmarked!";
                TempData["NoData"] = "You currently have no items bookmarked!";
            }

            ViewBag.Title = "My " + ViewBag.Title;
            return View(libraryUserBookmarks.ToList());
        }
        
        [Authorize]
        public ActionResult Add(int id = 0)
        {
            var title = _db.Titles.Find(id);

            var viewModel = new LibraryUserBookmarkViewModel()
            {
                TitleId = id,
                UserId = PublicFunctions.GetUserId(), //User.Identity.GetUserId(),
                Title = title.Title1,
                Description = title.Title1
            };

            ViewBag.Title = "Add Bookmark";
            return PartialView(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(LibraryUserBookmarkViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var newBookmark = new LibraryUserBookmark()
                    {
                        TitleID = viewModel.TitleId,
                        UserID = viewModel.UserId,
                        Description = viewModel.Description,
                        InputDate = DateTime.Now
                    };
                    _db.LibraryUserBookmarks.Add(newBookmark);
                    _db.SaveChanges();
                    return Json(new {success = true});
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", e.Message);
                }
            }
            return PartialView(viewModel);
        }
        
        // GET: LibraryUserBookmarks/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var bookmark = _db.LibraryUserBookmarks.Find(id);
            if (bookmark == null)
            {
                return HttpNotFound();
            }
            var viewModel = new LibraryUserBookmarkViewModel()
            {
                BookmarkId = bookmark.BookmarkID,
                TitleId = bookmark.TitleID,
                Title = bookmark.Title.Title1,
                UserId = bookmark.UserID,
                Description = bookmark.Description
            };
            ViewBag.Title = "Edit Bookmark";
            return PartialView(viewModel);
        }

        // POST: LibraryUserBookmarks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BookmarkID,TitleID,UserID,Description")] LibraryUserBookmarkViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var bookmark = _db.LibraryUserBookmarks.Find(viewModel.BookmarkId);
                if (bookmark == null)
                {
                    return HttpNotFound();
                }

                //bookmark.BookmarkID = viewModel.BookmarkId;
                bookmark.TitleID = viewModel.TitleId;
                bookmark.Description = viewModel.Description;
                bookmark.UserID = viewModel.UserId;

                _db.Entry(bookmark).State = EntityState.Modified;
                _db.SaveChanges();
                //return RedirectToAction("Index");
                return Json(new { success = true });
            }
            
            return PartialView(viewModel);
        }

        [HttpGet]
        public ActionResult Delete(int id = 0)
        {
            var bookmark = _repository.GetById<LibraryUserBookmark>(id);
            if (bookmark == null)
            {
                return HttpNotFound();
            }
           
            // Create new instance of DeleteConfirmationViewModel and pass it to _DeleteConfirmation Dialog (Reusable)
            var deleteConfirmationViewModel = new DeleteConfirmationViewModel
            {
                DeleteEntityId = id,
                HeaderText = _entityName,
                PostDeleteAction = "Delete",
                PostDeleteController = "LibraryUserBookmarks",
                DetailsText = bookmark.Description
            };
            return PartialView("_DeleteConfirmation", deleteConfirmationViewModel);
        }

        [HttpPost]
        public ActionResult Delete(DeleteConfirmationViewModel dcvm)
        {
            // Get item which we need to delete from EntityFramework
            var item = _db.LibraryUserBookmarks.Find(dcvm.DeleteEntityId);

            if (item == null)
            {
                return HttpNotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _db.LibraryUserBookmarks.Remove(item);
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
