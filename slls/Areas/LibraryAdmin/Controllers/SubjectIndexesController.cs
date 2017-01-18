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
    public class SubjectIndexesController : CatalogueBaseController
    {
        private readonly DbEntities _db = new DbEntities();
        private readonly GenericRepository _repository;

        public SubjectIndexesController()
        {
            ViewBag.Title = DbRes.T("SubjectIndex", "EntityType");
            _repository = new GenericRepository(typeof(SubjectIndex));
        }

        // GET: LibraryAdmin/SubjectIndexes
        public ActionResult Index()
        {
            var subjectIndexes = _db.SubjectIndexes.Include(s => s.Keyword).Include(s => s.Title);
            return View(subjectIndexes.ToList());
        }


        // GET: LibraryAdmin/SubjectIndexes/Add - when passed a Title ID
        public ActionResult Add(int id = 0)
        {
            var viewModel = new SubjectIndexAddViewModel
            {
                TitleId = id,
                Title = (from t in _db.Titles.Where(t => t.TitleID == id)
                    select t.Title1).FirstOrDefault()
            };

            var count = _db.Keywords.Count();
            if (count > 1000)
            {
                viewModel.LargeData = true;
            }
            else
            {
                viewModel.LargeData = false;
                viewModel.AvailableKeywords = _db.Keywords.Where(k => k.ParentKeywordID != null).Take(100)
                .Select(x => new SelectListItem
                {
                    Value = x.KeywordID.ToString(),
                    Text = x.KeywordTerm
                }).OrderBy(item => item.Text)
                .ToList();
            }
            
            ViewBag.Msg = "Add " + DbRes.T("Keywords.Keyword", "FieldDisplayName") + " to " + DbRes.T("Titles.Title", "FieldDisplayName");
            ViewBag.Title = "Add " + DbRes.T("Keywords.Keyword", "FieldDisplayName") + " to " + DbRes.T("Titles.Title", "FieldDisplayName");
            return PartialView(viewModel);
        }

        // POST: LibraryAdmin/SubjectIndexes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(SubjectIndexAddViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (viewModel.KeywordId == 0 && viewModel.Keyword.Length > 0)
                    {
                        viewModel.KeywordId = KeywordsController.GetKeywordId(viewModel.Keyword.Trim());
                    }
                    if (viewModel.KeywordId != 0)
                    {
                        //Check if the keyword has already been added to the title ...
                        bool exists = _db.SubjectIndexes.Any(s => s.KeywordID == viewModel.KeywordId && s.TitleID == viewModel.TitleId);

                        //If not, proceed ...
                        if (exists == false)
                        {
                            SubjectIndex si = new SubjectIndex
                            {
                                TitleID = viewModel.TitleId,
                                KeywordID = viewModel.KeywordId,
                                InputDate = DateTime.Now
                            };
                            _repository.Insert(si);
                        }
                    }

                    return Json(new { success = true });
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", e.Message);
                }
            }

            return Json(new { success = false });
        }

        // GET: LibraryAdmin/SubjectIndexes/Create
        public ActionResult Create(int id = 0)
        {
            ViewData["KeywordID"] = SelectListHelper.KeywordList();
            ViewData["TitleID"] = SelectListHelper.TitlesList(id);
            ViewBag.Title = "Add " + DbRes.T("Keywords.Keyword", "FieldDisplayName") + " to " + DbRes.T("Titles.Title", "FieldDisplayName");
            return View();
        }

        // POST: LibraryAdmin/SubjectIndexes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SubjectIndexID,TitleID,KeywordID,InputDate")] SubjectIndex subjectIndex)
        {
            if (ModelState.IsValid)
            {
                _db.SubjectIndexes.Add(subjectIndex);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewData["KeywordID"] = SelectListHelper.KeywordList(subjectIndex.KeywordID);
            ViewData["TitleID"] = SelectListHelper.TitlesList(subjectIndex.TitleID);
            return View(subjectIndex);
        }

        //// GET: LibraryAdmin/SubjectIndexes/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    SubjectIndex subjectIndex = _db.SubjectIndexes.Find(id);
        //    if (subjectIndex == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewData["KeywordID"] = new SelectList(_db.Keywords, "KeywordID", "KeywordTerm", subjectIndex.KeywordID);
        //    ViewData["TitleID"] = new SelectList(_db.Titles, "TitleID", "Title1", subjectIndex.TitleID);
        //    return View(subjectIndex);
        //}

        // POST: LibraryAdmin/SubjectIndexes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "SubjectIndexID,TitleID,KeywordID,InputDate")] SubjectIndex subjectIndex)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _db.Entry(subjectIndex).State = EntityState.Modified;
        //        _db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewData["KeywordID"] = new SelectList(_db.Keywords, "KeywordID", "KeywordTerm", subjectIndex.KeywordID);
        //    ViewData["TitleID"] = new SelectList(_db.Titles, "TitleID", "Title1", subjectIndex.TitleID);
        //    return View(subjectIndex);
        //}

        // GET: LibraryAdmin/SubjectIndexes/Delete/5
        [HttpGet]
        public ActionResult Delete(int id = 0)
        {
            var subjectIndex = _db.SubjectIndexes.Find(id);
            if (subjectIndex == null)
            {
                return HttpNotFound();
            }
            // Create new instance of DeleteConfirmationViewModel and pass it to _DeleteConfirmation Dialog (Reusable)
            DeleteConfirmationViewModel deleteConfirmationViewModel = new DeleteConfirmationViewModel
            {
                DeleteEntityId = id,
                HeaderText = DbRes.T("Keywords.Keyword", "FieldDisplayName") + " from " + DbRes.T("Titles.Title", "FieldDisplayName"),
                PostDeleteAction = "Delete",
                PostDeleteController = "SubjectIndexes",
                FunctionText = "Remove",
                ButtonText = "Remove",
                ConfirmationText = "Are you sure you want to remove the following",
                DetailsText = subjectIndex.Keyword.KeywordTerm
            };
            return PartialView("_DeleteConfirmation", deleteConfirmationViewModel);
        }

        [HttpPost]
        public ActionResult Delete(DeleteConfirmationViewModel dcvm)
        {
            // Get item which we need to delete from EntityFramework 
            var item = _db.SubjectIndexes.Find(dcvm.DeleteEntityId);

            if (item == null)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _db.SubjectIndexes.Remove(item);
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
