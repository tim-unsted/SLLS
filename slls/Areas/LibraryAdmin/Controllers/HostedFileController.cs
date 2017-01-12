using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using slls.Models;
using slls.ViewModels;
using Westwind.Globalization;

namespace slls.Areas.LibraryAdmin
{
    public class HostedFileController : AdminBaseController
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
            bool success;
            if (viewModel.Files != null)
            {
                if (viewModel.Files.First() != null)
                {
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
                                success = HandleUpload(fileStream: file.InputStream, name: name, type: type, path: path, ext: ext);
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
            return null;
        }

        private bool HandleUpload(Stream fileStream, string name, string type, string ext, string path)
        {
            var handled = false;
            
            try
            {
                //Attempt to compress the file ...
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
                var existingFileName = _db.HostedFiles.FirstOrDefault(f => f.FileName == name);
                while (existingFileName != null)
                {
                    name = name.Replace(ext, "");
                    name = name + "[" + i + "]";
                    name = name + ext;
                    existingFileName = _db.HostedFiles.FirstOrDefault(f => f.FileName == name);
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
                _db.HostedFiles.Add(file);
                handled = (_db.SaveChanges() > 0);

            }
            catch (Exception e)
            {
                // Oops, something went wrong, handle the exception
                ModelState.AddModelError("", e.Message);
                return false;
            }

            return handled;
        }


        private static byte[] Compress(Stream input)
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
            catch (Exception)
            {
                return null;
            }
        }

        //public static double GetFileSize(int FileId)
        //{
        //    return 1234.0;
        //}

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

                //Now delete the file itself ...
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
    }
}