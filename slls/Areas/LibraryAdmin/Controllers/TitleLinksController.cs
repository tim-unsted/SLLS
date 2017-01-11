using System;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using slls.Models;
using slls.Utils.Helpers;
using slls.ViewModels;
using Westwind.Globalization;

namespace slls.Areas.LibraryAdmin
{
    public class TitleLinksController : CatalogueBaseController
    {
        private readonly DbEntities _db = new DbEntities();
        private readonly string _entityName = DbRes.T("TitleLinks.Link", "FieldDisplayName");

        public TitleLinksController()
        {
            ViewBag.Title = DbRes.T("TitleLinks", "EntityType");
        }

        // GET: LibraryAdmin/TitleLinks
        public ActionResult Index()
        {
            var titleLinks = _db.TitleLinks;
            return View(titleLinks.ToList());
        }

        // GET: LibraryAdmin/BrokenLinks
        public ActionResult BrokenLinks()
        {
            var titleLinks = _db.TitleLinks.Where(x => x.IsValid == false);
            ViewBag.Title = "Broken " + ViewBag.Title;
            return View(titleLinks.ToList());
        }

        // GET: LibraryAdmin/TitleLinks/Add
        public ActionResult Add(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var title = _db.Titles.Find(id);
            if (title == null)
            {
                return HttpNotFound();
            }

            //Get a list of any hosted (i.e. stored in database) files
            var existingFiles = _db.HostedFiles.OrderBy(f => f.FileName).ToList();

            var viewModel = new TitleImageAddViewModel
            {
                TitleId = title.TitleID,
                Title = title.Title1
            };

            ViewBag.ExistingFile = new SelectList(existingFiles, "FileId", "FileName");
            ViewBag.ExistingFileCount = existingFiles.Count();
            ViewBag.Title = "Add New " + DbRes.T("TitleLinks.Link", "FieldDisplayName");
            return PartialView(viewModel);
        }

        // GET: LibraryAdmin/TitleLinks/Create
        public ActionResult Create()
        {
            var tlvm = new TitleImageAddViewModel
            {
            };

            ViewData["TitleId"] = SelectListHelper.TitlesList();
            ViewBag.Title = "Add New " + DbRes.T("TitleLinks.Link", "FieldDisplayName");
            return PartialView(tlvm);
        }

        // POST: LibraryAdmin/TitleLinks/Create
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult PostCreate(TitleImageAddViewModel viewModel)
        {
            //Check if we've been passed a URL ...
            if (!string.IsNullOrEmpty(viewModel.Url))
            {
                var titleLink = new TitleLink
                {
                    TitleID = viewModel.TitleId,
                    URL = viewModel.Url,
                    DisplayText = viewModel.DisplayText ?? viewModel.Url,
                    HoverTip = viewModel.HoverTip ?? viewModel.Url,
                    Login = viewModel.Login,
                    Password = viewModel.Password,
                    InputDate = DateTime.Now
                };

                _db.TitleLinks.Add(titleLink);
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
                        var titleId = viewModel.TitleId;

                        foreach (var file in viewModel.Files)
                        {
                            if (file != null && file.ContentLength > 0)
                            {
                                var name = Path.GetFileName(file.FileName);
                                FileInfo fi = new FileInfo(file.FileName);
                                var dateCreated = fi.CreationTime;
                                var dateLastUpdate = fi.LastWriteTime;
                                var type = file.ContentType;
                                var ext = fi.Extension;
                                var path = fi.FullName;

                                viewModel.Success = HandleUpload(fileStream: file.InputStream, name: name, type: type, path: path, createDate: dateCreated, lastUpdateDate: dateLastUpdate, titleId: titleId, ext: ext,
                                    displayText: viewModel.DisplayText, hoverTip: viewModel.HoverTip, login: viewModel.Login,
                                    password: viewModel.Password);
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

                var titleLink = new TitleLink()
                {
                    FileId = viewModel.ExistingFile,
                    TitleID = viewModel.TitleId,
                    DisplayText = viewModel.DisplayText ?? file.FileName,
                    HoverTip = viewModel.HoverTip ?? file.FileName,
                    Login = viewModel.Login,
                    Password = viewModel.Password,
                    InputDate = DateTime.Now
                };
                _db.TitleLinks.Add(titleLink);
                _db.SaveChanges();
                return Json(new { success = true });
            }

            return PartialView("Add", viewModel);
        }

        private bool HandleUpload(Stream fileStream, string name, string type, string ext, string path, DateTime createDate, DateTime lastUpdateDate, int titleId, string displayText, string hoverTip, string login, string password)
        {
            var handled = false;

            //Firstly, check to ensure we haven't already got this file by checking the path ...
            var existingFile = _db.HostedFiles.FirstOrDefault(f => f.Path == path);
            if (existingFile != null)
            {
                try
                {
                    //Get the Id of the existing file ...
                    var existingfileId = existingFile.FileId;

                    var titleLink = new TitleLink()
                    {
                        FileId = existingfileId,
                        TitleID = titleId,
                        DisplayText = string.IsNullOrEmpty(displayText) ? name : displayText,
                        HoverTip = string.IsNullOrEmpty(hoverTip) ? name : hoverTip,
                        Login = login,
                        Password = password,
                        InputDate = DateTime.Now
                    };

                    _db.TitleLinks.Add(titleLink);
                    handled = (_db.SaveChanges() > 0);
                }
                catch (Exception)
                {
                    return false;
                }
                return handled;
            }

            //Otherwise ...
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
                    CreateDate = createDate,
                    LastUpdateDate = lastUpdateDate,
                    InputDate = DateTime.Now
                };
                _db.HostedFiles.Add(file);
                _db.SaveChanges();

                //Get the Id of the newly inserted file ...
                var fileId = file.FileId;

                var titleLink = new TitleLink()
                {
                    FileId = fileId,
                    TitleID = titleId,
                    DisplayText = string.IsNullOrEmpty(displayText) ? name : displayText,
                    HoverTip = string.IsNullOrEmpty(hoverTip) ? name : hoverTip,
                    Login = login,
                    Password = password,
                    InputDate = DateTime.Now
                };

                _db.TitleLinks.Add(titleLink);
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



        // GET: LibraryAdmin/TitleLinks/Edit/5  
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var titleLink = _db.TitleLinks.Find(id);
            if (titleLink == null)
            {
                return HttpNotFound();
            }

            var viewModel = new TitleLinksEditViewModel
            {
                TitleLinkId = titleLink.TitleLinkID,
                TitleId = titleLink.TitleID,
                Title = (from t in _db.Titles.Where(t => t.TitleID == titleLink.TitleID)
                         select t.Title1).FirstOrDefault(),
                Url = titleLink.URL,
                FileId = titleLink.FileId,
                FileName = (from f in _db.HostedFiles.Where(x => x.FileId == titleLink.FileId) select f.FileName).FirstOrDefault(),
                DisplayText = titleLink.DisplayText,
                HoverTip = titleLink.HoverTip,
                Password = titleLink.Password,
                Login = titleLink.Login,
                LinkStatus = titleLink.LinkStatus,
                IsValid = titleLink.IsValid
            };

            if (titleLink.FileId > 0)
            {
                viewModel.InfoMsg =
                    "<p><strong>Note: </strong>You cannot edit the " + DbRes.T("TitleLinks.Linked_File", "FieldDisplayName").ToLower() + " here.  If this is incorrect or requires updating, please delete this link and create a new one to the correct, or updated, file.</p>";
            }
            ViewBag.Title = "Edit " + DbRes.T("TitleLinks.Link", "FieldDisplayName");
            return PartialView(viewModel);
        }

        // POST: LibraryAdmin/TitleLinks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TitleLinkID,TitleID,URL,HoverTip,DisplayText,Login,Password,IsValid,LinkStatus")] TitleLinksEditViewModel viewModel)
        {
            if (!ModelState.IsValid) return RedirectToAction("Edit", "Titles", new { id = viewModel.TitleId });
            var titleLink = _db.TitleLinks.Find(viewModel.TitleLinkId);
            if (titleLink == null)
            {
                return HttpNotFound();
            }
            titleLink.TitleLinkID = viewModel.TitleLinkId;
            titleLink.TitleID = viewModel.TitleId;
            titleLink.URL = viewModel.Url;
            titleLink.DisplayText = viewModel.DisplayText;
            titleLink.HoverTip = viewModel.HoverTip;
            titleLink.Login = viewModel.Login;
            titleLink.Password = viewModel.Password;
            titleLink.IsValid = viewModel.IsValid;
            titleLink.LinkStatus = viewModel.LinkStatus;
            titleLink.LastModified = DateTime.Now;

            _db.Entry(titleLink).State = EntityState.Modified;
            _db.SaveChanges();
            return Json(new { success = true });
            //return RedirectToAction("Edit", "Titles", new { id = viewModel.TitleId });
        }

        public ActionResult CheckLinks()
        {
            var titleLinks = _db.TitleLinks;
            foreach (var link in titleLinks.ToList())
            {
                try
                {
                    if (string.IsNullOrEmpty(link.URL))
                    {
                        link.LinkStatus = "The Url cannot be blank";
                        link.IsValid = false;
                    }
                    HttpWebRequest request = HttpWebRequest.Create(link.URL) as HttpWebRequest;
                    request.Timeout = 5000; //set the timeout to 5 seconds to keep the user from waiting too long for the page to load
                    request.Method = "HEAD"; //Get only the header information -- no need to download any content

                    HttpWebResponse response = request.GetResponse() as HttpWebResponse;

                    int statusCode = (int)response.StatusCode;
                    if (statusCode >= 100 && statusCode < 400) //Good requests
                    {
                        link.LinkStatus = "Ok";
                        link.IsValid = true;
                    }
                    else if (statusCode >= 500 && statusCode <= 510) //Server Errors
                    {
                        link.LinkStatus = "The remote server has thrown an internal error. Url is not valid";
                        link.IsValid = false;
                    }
                }
                catch (WebException ex)
                {
                    if (ex.Status == WebExceptionStatus.ProtocolError) //400 errors
                    {
                        link.LinkStatus = "400 errors";
                        link.IsValid = false;
                    }
                    else
                    {
                        link.LinkStatus = String.Format("Unhandled status [{0}] returned for url", ex.Status);
                        link.IsValid = false;
                    }
                }
                catch
                {
                    link.LinkStatus = "Could not test url";
                    link.IsValid = false;
                }

                try
                {
                    if (ModelState.IsValid)
                    {
                        _db.Entry(link).State = EntityState.Modified;
                        _db.SaveChanges();
                    }

                }
                catch
                {
                    _db.Entry(link).State = EntityState.Unchanged;
                }

            }
            return RedirectToAction(_db.TitleLinks.Any(x => x.IsValid == false) ? "BrokenLinks" : "Index");
        }

        //public strin

        [HttpGet]
        public ActionResult Delete(int id = 0)
        {
            var titleLink = _db.TitleLinks.Find(id);
            if (titleLink == null)
            {
                return HttpNotFound();
            }
            // Create new instance of DeleteConfirmationViewModel and pass it to _DeleteConfirmation Dialog (Reusable)
            DeleteConfirmationViewModel deleteConfirmationViewModel = new DeleteConfirmationViewModel
            {
                DeleteEntityId = id,
                HeaderText = DbRes.T("TitleLinks.Link", "FieldDisplayName") + " from " + DbRes.T("Titles.Title", "FieldDisplayName"),
                PostDeleteAction = "Delete",
                PostDeleteController = "TitleLinks",
                FunctionText = "Remove",
                ButtonText = "Remove",
                ConfirmationText = "Are you sure you want to remove the following",
                DetailsText = titleLink.DisplayText
            };
            return PartialView("_DeleteConfirmation", deleteConfirmationViewModel);
        }

        [HttpPost]
        public ActionResult Delete(DeleteConfirmationViewModel dcvm)
        {
            // Get item which we need to delete from EntityFramework 
            var item = _db.TitleLinks.Find(dcvm.DeleteEntityId);

            if (item == null)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _db.TitleLinks.Remove(item);
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
