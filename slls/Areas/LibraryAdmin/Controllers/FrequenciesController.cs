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
    public class FrequenciesController : CatalogueBaseController
    {
        private readonly DbEntities _db = new DbEntities();
        private readonly string _entityName = DbRes.T("Frequency.Frequency", "FieldDisplayName");
        private readonly GenericRepository _repository;

        public FrequenciesController()
        {
            ViewBag.Title = DbRes.T("Frequency", "EntityType");
            _repository = new GenericRepository(typeof(Frequency));
        }

        // GET: Frequencies
        public ActionResult Index()
        {
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("authlistsSeeAlso", ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["controller"].ToString());
            return View(_db.Frequencies.Where(f => f.Deleted == false).OrderBy(f => f.Frequency1));
        }

        
        // GET: Frequencies/Create
        public ActionResult Create()
        {
            ViewBag.Title = "Add New " + _entityName;
            var favm = new FrequencyAddViewModel();
            return PartialView(favm);
        }

        // POST: Frequencies/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
            [Bind(Include = "Frequency,Days")] FrequencyAddViewModel favm)
        {
            if (ModelState.IsValid)
            {
                var frequency = new Frequency
                {
                    Frequency1 = favm.Frequency,
                    Days = favm.Days,
                    CanUpdate = true,
                    CanDelete = true,
                    InputDate = DateTime.Now
                };
                _repository.Insert(frequency);
                CacheProvider.RemoveCache("frequencies");
                //return RedirectToAction("Index");
                return Json(new { success = true });
            }

            return PartialView(favm);
        }

        public ActionResult _add()
        {
            return PartialView();
        }

        // POST: Frequencies/_add
        [HttpPost]
        public JsonResult _add(Frequency frequency)
        {
            try
            {
                var newFrequency = _db.Frequencies.FirstOrDefault(x => x.Frequency1 == frequency.Frequency1);

                if (newFrequency == null)
                {
                    newFrequency = new Frequency
                    {
                        Frequency1 = frequency.Frequency1,
                        Days = 0,
                        CanUpdate = true,
                        CanDelete = true,
                        InputDate = DateTime.Now
                    };

                    _db.Frequencies.Add(newFrequency);
                    _db.SaveChanges();
                    CacheProvider.RemoveCache("frequencies");

                    return Json(new
                    {
                        success = true,
                        newData = newFrequency
                    });
                }
                return Json(new
                {
                    success = false,
                    errMsg = "This Frequency already exists!"
                });
            }
            catch (Exception)
            {
                return null;
            }
        }


        // GET: Frequencies/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var frequency = _repository.GetById<Frequency>(id.Value);
            if (frequency == null)
            {
                return HttpNotFound();
            }
            if (frequency.Deleted)
            {
                return HttpNotFound();
            }
            var fevm = new FrequencyEditViewModel
            {
                Frequency = frequency.Frequency1,
                FrequencyID = frequency.FrequencyID,
                Days = frequency.Days
            };
            ViewBag.Title = "Edit " + _entityName;
            return PartialView(fevm);
        }

        // POST: Frequencies/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
            [Bind(Include = "FrequencyID,Frequency,Days")] FrequencyEditViewModel viewModel)
        {
            if (!ModelState.IsValid) return PartialView(viewModel);
            var frequency = _repository.GetById<Frequency>(viewModel.FrequencyID);
            if (frequency == null)
            {
                return HttpNotFound();
            }
            if (frequency.Deleted)
            {
                return HttpNotFound();
            }
            //frequency.FrequencyID = viewModel.FrequencyID;
            frequency.Frequency1 = viewModel.Frequency;
            frequency.Days = viewModel.Days;
            frequency.LastModified = DateTime.Now;
            _repository.Update(frequency);
            CacheProvider.RemoveCache("frequencies");
            return Json(new { success = true });
            //return RedirectToAction("Index");
        }

        public static int GetFrequencyId(string frequency)
        {
            frequency = frequency.Trim();
            var db = new DbEntities();
            var allFrequencies = CacheProvider.GetAll<Frequency>("frequencies");
            var model = allFrequencies.FirstOrDefault(x => String.Equals(x.Frequency1, frequency, StringComparison.OrdinalIgnoreCase));
            if (model != null) return model.FrequencyID;
            //insert new frequency now ...
            var newFrequency = new Frequency
            {
                Frequency1 = frequency,
                CanUpdate = true,
                CanDelete = true,
                InputDate = DateTime.Now
            };
            db.Frequencies.Add(newFrequency);
            db.SaveChanges();
            CacheProvider.RemoveCache("frequencies");
            return newFrequency.FrequencyID;
        }
        
        [HttpGet]
        public ActionResult Delete(int id = 0)
        {
            var frequency = _db.Frequencies.Find(id);
            if (frequency == null)
            {
                return HttpNotFound();
            }
            if (frequency.Deleted)
            {
                return HttpNotFound();
            }
            if (frequency.CanDelete == false)
            {
                return RedirectToAction("Index");
            }
            // Create new instance of DeleteConfirmationViewModel and pass it to _DeleteConfirmation Dialog (Reusable)
            DeleteConfirmationViewModel deleteConfirmationViewModel = new DeleteConfirmationViewModel
            {
                DeleteEntityId = id,
                HeaderText = _entityName,
                PostDeleteAction = "Delete",
                PostDeleteController = "Frequencies",
                DetailsText = frequency.Frequency1
            };
            return PartialView("_DeleteConfirmation", deleteConfirmationViewModel);
        }

        [HttpPost]
        public ActionResult Delete(DeleteConfirmationViewModel dcvm)
        {
            // Get item which we need to delete from EntityFramework 
            var item = _db.Frequencies.Find(dcvm.DeleteEntityId);

            if (item == null)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _db.Frequencies.Remove(item);
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