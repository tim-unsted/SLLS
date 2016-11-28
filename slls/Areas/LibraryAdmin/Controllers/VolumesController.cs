using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using slls.App_Settings;
using slls.DAO;
using slls.Models;
using slls.Utils.Helpers;
using slls.ViewModels;
using Westwind.Globalization;

namespace slls.Areas.LibraryAdmin
{
    ////[RouteArea("LibraryAdmin", AreaPrefix = "Admin")]
    ////[Route("{action=index}")]
    public class VolumesController : CatalogueBaseController
    {
        private readonly DbEntities _db = new DbEntities();
        private readonly string _entityName = DbRes.T("CopyItems.Copy_Item", "FieldDisplayName");
        private readonly GenericRepository _repository;

        public VolumesController()
        {
            ViewBag.Title = DbRes.T("CopyItems", "EntityType");
            _repository = new GenericRepository(typeof(Volume));
        }

        //GET: Select a title ...
        public ActionResult Select()
        {
            //ViewData["TitleId"] = SelectListHelper.TitlesList();
            //ViewData["SeeAlso"] = MenuHelper.SeeAlso("volumesSeeAlso", ControllerContext.RouteData.Values["action"].ToString());
            //ViewBag.Title = "List " + DbRes.T("CopyItems","EntityType");
            ////ViewBag.Message = _entityName + " to edit:";
            //return View();
            return RedirectToAction("Index");
        }

        //POST: Select a title ...
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Select(int? id)
        {
            return RedirectToAction("Index", new { id });
        }

        // GET: Volumes
        public ActionResult Index(int id = 0)
        {
            var volumes = _db.Volumes.Include(v => v.Copy);
            
            // Filter by Title ID ...
            var copies = _db.Copies.Where(c => c.TitleID == id).Select(c => c.CopyID);
            volumes = volumes.Where(v => copies.Contains(v.CopyID));

            var viewModel = new VolumesIndexViewModel()
            {
                Volumes = volumes,
                TitleId = id,
                UsePreprintedBarcodes = Settings.GetParameterValue("Catalogue.UsePreprintedBarcodes", "true") == "true"
            };
            ViewData["TitleId"] = SelectListHelper.TitlesWithVolumes(id, msg: "Select a ");
            return View(viewModel);
        }

        public ActionResult DefaultBarcodes(int id = 0)
        {
            var volumes = _db.Volumes.Where(v => v.IsBarcodeEdited == false);

            // Optionally filter by Title ID ...
            if (id > 0)
            {
                var copies = _db.Copies.Where(c => c.TitleID == id).Select(c => c.CopyID);
                volumes = volumes.Where(v => copies.Contains(v.CopyID));
            }
            var viewModel = new VolumesIndexViewModel()
            {
                Volumes = volumes,
                TitleId = id,
                UsePreprintedBarcodes = Settings.GetParameterValue("Catalogue.UsePreprintedBarcodes", "true") == "true"
            };
            ViewData["TitleId"] = SelectListHelper.TitlesWithVolumes(id, msg: "Filter by ");
            ViewBag.Title = ViewBag.Title + " With System Generated " + DbRes.T("CopyItems.Barcode", "FieldDisplayName") + "s";
            return View(viewModel);
        }


        public ActionResult List(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var volumes = _db.vwVolumesWithLoans.Where(x => x.CopyID == id);
            return PartialView(volumes.ToList());
        }


        public ActionResult _ThumbnailDetails(int id = 0)
        {
            //var volumes = _db.vwVolumesWithLoans.Where(x => x.CopyID == id);
            var volumes = _db.Volumes.Where(x => x.CopyID == id).ToList();
            var viewModel = new VolumesWithLoansIndexViewModel() { VolumesWithLoans = new List<VolumesWithLoansViewModel>() };
            
            foreach (var volume in volumes)
            {
                var currentLoan = volume.Borrowings.FirstOrDefault(b => b.Returned == null);
                var row = new VolumesWithLoansViewModel
                {
                    CopyID = volume.CopyID,
                    CopyNumber = volume.Copy.CopyNumber,
                    Barcode = volume.Barcode,
                    VolumeID = volume.VolumeID,
                    Fullname = currentLoan == null ? "" : currentLoan.BorrowerUser.FullnameRev,
                    PrintLabel = volume.PrintLabel,
                    LabelText = volume.LabelText,
                    LoanType = volume.LoanType.LoanTypeName,
                    Borrowed = currentLoan == null ? null : currentLoan.Borrowed,
                    ReturnDue = currentLoan == null ? null : currentLoan.ReturnDue,
                    OnLoan = currentLoan != null
                };
                viewModel.VolumesWithLoans.Add(row);
            }
            
            //return PartialView(volumes);
            return PartialView(viewModel);
        }


        // GET: Volumes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var volume = _db.Volumes.Find(id);
            if (volume == null)
            {
                return HttpNotFound();
            }
            ViewBag.Title = _entityName + " Details";
            return View(volume);
        }
        
        // GET: Volumes/Create
        public ActionResult Create()
        {
            ViewData["TitleId"] = new SelectList(_db.Titles.Where(t => t.Copies.Any()).OrderBy(t => t.Title1.Substring(t.NonFilingChars)), "TitleID", "Title1");
            ViewData["CopyId"] = new SelectList("");
            ViewData["LoanTypeId"] = new SelectList(_db.LoanTypes, "LoanTypeID", "LoanTypeName");
            ViewBag.Title = "Add New " + _entityName;
            ViewBag.BtnText = DbRes.T("Buttons.Confirm_Add", "Terminology");
            var viewModel = new VolumesAddViewModel()
            {
                Barcode = Utils.PublicFunctions.NewBarcode(),
                PrintLabel = true,
                LoanTypeId = 1,
                UsePreprintedBarcodes = Settings.GetParameterValue("Catalogue.UsePreprintedBarcodes", "true") == "true"
            };
            return View(viewModel);
        }


        // POST: Volumes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
            [Bind(
                Include =
                    "CopyId,LabelText,RefOnly,Barcode,PrintLabel,LoanTypeId"
                )] VolumesAddViewModel viewModel)
        {
            var title = _db.Titles.Find(viewModel.TitleId);
            var newVolume = new Volume
            {
                CopyID = viewModel.CopyId,
                LabelText = viewModel.LabelText,
                Barcode = viewModel.Barcode ?? Utils.PublicFunctions.NewBarcode(),
                RefOnly = viewModel.RefOnly,
                PrintLabel = viewModel.PrintLabel,
                LoanTypeID = viewModel.LoanTypeId <= 0 ? Utils.PublicFunctions.GetDefaultLoanType(title.MediaID) : viewModel.LoanTypeId,
                InputDate = DateTime.Now
            };

            if (ModelState.IsValid)
            {
                try
                {
                    _db.Volumes.Add(newVolume);
                    _db.SaveChanges();
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", e.Message.ToString());
                }

                return RedirectToAction("Index", new { copyId = viewModel.CopyId});
            }

            ViewData["TitleId"] = new SelectList(_db.Titles.Where(t => t.Copies.Any()).OrderBy(t => t.Title1.Substring(t.NonFilingChars)), "TitleID", "Title1", viewModel.TitleId);
            ViewData["CopyId"] = new SelectList(_db.Copies.Where(c => c.TitleID == viewModel.TitleId), "CopyID", "CopyNumber", viewModel.CopyId);
            ViewData["LoanTypeId"] = new SelectList(_db.LoanTypes, "LoanTypeID", "LoanTypeName", Utils.PublicFunctions.GetDefaultLoanType(title.MediaID));
            ViewBag.BtnText = DbRes.T("Buttons.Confirm_Add", "Terminology");
            ViewBag.Title = "Add New " + _entityName;
            return PartialView(viewModel);
        }

        // GET: Volumes/Add -- passed a CopyID from the Copies view
        public ActionResult Add(int? id, int step = 1, string returnAction = "Edit", string returnController = "Copies")
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var titleid = _db.Copies.Find(id).TitleID;
            var copynumber = _db.Copies.Find(id).CopyNumber;
            var title = _db.Titles.Find(titleid);

            var viewModel = new VolumesAddViewModel
            {
                CopyId = id.Value,
                TitleId = titleid,
                Barcode = Utils.PublicFunctions.NewBarcode(),
                Title = title.Title1,
                CopyNumber = copynumber.ToString(),
                LoanTypeId = Utils.PublicFunctions.GetDefaultLoanType(title.MediaID),
                PrintLabel = true,
                UsePreprintedBarcodes = Settings.GetParameterValue("Catalogue.UsePreprintedBarcodes", "true") == "true",
                Step = step,
                AddMore = false,
                ReturnController = returnController,
                ReturnAction = returnAction
            };
            ViewData["LoanTypeId"] = new SelectList(_db.LoanTypes, "LoanTypeID", "LoanTypeName", Utils.PublicFunctions.GetDefaultLoanType(title.MediaID));
            ViewBag.Title = step > 1 ? "Step " + step + ": Add New " + _entityName : "Add New " + _entityName;
            ViewBag.BtnText = "Finish";
            return View(viewModel);
        }

        // POST: Volumes/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(VolumesAddViewModel viewModel)
        {
            var title = _db.Titles.Find(viewModel.TitleId);
            var newVolume = new Volume
            {
                CopyID = viewModel.CopyId,
                LabelText = viewModel.LabelText,
                Barcode = viewModel.Barcode ?? Utils.PublicFunctions.NewBarcode(),
                PrintLabel = viewModel.PrintLabel,
                LoanTypeID = viewModel.LoanTypeId <= 0 ? Utils.PublicFunctions.GetDefaultLoanType(title.MediaID) : viewModel.LoanTypeId,
                InputDate = DateTime.Now
            };

            if (ModelState.IsValid)
            {
                try
                {
                    _db.Volumes.Add(newVolume);
                    _db.SaveChanges();

                    //Clear the cache of opac titles if neccessary ...
                    if (newVolume.Copy.StatusType.Opac)
                    {
                        CacheProvider.RemoveCache("opactitles");
                    }
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", e.Message.ToString());
                }

                if (viewModel.AddMore)
                {
                    return RedirectToAction("Add","Volumes", new {id = viewModel.CopyId, step = viewModel.Step});
                }
                if(viewModel.ReturnController == "Titles")
                {
                    return RedirectToAction(viewModel.ReturnAction, "Titles", new { id = viewModel.TitleId });
                }
                if (viewModel.ReturnController == "Copies")
                {
                    return RedirectToAction(viewModel.ReturnAction, "Copies", new { id = viewModel.CopyId });
                }
                return RedirectToAction("Index", "Home", new { area = "LibraryAdmin" });
            }

            ViewBag.Title = "Add New " + _entityName;
            return View(viewModel);
        }

        
        // GET: Volumes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var volume = _db.Volumes.Find(id);
            if (volume == null)
            {
                return HttpNotFound();
            }

            ViewBag.Title = "Edit " + _entityName;
            var viewModel = new VolumesEditViewModel
            {
                CopyId = volume.CopyID,
                VolumeId = id.Value,
                Barcode = volume.Barcode,
                LabelText = volume.LabelText,
                RefOnly = volume.RefOnly,
                PrintLabel = volume.PrintLabel,
                Title = volume.Copy.Title.Title1,
                TitleId = volume.Copy.TitleID,
                CopyNumber = volume.Copy.CopyNumber,
                LoanTypeId = volume.LoanTypeID.Value,
                IsBarcodeEdited = volume.IsBarcodeEdited,
                UsePreprintedBarcodes = Settings.GetParameterValue("Catalogue.UsePreprintedBarcodes", "true") == "true",
            };
            viewModel.BarcodeNeedsEditing = viewModel.IsBarcodeEdited == false && viewModel.UsePreprintedBarcodes;
            ViewData["LoanTypeId"] = new SelectList(_db.LoanTypes, "LoanTypeID", "LoanTypeName", volume.LoanTypeID);
            ViewData["TitleId"] = new SelectList(_db.Titles.Where(t => t.Copies.Any()).OrderBy(t => t.Title1.Substring(t.NonFilingChars)), "TitleID", "Title1",viewModel.TitleId);
            ViewData["CopyId"] = new SelectList(_db.Copies.Where(c => c.TitleID == viewModel.TitleId), "CopyID", "CopyNumber", viewModel.CopyId);
            return PartialView(viewModel);
        }

        // POST: Volumes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
            [Bind(
                Include =
                    "VolumeID,CopyID,LabelText,RefOnly,Barcode,PrintLabel,LoanTypeId"
                )] VolumesEditViewModel viewModel)
        {
            var volume = _db.Volumes.Find(viewModel.VolumeId);
            if (volume == null)
            {
                return HttpNotFound();
            }
            volume.IsBarcodeEdited = viewModel.Barcode != volume.Barcode;
            volume.VolumeID = viewModel.VolumeId;
            volume.CopyID = viewModel.CopyId;
            volume.Barcode = viewModel.Barcode;
            volume.LabelText = viewModel.LabelText;
            volume.RefOnly = viewModel.RefOnly;
            volume.PrintLabel = viewModel.PrintLabel;
            volume.LoanTypeID = viewModel.LoanTypeId;
            volume.LastModified = DateTime.Now;
            
            if (ModelState.IsValid)
            {
                _db.Entry(volume).State = EntityState.Modified;
                _db.SaveChanges();
                return Json(new { success = true });
            }
            ViewBag.Title = "Edit " + _entityName;
            ViewData["TitleId"] = new SelectList(_db.Titles.Where(t => t.Copies.Any()).OrderBy(t => t.Title1.Substring(t.NonFilingChars)), "TitleID", "Title1", viewModel.TitleId);
            ViewData["CopyId"] = new SelectList(_db.Copies.Where(c => c.CopyID == 0), "CopyID", "CopyNumber", viewModel.CopyId);
            ViewData["LoanTypeId"] = new SelectList(_db.LoanTypes, "LoanTypeID", "LoanTypeName");
            return View(viewModel);
        }

        //EditTitle
        public ActionResult EditTitle(int? id)
        {
            if (id != null)
            {
                return RedirectToAction("Edit", "Titles", new {id = id.Value});
            }
            return RedirectToAction("Index");
        }

        //EditCopy
        public ActionResult EditCopy(int? id)
        {
            if (id != null)
            {
                return RedirectToAction("Edit", "Copies", new {id = id.Value});
            }
            return RedirectToAction("Index");
        }

        // GET: Volumes/Delete/5
        //[Route("Delete")]
        //[Route("~/LibraryAdmin/Volumes/Delete")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var volume = _db.Volumes.Find(id);
            if (volume == null)
            {
                return HttpNotFound();
            }
            var titleid = volume.Copy.TitleID;
            var copynumber = volume.Copy.CopyNumber;
            var title = _db.Titles.Find(titleid).Title1;
            ViewBag.Title = "Delete " + _entityName + "?";
            var viewModel = new VolumesEditViewModel
            {
                CopyId = volume.CopyID,
                VolumeId = id.Value,
                Barcode = volume.Barcode,
                LabelText = volume.LabelText,
                RefOnly = volume.RefOnly,
                PrintLabel = volume.PrintLabel,
                Title = title + " - Copy: " + copynumber
            };
            return PartialView(viewModel);
        }

        // POST: Volumes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(VolumesEditViewModel viewModel)
        {
            var volumeid = viewModel.VolumeId;
            var volume = _db.Volumes.Find(volumeid);
            if (volume == null)
            {
                return HttpNotFound();
            }

            //Clear the cache of opac titles if neccessary ...
            if (volume.Copy.StatusType.Opac)
            {
                CacheProvider.RemoveCache("opactitles");
            }

            _db.Volumes.Remove(volume);
            _db.SaveChanges();
            
            return RedirectToAction("Edit", "Copies", new { id = viewModel.CopyId });
        }

        [HttpGet]
        public ActionResult DeleteItem(int id = 0)
        {
            var volume = _db.Volumes.Find(id);
            if (volume == null)
            {
                return HttpNotFound();
            }
            // Create new instance of DeleteConfirmationViewModel and pass it to _DeleteConfirmation Dialog (Reusable)
            DeleteConfirmationViewModel deleteConfirmationViewModel = new DeleteConfirmationViewModel
            {
                DeleteEntityId = id,
                HeaderText = _entityName,
                PostDeleteAction = "Delete",
                PostDeleteController = "Volumes",
                DetailsText = volume.Barcode + ": " + volume.LabelText
            };
            return PartialView("_DeleteConfirmation", deleteConfirmationViewModel);
        }

        [HttpPost]
        public ActionResult DeleteItem(DeleteConfirmationViewModel dcvm)
        {
            // Get item which we need to delete from EntityFramework 
            var volume = _db.Volumes.Find(dcvm.DeleteEntityId);

            if (volume == null)
            {
                return HttpNotFound();
            }

            //Clear the cache of opac titles if neccessary ...
            if (volume.Copy.StatusType.Opac)
            {
                CacheProvider.RemoveCache("opactitles");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _db.Volumes.Remove(volume);
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

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult Autocomplete(string term)
        {
            var volumes = (from v in _db.Volumes
                          where v.Barcode.Contains(term)
                          orderby v.Barcode
                          select new { v.Barcode, v.VolumeID }).Take(10);

            IList<SelectListItem> list = new List<SelectListItem>();

            foreach (var v in volumes)
            {
                list.Add(new SelectListItem { Text = v.Barcode, Value = v.VolumeID.ToString() });
            }

            var result = list.Select(item => new KeyValuePair<string, string>(item.Value.ToString(), item.Text)).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
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