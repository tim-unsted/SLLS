using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using slls.DAO;
using slls.Models;
using slls.Utils.Helpers;
using slls.ViewModels;

namespace slls.Areas.LibraryAdmin
{
    public class TitleAdditionalFieldDefsController : CatalogueBaseController
    {
        private readonly DbEntities _db = new DbEntities();
        private readonly GenericRepository _repository;

        public TitleAdditionalFieldDefsController()
        {
            _repository = new GenericRepository(typeof(TitleAdditionalFieldDef));
        }

        // GET: LibraryAdmin/TitleAdditionalFieldDefs
        public ActionResult Index()
        {
            ViewBag.Title = "Custom Title Field Definitions";
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("authlistsSeeAlso", ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["controller"].ToString());
            return View(_db.TitleAdditionalFieldDefs.ToList());
        }

        public ActionResult LongTextLabels()
        {
            ViewBag.Title = "Long Text Labels";
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("authlistsSeeAlso", ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["controller"].ToString());
            return View(_db.TitleAdditionalFieldDefs.Where(x => x.IsLongText).ToList());
        }

        public ActionResult CustomFields()
        {
            ViewBag.Title = "Custom Fields";
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("authlistsSeeAlso", ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["controller"].ToString());
            return View(_db.TitleAdditionalFieldDefs.Where(x => x.IsLongText == false).ToList());
        }
        

        // GET: LibraryAdmin/TitleAdditionalFieldDefs/AddCustomField
        public ActionResult AddCustomField()
        {
            var model = new CustomFieldAdd();
            ViewBag.Title = "Add New Custom Field";
            return PartialView(model);
        }

        // POST: LibraryAdmin/TitleAdditionalFieldDefs/AddCustomField
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddCustomField([Bind(Include = "FieldName,IsDate,IsNumeric,IsBoolean,IsLongText,ShowOnOPAC")] CustomFieldAdd viewModel)
        {
            var table = new TitleAdditionalFieldDef
            {
                FieldName = viewModel.FieldName,
                IsDate = viewModel.IsDate,
                IsBoolean = viewModel.IsBoolean,
                IsLongText = false,
                ShowOnOPAC = viewModel.ShowOnOpac,
                InputDate = DateTime.Now
            };
            
            if (ModelState.IsValid)
            {
                _db.TitleAdditionalFieldDefs.Add(table);
                _db.SaveChanges();
                return Json(new { success = true });
                //return RedirectToAction("CustomFields");
            }

            ViewBag.Title = "Add New Custom Field";
            return PartialView(viewModel);
        }

        
        // GET: LibraryAdmin/TitleAdditionalFieldDefs/AddLongTextLabel
        public ActionResult AddLongTextLabel()
        {
            var viewModel = new LongTextLabelAdd();
            ViewBag.Title = "Add New Long Text Label";
            return PartialView(viewModel);
        }

        // POST: LibraryAdmin/TitleAdditionalFieldDefs/AddLongTextLabel
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddLongTextLabel([Bind(Include = "TextLabel,IsLongText,ShowOnOPAC")] LongTextLabelAdd viewModel)
        {
            var newLabel = new TitleAdditionalFieldDef
            {
                FieldName = viewModel.TextLabel,
                IsLongText = true,
                ShowOnOPAC = viewModel.ShowOnOpac,
                InputDate = DateTime.Now
            };

            if (ModelState.IsValid)
            {
                _db.TitleAdditionalFieldDefs.Add(newLabel);
                _db.SaveChanges();
                //return RedirectToAction("LongTextLabels");
                return Json(new { success = true });
            }

            ViewBag.Title = "Add New Long Text Label";
            return PartialView(viewModel);
        }


        // GET: LibraryAdmin/TitleAdditionalFieldDefs/EditCustomField/5
        public ActionResult EditCustomField(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var titleAdditionalFieldDef = _db.TitleAdditionalFieldDefs.Find(id);
            if (titleAdditionalFieldDef == null)
            {
                return HttpNotFound();
            }
            if (titleAdditionalFieldDef.Deleted)
            {
                return HttpNotFound();
            }

            var viewModel = new TitleAdditionalFieldDefEdit
            {
                FieldId = titleAdditionalFieldDef.FieldID,
                FieldName = titleAdditionalFieldDef.FieldName,
                IsDate = titleAdditionalFieldDef.IsDate,
                IsBoolean = titleAdditionalFieldDef.IsBoolean,
                IsNumeric = titleAdditionalFieldDef.IsNumeric,
                IsLongText = titleAdditionalFieldDef.IsLongText,
                ShowOnOpac = titleAdditionalFieldDef.ShowOnOPAC
            };

            ViewBag.Title = "Edit Custom Field Definition";
            return PartialView(viewModel);
        }

        // POST: LibraryAdmin/TitleAdditionalFieldDefs/EditCustomField/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCustomField([Bind(Include = "FieldID,FieldName,IsDate,IsNumeric,IsBoolean,IsLongText,ShowOnOPAC")] TitleAdditionalFieldDefEdit viewModel)
        {
            if (ModelState.IsValid)
            {
                var titleAdditionalFieldDef = _db.TitleAdditionalFieldDefs.Find(viewModel.FieldId);
                if (titleAdditionalFieldDef == null)
                {
                    return HttpNotFound();
                }
                if (titleAdditionalFieldDef.Deleted)
                {
                    return HttpNotFound();
                }

                titleAdditionalFieldDef.FieldID = viewModel.FieldId;
                titleAdditionalFieldDef.FieldName = viewModel.FieldName;
                titleAdditionalFieldDef.IsDate = viewModel.IsDate;
                titleAdditionalFieldDef.IsNumeric = viewModel.IsNumeric;
                titleAdditionalFieldDef.IsBoolean = viewModel.IsBoolean;
                titleAdditionalFieldDef.IsLongText = false;
                titleAdditionalFieldDef.ShowOnOPAC = viewModel.ShowOnOpac;
                titleAdditionalFieldDef.LastModified = DateTime.Now;

                _db.Entry(titleAdditionalFieldDef).State = EntityState.Modified;
                _db.SaveChanges();
                return Json(new { success = true });
                //return RedirectToAction("CustomFields");
            }

            ViewBag.Title = "Edit Custom Field Definition";
            return PartialView(viewModel);
        }
        

        // GET: LibraryAdmin/TitleAdditionalFieldDefs/EditLongTextLabel/5
        public ActionResult EditLongTextLabel(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            TitleAdditionalFieldDef titleAdditionalFieldDef = _db.TitleAdditionalFieldDefs.Find(id);
            if (titleAdditionalFieldDef == null)
            {
                return HttpNotFound();
            }

            var viewModel = new TitleAdditionalFieldDefEdit
            {
                FieldId = titleAdditionalFieldDef.FieldID,
                FieldName = titleAdditionalFieldDef.FieldName,
                IsLongText = true,
                ShowOnOpac = titleAdditionalFieldDef.ShowOnOPAC
            };

            ViewBag.Title = "Edit Long Text Label";
            return PartialView(viewModel);
        }

        // POST: LibraryAdmin/TitleAdditionalFieldDefs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditLongTextLabel([Bind(Include = "FieldID,FieldName,IsDate,IsNumeric,IsBoolean,IsLongText,ShowOnOPAC")] TitleAdditionalFieldDefEdit viewModel)
        {
            if (ModelState.IsValid)
            {
                TitleAdditionalFieldDef table = new TitleAdditionalFieldDef
                {
                    FieldID = viewModel.FieldId,
                    FieldName = viewModel.FieldName,
                    IsLongText = true,
                    ShowOnOPAC = viewModel.ShowOnOpac,
                    LastModified = DateTime.Now
                };

                _db.Entry(table).State = EntityState.Modified;
                _db.SaveChanges();
                return Json(new { success = true });
                //return RedirectToAction("LongTextLabels");
            }

            ViewBag.Title = "Edit Long Text Label";
            return PartialView(viewModel);
        }


        public static int GetLongTextLabelId(string textLabel)
        {
            var db = new DbEntities();
            var model = db.TitleAdditionalFieldDefs.FirstOrDefault(x => x.FieldName == textLabel);
            if (model != null) return model.FieldID;
            //insert new label now ...
            var newLabel = new TitleAdditionalFieldDef
            {
                FieldName = textLabel,
                IsLongText = true,
                ShowOnOPAC = true,
                InputDate = DateTime.Now
            };
            db.TitleAdditionalFieldDefs.Add(newLabel);
            db.SaveChanges();
            return newLabel.FieldID;
        }
        

        [HttpGet]
        public ActionResult Delete(int id = 0)
        {
            var fieldDef = _db.TitleAdditionalFieldDefs.Find(id);
            if (fieldDef == null)
            {
                return HttpNotFound();
            }
            // Create new instance of DeleteConfirmationViewModel and pass it to _DeleteConfirmation Dialog (Reusable)
            DeleteConfirmationViewModel deleteConfirmationViewModel = new DeleteConfirmationViewModel
            {
                DeleteEntityId = id,
                //HeaderText = DbRes.T("Links.Link", "FieldDisplayName") + " from " + DbRes.T("Titles.Title", "FieldDisplayName"),
                HeaderText = "Custom Title Field",
                PostDeleteAction = "Delete",
                PostDeleteController = "TitleAdditionalFieldDefs",
                FunctionText = "Delete",
                ButtonText = "Delete",
                ConfirmationHeaderText = "You are about to delete the following",
                DetailsText = fieldDef.FieldName
            };
            return PartialView("_DeleteConfirmation", deleteConfirmationViewModel);
        }

        [HttpPost]
        public ActionResult Delete(DeleteConfirmationViewModel dcvm)
        {
            // Get item which we need to delete from EntityFramework 
            var fieldDef = _db.TitleAdditionalFieldDefs.Find(dcvm.DeleteEntityId);

            if (fieldDef == null)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _db.TitleAdditionalFieldDefs.Remove(fieldDef);
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
