using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using slls.DAO;
using slls.Models;
using slls.Utils.Helpers;
using slls.ViewModels;
using Westwind.Globalization;

namespace slls.Areas.LibraryAdmin
{
    public class MediaTypesController : AdminBaseController
    {
        private readonly DbEntities _db = new DbEntities();
        private readonly GenericRepository _repository;
        private readonly string _entityName = DbRes.T("MediaTypes.Media_Type", "FieldDisplayName");

        public MediaTypesController()
        {
            _repository = new GenericRepository(typeof(MediaType));
            ViewBag.Title = DbRes.T("MediaTypes", "EntityType");
        }

        // GET: MediaTypes
        public ActionResult Index()
        {
            //return View(db.MediaTypes.ToList());
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("authlistsSeeAlso", ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["controller"].ToString());
            return View(CacheProvider.GetAll<MediaType>("mediatypes").Where(m => m.Deleted == false).OrderBy((m => m.Media)));
        }

        
        // GET: MediaTypes/Create
        public ActionResult Create()
        {
            ViewBag.Title = "Add New " + _entityName;
            ViewData["LoanTypeID"] = SelectListHelper.LoanTypes(addNew: false, id: Utils.PublicFunctions.GetDefaultValue("MediaTypes", "LoanTypeID"));
            var viewModel = new MediaTypesAddViewModel();
            return PartialView(viewModel);
        }

        // POST: MediaTypes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
            [Bind(Include = "Media,LoanTypeID")] MediaTypesAddViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var mediaType = new MediaType
                {
                    Media = viewModel.Media,
                    LoanTypeID = viewModel.LoanTypeID <= 0 ? Utils.PublicFunctions.GetDefaultValue("MediaTypes", "LoanTypeID") : viewModel.LoanTypeID,
                    CanUpdate = true,
                    CanDelete = true,
                    InputDate = DateTime.Now
                };
                _repository.Insert(mediaType);
                CacheProvider.RemoveCache("mediatypes");
                return Json(new { success = true });
                //return RedirectToAction("Index");
            }

            return PartialView(viewModel);
        }

        public ActionResult _add()
        {
            return PartialView();
        }

        // POST: MediaTypes/_add
        [HttpPost]
        public JsonResult _add(MediaType newMedia)
        {
            if (!ModelState.IsValid) return null;
            var allMediaTypes = CacheProvider.GetAll<MediaType>("mediatypes");
            var newMediaType = allMediaTypes.FirstOrDefault(m => m.Media == newMedia.Media);

            if (newMediaType != null)
                return Json(new
                {
                    success = false,
                    errMsg = "This Media Type already exists!"
                });
            newMediaType = new MediaType
            {
                Media = newMedia.Media,
                LoanTypeID = Utils.PublicFunctions.GetDefaultValue("MediaTypes", "LoanTypeID"),
                CanUpdate = true,
                CanDelete = true,
                InputDate = DateTime.Now
            };
            _repository.Insert(newMediaType);
            CacheProvider.RemoveCache("mediatypes");

            return Json(new
            {
                success = true,
                newData = newMediaType
            });
        }

        // GET: MediaTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var mediaType = _repository.GetById<MediaType>(id.Value);
            if (mediaType == null)
            {
                return HttpNotFound();
            }
            if (mediaType.Deleted)
            {
                return HttpNotFound();
            }

            ViewBag.Title = "Edit " + _entityName;
            var viewModel = new MediaTypesEditViewModel
            {
                MediaID = mediaType.MediaID,
                LoanTypeID = mediaType.LoanTypeID,
                Media = mediaType.Media
            };

            ViewData["LoanTypeID"] = SelectListHelper.LoanTypes(addNew: false, id: viewModel.LoanTypeID);
            return PartialView(viewModel);
        }

        // POST: MediaTypes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MediaID,Media,LoanTypeID")] MediaTypesEditViewModel viewModel)
        {
            if (!ModelState.IsValid) return PartialView(viewModel);
            var mediaType = _repository.GetById<MediaType>(viewModel.MediaID);
            if (mediaType == null)
            {
                return HttpNotFound();
            }
            if (mediaType.Deleted)
            {
                return HttpNotFound();
            }
            //mediaType.MediaID = viewModel.MediaID;
            mediaType.Media = viewModel.Media;
            mediaType.LoanTypeID = viewModel.LoanTypeID <= 0 ? Utils.PublicFunctions.GetDefaultValue("MediaTypes", "LoanTypeID") : viewModel.LoanTypeID;
            mediaType.LastModified = DateTime.Now;
            _repository.Update(mediaType);
            CacheProvider.RemoveCache("mediatypes");
            return Json(new { success = true });
            //return RedirectToAction("Index");
        }

        public static int GetMediaId(string media)
        {
            media = media.Trim();
            var db = new DbEntities();
            var allMediaTypes = CacheProvider.GetAll<MediaType>("mediatypes");
            var model = allMediaTypes.FirstOrDefault(x => String.Equals(x.Media, media, StringComparison.OrdinalIgnoreCase));
            if (model != null) return model.MediaID;
            //insert new media type now ...
            var newMedia = new MediaType
            {
                Media = media,
                LoanTypeID = Utils.PublicFunctions.GetDefaultValue("MediaTypes", "LoanTypeID"),
                CanUpdate = true,
                CanDelete = true,
                InputDate = DateTime.Now
            };
            db.MediaTypes.Add(newMedia);
            db.SaveChanges();
            CacheProvider.RemoveCache("mediatypes");
            return newMedia.MediaID;
        }

        
        [HttpGet]
        public ActionResult Delete(int id = 0)
        {
            var mediaType = _repository.GetById<MediaType>(id);
            if (mediaType == null)
            {
                return HttpNotFound();
            }
            if (mediaType.Deleted)
            {
                return HttpNotFound();
            }
            if (mediaType.CanDelete == false)
            {
                return RedirectToAction("Index");
            }
            // Create new instance of DeleteConfirmationViewModel and pass it to _DeleteConfirmation Dialog (Reusable)
            DeleteConfirmationViewModel deleteConfirmationViewModel = new DeleteConfirmationViewModel
            {
                DeleteEntityId = id,
                HeaderText = _entityName,
                PostDeleteAction = "Delete",
                PostDeleteController = "MediaTypes",
                DetailsText = mediaType.Media
            };
            return PartialView("_DeleteConfirmation", deleteConfirmationViewModel);
        }

        [HttpPost]
        public ActionResult Delete(DeleteConfirmationViewModel dcvm)
        {
            // Get item which we need to delete from EntityFramework
            var media = _repository.GetById<MediaType>(dcvm.DeleteEntityId);

            if (media == null)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _repository.Delete(media);
                    CacheProvider.RemoveCache("mediatypes");
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