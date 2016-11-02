using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
    public class KeywordsController : AdminBaseController
    {
        private readonly DbEntities _db = new DbEntities();
        private readonly GenericRepository _repository;
        private readonly string _entityName = DbRes.T("Keywords.Keyword", "FieldDisplayName");

        public KeywordsController()
        {
            _repository = new GenericRepository(typeof(Keyword));
            ViewBag.Title = DbRes.T("Keywords", "EntityType");
        }

        // GET: Keywords
        public ActionResult Index(string selectedLetter)
        {
            var allKeywords = CacheProvider.GetAll<Keyword>("keywords")
                .Where(k => k.KeywordID > 0).ToList();

            var count = allKeywords.Count();

            if (selectedLetter == null)
            {
                selectedLetter = count < 100 ? "All" : "a";
            }

            var model = new KeywordsIndexViewModel
            {
                //Fill a list with the first letters of all keywords ...
                SelectedLetter = selectedLetter,
                FirstLetters = allKeywords
                .Where(k => string.IsNullOrEmpty(k.KeywordTerm) == false)
                    .GroupBy(k => k.KeywordTerm.Substring(0, 1))
                    .Select(x => x.Key.ToUpper())
                    .ToList()
            };

            // Get a view of keywords starting with the selected letter/number ...
            if (string.IsNullOrEmpty(selectedLetter) || selectedLetter == "All")
            {
                model.keywords = allKeywords;
            }
            else
            {
                if (selectedLetter == "0-9")
                {
                    var numbers = Enumerable.Range(0, 10).Select(i => i.ToString());
                    model.keywords = allKeywords
                        .Where(k => numbers.Contains(k.KeywordTerm.Substring(0, 1)))
                        .ToList();
                }
                else if (selectedLetter == "non alpha")
                {
                    //Get a list 
                    var nonalpha1 = Enumerable.Range(32, 16).Select(i => ((char)i).ToString()).ToList();
                    var nonalpha2 = Enumerable.Range(91, 6).Select(i => ((char)i).ToString()).ToList();
                    var nonalpha3 = Enumerable.Range(123, 4).Select(i => ((char)i).ToString()).ToList();
                    IEnumerable<string> nonalpha = nonalpha1.Concat(nonalpha2).Concat(nonalpha3);

                    model.keywords = allKeywords
                        .Where(k => nonalpha.Contains(k.KeywordTerm.Substring(0, 1)))
                        .ToList();
                }
                else
                {
                    model.keywords = allKeywords
                        .Where(k => k.KeywordTerm.StartsWith(selectedLetter, StringComparison.InvariantCultureIgnoreCase))
                        .ToList();
                }
            }

            ViewData["SeeAlso"] = MenuHelper.SeeAlso("authlistsSeeAlso", ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["controller"].ToString());
            return View(model);
        }
        
        // GET: Keywords/Create
        public ActionResult Create()
        {
            ViewData["ParentKeywordID"] = SelectListHelper.KeywordList(addNew:false);
            KeywordsCreateViewModel kcvm = new KeywordsCreateViewModel();
            ViewBag.Title = "Add New " + _entityName;
            return PartialView();
        }

        // POST: Keywords/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(KeywordsCreateViewModel kvm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var keyword = new Keyword
                    {
                        KeywordTerm = kvm.KeywordTerm,
                        ParentKeywordID = kvm.ParentKeywordID == 0 ? -1 : kvm.ParentKeywordID,
                        CanUpdate = true,
                        CanDelete = true,
                        InputDate = DateTime.Now
                    };
                    _repository.Insert(keyword);
                    CacheProvider.RemoveCache("keywords");
                    return Json(new { success = true });
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", e.Message);
                }
            }

            ViewData["ParentKeywordID"] = SelectListHelper.KeywordList(kvm.ParentKeywordID.Value); //new SelectList(_db.Keywords, "KeywordID", "KeywordTerm", kvm.ParentKeywordID);
            return PartialView(kvm);
        }

        public ActionResult _add()
        {
            return PartialView();
        }

        // POST: Keywords/_add
        [HttpPost]
        public JsonResult _add(Keyword keyword)
        {
            if (ModelState.IsValid)
            {
                var newKeyword = _db.Keywords.FirstOrDefault(x => x.KeywordTerm == keyword.KeywordTerm);

                if (newKeyword == null)
                {
                    newKeyword = new Keyword
                    {
                        KeywordTerm = keyword.KeywordTerm,
                        ParentKeywordID = -1,
                        CanUpdate = true,
                        CanDelete = true,
                        InputDate = DateTime.Now
                    };

                    _db.Keywords.Add(newKeyword);
                    CacheProvider.RemoveCache("keywords");
                    _db.SaveChanges();

                    return Json(new
                    {
                        success = true,
                        newData = newKeyword
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

        // GET: Keywords/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var keyword = _repository.GetById<Keyword>(id.Value);
            if (keyword == null)
            {
                return HttpNotFound();
            }
            var viewModel = new KeywordsEditViewModel
            {
                KeywordID = keyword.KeywordID,
                ParentKeywordID = keyword.ParentKeywordID,
                KeywordTerm = keyword.KeywordTerm
            };

            ViewData["ParentKeywordID"] = SelectListHelper.KeywordList(selected:viewModel.ParentKeywordID.Value, addNew:false, avoid:id.Value);
            ViewBag.Title = "Edit " + _entityName;
            
            return PartialView(viewModel);
        }

        // POST: Keywords/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, KeywordsEditViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var keyword = _repository.GetById<Keyword>(id);
                    if (keyword == null)
                    {
                        return HttpNotFound();
                    }
                    if (keyword.Deleted)
                    {
                        return HttpNotFound();
                    }
                    keyword.KeywordTerm = viewModel.KeywordTerm;
                    keyword.ParentKeywordID = viewModel.ParentKeywordID == 0 ? -1 : viewModel.ParentKeywordID;
                    keyword.LastModified = DateTime.Now;
                    _repository.Update(keyword);
                    CacheProvider.RemoveCache("keywords");
                    return Json(new { success = true });
                    //return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", e.Message);
                }
            }
            ViewData["ParentKeywordID"] = SelectListHelper.KeywordList(viewModel.ParentKeywordID.Value);
            return PartialView(viewModel);
        }


        public static int GetKeywordId(string keyword)
        {
            keyword = keyword.Trim();
            var db = new DbEntities();
            var allKeywords = CacheProvider.GetAll<Keyword>("keywords");
            var model = allKeywords.FirstOrDefault(x => String.Equals(x.KeywordTerm, keyword, StringComparison.OrdinalIgnoreCase));
            if (model != null) return model.KeywordID;
            //Otherwise, insert new keyword now ...
            var newKeyword = new Keyword
            {
                KeywordTerm = keyword,
                ParentKeywordID = -1,
                InputDate = DateTime.Now
            };
            db.Keywords.Add(newKeyword);
            db.SaveChanges();
            CacheProvider.RemoveCache("keywords");
            return newKeyword.KeywordID;
        }

        
        [HttpGet]
        public ActionResult Delete(int id = 0)
        {
            var keyword = _db.Keywords.Find(id);
            if (keyword == null)
            {
                return HttpNotFound();
            }
            // Create new instance of DeleteConfirmationViewModel and pass it to _DeleteConfirmation Dialog (Reusable)
            DeleteConfirmationViewModel deleteConfirmationViewModel = new DeleteConfirmationViewModel
            {
                DeleteEntityId = id,
                HeaderText = _entityName,
                PostDeleteAction = "Delete",
                PostDeleteController = "Keywords",
                DetailsText = keyword.KeywordTerm
            };
            return PartialView("_DeleteConfirmation", deleteConfirmationViewModel);
        }

        [HttpPost]
        public ActionResult Delete(DeleteConfirmationViewModel dcvm)
        {
            // Get item which we need to delete from EntityFramework
            var keyword = _repository.GetById<Keyword>(dcvm.DeleteEntityId);

            if (keyword == null)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _repository.Delete(keyword);
                    return Json(new { success = true });
                }
                catch (SqlException e)
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
