using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using slls.DAO;
using slls.Models;
using slls.ViewModels;

namespace slls.Areas.Config
{
    public class IpAddressesController : ConfigBaseController
    {
        private readonly DbEntities _db = new DbEntities();
        private readonly string _entityName = "IP Address";
        
        // GET: Config/IpAddress
        public ActionResult Index()
        {
            var ipAddresses = _db.IpAddresses;
            ViewBag.Title = "IP Addresses";
            return View(ipAddresses);
        }

        public ActionResult Add()
        {
            ViewBag.Title = "Add new " + _entityName;
            var viewModel = new IpAddressAddEditViewModel();
            return PartialView(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(IpAddressAddEditViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var ipAddress = new IpAddress()
                {
                    IpAddress1 = viewModel.IpAddress1,
                    AllowPassThrough = viewModel.AllowPassThrough,
                    CanUpdate = viewModel.CanUpdate,
                    CanDelete = viewModel.CanDelete,
                    InputDate = DateTime.Now
                };
                _db.IpAddresses.Add(ipAddress);
                _db.SaveChanges();
                CacheProvider.RemoveCache("ipaddresses");
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }

        // GET: StatusTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var ipAddress = _db.IpAddresses.Find(id.Value);
            if (ipAddress == null)
            {
                return HttpNotFound();
            }

            var viewModel = new IpAddressAddEditViewModel()
            {
                RecId = ipAddress.RecId,
                IpAddress1 = ipAddress.IpAddress1,
                AllowPassThrough = ipAddress.AllowPassThrough,
                Blocked = ipAddress.Blocked,
                CanDelete = ipAddress.CanDelete,
                CanUpdate = ipAddress.CanUpdate
            };
            ViewBag.Title = "Edit " + _entityName;
            return PartialView(viewModel);
        }

        // POST: IpAddress/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(IpAddressAddEditViewModel viewModel)
        {
            if (!ModelState.IsValid) return Json(new { success = false });

            var ipAddress = _db.IpAddresses.Find(viewModel.RecId);
            if (ipAddress == null)
            {
                return HttpNotFound();
            }

            ipAddress.IpAddress1 = viewModel.IpAddress1;
            ipAddress.AllowPassThrough = viewModel.AllowPassThrough;
            ipAddress.Blocked = viewModel.Blocked;
            _db.Entry(ipAddress).State = EntityState.Modified;
            _db.SaveChanges();
            CacheProvider.RemoveCache("ipaddresses");

            return Json(new { success = true });
        }

        public static void AllowCurrentIpAddress()
        {
            //Get users IP Address 
            string ipAddress = System.Web.HttpContext.Current.Request.UserHostAddress;
            //Insert into IpAddresses if not exists
            var db = new DbEntities();
            var existing = db.IpAddresses.FirstOrDefault(x => x.IpAddress1 == ipAddress);
            if (existing == null)
            {
                var newIpAddress = new IpAddress()
                {
                    IpAddress1 = ipAddress,
                    AllowPassThrough = false,
                    CanUpdate = true,
                    CanDelete = true,
                    InputDate = DateTime.Now
                };
                db.IpAddresses.Add(newIpAddress);
                db.SaveChanges();
            }
            else
            {
                existing.AllowPassThrough = true;
                db.Entry(existing).State = EntityState.Modified;
                db.SaveChanges();
                CacheProvider.RemoveCache("ipaddresses");
            }
        }


        [HttpGet]
        public ActionResult Delete(int id = 0)
        {
            var ipAddress = _db.IpAddresses.Find(id);
            if (ipAddress == null)
            {
                return HttpNotFound();
            }
            
            if (ipAddress.CanDelete == false)
            {
                return RedirectToAction("Index");
            }

            // Create new instance of DeleteConfirmationViewModel and pass it to _DeleteConfirmation Dialog (Reusable)
            DeleteConfirmationViewModel deleteConfirmationViewModel = new DeleteConfirmationViewModel
            {
                DeleteEntityId = id,
                HeaderText = _entityName,
                PostDeleteAction = "Delete",
                PostDeleteController = "IpAddress",
                DetailsText = ipAddress.IpAddress1
            };
            return PartialView("_DeleteConfirmation", deleteConfirmationViewModel);
        }

        [HttpPost]
        public ActionResult Delete(DeleteConfirmationViewModel dcvm)
        {
            // Get item which we need to delete from EntityFramework
            var ipAddress = _db.IpAddresses.Find(dcvm.DeleteEntityId);

            if (ipAddress == null)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _db.IpAddresses.Remove(ipAddress);
                    _db.SaveChanges();
                    CacheProvider.RemoveCache("ipaddresses");
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