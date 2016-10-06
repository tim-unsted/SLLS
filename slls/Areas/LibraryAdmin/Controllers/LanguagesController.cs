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
    public class LanguagesController : Controller
    {
        private readonly DbEntities _db = new DbEntities();
        private readonly string _entityName = DbRes.T("Languages.Language", "FieldDisplayName");
        private readonly GenericRepository _repository;

        public LanguagesController()
        {
            ViewBag.Title = DbRes.T("Languages", "EntityType");
            _repository = new GenericRepository(typeof(Language));
        }
        
        // GET: LibraryAdmin/Languages
        public ActionResult Index()
        {
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("authlistsSeeAlso", ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["controller"].ToString());
            return View(_repository.GetAll<Language>().Where(l => l.Deleted == false).OrderBy(l => l.Language1));
        }

        
        // GET: LibraryAdmin/Languages/Create
        public ActionResult Create()
        {
            var lavm = new LanguagesAddViewModel();
            ViewBag.Title = "Add new " + _entityName;
            return PartialView(lavm);
        }

        // POST: LibraryAdmin/Languages/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Language")] LanguagesAddViewModel lavm)
        {
            if (ModelState.IsValid)
            {
                var language = new Language
                {
                    Language1 = lavm.Language,
                    CanUpdate = true,
                    CanDelete = true,
                    InputDate = DateTime.Now
                };
               _repository.Insert(language);
                CacheProvider.RemoveCache("languages");
                return Json(new { success = true });
                //return RedirectToAction("Index");
            }

            ViewBag.Title = "Add new " + _entityName;
            return PartialView(lavm);
        }


        public ActionResult _add()
        {
            return PartialView();
        }

        // POST: Languages/_add
        [HttpPost]
        public JsonResult _add(Language language)
        {
            if (ModelState.IsValid)
            {
                var newLanguage = CacheProvider.GetAll<Language>("languages").FirstOrDefault(x => x.Language1 == language.Language1);

                if (newLanguage == null)
                {
                    newLanguage = new Language
                    {
                        Language1 = language.Language1,
                        CanUpdate = true,
                        CanDelete = true,
                        InputDate = DateTime.Now
                    };

                    _db.Languages.Add(newLanguage);
                    _db.SaveChanges();
                    CacheProvider.RemoveCache("languages");

                    return Json(new
                    {
                        success = true,
                        newData = newLanguage
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        errMsg = "This " + _entityName + " already exists!"
                    });
                }
            }
            return null;
        }


        // GET: LibraryAdmin/Languages/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var language = _repository.GetById<Language>(id.Value);
            if (language == null)
            {
                return HttpNotFound();
            }
            if (language.Deleted)
            {
                return HttpNotFound();
            }

            var levm = new LanguagesEditViewModel
            {
                LanguageID = language.LanguageID,
                Language = language.Language1
            };
            ViewBag.Title = "Edit " + _entityName;
            return PartialView(levm);
        }

        // POST: LibraryAdmin/Languages/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "LanguageID,Language")] LanguagesEditViewModel levm)
        {
            if (ModelState.IsValid)
            {
                var language = _repository.GetById<Language>(levm.LanguageID);
                if (language == null)
                {
                    return HttpNotFound();
                }
                if (language.Deleted)
                {
                    return HttpNotFound();
                }
                //language.LanguageID = levm.LanguageID;
                language.Language1 = levm.Language;
                language.LastModified = DateTime.Now;
                _repository.Update(language);
                CacheProvider.RemoveCache("languages");
                return Json(new { success = true });
                //return RedirectToAction("Index");
            }
            ViewBag.Title = "Edit " + _entityName;
            return PartialView(levm);
        }

        public static int GetLanguageId(string language)
        {
            language = language.Trim();
            var db = new DbEntities();
            var allLanguages = CacheProvider.GetAll<Language>("languages");
            var model = allLanguages.FirstOrDefault(x => String.Equals(x.Language1, language, StringComparison.OrdinalIgnoreCase));
            if (model != null) return model.LanguageID;
            //insert new language now ...
            var newLanguage = new Language
            {
                Language1 = language,
                CanUpdate = true,
                CanDelete = true,
                InputDate = DateTime.Now
            };
            db.Languages.Add(newLanguage);
            db.SaveChanges();
            CacheProvider.RemoveCache("languages");
            return newLanguage.LanguageID;
        }

        // GET: LibraryAdmin/Languages/Delete/5
        [HttpGet]
        public ActionResult Delete(int id = 0)
        {
            var language = _repository.GetById<Language>(id);
            if (language == null)
            {
                return HttpNotFound();
            }
            if (language.Deleted)
            {
                return HttpNotFound();
            }
            if (language.CanDelete == false)
            {
                return RedirectToAction("Index");
            }

            // Create new instance of DeleteConfirmationViewModel and pass it to _DeleteConfirmation Dialog (Reusable)
            DeleteConfirmationViewModel deleteConfirmationViewModel = new DeleteConfirmationViewModel
            {
                DeleteEntityId = id,
                HeaderText = _entityName,
                PostDeleteAction = "Delete",
                PostDeleteController = "Languages",
                DetailsText = language.Language1
            };
            return PartialView("_DeleteConfirmation", deleteConfirmationViewModel);
        }

        [HttpPost]
        public ActionResult Delete(DeleteConfirmationViewModel dcvm)
        {
            // Get item which we need to delete from EntityFramework 
            var item = _db.Languages.Find(dcvm.DeleteEntityId);

            if (item == null)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _db.Languages.Remove(item);
                    _db.SaveChanges();
                    CacheProvider.RemoveCache("languages");
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
