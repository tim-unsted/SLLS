using System;
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
    public class PublishersController : AdminBaseController
    {
        private DbEntities _db = new DbEntities();
        private readonly string _entityName = DbRes.T("Publishers.Publisher", "FieldDisplayName");
        private readonly GenericRepository _repository;

        public PublishersController()
        {
            ViewBag.Title = DbRes.T("Publishers", "EntityType");
            _repository = new GenericRepository(typeof(Publisher));
        }

        // GET: LibraryAdmin/Publishers
        public ActionResult Index()
        {
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("authlistsSeeAlso", ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["controller"].ToString());
            return View(CacheProvider.GetAll<Publisher>("publishers").Where(p => p.Deleted == false).OrderBy(p => p.PublisherName));
        }

        
        // GET: LibraryAdmin/Publishers/Create
        public ActionResult Create()
        {
            ViewBag.Title = "Add New " + _entityName;
            var viewModel = new PublisherAddViewModel();
            return PartialView(viewModel);
        }

        // POST: LibraryAdmin/Publishers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PublisherName")] PublisherAddViewModel pavm)
        {
            if (ModelState.IsValid)
            {
                var publisher = new Publisher
                {
                    PublisherName = pavm.PublisherName,
                    CanUpdate = true,
                    CanDelete = true,
                    InputDate = DateTime.Now
                };
                _repository.Insert(publisher);
                CacheProvider.RemoveCache("publishers");
                return Json(new { success = true });
                //return RedirectToAction("Index");
            }

            return PartialView(pavm);
        }

        public ActionResult _add()
        {
            return PartialView();
        }

        // POST: Publishers/_add
        [HttpPost]
        public JsonResult _add(Publisher publisher)
        {
            //if (!string.IsNullOrEmpty(publisher))
            if (ModelState.IsValid)
            {
                var allPublishers = CacheProvider.GetAll<Publisher>("publishers");
                var newPublisher = allPublishers.FirstOrDefault(x => x.PublisherName == publisher.PublisherName);

                if (newPublisher == null)
                {
                    newPublisher = new Publisher
                    {
                        PublisherName = publisher.PublisherName,
                        CanUpdate = true,
                        CanDelete = true,
                        InputDate = DateTime.Now
                    };

                    _db.Publishers.Add(newPublisher);
                    _db.SaveChanges();
                    CacheProvider.RemoveCache("publishers");

                    return Json(new
                    {
                        success = true,
                        newData = newPublisher
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        errMsg = "This Publisher already exists!"
                    });
                }
            }
            return null;
        }

        // GET: LibraryAdmin/Publishers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var publisher = _repository.GetById<Publisher>(id.Value);
            if (publisher == null)
            {
                return HttpNotFound();
            }
            if (publisher.Deleted)
            {
                return HttpNotFound();
            }

            var viewModel = new PublisherEditViewModel
            {
                PublisherID = publisher.PublisherID,
                PublisherName = publisher.PublisherName,
                CanDelete = publisher.CanDelete,
                CanUpdate = publisher.CanUpdate
            };
            ViewBag.Title = "Edit " + _entityName;
            return PartialView(viewModel);
        }

        // POST: LibraryAdmin/Publishers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PublisherID,PublisherName,CanUpdate,CanDelete")] PublisherEditViewModel viewModel)
        {
            if (!ModelState.IsValid) return PartialView(viewModel);
            var publisher = _repository.GetById<Publisher>(viewModel.PublisherID);
            if (publisher == null)
            {
                return HttpNotFound();
            }
            if (publisher.Deleted)
            {
                return HttpNotFound();
            }
            //publisher.PublisherID = viewModel.PublisherID;
            publisher.PublisherName = viewModel.PublisherName;
            publisher.LastModified = DateTime.Now;
            _repository.Update(publisher);
            CacheProvider.RemoveCache("publishers");
            return Json(new { success = true });
            //return RedirectToAction("Index");
        }


        public static int GetPublisherId(string publisher)
        {
            publisher = publisher.Trim();
            var db = new DbEntities();
            CacheProvider.RemoveCache("publishers");
            var allPublishers = CacheProvider.GetAll<Publisher>("publishers").ToList();
            var model = allPublishers.FirstOrDefault(p => String.Equals(p.PublisherName, publisher, StringComparison.OrdinalIgnoreCase));
            if (model != null) return model.PublisherID;
            //insert new publisher now ...
            var newPublisher = new Publisher
            {
                PublisherName = publisher,
                CanUpdate = true,
                CanDelete = true,
                InputDate = DateTime.Now
            };

            db.Publishers.Add(newPublisher);
            db.SaveChanges();
            CacheProvider.RemoveCache("publishers");
            return newPublisher.PublisherID;
        }


        // GET: LibraryAdmin/Publishers/Delete/5
        public ActionResult Delete(int? id)
        {
            var publisher = _repository.GetById<Publisher>(id.Value);
            if (publisher == null)
            {
                return HttpNotFound();
            }
            if (publisher.Deleted)
            {
                return HttpNotFound();
            }
            if (publisher.CanDelete == false)
            {
                return RedirectToAction("Index");
            }
            // Create new instance of DeleteConfirmationViewModel and pass it to _DeleteConfirmation Dialog (Reusable)
            DeleteConfirmationViewModel deleteConfirmationViewModel = new DeleteConfirmationViewModel
            {
                DeleteEntityId = id.Value,
                HeaderText = "Publisher",
                PostDeleteAction = "Delete",
                PostDeleteController = "Publishers",
                DetailsText = publisher.PublisherName
            };
            return PartialView("_DeleteConfirmation", deleteConfirmationViewModel);
        }

        [HttpPost]
        public ActionResult Delete(DeleteConfirmationViewModel dcvm)
        {
            // Get item which we need to delete from EntityFramework 
            var item = _db.Publishers.Find(dcvm.DeleteEntityId);

            if (item == null)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _db.Publishers.Remove(item);
                    _db.SaveChanges();
                    CacheProvider.RemoveCache("publishers");
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
