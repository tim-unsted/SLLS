using System;
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
    public class SupplierPeopleCommsController : AdminBaseController
    {
        private readonly DbEntities _db = new DbEntities();

        // GET: LibraryAdmin/SupplierPeopleComms
        public ActionResult Index()
        {
            var supplierPeopleComms = _db.SupplierPeopleComms.Include(s => s.CommMethodType).Include(s => s.SupplierPeople);
            return View(supplierPeopleComms.ToList());
        }


        // GET: LibraryAdmin/SupplierPeopleComms/List/{id] -- ContactID provided
        public ActionResult List(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var supplierPeopleComms = _db.SupplierPeopleComms.Include(s => s.CommMethodType).Where(s => s.ContactID == id);
            return PartialView(supplierPeopleComms.ToList());
        }
        

        // GET: LibraryAdmin/SupplierPeopleComms/Create
        public ActionResult Create()
        {
            ViewBag.MethodID = new SelectList(_db.CommMethodTypes, "MethodID", "Method");
            ViewBag.ContactID = new SelectList(_db.SupplierPeoples, "ContactID", "Title");
            return View();
        }


        // GET: LibraryAdmin/SupplierPeopleComms/Add/{id} - passed ContactID
        public ActionResult Add(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var contactName = _db.SupplierPeoples.Find(id).Fullname;

            var viewModel = new SupplierPeopleCommsViewModel
            {
                ContactID = id.Value,
                ContactName = contactName
            };

            ViewBag.MethodID = SelectListHelper.CommTypesList(); //new SelectList(_db.CommMethodTypes, "MethodID", "Method");
            ViewBag.Title = "Add " + DbRes.T("CommunicationTypes.Methods", "FieldDisplayName");
            return PartialView(viewModel);
        }


        // POST: LibraryAdmin/SupplierPeopleComms/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ContactID,MethodID,Detail")] SupplierPeopleCommsViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var supplierPeopleComm = new SupplierPeopleComm
                {
                    ContactID = viewModel.ContactID,
                    MethodID = viewModel.MethodID,
                    Detail = viewModel.Detail,
                    InputDate = DateTime.Now
                };
                
                _db.SupplierPeopleComms.Add(supplierPeopleComm);
                _db.SaveChanges();
                //return RedirectToAction("Index");
                return Json(new { success = true });
            }

            //ViewBag.MethodID = new SelectList(_db.CommMethodTypes, "MethodID", "Method", supplierPeopleComm.MethodID);
            //ViewBag.ContactID = new SelectList(_db.SupplierPeoples, "ContactID", "Title", supplierPeopleComm.ContactID);
            return PartialView("Add", viewModel);
        }

        // GET: LibraryAdmin/SupplierPeopleComms/Edit/5 -- passed 
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var supplierPeopleComm = _db.SupplierPeopleComms.Find(id);
            if (supplierPeopleComm == null)
            {
                return HttpNotFound();
            }

            var viewModel = new SupplierPeopleCommsViewModel
            {
                CommID = supplierPeopleComm.CommID,
                ContactID = supplierPeopleComm.ContactID,
                MethodID = supplierPeopleComm.MethodID,
                Detail = supplierPeopleComm.Detail,
                ContactName = _db.SupplierPeoples.Find(supplierPeopleComm.ContactID).Fullname
            };
            
            ViewBag.MethodID = new SelectList(_db.CommMethodTypes, "MethodID", "Method", supplierPeopleComm.MethodID);
            ViewBag.Title = "Edit " + DbRes.T("CommunicationTypes.Methods", "FieldDisplayName");
            return PartialView(viewModel);
        }

        // POST: LibraryAdmin/SupplierPeopleComms/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CommID,MethodID,Detail")] SupplierPeopleCommsViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var supplierPeopleComm = _db.SupplierPeopleComms.Find(viewModel.CommID);
                if (supplierPeopleComm == null)
                {
                    return HttpNotFound();
                }
                supplierPeopleComm.CommID = viewModel.CommID;
                supplierPeopleComm.ContactID = viewModel.ContactID;
                supplierPeopleComm.MethodID = viewModel.MethodID;
                supplierPeopleComm.Detail = viewModel.Detail;

                _db.Entry(supplierPeopleComm).State = EntityState.Modified;
                _db.SaveChanges();
                //return RedirectToAction("Index");
                return Json(new { success = true });
            }
            ViewBag.MethodID = new SelectList(_db.CommMethodTypes, "MethodID", "Method", viewModel.MethodID);
            return View(viewModel);
        }

        [HttpGet]
        public ActionResult Delete(int id = 0)
        {
            var comm = _db.SupplierPeopleComms.Find(id);
            if (comm == null)
            {
                return HttpNotFound();
            }
            if (comm.Deleted)
            {
                return HttpNotFound();
            }

            // Create new instance of DeleteConfirmationViewModel and pass it to _DeleteConfirmation Dialog (Reusable)
            DeleteConfirmationViewModel deleteConfirmationViewModel = new DeleteConfirmationViewModel
            {
                DeleteEntityId = id,
                HeaderText = "Contact Details",
                PostDeleteAction = "Delete",
                PostDeleteController = "SupplierPeopleComms",
                DetailsText = comm.CommMethodType.Method + ": " + comm.Detail
            };
            return PartialView("_DeleteConfirmation", deleteConfirmationViewModel);
        }

        [HttpPost]
        public ActionResult Delete(DeleteConfirmationViewModel dcvm)
        {
            // Get item which we need to delete from EntityFramework 
            var item = _db.SupplierPeopleComms.Find(dcvm.DeleteEntityId);

            if (item == null)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _db.SupplierPeopleComms.Remove(item);
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
