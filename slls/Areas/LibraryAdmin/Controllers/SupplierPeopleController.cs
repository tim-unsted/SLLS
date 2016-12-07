using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using slls.Models;
using slls.Utils.Helpers;
using slls.ViewModels;
using Westwind.Globalization;

namespace slls.Areas.LibraryAdmin
{
    [RouteArea("LibraryAdmin", AreaPrefix = "Admin")]
    [RoutePrefix("Contacts")]
    //[Route("{action=index}")]
    public class SupplierPeopleController : FinanceBaseController
    {
        private readonly DbEntities _db = new DbEntities();
        private readonly string _entityName = DbRes.T("SupplierPeople.Contact", "FieldDisplayName");

        public SupplierPeopleController()
        {
            ViewBag.Title = DbRes.T("Suppliers.Contacts", "FieldDisplayName");
        }

        //GET: Select a contact ...
        public ActionResult Select()
        {
            ViewData["contactId"] = SelectListHelper.ContactsList();
            ViewBag.Title = "View/Edit " + _entityName;
            ViewBag.Message = "Select a " + _entityName;
            return View();
        }

        //POST: Select a title ...
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Select(int? id)
        {
            return RedirectToAction("Edit", new {id });
        }

        //Autocomplete
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult Autocomplete(string term)
        {
            var contacts = (from c in _db.SupplierPeoples
                            join a in _db.SupplierAddresses on c.AddressID equals a.AddressID
                            join s in _db.Suppliers on a.SupplierID equals  s.SupplierID
                             where (c.Firstname.Contains(term) || c.Surname.Contains(term))
                            orderby c.Firstname,c.Surname
                            select new { Fullname = c.Firstname + " " + c.Surname + " (" + s.SupplierName + ")", c.ContactID }).Take(10);

            IList<SelectListItem> list = new List<SelectListItem>();

            foreach (var x in contacts)
            {
                list.Add(new SelectListItem { Text = x.Fullname, Value = x.ContactID.ToString() });
            }

            var result = list.Select(item => new KeyValuePair<string, string>(item.Value.ToString(), item.Text)).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // GET: LibraryAdmin/SupplierPeople
        [Route("Index")]
        [Route("All")]
        [Route("~/LibraryAdmin/Contacts")]
        [Route("~/LibraryAdmin/Contacts/All")]
        [Route("~/LibraryAdmin/SupplierPeople")]
        [Route("~/LibraryAdmin/Contacts/Index")]
        [Route("~/LibraryAdmin/SupplierPeople/Index")]
        public ActionResult Index(string selectedLetter = "")
        {
            ViewBag.Title = DbRes.T("Suppliers.Contacts", "FieldDisplayName");
            
            var model = new SupplierPeopleListViewModel();

            //Fill a list with the first letters of all user's surnames ...
            model.FirstLetters = _db.SupplierPeoples
                .Where(p => !string.IsNullOrEmpty(p.Surname.Substring(0, 1)))
                .GroupBy(p => p.Surname.Substring(0, 1))
                .Select(x => x.Key.ToUpper())
                .ToList();

            if (string.IsNullOrEmpty(selectedLetter))
            {
                selectedLetter = model.FirstLetters.FirstOrDefault();
            }

            model.SelectedLetter = selectedLetter;

            if (selectedLetter == "All")
            {
                model.Contacts = _db.SupplierPeoples.ToList();
            }
            else
            {
                if (selectedLetter == "0-9")
                {
                    var numbers = Enumerable.Range(0, 10).Select(i => i.ToString());
                    model.Contacts = _db.SupplierPeoples
                        .Where(p => numbers.Contains(p.Surname.Substring(0, 1)))
                        .ToList();
                }
                else if (selectedLetter == "non alpha")
                {
                    //Get a list 
                    var nonalpha1 = Enumerable.Range(32, 16).Select(i => ((char)i).ToString()).ToList();
                    var nonalpha2 = Enumerable.Range(91, 6).Select(i => ((char)i).ToString()).ToList();
                    var nonalpha3 = Enumerable.Range(123, 4).Select(i => ((char)i).ToString()).ToList();
                    IEnumerable<string> nonalpha = nonalpha1.Concat(nonalpha2).Concat(nonalpha3);

                    model.Contacts = _db.SupplierPeoples
                        .Where(p => nonalpha.Contains(p.Surname.Substring(0, 1)))
                        .ToList();
                }
                {
                    model.Contacts = _db.SupplierPeoples
                        .Where(p => p.Surname.StartsWith(selectedLetter))
                        .ToList();
                }
            }
            
            return View(model);
        }


        // GET: LibraryAdmin/SupplierPeople/List/{id} -- passed SupplierID
        [Route("BySupplier")]
        [Route("ListBySupplier")]
        [Route("~/LibraryAdmin/SupplierPeople/ListBySupplier")]
        [Route("~/LibraryAdmin/Contacts/ListBySupplier")]
        public ActionResult ListBySupplier(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var supplierPeople = from people in _db.SupplierPeoples
                                 join addresses in _db.SupplierAddresses on people.AddressID equals addresses.AddressID
                                 where addresses.SupplierID == id
                                 select people;

            return PartialView(supplierPeople.ToList());
        }


        // GET: LibraryAdmin/SupplierPeople/List/{id} -- passed AddressID
        [Route("ByAddress")]
        [Route("ListByAddress")]
        [Route("~/LibraryAdmin/SupplierPeople/ListByAddress")]
        [Route("~/LibraryAdmin/Contacts/ListByAddress")]
        public ActionResult ListByAddress(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var supplierPeople = _db.SupplierPeoples.Where(p => p.AddressID == id);
            return PartialView(supplierPeople.ToList());
        }


        // GET: LibraryAdmin/SupplierPeople/Create -- SupplierID maybe provided
        [Route("Add")]
        [Route("Create")]
        [Route("~/LibraryAdmin/SupplierPeople/Create")]
        [Route("~/LibraryAdmin/Contacts/Create")]
        public ActionResult Create(int supplierId = 0, int addressId = 0, string callingAction = "Add")
        {
            ViewData["SupplierID"] = new SelectList(_db.Suppliers.Where(s => s.SupplierAddresses.Any()).OrderBy(s => s.SupplierName), "SupplierID", "SupplierName", supplierId);

            if (addressId == 0 && supplierId > 0)
            {
                var address = _db.SupplierAddresses.FirstOrDefault(x => x.SupplierID == supplierId);
                if (address != null) addressId = address.AddressID;
            }

            ViewData["AddressID"] = new SelectList(_db.SupplierAddresses.Where(s => s.SupplierID == supplierId).OrderBy(s => s.Division), "AddressID", "Division", addressId);
            ViewBag.Title = "Add New " + _entityName;

            var viewModel = new SupplierPeopleAddViewModel
            {
                SupplierID = supplierId,
                AddressID = addressId,
                CallingAction = callingAction
            };
            return PartialView(viewModel);
        }

        
        // POST: LibraryAdmin/SupplierPeople/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PostCreate(SupplierPeopleAddViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var newContact = new SupplierPeople
                {
                    AddressID = viewModel.AddressID,
                    Title = viewModel.Title,
                    Initials = viewModel.Initials,
                    Firstname = viewModel.Firstname,
                    Surname = viewModel.Surname,
                    Position = viewModel.Position,
                    InputDate = DateTime.Now
                };

                _db.SupplierPeoples.Add(newContact);
                _db.SaveChanges();

                // Add an email address if one has been supplied ...
                if (newContact.ContactID != null && !string.IsNullOrEmpty(viewModel.Email))
                {
                    var commMethodType = _db.CommMethodTypes.FirstOrDefault(m => m.Method == "Work email");
                    if (commMethodType == null)
                    {
                        commMethodType = _db.CommMethodTypes.FirstOrDefault(m => m.Method == "Home Email");
                    }
                    if (commMethodType == null)
                    {
                        commMethodType = _db.CommMethodTypes.FirstOrDefault(m => m.Method == "Email");
                    }
                    if (commMethodType == null)
                    {
                        commMethodType = new CommMethodType()
                        {
                            Method = "Email",
                            CanDelete = true,
                            CanUpdate = true,
                            InputDate = DateTime.Now
                        };
                        _db.CommMethodTypes.Add(commMethodType);
                        _db.SaveChanges();
                    }

                    var newContactMethod = new SupplierPeopleComm()
                    {
                        MethodID = commMethodType.MethodID,
                        ContactID = newContact.ContactID,
                        Detail = viewModel.Email
                    };
                    _db.SupplierPeopleComms.Add(newContactMethod);
                    _db.SaveChanges();
                }

                // Add a phone number of one has been supplied ...
                if (newContact.ContactID != null && !string.IsNullOrEmpty(viewModel.Phone))
                {
                    CommMethodType commMethodType;
                    if (viewModel.Phone.StartsWith("07"))
                    {
                        commMethodType = _db.CommMethodTypes.FirstOrDefault(m => m.Method == "Mobile");
                        if (commMethodType == null)
                        {
                            commMethodType = new CommMethodType()
                            {
                                Method = "Mobile",
                                CanDelete = true,
                                CanUpdate = true,
                                InputDate = DateTime.Now
                            };
                            _db.CommMethodTypes.Add(commMethodType);
                            _db.SaveChanges();
                        }
                    }
                    else
                    {
                        commMethodType = _db.CommMethodTypes.FirstOrDefault(m => m.Method == "Work tel");
                        if (commMethodType == null)
                        {
                            commMethodType = _db.CommMethodTypes.FirstOrDefault(m => m.Method == "Home tel");
                        }
                        if (commMethodType == null)
                        {
                            commMethodType = _db.CommMethodTypes.FirstOrDefault(m => m.Method == "Mobile");
                        }
                        if (commMethodType == null)
                        {
                            commMethodType = new CommMethodType()
                            {
                                Method = "Phone",
                                CanDelete = true,
                                CanUpdate = true,
                                InputDate = DateTime.Now
                            };
                            _db.CommMethodTypes.Add(commMethodType);
                            _db.SaveChanges();
                        }
                    }

                    var newContactMethod = new SupplierPeopleComm()
                    {
                        MethodID = commMethodType.MethodID,
                        ContactID = newContact.ContactID,
                        Detail = viewModel.Phone
                    };
                    _db.SupplierPeopleComms.Add(newContactMethod);
                    _db.SaveChanges();
                }

                if (viewModel.CallingAction == "Create")
                {
                    UrlHelper urlHelper = new UrlHelper(HttpContext.Request.RequestContext);
                    string actionUrl = urlHelper.Action("Edit", "Suppliers", new { id = newContact.SupplierID });
                    return Json(new { success = true, redirectTo = actionUrl });
                }
                if (viewModel.CallingAction == "Add")
                {
                    UrlHelper urlHelper = new UrlHelper(HttpContext.Request.RequestContext);
                    string actionUrl = urlHelper.Action("Edit", "SupplierPeople", new { id = newContact.ContactID });
                    return Json(new { success = true, redirectTo = actionUrl });
                }
                return Json(new { success = true });
            }

            ViewData["SupplierID"] = new SelectList(_db.Suppliers.Where(s => s.SupplierAddresses.Any()).OrderBy(s => s.SupplierName), "SupplierID", "SupplierName", viewModel.SupplierID);
            ViewData["AddressID"] = new SelectList(_db.SupplierAddresses.Where(s => s.SupplierID == viewModel.SupplierID).OrderBy(s => s.Division), "AddressID", "Division");
            ViewBag.Title = "Add New " + _entityName;
            return PartialView("Create", viewModel);
        }

        // GET: LibraryAdmin/SupplierPeople/Edit/5 -- ContactID provided
        [Route("Edit")]
        [Route("Edit/{id}")]
        [Route("~/LibraryAdmin/Contacts/Edit")]
        [Route("~/LibraryAdmin/Contacts/Edit/{id}")]
        [Route("~/LibraryAdmin/SupplierPeople/Edit")]
        [Route("~/LibraryAdmin/SupplierPeople/Edit/{id}")]
        public ActionResult Edit(int? id, string callingController = "Suppliers", string callingAction = "Edit")
        {
            if (id == null || id == 0 )
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                return RedirectToAction("Select");
            }
            var supplierPeople = _db.SupplierPeoples.Find(id);
            if (supplierPeople == null)
            {
                //return HttpNotFound();
                return RedirectToAction("Select");
            }

            var supplierId = _db.SupplierAddresses.Where(a => a.AddressID == supplierPeople.AddressID).Select(a => a.SupplierID).FirstOrDefault();

            var viewModel = new SupplierPeopleEditViewModel
            {
                SupplierID = supplierId.Value,
                ContactID = supplierPeople.ContactID,
                AddressID = supplierPeople.AddressID,
                Title = supplierPeople.Title,
                Initials = supplierPeople.Initials,
                Firstname = supplierPeople.Firstname,
                Surname = supplierPeople.Surname,
                Position = supplierPeople.Position,
                Email = supplierPeople.Email,
                SupplierName = _db.Suppliers.Find(supplierId).SupplierName,
                CallingAction = callingAction,
                CallingController = callingController
            };

            ViewData["SelectContact"] = SelectListHelper.ContactsList(id.Value);
            ViewData["AddressID"] = new SelectList(_db.SupplierAddresses.Where(s => s.SupplierID == supplierId.Value).OrderBy(s => s.Division), "AddressID", "Division", supplierPeople.AddressID);
            var commTypesCount = _db.SupplierPeopleComms.Count(c => c.ContactID == supplierPeople.ContactID);
            ViewBag.CommTypesCount = commTypesCount;
            ViewBag.Title = "View/Edit " + _entityName;
            return View(viewModel);
        }

        // POST: LibraryAdmin/SupplierPeople/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PostEdit([Bind(Include = "ContactID,SupplierID,AddressID,Title,Initials,Surname,Firstname,Position,Email,CallingController,CallingAction")] SupplierPeopleEditViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var supplierPeople = _db.SupplierPeoples.Find(viewModel.ContactID);
                if (supplierPeople == null)
                {
                    return RedirectToAction("Select");
                }
                //supplierPeople.ContactID = viewModel.ContactID;
                supplierPeople.Title = viewModel.Title;
                supplierPeople.AddressID = viewModel.AddressID;
                supplierPeople.Email = viewModel.Email;
                supplierPeople.Firstname = viewModel.Firstname;
                supplierPeople.Initials = viewModel.Initials;
                supplierPeople.Position = viewModel.Position;
                supplierPeople.Surname = viewModel.Surname;
                supplierPeople.LastModified = DateTime.Now;
                _db.Entry(supplierPeople).State = EntityState.Modified;
                _db.SaveChanges();

                switch (viewModel.CallingController)
                {
                    case "SupplierAddresses":
                        return RedirectToAction(viewModel.CallingAction, "SupplierAddresses", new { id = viewModel.AddressID });
                    case "Suppliers":
                        return RedirectToAction(viewModel.CallingAction, "Suppliers", new { id = viewModel.SupplierID });
                    case "SupplierPeople":
                        return RedirectToAction("Index", "SupplierPeople");
                }
            }

            ViewData["SelectContact"] = SelectListHelper.ContactsList(viewModel.ContactID);
            ViewData["AddressID"] = new SelectList(_db.SupplierAddresses.Where(s => s.SupplierID == viewModel.SupplierID).OrderBy(s => s.Division), "AddressID", "Division", viewModel.AddressID);
            var commTypesCount = _db.SupplierPeopleComms.Count(c => c.ContactID == viewModel.ContactID);
            ViewBag.CommTypesCount = commTypesCount;
            ViewBag.Title = "View/Edit " + _entityName;
            return View("Edit", viewModel);
        }

        [Route("EditSupplier/{id}")]
        [Route("~/LibraryAdmin/Contacts/EditSupplier/{id}")]
        [Route("~/LibraryAdmin/SupplierPeople/EditSupplier/{id}")]
        public ActionResult EditSupplier(int id = 0)
        {
            return RedirectToAction("Edit", "Suppliers", new {id });
        }


        //Method used to supply a JSON list of addresses when selecting a supplier (Ajax stuf)
        public JsonResult GetSelectedAddresses(int SupplierID = 0)
        {
            //ViewData["AddressID"] = new SelectList(_db.SupplierAddresses.Where(a => a.SupplierID == SupplierID).ToList(), "AddressID", "Division");
            var addresses = new SelectList(_db.SupplierAddresses.Where(a => a.SupplierID == SupplierID).ToList(), "AddressID", "Division");

            return Json(new
            {
                success = true,
                //SupplierAddressData = ViewData["AddressID"]
                SupplierAddressData = addresses
            });
        }


        [HttpGet]
        public ActionResult Delete(int id = 0, string callingAction = "", string callingController = "")
        {
            var supplierPerson = _db.SupplierPeoples.Find(id);
            if (supplierPerson == null)
            {
                return HttpNotFound();
            }
            if (supplierPerson.Deleted)
            {
                return HttpNotFound();
            }

            // Create new instance of DeleteConfirmationViewModel and pass it to _DeleteConfirmation Dialog (Reusable)
            DeleteConfirmationViewModel deleteConfirmationViewModel = new DeleteConfirmationViewModel
            {
                DeleteEntityId = id,
                HeaderText = _entityName,
                PostDeleteAction = "Delete",
                PostDeleteController = "SupplierPeople",
                CallingAction = callingAction,
                CallingController = callingController,
                DetailsText = supplierPerson.Fullname
            };
            return PartialView("_DeleteConfirmation", deleteConfirmationViewModel);
        }

        [HttpPost]
        public ActionResult Delete(DeleteConfirmationViewModel dcvm)
        {
            // Get item which we need to delete from EntityFramework 
            var item = _db.SupplierPeoples.Find(dcvm.DeleteEntityId);

            if (item == null)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _db.SupplierPeoples.Remove(item);
                    _db.SaveChanges();
                    if ((dcvm.CallingController == "SupplierPeople") && (dcvm.CallingAction == "Edit"))
                    {
                        RedirectToAction("Index");
                    }
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
