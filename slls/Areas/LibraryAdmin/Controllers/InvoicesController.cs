using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using slls.Models;
using slls.Utils.Helpers;

namespace slls.Areas.LibraryAdmin.Controllers
{
    public class InvoicesController : FinanceBaseController
    {
        private readonly DbEntities _db = new DbEntities();
        private ApplicationUserManager _userManager;

        private ApplicationUserManager UserManager
        {
            get { return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            set { _userManager = value; }
        }

        // GET: LibraryAdmin/Invoices
        public ActionResult Index()
        {
            var invoices = _db.OrderDetails.Include(o => o.AccountYear).Include(o => o.BudgetCode).Include(o => o.OrderCategory).Include(o => o.Supplier).Include(o => o.Title);
            return View(invoices.ToList());
        }

        // GET: LibraryAdmin/Invoices/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderDetail orderDetail = _db.OrderDetails.Find(id);
            if (orderDetail == null)
            {
                return HttpNotFound();
            }
            return View(orderDetail);
        }
        

        // GET: LibraryAdmin/Invoices/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderDetail orderDetail = _db.OrderDetails.Find(id);
            if (orderDetail == null)
            {
                return HttpNotFound();
            }
            var users = UserManager.Users.ToList();
            ViewBag.AccountYearID = new SelectList(_db.AccountYears, "AccountYearID", "AccountYear1", orderDetail.AccountYearID);
            ViewBag.BudgetCodeID = new SelectList(_db.BudgetCodes, "BudgetCodeID", "BudgetCode1", orderDetail.BudgetCodeID);
            ViewBag.Authority = SelectListHelper.SelectUsersByLastname(liveOnly: false, id: orderDetail.AuthoriserUser.Id);
            ViewBag.RequestedBy = SelectListHelper.SelectUsersByLastname(liveOnly: false, id: orderDetail.RequesterUser.Id);
            ViewBag.OrderCategoryID = new SelectList(_db.OrderCategories, "OrderCategoryID", "OrderCategory1", orderDetail.OrderCategoryID);
            ViewBag.SupplierID = new SelectList(_db.Suppliers, "SupplierID", "SupplierName", orderDetail.SupplierID);
            ViewBag.TitleID = new SelectList(_db.Titles, "TitleID", "Title1", orderDetail.TitleID);
            return View(orderDetail);
        }

        // POST: LibraryAdmin/Invoices/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(OrderDetail orderDetail)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(orderDetail).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            var users = UserManager.Users.ToList();
            ViewBag.AccountYearID = new SelectList(_db.AccountYears, "AccountYearID", "AccountYear1", orderDetail.AccountYearID);
            ViewBag.BudgetCodeID = new SelectList(_db.BudgetCodes, "BudgetCodeID", "BudgetCode1", orderDetail.BudgetCodeID);
            ViewBag.Authority = SelectListHelper.SelectUsersByLastname(liveOnly: false,id: orderDetail.AuthoriserUser.Id);
            ViewBag.RequestedBy = SelectListHelper.SelectUsersByLastname(liveOnly: false, id: orderDetail.RequesterUser.Id);
            ViewBag.OrderCategoryID = new SelectList(_db.OrderCategories, "OrderCategoryID", "OrderCategory1", orderDetail.OrderCategoryID);
            ViewBag.SupplierID = new SelectList(_db.Suppliers, "SupplierID", "SupplierName", orderDetail.SupplierID);
            ViewBag.TitleID = new SelectList(_db.Titles, "TitleID", "Title1", orderDetail.TitleID);
            return View(orderDetail);
        }

        // GET: LibraryAdmin/Invoices/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderDetail orderDetail = _db.OrderDetails.Find(id);
            if (orderDetail == null)
            {
                return HttpNotFound();
            }
            return View(orderDetail);
        }

        // POST: LibraryAdmin/Invoices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            OrderDetail orderDetail = _db.OrderDetails.Find(id);
            _db.OrderDetails.Remove(orderDetail);
            _db.SaveChanges();
            return RedirectToAction("Index");
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
