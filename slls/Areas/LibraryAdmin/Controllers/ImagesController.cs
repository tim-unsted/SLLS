using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using slls.App_Settings;
using slls.DAO;
using slls.Models;
using slls.ViewModels;
using Westwind.Globalization;

namespace slls.Areas.LibraryAdmin
{
    public class ImagesController : Controller
    {
        private readonly DbEntities _db = new DbEntities();
        
        // GET: LibraryAdmin/Images
        public ActionResult Index()
        {
            var images = _db.Images;
            
            ViewBag.Title = "Hosted Images";
            return View(images);
        }

        public ActionResult Show(int? id)
        {
            var image = _db.Images.Find(id);
            ViewBag.ImageSrc = "<img src=\"@Url.Action(\"RenderImage\", \"Home\", new { id = " + image.ImageId +
                               ", area = \"\" })\" alt=\"" + image.Name + "\"/>";
            return PartialView(image);
        }

        public ActionResult Add()
        {
            ViewBag.Title = "Upload Images";
            return PartialView();
        }

        public ActionResult RenderImage(int? id)
        {
            var coverImage = _db.Images.Find(id);
            var buffer = coverImage.Image;
            return File(buffer, "image/jpg", string.Format("{0}.jpg", id));
        }

        [HttpPost]
        public ActionResult PostAdd(UploadFileViewModel viewModel)
        {
            bool success = false;

            //Download from Url ...
            if (!string.IsNullOrEmpty(viewModel.Url))
            {
                success = DownloadImageFromUrl(viewModel.Url) > 0;
                return Json(new { success = success });
            }

            //upload from device/local network ...
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
                            UploadImage(fileStream: file.InputStream, name: name, type: type, ext: ext, source: path) > 0;
                    }
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
            }
            return Json(new { success = success });
        }

        public static int DownloadImageFromUrl(string url = "")
        {
            using (var db = new DbEntities())
            {
                using (var client = new WebClient())
                {
                    try
                    {
                        var image = client.DownloadData(url);
                        var uri = new Uri(url);
                        url = uri.GetLeftPart(UriPartial.Path);
                        var fullPath = url;
                        var name = Path.GetFileName(fullPath) ?? "";
                        var ext = Path.GetExtension(fullPath) ?? "";

                        //Check the name of the file - append [n] for each duplicate ...
                        var i = 1;
                        var existingFileName = db.Images.FirstOrDefault(f => f.Source == name);
                        while (existingFileName != null)
                        {
                            name = name.Replace(ext, "");
                            name = name + "[" + i + "]";
                            name = name + ext;
                            existingFileName = db.Images.FirstOrDefault(f => f.Source == name);
                            i++;
                        }

                        var img = new CoverImage
                        {
                            Image = image,
                            Name = string.IsNullOrEmpty(name) ? url : name,
                            Source = url,
                            Size = image.Length,
                            Type = image.GetType().ToString(),
                            InputDate = DateTime.Now
                        };

                        db.Images.Add(img);
                        db.SaveChanges();

                        return img.ImageId;
                    }
                    catch (Exception e)
                    {
                        return 0;
                    }
                }
            }
        }

        public static int UploadImage(Stream fileStream, string name, string type, string ext, string source)
        {
            using (var db = new DbEntities())
            {
                try
                {
                    var existingFile = db.Images.FirstOrDefault(f => f.Source == source);
                    if (existingFile != null)
                    {
                        return existingFile.ImageId;
                    }
                    else
                    {
                        var compressedBytes = new byte[fileStream.Length];

                        //Check the name of the file - append [n] for each duplicate ...
                        var i = 1;
                        var existingFileName = db.Images.FirstOrDefault(f => f.Source == name);
                        while (existingFileName != null)
                        {
                            name = name.Replace(ext, "");
                            name = name + "[" + i + "]";
                            name = name + ext;
                            existingFileName = db.Images.FirstOrDefault(f => f.Source == name);
                            i++;
                        }

                        //Save the file (binary data) to the database ...
                        fileStream.Read(compressedBytes, 0, compressedBytes.Length);
                        var image = new CoverImage()
                        {
                            Image = compressedBytes,
                            Source = source,
                            Name = name,
                            Type = type,
                            Size = compressedBytes.Length/1024,
                            InputDate = DateTime.Now
                        };
                        db.Images.Add(image);
                        db.SaveChanges();

                        //Get the Id of the newly inserted file ...
                        return image.ImageId;
                    }
                }
                catch (Exception e)
                {
                    return 0;
                }
            }
        }

        // GET: Images/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false });
            }
            var image = _db.Images.Find(id);
            if (image == null)
            {
                return Json(new { success = false });
            }

            var viewModel = new EditImageViewModel
            {
                ImageId = image.ImageId,
                ImageName = image.Name,
                IsLogoImage = CssManager.LogoImageId == image.ImageId,
                ImageSrc = "<img src=\"@Url.Action(\"RenderImage\", \"Home\", new { id = " + image.ImageId + ", area = \"\" })\" alt=\"image\"/>"
            };
            ViewBag.Title = "Edit Image";
            return PartialView(viewModel);
        }

        // POST: Images/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditImageViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false });
            }
            var image = _db.Images.Find(viewModel.ImageId);
            if (image == null)
            {
                return Json(new { success = false });
            }
            image.Name = viewModel.ImageName;
            _db.Entry(image).State = EntityState.Modified;
            _db.SaveChanges();

            if (viewModel.IsLogoImage)
            {
                CssManager.LogoImageId = viewModel.ImageId;
            }
            return Json(new { success = true });
        }

        // GET: Images/Delete/5
        public ActionResult Delete(int? id) // id = ImageId
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var image = _db.Images.Find(id);
            if (image == null)
            {
                return HttpNotFound();
            }
            ViewBag.Title = "Confirm Delete " + DbRes.T("Images.Image", "FieldDisplayName");
            return PartialView(image);
        }

        // POST: Images/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id) // id = ImageId
        {
            // Get image which we need to delete from EntityFramework
            var image = _db.Images.Find(id);

            if (image == null)
            {
                return Json(new { success = false }); 
            }

            try
            {
                // Remove any TitleImage records ...
                var titleImages = _db.TitleImages.Where(t => t.ImageId == id);
                if (titleImages.Any())
                {
                    foreach (var item in titleImages.ToList())
                    {
                        _db.TitleImages.Remove(item);
                        _db.SaveChanges();
                    }
                }

                // Remove the image ...
                _db.Images.Remove(image);
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                return Json(new { success = false }); 
                throw;
            }

            return Json(new { success = true }); 
        }
    }
}