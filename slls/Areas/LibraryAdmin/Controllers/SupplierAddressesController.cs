using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using slls.Models;
using slls.ViewModels;

namespace slls.Areas.LibraryAdmin
{
    public class SupplierAddressesController : AdminBaseController
    {
        private readonly DbEntities _db = new DbEntities();

        // GET: LibraryAdmin/SupplierAddresses
        public ActionResult Index()
        {
            var supplierAddresses = _db.SupplierAddresses.Include(s => s.Supplier);
            return View(supplierAddresses.ToList());
        }

        // GET: LibraryAdmin/SupplierAddresses/List
        public ActionResult List(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var supplierAddresses = _db.SupplierAddresses.Where(a => a.SupplierID == id);
            return PartialView(supplierAddresses.ToList());
        }

        
        // GET: LibraryAdmin/SupplierAddresses/Create
        public ActionResult Create()
        {
            ViewBag.SupplierID = new SelectList(_db.Suppliers, "SupplierID", "SupplierName");
            return View();
        }

        // GET: LibraryAdmin/SupplierAddresses/Add/{id}
        public ActionResult Add(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var supplierName = _db.Suppliers.Find(id).SupplierName;

            var viewModel = new SupplierAddressAddViewModel
            {
                SupplierID = id,
                SupplierName = supplierName,
                Division = "New Address"
            };
            ViewBag.Title = "Add New Address";
            return PartialView(viewModel);
        }

        // POST: LibraryAdmin/SupplierAddresses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AddressID,SupplierID,Division,Address1,Address2,Town_City,County,Postcode,Country,DX,MainTel,MainFax,Account,ActivityCode,URL,WebPassword,Notes,Email,Phone1,InputDate")] SupplierAddressAddViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var newAddress = new SupplierAddress
                {
                    SupplierID = viewModel.SupplierID,
                    Address1 = viewModel.Address1,
                    Address2 = viewModel.Address2,
                    Town_City = viewModel.Town_City,
                    County = viewModel.County,
                    Postcode = viewModel.Postcode,
                    Country = viewModel.Country,
                    DX = viewModel.DX,
                    Division = viewModel.Division,
                    MainFax = viewModel.MainFax,
                    MainTel = viewModel.MainTel,
                    Phone1 = viewModel.Phone1,
                    Account = viewModel.Account,
                    ActivityCode = viewModel.ActivityCode,
                    URL = viewModel.URL,
                    Email = viewModel.Email,
                    WebPassword = viewModel.WebPassword,
                    Notes = viewModel.Notes,
                    InputDate = DateTime.Now
                };

                _db.SupplierAddresses.Add(newAddress);
                _db.SaveChanges();
                //return RedirectToAction("Edit", "Suppliers", new {id = viewModel.SupplierID});
                return Json(new { success = true });
            }

            ViewBag.SupplierID = new SelectList(_db.Suppliers, "SupplierID", "SupplierName", viewModel.SupplierID);
            return PartialView("Add", viewModel);
        }

        // GET: LibraryAdmin/SupplierAddresses/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SupplierAddress supplierAddress = _db.SupplierAddresses.Find(id);
            if (supplierAddress == null)
            {
                return HttpNotFound();
            }
            //ViewBag.SupplierID = new SelectList(db.Suppliers, "SupplierID", "SupplierName", supplierAddress.SupplierID);
            ViewBag.Title = "Edit Supplier Address";
            ViewBag.PeopleCount = _db.SupplierPeoples.Count(x => x.AddressID == supplierAddress.AddressID);
            return View(supplierAddress);
        }

        // POST: LibraryAdmin/SupplierAddresses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AddressID,SupplierID,Division,Address1,Address2,Town_City,County,Postcode,Country,DX,MainTel,MainFax,Account,ActivityCode,URL,WebPassword,Notes,Email,Phone1")] SupplierAddress supplierAddress)
        {
            if (ModelState.IsValid)
            {
                supplierAddress.LastModified = DateTime.Now;
                _db.Entry(supplierAddress).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Edit","Suppliers", new{ id = supplierAddress.SupplierID });
            }
            //ViewBag.SupplierID = new SelectList(_db.Suppliers, "SupplierID", "SupplierName", supplierAddress.SupplierID);
            ViewBag.Title = "Edit Supplier Address";
            ViewBag.PeopleCount = _db.SupplierPeoples.Count(x => x.AddressID == supplierAddress.AddressID);
            return View(supplierAddress);
        }

        [HttpGet]
        public ActionResult Delete(int id = 0)
        {
            var supplierAddress = _db.SupplierAddresses.Find(id);
            if (supplierAddress == null)
            {
                return HttpNotFound();
            }
            if (supplierAddress.Deleted)
            {
                return HttpNotFound();
            }
            

            // Create new instance of DeleteConfirmationViewModel and pass it to _DeleteConfirmation Dialog (Reusable)
            DeleteConfirmationViewModel deleteConfirmationViewModel = new DeleteConfirmationViewModel
            {
                DeleteEntityId = id,
                HeaderText = "Supplier Address",
                PostDeleteAction = "Delete",
                PostDeleteController = "SupplierAddresses",
                DetailsText = supplierAddress.Division
            };
            return PartialView("_DeleteConfirmation", deleteConfirmationViewModel);
        }

        [HttpPost]
        public ActionResult Delete(DeleteConfirmationViewModel dcvm)
        {
            // Get item which we need to delete from EntityFramework 
            var item = _db.SupplierAddresses.Find(dcvm.DeleteEntityId);

            if (item == null)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _db.SupplierAddresses.Remove(item);
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
