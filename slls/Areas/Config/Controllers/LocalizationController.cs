using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Resources;
using System.Web;
using System.Web.Mvc;
using slls.DAO;
using slls.Models;
using slls.ViewModels;
using Westwind.Globalization;

namespace slls.Areas.Config
{
    public class LocalizationController : ConfigBaseController
    {
        private readonly DbEntities _db = new DbEntities();
        private readonly string _entityName = DbRes.T("Localization.Resource", "FieldDisplayName");
        private readonly GenericRepository _repository;

        public LocalizationController()
        {
            _repository = new GenericRepository(typeof(Models.Localization));
            ViewBag.Title = DbRes.T("Localization.Vocabulary", "EntityType");
        }

        // GET: Config/Localizations
        public ActionResult Index(string entityType = "")
        {
            var allResources = _db.Localizations.OrderBy(x => x.ResourceId);
            var entities = new List<string>();
            foreach (var row in allResources)
            {
               var resource = row.ResourceId.Split('.');
                entities.Add(resource[0]);
            }

            var viewmodel = new LocalizationIndexViewModel();

            if (!string.IsNullOrEmpty(entityType))
            {
                viewmodel.localizations = _db.Localizations.Where(x => x.ResourceId.StartsWith(entityType + ".") || x.ResourceId == entityType);
            }
            else
            {
                viewmodel.localizations = _db.Localizations;
            }
            ViewBag.Message = "Resources";
            ViewBag.Phrase = ViewBag.Title.Replace(" & ", " and ").ToLower();
            ViewData["Entities"] = new SelectList(entities.Distinct(), "");
            return View(viewmodel);
        }
        
        // GET: Config/Localizations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var item = _repository.GetById<Models.Localization>(id.Value);
            if (item == null)
            {
                return HttpNotFound();
            }
            ViewBag.Title = "Edit " + ViewBag.Title;
            var viewModel = new LocalizationEditViewModel(item);
            return PartialView(viewModel);
        }

        // POST: Config/Localizations/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, LocalizationEditViewModel localization)
        {
            if (ModelState.IsValid)
            {
                var item = _repository.GetById<Models.Localization>(id);
                item.Value = localization.Value;
                item.LocaleId = localization.LocaleId;
                item.Updated = DateTime.Now;

                _repository.Update(item);
                DbResourceConfiguration.ClearResourceCache();
                DbRes.ClearResources();
                
                return Json(new { success = true });
                //return RedirectToAction("Index");
            }
            return PartialView(localization);
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
