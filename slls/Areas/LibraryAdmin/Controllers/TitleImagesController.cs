﻿using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web.Mvc;
using slls.App_Settings;
using slls.Controllers;
using slls.DAO;
using slls.Hubs;
using slls.Models;
using slls.ViewModels;
using Westwind.Globalization;

namespace slls.Areas.LibraryAdmin
{
    public class TitleImagesController : CatalogueBaseController
    {
        private readonly DbEntities _db = new DbEntities();
        private readonly GenericRepository _repository;

        public TitleImagesController()
        {
            _repository = new GenericRepository(typeof(TitleImage));
        }


        public ActionResult Add(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var allSources = ConfigurationManager.AppSettings["AutoCatImageSources"];
            var sources = allSources.Split(',').ToList();

            //Get a list of all hosted (i.e. stored in database) images
            var existingImages = _db.Images.OrderBy(f => f.Source).ToList();

            var title = _db.Titles.Find(id);
            var viewModel = new LinkedFileAddViewModel
            {
                TitleId = id.Value,
                Title = title.Title1,
                Isbn = title.ISBN13 ?? title.ISBN10,
                Sources = sources,
                HasAutocat = true
            };

            ViewBag.ExistingImage = new SelectList(existingImages, "ImageId", "Source");
            ViewBag.ExistingImageCount = existingImages.Count();
            ViewBag.Title = "Add New " + DbRes.T("Images.Image", "FieldDisplayName");
            return PartialView(viewModel);
        }


        [HttpPost]
        public ActionResult Add(LinkedFileAddViewModel viewModel)
        {
            if (!string.IsNullOrEmpty(viewModel.Url))
            {
                //Download the image from the UTL and return the new ImageID ...
                var imageId = FilesController.DownloadImageFromUrl(viewModel.Url);

                if (imageId != 0)
                {
                    var titleImage = new TitleImage
                    {
                        ImageId = imageId,
                        TitleId = viewModel.TitleId,
                        Alt = viewModel.Title,
                        HoverText = viewModel.Title,
                        IsPrimary = false,
                        InputDate = DateTime.Now
                    };

                    _db.TitleImages.Add(titleImage);
                    _db.SaveChanges();

                    CheckForPrimaryImage(viewModel.TitleId);

                    return Json(new { success = true });
                }
                return Json(new { success = false });
            }

            //Upload images from the device/local network ...
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
                                var ext = Path.GetExtension(file.FileName);
                                var type = file.ContentType;
                                var imageId = FilesController.UploadImage(fileStream: file.InputStream, name: name, type: type, ext: ext, source: name);

                                var titleImage = new TitleImage
                                {
                                    ImageId = imageId,
                                    TitleId = viewModel.TitleId,
                                    Alt = viewModel.Title,
                                    HoverText = viewModel.Title,
                                    IsPrimary = viewModel.IsPrimary,
                                    InputDate = DateTime.Now
                                };
                                _db.TitleImages.Add(titleImage);
                                _db.SaveChanges();
                            }
                        }
                        CheckForPrimaryImage(viewModel.TitleId);
                        return Json(new { success = true });
                    }
                    catch (Exception e)
                    {
                        ModelState.AddModelError("", e.Message);
                        return Json(new { success = false });
                    }
                    return Json(new { success = false });
                }
            }

            // Use an existing image already in the database ...
            if (viewModel.ExistingImage != 0)
            {
                try
                {
                    var image = _db.Images.Find(viewModel.ExistingImage);
                    if (image == null) return null;

                    var titleImage = new TitleImage
                    {
                        ImageId = image.ImageId,
                        TitleId = viewModel.TitleId,
                        Alt = viewModel.Title,
                        HoverText = viewModel.Title,
                        IsPrimary = false,
                        InputDate = DateTime.Now
                    };

                    _db.TitleImages.Add(titleImage);
                    _db.SaveChanges();

                    CheckForPrimaryImage(viewModel.TitleId);
                    return Json(new { success = true });
                }
                catch (Exception)
                {
                    return Json(new { success = false });
                    throw;
                }
                return Json(new { success = false });
            }
            return Json(new { success = false });
        }

        //This is using AutoCat ...
        public ActionResult DownloadImageFromAutoCat(LinkedFileAddViewModel viewModel, string source)
        {
            try
            {
                var isbn = viewModel.Isbn;
                var url = "";

                if (isbn == null)
                {
                    Response.StatusCode = 500;
                    return Json(new { message = "AutoCat requires an ISBN to look up data. This item does not have an ISBN!\nPlease check." }, JsonRequestBehavior.AllowGet);
                }

                url = AutoCat.AutoCat.GetImageUrl(source, isbn);

                //Check that we haven't got a 404 or invalid page error
                //if (url.IndexOf("Error", StringComparison.Ordinal) != -1)
                if (url == null)
                {
                    Response.StatusCode = 500;
                    return Json(new { message = "AutoCat cannot find an image using this item's ISBN.\n\nPlease check that the ISBN is correct." }, JsonRequestBehavior.AllowGet);
                }

                //We've got a URL, now download the image ...
                var imageId = FilesController.DownloadImageFromUrl(url);

                if (imageId > 0)
                {
                    var titleImage = new TitleImage
                    {
                        ImageId = imageId,
                        TitleId = viewModel.TitleId,
                        Alt = viewModel.Title,
                        HoverText = viewModel.Title,
                        IsPrimary = false,
                        InputDate = DateTime.Now
                    };

                    _db.TitleImages.Add(titleImage);
                    _db.SaveChanges();

                    //Check that there is a primary image ...
                    CheckForPrimaryImage(viewModel.TitleId);

                    return Json(new
                    {
                        redirectUrl = Url.Action("Edit", "Titles", new { id = viewModel.TitleId }),
                        isRedirect = true
                    });
                }

            }
            catch (Exception e)
            {
                Response.StatusCode = 500;
                return Json(new { message = e.Message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new
            {
                redirectUrl = Url.Action("Edit", "Titles", new { id = viewModel.TitleId }),
                isRedirect = true
            });
        }


        public void CheckForPrimaryImage(int titleid = 0)
        {
            var titleImages = _db.TitleImages.Where(i => i.TitleId == titleid);
            if (!titleImages.Any()) return;
            var primaryImage = titleImages.Where(i => i.IsPrimary);
            if (primaryImage.Any()) return;
            var newPrimaryImage = titleImages.FirstOrDefault();
            if (newPrimaryImage != null) newPrimaryImage.IsPrimary = true;
            _repository.Update(newPrimaryImage);
        }


        public ActionResult DownloadImagesAutoCat()
        {
            //var titlesWithIsbn = (from t in _db.Titles where t.TitleImages.Any() == false && (t.ISBN10 != null || t.ISBN13 != null) select t).Count();
            var allSources = ConfigurationManager.AppSettings["AutoCatImageSources"];
            ViewBag.Title = "Download Images With AutoCat";
            var viewModel = new DownloadImagesViewmodel
            {
                CountAvailable = (from t in _db.Titles where t.TitleImages.Any() == false && (t.ISBN10 != null || t.ISBN13 != null) select t).Count(),
                CountNoImage = (from t in _db.Titles where t.TitleImages.Any() == false select t).Count(),
                CountNoIsbn = (from t in _db.Titles where (t.ISBN10 != null || t.ISBN13 != null) select t).Count(),
                Who = System.Web.HttpContext.Current.User.Identity.Name,
                Sources = allSources.Split(',').ToList().Select(x => new SelectListItem
                {
                    Selected = true,
                    Text = x.ToString(),
                    Value = x.ToString()
                })
            };

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult PostDownloadImagesAutoCat(DownloadImagesViewmodel viewModel)
        {
            var titlesWithIsbn = from t in _db.Titles where (t.ISBN10 != null) || (t.ISBN13 != null) select t;
            var titlesNoImage = from t in titlesWithIsbn
                                where t.TitleImages.Any() == false
                                select t;

            double startCount = titlesNoImage.Count();
            double progress = 0;
            var downloadCount = 0;

            ProgressHub.NotifyStart("Started. Looking for images to download ...", viewModel.Who);

            foreach (var title in titlesNoImage.ToList())
            {
                progress++;
                var isbn = title.ISBN13 ?? title.ISBN10;

                if (string.IsNullOrEmpty(isbn) == false)
                {
                    //Get an image url from selected sources
                    foreach (var source in viewModel.SelectedSources)
                    {
                        var url = AutoCat.AutoCat.GetImageUrl(source, isbn);

                        if (url != null)
                        {
                            var imageId = FilesController.DownloadImageFromUrl(url);

                            if (imageId > 0)
                            {
                                var titleImage = new TitleImage
                                    {
                                        ImageId = imageId,
                                        TitleId = title.TitleID,
                                        Alt = title.Title1,
                                        HoverText = title.Title1,
                                        IsPrimary = title.TitleImages.Count == 0,
                                        InputDate = DateTime.Now
                                    };

                                _db.TitleImages.Add(titleImage);
                                _db.SaveChanges();
                            }
                        }
                        downloadCount++;
                        ProgressHub.SendMessage("Working ... " + downloadCount + " images downloaded.", viewModel.Who);
                        break;
                    }
                }
                var progressPercent = (progress / startCount) * 100;
                ProgressHub.NotifyProgress((int)progressPercent + "%", (int)progressPercent, viewModel.Who);
                //set a 1 second delay on each loop ...
                Thread.Sleep(1000);
            }

            string msg = "";
            if (downloadCount == 0)
            {
                msg = "Finished. No images were found! The ISBNs may be incorrect or there may be no image held at the source. Check the ISBNs or add the images manually.";
            }
            else
            {
                msg = "Finished! A total of " + downloadCount + " images downloaded.";
            }

            ProgressHub.NotifyEnd(msg, viewModel.Who);

            return null;
        }



        [HttpPost]
        public ActionResult ProcessApplication([Bind(Prefix = "id")] string taskId)
        {
            ProgressHub.NotifyStart("initializing and preparing", taskId);

            // A bit of work
            Thread.Sleep(2000);
            ProgressHub.NotifyProgress("10%", 10, taskId);
            ProgressHub.SendMessage("3 titles processed", taskId);

            // A bit of work 
            Thread.Sleep(1000);
            ProgressHub.NotifyProgress("20%", 20, taskId);
            ProgressHub.SendMessage("5 titles processed", taskId);

            // A bit of work        
            Thread.Sleep(1000);
            ProgressHub.NotifyProgress("30%", 30, taskId);
            ProgressHub.SendMessage("11 titles processed", taskId);

            // A bit of work
            Thread.Sleep(1000);
            ProgressHub.NotifyProgress("40%", 40, taskId);
            ProgressHub.SendMessage("18 titles processed", taskId);

            // A bit of work
            Thread.Sleep(1000);
            ProgressHub.NotifyProgress("50%", 50, taskId);
            ProgressHub.SendMessage("Half way there!", taskId);

            // A bit of work
            Thread.Sleep(1000);
            ProgressHub.NotifyProgress("60%", 60, taskId);
            ProgressHub.SendMessage("32 titles processed", taskId);

            // A bit of work
            Thread.Sleep(1000);
            ProgressHub.NotifyProgress("70%", 70, taskId);
            ProgressHub.SendMessage("38 titles processed", taskId);

            Thread.Sleep(1000);
            ProgressHub.NotifyProgress("80%", 80, taskId);
            ProgressHub.SendMessage("43 titles processed", taskId);

            // A bit of work
            Thread.Sleep(1000);
            ProgressHub.NotifyProgress("90%", 90, taskId);
            ProgressHub.SendMessage("53 titles processed", taskId);

            // A bit of work
            Thread.Sleep(1000);
            ProgressHub.NotifyProgress("100%", 100, taskId);
            ProgressHub.SendMessage("60 titles processed", taskId);

            //Final piece of work
            Thread.Sleep(2000);
            ProgressHub.NotifyEnd("Finished!", taskId);


            return null;
        }



        public ActionResult Show(int? id)
        {
            var titleImages = _db.TitleImages.Find(id);
            return PartialView(titleImages);
        }


        //private byte[] LoadImage(int id, out string type)
        //{
        //    byte[] fileBytes = null;
        //    string fileType = null;

        //    var titleImage = _db.TitleImages.FirstOrDefault(doc => doc.ImageId == id);
        //    if (titleImage != null)
        //    {
        //        fileBytes = titleImage.Image;
        //        fileType = titleImage.Type;
        //    }

        //    type = fileType;
        //    return fileBytes;
        //}


        public ActionResult List(int id = 0) //id = TitleID
        {
            var title = _db.Titles.Find(id);
            var viewModel = title.TitleImages
                .Select(titleImage => new LinkedFileListViewmodel
                {
                    TitleImageId = titleImage.TitleImageId,
                    TitleId = titleImage.TitleId,
                    ImageId = titleImage.ImageId,
                    Alt = titleImage.Alt,
                    HoverText = titleImage.HoverText,
                    IsPrimary = titleImage.IsPrimary
                }).ToList();

            return PartialView(viewModel);
        }

        // GET: LibraryAdmin/TitleImages/Edit/5
        public ActionResult Edit(int? id) // id = TitleImageId
        {
            var titleImage = _db.TitleImages.Find(id);

            if (id != null)
            {
                var viewModel = new LinkedFileEditViewModel
                {
                    ImageId = titleImage.ImageId,
                    Title = titleImage.Title.Title1,
                    Alt = titleImage.Alt,
                    HoverText = titleImage.HoverText,
                    IsPrimary = titleImage.IsPrimary,
                    TitleId = titleImage.TitleId,
                    TitleImageId = titleImage.TitleImageId
                };
                ViewBag.Title = "Edit " + DbRes.T("Titles.Title", "FieldDisplayName") + " " + DbRes.T("Images.Image", "FieldDisplayName") + " Details";
                return PartialView(viewModel);
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        // POST: LibraryAdmin/TitleImages/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(LinkedFileEditViewModel viewModel)
        {
            //Mark all images as Primary = false if we're passing Primary = true; we'll set the value to true for just the image passed
            if (viewModel.IsPrimary)
            {
                _db.TitleImages
              .Where(i => i.TitleId == viewModel.TitleId)
              .ToList()
              .ForEach(x => x.IsPrimary = false);
                _db.SaveChanges();
            }

            var titleImage = _repository.GetById<TitleImage>(viewModel.TitleImageId);

            titleImage.TitleId = viewModel.TitleId;
            titleImage.ImageId = viewModel.ImageId;
            titleImage.Alt = viewModel.Alt;
            titleImage.HoverText = viewModel.HoverText;
            titleImage.IsPrimary = viewModel.IsPrimary;

            if (ModelState.IsValid)
            {
                _repository.Update(titleImage);
                return Json(new { success = true });
                //return RedirectToAction("Edit", "Titles", new { id = titleImage.TitleId });

            }
            return PartialView(viewModel);
        }

        // GET: TitleImages/Delete/5
        public ActionResult Delete(int? id) // id = TitleImageId
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var titleImage = _db.TitleImages.Find(id);
            if (titleImage == null)
            {
                return HttpNotFound();
            }
            ViewBag.Title = "Confirm Delete " + DbRes.T("Images.Image", "FieldDisplayName");
            return PartialView(titleImage);
        }

        // POST: Volumes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id) // id = TitleImageId
        {
            // Get item which we need to delete from EntityFramework
            var titleImage = _db.TitleImages.Find(id);
            var titleId = titleImage.TitleId;

            if (!ModelState.IsValid) return RedirectToAction("Edit", "Titles", new { id = titleImage.TitleId }); ;
            try
            {
                _db.TitleImages.Remove(titleImage);
                _db.SaveChanges();

                //Check that there is a primary image ...
                //CheckForPrimaryImage(titleId);

                //return RedirectToAction("Edit", "Titles", new { id = titleImage.TitleId });
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
            }
            //return PartialView(titleImage);
            return RedirectToAction("Edit", "Titles", new { id = titleImage.TitleId });
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