using System;
using System.Collections.Generic;
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
    public class TitleAdditionalFieldDataController : Controller
    {
        private readonly DbEntities _db = new DbEntities();

        // GET: LibraryAdmin/TitleAdditionalFieldData
        //public ActionResult Index()
        //{
        //    var titleAdditionalFieldData = _db.TitleAdditionalFieldDatas.Include(t => t.Title).Include(t => t.TitleAdditionalFieldDef);
        //    return View(titleAdditionalFieldData.ToList());
        //}


        // GET: LibraryAdmin/TitleAdditionalFieldData/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    TitleAdditionalFieldData titleAdditionalFieldData = _db.TitleAdditionalFieldDatas.Find(id);
        //    if (titleAdditionalFieldData == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(titleAdditionalFieldData);
        //}


        public ActionResult CustomFields(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //Get the title details - this will include any data in the custom fields (TitleAdditionalFieldData)
            var title = _db.Titles.Find(id);

            //Create a new view model containing any existing data ...
            var viewModel = title.TitleAdditionalFieldDatas
                .Where(data => data.TitleAdditionalFieldDef.IsLongText == false)
                .Select(item => new TitleCustomDataIndexViewModel
                {
                    RecId = item.RecID,
                    FieldId = item.FieldID,
                    TitleId = item.TitleID,
                    FieldData = item.FieldData,
                    FieldName = item.TitleAdditionalFieldDef.FieldName
                }).ToList();

            //Add empty fields for other custom field not yet used ...
            foreach (var item in _db.TitleAdditionalFieldDefs.Where(data => data.IsLongText == false))
            {
                if (viewModel.All(data => data.FieldId != item.FieldID))
                {
                    viewModel.Add(new TitleCustomDataIndexViewModel { RecId = 0, FieldId = item.FieldID, TitleId = id.Value, FieldData = "", FieldName = item.FieldName });
                }
            }

            return PartialView(viewModel.OrderBy(data => data.FieldId));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateCustomFields(List<TitleCustomDataIndexViewModel> viewModel)
        {
            foreach (var item in viewModel)
            {
                var newData = new TitleAdditionalFieldData
                {
                    RecID = item.RecId,
                    TitleID = item.TitleId,
                    FieldID = item.FieldId,
                    FieldData = item.FieldData
                };
                if (_db.TitleAdditionalFieldDatas.Any(data => data.FieldID == newData.FieldID && data.TitleID == newData.TitleID))
                {
                    //Data already exists, so just update it ...
                    newData.InputDate = DateTime.Now;
                    _db.Entry(newData).State = EntityState.Modified;
                    _db.SaveChanges();
                }
                else
                {
                    //Data does not exist, so add it now if the value is not NULL or ""
                    if (newData.FieldData != null)
                    {
                        newData.LastModified = DateTime.Now;
                        _db.TitleAdditionalFieldDatas.Add(newData);
                        _db.SaveChanges();
                    }

                }
            }

            var titleId = from t in viewModel
                          select t.TitleId;
            return RedirectToAction("Edit", "Titles", new { id = titleId.FirstOrDefault() });
        }


        // GET: LibraryAdmin/TitleAdditionalFieldData/AddLongText/5
        public ActionResult AddLongText(int id = 0)
        {
            //Get the Title of the item we're adding to ...
            var titlerecord = _db.Titles.Find(id);
            if (titlerecord == null)
            {
                return HttpNotFound();
            }
            if (titlerecord.Deleted)
            {
                return HttpNotFound();
            }

            var title = titlerecord.Title1;

            var viewModel = new TitleCustomDataAddViewModel
            {
                TitleId = id,
                Title = title
            };

            //Present the user with a list of possible text types ...
            //var textTypes = from f in _db.TitleAdditionalFieldDefs
            //                where f.IsLongText && !(from d in _db.TitleAdditionalFieldDatas
            //                                        where d.TitleID == id
            //                                        select d.FieldID)
            //                    .Contains(f.FieldID)
            //                select f;

            var textTypes = from f in _db.TitleAdditionalFieldDefs
                            where f.IsLongText
                            select f;

            ViewData["FieldID"] = new SelectList(textTypes, "FieldID", "FieldName");
            ViewBag.Title = "Add New " + DbRes.T("TitleTexts.Text", "FieldDisplayName");
            return PartialView(viewModel);
        }


        // GET: LibraryAdmin/TitleAdditionalFieldData/AddCustomData
        public ActionResult AddCustomData(int id = 0)
        {
            //Get the Title of the item we're adding to ...
            var title = from t in _db.Titles
                        where t.TitleID == id
                        select t.Title1;

            var viewModel = new TitleCustomDataAddViewModel
            {
                TitleId = id,
                Title = title.FirstOrDefault()
            };

            ViewData["FieldID"] = new SelectList(_db.TitleAdditionalFieldDefs.Where(x => x.IsLongText == false), "FieldID", "FieldName");
            ViewBag.Title = "Add Custom Data" ;
            return PartialView(viewModel);
        }


        // POST: LibraryAdmin/TitleAdditionalFieldData/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "FieldID,TitleID,FieldData")] TitleCustomDataAddViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var newData = new TitleAdditionalFieldData
                {
                    TitleID = viewModel.TitleId,
                    FieldID = viewModel.FieldId,
                    FieldData = viewModel.FieldData,
                    InputDate = DateTime.Now
                };

                _db.TitleAdditionalFieldDatas.Add(newData);
                _db.SaveChanges();
                return Json(new { success = true });
                //return RedirectToAction("Edit", "Titles", new { id = viewModel.TitleId });
            }

            //ViewData["FieldID"] = new SelectList(_db.TitleAdditionalFieldDefs, "FieldID", "FieldName", viewModel.FieldId);
            //return PartialView("Add", viewModel);
            return RedirectToAction("Edit", "Titles", new { id = viewModel.TitleId });
        }

        // GET: LibraryAdmin/TitleAdditionalFieldData/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var data = _db.TitleAdditionalFieldDatas.Find(id);
            if (data == null)
            {
                return HttpNotFound();
            }

            var viewModel = new TitleCustomDataEditViewModel
            {
                RecId = data.RecID,
                FieldId= data.FieldID,
                TitleId = data.TitleID,
                FieldData = data.FieldData,
                IsLongText = data.TitleAdditionalFieldDef.IsLongText,
                Title = data.Title.Title1
            };

            ViewData["FieldID"] = new SelectList(_db.TitleAdditionalFieldDefs, "FieldID", "FieldName", viewModel.FieldId);
            ViewBag.Title = "Edit " + DbRes.T("TitleTexts.Text", "FieldDisplayName");
            return PartialView(viewModel);
        }

        // POST: LibraryAdmin/TitleAdditionalFieldData/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "RecId,FieldId,TitleId,FieldData")] TitleCustomDataEditViewModel viewModel)
        {
            var editedData = _db.TitleAdditionalFieldDatas.Find(viewModel.RecId);
            if (editedData == null)
            {
                return null;
            }

            editedData.FieldID = viewModel.FieldId;
            editedData.TitleID = viewModel.TitleId;
            editedData.FieldData = viewModel.FieldData;
            editedData.LastModified = DateTime.Now;
            
            if (ModelState.IsValid)
            {
                _db.Entry(editedData).State = EntityState.Modified;
                _db.SaveChanges();
                //return RedirectToAction("Index");
                return Json(new { success = true });
                //return RedirectToAction("Edit", "Titles", new { id = viewModel.TitleId });
            }
            ViewData["FieldID"] = new SelectList(_db.TitleAdditionalFieldDefs, "FieldID", "FieldName", viewModel.FieldId);
            ViewBag.Title = "Edit " + DbRes.T("TitleTexts.Text", "FieldDisplayName");
            return View(viewModel);
        }

        [HttpGet]
        public ActionResult Delete(int id = 0)
        {
            var data = _db.TitleAdditionalFieldDatas.Find(id);
            if (data == null)
            {
                return HttpNotFound();
            }
            
            // Create new instance of DeleteConfirmationViewModel and pass it to _DeleteConfirmation Dialog (Reusable)
            DeleteConfirmationViewModel deleteConfirmationViewModel = new DeleteConfirmationViewModel
            {
                DeleteEntityId = id,
                HeaderText = data.TitleAdditionalFieldDef.FieldName,
                PostDeleteAction = "Delete",
                PostDeleteController = "TitleAdditionalFieldData",
                DetailsText = StringHelper.Truncate(data.FieldData, 50) //data.FieldData.Substring(0, 50)
            };
            return PartialView("_DeleteConfirmation", deleteConfirmationViewModel);
        }

        [HttpPost]
        public ActionResult Delete(DeleteConfirmationViewModel dcvm)
        {
            // Get item which we need to delete from EntityFramework 
            var data = _db.TitleAdditionalFieldDatas.Find(dcvm.DeleteEntityId);

            if (data == null)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _db.TitleAdditionalFieldDatas.Remove(data);
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
