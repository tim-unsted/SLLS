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
    //Only allow admin role to access this functionality ...
    [Authorize(Roles = "Admin")]
    public class AccountYearsController : FinanceBaseController
    {
        private readonly DbEntities _db = new DbEntities();
        private readonly string _entityName = DbRes.T("AccountYears.Account_Year", "FieldDisplayName");
        private readonly GenericRepository _repository;

        public AccountYearsController()
        {
            ViewBag.Title = DbRes.T("AccountYears", "EntityType");
            _repository = new GenericRepository(typeof(AccountYear));
        }

        // GET: AccountYears
        public ActionResult Index()
        {
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("authlistsSeeAlso", ControllerContext.RouteData.Values["action"].ToString(), ControllerContext.RouteData.Values["controller"].ToString());
            return View(_repository.GetAll<AccountYear>().Where(a => a.Deleted == false).OrderBy(a => a.ListPos).ThenByDescending(a => a.YearStartDate));
        }
        
        // GET: AccountYears/Create
        public ActionResult Create()
        {
            ViewBag.Title = "Add New " + _entityName;
            var acvm = new AccountYearsAddViewModel();
            return PartialView(acvm);
        }

        // POST: AccountYears/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
            [Bind(Include = "AccountYear,YearStartDate,YearEndDate")] AccountYearsAddViewModel acvm)
        {
            var accountYear = new AccountYear
            {
                AccountYear1 = acvm.AccountYear,
                YearStartDate = acvm.YearStartDate,
                YearEndDate = acvm.YearEndDate,
                CanUpdate = true,
                CanDelete = true,
                InputDate = DateTime.Now
            };
            _repository.Insert(accountYear);
            CacheProvider.RemoveCache("accountyears");
            return Json(new {success = true});
        }

        // GET: AccountYears/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var accountYear = _repository.GetById<AccountYear>(id.Value);
            if (accountYear == null)
            {
                return HttpNotFound();
            }
            if (accountYear.Deleted)
            {
                return HttpNotFound();
            }

            var viewModel = new AccountYearsEditViewModel
            {
                AccountYearID = accountYear.AccountYearID,
                AccountYear = accountYear.AccountYear1,
                YearStartDate = accountYear.YearStartDate,
                YearEndDate = accountYear.YearEndDate,
                CanDelete = accountYear.CanDelete,
                CanUpdate = accountYear.CanUpdate
            };
            ViewBag.Title = "Edit " + _entityName;
            return PartialView(viewModel);
        }

        // POST: AccountYears/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
            [Bind(Include = "AccountYearID,AccountYear,YearStartDate,YearEndDate")] AccountYearsEditViewModel viewModel)
        {
            var accountYear = _repository.GetById<AccountYear>(viewModel.AccountYearID);
            if (accountYear == null)
            {
                return HttpNotFound();
            }
            if (accountYear.Deleted)
            {
                return HttpNotFound();
            }
            accountYear.AccountYear1 = viewModel.AccountYear;
            accountYear.YearStartDate = viewModel.YearStartDate;
            accountYear.YearEndDate = viewModel.YearEndDate;
            accountYear.LastModified = DateTime.Now;

            _repository.Update(accountYear);
            CacheProvider.RemoveCache("accountyears");
            return Json(new { success = true });
        }
        
        [HttpGet]
        public ActionResult Delete(int id = 0)
        {
            var accountYear = _db.AccountYears.Find(id);
            if (accountYear == null)
            {
                return HttpNotFound();
            }
            if (accountYear.Deleted)
            {
                return HttpNotFound();
            }

            //Check if we can delete this item ...
            if (accountYear.CanDelete == false)
            {
                return RedirectToAction("Index");
            }

            // Create new instance of DeleteConfirmationViewModel and pass it to _DeleteConfirmation Dialog (Reusable)
            DeleteConfirmationViewModel deleteConfirmationViewModel = new DeleteConfirmationViewModel
            {
                DeleteEntityId = id,
                HeaderText = _entityName,
                PostDeleteAction = "Delete",
                PostDeleteController = "AccountYears",
                DetailsText = accountYear.AccountYear1
            };
            return PartialView("_DeleteConfirmation", deleteConfirmationViewModel);
        }

        [HttpPost]
        public ActionResult Delete(DeleteConfirmationViewModel dcvm)
        {
            // Get item which we need to delete from EntityFramework 
            var item = _db.AccountYears.Find(dcvm.DeleteEntityId);

            if (item == null)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _db.AccountYears.Remove(item);
                    _db.SaveChanges();
                    CacheProvider.RemoveCache("accountyears");
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