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
    public class AudiencesController : Controller
    {
        private readonly DbEntities _db = new DbEntities();

        // GET: LibraryAdmin/Audiences
        public ActionResult Index()
        {
            var audiences = _db.Audiences.Where(x => x.AudienceId != 1);
            ViewBag.Title = DbRes.T("Audiences", "EntityType");
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("authlistsSeeAlso", ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["controller"].ToString());
            return View(audiences.ToList());
        }
        
        // GET: LibraryAdmin/Audiences/Create
        public ActionResult Create()
        {
            ViewBag.Title = "Add New " + DbRes.T("Audiences.Audience", "FieldDisplayName");
            var viewModel = new AudienceAddViewModel();
            return PartialView(viewModel);
        }

        // POST: LibraryAdmin/Audiences/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Audience")] AudienceAddViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var audience = new Audience()
                {
                    Audience1 = viewModel.Audience,
                    CanUpdate = true,
                    CanDelete = true,
                    InputDate = DateTime.Today
                };
                _db.Audiences.Add(audience);
                _db.SaveChanges();
                return Json(new {success = true});
            }

            return Json(new { success = false });
        }

        public ActionResult _add()
        {
            return PartialView();
        }

        // POST: Audiences/_add
        [HttpPost]
        public JsonResult _add(Audience audience)
        {
            if (ModelState.IsValid)
            {
                var newAudience = _db.Audiences.FirstOrDefault(x => x.Audience1 == audience.Audience1);

                if (newAudience == null)
                {
                    newAudience = new Audience
                    {
                        Audience1 = audience.Audience1,
                        CanUpdate = true,
                        CanDelete = true,
                        InputDate = DateTime.Now
                    };

                    _db.Audiences.Add(newAudience);
                    _db.SaveChanges();
                    CacheProvider.RemoveCache("Audiences");

                    return Json(new
                    {
                        success = true,
                        newData = newAudience
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        errMsg = "This " + DbRes.T("Audiences.Audience", "FieldDisplayName") + " already exists!"
                    });
                }
            }
            return null;
        }

        // GET: LibraryAdmin/Audiences/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var audience = _db.Audiences.Find(id);
            if (audience == null)
            {
                return HttpNotFound();
            }
            var viewModel = new AudienceEditViewModel()
            {
                Audience = audience.Audience1,
                AudienceId = audience.AudienceId
            };
            ViewBag.Title = "Edit " + DbRes.T("Audiences.Audience", "FieldDisplayName");
            return PartialView(viewModel);
        }

        // POST: LibraryAdmin/Audiences/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AudienceId,Audience")] AudienceEditViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var audience = _db.Audiences.Find(viewModel.AudienceId);
                if (audience == null)
                {
                    return HttpNotFound();
                }
                audience.Audience1 = viewModel.Audience;
                _db.Entry(audience).State = EntityState.Modified;
                _db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpGet]
        public ActionResult Delete(int id = 0)
        {
            var audience = _db.Audiences.Find(id);
            if (audience == null)
            {
                return HttpNotFound();
            }
            if (audience.Deleted)
            {
                return HttpNotFound();
            }
            if (audience.CanDelete == false)
            {
                return RedirectToAction("Index");
            }
            // Create new instance of DeleteConfirmationViewModel and pass it to _DeleteConfirmation Dialog (Reusable)
            DeleteConfirmationViewModel dcvm = new DeleteConfirmationViewModel
            {
                DeleteEntityId = id,
                HeaderText = DbRes.T("Audiences.Audience", "FieldDisplayName"),
                PostDeleteAction = "Delete",
                PostDeleteController = "Audiences",
                DetailsText = audience.Audience1
            };
            return PartialView("_DeleteConfirmation", dcvm);
        }

        [HttpPost]
        public ActionResult Delete(DeleteConfirmationViewModel dcvm)
        {
            // Get item which we need to delete from EntityFramework 
            var audience = _db.Audiences.Find(dcvm.DeleteEntityId);

            if (audience == null)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _db.Audiences.Remove(audience);
                    _db.SaveChanges();
                    CacheProvider.RemoveCache("audiences");
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
