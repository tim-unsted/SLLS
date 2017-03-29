using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls.Expressions;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using slls.App_Settings;
using slls.Controllers;
using slls.DAO;
using slls.Models;
using slls.Utils;
using slls.Utils.Helpers;
using slls.ViewModels;
using Westwind.Globalization;

namespace slls.Areas.LibraryAdmin
{
    public class CirculationController : SerialsBaseController
    {
        private readonly DbEntities _db = new DbEntities();
        private readonly GenericRepository _repository;
        private readonly string _entityName = DbRes.T("Circulation.Circulation", "FieldDisplayName");

        public CirculationController()
        {
            _repository = new GenericRepository(typeof(Circulation));
            ViewBag.Title = DbRes.T("Circulation", "EntityType");
        }

        // GET: LibraryAdmin/Circulations
        public ActionResult Index()
        {
            var circulatedCopies = _db.Copies.Include(c => c.Title).Where(c => c.Circulated);
            ViewBag.Title = DbRes.T("Circulation.Circulated_Items", "FieldDisplayName");
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("circulationSeeAlso",
                ControllerContext.RouteData.Values["action"].ToString(), null, "sortOrder");
            return View(circulatedCopies.ToList());
        }


        public ActionResult CirculationList(int id = 0)
        {
            IEnumerable<Circulation> circulationList = from c in _db.Circulations where c.CopyID == id select c;

            var viewModel = new CirculationListViewModel()
            {
                CirculationList = circulationList
            };

            if (id > 0)
            {
                var copy = _db.Copies.Find(id);
                viewModel.SelectCopy = copy.Title.Title1 + " - Copy: " + copy.CopyNumber;
            }
            
            //ViewData["Copy"] = allCirculatedItems;
            ViewData["CopyID"] = id;
            ViewBag.Title = DbRes.T("Circulation.Circulation_List", "FieldDisplayName") + " By Item";
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("circulationSeeAlso", ControllerContext.RouteData.Values["action"].ToString(), null, "sortOrder");
            return View(viewModel);
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SelectCirculatedCopies(string term)
        {
            //if (term.Length < 3) return null;

            term = " " + term;
            var titles = (from c in _db.vwSelectCirculatedCopies
                          where c.Title.Contains(term)
                          orderby c.Title.Substring(c.NonFilingChars)
                          select new { CopyId = c.CopyId, TitleId = c.TitleId, Title = c.Title, CopyNumber = c.CopyNumber, Year = c.Year, Edition = c.Edition, AuthorString = c.AuthorString }).Take(250);

            return Json(titles, JsonRequestBehavior.AllowGet);
        }


        public ActionResult CirculationByUser(string selectedUser)
        {
            //Get any items circulated to the selected user ...
            var circulationList = _db.Circulations.Where(c => c.RecipientUser.Id == selectedUser && c.Copy.Circulated);

            //Get some more details about the selected user, if any ...
            if (!string.IsNullOrEmpty(selectedUser))
            {
                var recipient = _db.Users.Find(selectedUser);
                ViewData["UserName"] = recipient.Firstname ?? recipient.Lastname;
            }
            ViewData["SelectedUser"] = Utils.Helpers.SelectListHelper.SelectUsersByLastname();
            ViewData["UserID"] = selectedUser;
            ViewBag.Title = ViewBag.Title + " By " + DbRes.T("Circulation.Recipient", "FieldDisplayName");
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("circulationSeeAlso",
                ControllerContext.RouteData.Values["action"].ToString(), null, "sortOrder");
            return View(circulationList);
        }


        public ActionResult AddItemToRecipientsList(string id)
        {
            var recipient = _db.Users.Find(id);
            if (recipient == null)
            {
                return HttpNotFound();
            }
            var lbvm = new ListBoxViewModel
            {
                PostSelectController = "Circulation",
                PostSelectAction = "PostAddItemToRecipientsList",
                SelectedItems = null,
                HeaderText =
                    "Add " + recipient.Firstname + " to more " +
                    DbRes.T("Circulation.Circulation_Lists", "FieldDisplayName"),
                DetailsText =
                    "Select the " + DbRes.T("Circulation.Circulation_lists", "FieldDisplayName") + " you wish to add " +
                    recipient.Firstname.Trim() + " to",
                SelectLabel = DbRes.T("Circulation.Circulated_Items", "FieldDisplayName"),
                OkButtonText = "Add Selected",
                PostSelectIdString = recipient.Id
            };

            lbvm.AvailableItems = _db.Copies.Where(c => c.Circulated)
                .Select(x => new SelectListItem
                {
                    Value = x.CopyID.ToString(),
                    Text =
                        x.Title.Title1.Substring(x.Title.NonFilingChars) + " (Copy: " + x.CopyNumber.ToString() + "; - " +
                        x.Location.Location1 + ")"
                }).OrderBy(c => c.Text)
                .ToList();

            ViewBag.Title = "Add " + recipient.Firstname + " to more " +
                            DbRes.T("Circulation.Circulation_Lists", "FieldDisplayName");
            return PartialView("_MultiSelectListBox", lbvm);
        }

        [HttpPost]
        public ActionResult PostAddItemToRecipientsList(ListBoxViewModel lbvm)
        {
            var recipient = _db.Users.Find(lbvm.PostSelectIdString);
            if (recipient == null)
            {
                return HttpNotFound();
            }
            foreach (var copyId in lbvm.SelectedItems)
            {
                var copy = _db.Copies.Find(Convert.ToInt32(copyId));
                if (copy != null)
                {
                    var circulation =
                        _db.Circulations.Where(c => c.CopyID == copy.CopyID && c.RecipientUser.Id == recipient.Id);
                    if (circulation.Any()) continue;
                    var sortOrder = _db.Circulations.Where(x => x.CopyID == copy.CopyID).Max(x => x.SortOrder) ?? 0;
                    var newCirculation = new Circulation
                    {
                        CopyID = copy.CopyID,
                        //Id = recipient.Id,
                        RecipientUser = recipient,
                        SortOrder = sortOrder + 1,
                        InputDate = DateTime.Now
                    };
                    _db.Circulations.Add(newCirculation);
                    _db.SaveChanges();
                }
            }
            //return RedirectToAction("CirculationByUser", new { SelectedUser = recipient.Id });
            return Json(new { success = true }); 
        }


        [HttpGet]
        public ActionResult CloneUsersList(string id)
        {
            var libraryUser = _db.Users.Find(id);
            if (libraryUser == null)
            {
                return HttpNotFound();
            }

            var viewModel = new SelectPopupViewModel
            {
                PostSelectController = "Circulation",
                PostSelectAction = "CloneUsersList",
                SelectedItem = "0",
                HeaderText = "Clone an Existing " + DbRes.T("Circulation.Recipient", "FieldDisplayName") + "'s List",
                DetailsText = "",
                SelectLabel = "",
                SelectText = "",
                OkButtonText = "Clone Selected",
                PostSelectIdString = libraryUser.Id
            };

            viewModel.AvailableItems =
                _db.Users.Where(u => u.Id != libraryUser.Id && u.Circulations.Any() && u.IsLive && u.CanDelete && u.Lastname != null)
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id,
                        Text = x.Lastname + ", " + x.Firstname + " (" + x.Circulations.Count.ToString() + " Lists)"
                    }).OrderBy(c => c.Text)
                    .ToList();

            List<SelectListItem> cloneOptions = new List<SelectListItem>();
            cloneOptions.Add(new SelectListItem { Text = "Append", Value = "a" });
            cloneOptions.Add(new SelectListItem { Text = "Overwrite", Value = "o" });
            viewModel.AdditionalOptions = cloneOptions;
            ViewBag.Title = viewModel.HeaderText;
            return PartialView("CloneRecipient", viewModel);
        }

        [HttpPost]
        public ActionResult CloneUsersList(SelectPopupViewModel viewModel)
        {
            //Check the details of the select user ...
            var libraryUser = _db.Users.Find(viewModel.SelectedItemString);
            if (libraryUser == null)
            {
                return HttpNotFound();
            }
            var success = true;

            //Remove all current items if the post option is "overwrite"
            if (viewModel.SelectedOption == "o")
            {
                IEnumerable<Circulation> circulations = _db.Circulations.Where(c => c.RecipientUser.Id == viewModel.PostSelectIdString);
                foreach (var circulation in circulations.ToList())
                {
                    var copyId = circulation.CopyID;
                    _db.Circulations.Remove(circulation);
                    _db.SaveChanges();
                    ReSortRecipients(copyId.Value);
                }
            }

            //Get a list of all circulation items for the selected user (i.e. the one to clone) ...
            var cloned = _db.Circulations.Where(c => c.RecipientUser.Id == libraryUser.Id);
            if (!cloned.Any())
            {
                return Json(new { success = false });
            }

            //Get a list of the current items for the user we are adding to ...
            var existing = _db.Circulations.Where(c => c.RecipientUser.Id == viewModel.PostSelectIdString);

            //Get a string/list of current items for the user we are adding to - we can use this to check we are not duplicating items when cloning ...
            var existingItems = new List<string>();
            foreach (var item in existing)
            {
                existingItems.Add(item.CopyID.ToString());
            }

            //Loop through the list of cloned items and add each item to the current user (PostSelectId) if it doesn't already exist ...
            foreach (var item in cloned.ToList())
            {
                //Check if we already have this item in the list ...
                if (existingItems.Contains(item.CopyID.ToString())) continue;
                var newCirculation = new Circulation
                {
                    //Id = viewModel.PostSelectIdString,
                    RecipientUser = _db.Users.Find(viewModel.PostSelectIdString),
                    CopyID = item.CopyID,
                    SortOrder = item.Copy.Circulations.Max(c => c.SortOrder) + 1,
                    InputDate = DateTime.Now
                };
                //_repository.Insert(newCirculation);
                _db.Circulations.Add(newCirculation);
                _db.SaveChanges();
            }

            if (success)
            {
                TempData["SuccessDialogMsg"] = "Recipient's circulated items were cloned successfully";
            }
            else
            {
                TempData["ErrorDialogMsg"] = "An error was encountered whilst attempting to clone the selected recipient's circulated items. Please check and try again.";
            }

            return Json(new { success = success });

        }


        public ActionResult AddRecipient(int id = 0)
        {
            var copy = _db.Copies.Find(id);
            if (copy == null)
            {
                return HttpNotFound();
            }

            var lbvm = new ListBoxViewModel
            {
                PostSelectController = "Circulation",
                PostSelectAction = "PostAddRecipient",
                SelectedItems = null,
                HeaderText =
                    "Add " + DbRes.T("Circulation.Recipient", "FieldDisplayName") + " to " +
                    DbRes.T("Circulation.Circulation_List", "FieldDisplayName"),
                DetailsHeader = copy.Title.Title1 + " - Copy " + copy.CopyNumber,
                DetailsText =
                    "Select the " + DbRes.T("Circulation.Recipients", "FieldDisplayName") + " you wish to add to the " +
                    DbRes.T("Circulation.Circulation_List", "FieldDisplayName") + " for this Item",
                SelectLabel = DbRes.T("Circulation.Recipients", "FieldDisplayName"),
                OkButtonText = "Add Selected",
                PostSelectId = copy.CopyID
            };

            lbvm.AvailableItems = _db.Users.Include(u => u.Department).Where(u => u.IsLive && u.CanDelete && u.Lastname != null)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Lastname + ", " + x.Firstname + " (" + x.Department.Department1 + ")"
                }).OrderBy(u => u.Text)
                .ToList();

            ViewBag.Title = "Add " + DbRes.T("Circulation.Recipient", "FieldDisplayName") + " to " +
                            DbRes.T("Circulation.Circulation_List", "FieldDisplayName");
            return PartialView("_MultiSelectListBox", lbvm);
        }

        [HttpPost]
        public ActionResult PostAddRecipient(ListBoxViewModel lbvm)
        {
            var copy = _db.Copies.Find(lbvm.PostSelectId);
            if (copy == null)
            {
                return RedirectToAction("Index");
            }

            var maxSortOrder = _db.Circulations.Where(c => c.CopyID == copy.CopyID).Max(x => x.SortOrder) ?? 0;

            foreach (var userid in lbvm.SelectedItems.ToList())
            {
                maxSortOrder++;
                var recipient = _db.Users.Find(userid);
                if (recipient != null)
                {
                    var newCirculation = new Circulation
                    {
                        CopyID = copy.CopyID,
                        RecipientUser = recipient,
                        SortOrder = maxSortOrder,
                        InputDate = DateTime.Now
                    };
                    _db.Circulations.Add(newCirculation);
                    _db.SaveChanges();
                }

            }
            //return RedirectToAction("CirculationList", new { Copy = copy.CopyID });
            return Json(new { success = true }); 
        }


        //Remove an item from a recipient's list
        public ActionResult RemoveItemFromRecipient(int? id)
        {
            var circulation = _db.Circulations.Find(id);
            if (circulation == null)
            {
                return HttpNotFound();
            }

            var userId = circulation.RecipientUser.Id;
            var recipient = _db.Users.Find(userId);

            // Create new instance of DeleteConfirmationViewModel and pass it to _DeleteConfirmation Dialog (Reusable)
            DeleteConfirmationViewModel deleteConfirmationViewModel = new DeleteConfirmationViewModel
            {
                DeleteEntityId = circulation.CirculationID,
                HeaderText = "from " + recipient.Firstname.TrimEnd() + "'s list",
                PostDeleteAction = "PostRemoveRecipient",
                PostDeleteController = "Circulation",
                DetailsText = circulation.Copy.Title.Title1 + " - Copy: " + circulation.Copy.CopyNumber,
                ButtonText = "Confirm",
                FunctionText = "Confirm Remove Item",
                ConfirmationHeaderText = "You are about to remove the following item "
            };
            return PartialView("_DeleteConfirmation", deleteConfirmationViewModel);
        }


        //Remove recipient from circulation list
        public ActionResult RemoveRecipient(int? id)
        {
            var circulation = _db.Circulations.Find(id);
            if (circulation == null)
            {
                return HttpNotFound();
            }
            var userId = circulation.RecipientUser.Id;
            var recipient = _db.Users.Find(userId);


            // Create new instance of DeleteConfirmationViewModel and pass it to _DeleteConfirmation Dialog (Reusable)
            DeleteConfirmationViewModel deleteConfirmationViewModel = new DeleteConfirmationViewModel
            {
                DeleteEntityId = circulation.CirculationID,
                HeaderText = string.Format("{0} {1}", recipient.Firstname, recipient.Lastname), //circulation.LibraryUser.Fullname,
                PostDeleteAction = "PostRemoveRecipient",
                PostDeleteController = "Circulation",
                DetailsText = "",
                ButtonText = "Remove",
                FunctionText = "Remove",
                ConfirmationHeaderText = "You are about to remove "
            };
            return PartialView("_DeleteConfirmation", deleteConfirmationViewModel);
        }


        [HttpPost]
        public ActionResult PostRemoveRecipient(DeleteConfirmationViewModel dcvm)
        {
            var circulation = _db.Circulations.Find(dcvm.DeleteEntityId);
            if (circulation == null)
            {
                return HttpNotFound();
            }

            var copyId = circulation.CopyID;
            try
            {
                _db.Circulations.Remove(circulation);
                _db.SaveChanges();
                ReSortRecipients(copyId.Value);
                return Json(new { success = true });
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
            }
            return RedirectToAction("CirculationList", new { Copy = copyId });
        }


        public ActionResult RemoveAllRecipients(int? id)
        {
            var copy = _db.Copies.Find(id);
            if (copy == null)
            {
                return HttpNotFound();
            }
            // Create new instance of DeleteConfirmationViewModel and pass it to _DeleteConfirmation Dialog (Reusable)
            DeleteConfirmationViewModel deleteConfirmationViewModel = new DeleteConfirmationViewModel
            {
                DeleteEntityId = copy.CopyID,
                HeaderText = "All " + DbRes.T("Circulation.Recipients", "FieldDisplayName"),
                PostDeleteAction = "PostRemoveAllRecipients",
                PostDeleteController = "Circulation",
                DetailsText = copy.Title.Title1 + "- Copy: " + copy.CopyNumber,
                ButtonText = "Remove",
                FunctionText = "Remove",
                ConfirmationHeaderText = "You are about to remove "
            };
            return PartialView("_DeleteConfirmation", deleteConfirmationViewModel);
        }

        [HttpPost]
        public ActionResult PostRemoveAllRecipients(DeleteConfirmationViewModel dcvm)
        {
            var copy = _db.Copies.Find(dcvm.DeleteEntityId);
            if (copy == null)
            {
                return HttpNotFound();
            }

            IEnumerable<Circulation> circulations = _db.Circulations.Where(c => c.CopyID == copy.CopyID);

            foreach (var recipient in circulations.ToList())
            {
                _db.Circulations.Remove(recipient);
                _db.SaveChanges();
            }
            return Json(new { success = true });
            //return RedirectToAction("CirculationList", new {Copy = copy.CopyID});
        }


        public ActionResult RemoveRecipientFromAll(string id)
        {
            var libraryUser = _db.Users.Find(id);
            if (libraryUser == null)
            {
                return HttpNotFound();
            }
            // Create new instance of DeleteConfirmationViewModel and pass it to _DeleteConfirmation Dialog (Reusable)
            DeleteConfirmationViewModel deleteConfirmationViewModel = new DeleteConfirmationViewModel
            {
                DeleteEntityIdString = libraryUser.Id,
                HeaderText =
                    libraryUser.Firstname + " From All " + DbRes.T("Circulation.Circulation_Lists", "FieldDisplayName") +
                    "?",
                PostDeleteAction = "PostRemoveRecipientFromAll",
                PostDeleteController = "Circulation",
                DetailsText = "",
                ButtonText = "Remove",
                FunctionText = "Remove",
                ConfirmationHeaderText = "You are about to remove "
            };
            return PartialView("_DeleteConfirmation", deleteConfirmationViewModel);
        }


        [HttpPost]
        public ActionResult PostRemoveRecipientFromAll(DeleteConfirmationViewModel dcvm)
        {
            var libraryUser = _db.Users.Find(dcvm.DeleteEntityIdString);
            if (libraryUser == null)
            {
                return HttpNotFound();
            }
            IEnumerable<Circulation> circulations = _db.Circulations.Where(c => c.RecipientUser.Id == libraryUser.Id);

            foreach (var circulation in circulations.ToList())
            {
                try
                {
                    var copyId = circulation.CopyID;
                    _db.Circulations.Remove(circulation);
                    _db.SaveChanges();
                    ReSortRecipients(copyId.Value);
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", e.Message);
                }
            }
            return Json(new { success = true });
        }


        public void ResortAll()
        {
            var circulatedItems = _db.Copies.Where(c => c.Circulated);
            foreach (var copy in circulatedItems.ToList())
            {
                ReSortRecipients(copy.CopyID);
            }
        }

        public void ReSortRecipients(int id = 0)
        {
            var copy = _db.Copies.Find(id);
            if (copy == null)
            {
                return;
            }

            var i = 1;
            //IEnumerable<Circulation> circulations =
            var circulations =
            from c in
                _db.Circulations.Where(x => x.CopyID == copy.CopyID)
                    .OrderBy(x => x.SortOrder)
            //.ThenBy(x => x.Recipient.Lastname)
            select c;

            foreach (Circulation circulation in circulations.ToList())
            {
                circulation.SortOrder = i;
                circulation.LastModified = DateTime.Now;
                if (ModelState.IsValid)
                {
                    _db.Entry(circulation).State = EntityState.Modified;
                    _db.SaveChanges();
                }
                i++;
            }
        }

        public ActionResult _MoveRecipientUpList(int? id)
        {
            var recipient = _db.Circulations.Find(id);
            if (recipient == null)
            {
                return null;
            }
            MoveRecipientUp(recipient);
            //return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            return RedirectToAction("QuickCheckIn", "PartsReceived", new { id = recipient.CopyID });
        }


        public ActionResult MoveRecipientUpList(int? id)
        {
            var recipient = _db.Circulations.Find(id);
            if (recipient == null)
            {
                return HttpNotFound();
            }
            MoveRecipientUp(recipient);
            return RedirectToAction("CirculationList", new { id = recipient.CopyID });
        }


        public void MoveRecipientUp(Circulation item)
        {
            var copyId = item.CopyID;
            var userId = item.RecipientUser.Id;
            var oldSortOrder = item.SortOrder;
            var newSortOrder = oldSortOrder - 1;

            //Update the recipient's position/sort order to the desired 'newSortOrder' ...
            item.SortOrder = newSortOrder;
            item.LastModified = DateTime.Now;
            _db.Entry(item).State = EntityState.Modified;
            _db.SaveChanges();

            //Get the circulation record that this one is swapping with ...
            var circulation =
                _db.Circulations.FirstOrDefault(
                    c => c.CopyID == copyId && c.SortOrder == newSortOrder && c.RecipientUser.Id != userId);

            if (circulation != null)
            {
                circulation.SortOrder = oldSortOrder; //newSortOrder - 1
                circulation.LastModified = DateTime.Now;
                if (ModelState.IsValid)
                {
                    _db.Entry(circulation).State = EntityState.Modified;
                    _db.SaveChanges();
                }
            }
            ReSortRecipients(copyId.Value);
        }


        public ActionResult _MoveRecipientDownList(int? id)
        {
            var recipient = _db.Circulations.Find(id);
            if (recipient == null)
            {
                return null;
            }
            MoveRecipientDown(recipient);
            //return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            return RedirectToAction("QuickCheckIn", "PartsReceived", new { id = recipient.CopyID });
        }


        public ActionResult MoveRecipientDownList(int? id)
        {
            var recipient = _db.Circulations.Find(id);
            if (recipient == null)
            {
                return HttpNotFound();
            }
            MoveRecipientDown(recipient);
            return RedirectToAction("CirculationList", new { id = recipient.CopyID });
        }

        public void MoveRecipientDown(Circulation item)
        {
            var copyId = item.CopyID;
            var userId = item.RecipientUser.Id;
            var oldSortOrder = item.SortOrder;
            var newSortOrder = oldSortOrder + 1;

            //Update the recipient's position/sort order to the desired 'newSortOrder' ...
            item.SortOrder = newSortOrder;
            item.LastModified = DateTime.Now;
            _db.Entry(item).State = EntityState.Modified;
            _db.SaveChanges();

            //Get the circulation record that this one is swapping with ...
            var circulation =
                _db.Circulations.FirstOrDefault(
                    c => c.CopyID == copyId && c.SortOrder == newSortOrder && c.RecipientUser.Id != userId);

            if (circulation != null)
            {
                circulation.SortOrder = oldSortOrder;
                circulation.LastModified = DateTime.Now;
                if (ModelState.IsValid)
                {
                    _db.Entry(circulation).State = EntityState.Modified;
                    _db.SaveChanges();
                }
            }

            ReSortRecipients(copyId.Value);
        }


        //Add copy to circulation
        public ActionResult AddCopyToCirculation()
        {
            var lbvm = new ListBoxViewModel
            {
                PostSelectController = "Circulation",
                PostSelectAction = "PostAddCopyToCirculation",
                SelectedItems = null,
                HeaderText = "Add " + DbRes.T("Copies", "EntityType").ToLower() + " to " + DbRes.T("Circulation.Circulated_Items", "FieldDisplayName") + " List",
                DetailsText =
                    "Select the " + DbRes.T("Copies", "EntityType").ToLower() + " you wish to include in the list of " +
                    DbRes.T("Circulation.Circulated_Items", "FieldDisplayName"),
                OkButtonText = "Add Selected",
                SelectLabel = "Copies"
            };

            var availableItems = _db.Copies.Where(c => c.Circulated == false).OrderBy(c => c.Title.Title1.Substring(c.Title.NonFilingChars)).Select(item => new SelectListItem
            {
                Value = item.CopyID.ToString(),
                Text = item.Title.Title1 + " - Copy: " + item.CopyNumber.ToString()
            }).ToList();

            lbvm.AvailableItems = availableItems.Select(l => new SelectListItem { Text = l.Text, Value = l.Value });

            ViewBag.Title = "Add " + DbRes.T("Copies", "EntityType").ToLower() + " to " + DbRes.T("Circulation.Circulated_Items", "FieldDisplayName") + " List";
            return PartialView("_MultiSelectListBox", lbvm);
        }

        [HttpPost]
        public ActionResult PostAddCopyToCirculation(ListBoxViewModel lbvm)
        {
            foreach (var copyid in lbvm.SelectedItems)
            {
                var copy = _db.Copies.Find(int.Parse(copyid));
                if (copy != null)
                {
                    copy.Circulated = true;
                    copy.LastModified = DateTime.Now;
                    if (ModelState.IsValid)
                    {
                        try
                        {
                            _db.Entry(copy).State = EntityState.Modified;
                            _db.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            ModelState.AddModelError("", e.Message);
                        }
                    }
                }
            }
            //return RedirectToAction("Index");
            return Json(new { success = true }); 
        }

        //Remove from Circulation
        public ActionResult RemoveFromCirculation(int? id)
        {
            var copy = _db.Copies.Find(id);
            if (copy == null)
            {
                return HttpNotFound();
            }
            // Create new instance of DeleteConfirmationViewModel and pass it to _DeleteConfirmation Dialog (Reusable)
            DeleteConfirmationViewModel deleteConfirmationViewModel = new DeleteConfirmationViewModel
            {
                DeleteEntityId = copy.CopyID,
                HeaderText = " item from " + DbRes.T("Circulation.Circulated_Items", "FieldDisplayName") + " List",
                PostDeleteAction = "PostRemoveFromCirculation",
                PostDeleteController = "Circulation",
                DetailsText = copy.Title.Title1 + " - Copy: " + copy.CopyNumber,
                ButtonText = "Remove",
                FunctionText = "Remove",
                ConfirmationHeaderText = "You are about to remove the following"
            };
            return PartialView("_DeleteConfirmation", deleteConfirmationViewModel);
        }

        [HttpPost]
        public ActionResult PostRemoveFromCirculation(DeleteConfirmationViewModel dcvm)
        {
            // Get item which we need to delete from EntityFramework 
            var copy = _db.Copies.Find(dcvm.DeleteEntityId);

            if (copy == null)
            {
                return HttpNotFound();
            }
            copy.Circulated = false;
            copy.LastModified = DateTime.Now;
            if (ModelState.IsValid)
            {
                try
                {
                    _db.Entry(copy).State = EntityState.Modified;
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


        public ActionResult ImportCirculationList(int? id)
        {
            var copy = _db.Copies.Find(id);
            if (copy == null)
            {
                return HttpNotFound();
            }
            var viewModel = new SelectPopupViewModel
            {
                PostSelectController = "Circulation",
                PostSelectAction = "PostImportCirculationList",
                SelectedItem = "0",
                HeaderText = "Clone Existing " + DbRes.T("Circulation.Circulation_List", "FieldDisplayName"),
                DetailsText =
                    "Cloning an exiting " + DbRes.T("Circulation.Circulation_List", "FieldDisplayName") +
                    " will import all " + DbRes.T("Circulation.Recipients", "FieldDisplayName") +
                    " from the selected list to the current one. Choose whether to append " +
                    DbRes.T("Circulation.Recipients", "FieldDisplayName") +
                    " to the bottom of the current list or remove all " +
                    DbRes.T("Circulation.Recipients", "FieldDisplayName") +
                    " from the current list and replace with the cloned list.",
                SelectLabel = "",
                OkButtonText = "Clone Selected",
                PostSelectId = copy.CopyID
            };

            viewModel.AvailableItems = _db.Copies.Where(c => c.CopyID != copy.CopyID && c.Circulations.Any())
                .Select(x => new SelectListItem
                {
                    Value = x.CopyID.ToString(),
                    Text =
                        x.Title.Title1.Substring(x.Title.NonFilingChars) + " - Copy: " + x.CopyNumber.ToString() +
                        "; - " + x.Location.Location1 + " (" + x.Circulations.Count() + " recipients)"
                }).OrderBy(c => c.Text)
                .ToList();

            List<SelectListItem> cloneOptions = new List<SelectListItem>();
            cloneOptions.Add(new SelectListItem { Text = "Append", Value = "a" });
            cloneOptions.Add(new SelectListItem { Text = "Overwrite", Value = "o" });
            viewModel.AdditionalOptions = cloneOptions;
            ViewBag.Title = "Clone " + DbRes.T("Circulation.Circulation_List", "FieldDisplayName");
            return PartialView("CloneList", viewModel);
        }


        public ActionResult PostImportCirculationList(SelectPopupViewModel viewModel)
        {
            var copy = _db.Copies.Find(int.Parse(viewModel.SelectedItem));
            if (copy == null)
            {
                return HttpNotFound();
            }

            //If the option is to "overwrite" then remove all current recipients first
            if (viewModel.SelectedOption == "o")
            {
                IEnumerable<Circulation> circulations = _db.Circulations.Where(c => c.CopyID == viewModel.PostSelectId);
                foreach (var recipient in circulations.ToList())
                {
                    _db.Circulations.Remove(recipient);
                    _db.SaveChanges();
                }
            }

            //Get a list of all circulation records for the selected item (i.e. the one to copy) ...
            var imported = _db.Circulations.Where(c => c.CopyID == copy.CopyID).OrderBy(c => c.SortOrder);
            if (!imported.Any())
            {
                return Json(new { success = false });
            }

            //Get a list of the current records for the copy we are added to ...
            var existing = _db.Circulations.Where(c => c.CopyID == viewModel.PostSelectId);

            //Get the current MAX sort order for the copy we are adding to ...
            var sortOrder = existing.Max(c => c.SortOrder) ?? 0;

            //Get a string/array of current users - we can use this to check we are not duplicating users when importing ...
            var users = new List<string>();
            foreach (var item in existing)
            {
                users.Add(item.RecipientUser.Id.ToString());
            }

            //Loop through this list and add the user to the current copy (PostSelectId)
            foreach (var item in imported.ToList())
            {
                //Check if we already have this user in the list ...
                if (users.Contains(item.RecipientUser.Id.ToString())) continue;
                sortOrder++;
                var newCirculation = new Circulation
                {
                    CopyID = viewModel.PostSelectId,
                    //Id = item.Id,
                    RecipientUser = item.RecipientUser,
                    SortOrder = sortOrder,
                    InputDate = DateTime.Now
                };
                //_repository.Insert(newCirculation);
                _db.Circulations.Add(newCirculation);
                _db.SaveChanges();
            }
            TempData["SuccessDialogMsg"] = "Circulation list has been cloned successfully.";
            return Json(new { success = true });
        }


        public ActionResult ReplaceRecipient(string id)
        {
            var libraryUser = _db.Users.Find(id);
            if (libraryUser == null)
            {
                return HttpNotFound();
            }

            var viewModel = new SelectPopupViewModel
            {
                PostSelectController = "Circulation",
                PostSelectAction = "ReplaceRecipient",
                SelectedItem = "0",
                HeaderText = "Replace " + libraryUser.Firstname + "?",
                DetailsText =
                    "Replace " + string.Format("{0} {1}", libraryUser.Firstname, libraryUser.Lastname) + " with another " +
                    DbRes.T("Circulation.Recipient", "FieldDisplayName") + " on all of " + libraryUser.Firstname +
                    "'s currently " + DbRes.T("Circulation.Circulated_Items", "FieldDisplayName"),
                SelectLabel = "",
                OkButtonText = "Replace " + libraryUser.Firstname,
                PostSelectIdString = libraryUser.Id
            };

            viewModel.AvailableItems = _db.Users.Where(u => u.Id != libraryUser.Id && u.IsLive && u.CanDelete && u.Lastname != null)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Lastname + ", " + x.Firstname
                }).OrderBy(c => c.Text)
                .ToList();

            return PartialView("_SelectPopup", viewModel);
        }

        [HttpPost]
        public ActionResult ReplaceRecipient(SelectPopupViewModel viewModel)
        {
            var user1 = _db.Users.Find(viewModel.PostSelectIdString);
            var user2 = _db.Users.Find(viewModel.SelectedItem);

            if (user1 != null && user2 != null)
            {
                //Do the actual work ...
                DoGlobalReplaceRecipient(user1.Id, user2.Id);
            }
            TempData["SuccessDialogMsg"] = "Recipient has been replaced successfully.";
            return Json(new { success = true });
        }



        public ActionResult GlobalReplaceRecipient()
        {
            var viewModel = new Select2PopupViewModel
            {
                PostSelectController = "Circulation",
                PostSelectAction = "GlobalReplaceRecipient",
                HeaderText = "Global Replace " + DbRes.T("Circulation.Recipient", "FieldDisplayName"),
                DetailsText =
                    "Use this form to change a " + DbRes.T("Circulation.Recipient", "FieldDisplayName") + " on all " +
                    DbRes.T("Circulation.Circulation_List", "FieldDisplayName") + " from one " +
                    DbRes.T("Users.LibraryUser", "FieldDisplayName") + " to another",

                SelectLabel1 = "Select the current recipient (to be replaced):",
                SelectLabel2 = "Select the new recipient:",

                SelectedItem1 = "0",
                SelectedItem2 = "0",

                OkButtonText = "Global Replace"
            };

            var user1List = new List<SelectListItem>();

            //Add a default item ...
            user1List.Add(new SelectListItem
            {
                Text = "Select current " + DbRes.T("Circulation.Recipient", "FieldDisplayName"),
                Value = "0"
            });

            user1List.AddRange(_db.Users.Where(u => u.Circulations.Any() && u.IsLive && u.CanDelete && u.Lastname != null)
                .OrderBy(u => u.Lastname)
                .ThenBy(u => u.Firstname)
                .Select(x => new SelectListItem
                {
                    Value = x.Id,
                    Text = x.Lastname + ", " + x.Firstname
                }));

            viewModel.AvailableItems1 = user1List;

            var user2List = new List<SelectListItem>();

            //Add a default item ...
            user2List.Add(new SelectListItem
            {
                Text = "Select new " + DbRes.T("Circulation.Recipient", "FieldDisplayName"),
                Value = "0"
            });

            user2List.AddRange(_db.Users.Where(u => u.IsLive && u.CanDelete && u.Lastname != null).OrderBy(u => u.Lastname).ThenBy(u => u.Firstname)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Lastname + ", " + x.Firstname
                }));

            viewModel.AvailableItems2 = user2List;
            ViewBag.Title = viewModel.HeaderText;
            return PartialView("_Select2Popup", viewModel);
        }


        [HttpPost]
        public ActionResult GlobalReplaceRecipient(Select2PopupViewModel viewModel)
        {
            var user1 = _db.Users.Find(viewModel.SelectedItem1);
            if (user1 == null)
            {
                return HttpNotFound();
            }
            var user2 = _db.Users.Find(viewModel.SelectedItem2);
            if (user2 == null)
            {
                return HttpNotFound();
            }

            //Do the actual work ...
            var success = DoGlobalReplaceRecipient(user1.Id, user2.Id);

            if (success)
            {
                TempData["SuccessDialogMsg"] = "The selected recipient has been globally replaced successfully.";
            }
            else
            {
                TempData["ErrorDialogMsg"] = "An error was encountered when attempting to globally replace the selected recipient. Please try again.";
            }

            return Json(new { success = success });
        }

        public bool DoGlobalReplaceRecipient(string userId1, string userId2)
        {
            var user1Circulations = _db.Circulations.Where(c => c.RecipientUser.Id == userId1);

            //Firstly, remove user2 from any lists that user1 is already on, otherwise user2 will end up on the list twice!
            foreach (var item in user1Circulations.ToList())
            {
                var toRemove =
                    _db.Circulations.FirstOrDefault(c => c.CopyID == item.CopyID && c.RecipientUser.Id == userId2);
                if (toRemove != null)
                {
                    try
                    {
                        // _repository.Delete(toRemove);
                        _db.Circulations.Remove(toRemove);
                        _db.SaveChanges();
                        ReSortRecipients(item.CopyID.Value);
                    }
                    catch (Exception e)
                    {
                       // ModelState.AddModelError("", e.Message);
                        return false;
                    }
                }
            }

            //Now update all of user1's records with user2
            foreach (var toUpdate in user1Circulations.ToList())
            {
                try
                {
                    toUpdate.RecipientUser = _db.Users.Find(userId2);
                    _db.Entry(toUpdate).State = EntityState.Modified;
                    _db.SaveChanges();
                }
                catch (Exception e)
                {
                    //ModelState.AddModelError("", e.Message);
                    return false;
                }
            }
            return true;
        }


        public ActionResult AddRecipientToAll()
        {
            var viewModel = new SelectPopupViewModel
            {
                PostSelectController = "Circulation",
                PostSelectAction = "AddRecipientToAll",
                SelectedItem = "0",
                HeaderText =
                    "Add " + DbRes.T("Circulation.Recipient", "FieldDisplayName") + " to all " +
                    DbRes.T("Circulation.Circulation_Lists", "FieldDisplayName"),
                DetailsText =
                    "Add a " + DbRes.T("Circulation.Recipient", "FieldDisplayName") + " to the top or bottom of all " +
                    DbRes.T("Circulation.Circulation_Lists", "FieldDisplayName") + ".",
                SelectLabel = "Select a " + DbRes.T("Circulation.Recipient", "FieldDisplayName"),
                SelectText = "",
                OptionLabel = "Choose where to add the selected " + DbRes.T("Circulation.Recipient", "FieldDisplayName"),
                OkButtonText = "Add Selected"
            };

            //viewModel.AvailableItems = _db.Users.Where(u => u.IsLive && u.CanDelete && u.Lastname != null)
            //    .Select(x => new SelectListItem
            //    {
            //        Value = x.Id.ToString(),
            //        Text = x.Lastname + ", " + x.Firstname
            //    }).OrderBy(c => c.Text)
            //    .ToList();
            
            viewModel.AvailableItems = SelectListHelper.LibraryUserList();

            List<SelectListItem> cloneOptions = new List<SelectListItem>();
            cloneOptions.Add(new SelectListItem { Text = "Add to top of list", Value = "t" });
            cloneOptions.Add(new SelectListItem { Text = "Add to bottom of list", Value = "b" });
            viewModel.AdditionalOptions = cloneOptions;
            ViewBag.Title = viewModel.HeaderText;
            return PartialView("_SelectPopupWithOption", viewModel);
        }


        [HttpPost]
        public ActionResult AddRecipientToAll(SelectPopupViewModel viewModel)
        {
            var libraryUser = _db.Users.Find(viewModel.SelectedItem);
            if (libraryUser == null)
            {
                return HttpNotFound();
            }

            var position = viewModel.SelectedOption == "t" ? 0 : 999;
            
            var success = DoAddRecipientToAll(libraryUser.Id, position);

            ResortAll();

            if (success)
            {
                TempData["SuccessDialogMsg"] = libraryUser.Firstname + " has been added to ALL circulation lists.";
            }
            else
            {
                TempData["ErrorDialogMsg"] = "An error was encountered whilst attempting to add " + libraryUser.Firstname + " to all circulation lists. Please check and try again.";
            }

            return Json(new { success = success });
        }


        public ActionResult AddSelectedRecipientToAll(string id)
        {
            var libraryUser = _db.Users.Find(id);
            if (libraryUser == null)
            {
                return HttpNotFound();
            }

            var viewModel = new GenericConfirmationViewModel
            {
                ConfirmEntityId = libraryUser.Id,
                DetailsText = "You are about to add " + libraryUser.Firstname + " to ALL circulation lists.",
                ConfirmationText = "Are you sure you want to continue",
                ConfirmButtonText = "Ok",
                HeaderText = "Add recipient to All lists",
                PostConfirmController = "Circulation",
                PostConfirmAction = "AddSelectedRecipientToAll"
            };

            return PartialView("_GenericConfirmation", viewModel);
        }


        [HttpPost]
        public ActionResult AddSelectedRecipientToAll(GenericConfirmationViewModel viewModel)
        {
            var libraryUser = _db.Users.Find(viewModel.ConfirmEntityId);
            if (libraryUser == null)
            {
                return HttpNotFound();
            }

            var success = DoAddRecipientToAll(libraryUser.Id, 999);
            
            ResortAll();

            if (success)
            {
                TempData["SuccessDialogMsg"] = libraryUser.Firstname + " has been added to ALL circulation lists.";
            }
            else
            {
                TempData["ErrorDialogMsg"] = "An error was encountered whilst attempting to add " + libraryUser.Firstname + " to all circulation lists. Please check and try again.";
            }
            
            return Json(new { success = success });
        }


        public bool DoAddRecipientToAll(string userId, int position = 999)
        {
            //Get a list of all cisrulated items ...
            var circulatedItems = _db.Copies.Where(c => c.Circulated);
            try
            {
                foreach (var copy in circulatedItems.ToList())
                {
                    var newCirculation = new Circulation
                    {
                        RecipientUser = _db.Users.Find(userId),
                        SortOrder = position,
                        CopyID = copy.CopyID
                    };
                    _db.Circulations.Add(newCirculation);
                    _db.SaveChanges();
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public ActionResult RemoveNonLiveRecipients()
        {
            // Create new instance of DeleteConfirmationViewModel and pass it to _DeleteConfirmation Dialog (Reusable)
            var deleteConfirmationViewModel = new DeleteConfirmationViewModel
            {
                HeaderText = "All Non-Live " + DbRes.T("Circulation.Recipients", "FieldDisplayName"),
                PostDeleteAction = "PostRemoveNonLiveRecipients",
                PostDeleteController = "Circulation",
                ButtonText = "Remove",
                FunctionText = "Remove",
                ConfirmationHeaderText = "You are about to remove "
            };
            return PartialView("_DeleteConfirmation", deleteConfirmationViewModel);
        }


        public ActionResult PostRemoveNonLiveRecipients(DeleteConfirmationViewModel viewModel)
        {
            var recipients = _db.Users.Where(u => u.IsLive && u.CanDelete && u.Lastname != null);
            var circulations = from c in _db.Circulations
                               join u in recipients on c.RecipientUser.Id equals u.Id
                               where u.IsLive != true
                               select c;
            var success = true;

            foreach (var circulation in circulations.ToList())
            {
                var copyId = circulation.CopyID;
                

                try
                {
                    //Delete the circulation record ...
                    _db.Circulations.Remove(circulation);
                    _db.SaveChanges();
                }
                catch (Exception)
                {
                    success = false;
                }

                //Reorder the affected liss ...
                if (copyId != null) ReSortRecipients(copyId.Value);
            }

            if (success)
            {
                TempData["SuccessDialogMsg"] = "All Non-Live Recipients have been removed.";
            }
            else
            {
                TempData["ErrorDialogMsg"] = "An error was encountered whilst attempting to remove all non-live recipients. Please check and try again.";
            }

            return Json(new { success = success });
        }


        public ActionResult ClearCirculationSlips()
        {
            // Create new instance of DeleteConfirmationViewModel and pass it to _DeleteConfirmation Dialog (Reusable)
            var deleteConfirmationViewModel = new DeleteConfirmationViewModel
            {
                HeaderText = "All Pending " + DbRes.T("Circulation.Circulation_Slips", "FieldDisplayName"),
                PostDeleteAction = "PostClearCirculationSlips",
                PostDeleteController = "Circulation",
                ButtonText = "Clear Slips",
                FunctionText = "Clear",
                ConfirmationHeaderText = "You are about to clear "
            };
            return PartialView("_DeleteConfirmation", deleteConfirmationViewModel);
        }


        [HttpPost]
        public ActionResult PostClearCirculationSlips()
        {
            var pendingParts = _db.PartsReceiveds.Where(p => p.PrintList);
            var success = true;

            foreach (var part in pendingParts.ToList())
            {
                try
                {
                    //Update the part, setting PrintList=0...
                    part.PrintList = false;
                    //_repository.Update(part);
                    _db.Entry(part).State = EntityState.Modified;
                    _db.SaveChanges();
                }
                catch (Exception)
                {
                    success = false;
                }
            }
            
            if (success)
            {
                TempData["SuccessDialogMsg"] = "All Pending Circulation Slips have been cleared.";
            }
            else
            {
                TempData["ErrorDialogMsg"] = "An error was encountered whilst attempting to clear all circulation slips. Please check and try again.";
            }

            return Json(new { success = success });
        }


        public ActionResult _CirculationListSubForm(int id = 0)
        {
            var circList = _db.Circulations.Where(c => c.CopyID == id).OrderBy(c => c.SortOrder);

            return PartialView(circList);
        }


        //Print all outstanding slips ...
        public ActionResult PrintAllCirculationSlips()
        {
            var viewModel = new CirculationSlipViewModel
            {
                PartsReceived = _db.PartsReceiveds.Where(p => p.PrintList && p.Copy.Circulations.Any()),
                HasData = _db.PartsReceiveds.Any(p => p.PrintList && p.Copy.Circulations.Any())
            };

            ViewBag.Title = "Circulation Slip";
            return View("CirculationSlip", viewModel);
        }


        //Print all outstanding slips for a given copy - normally just the one
        public ActionResult PrintCirculationSlip(int? id) // passed CopyID
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var copy = _db.Copies.Find(id);
            if (copy == null)
            {
                return HttpNotFound();
            }

            var viewModel = new CirculationSlipViewModel
            {
                PartsReceived = _db.PartsReceiveds.Where(p => p.CopyID == copy.CopyID && p.PrintList),
                HasData = _db.PartsReceiveds.Any(p => p.CopyID == copy.CopyID && p.PrintList)

            };

            ViewBag.Title = "Circulation Slip";
            return PartialView("CirculationSlip", viewModel);
        }


        //Display all recipients for a given copy ...
        public ActionResult CirculationSlipRecipients(int? id) // passed CopyID
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var copy = _db.Copies.Find(id);
            if (copy == null)
            {
                return HttpNotFound();
            }

            var viewModel = new CirculationSlipRecipientListViewModel
            {
                CirculationList = _db.Circulations.Where(c => c.CopyID == copy.CopyID)
            };

            return PartialView("_CirculationSlipRecipients", viewModel);
        }


        public ActionResult Report_CirculatedItemsAZ()
        {
            var circulations =
                _db.Copies
                    .Where(c => c.Circulated)
                    .OrderBy(c => c.Title.Title1.Substring(c.Title.NonFilingChars))
                    .ThenBy(c => c.CopyNumber);

            var viewModel = new CirculationReportsViewModel()
            {
                Copies = circulations,
                HasData = circulations.Any()
            };

            ViewBag.Title = DbRes.T("Circulation.Circulated_Items", "FieldDisplayName") + " A to Z";

            return View("Reports/CirculatedItemsAZ", viewModel);
        }


        public ActionResult Report_CirculatedItemsAZWithHoldings()
        {
            var circulatedTitles = (from t in _db.Titles
                                    join c in _db.Copies on t.TitleID equals c.TitleID
                                    where t.Deleted == false && c.Circulated
                                    select t).Distinct();

            var viewModel = new CirculationReportsViewModel()
            {
                Titles = circulatedTitles,
                HasData = circulatedTitles.Any()
            };

            ViewBag.Title = DbRes.T("Circulation.Circulated_Items", "FieldDisplayName") + " A to Z - With " +
                            DbRes.T("Copies.Holdings", "FieldDisplayName");
            return View("Reports/CirculatedItemsAZWithHoldings", viewModel);
        }


        public ActionResult Report_CirculationListTitle()
        {
            var circulatedTitles = (from t in _db.Titles
                                    join c in _db.Copies on t.TitleID equals c.TitleID
                                    where t.Deleted == false && c.Circulations.Any()
                                    orderby t.Title1.Substring(t.NonFilingChars)
                                    select t).Distinct();

            var viewModel = new CirculationReportsViewModel()
            {
                Titles = circulatedTitles,
                HasData = circulatedTitles.Any()
            };

            ViewBag.Title = DbRes.T("Circulation.Circulation_Lists", "FieldDisplayName") + " - By " +
                            DbRes.T("Titles.Title", "FieldDisplayName");

            return View("Reports/CirculationListByTitle", viewModel);
        }


        public ActionResult Report_CirculatedItemsOneNoRecipients()
        {
            var circulatedTitles = (from t in _db.Titles
                                    join c in _db.Copies on t.TitleID equals c.TitleID
                                    where t.Deleted == false && c.Circulated && (c.Circulations.Any() == false || c.Circulations.Count == 1)
                                    //orderby t.Title1.Substring(t.NonFilingChars)
                                    select t).Distinct();

            var viewModel = new CirculationReportsViewModel()
            {
                Titles = circulatedTitles,
                HasData = circulatedTitles.Any()
            };

            ViewBag.Title = DbRes.T("Circulation.Circulated_Items", "FieldDisplayName") + " - One or No " +
                            DbRes.T("Circulation.Recipients", "FieldDisplayName");

            return View("Reports/CirculatedItemsOneNoRecipients", viewModel);
        }


        public ActionResult Report_CirculationListByPublisher()
        {
            var circulatedPublishers = (from p in CacheProvider.GetAll<Publisher>("publishers")
                                        join t in _db.Titles on p.PublisherID equals t.PublisherID
                                        join c in _db.Copies on t.TitleID equals c.TitleID
                                        where t.Deleted == false && c.Circulations.Any()
                                        orderby p.PublisherName
                                        select p).Distinct().ToList();

            var viewModel = new CirculationReportsViewModel()
            {
                Publishers = circulatedPublishers,
                HasData = circulatedPublishers.Any()
            };

            ViewBag.Title = DbRes.T("Circulation.Circulation_Lists", "FieldDisplayName") + " - By " +
                            DbRes.T("Publishers.Publisher", "FieldDisplayName")
                            ;
            return View("Reports/CirculationListByPublisher", viewModel);
        }


        public ActionResult _CirculatedTitlesByPublisher(int id = 0)
        {
            var publisher = _db.Publishers.Find(id);
            if (publisher == null)
            {
                return null;
            }

            var circulatedTitles = (from t in _db.Titles
                                    join c in _db.Copies on t.TitleID equals c.TitleID
                                    where t.Deleted == false && t.PublisherID == id && c.Circulations.Any()
                                    orderby t.Title1.Substring(t.NonFilingChars)
                                    select t).Distinct();

            return PartialView("Reports/_CirculatedTitlesByPublisher", circulatedTitles);
        }


        public ActionResult Report_CirculatedItemsByLocation()
        {
            var locations = (from c in _db.Copies
                             where c.Circulated
                             select c.Location.ParentLocation).Distinct();

            var viewModel = new CirculationReportsViewModel()
            {
                Locations = locations,
                HasData = locations.Any()
            };

            ViewBag.Title = DbRes.T("Circulation.Circulated_Items", "FieldDisplayName") + " By " +
                            DbRes.T("Locations.Location", "FieldDisplayName");

            return View("Reports/CirculatedItemsByLocation", viewModel);
        }

        public ActionResult _CirculatedItemsByLocation(int id = 0)
        {
            var location = _db.Locations.Find(id);
            if (location == null)
            {
                return null;
            }

            var circulatedTitles = (from t in _db.Titles
                                    join c in _db.Copies on t.TitleID equals c.TitleID
                                    where c.Circulated && c.LocationID == id
                                    orderby t.Title1.Substring(t.NonFilingChars)
                                    select t).Distinct();

            return PartialView("Reports/_CirculatedItemsByLocation", circulatedTitles);
        }


        public ActionResult Report_CirculatedItemsBySubject()
        {
            var keywords = (from t in _db.Titles
                            join s in _db.SubjectIndexes on t.TitleID equals s.TitleID
                            join k in _db.Keywords on s.KeywordID equals k.KeywordID
                            join c in _db.Copies on t.TitleID equals c.TitleID
                            where c.Circulated
                            orderby k.KeywordTerm
                            select k).Distinct();

            var viewModel = new CirculationReportsViewModel()
            {
                Keywords = keywords,
                HasData = keywords.Any()
            };

            ViewBag.Title = DbRes.T("Circulation.Circulated_Items", "FieldDisplayName") + " By " +
                            DbRes.T("Keywords.Keyword", "FieldDisplayName");

            return View("Reports/CirculatedItemsBySubject", viewModel);
        }

        public ActionResult _CirculatedItemsBySubject(int id = 0)
        {
            var keyword = _db.Keywords.Find(id);
            if (keyword == null)
            {
                return null;
            }

            var circulatedTitles = (from t in _db.Titles
                                    join s in _db.SubjectIndexes on t.TitleID equals s.TitleID
                                    join c in _db.Copies on t.TitleID equals c.TitleID
                                    where s.KeywordID == id && c.Circulated
                                    select t).Distinct();

            return PartialView("Reports/_CirculatedItems", circulatedTitles);
        }


        public ActionResult Report_CirculatedItemsByClassmark()
        {
            var classmarks = (from s in _db.Classmarks
                              join t in _db.Titles on s.ClassmarkID equals t.ClassmarkID
                              join c in _db.Copies on t.TitleID equals c.TitleID
                              where c.Circulated && t.Deleted == false
                              select s).Distinct();

            var viewModel = new CirculationReportsViewModel()
            {
                Classmarks = classmarks,
                HasData = classmarks.Any()
            };

            ViewBag.Title = DbRes.T("Circulation.Circulated_Items", "FieldDisplayName") + " By " +
                            DbRes.T("Classmarks.Classmark", "FieldDisplayName");
            return View("Reports/CirculatedItemsByClassmark", viewModel);
        }

        public ActionResult _CirculatedItemsByClassmark(int id = 0)
        {
            var classmark = _db.Classmarks.Find(id);
            if (classmark == null)
            {
                return null;
            }

            var circulatedTitles = (from t in _db.Titles
                                    join s in _db.Classmarks on t.ClassmarkID equals s.ClassmarkID
                                    join c in _db.Copies on t.TitleID equals c.TitleID
                                    where c.Circulated && t.Deleted == false
                                    select t).Distinct();

            return PartialView("Reports/_CirculatedItems", circulatedTitles);
        }


        public ActionResult Report_CirculationReviewMemoByRecipient()
        {
            var viewModel = new SelectPopupViewModel
            {
                PostSelectController = "Circulation",
                PostSelectAction = "Post_CirculationReviewMemoByRecipient",
                SelectedItem = "0",
                HeaderText = DbRes.T("Circulation.Review_Memo", "FieldDisplayName"),
                DetailsHeader = "",
                DetailsText = "Select a recipient to print an individual " + DbRes.T("Circulation.Review_Memo", "FieldDisplayName"),
                SelectLabel = "",
                SelectText = "Select a " + DbRes.T("Circulation.Recipient", "FieldDisplayName"),
                OkButtonText = "OK",
                PostSelectId = 0
            };

            viewModel.AvailableItems =
                _db.Users.Where(u => u.Circulations.Any() && u.IsLive && u.CanDelete && u.Lastname != null)
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.Lastname + ", " + x.Firstname
                    }).OrderBy(c => c.Text)
                    .ToList();

            return PartialView("_AjaxSelectPopup", viewModel);
        }

        [HttpPost]
        public ActionResult Post_CirculationReviewMemoByRecipient(SelectPopupViewModel viewModel)
        {
            var recipient = _db.Users.Find(viewModel.SelectedItem);
            if (recipient == null)
            {
                return null;
            }

            //return RedirectToAction("Report_CirculationReviewMemo", new {id = recipient.UserID});

            return Json(new
            {
                redirectUrl = Url.Action("Report_CirculationReviewMemo", new { id = recipient.Id }),
                //newWindow = true,
                //target = "_blank",
                isRedirect = true
            });
        }


        public ActionResult Report_CirculationReviewMemo(string id)
        {
            IQueryable<ApplicationUser> recipients;

            if (string.IsNullOrEmpty(id))
            {
                recipients = _db.Users.Where(u => u.Circulations.Any() && u.IsLive && u.CanDelete && u.Lastname != null).OrderBy(r => r.Lastname);
                ViewBag.Title = "Circulation List Review Memos";
            }
            else
            {
                recipients = _db.Users.Where(u => u.Id == id);
                ViewBag.Title = "Circulation List Review Memo";
            }

            var viewModel = new CirculationReportsViewModel()
            {
                Recipients = recipients,
                HasData = recipients.Any()
            };

            //ViewBag.Title = "Circulation List Review Memo";
            ViewBag.MemoText = Settings.GetParameterValue("Circulation.ReviewMemoText", "You are currently on circulation lists to read the journals listed below.  Please review the list and let us know if there are any you do not wish to see anymore, or if there are any you would like to see in addition to those listed.  If you need a list of journals currently received by the firm, please contact us.", dataType: "longtext");

            return View("Reports/CirculationReviewMemos", viewModel);
        }

        public ActionResult _CirculatedItemsByUser(string id)
        {
            var recipient = _db.Users.Find(id);
            if (recipient == null)
            {
                return null;
            }

            //Get a simple list of titles circulated to the passed recipient ...
            var circulatedItems = (from t in _db.Titles
                                   join c in _db.Copies on t.TitleID equals c.TitleID
                                   join r in _db.Circulations on c.CopyID equals r.CopyID
                                   where c.Deleted == false && r.RecipientUser.Id == id
                                   select t).Distinct();

            return PartialView("Reports/_CirculatedItemsByUser", circulatedItems);
        }


        public ActionResult EmailAllRecipients(int id = 0)
        {
            if (id == 0)
            {
                return Json(new { success = false });
            }

            var copy = _db.Copies.Find(id);
            if (copy == null)
            {
                return Json(new { success = false });
            }

            var circulations = _db.Circulations.Where(c => c.CopyID == id);
            var emailAddresses = "";
            foreach (var item in circulations)
            {
                var user = _db.Users.Find(item.RecipientUser.Id);
                if (user != null)
                {
                    if (!string.IsNullOrEmpty(user.Email))
                    {
                        emailAddresses = emailAddresses + user.Email + ";";
                    }
                }
            }

            using (var messaging = new MessagingController())
            {
                return messaging.NewEmailPopup(emailAddresses, "Message about " + copy.Title.Title1);
            }
        }


        [HttpGet]
        public ActionResult Delete(int id = 0)
        {
            var circulation = _db.Circulations.Find(id);
            if (circulation == null)
            {
                return HttpNotFound();
            }
            if (circulation.Deleted)
            {
                return HttpNotFound();
            }

            var userId = circulation.RecipientUser.Id;
            var recipient = _db.Users.Find(userId);

            // Create new instance of DeleteConfirmationViewModel and pass it to _DeleteConfirmation Dialog (Reusable)
            DeleteConfirmationViewModel deleteConfirmationViewModel = new DeleteConfirmationViewModel
            {
                DeleteEntityId = id,
                HeaderText = _entityName,
                PostDeleteAction = "Delete",
                PostDeleteController = "Circulation",
                DetailsText = string.Format("{0} {1}", recipient.Firstname, recipient.Lastname) + " from " + DbRes.T("Circulation.Circulation_List", "FieldDisplayName")
            };
            return PartialView("_DeleteConfirmation", deleteConfirmationViewModel);
        }

        [HttpPost]
        public ActionResult Delete(DeleteConfirmationViewModel dcvm)
        {
            // Get item which we need to delete from EntityFramework
            var circulation = _db.Circulations.Find(dcvm.DeleteEntityId);

            if (circulation == null)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _db.Circulations.Remove(circulation);
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
                _repository.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
