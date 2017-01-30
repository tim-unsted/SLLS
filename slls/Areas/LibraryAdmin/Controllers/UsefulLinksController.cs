using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;
using slls.DAO;
using slls.Models;
using slls.Utils.Helpers;
using slls.ViewModels;
using Westwind.Globalization;

namespace slls.Areas.LibraryAdmin
{
    public class UsefulLinksController : OpacAdminBaseController
    {
        private readonly DbEntities _db = new DbEntities();
        private readonly string _entityName = DbRes.T("Useful_Links.Useful_Link", "FieldDisplayName");
        private readonly GenericRepository _repository;

        public UsefulLinksController()
        {
            ViewBag.Title = DbRes.T("Useful_Links", "EntityType");
            _repository = new GenericRepository(typeof(UsefulLink));
        }

        // Create a set of simple dictionaries that we can used to fill dropdown lists in views ...
        public Dictionary<string, string> GetTargets()
        {
            return new Dictionary<string, string>
            {
                {"_self", "Opens in current page/tab"},
                {"_blank", "Opens in new page/tab"}
            };
        }

        // GET: LibraryAdmin/UsefulLinks
        public ActionResult Index()
        {
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("opacAdminSeeAlso", ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["controller"].ToString());
            return View(_db.UsefulLinks.ToList());
        }
        
        // GET: LibraryAdmin/UsefulLinks/Create
        public ActionResult Create()
        {
            var viewModel = new LinkedFileAddViewModel()
            {
                AlertText = "Use this form to create a new 'Useful link'. These are seen on the OPAC 'Home' page, if enabled. ",
                PostAction = "PostCreate",
                PostController = "UsefulLinks"
            };
            var existingFiles = _db.HostedFiles.OrderBy(f => f.FileName).ToList();
            ViewBag.ExistingFile = new SelectList(existingFiles, "FileId", "FileName");
            ViewBag.ExistingFileCount = existingFiles.Count();
            ViewBag.Targets = GetTargets();
            ViewBag.Title = "Add New Useful Link";
            return PartialView("AddLink", viewModel);
        }

        // POST: LibraryAdmin/UsefulLinks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PostCreate(LinkedFileAddViewModel viewModel)
        {
            //Check if we've been passed a URL ...
            if (!string.IsNullOrEmpty(viewModel.Url))
            {
                var newUsefulLink = new UsefulLink
                {
                    Url = viewModel.Url,
                    DisplayText = viewModel.DisplayText ?? viewModel.Url,
                    HoverTip = viewModel.HoverTip ?? viewModel.Url,
                    Target = "_blank",
                    Enabled = true,
                    //Login = viewModel.Login,
                    //Password = viewModel.Password,
                    InputDate = DateTime.Now
                };

                _db.UsefulLinks.Add(newUsefulLink);
                _db.SaveChanges();
                return Json(new { success = true });
            }
            //Otherwise, check if we've been passed any new files ...
            if (viewModel.Files != null)
            {
                if (viewModel.Files.First() != null)
                {
                    try
                    {
                        //var titleId = viewModel.TitleId;

                        foreach (var file in viewModel.Files)
                        {
                            if (file != null && file.ContentLength > 0)
                            {
                                var name = Path.GetFileName(file.FileName);
                                var type = file.ContentType;
                                var ext = Path.GetExtension(file.FileName);
                                var path = Path.GetFullPath(file.FileName);
                                var fileId = HostedFileController.UploadFile(fileStream: file.InputStream, name: name, type: type, ext: ext, path: path);

                                if (fileId != 0)
                                {
                                    var usefulLink = new UsefulLink()
                                    {
                                        FileId = fileId,
                                        DisplayText = viewModel.DisplayText ?? name,
                                        HoverTip = viewModel.HoverTip ?? name,
                                        //Login = viewModel.Login,
                                        //Password = viewModel.Password,
                                        InputDate = DateTime.Now
                                    };
                                    _db.UsefulLinks.Add(usefulLink);
                                    _db.SaveChanges();
                                }
                                viewModel.Success = true;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        ModelState.AddModelError("", e.Message);
                    }
                    return Json(new { success = true });
                }
            }
            if (viewModel.ExistingFile != 0)
            {
                var file = _db.HostedFiles.Find(viewModel.ExistingFile);
                if (file == null) return null;

                var usefulLink = new UsefulLink()
                {
                    FileId = viewModel.ExistingFile,
                    DisplayText = viewModel.DisplayText ?? file.FileName,
                    HoverTip = viewModel.HoverTip ?? file.FileName,
                    //Login = viewModel.Login,
                    //Password = viewModel.Password,
                    InputDate = DateTime.Now
                };
                _db.UsefulLinks.Add(usefulLink);
                _db.SaveChanges();
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }
            

        // GET: LibraryAdmin/UsefulLinks/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var usefulLink = _db.UsefulLinks.Find(id);
            if (usefulLink == null)
            {
                return HttpNotFound();
            }

            var viewModel = new UsefulLinksAddEditViewModel()
            {
                LinkID = usefulLink.LinkID,
                Url = usefulLink.Url,
                FileId = usefulLink.FileId,
                FileName = (from f in _db.HostedFiles.Where(x => x.FileId == usefulLink.FileId) select f.FileName).FirstOrDefault(),
                DisplayText = usefulLink.DisplayText,
                HoverTip = usefulLink.HoverTip,
                Target = usefulLink.Target,
                Enabled = usefulLink.Enabled
            };

            if (usefulLink.FileId > 0)
            {
                viewModel.InfoMsg =
                    "<p><strong>Note: </strong>You cannot edit the " + DbRes.T("Links.Linked_File", "FieldDisplayName").ToLower() + " here.  If this is incorrect or requires updating, please delete this link and create a new one to the correct, or updated, file.</p>";
            }
            ViewBag.Targets = GetTargets();
            ViewBag.Title = "Edit Useful Link";
            return PartialView(viewModel);
        }

        // POST: LibraryAdmin/UsefulLinks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UsefulLinksAddEditViewModel viewModel)
        {
            if (!ModelState.IsValid) return View(viewModel);
            var usefulLink = _db.UsefulLinks.Find(viewModel.LinkID);
            if (usefulLink == null)
            {
                return HttpNotFound();
            }
            usefulLink.Url = viewModel.Url;
            usefulLink.DisplayText = viewModel.DisplayText;
            usefulLink.HoverTip = viewModel.HoverTip;
            usefulLink.Target = viewModel.Target;
            usefulLink.Enabled = viewModel.Enabled;
            usefulLink.LastModified = DateTime.Now;

            _db.Entry(usefulLink).State = EntityState.Modified;
            _db.SaveChanges();
            return Json(new { success = true });
        }

        [HttpGet]
        public ActionResult Delete(int id = 0)
        {
            var usefulLink = _repository.GetById<UsefulLink>(id);
            if (usefulLink == null)
            {
                return HttpNotFound();
            }

            if (usefulLink.Deleted)
            {
                return HttpNotFound();
            }
            
            // Create new instance of DeleteConfirmationViewModel and pass it to _DeleteConfirmation Dialog (Reusable)
            var deleteConfirmationViewModel = new DeleteConfirmationViewModel
            {
                DeleteEntityId = id,
                HeaderText = _entityName,
                PostDeleteAction = "Delete",
                PostDeleteController = "UsefulLinks",
                DetailsText = usefulLink.DisplayText
            };
            return PartialView("_DeleteConfirmation", deleteConfirmationViewModel);
        }

        [HttpPost]
        public ActionResult Delete(DeleteConfirmationViewModel dcvm)
        {
            // Get item which we need to delete from EntityFramework
            var item = _repository.GetById<UsefulLink>(dcvm.DeleteEntityId);

            if (item == null)
            {
                return HttpNotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //_db.UsefulLinks.Remove(item);
                    //_db.SaveChanges();
                    _repository.Delete(item);
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
