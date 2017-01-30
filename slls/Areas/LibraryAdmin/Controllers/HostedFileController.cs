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
                            UploadFile(fileStream: file.InputStream, name: name, type: type,
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

        public static int UploadFile(Stream fileStream, string name, string type, string ext, string path)
        {
            try
            {
                using (var db = new DbEntities())
                {
                    var existingFile = db.HostedFiles.FirstOrDefault(f => f.Path == path);
                    if (existingFile != null)
                    {
                        return existingFile.FileId;
                    }
                    else
                    {
                        var compressedBytes = Compress(fileStream);
                        bool compressed;
                        if (compressedBytes != null)
                        {
                            compressed = true;
                        }
                        else
                        {
                            compressedBytes = new byte[fileStream.Length];
                            compressed = false;
                        }

                        //Check the name of the file - append [n] for each duplicate ...
                        var i = 1;
                        var existingFileName = db.HostedFiles.FirstOrDefault(f => f.FileName == name);
                        while (existingFileName != null)
                        {
                            name = name.Replace(ext, "");
                            name = name + "[" + i + "]";
                            name = name + ext;
                            existingFileName = db.HostedFiles.FirstOrDefault(f => f.FileName == name);
                            i++;
                        }

                        //Save the file (binary data) to the database ...
                        fileStream.Read(compressedBytes, 0, compressedBytes.Length);
                        var file = new HostedFile
                        {
                            Data = compressedBytes,
                            FileName = name,
                            FileExtension = ext,
                            Compressed = compressed,
                            SizeStored = compressedBytes.Length / 1024,
                            Path = path,
                            InputDate = DateTime.Now
                        };
                        db.HostedFiles.Add(file);
                        db.SaveChanges();

                        //Get the Id of the newly inserted file ...
                        var fileId = file.FileId;
                        return fileId;
                    }
                }
            }
            catch (Exception e)
            {
                return 0;
            }
        }

        public static byte[] Compress(Stream input)
        {
            try
            {
                using (var compressStream = new MemoryStream())
                using (var compressor = new DeflateStream(compressStream, CompressionMode.Compress))
                {
                    input.CopyTo(compressor);
                    compressor.Close();
                    return compressStream.ToArray();
                }
            }
            catch (Exception e)
            {
                return null;
            }
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
                    catch (Exception e)
                    {
                        ModelState.AddModelError("", e.Message);
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
                    catch (Exception e)
                    {
                        ModelState.AddModelError("", e.Message);
                    }
                }

                //Finally, delete the file itself ...
                var file = _db.HostedFiles.Find(viewModel.FileId);
                _db.HostedFiles.Remove(file);
                _db.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
            }

            return null;
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