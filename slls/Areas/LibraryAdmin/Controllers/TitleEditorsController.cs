using System;
using System.Data.Entity;
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
    public class TitleEditorsController : CatalogueBaseController
    {
        private readonly DbEntities _db = new DbEntities();
        private readonly GenericRepository _repository;
        private readonly string _entityName = DbRes.T("Authors.Editor", "FieldDisplayName");

        public TitleEditorsController()
        {
            ViewBag.Title = DbRes.T("TitleEditors", "EntityType");
            _repository = new GenericRepository(typeof(TitleEditor));
        }

        // GET: LibraryAdmin/TitleEditors
        public ActionResult List(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var titleEditors = _db.TitleEditors
                .Where(t => t.TitleID == id)
                .Include(t => t.Author)
                .Include(t => t.Title)
                .OrderBy(t => t.OrderSeq)
                .ThenBy(x => x.Author.DisplayName);

            if (titleEditors.Any())
            {
                return View(titleEditors.ToList());
            }
            return RedirectToAction("Edit", "Titles", new { id });
        }


        // GET: LibraryAdmin/TitleEditors/Add  -- for adding a new editor to a specified title
        public ActionResult Add(int id = 0)
        {
            //Get the Title of the item we're editing ...
            var title = from t in _db.Titles
                        where t.TitleID == id
                        select t.Title1;

            var tevm = new TitleEditorAddViewModel
            {
                TitleId = id,
                Title = title.SingleOrDefault(),
                SelectedEditors = null,
                AvailableEditors = SelectListHelper.EditorsList()
            };

            ViewBag.Title = "Add " + DbRes.T("Authors.Editor", "FieldDisplayName") + " to " + DbRes.T("Titles.Title", "FieldDisplayName");
            ViewBag.Msg = "Add " + DbRes.T("Authors.Editor", "FieldDisplayName") + " to " + DbRes.T("Titles.Title", "FieldDisplayName");
            return PartialView(tevm);

        }

        // POST: LibraryAdmin/TitleEditors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TitleEditorAddViewModel tevm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (tevm.SelectedEditors != null)
                    {
                        foreach (var editorId in tevm.SelectedEditors)
                        {
                            if (editorId > 0)
                            {
                                //Check if the editor has already been added to the title ...
                                bool exists = _db.TitleEditors.Any(a => a.AuthorID == editorId && a.TitleID == tevm.TitleId);

                                //If not, proceed ...
                                if (exists == false)
                                {
                                    var ta = new TitleEditor
                                    {
                                        TitleID = tevm.TitleId,
                                        AuthorID = editorId,
                                        InputDate = DateTime.Now
                                    };
                                    _repository.Insert(ta);
                                }
                            }
                        }
                        //return RedirectToAction("Edit", "Titles", new { id = tevm.TitleId });
                        return Json(new { success = true });
                    }
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", e.Message);
                }

            }

            return PartialView("Add",tevm);
        }


        // GET: LibraryAdmin/TitleEditors/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var titleEditor = _repository.GetById<TitleEditor>(id.Value);
            if (titleEditor == null)
            {
                return HttpNotFound();
            }

            TitleEditorEditViewModel tevm = new TitleEditorEditViewModel();
            tevm.TitleEditorId = titleEditor.TitleEditorID;
            tevm.TitleId = titleEditor.TitleID;
            tevm.AuthorId = titleEditor.AuthorID;
            tevm.OrderSeq = titleEditor.OrderSeq;
            tevm.Title = titleEditor.Title.Title1;
            ViewData["AuthorID"] = SelectListHelper.EditorsList(titleEditor.AuthorID);
            return PartialView(tevm);
        }

        // POST: LibraryAdmin/TitleEditors/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, TitleEditorEditViewModel tevm)
        {
            if (ModelState.IsValid)
            {
                var titleid = tevm.TitleId;
                var titleEditor = _repository.GetById<TitleEditor>(tevm.TitleEditorId);
                if (titleEditor == null)
                {
                    return HttpNotFound();
                }
                titleEditor.AuthorID = tevm.AuthorId;
                titleEditor.TitleID = tevm.TitleId;
                titleEditor.OrderSeq = tevm.OrderSeq;
                titleEditor.LastModified = DateTime.Now;
                _repository.Update(titleEditor);
                return Json(new { success = true });
                //return RedirectToAction("Edit", "Titles", new { id = titleid });
            }
            ViewData["AuthorID"] = SelectListHelper.EditorsList(tevm.AuthorId);
            return PartialView(tevm);
        }

        [HttpGet]
        public ActionResult Delete(int id = 0)
        {
            var titleEditor = _db.TitleEditors.Find(id);
            if (titleEditor == null)
            {
                return HttpNotFound();
            }
            // Create new instance of DeleteConfirmationViewModel and pass it to _DeleteConfirmation Dialog (Reusable)
            DeleteConfirmationViewModel deleteConfirmationViewModel = new DeleteConfirmationViewModel
            {
                DeleteEntityId = id,
                HeaderText = DbRes.T("Authors.Editor", "FieldDisplayName") + " from " + DbRes.T("Titles.Title", "FieldDisplayName"),
                PostDeleteAction = "Delete",
                PostDeleteController = "TitleEditors",
                FunctionText = "Remove",
                ButtonText = "Remove",
                ConfirmationText = "Are you sure you want to remove the following",
                DetailsText = titleEditor.Author.DisplayName
            };
            return PartialView("_DeleteConfirmation", deleteConfirmationViewModel);
        }

        [HttpPost]
        public ActionResult Delete(DeleteConfirmationViewModel dcvm)
        {
            // Get item which we need to delete from EntityFramework 
            var item = _db.TitleEditors.Find(dcvm.DeleteEntityId);

            if (item == null)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _db.TitleEditors.Remove(item);
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