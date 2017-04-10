using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using slls.App_Settings;
using slls.Controllers;
using slls.DAO;
using slls.Models;
using slls.ViewModels;
using Westwind.Globalization;

namespace slls.Areas.LibraryAdmin
{
    public class ImagesController : AdminBaseController
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
            ViewBag.ImageSrc = "<img src=\"@Url.Action(\"RenderImage\", \"Files\", new { id = " + image.ImageId +
                               ", area = \"\" })\" alt=\"" + image.Name + "\"/>";
            return PartialView(image);
        }

        public ActionResult Add()
        {
            ViewBag.Title = "Upload Images";
            return PartialView();
        }
        
        [HttpPost]
        public ActionResult PostAdd(UploadFileViewModel viewModel)
        {
            bool success = false;

            //Download from Url ...
            if (!string.IsNullOrEmpty(viewModel.Url))
            {
                success = FilesController.DownloadImageFromUrl(viewModel.Url) > 0;
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
                            FilesController.UploadImage(fileStream: file.InputStream, name: name, type: type, ext: ext, source: path) > 0;
                    }
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
            }
            return Json(new { success = success });
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
                ImageSrc = "<img src=\"@Url.Action(\"RenderImage\", \"Files\", new { id = " + image.ImageId + ", area = \"\" })\" alt=\"image\"/>"
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

            //if (viewModel.IsLogoImage)
            //{
            //    Settings.UpdateParameter("Styling.LogoImageID", viewModel.ImageId.ToString());
            //    CssManager.LogoImageId = viewModel.ImageId;
            //}
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
            catch (Exception)
            {
                return Json(new { success = false }); 
                throw;
            }

            return Json(new { success = true }); 
        }
    }
}