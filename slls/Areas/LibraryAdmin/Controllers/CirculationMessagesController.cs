using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using slls.Models;
using slls.Utils.Helpers;
using slls.ViewModels;
using Westwind.Globalization;

namespace slls.Areas.LibraryAdmin
{
    public class CirculationMessagesController : SerialsBaseController
    {
        private readonly DbEntities _db = new DbEntities();

        // GET: LibraryAdmin/CirculationMessages
        public ActionResult Index()
        {
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("circulationSeeAlso",
                ControllerContext.RouteData.Values["action"].ToString(), null, "sortOrder");
            ViewBag.Title = DbRes.T("Circulation.Circulation_Slip_Message", "FieldDisplayName") + "s";
            return View(_db.CirculationMessages.ToList());
        }

        // GET: LibraryAdmin/CirculationMessages/Create
        public ActionResult Create()
        {
            ViewBag.Title = "Add New " + DbRes.T("Circulation.Circulation_Slip_Message", "FieldDisplayName");
            return PartialView();
        }

        // POST: LibraryAdmin/CirculationMessages/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CirculationMsg")] CirculationMessage circulationMessage)
        {
            circulationMessage.CanDelete = true;
            circulationMessage.CanUpdate = true;
            circulationMessage.InputDate = DateTime.Now;
            if (ModelState.IsValid)
            {
                _db.CirculationMessages.Add(circulationMessage);
                _db.SaveChanges();
                //return RedirectToAction("Index");
                return Json(new { success = true });
            }

            ViewBag.Title = "Add New " + DbRes.T("Circulation.Circulation_Slip_Message", "FieldDisplayName");
            return PartialView(circulationMessage);
        }

        // GET: LibraryAdmin/CirculationMessages/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CirculationMessage circulationMessage = _db.CirculationMessages.Find(id);
            if (circulationMessage == null)
            {
                return HttpNotFound();
            }
            ViewBag.Title = "Edit " + DbRes.T("Circulation.Circulation_Slip_Message", "FieldDisplayName");
            return PartialView(circulationMessage);
        }

        // POST: LibraryAdmin/CirculationMessages/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CirculationMsgID,CirculationMsg")] CirculationMessage circulationMessage)
        {
            circulationMessage.LastModified = DateTime.Now;
            if (ModelState.IsValid)
            {
                _db.Entry(circulationMessage).State = EntityState.Modified;
                _db.SaveChanges();
                //return RedirectToAction("Index");
                return Json(new { success = true });
            }
            ViewBag.Title = "Edit " + DbRes.T("Circulation.Circulation_Slip_Message", "FieldDisplayName");
            return PartialView(circulationMessage);
        }

        [HttpGet]
        public ActionResult Delete(int id = 0)
        {
            var msg = _db.CirculationMessages.Find(id);
            if (msg == null)
            {
                return HttpNotFound();
            }
            if (msg.Deleted)
            {
                return HttpNotFound();
            }
            
            // Create new instance of DeleteConfirmationViewModel and pass it to _DeleteConfirmation Dialog (Reusable)
            DeleteConfirmationViewModel deleteConfirmationViewModel = new DeleteConfirmationViewModel
            {
                DeleteEntityId = id,
                HeaderText = DbRes.T("Circulation.Circulation_Slip_Message", "FieldDisplayName"),
                PostDeleteAction = "Delete",
                PostDeleteController = "CirculationMessages",
                DetailsText = msg.CirculationMsg
            };
            return PartialView("_DeleteConfirmation", deleteConfirmationViewModel);
        }

        [HttpPost]
        public ActionResult Delete(DeleteConfirmationViewModel dcvm)
        {
            // Get item which we need to delete from EntityFramework 
            var msg = _db.CirculationMessages.Find(dcvm.DeleteEntityId);

            if (msg == null)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _db.CirculationMessages.Remove(msg);
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
            }
            base.Dispose(disposing);
        }
    }
}
