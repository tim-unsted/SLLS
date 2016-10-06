using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;
using slls.DAO;
using slls.Models;
using slls.Utils.Helpers;
using slls.ViewModels;
using Westwind.Globalization;

namespace slls.Areas.LibraryAdmin
{
    public class UsefulLinksController : Controller
    {
        private readonly DbEntities _db = new DbEntities();
        private readonly string _entityName = DbRes.T("Useful_Links.Useful_Link", "FieldDisplayName");
        private readonly GenericRepository _repository;

        public UsefulLinksController()
        {
            ViewBag.Title = DbRes.T("Useful_Links", "EntityType");
            _repository = new GenericRepository(typeof(UsefulLink));
        }

        // Create a set of simple dictionaries that we can used to fill dropdown lists in views ...
        public Dictionary<string, string> GetTargets()
        {
            return new Dictionary<string, string>
            {
                {"_self", "Opens in current page/tab"},
                {"_blank", "Opens in new page/tab"}
            };
        }

        // GET: LibraryAdmin/UsefulLinks
        public ActionResult Index()
        {
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("opacAdminSeeAlso", ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["controller"].ToString());
            return View(_db.UsefulLinks.ToList());
        }
        
        // GET: LibraryAdmin/UsefulLinks/Create
        public ActionResult Create()
        {
            var viewModel = new UsefulLinksAddEditViewModel()
            {
                Target = "_blank"
            };
            ViewBag.Targets = GetTargets();
            ViewBag.Title = "Add New Useful Link";
            return PartialView(viewModel);
        }

        // POST: LibraryAdmin/UsefulLinks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LinkAddress,DisplayText,ToolTip,Target,Enabled")] UsefulLinksAddEditViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var newUsefulLink = new UsefulLink()
                {
                    LinkAddress = viewModel.LinkAddress,
                    DisplayText = viewModel.DisplayText,
                    ToolTip = viewModel.ToolTip,
                    Target = viewModel.Target,
                    Enabled = true,
                    InputDate = DateTime.Now
                };
                _db.UsefulLinks.Add(newUsefulLink);
                _db.SaveChanges();
                return Json(new { success = true });
                //return RedirectToAction("Index");
            }

            return PartialView(viewModel);
        }

        // GET: LibraryAdmin/UsefulLinks/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var usefulLink = _db.UsefulLinks.Find(id);
            if (usefulLink == null)
            {
                return HttpNotFound();
            }

            var viewModel = new UsefulLinksAddEditViewModel()
            {
                LinkID = usefulLink.LinkID,
                LinkAddress = usefulLink.LinkAddress,
                DisplayText = usefulLink.DisplayText,
                ToolTip = usefulLink.ToolTip,
                Target = usefulLink.Target,
                Enabled = usefulLink.Enabled
            };

            ViewBag.Targets = GetTargets();
            ViewBag.Title = "Edit Useful Link";
            return PartialView(viewModel);
        }

        // POST: LibraryAdmin/UsefulLinks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "LinkID,LinkAddress,DisplayText,ToolTip,Target,Enabled")] UsefulLinksAddEditViewModel viewModel)
        {
            if (!ModelState.IsValid) return View(viewModel);
            var usefulLink = _db.UsefulLinks.Find(viewModel.LinkID);
            if (usefulLink == null)
            {
                return HttpNotFound();
            }
            usefulLink.LinkAddress = viewModel.LinkAddress;
            usefulLink.DisplayText = viewModel.DisplayText;
            usefulLink.ToolTip = viewModel.ToolTip;
            usefulLink.Target = viewModel.Target;
            usefulLink.Enabled = viewModel.Enabled;
            usefulLink.LastModified = DateTime.Now;

            _db.Entry(usefulLink).State = EntityState.Modified;
            _db.SaveChanges();
            //return RedirectToAction("Index");
            return Json(new { success = true });
        }

        [HttpGet]
        public ActionResult Delete(int id = 0)
        {
            var usefulLink = _repository.GetById<UsefulLink>(id);
            if (usefulLink == null)
            {
                return HttpNotFound();
            }

            if (usefulLink.Deleted)
            {
                return HttpNotFound();
            }
            
            // Create new instance of DeleteConfirmationViewModel and pass it to _DeleteConfirmation Dialog (Reusable)
            var deleteConfirmationViewModel = new DeleteConfirmationViewModel
            {
                DeleteEntityId = id,
                HeaderText = _entityName,
                PostDeleteAction = "Delete",
                PostDeleteController = "UsefulLinks",
                DetailsText = usefulLink.DisplayText
            };
            return PartialView("_DeleteConfirmation", deleteConfirmationViewModel);
        }

        [HttpPost]
        public ActionResult Delete(DeleteConfirmationViewModel dcvm)
        {
            // Get item which we need to delete from EntityFramework
            var item = _repository.GetById<UsefulLink>(dcvm.DeleteEntityId);

            if (item == null)
            {
                return HttpNotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //_db.UsefulLinks.Remove(item);
                    //_db.SaveChanges();
                    _repository.Delete(item);
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
