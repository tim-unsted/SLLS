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
    public class BudgetCodesController : AdminBaseController
    {
        private readonly DbEntities _db = new DbEntities();
        private readonly string _entityName = DbRes.T("BudgetCode.Budget_Code", "FieldDisplayName");
        private readonly GenericRepository _repository;

        public BudgetCodesController()
        {
            ViewBag.Title = DbRes.T("BudgetCodes", "EntityType");
            _repository = new GenericRepository(typeof(BudgetCode));
        }

        // GET: BudgetCodes
        public ActionResult Index()
        {
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("authlistsSeeAlso", ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["controller"].ToString());
            return View(_repository.GetAll<BudgetCode>().Where(b => b.Deleted == false));
        }
        
        // GET: BudgetCodes/Create
        public ActionResult Create()
        {
            ViewBag.Title = "Add new " + _entityName;
            var bcvm = new BudgetCodeAddViewModel
            {
                AllocationOneOffs = 0,
                AllocationSubs = 0
            };
            return PartialView(bcvm);
        }

        // POST: BudgetCodes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
            [Bind(
                Include = "BudgetCode,AllocationSubs,AllocationOneOffs")
            ] BudgetCodeAddViewModel bcvm)
        {
            if (ModelState.IsValid)
            {
                var budgetCode = new BudgetCode
                {
                    BudgetCode1 = bcvm.BudgetCode,
                    AllocationOneOffs = bcvm.AllocationOneOffs,
                    AllocationSubs = bcvm.AllocationSubs,
                    CanUpdate = true,
                    CanDelete = true,
                    InputDate = DateTime.Now
                };
                _repository.Insert(budgetCode);
                CacheProvider.RemoveCache("budgetcodes");
                return Json(new { success = true });
                //return RedirectToAction("Index");
            }
            return PartialView(bcvm);
        }

        public ActionResult _add()
        {
            return PartialView();
        }

        // POST: BudgetCodes/_add
        [HttpPost]
        public JsonResult _add(BudgetCode budgetcode)
        {
            if (ModelState.IsValid)
            {
                var newBudgetCode = _db.BudgetCodes.FirstOrDefault(x => x.BudgetCode1 == budgetcode.BudgetCode1);

                if (newBudgetCode == null)
                {
                    newBudgetCode = new BudgetCode()
                    {
                        BudgetCode1 = budgetcode.BudgetCode1,
                        AllocationOneOffs = 0,
                        AllocationSubs = 0,
                        CanUpdate = true,
                        CanDelete = true,
                        InputDate = DateTime.Now
                    };

                    _db.BudgetCodes.Add(newBudgetCode);
                    _db.SaveChanges();
                    CacheProvider.RemoveCache("budgetcodes");

                    return Json(new
                    {
                        success = true,
                        newData = newBudgetCode
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        errMsg = "This Budget Code already exists!"
                    });
                }
            }
            return null;
        }

        // GET: BudgetCodes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var budgetCode = _repository.GetById<BudgetCode>(id.Value);
            if (budgetCode == null)
            {
                return HttpNotFound();
            }
            if (budgetCode.Deleted)
            {
                return HttpNotFound();
            }

            var bcvm = new BudgetCodeEditViewModel
            {
                BudgetCodeID = budgetCode.BudgetCodeID,
                BudgetCode = budgetCode.BudgetCode1,
                AllocationOneOffs = budgetCode.AllocationOneOffs,
                AllocationSubs = budgetCode.AllocationSubs,
                CanUpdate = budgetCode.CanUpdate,
                CanDelete = budgetCode.CanDelete
            };
            ViewBag.Title = "Edit " + _entityName;
            return PartialView(bcvm);
        }

        // POST: BudgetCodes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
            [Bind(
                Include = "BudgetCodeID,BudgetCode,AllocationSubs,AllocationOneOffs,CanUpdate,CanDelete")
            ] BudgetCodeEditViewModel viewModel)
        {
            if (!ModelState.IsValid) return PartialView(viewModel);
            var budgetCode = _repository.GetById<BudgetCode>(viewModel.BudgetCodeID);
            if (budgetCode == null)
            {
                return HttpNotFound();
            }
            if (budgetCode.Deleted)
            {
                return HttpNotFound();
            }
            budgetCode.BudgetCodeID = viewModel.BudgetCodeID;
            budgetCode.BudgetCode1 = viewModel.BudgetCode;
            budgetCode.AllocationOneOffs = viewModel.AllocationOneOffs;
            budgetCode.AllocationSubs = viewModel.AllocationSubs;
            budgetCode.LastModified = DateTime.Now;
            _repository.Update(budgetCode);
            CacheProvider.RemoveCache("budgetcodes");
            //return RedirectToAction("Index");
            return Json(new { success = true });
        }
        
        [HttpGet]
        public ActionResult Delete(int id = 0)
        {
            var budgetCode = _repository.GetById<BudgetCode>(id);
            if (budgetCode == null)
            {
                return HttpNotFound();
            }

            if (budgetCode.Deleted)
            {
                return HttpNotFound();
            }

            if (budgetCode.CanDelete == false)
            {
                return RedirectToAction("Index");
            }
            
            // Create new instance of DeleteConfirmationViewModel and pass it to _DeleteConfirmation Dialog (Reusable)
            DeleteConfirmationViewModel deleteConfirmationViewModel = new DeleteConfirmationViewModel
            {
                DeleteEntityId = id,
                HeaderText = _entityName,
                PostDeleteAction = "Delete",
                PostDeleteController = "BudgetCodes",
                DetailsText = budgetCode.BudgetCode1
            };
            return PartialView("_DeleteConfirmation", deleteConfirmationViewModel);
        }

        [HttpPost]
        public ActionResult Delete(DeleteConfirmationViewModel dcvm)
        {
            // Get item which we need to delete from EntityFramework
            var item = _db.BudgetCodes.Find(dcvm.DeleteEntityId);

            if (item == null)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _db.BudgetCodes.Remove(item);
                    _db.SaveChanges();
                    CacheProvider.RemoveCache("budgetcodes");
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