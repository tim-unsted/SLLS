using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using slls.Controllers;
using slls.Models;
using slls.Utils.Helpers;
using slls.ViewModels;
using Westwind.Globalization;

namespace slls.Areas.LibraryAdmin
{
    [AuthorizeRoles(Roles.CatalogueAdmin, Roles.UsersAdmin, Roles.FinanceAdmin, Roles.LoansAdmin, Roles.SerialsAdmin, Roles.OpacAdmin, Roles.BaileyAdmin, Roles.SystemAdmin, Roles.BaileyAdmin)]
    public class HostedFileController : sllsBaseController
    {
        private readonly DbEntities _db = new DbEntities();

        // GET: LibraryAdmin/HostedFile
        public ActionResult Index()
        {
            var files = _db.HostedFiles;
            ViewBag.Title = DbRes.T("Hosted_Files", "EntityType");
            return View(files);
        }

        public ActionResult Add()
        {
            ViewBag.Title = "Upload Files";
            return PartialView();
        }

        [HttpPost]
        public ActionResult PostAdd(UploadFileViewModel viewModel)
        {
            bool success = false;

            if (viewModel.Files == null) return null;
            if (viewModel.Files.First() == null) return null;
            try
            {
                foreach (var file in viewModel.Files.ToList())
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        var name = Path.GetFileName(file.FileName);
                        var type = file.ContentType;
                        var ext = Path.GetExtension(file.FileName);
                        var path = Path.GetFullPath(file.FileName);
                        success =
                            FilesController.UploadFile(fileStream: file.InputStream, name: name, type: type,
                                path: path, ext: ext) > 0;
                    }
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
            }
            return Json(new { success = success });
        }

        


        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var hostedFile = _db.HostedFiles.Find(id);
            if (hostedFile == null)
            {
                return HttpNotFound();
            }

            var viewModel = new HostedFilesViewModel()
            {
                FileId = hostedFile.FileId,
                FileName = hostedFile.FileName,
                Path = hostedFile.Path,
                CreateDate = hostedFile.CreateDate,
                LastUpdateDate = hostedFile.LastUpdateDate,
                InputDate = hostedFile.InputDate,
                TitleLinks = _db.TitleLinks.Count(l => l.FileId == id)
            };

            ViewBag.Title = "Delete " + DbRes.T("Hosted_Files.File", "FieldDisplayName");
            return PartialView(viewModel);
        }

        [HttpPost]
        public ActionResult PostDelete(HostedFilesViewModel viewModel)
        {
            if (viewModel.FileId == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var success = true;
            try
            {
                //First, delete any title links using this file ...
                var titleLinks = _db.TitleLinks.Where(l => l.FileId == viewModel.FileId).ToList();
                foreach (var link in titleLinks)
                {
                    try
                    {
                        _db.TitleLinks.Remove(link);
                        _db.SaveChanges();
                    }
                    catch (Exception)
                    {
                        //ModelState.AddModelError("", e.Message);
                        success = false;
                    }
                }

                //Next, any order links using this file ...
                var orderLinks = _db.OrderLinks.Where(l => l.FileId == viewModel.FileId).ToList();
                foreach (var link in orderLinks)
                {
                    try
                    {
                        _db.OrderLinks.Remove(link);
                        _db.SaveChanges();
                    }
                    catch (Exception)
                    {
                        //ModelState.AddModelError("", e.Message);
                        success = false;
                    }
                }

                //Finally, delete the file itself ...
                try
                {
                    var file = _db.HostedFiles.Find(viewModel.FileId);
                    _db.HostedFiles.Remove(file);
                    _db.SaveChanges();
                }
                catch (Exception)
                {
                    success = false;
                }
                
                return Json(new { success = success });
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
            }

            TempData["ErrorDialogMsg"] = "An error was encountered whilst attempting to delete this file. Please check and try again.";
            return Json(new { success = false });
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