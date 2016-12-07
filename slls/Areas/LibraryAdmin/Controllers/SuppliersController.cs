using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using slls.DAO;
using slls.Models;
using slls.Utils.Helpers;
using slls.ViewModels;
using Westwind.Globalization;

namespace slls.Areas.LibraryAdmin
{
    public class SuppliersController : FinanceBaseController
    {
        private readonly DbEntities _db = new DbEntities();
        private readonly string _entityName = DbRes.T("Suppliers.Supplier", "FieldDisplayName");
        private readonly GenericRepository _repository;

        public SuppliersController()
        {
            ViewBag.Title = DbRes.T("Suppliers", "EntityType");
            _repository = new GenericRepository(typeof(Supplier));
        }

        //GET: Select a supplier ...
        public ActionResult Select()
        {
            ViewData["SupplierId"] = SelectListHelper.SupplierList();
            //ViewData["SeeAlso"] = MenuHelper.SeeAlso("titlesSeeAlso", this.ControllerContext.RouteData.Values["action"].ToString());
            ViewBag.Title = "View/Edit " + _entityName;
            ViewBag.Message = _entityName + " to view:";
            return View();
        }

        //POST: Select a supplier ...
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Select(int? id)
        {
            return RedirectToAction("Edit", new {id });
        }

        // GET: Suppliers
        public ActionResult Index(string selectedLetter = "")
        {
            var viewModel = new SuppliersListViewModel();
            
            //Fill a list with the first letters of all suppliers names ...
            viewModel.FirstLetters = _db.Suppliers
                .Where(s => !string.IsNullOrEmpty(s.SupplierName.Substring(0, 1)))
                .GroupBy(s => s.SupplierName.Substring(0, 1))
                .Select(x => x.Key.ToUpper())
                .ToList();

            if (string.IsNullOrEmpty(selectedLetter))
            {
                selectedLetter = viewModel.FirstLetters.FirstOrDefault();
            }

            viewModel.SelectedLetter = selectedLetter;

            if (selectedLetter == "All")
            {
                viewModel.Suppliers = _db.Suppliers.ToList();
            }
            else
            {
                if (selectedLetter == "0-9")
                {
                    var numbers = Enumerable.Range(0, 10).Select(i => i.ToString());
                    viewModel.Suppliers = _db.Suppliers
                        .Where(s => numbers.Contains(s.SupplierName.Substring(0, 1)))
                        .ToList();
                }
                else if (selectedLetter == "non alpha")
                {
                    //Get a list 
                    var nonalpha1 = Enumerable.Range(32, 16).Select(i => ((char)i).ToString()).ToList();
                    var nonalpha2 = Enumerable.Range(91, 6).Select(i => ((char)i).ToString()).ToList();
                    var nonalpha3 = Enumerable.Range(123, 4).Select(i => ((char)i).ToString()).ToList();
                    IEnumerable<string> nonalpha = nonalpha1.Concat(nonalpha2).Concat(nonalpha3);

                    viewModel.Suppliers = _db.Suppliers
                        .Where(s => nonalpha.Contains(s.SupplierName.Substring(0, 1)))
                        .ToList();
                }
                else
                {
                    viewModel.Suppliers = _db.Suppliers
                        .Where(s => s.SupplierName.StartsWith(selectedLetter))
                        .ToList();
                }
            }

            ViewData["SeeAlso"] = MenuHelper.SeeAlso("authlistsSeeAlso", ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["controller"].ToString());
            return View(viewModel);
        }
        
       
        // GET: Suppliers/Create
        public ActionResult Create()
        {
            ViewBag.Title = "Add New " + _entityName;
            var viewModel = new SuppliersAddViewModel();
            return PartialView(viewModel);
        }

        // POST: Suppliers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
            [Bind(Include = "SupplierName,Notes")] SuppliersAddViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var supplier = new Supplier
                {
                    SupplierName = viewModel.SupplierName,
                    Notes = viewModel.Notes,
                    CanUpdate = true,
                    CanDelete = true,
                    InputDate = DateTime.Now
                };
                _repository.Insert(supplier);
                CacheProvider.RemoveCache("suppliers");
                
                var supplierAddress = new SupplierAddress
                {
                    SupplierID = supplier.SupplierID,
                    Division = "Default Address",
                    InputDate = DateTime.Now
                };
                _repository.Insert(supplierAddress);
                //return Json(new { success = true });
                //return RedirectToAction("Edit", new{id = supplier.SupplierID});
                return RedirectToAction("Edit", "SupplierAddresses", new { id = supplierAddress.AddressID});
            }

            return PartialView(viewModel);
        }

        // GET: Suppliers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Select");
            }
            var supplier = _repository.GetById<Supplier>(id.Value);
            if (supplier == null)
            {
                return RedirectToAction("Select");
            }
            if (supplier.Deleted)
            {
                return RedirectToAction("Select");
            }

            var viewModel = new SuppliersEditViewModel
            {
                SupplierID = supplier.SupplierID,
                SupplierName = supplier.SupplierName,
                Notes = supplier.Notes,
                CanDelete = supplier.CanDelete,
                CanUpdate = supplier.CanUpdate
            };
            
            var contacts = from people in _db.SupplierPeoples
                join addresses in _db.SupplierAddresses on people.AddressID equals addresses.AddressID
                where addresses.SupplierID == supplier.SupplierID
                select people;

            ViewBag.ContactsCount = contacts.Count();
            ViewBag.AddressCount = _db.SupplierAddresses.Count(x => x.SupplierID == supplier.SupplierID);

            ViewData["SelectSupplier"] = SelectListHelper.SupplierList(id.Value);
            ViewBag.Title = "View/Edit " + _entityName;
            return View(viewModel);
        }

        // POST: Suppliers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
            [Bind(Include = "SupplierID,SupplierName,Notes,CanUpdate,CanDelete")] SuppliersEditViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var supplier = _repository.GetById<Supplier>(viewModel.SupplierID);
                if (supplier == null)
                {
                    return RedirectToAction("Select");
                }
                if (supplier.Deleted)
                {
                    return RedirectToAction("Select");
                }
                supplier.SupplierID = viewModel.SupplierID;
                supplier.SupplierName = viewModel.SupplierName;
                supplier.Notes = viewModel.Notes;
                supplier.LastModified = DateTime.Now;
                _repository.Update(supplier);
                CacheProvider.RemoveCache("suppliers");
                return Json(new { success = true });
                //return RedirectToAction("Index");
            }

            ViewData["SelectSupplier"] = SelectListHelper.SupplierList(viewModel.SupplierID);
            ViewBag.Title = "Edit " + _entityName;
            return View(viewModel);
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult Autocomplete(string term)
        {
            var suppliers = (from s in _db.Suppliers
                          where s.SupplierName.Contains(term)
                          orderby s.SupplierName
                          select new { s.SupplierName, s.SupplierID }).Take(10);

            IList<SelectListItem> list = new List<SelectListItem>();

            foreach (var x in suppliers)
            {
                list.Add(new SelectListItem { Text = x.SupplierName, Value = x.SupplierID.ToString() });
            }

            var result = list.Select(item => new KeyValuePair<string, string>(item.Value.ToString(), item.Text)).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult Delete(int id = 0)
        {
            var supplier = _repository.GetById<Supplier>(id);
            if (supplier == null)
            {
                return HttpNotFound();
            }
            if (supplier.Deleted)
            {
                return HttpNotFound();
            }
            if (supplier.CanDelete == false)
            {
                return RedirectToAction("Index");
            }
            // Create new instance of DeleteConfirmationViewModel and pass it to _DeleteConfirmation Dialog (Reusable)
            DeleteConfirmationViewModel deleteConfirmationViewModel = new DeleteConfirmationViewModel
            {
                DeleteEntityId = id,
                HeaderText = _entityName,
                PostDeleteAction = "Delete",
                PostDeleteController = "Suppliers",
                DetailsText = supplier.SupplierName
            };
            return PartialView("_DeleteConfirmation", deleteConfirmationViewModel);
        }

        [HttpPost]
        public ActionResult Delete(DeleteConfirmationViewModel dcvm)
        {
            // Get item which we need to delete from EntityFramework 
            var item = _db.Suppliers.Find(dcvm.DeleteEntityId);

            if (item == null)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _db.Suppliers.Remove(item);
                    _db.SaveChanges();
                    CacheProvider.RemoveCache("suppliers");
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