﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Expressions;
using System.Web.WebSockets;
using Microsoft.AspNet.Identity.Owin;
using slls.Models;
using slls.Utils;
using slls.Utils.Helpers;
using slls.ViewModels;
using Westwind.Globalization;

namespace slls.Areas.LibraryAdmin
{
    [RouteArea("LibraryAdmin", AreaPrefix = "Admin")]
    [RoutePrefix("Orders")]
    [Route("{action=index}")]
    public class OrderDetailsController : AdminBaseController
    {
        private readonly DbEntities _db = new DbEntities();
        private ApplicationUserManager _userManager;

        private ApplicationUserManager UserManager
        {
            get { return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            set { _userManager = value; }
        }

        // GET: Admin/Orders/ViewAll
        //[Route]
        [Route("Index")]
        [Route("All")]
        [Route("AllOrders")]
        [Route("ViewAll")]
        [Route("~/LibraryAdmin/OrderDetails/Index")]
        public ActionResult Index(int listSupplier = 0, int month = 0, int year = 0)
        {
            if (year == 0)
            {
                year = DateTime.Today.Year;
            }

            if (month == 0)
            {
                month = -1;
            }

            var viewModel = new OrderDetailsListViewModel()
            {
                Month = month,
                Year = year
            };

            var allOrders = year == -1 ? _db.OrderDetails : _db.OrderDetails.Where(o => o.OrderDate.Value.Year == year);
            allOrders = month == -1 ? allOrders : allOrders.Where(o => o.OrderDate.Value.Month == month);
            viewModel.Orders = listSupplier > 0 ? allOrders.Where(o => o.SupplierID == listSupplier) : allOrders;

            //Get a list of months ...
            var months = new Dictionary<int, string> { { -1, "All Months" } };
            for (int i = 0; i < 12; i++)
            {
                months.Add(i + 1, System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.MonthNames[i]);
            }

            //Get a list of years back to 1970 ..
            var years = new Dictionary<int, string> { { -1, "All Years" } };
            for (int i = 1970; i < DateTime.Today.AddYears(1).Year; i++)
            {
                years.Add(i, i.ToString());
            }

            //Get the list of suppliers with orders ...
            var suppliers = (from x in _db.Suppliers
                             join o in allOrders on x.SupplierID equals o.SupplierID
                             select new SupplierList { SupplierId = x.SupplierID, SupplierName = x.SupplierName, Count = allOrders.Count(s => s.SupplierID == x.SupplierID) })
                .Distinct().OrderBy(x => x.SupplierName);

            ViewData["ListSupplier"] = SelectListHelper.SuppliersListCustom(suppliers); //supplierList;
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("ordersSeeAlso", "index");
            ViewData["Months"] = months;
            ViewData["Years"] = years;
            ViewBag.InfoMsg =
                "To limit the list to just those order details you want to see, use the Supplier, Year and Month drop-down lists below:";
            ViewBag.Title = "All " + DbRes.T("Orders", "EntityType");
            return View(viewModel);
        }

        // GET: Admin/Invoices/
        [Route("Invoices")]
        [Route("AllInvoices")]
        [Route("~/LibraryAdmin/OrderDetails/InvoicesBySupplier")]
        [Route("~/LibraryAdmin/OrderDetails/AllInvoices")]
        public ActionResult AllInvoices(int listSupplier = 0, int month = 0, int year = 0)
        {
            if (year == 0)
            {
                year = DateTime.Today.Year;
            }

            if (month == 0)
            {
                month = -1;
            }

            var viewModel = new OrderDetailsListViewModel()
            {
                Month = month,
                Year = year
            };

            var allInvoices =
                from o in
                    _db.OrderDetails
                where (o.InvoiceRef != null && !o.InvoiceRef.Equals(string.Empty))
                select o;

            allInvoices = year == -1 ? allInvoices : allInvoices.Where(i => i.InvoiceDate.Value.Year == year);
            allInvoices = month == -1 ? allInvoices : allInvoices.Where(i => i.InvoiceDate.Value.Month == month);
            viewModel.Orders = listSupplier > 0 ? allInvoices.Where(i => i.SupplierID == listSupplier) : allInvoices;

            //Get a list of months ...
            var months = new Dictionary<int, string> { { -1, "All Months" } };
            for (int i = 0; i < 12; i++)
            {
                months.Add(i + 1, System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.MonthNames[i]);
            }

            //Get a list of years back to 1970 ..
            var years = new Dictionary<int, string> { { -1, "All Years" } };
            for (int i = 1970; i < DateTime.Today.AddYears(1).Year; i++)
            {
                years.Add(i, i.ToString());
            }

            //Get the list of suppliers with invoices ...
            var suppliers = (from x in _db.Suppliers
                             join o in allInvoices on x.SupplierID equals o.SupplierID
                             select new SupplierList { SupplierId = x.SupplierID, SupplierName = x.SupplierName, Count = allInvoices.Count(s => s.SupplierID == x.SupplierID)})
                .Distinct().OrderBy(x => x.SupplierName);
            
            ViewData["Months"] = months;
            ViewData["Years"] = years;
            ViewBag.InfoMsg =
                "To limit the list to just those invoices you want to see, use the Supplier, Year and Month drop-down lists below:";
            ViewData["ListSupplier"] = SelectListHelper.SuppliersListCustom(suppliers);
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("ordersSeeAlso", "AllInvoices");
            ViewBag.Title = "All " + DbRes.T("Invoices", "EntityType");
            return View(viewModel);
        }

        // GET: Admin/Orders/AllReceived
        [Route("Received")]
        [Route("AllReceived")]
        [Route("ReceivedOnly")]
        [Route("ViewReceived")]
        [Route("~/LibraryAdmin/OrderDetails/AllReceived")]
        public ActionResult AllReceived(int listSupplier = 0)
        {
            //Get the list of suppliers with orders ...
            var suppliers = (from x in _db.Suppliers
                             join o in _db.OrderDetails on x.SupplierID equals o.SupplierID
                             where (o.ReceivedDate != null)
                             select new SupplierList { SupplierId = x.SupplierID, SupplierName = x.SupplierName, Count = x.OrderDetails.Count(y => y.ReceivedDate != null) })
                .Distinct().OrderBy(x => x.SupplierName);

            var receivedOrders = _db.OrderDetails.Include(o => o.AccountYear).Include(o => o.BudgetCode).Include(o => o.OrderCategory).Include(o => o.Supplier).Include(o => o.Title)
                .Where(o => o.ReceivedDate != null);
            if (listSupplier > 0)
            {
                receivedOrders = receivedOrders.Where(o => o.SupplierID == listSupplier);
            }

            var viewModel = new OrderDetailsListViewModel()
            {
                Orders = receivedOrders
            };

            ViewData["ListSupplier"] = SelectListHelper.SuppliersListCustom(suppliers);
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("ordersSeeAlso", "AllReceived");
            ViewBag.Title = "Received " + DbRes.T("Orders", "EntityType");
            return View(viewModel);
        }

        // GET: Admin/Orders/AllUnassigned
        [Route("Unassigned")]
        [Route("AllUnassigned")]
        [Route("Unallocated")]
        [Route("AllUnallocated")]
        [Route("~/LibraryAdmin/OrderDetails/AllUnassigned")]
        public ActionResult AllUnassigned(int listSupplier = 0)
        {
            //Get the list of suppliers with unassigned orders ...
            var suppliers = (from x in _db.Suppliers
                             join o in _db.OrderDetails on x.SupplierID equals o.SupplierID
                             where (o.BudgetCodeID == null || o.AccountYearID == null)
                             select new SupplierList { SupplierId = x.SupplierID, SupplierName = x.SupplierName, Count = x.OrderDetails.Count(y => y.BudgetCode == null || y.AccountYearID == null) })
                .Distinct().OrderBy(x => x.SupplierName);
            
            var unallocatedOrders = _db.OrderDetails.Include(o => o.AccountYear).Include(o => o.BudgetCode).Include(o => o.OrderCategory).Include(o => o.Supplier).Include(o => o.Title)
                .Where(o => o.BudgetCodeID == null || o.AccountYearID == null);
            if (listSupplier > 0)
            {
                unallocatedOrders = unallocatedOrders.Where(o => o.SupplierID == listSupplier);
            }

            var viewModel = new OrderDetailsListViewModel()
            {
                Orders = unallocatedOrders
            };

            ViewData["ListSupplier"] = SelectListHelper.SuppliersListCustom(suppliers);
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("ordersSeeAlso", "AlUnassigned");
            ViewBag.Title = "Unallocated " + DbRes.T("Orders", "EntityType");
            return View(viewModel);
        }

        // GET: Admin/Orders/OutstandingOrders
        [Route("Outstanding")]
        [Route("AllOutstanding")]
        [Route("OutstandingOnly")]
        [Route("ViewOutstanding")]
        [Route("~/LibraryAdmin/OrderDetails/AllOutstanding")]
        public ActionResult AllOutstanding(int listSupplier = 0)
        {
            //Get the list of suppliers with outstanding orders ...
            var suppliers = (from x in _db.Suppliers
                             join o in _db.OrderDetails on x.SupplierID equals o.SupplierID
                             where (o.ReceivedDate == null)
                             select new SupplierList { SupplierId = x.SupplierID, SupplierName = x.SupplierName, Count = x.OrderDetails.Count(y => y.ReceivedDate == null) })
                .Distinct().OrderBy(x => x.SupplierName);

            var outstandingOrders = _db.OrderDetails.Include(o => o.AccountYear).Include(o => o.BudgetCode).Include(o => o.OrderCategory).Include(o => o.Supplier).Include(o => o.Title)
                .Where(o => o.ReceivedDate == null);
            if (listSupplier > 0)
            {
                outstandingOrders = outstandingOrders.Where(o => o.SupplierID == listSupplier);
            }

            var viewModel = new OrderDetailsListViewModel()
            {
                Orders = outstandingOrders
            };

            ViewData["ListSupplier"] = SelectListHelper.SuppliersListCustom(suppliers);
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("ordersSeeAlso", "AllOutstanding");
            ViewBag.Title = "Current (Outstanding) " + DbRes.T("Orders", "EntityType");
            return View(viewModel);
        }

        // GET: Admin/Orders/OverdueOrders
        [Route("Overdue")]
        [Route("AllOverdue")]
        [Route("OverdueOnly")]
        [Route("ViewOverdue")]
        [Route("~/LibraryAdmin/OrderDetails/AllOverdue")]
        public ActionResult AllOverdue(int listSupplier = 0)
        {
            //Get the list of suppliers with overdue orders ...
            var today = DateTime.Today; //Today at 00:00:00
            var suppliers = (from x in _db.Suppliers
                             join o in _db.OrderDetails on x.SupplierID equals o.SupplierID
                             where (o.ReceivedDate == null && o.Expected <= today)
                             select new SupplierList { SupplierId = x.SupplierID, SupplierName = x.SupplierName, Count = x.OrderDetails.Count(y => y.ReceivedDate == null && y.Expected <= today) })
                .Distinct().OrderBy(x => x.SupplierName);
            
            var overdueOrders = _db.OrderDetails.Include(o => o.AccountYear).Include(o => o.BudgetCode).Include(o => o.OrderCategory).Include(o => o.Supplier).Include(o => o.Title)
                .Where(o => o.ReceivedDate == null && o.Expected <= today);
            if (listSupplier > 0)
            {
                overdueOrders = overdueOrders.Where(o => o.SupplierID == listSupplier);
            }

            var viewModel = new OrderDetailsListViewModel()
            {
                Orders = overdueOrders
            };

            ViewData["ListSupplier"] = SelectListHelper.SuppliersListCustom(suppliers);
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("ordersSeeAlso", "AllOverdue");
            ViewBag.Title = "Overdue " + DbRes.T("Orders", "EntityType");
            return View(viewModel);
        }

        // GET: Admin/Orders/AllOnApproval
        [Route("OnApproval")]
        [Route("AllOnApproval")]
        [Route("OnApprovalOnly")]
        [Route("ViewOnApproval")]
        [Route("~/LibraryAdmin/OrderDetails/AllOnApproval")]
        public ActionResult AllOnApproval(int listSupplier = 0)
        {
            //Get the list of suppliers with outstanding orders ...
            var suppliers = (from x in _db.Suppliers
                             join o in _db.OrderDetails on x.SupplierID equals o.SupplierID
                             where (o.OnApproval)
                             select new SupplierList { SupplierId = x.SupplierID, SupplierName = x.SupplierName, Count = x.OrderDetails.Count(y => y.OnApproval) })
                .Distinct().OrderBy(x => x.SupplierName);
            
            var onApprovalOrders = _db.OrderDetails.Include(o => o.AccountYear).Include(o => o.BudgetCode).Include(o => o.OrderCategory).Include(o => o.Supplier).Include(o => o.Title)
                .Where(o => o.OnApproval);
            if (listSupplier > 0)
            {
                onApprovalOrders = onApprovalOrders.Where(o => o.SupplierID == listSupplier);
            }

            var viewModel = new OrderDetailsListViewModel()
            {
                Orders = onApprovalOrders,
                AllUsers = UserManager.Users.AsEnumerable()
            };

            ViewData["ListSupplier"] = SelectListHelper.SuppliersListCustom(suppliers);
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("ordersSeeAlso", "AllOnApproval");
            ViewBag.Title = DbRes.T("Orders", "EntityType") + " On Approval";
            return View(viewModel);
        }

        //GET: Select an order ...
        [Route("Select")]
        [Route("SelectOrder")]
        [Route("~/LibraryAdmin/OrderDetails/Select")]
        public ActionResult Select(string callingAction = "edit")
        {
            var viewModel = new SelectOrderViewmodel {Orders = SelectListHelper.OrdersList()};

            ViewData["SeeAlso"] = MenuHelper.SeeAlso("ordersSeeAlso");
            switch (callingAction)
            {
                case "edit":
                {
                    ViewBag.Title = "Edit Order";
                    viewModel.BtnText = "Edit Order";
                    viewModel.Message = "Select an Order to edit";
                    viewModel.ReturnAction = "Edit";
                    break;
                }

                case "print":
                {
                    ViewBag.Title = "Print Order";
                    viewModel.BtnText = "Print Order";
                    viewModel.Message = "Select an Order to Print";
                    viewModel.ReturnAction = "PrintOrder";
                    break;
                }

                case "reprint":
                {
                    ViewBag.Title = "Reprint Order";
                    viewModel.BtnText = "Reprint Order";
                    viewModel.Message = "Select an Order to Reprint";
                    viewModel.ReturnAction = "PrintOrder";
                    break;
                }

                case "duplicate":
                {
                    ViewBag.Title = "Duplicate Order";
                    viewModel.BtnText = "Duplicate Order";
                    viewModel.Message = "Select an Order to Duplicate";
                    viewModel.ReturnAction = "DuplicateOrder";
                    break;
                }

                default:
                {
                    ViewBag.Title = "Select Order";
                    viewModel.BtnText = "Ok";
                    viewModel.Message = "Select an Order";
                    viewModel.ReturnAction = "Edit";
                    break;
                }
            }
            
            return View(viewModel);
        }

        //POST: Select an order ...
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PostSelect(SelectOrderViewmodel viewModel)
        {
            if (viewModel.OrderID == 0)
            {
                return null;
            }
            return RedirectToAction(viewModel.ReturnAction, new { id = viewModel.OrderID });
        }

        public ActionResult PrintOrder(int id = 0)
        {
            if (id == 0)
            {
                return RedirectToAction("Select", new { callingAction = "print" });
            }
            var orderDetail = _db.OrderDetails.Find(id);
            if (orderDetail == null)
            {
                return HttpNotFound();
            }
            if (orderDetail.Deleted)
            {
                return HttpNotFound();
            }

            return View("Reports/PrintOrder", orderDetail);
        }


        // GET: Admin/Orders/BySupplier
        [Route("BySupplier")]
        [Route("~/LibraryAdmin/OrderDetails/OrdersBySupplier")]
        public ActionResult OrdersBySupplier(int listSupplier = 0)
        {
            //Get the list of suppliers with orders ...
            var suppliers = (from x in _db.Suppliers
                             join o in _db.OrderDetails on x.SupplierID equals o.SupplierID
                             select new SupplierList { SupplierId = x.SupplierID, SupplierName = x.SupplierName, Count = x.OrderDetails.Count() })
                .Distinct().OrderBy(x => x.SupplierName);
            
            //Get the actual results if the user has selected anything ...
            var supplierOrders =
                from o in
                    _db.OrderDetails
                where o.SupplierID == listSupplier
                select o;

            var viewModel = new OrderDetailsListViewModel()
            {
                Orders = supplierOrders
            };

            ViewData["ListSupplier"] = SelectListHelper.SuppliersListCustom(suppliers);
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("ordersSeeAlso", "OrdersBySupplier");
            ViewBag.Title = DbRes.T("Orders", "EntityType") + " By " +
                            DbRes.T("Suppliers.Supplier", "FieldDisplayName");
            return View(viewModel);
        }


        // GET: Admin/Orders/ByRequester
        [Route("ByRequester")]
        [Route("~/LibraryAdmin/OrderDetails/OrdersByRequester")]
        public ActionResult OrdersByRequester(string listRequesters = "")
        {
            //Get the list of requesters with orders ...
            //Get the list of authorisers with orders ...
            IEnumerable<SelectListItem> requestersList =
                UserManager.Users.Where(u => u.RequestedOrders.Count > 0)
                    .Select(
                        x =>
                            new SelectListItem
                            {
                                Value = x.Id,
                                Text = x.Lastname + " (" + x.RequestedOrders.Count + ")"
                            });

            //Get the actual results if the user has selected anything ...
            var requesterOrders =
                from o in
                    _db.OrderDetails
                where o.RequesterUser.Id == listRequesters
                select o;

            var viewModel = new OrderDetailsListViewModel()
            {
                Orders = requesterOrders
            };

            ViewData["ListRequesters"] = requestersList;
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("ordersSeeAlso", "OrdersByRequestor");
            ViewBag.Title = DbRes.T("Orders", "EntityType") + " " +
                            DbRes.T("Orders.Requested_By", "FieldDisplayName");
            return View(viewModel);
        }

        // GET: Admin/Orders/ByAuthoriser
        [Route("ByAuthoriser")]
        [Route("~/LibraryAdmin/OrderDetails/OrdersByAuthoriser")]
        public ActionResult OrdersByAuthoriser(string listAuthorisers = "")
        {
            //Get the list of authorisers with orders ...
            IEnumerable<SelectListItem> authorisersList =
                UserManager.Users.Where(u => u.AuthorisedOrders.Count > 0)
                    .Select(
                        x =>
                            new SelectListItem
                            {
                                Value = x.Id,
                                Text = x.Lastname + " (" + x.AuthorisedOrders.Count + ")"
                            });

            //Get the actual results if the user has selected anything ...
            var authoriserOrders =
                from o in
                    _db.OrderDetails
                where o.AuthoriserUser.Id == listAuthorisers
                select o;

            var viewModel = new OrderDetailsListViewModel()
            {
                Orders = authoriserOrders
            };

            ViewData["ListAuthorisers"] = authorisersList;
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("ordersSeeAlso", "OrdersByRequestor");
            ViewBag.Title = DbRes.T("Orders", "EntityType") + " " +
                            DbRes.T("Orders.Authorised_By", "FieldDisplayName");
            return View(viewModel);
        }


        // GET: Admin/Orders/ByTitle
        [Route("ByTitle")]
        [Route("~/LibraryAdmin/OrderDetails/OrdersByTitle")]
        public ActionResult OrdersByTitle(int listTitles = 0)
        {
            //Get the list of ordered titles  ...
            var titles = (from x in _db.Titles
                          where x.OrderDetails.Count > 0
                          select
                              new { x.TitleID, Title = x.Title1 + " (" + x.OrderDetails.Count + ")", x.NonFilingChars })
                .Distinct().OrderBy(x => x.Title.Substring(x.NonFilingChars));

            //Start a new list selectlist items ...
            var titlesList = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select a " + DbRes.T("Titles.Title", "FieldDisplayName"),
                    Value = "0"
                }
            };
            titlesList.AddRange(titles.Select(item => new SelectListItem
            {
                Text = item.Title,
                Value = item.TitleID.ToString()
            }));

            //Add the actual ordered titles ...
            //Get the actual results if the user has selected anything ...
            var titleOrders =
                from o in
                    _db.OrderDetails
                where o.TitleID == listTitles
                select o;

            var viewModel = new OrderDetailsListViewModel()
            {
                Orders = titleOrders
            };

            ViewData["ListTitles"] = titlesList;
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("ordersSeeAlso", "OrdersByTitle");
            ViewBag.Title = DbRes.T("Orders", "EntityType") + " By " +
                            DbRes.T("Titles.Title", "FieldDisplayName");
            return View(viewModel);
        }

        // GET: Admin/Orders/ByBudgetCode
        [Route("ByBudgetCode")]
        [Route("~/LibraryAdmin/OrderDetails/OrdersByBudgetCode")]
        public ActionResult OrdersByBudgetCode(int listBudgetCodes = 0)
        {
            //Get the list of ordered titles  ...
            var budgetCodes = (from x in _db.BudgetCodes
                               where x.OrderDetails.Count > 0
                               select new { x.BudgetCodeID, BudgetCode = x.BudgetCode1 + " (" + x.OrderDetails.Count + ")" })
                .Distinct().OrderBy(x => x.BudgetCode);

            //Start a new list selectlist items ...
            var budgetCodesList = new List<SelectListItem>();
            budgetCodesList.AddRange(budgetCodes.Select(item => new SelectListItem
            {
                Text = item.BudgetCode,
                Value = item.BudgetCodeID.ToString()
            }));

            //Add the actual orders ...
            //Get the actual results if the user has selected anything ...
            var bcOrders =
                from o in
                    _db.OrderDetails
                where o.BudgetCodeID == listBudgetCodes
                select o;

            var viewModel = new OrderDetailsListViewModel()
            {
                Orders = bcOrders
            };

            ViewData["ListBudgetCodes"] = budgetCodesList;
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("ordersSeeAlso", "OrdersByBudgetCode");
            ViewBag.Title = DbRes.T("Orders", "EntityType") + " By " +
                            DbRes.T("BudgetCode.Budget_Code", "FieldDisplayName");

            return View(viewModel);
        }

        // GET: Admin/Orders/ByCategory
        [Route("ByCategory")]
        [Route("~/LibraryAdmin/OrderDetails/OrdersByCategory")]
        public ActionResult OrdersByCategory(int listCategories = 0)
        {
            //Get the list of ordered titles  ...
            var orderCategories = (from x in _db.OrderCategories
                               where x.OrderDetails.Count > 0
                               select new { x.OrderCategoryID, OrderCategory = x.OrderCategory1 + " (" + x.OrderDetails.Count + ")" })
                .Distinct().OrderBy(x => x.OrderCategory);

            //Start a new list selectlist items ...
            var orderCategoriesList = new List<SelectListItem>();
            orderCategoriesList.AddRange(orderCategories.Select(item => new SelectListItem
            {
                Text = item.OrderCategory,
                Value = item.OrderCategoryID.ToString()
            }));

            //Add the actual orders ...
            //Get the actual results if the user has selected anything ...
            var ocOrders =
                from o in
                    _db.OrderDetails
                where o.OrderCategoryID == listCategories
                select o;

            var viewModel = new OrderDetailsListViewModel()
            {
                Orders = ocOrders
            };

            ViewData["listCategories"] = orderCategoriesList;
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("ordersSeeAlso", "OrdersByCategory");
            ViewBag.Title = DbRes.T("Orders", "EntityType") + " By " +
                            DbRes.T("OrderCategories.Order_Category", "FieldDisplayName");

            return View(viewModel);
        }

        // GET: Admin/Orders/ByAccountYear
        [Route("ByAccountYear")]
        [Route("~/LibraryAdmin/OrderDetails/OrdersByAccountYear")]
        public ActionResult OrdersByAccountYear(int listAccountYears = 0)
        {
            //Get the list of AccountYears  ...
            var accountYears = (from x in _db.AccountYears
                                where x.OrderDetails.Count > 0
                                select new { x.AccountYearID, AccountYear = x.AccountYear1 + " (" + x.OrderDetails.Count + ")" })
                .Distinct().OrderBy(x => x.AccountYear);

            //Start a new list selectlist items ...
            var accountYearsList = new List<SelectListItem>();
            accountYearsList.AddRange(accountYears.Select(item => new SelectListItem
            {
                Text = item.AccountYear,
                Value = item.AccountYearID.ToString()
            }));

            //Add the actual orders ...
            //Get the actual results if the user has selected anything ...
            var orders =
                from o in
                    _db.OrderDetails
                where o.AccountYearID == listAccountYears
                select o;

            var viewModel = new OrderDetailsListViewModel()
            {
                Orders = orders
            };

            ViewData["ListAccountYears"] = accountYearsList;
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("ordersSeeAlso", "OrdersByAccountYear");
            ViewBag.Title = DbRes.T("Orders", "EntityType") + " By " +
                            DbRes.T("AccountYears.Account_Year", "FieldDisplayName");
            return View(viewModel);
        }

        // GET: LibraryAdmin/OrderDetails/Add/5
        [Route("Add/{id?}")]
        [Route("~/LibraryAdmin/OrderDetails/Add/{id?}")]
        public ActionResult Add(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var title = _db.Titles.Find(id.Value);

            if (title == null)
            {
                return HttpNotFound();
            }
            if (title.Deleted)
            {
                return HttpNotFound();
            }

            var viewModel = new OrderDetailsAddViewModel
            {
                TitleID = id.Value,
                Title = title.Title1,
                OrderDate = DateTime.Now,
                Expected = DateTime.Now.AddDays(28),
                NumCopies = 1,
                Price = 0,
                VAT = 0,
                //Titles = new SelectList(_db.Titles, "TitleID", "Title1"),
                Suppliers = SelectListHelper.SupplierList(PublicFunctions.GetDefaultValue("OrderDetails", "SupplierID")),
                OrderCategories = SelectListHelper.OrderCategoryList(PublicFunctions.GetDefaultValue("OrderDetails", "OrderCategoryID")),
                BudgetCodes = SelectListHelper.BudgetCodesList(PublicFunctions.GetDefaultValue("OrderDetails", "BudgetCodeID"), "Select a " + DbRes.T("BudgetCode.Budget_Code", "FieldDisplayName"), true, false),
                RequestUsers = new SelectList(UserManager.Users, "Id", "FullnameRev"),
                AuthorityUsers = new SelectList(UserManager.Users, "Id", "FullnameRev"),
                CallingAction = "add"
            };

            ViewBag.Title = "Add " + DbRes.T("Orders.New_Order", "FieldDisplayName");
            return PartialView(viewModel);
        }

        // GET: LibraryAdmin/OrderDetails/Create
        [Route("Create/{id?}")]
        [Route("~/LibraryAdmin/OrderDetails/Create/{id?}")]
        public ActionResult Create(int? id)
        {
            var titleid = id ?? 0;
            var viewModel = new OrderDetailsAddViewModel
            {
                NumCopies = 1,
                Price = 0,
                VAT = 0,
                OrderDate = DateTime.Now,
                Expected = DateTime.Now.AddDays(28),
                Titles = new SelectList(_db.Titles, "TitleID", "Title1", titleid),
                Suppliers = SelectListHelper.SupplierList(PublicFunctions.GetDefaultValue("OrderDetails", "SupplierID")),
                OrderCategories = SelectListHelper.OrderCategoryList(PublicFunctions.GetDefaultValue("OrderDetails", "OrderCategoryID")),
                BudgetCodes = SelectListHelper.BudgetCodesList(PublicFunctions.GetDefaultValue("OrderDetails", "BudgetCodeID"), "Select a " + DbRes.T("BudgetCode.Budget_Code", "FieldDisplayName"), true, false),
                RequestUsers = new SelectList(UserManager.Users, "Id", "FullnameRev"),
                AuthorityUsers = new SelectList(UserManager.Users, "Id", "FullnameRev"),
                CallingAction = "create"
            };

            ViewBag.Title = "Add " + DbRes.T("Orders.New_Order", "FieldDisplayName");
            return PartialView(viewModel);
        }

        // POST: LibraryAdmin/OrderDetails/Create
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult PostCreate(OrderDetailsAddViewModel viewModel)
        {
            var newOrder = new OrderDetail
            {
                TitleID = viewModel.TitleID ?? 0,
                OrderCategoryID = viewModel.OrderCategoryID == 0 ? 1 : viewModel.OrderCategoryID,
                OrderDate = viewModel.OrderDate ?? DateTime.Now,
                OrderNo = viewModel.OrderNo,
                Expected = viewModel.Expected ?? DateTime.Now.AddDays(28),
                RequesterUser = _db.Users.Find(viewModel.RequestedBy),
                AuthoriserUser = _db.Users.Find(viewModel.Authority),
                NumCopies = viewModel.NumCopies ?? 1,
                Price = viewModel.Price ?? 0,
                VAT = viewModel.VAT ?? 0,
                SupplierID = viewModel.SupplierID == 0 ? 1 : viewModel.SupplierID,
                Notes = viewModel.Notes
            };

            if (ModelState.IsValid)
            {
                _db.OrderDetails.Add(newOrder);
                _db.SaveChanges();
                //return viewModel.CallingAction == "add" ? RedirectToAction("Edit", "Titles", new { id = viewModel.TitleID }) : RedirectToAction("AllOutstanding", new { listSupplier = viewModel.SupplierID });
                return Json(new { success = true });
            }
            
            return RedirectToAction("Add");
        }

        public ActionResult DuplicateOrder(int id = 0)
        {
            if (id == 0)
            {
                return RedirectToAction("Select", new { callingAction = "duplicate" });
            }
            
            var orderDetail = _db.OrderDetails.Find(id);
            if (orderDetail == null)
            {
                return HttpNotFound();
            }
            if (orderDetail.Deleted)
            {
                return HttpNotFound();
            }

            var orderId = 0;
            var newOrder = new OrderDetail
            {
                TitleID = orderDetail.TitleID,
                OrderCategoryID = orderDetail.OrderCategoryID,
                OrderDate = DateTime.Now,
                OrderNo = orderDetail.OrderNo + " (duplicate)",
                Expected = DateTime.Now.AddDays(28),
                RequesterUser = orderDetail.RequesterUser,
                AuthoriserUser = orderDetail.AuthoriserUser,
                NumCopies = 1,
                Price = 0,
                VAT = 0,
                SupplierID = orderDetail.SupplierID == 0 ? 1 : orderDetail.SupplierID
            };

            if (ModelState.IsValid)
            {
                _db.OrderDetails.Add(newOrder);
                _db.SaveChanges();
                orderId = newOrder.OrderID;
            }

            return orderId > 0 ? RedirectToAction("Edit", new { id = orderId }) : RedirectToAction("Index");
        }

        // GET: LibraryAdmin/OrderDetails/EditTitle/5
        [Route("EditTitle/{id?}")]
        [Route("~/LibraryAdmin/OrderDetails/EditTitle/{id}")]
        public ActionResult EditTitle(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return RedirectToAction("Edit", "Titles", new { id });
        }

        // GET: LibraryAdmin/OrderDetails/Edit/5
        [Route("Edit/{id?}")]
        [Route("~/LibraryAdmin/OrderDetails/Edit/{id}")]
        public ActionResult Edit(int? id, string callingController = "orderdetails")
        {
            if (id == 0)
            {
                return RedirectToAction("Select", new { callingAction = "edit"});
            }
            var orderDetail = _db.OrderDetails.Find(id);
            if (orderDetail == null)
            {
                return HttpNotFound();
            }
            if (orderDetail.Deleted)
            {
                return HttpNotFound();
            }

            var viewModel = new OrderDetailsEditViewModel
            {
                OrderID = orderDetail.OrderID,
                OrderDate = orderDetail.OrderDate,
                OrderCategoryID = orderDetail.OrderCategoryID,
                OrderNo = orderDetail.OrderNo,
                OnApproval = orderDetail.OnApproval,
                AccountYearID = orderDetail.AccountYearID,
                Authority = orderDetail.AuthoriserUser == null ? null : orderDetail.AuthoriserUser.Id,
                Accepted = orderDetail.Accepted,
                BudgetCodeID = orderDetail.BudgetCodeID,
                RequestedBy = orderDetail.RequesterUser == null ? null : orderDetail.RequesterUser.Id,
                Cancelled = orderDetail.Cancelled,
                Chased = orderDetail.Chased,
                NumCopies = orderDetail.NumCopies,
                InvoiceDate = orderDetail.InvoiceDate,
                MonthSubDue = orderDetail.MonthSubDue,
                ReceivedDate = orderDetail.ReceivedDate,
                ReturnedDate = orderDetail.ReturnedDate,
                Expected = orderDetail.Expected,
                InvoiceRef = orderDetail.InvoiceRef,
                Item = orderDetail.Item,
                TitleID = orderDetail.TitleID,
                Link = orderDetail.Link,
                Notes = orderDetail.Notes,
                Price = orderDetail.Price,
                Passed = orderDetail.Passed,
                Report = orderDetail.Report,
                SupplierID = orderDetail.SupplierID,
                VAT = orderDetail.VAT,
                Titles = new SelectList(_db.Titles, "TitleID", "Title1"),
                Suppliers = new SelectList(_db.Suppliers, "SupplierID", "SupplierName"),
                RequestUsers = new SelectList(UserManager.Users, "Id", "FullnameRev"),
                AuthorityUsers = new SelectList(UserManager.Users, "Id", "FullnameRev"),
                CallingController = callingController
            };

            //if (viewModel.CallingController == "titles")
            //{
            //    //Get all copies for this title ...
            //    var query = _db.OrderDetails
            //        .Where(x => x.TitleID == orderDetail.TitleID)
            //        .OrderByDescending(x => x.OrderDate)
            //        .ThenBy(x => x.OrderNo);

            //    //Get the index of where the current OrderID is in a list ordered by OrderNo ASC ..
            //    var rowIndex = query.AsEnumerable().Select((x, index) => new { x.OrderID, index }).First(i => i.OrderID == id).index;

            //    //Get the first OrderID ...
            //    var firstId = query.AsEnumerable().Select(i => i.OrderID).First();

            //    //Get the last OrderID ...
            //    var lastId = query.AsEnumerable().Select(i => i.OrderID).Last();

            //    //Get the next OrderID to move forward to ...
            //    var nextId = query.AsEnumerable()
            //        .OrderBy(i => i.OrderNo)
            //        .SkipWhile(i => i.OrderID != id)
            //        .Skip(1)
            //        .Select(i => i.OrderID).FirstOrDefault();

            //    //Get the previous OrderID to move back to ...
            //    var previousId = query.AsEnumerable()
            //        .OrderByDescending(i => i.OrderNo)
            //        .SkipWhile(i => i.OrderID != id)
            //        .Skip(1)
            //        .Select(i => i.OrderID).FirstOrDefault();

            //    ViewBag.FirstID = firstId;
            //    ViewBag.LastID = lastId;
            //    ViewBag.NextID = nextId;
            //    ViewBag.PreviousID = previousId;
            //    ViewBag.RowIndex = rowIndex + 1;
            //    ViewBag.RecordCount = query.Count();
            //    ViewBag.RecordType = "Order";

            //}

            ViewData["selectOrder"] = SelectListHelper.OrdersList(id.Value);
            ViewData["AccountYearID"] = new SelectList(_db.AccountYears, "AccountYearID", "AccountYear1", orderDetail.AccountYearID);
            ViewData["BudgetCodeID"] = new SelectList(_db.BudgetCodes, "BudgetCodeID", "BudgetCode1", orderDetail.BudgetCodeID);
            ViewData["OrderCategoryID"] = new SelectList(_db.OrderCategories, "OrderCategoryID", "OrderCategory1", orderDetail.OrderCategoryID);
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("ordersSeeAlso", "Edit");
            ViewBag.Title = "Order Details";
            return View(viewModel);
        }

        // POST: LibraryAdmin/OrderDetails/Update/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update([Bind(Include = "OrderID,OrderNo,OrderDate,SupplierID,TitleID,Item,NumCopies,Price,VAT,Authority,RequestedBy,OnApproval,Expected,AccountYearID,OrderCategoryID,BudgetCodeID,Cancelled,Chased,Report,ReceivedDate,Accepted,ReturnedDate,InvoiceRef,Passed,MonthSubDue,InvoiceDate,Link,Notes,CallingController")] OrderDetailsEditViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var orderDetail = _db.OrderDetails.Find(viewModel.OrderID);
                if (orderDetail == null)
                {
                    return HttpNotFound();
                }
                if (orderDetail.Deleted)
                {
                    return HttpNotFound();
                }
                orderDetail.OrderID = viewModel.OrderID;
                orderDetail.AccountYearID = viewModel.AccountYearID;
                orderDetail.Accepted = viewModel.Accepted;
                orderDetail.AuthoriserUser = _db.Users.Find(viewModel.Authority);
                orderDetail.OnApproval = viewModel.OnApproval;
                orderDetail.BudgetCodeID = viewModel.BudgetCodeID;
                orderDetail.RequesterUser = _db.Users.Find(viewModel.RequestedBy);
                orderDetail.Chased = viewModel.Chased;
                orderDetail.Cancelled = viewModel.Cancelled;
                orderDetail.NumCopies = viewModel.NumCopies;
                orderDetail.OrderCategoryID = viewModel.OrderCategoryID;
                orderDetail.InvoiceDate = viewModel.InvoiceDate;
                orderDetail.MonthSubDue = viewModel.MonthSubDue;
                orderDetail.OrderDate = viewModel.OrderDate;
                orderDetail.ReceivedDate = viewModel.ReceivedDate;
                orderDetail.ReturnedDate = viewModel.ReturnedDate;
                orderDetail.Expected = viewModel.Expected;
                orderDetail.InvoiceRef = viewModel.InvoiceRef;
                orderDetail.Item = viewModel.Item;
                orderDetail.TitleID = viewModel.TitleID;
                orderDetail.SupplierID = viewModel.SupplierID;
                orderDetail.Link = viewModel.Link;
                orderDetail.Notes = viewModel.Notes;
                orderDetail.OrderNo = viewModel.OrderNo;
                orderDetail.Price = viewModel.Price;
                orderDetail.Passed = viewModel.Passed;
                orderDetail.Report = viewModel.Report;
                orderDetail.VAT = viewModel.VAT;

                _db.Entry(orderDetail).State = EntityState.Modified;
                _db.SaveChanges();
                if (viewModel.CallingController == "orderdetails")
                {
                    return RedirectToAction("Index");
                }
                return RedirectToAction("Edit", "Titles", new { id = viewModel.TitleID });
            }
            ViewData["selectOrder"] = SelectListHelper.OrdersList(viewModel.OrderID);
            ViewData["AccountYearID"] = new SelectList(_db.AccountYears, "AccountYearID", "AccountYear1", viewModel.AccountYearID);
            ViewData["BudgetCodeID"] = new SelectList(_db.BudgetCodes, "BudgetCodeID", "BudgetCode1", viewModel.BudgetCodeID);
            ViewData["Authority"] = new SelectList(UserManager.Users, "Id", "Username", viewModel.Authority);
            ViewData["RequestedBy"] = new SelectList(UserManager.Users, "Id", "Username", viewModel.RequestedBy);
            ViewData["OrderCategoryID"] = new SelectList(_db.OrderCategories, "OrderCategoryID", "OrderCategory1", viewModel.OrderCategoryID);
            ViewBag.Title = "Order Details";
            return View("Edit", viewModel);
        }

        // GET: LibraryAdmin/OrderDetails/Edit/5
        [Route("AddReceipt/{id?}")]
        [Route("~/LibraryAdmin/OrderDetails/AddReceipt/{id}")]
        public ActionResult AddReceipt(int? id, string callingAction = "index")
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderDetail orderDetail = _db.OrderDetails.Find(id);
            if (orderDetail == null)
            {
                return HttpNotFound();
            }
            if (orderDetail.Deleted)
            {
                return HttpNotFound();
            }

            var viewModel = new OrderReceiptsViewModel
            {
                OrderID = orderDetail.OrderID,
                OrderCategoryID = orderDetail.OrderCategoryID,
                OrderNo = orderDetail.OrderNo,
                AccountYearID = orderDetail.AccountYearID,
                BudgetCodeID = orderDetail.BudgetCodeID,
                InvoiceDate = orderDetail.InvoiceDate ?? DateTime.Now,
                ReceivedDate = orderDetail.ReceivedDate ?? DateTime.Now,
                InvoiceRef = orderDetail.InvoiceRef,
                TitleID = orderDetail.TitleID,
                Title = orderDetail.Title.Title1,
                Link = orderDetail.Link,
                Notes = orderDetail.Notes,
                Price = orderDetail.Price,
                Passed = orderDetail.Passed ?? DateTime.Now,
                VAT = orderDetail.VAT,
                CallingController = "OrderDetails",
                CallingAction = callingAction
            };

            ViewData["AccountYearID"] = new SelectList(_db.AccountYears, "AccountYearID", "AccountYear1", orderDetail.AccountYearID);
            ViewData["BudgetCodeID"] = new SelectList(_db.BudgetCodes, "BudgetCodeID", "BudgetCode1", orderDetail.BudgetCodeID);
            ViewData["OrderCategoryID"] = new SelectList(_db.OrderCategories, "OrderCategoryID", "OrderCategory1", orderDetail.OrderCategoryID);
            ViewBag.Title = "Order Receipt";
            return PartialView(viewModel);
        }

        // POST: LibraryAdmin/OrderDetails/DoReceipt/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DoReceipt([Bind(Include = "OrderID,OrderNo,TitleID,Price,VAT,OnApproval,AccountYearID,OrderCategoryID,BudgetCodeID,ReceivedDate,Accepted,InvoiceRef,Passed,InvoiceDate,Link,Notes,CallingController,CallingAction")] OrderReceiptsViewModel viewModel)
        {
            var orderDetail = _db.OrderDetails.Find(viewModel.OrderID);
            if (orderDetail == null)
            {
                return HttpNotFound();
            }
            if (orderDetail.Deleted)
            {
                return HttpNotFound();
            }

            if (ModelState.IsValid)
            {
                orderDetail.ReceivedDate = viewModel.ReceivedDate ?? DateTime.Now;
                orderDetail.InvoiceDate = viewModel.InvoiceDate;
                orderDetail.InvoiceRef = viewModel.InvoiceRef;
                orderDetail.Passed = viewModel.Passed;
                orderDetail.Link = viewModel.Link;
                orderDetail.BudgetCodeID = viewModel.BudgetCodeID;
                orderDetail.OrderCategoryID = viewModel.OrderCategoryID;
                orderDetail.AccountYearID = viewModel.AccountYearID;
                orderDetail.Price = viewModel.Price;
                orderDetail.VAT = viewModel.VAT;

                _db.Entry(orderDetail).State = EntityState.Modified;
                _db.SaveChanges();

                if (!string.IsNullOrEmpty(viewModel.CallingAction))
                {
                    return RedirectToAction(viewModel.CallingAction);
                }
                return RedirectToAction("Index");

            }
            return PartialView("AddReceipt", viewModel);
        }



        [Route("ReportGenerator")]
        [Route("~/LibraryAdmin/OrderDetails/ReportGenerator")]
        public ActionResult ReportGenerator()
        {
            if (TempData["NoReportSelected"] != null)
            {
                if ((bool)TempData["NoReportSelected"] == true)
                {
                    TempData["NoReportSelected"] = false;
                    ModelState.AddModelError("NotSelected", "No report selected! Please select a report from the list.");
                }
            }

            if (TempData["NoData"] != null)
            {
                if ((bool)TempData["NoData"] == true)
                {
                    TempData["NoData"] = false;
                    ModelState.AddModelError("NoData", "There is no data for this report! Please select some alternative criteria and try again.");
                }
            }

            var viewModel = new ReportsGeneratorViewModel
            {
                Reports = SelectListHelper.FinanceReportsList(),
                BudgetCodes = SelectListHelper.BudgetCodesList(addDefault: true, addNew: false, msg: "All"),
                AccountYears = SelectListHelper.AccountYearsList(addDefault: false, addNew: false, msg: null),
                OrderCategories = SelectListHelper.OrderCategoriesList(addDefault: true, addNew: false, msg: "All"),
                StartDate = DateTime.Now.AddYears(-1),
                EndDate = DateTime.Now.Date
            };

            ViewBag.Title = "Expenditure Report Generator";
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult PostReportGenerator(ReportsGeneratorViewModel viewModel)
        {
            var reportId = 0;
            var reportName = "";
            var friendlyName = "";
            var budgetCodeId = "";
            var accountYearId = 0;
            var startDate = DateTime.Now.AddYears(-1);
            var endDate = DateTime.Now;

            //Get the single selected report ...
            if (viewModel.SelectedReport != null)
            {
                reportId = viewModel.SelectedReport.FirstOrDefault();
            };

            if (reportId == 0)
            {
                TempData["NoReportSelected"] = true;
                return RedirectToAction("ReportGenerator");
            }

            //Get one or more budget codes to filter by ...
            if (viewModel.SelectedBudgetCode != null)
            {
                budgetCodeId = string.Join(",", viewModel.SelectedBudgetCode);
            }

            //Get the single selected AccountYear ...
            if (viewModel.SelectedAccountYear != null)
            {
                accountYearId = viewModel.SelectedAccountYear.FirstOrDefault();
            }

            //If accountYearId == 0 then get the start and end dates ...
            if (viewModel.StartDate != null)
            {
                startDate = viewModel.StartDate.Date;
            }

            if (viewModel.EndDate != null)
            {
                endDate = viewModel.EndDate.Date;
            }

            if (reportId == 0)
            {
                ViewBag.Title = "Reports Generator";
                return View("ReportGenerator", viewModel);
            }

            var selectedReport = _db.ReportTypes.Find(reportId);
            reportName = selectedReport.ReportName;
            friendlyName = selectedReport.FriendlyName;

            switch (reportName)
            {
                case "AccountSummary":
                    {
                        return RedirectToAction("AccountSummary_Report", new { caption = friendlyName, budgetCodeId, accountYearId, startDate, endDate });
                    }
                case "AccountByBudgetCode":
                        {
                            return RedirectToAction("AccountByBudgetCode", new { caption = friendlyName, budgetCodeId, accountYearId, startDate, endDate });
                        }
                case "AccountByMedia":
                        {
                            return RedirectToAction("AccountByMedia", new { caption = friendlyName, budgetCodeId, accountYearId, startDate, endDate });
                        }
                case "AccountBySupplier":
                        {
                            return RedirectToAction("AccountBySupplier", new { caption = friendlyName, budgetCodeId, accountYearId, startDate, endDate });
                        }
                case "AccountOneOffsSummary":
                        {
                            return RedirectToAction("AccountOneOffsSummary", new { caption = friendlyName, budgetCodeId, accountYearId, startDate, endDate });
                        }
                case "AccountSubscriptionsSummary":
                        {
                            return RedirectToAction("AccountSubscriptionsSummary", new { caption = friendlyName, budgetCodeId, accountYearId, startDate, endDate });
                        }
                case "AccountSinglePurchasesByBudgetCode":
                        {
                            return RedirectToAction("AccountSinglePurchasesByBudgetCode", new { caption = friendlyName, budgetCodeId, accountYearId, startDate, endDate });
                        }
                case "AccountSinglePurchasesByMedia":
                        {
                            return RedirectToAction("AccountSinglePurchasesByMedia", new { caption = friendlyName, budgetCodeId, accountYearId, startDate, endDate });
                        }
                case "AccountSinglePurchasesBySupplier":
                        {
                            return RedirectToAction("AccountSinglePurchasesBySupplier", new { caption = friendlyName, budgetCodeId, accountYearId, startDate, endDate });
                        }
                case "AccountSubscriptionsByBudgetCode":
                        {
                            return RedirectToAction("AccountSubscriptionsByBudgetCode", new { caption = friendlyName, budgetCodeId, accountYearId, startDate, endDate });
                        }
                case "AccountSubscriptionsByMedia":
                        {
                            return RedirectToAction("AccountSubscriptionsByMedia", new { caption = friendlyName, budgetCodeId, accountYearId, startDate, endDate });
                        }
                case "AccountSubscriptionsDetailByMedia":
                        {
                            return RedirectToAction("AccountSubscriptionsDetailByMedia", new { caption = friendlyName, budgetCodeId, accountYearId, startDate, endDate });
                        }
                case "AccountSubscriptionsBySupplier":
                        {
                            return RedirectToAction("AccountSubscriptionsBySupplier", new { caption = friendlyName, budgetCodeId, accountYearId, startDate, endDate });
                        }
                case "AccountSubscriptionsDetailByBudgetCode":
                        {
                            return RedirectToAction("AccountSubscriptionsDetailByBudgetCode", new { caption = friendlyName, budgetCodeId, accountYearId, startDate, endDate });
                        }
                case "AccountSinglePurchasesDetailByBudgetCode":
                        {
                            return RedirectToAction("AccountSinglePurchasesDetailByBudgetCode", new { caption = friendlyName, budgetCodeId, accountYearId, startDate, endDate });
                        }
                case "AccountSinglePurchasesDetailByMedia":
                        {
                            return RedirectToAction("AccountSinglePurchasesDetailByMedia", new { caption = friendlyName, budgetCodeId, accountYearId, startDate, endDate });
                        }
                case "AccountSinglePurchasesDetailBySupplier":
                        {
                            return RedirectToAction("AccountSinglePurchasesDetailBySupplier", new { caption = friendlyName, budgetCodeId, accountYearId, startDate, endDate });
                        }
                case "AccountSubscriptionsDetailBySupplier":
                        {
                            return RedirectToAction("AccountSubscriptionsDetailBySupplier", new { caption = friendlyName, budgetCodeId, accountYearId, startDate, endDate });
                        }
                case "Cancellations":
                        {
                            return RedirectToAction("Cancellations_Report", new { caption = friendlyName, accountYearId, startDate, endDate });
                        }
                default:
                    {
                        return null;
                    }
            }
        }

        public ActionResult Cancellations_Report(string caption, int accountYearId, DateTime startDate, DateTime endDate )
        {
            var cancelledCopies = from c in _db.Copies
                where c.Cancellation != null
                select c;

            if (accountYearId > 0)
            {
                cancelledCopies = from c in cancelledCopies
                         where c.AccountYearID == accountYearId
                         select c;
            }
            else
            {
                cancelledCopies = from c in cancelledCopies
                         where c.Cancellation >= startDate && c.Cancellation <= endDate
                         select c;
            }
            
            var accountYear = _db.AccountYears.Find(accountYearId).AccountYear1;

            var viewModel = new OrderReportsViewModel
            {
                AccountYear = accountYear,
                Copies = cancelledCopies,
                HasData = cancelledCopies.Any()
            };

            ViewBag.Title = caption;
            return View("Reports/Cancellations", viewModel);
        }

        public ActionResult AccountSummary_Report(string caption, string budgetCodeId, int accountYearId, DateTime startDate, DateTime endDate)
        {
            var orders = from o in _db.OrderDetails
                         where o.ReceivedDate != null
                         select o;

            if (accountYearId > 0)
            {
                orders = from o in orders
                         where o.AccountYearID == accountYearId
                         select o;
            }
            else
            {
                orders = from o in orders
                         where o.ReceivedDate >= startDate && o.ReceivedDate <= endDate
                         select o;
            }

            if (budgetCodeId != null)
            {
                if (budgetCodeId != "0")
                {
                    orders = from o in orders
                             where budgetCodeId.Contains(o.BudgetCodeID.ToString())
                             select o;
                }
            }

            //if (!orders.Any())
            //{
            //    TempData["NoData"] = true;
            //    return RedirectToAction("ReportGenerator");
            //}

            var orderCategories = (from c in orders
                                   select c.OrderCategory).Distinct();

            //if (!orderCategories.Any())
            //{
            //    TempData["NoData"] = true;
            //    return RedirectToAction("ReportGenerator");
            //}

            var accountYear = _db.AccountYears.Find(accountYearId).AccountYear1;

            var viewModel = new OrderReportsViewModel
            {
                OrderCategories = orderCategories,
                AccountYearId = accountYearId,
                AccountYear = accountYear,
                BudgetCodeId = budgetCodeId,
                StartDate = startDate,
                EndDate = endDate,
                HasData = orderCategories.Any()
            };

            ViewBag.Title = caption;
            return View("Reports/AccountSummary", viewModel);
        }

        public ActionResult AccountSinglePurchasesByBudgetCode(string caption, string budgetCodeId, int accountYearId, DateTime startDate, DateTime endDate)
        {
            var allOrders = from o in _db.OrderDetails
                         where o.ReceivedDate != null && o.OrderCategory.Sub == false
                         select o;

            if (accountYearId > 0)
            {
                allOrders = from o in allOrders
                         where o.AccountYearID == accountYearId
                         select o;
            }
            else
            {
                allOrders = from o in allOrders
                         where o.ReceivedDate >= startDate && o.ReceivedDate <= endDate
                         select o;
            }

            if (budgetCodeId != null)
            {
                if (budgetCodeId != "0")
                {
                    allOrders = from o in allOrders
                             where budgetCodeId.Contains(o.BudgetCodeID.ToString())
                             select o;
                }
            }

            //if (!allOrders.Any())
            //{
            //    TempData["NoData"] = true;
            //    return RedirectToAction("ReportGenerator");
            //}

            var allBudgetCodes = (from o in allOrders
                                  where o.BudgetCodeID != null
                                  select o.BudgetCode).Distinct();
            
            var accountYear = _db.AccountYears.Find(accountYearId).AccountYear1;

            var viewModel = new OrderReportsViewModel
            {
                Orders = allOrders,
                BudgetCodes = allBudgetCodes,
                AccountYearId = accountYearId,
                AccountYear = accountYear,
                BudgetCodeId = budgetCodeId,
                StartDate = startDate,
                EndDate = endDate,
                HasData = allBudgetCodes.Any()
            };

            ViewBag.Title = caption;
            return View("Reports/AccountByBudgetCode", viewModel);
        }

        public ActionResult AccountSubscriptionsByBudgetCode(string caption, string budgetCodeId, int accountYearId, DateTime startDate, DateTime endDate)
        {
            var allOrders = from o in _db.OrderDetails
                            where o.ReceivedDate != null && o.OrderCategory.Sub
                            select o;

            if (accountYearId > 0)
            {
                allOrders = from o in allOrders
                            where o.AccountYearID == accountYearId
                            select o;
            }
            else
            {
                allOrders = from o in allOrders
                            where o.ReceivedDate >= startDate && o.ReceivedDate <= endDate
                            select o;
            }

            if (budgetCodeId != null)
            {
                if (budgetCodeId != "0")
                {
                    allOrders = from o in allOrders
                                where budgetCodeId.Contains(o.BudgetCodeID.ToString())
                                select o;
                }
            }

            //if (!allOrders.Any())
            //{
            //    TempData["NoData"] = true;
            //    return RedirectToAction("ReportGenerator");
            //}

            var allBudgetCodes = (from o in allOrders
                                  where o.BudgetCodeID != null
                                  select o.BudgetCode).Distinct();

            var accountYear = _db.AccountYears.Find(accountYearId).AccountYear1;

            var viewModel = new OrderReportsViewModel
            {
                Orders = allOrders,
                BudgetCodes = allBudgetCodes,
                AccountYearId = accountYearId,
                AccountYear = accountYear,
                BudgetCodeId = budgetCodeId,
                StartDate = startDate,
                EndDate = endDate,
                HasData = allBudgetCodes.Any()
            };

            ViewBag.Title = caption;
            return View("Reports/AccountByBudgetCode", viewModel);
        }

        public ActionResult AccountSubscriptionsDetailByBudgetCode(string caption, string budgetCodeId, int accountYearId, DateTime startDate, DateTime endDate)
        {
            var allOrders = from o in _db.OrderDetails
                            where o.ReceivedDate != null && o.OrderCategory.Sub
                            select o;

            if (accountYearId > 0)
            {
                allOrders = from o in allOrders
                            where o.AccountYearID == accountYearId
                            select o;
            }
            else
            {
                allOrders = from o in allOrders
                            where o.ReceivedDate >= startDate && o.ReceivedDate <= endDate
                            select o;
            }

            if (budgetCodeId != null)
            {
                if (budgetCodeId != "0")
                {
                    allOrders = from o in allOrders
                                where budgetCodeId.Contains(o.BudgetCodeID.ToString())
                                select o;
                }
            }

            //if (!allOrders.Any())
            //{
            //    TempData["NoData"] = true;
            //    return RedirectToAction("ReportGenerator");
            //}

            var allBudgetCodes = (from o in allOrders
                                  where o.BudgetCodeID != null
                                  select o.BudgetCode).Distinct();

            var accountYear = _db.AccountYears.Find(accountYearId).AccountYear1;

            var viewModel = new OrderReportsViewModel
            {
                Orders = allOrders,
                BudgetCodes = allBudgetCodes,
                AccountYearId = accountYearId,
                AccountYear = accountYear,
                BudgetCodeId = budgetCodeId,
                StartDate = startDate,
                EndDate = endDate,
                Sub = true,
                HasData = allBudgetCodes.Any()
            };

            ViewBag.Title = caption;
            return View("Reports/AccountDetailByBudgetCode", viewModel);
        }

        public ActionResult AccountSinglePurchasesDetailByBudgetCode(string caption, string budgetCodeId, int accountYearId, DateTime startDate, DateTime endDate)
        {
            var allOrders = from o in _db.OrderDetails
                            where o.ReceivedDate != null && o.OrderCategory.Sub == false
                            select o;

            if (accountYearId > 0)
            {
                allOrders = from o in allOrders
                            where o.AccountYearID == accountYearId
                            select o;
            }
            else
            {
                allOrders = from o in allOrders
                            where o.ReceivedDate >= startDate && o.ReceivedDate <= endDate
                            select o;
            }

            if (budgetCodeId != null)
            {
                if (budgetCodeId != "0")
                {
                    allOrders = from o in allOrders
                                where budgetCodeId.Contains(o.BudgetCodeID.ToString())
                                select o;
                }
            }

            //if (!allOrders.Any())
            //{
            //    TempData["NoData"] = true;
            //    return RedirectToAction("ReportGenerator");
            //}

            var allBudgetCodes = (from o in allOrders
                                  where o.BudgetCodeID != null
                                  select o.BudgetCode).Distinct();

            var accountYear = _db.AccountYears.Find(accountYearId).AccountYear1;

            var viewModel = new OrderReportsViewModel
            {
                Orders = allOrders,
                BudgetCodes = allBudgetCodes,
                AccountYearId = accountYearId,
                AccountYear = accountYear,
                BudgetCodeId = budgetCodeId,
                StartDate = startDate,
                EndDate = endDate,
                Sub = false,
                OneOff = true,
                HasData = allBudgetCodes.Any()
            };

            ViewBag.Title = caption;
            return View("Reports/AccountDetailByBudgetCode", viewModel);
        }

        public ActionResult AccountSinglePurchasesDetailByMedia(string caption, string budgetCodeId, int accountYearId, DateTime startDate, DateTime endDate)
        {
            var allOrders = from o in _db.OrderDetails
                            where o.ReceivedDate != null && o.OrderCategory.Sub == false
                            select o;

            if (accountYearId > 0)
            {
                allOrders = from o in allOrders
                            where o.AccountYearID == accountYearId
                            select o;
            }
            else
            {
                allOrders = from o in allOrders
                            where o.ReceivedDate >= startDate && o.ReceivedDate <= endDate
                            select o;
            }

            if (budgetCodeId != null)
            {
                if (budgetCodeId != "0")
                {
                    allOrders = from o in allOrders
                                where budgetCodeId.Contains(o.BudgetCodeID.ToString())
                                select o;
                }
            }

            //if (!allOrders.Any())
            //{
            //    TempData["NoData"] = true;
            //    return RedirectToAction("ReportGenerator");
            //}

            var allTitles = (from o in allOrders
                             select o.Title).Distinct();

            var allMediaTypes = (from t in allTitles
                                 select t.MediaType).Distinct();

            var accountYear = _db.AccountYears.Find(accountYearId).AccountYear1;

            var viewModel = new OrderReportsViewModel
            {
                Orders = allOrders,
                Titles = allTitles,
                MediaTypes = allMediaTypes,
                AccountYear = accountYear,
                BudgetCodeId = budgetCodeId,
                StartDate = startDate,
                EndDate = endDate,
                HasData = allOrders.Any()
            };

            ViewBag.Title = caption;
            return View("Reports/AccountDetailByMedia", viewModel);
        }

        public ActionResult AccountSinglePurchasesDetailBySupplier(string caption, string budgetCodeId, int accountYearId, DateTime startDate, DateTime endDate)
        {
            var allOrders = from o in _db.OrderDetails
                            where o.ReceivedDate != null && o.OrderCategory.Sub == false
                            select o;

            if (accountYearId > 0)
            {
                allOrders = from o in allOrders
                            where o.AccountYearID == accountYearId
                            select o;
            }
            else
            {
                allOrders = from o in allOrders
                            where o.ReceivedDate >= startDate && o.ReceivedDate <= endDate
                            select o;
            }

            if (budgetCodeId != null)
            {
                if (budgetCodeId != "0")
                {
                    allOrders = from o in allOrders
                                where budgetCodeId.Contains(o.BudgetCodeID.ToString())
                                select o;
                }
            }

            //if (!allOrders.Any())
            //{
            //    TempData["NoData"] = true;
            //    return RedirectToAction("ReportGenerator");
            //}

            var allTitles = (from o in allOrders
                             select o.Title).Distinct();
            
            var allSuppliers = (from o in allOrders
                                select o.Supplier).Distinct();

            var accountYear = _db.AccountYears.Find(accountYearId).AccountYear1;

            var viewModel = new OrderReportsViewModel
            {
                Orders = allOrders,
                Titles = allTitles,
                Suppliers = allSuppliers,
                AccountYear = accountYear,
                BudgetCodeId = budgetCodeId,
                StartDate = startDate,
                EndDate = endDate,
                HasData = allOrders.Any()
            };

            ViewBag.Title = caption;
            return View("Reports/AccountDetailBySupplier", viewModel);
        }

        public ActionResult AccountOneOffsSummary(string caption, string budgetCodeId, int accountYearId, DateTime startDate, DateTime endDate)
        {
            var allOrders = from o in _db.OrderDetails
                         where o.ReceivedDate != null && o.OrderCategory.Sub == false
                         select o;

            if (accountYearId > 0)
            {
                allOrders = from o in allOrders
                         where o.AccountYearID == accountYearId
                         select o;
            }
            else
            {
                allOrders = from o in allOrders
                         where o.ReceivedDate >= startDate && o.ReceivedDate <= endDate
                         select o;
            }

            if (budgetCodeId != null)
            {
                if (budgetCodeId != "0")
                {
                    allOrders = from o in allOrders
                             where budgetCodeId.Contains(o.BudgetCodeID.ToString())
                             select o;
                }
            }

            //if (!allOrders.Any())
            //{
            //    TempData["NoData"] = true;
            //    return RedirectToAction("ReportGenerator");
            //}

            var allBudgetCodes = (from o in allOrders
                               where o.BudgetCodeID != null
                               select o.BudgetCode).Distinct();
            
            var accountYear = _db.AccountYears.Find(accountYearId).AccountYear1;

            var viewModel = new OrderReportsViewModel
            {
                Orders = allOrders,
                BudgetCodes = allBudgetCodes,
                AccountYearId = accountYearId,
                AccountYear = accountYear,
                BudgetCodeId = budgetCodeId,
                StartDate = startDate,
                EndDate = endDate,
                HasData = allBudgetCodes.Any()
            };

            ViewBag.Title = caption;
            return View("Reports/AccountOneOffsSummary", viewModel);
        }

        public ActionResult AccountSubscriptionsSummary(string caption, string budgetCodeId, int accountYearId, DateTime startDate, DateTime endDate)
        {
            var allOrders = from o in _db.OrderDetails
                            where o.ReceivedDate != null && o.OrderCategory.Sub
                            select o;

            if (accountYearId > 0)
            {
                allOrders = from o in allOrders
                            where o.AccountYearID == accountYearId
                            select o;
            }
            else
            {
                allOrders = from o in allOrders
                            where o.ReceivedDate >= startDate && o.ReceivedDate <= endDate
                            select o;
            }

            if (budgetCodeId != null)
            {
                if (budgetCodeId != "0")
                {
                    allOrders = from o in allOrders
                                where budgetCodeId.Contains(o.BudgetCodeID.ToString())
                                select o;
                }
            }

            //if (!allOrders.Any())
            //{
            //    TempData["NoData"] = true;
            //    return RedirectToAction("ReportGenerator");
            //}

            var allBudgetCodes = (from o in allOrders
                                  where o.BudgetCodeID != null
                                  select o.BudgetCode).Distinct();

            var accountYear = _db.AccountYears.Find(accountYearId).AccountYear1;

            var viewModel = new OrderReportsViewModel
            {
                Orders = allOrders,
                BudgetCodes = allBudgetCodes,
                AccountYearId = accountYearId,
                AccountYear = accountYear,
                BudgetCodeId = budgetCodeId,
                StartDate = startDate,
                EndDate = endDate,
                HasData = allBudgetCodes.Any()
            };

            ViewBag.Title = caption;
            return View("Reports/AccountSubscriptionsSummary", viewModel);
        }

        public ActionResult AccountByBudgetCode(string caption, string budgetCodeIdString, int accountYearId, DateTime startDate, DateTime endDate)
        {
            var orders = from o in _db.OrderDetails
                         where o.ReceivedDate != null
                         select o;
            
            if (accountYearId > 0)
            {
                orders = from o in orders
                         where o.AccountYearID == accountYearId
                         select o;
            }
            else
            {
                orders = from o in orders
                         where o.ReceivedDate >= startDate && o.ReceivedDate <= endDate
                         select o;
            }

            if (budgetCodeIdString != null)
            {
                if (budgetCodeIdString != "0")
                {
                    orders = from o in orders
                             where budgetCodeIdString.Contains(o.BudgetCodeID.ToString())
                             select o;
                }
            }

            //if (!orders.Any())
            //{
            //    TempData["NoData"] = true;
            //    return RedirectToAction("ReportGenerator");
            //}

            var budgetCodes = (from o in orders
                               where o.BudgetCodeID != null
                               select o.BudgetCode).Distinct();

            var accountYear = _db.AccountYears.Find(accountYearId).AccountYear1;

            var viewModel = new OrderReportsViewModel
            {
                //Orders = orders,
                BudgetCodes = budgetCodes,
                AccountYearId = accountYearId,
                AccountYear = accountYear,
                StartDate = startDate,
                EndDate = endDate,
                HasData = budgetCodes.Any()
            };

            ViewBag.Title = caption;
            return View("Reports/AccountByBudgetCode", viewModel);
        }

        public ActionResult AccountByMedia(string caption, string budgetCodeIdString, int accountYearId, DateTime startDate, DateTime endDate)
        {
            var allOrders = from o in _db.OrderDetails
                         where o.ReceivedDate != null
                         select o;

            if (accountYearId > 0)
            {
                allOrders = from o in allOrders
                         where o.AccountYearID == accountYearId
                         select o;
            }
            else
            {
                allOrders = from o in allOrders
                         where o.ReceivedDate >= startDate && o.ReceivedDate <= endDate
                         select o;
            }

            if (budgetCodeIdString != null)
            {
                if (budgetCodeIdString != "0")
                {
                    allOrders = from o in allOrders
                             where budgetCodeIdString.Contains(o.BudgetCodeID.ToString())
                             select o;
                }
            }

            //if (!allOrders.Any())
            //{
            //    TempData["NoData"] = true;
            //    return RedirectToAction("ReportGenerator");
            //}

            var allTitles = (from o in allOrders
                          select o.Title).Distinct();

            var allMediaTypes = (from t in allTitles
                              select t.MediaType).Distinct();

            var accountYear = _db.AccountYears.Find(accountYearId).AccountYear1;

            var viewModel = new OrderReportsViewModel
            {
                Orders = allOrders,
                Titles = allTitles,
                MediaTypes = allMediaTypes,
                AccountYearId = accountYearId,
                AccountYear = accountYear,
                StartDate = startDate,
                EndDate = endDate,
                HasData = allOrders.Any()
            };

            ViewBag.Title = caption;
            return View("Reports/AccountByMedia", viewModel);
        }

        public ActionResult AccountSinglePurchasesByMedia(string caption, string budgetCodeIdString, int accountYearId, DateTime startDate, DateTime endDate)
        {
            var allOrders = from o in _db.OrderDetails
                            where o.ReceivedDate != null && o.OrderCategory.Sub == false
                            select o;

            if (accountYearId > 0)
            {
                allOrders = from o in allOrders
                            where o.AccountYearID == accountYearId
                            select o;
            }
            else
            {
                allOrders = from o in allOrders
                            where o.ReceivedDate >= startDate && o.ReceivedDate <= endDate
                            select o;
            }

            if (budgetCodeIdString != null)
            {
                if (budgetCodeIdString != "0")
                {
                    allOrders = from o in allOrders
                                where budgetCodeIdString.Contains(o.BudgetCodeID.ToString())
                                select o;
                }
            }

            //if (!allOrders.Any())
            //{
            //    TempData["NoData"] = true;
            //    return RedirectToAction("ReportGenerator");
            //}

            var allTitles = (from o in allOrders
                             select o.Title).Distinct();

            var allMediaTypes = (from t in allTitles
                                 select t.MediaType).Distinct();

            var accountYear = _db.AccountYears.Find(accountYearId).AccountYear1;

            var viewModel = new OrderReportsViewModel
            {
                Orders = allOrders,
                Titles = allTitles,
                MediaTypes = allMediaTypes,
                AccountYearId = accountYearId,
                AccountYear = accountYear,
                StartDate = startDate,
                EndDate = endDate,
                HasData = allOrders.Any()
            };

            ViewBag.Title = caption;
            return View("Reports/AccountByMedia", viewModel);
        }

        public ActionResult AccountSubscriptionsByMedia(string caption, string budgetCodeIdString, int accountYearId, DateTime startDate, DateTime endDate)
        {
            var allOrders = from o in _db.OrderDetails
                            where o.ReceivedDate != null && o.OrderCategory.Sub
                            select o;

            if (accountYearId > 0)
            {
                allOrders = from o in allOrders
                            where o.AccountYearID == accountYearId
                            select o;
            }
            else
            {
                allOrders = from o in allOrders
                            where o.ReceivedDate >= startDate && o.ReceivedDate <= endDate
                            select o;
            }

            if (budgetCodeIdString != null)
            {
                if (budgetCodeIdString != "0")
                {
                    allOrders = from o in allOrders
                                where budgetCodeIdString.Contains(o.BudgetCodeID.ToString())
                                select o;
                }
            }

            //if (!allOrders.Any())
            //{
            //    TempData["NoData"] = true;
            //    return RedirectToAction("ReportGenerator");
            //}

            var allTitles = (from o in allOrders
                             select o.Title).Distinct();

            var allMediaTypes = (from t in allTitles
                                 select t.MediaType).Distinct();

            var accountYear = _db.AccountYears.Find(accountYearId).AccountYear1;

            var viewModel = new OrderReportsViewModel
            {
                Orders = allOrders,
                Titles = allTitles,
                MediaTypes = allMediaTypes,
                AccountYearId = accountYearId,
                AccountYear = accountYear,
                StartDate = startDate,
                EndDate = endDate,
                HasData = allOrders.Any()
            };

            ViewBag.Title = caption;
            return View("Reports/AccountByMedia", viewModel);
        }

        public ActionResult AccountSubscriptionsDetailByMedia(string caption, string budgetCodeIdString, int accountYearId, DateTime startDate, DateTime endDate)
        {
            var allOrders = from o in _db.OrderDetails
                            where o.ReceivedDate != null && o.OrderCategory.Sub
                            select o;

            if (accountYearId > 0)
            {
                allOrders = from o in allOrders
                            where o.AccountYearID == accountYearId
                            select o;
            }
            else
            {
                allOrders = from o in allOrders
                            where o.ReceivedDate >= startDate && o.ReceivedDate <= endDate
                            select o;
            }

            if (budgetCodeIdString != null)
            {
                if (budgetCodeIdString != "0")
                {
                    allOrders = from o in allOrders
                                where budgetCodeIdString.Contains(o.BudgetCodeID.ToString())
                                select o;
                }
            }

            //if (!allOrders.Any())
            //{
            //    TempData["NoData"] = true;
            //    return RedirectToAction("ReportGenerator");
            //}

            var allTitles = (from o in allOrders
                             select o.Title).Distinct();

            var allMediaTypes = (from t in allTitles
                                 select t.MediaType).Distinct();

            var accountYear = _db.AccountYears.Find(accountYearId).AccountYear1;

            var viewModel = new OrderReportsViewModel
            {
                Orders = allOrders,
                Titles = allTitles,
                MediaTypes = allMediaTypes,
                AccountYearId = accountYearId,
                AccountYear = accountYear,
                StartDate = startDate,
                EndDate = endDate,
                HasData = allOrders.Any()
            };

            ViewBag.Title = caption;
            return View("Reports/AccountDetailByMedia", viewModel);
        }

        public ActionResult AccountBySupplier(string caption, string budgetCodeIdString, int accountYearId, DateTime startDate, DateTime endDate)
        {
            var allOrders = from o in _db.OrderDetails
                            where o.ReceivedDate != null
                            select o;

            if (accountYearId > 0)
            {
                allOrders = from o in allOrders
                            where o.AccountYearID == accountYearId
                            select o;
            }
            else
            {
                allOrders = from o in allOrders
                            where o.ReceivedDate >= startDate && o.ReceivedDate <= endDate
                            select o;
            }

            if (budgetCodeIdString != null)
            {
                if (budgetCodeIdString != "0")
                {
                    allOrders = from o in allOrders
                                where budgetCodeIdString.Contains(o.BudgetCodeID.ToString())
                                select o;
                }
            }

            //if (!allOrders.Any())
            //{
            //    TempData["NoData"] = true;
            //    return RedirectToAction("ReportGenerator");
            //}
            
            var allSuppliers = (from o in allOrders
                                select o.Supplier).Distinct();

            var accountYear = _db.AccountYears.Find(accountYearId).AccountYear1;

            var viewModel = new OrderReportsViewModel
            {
                Orders = allOrders,
                Suppliers = allSuppliers,
                AccountYearId = accountYearId,
                AccountYear = accountYear,
                StartDate = startDate,
                EndDate = endDate,
                HasData = allOrders.Any()
            };

            ViewBag.Title = caption;
            return View("Reports/AccountBySupplier", viewModel);
        }

        public ActionResult AccountSinglePurchasesBySupplier(string caption, string budgetCodeIdString, int accountYearId, DateTime startDate, DateTime endDate)
        {
            var allOrders = from o in _db.OrderDetails
                            where o.ReceivedDate != null && o.OrderCategory.Sub == false
                            select o;

            if (accountYearId > 0)
            {
                allOrders = from o in allOrders
                            where o.AccountYearID == accountYearId
                            select o;
            }
            else
            {
                allOrders = from o in allOrders
                            where o.ReceivedDate >= startDate && o.ReceivedDate <= endDate
                            select o;
            }

            if (budgetCodeIdString != null)
            {
                if (budgetCodeIdString != "0")
                {
                    allOrders = from o in allOrders
                                where budgetCodeIdString.Contains(o.BudgetCodeID.ToString())
                                select o;
                }
            }

            //if (!allOrders.Any())
            //{
            //    TempData["NoData"] = true;
            //    return RedirectToAction("ReportGenerator");
            //}
            
            var allSuppliers = (from o in allOrders
                                select o.Supplier).Distinct();

            var accountYear = _db.AccountYears.Find(accountYearId).AccountYear1;

            var viewModel = new OrderReportsViewModel
            {
                Orders = allOrders,
                Suppliers = allSuppliers,
                AccountYearId = accountYearId,
                AccountYear = accountYear,
                StartDate = startDate,
                EndDate = endDate,
                HasData = allOrders.Any()
            };

            ViewBag.Title = caption;
            return View("Reports/AccountBySupplier", viewModel);
        }

        public ActionResult AccountSubscriptionsBySupplier(string caption, string budgetCodeIdString, int accountYearId, DateTime startDate, DateTime endDate)
        {
            var allOrders = from o in _db.OrderDetails
                            where o.ReceivedDate != null && o.OrderCategory.Sub
                            select o;

            if (accountYearId > 0)
            {
                allOrders = from o in allOrders
                            where o.AccountYearID == accountYearId
                            select o;
            }
            else
            {
                allOrders = from o in allOrders
                            where o.ReceivedDate >= startDate && o.ReceivedDate <= endDate
                            select o;
            }

            if (budgetCodeIdString != null)
            {
                if (budgetCodeIdString != "0")
                {
                    allOrders = from o in allOrders
                                where budgetCodeIdString.Contains(o.BudgetCodeID.ToString())
                                select o;
                }
            }

            //if (!allOrders.Any())
            //{
            //    TempData["NoData"] = true;
            //    return RedirectToAction("ReportGenerator");
            //}
            
            var allSuppliers = (from o in allOrders
                                select o.Supplier).Distinct();

            var accountYear = _db.AccountYears.Find(accountYearId).AccountYear1;

            var viewModel = new OrderReportsViewModel
            {
                Orders = allOrders,
                Suppliers = allSuppliers,
                AccountYearId = accountYearId,
                AccountYear = accountYear,
                StartDate = startDate,
                EndDate = endDate,
                HasData = allOrders.Any()
            };

            ViewBag.Title = caption;
            return View("Reports/AccountBySupplier", viewModel);
        }

        public ActionResult AccountSubscriptionsDetailBySupplier(string caption, string budgetCodeIdString, int accountYearId, DateTime startDate, DateTime endDate)
        {
            var allOrders = from o in _db.OrderDetails
                            where o.ReceivedDate != null && o.OrderCategory.Sub
                            select o;

            if (accountYearId > 0)
            {
                allOrders = from o in allOrders
                            where o.AccountYearID == accountYearId
                            select o;
            }
            else
            {
                allOrders = from o in allOrders
                            where o.ReceivedDate >= startDate && o.ReceivedDate <= endDate
                            select o;
            }

            if (budgetCodeIdString != null)
            {
                if (budgetCodeIdString != "0")
                {
                    allOrders = from o in allOrders
                                where budgetCodeIdString.Contains(o.BudgetCodeID.ToString())
                                select o;
                }
            }

            //if (!allOrders.Any())
            //{
            //    TempData["NoData"] = true;
            //    return RedirectToAction("ReportGenerator");
            //}

            var allSuppliers = (from o in allOrders
                                select o.Supplier).Distinct();

            var accountYear = _db.AccountYears.Find(accountYearId).AccountYear1;

            var viewModel = new OrderReportsViewModel
            {
                Orders = allOrders,
                Suppliers = allSuppliers,
                AccountYearId = accountYearId,
                AccountYear = accountYear,
                StartDate = startDate,
                EndDate = endDate,
                HasData = allOrders.Any()
            };

            ViewBag.Title = caption;
            return View("Reports/AccountDetailBySupplier", viewModel);
        }

        public ActionResult ItemsOnOrder_Report()
        {
            var allOrders = from o in _db.OrderDetails
                where o.ReceivedDate == null && o.Cancelled == null
                select o;

            var suppliers = (from o in allOrders
                             select o.Supplier).Distinct();

            var viewModel = new OrderReportsViewModel
            {
                Suppliers = suppliers,
                HasData = allOrders.Any()
            };

            ViewBag.Title = "Items On Order";
            return View("Reports/ItemsOnOrder", viewModel);
        }

        public ActionResult StandingOrders_Report()
        {
            var allOrders = from o in _db.OrderDetails
                where o.OrderCategory.OrderCategory1.ToLower() == "standing order"
                select o;

            var suppliers = (from o in allOrders
                             select o.Supplier).Distinct();

            var viewModel = new OrderReportsViewModel
            {
                Suppliers = suppliers,
                Orders = allOrders,
                HasData = allOrders.Any()
            };

            ViewBag.Title = "Standing Orders";
            return View("Reports/StandingOrders", viewModel);
        }

        public ActionResult OverdueItemsOnOrder_Report()
        {
            var allOrders = from o in _db.OrderDetails
                where o.ReceivedDate == null && o.Cancelled == null && o.Expected < DateTime.Today
                select o;
            
            var suppliers = (from o in allOrders
                             select o.Supplier).Distinct();

            var viewModel = new OrderReportsViewModel
            {
                Orders = allOrders,
                Suppliers = suppliers,
                HasData = allOrders.Any()
            };

            ViewBag.Title = "Overdue Items On Order";
            return View("Reports/OverdueItemsOnOrder", viewModel);
        }

        public ActionResult ItemsOnOrderByBudgetCode_Report()
        {
            var allOrders = from o in _db.OrderDetails
                            where o.ReceivedDate == null && o.Cancelled == null && o.BudgetCodeID != null
                            select o;

            var budgetCodes = (from o in allOrders
                               select o.BudgetCode).Distinct();

            var totalSum = (from o in _db.OrderDetails
                            where o.BudgetCodeID != null && o.ReceivedDate == null && o.Cancelled == null
                            select o.Price).Sum();

            var viewModel = new OrderReportsViewModel
            {
                BudgetCodes = budgetCodes,
                Orders = allOrders,
                HasData = allOrders.Any()
            };

            ViewBag.Title = "Items On Order By Budget Code";
            ViewData["TotalSum"] = totalSum;
            return View("Reports/ItemsOnOrderByBudgetCode", viewModel);
        }

        public ActionResult ChasedOrders_Report()
        {
            var allOrders = from o in _db.OrderDetails
                            where o.ReceivedDate == null && o.Cancelled == null && (o.Chased != null || !string.IsNullOrEmpty(o.Report))
                            select o;

            var suppliers = (from o in allOrders
                             select o.Supplier).Distinct();

            var totalSum = (from o in allOrders
                            select o.Price).Sum();

            var viewModel = new OrderReportsViewModel
            {
                Suppliers = suppliers,
                Orders = allOrders,
                HasData = allOrders.Any()
            };

            ViewBag.Title = "Chased Orders By Supplier";
            ViewData["TotalSum"] = totalSum;
            return View("Reports/ChasedOrders", viewModel);
        }

        public ActionResult ItemsOnApproval_Report()
        {
            var allOrders = from o in _db.OrderDetails
                             where o.OnApproval && !o.Accepted && o.ReturnedDate == null
                             select o;

            var allUsers = UserManager.Users.AsEnumerable();

            var suppliers = (from o in allOrders
                             select o.Supplier).Distinct();

            var totalSum = (from o in allOrders
                            select o.Price).Sum();

            var viewModel = new OrderReportsViewModel
            {
                Suppliers = suppliers,
                Orders = allOrders,
                AllUsers = allUsers,
                HasData = allOrders.Any()
            };

            ViewBag.Title = "Items on Approval By Supplier";
            ViewData["TotalSum"] = totalSum;
            return View("Reports/ItemsOnApproval", viewModel);
        }

        //public ActionResult _noData(string caption = "")
        //{
        //    ViewBag.Title = caption;
        //    ViewBag.Msg = "There is no data for this report. Please try again using different criteria";
        //    return PartialView();
        //}


        public ActionResult OrdersByRequestor_Report()
        {
            // Create new instance of AccountYearSelectorViewModel and pass it to _AuthoritySelector Dialog (Reusable)
            var viewModel = new AuthoritySelectorViewModel()
            {
                HeaderText = "Report: Orders Grouped By Requestor",
                PostAction = "OrdersByRequestor_Report",
                PostController = "OrderDetails",
                DetailsText = "Please select an Account Year from the list below.",
                LabelText = "Account Year:",
                AuthorityList = SelectListHelper.AccountYearsList(addDefault: false, addNew: false, msg: null),
                ButtonText = "View Report"
            };
            return PartialView("_AuthoritySelector", viewModel);
        }

        public ActionResult OrdersByAuthoriser_Report()
        {
            // Create new instance of AccountYearSelectorViewModel and pass it to _AuthoritySelector Dialog (Reusable)
            var viewModel = new AuthoritySelectorViewModel()
            {
                HeaderText = "Report: Orders Grouped By Authoriser",
                PostAction = "OrdersByAuthoriser_Report",
                PostController = "OrderDetails",
                DetailsText = "Please select an Account Year from the list below.",
                LabelText = "Account Year:",
                AuthorityList = SelectListHelper.AccountYearsList(addDefault: false, addNew: false, msg: null),
                ButtonText = "View Report"
            };
            return PartialView("_AuthoritySelector", viewModel);
        }

        [HttpPost]
        public ActionResult OrdersByRequestor_Report(AuthoritySelectorViewModel selectedAccountYear)
        {
            var accountYearId = selectedAccountYear.SelectedValue.FirstOrDefault();
            var accountYear = _db.AccountYears.Find(accountYearId).AccountYear1;

            var allOrders = (from o in _db.OrderDetails
                where o.AccountYearID == accountYearId && o.RequesterUser != null
                select o).AsEnumerable();

            var users = UserManager.Users.AsEnumerable();

            //var requestors = (from o in allOrders
            //                   join u in users on o.RequesterUser equals u.Id
            //                   select u).Distinct();

            var requestors = (from o in allOrders
                            select o.RequesterUser).Distinct();
            
            var viewModel = new OrderReportsViewModel()
            {
                AccountYearId = accountYearId,
                AccountYear = accountYear,
                Requestors = requestors,
                Orders = allOrders,
                HasData = requestors.Any(),
                NoDataMsg = "Sorry, there are no requested orders for the selected account year."
            };

            ViewBag.Title = "Orders Grouped By Requestor";
            return View("Reports/OrdersByRequestor", viewModel);
        }

        [HttpPost]
        public ActionResult OrdersByAuthoriser_Report(AuthoritySelectorViewModel selectedAccountYear)
        {
            var accountYearId = selectedAccountYear.SelectedValue.FirstOrDefault();
            var accountYear = _db.AccountYears.Find(accountYearId).AccountYear1;

            var allOrders = (from o in _db.OrderDetails
                            where o.AccountYearID == accountYearId && o.AuthoriserUser != null
                            select o).AsEnumerable();
            
            var users = UserManager.Users.AsEnumerable();

            var authorisers = (from o in allOrders
                              select o.AuthoriserUser).Distinct();

            var viewModel = new OrderReportsViewModel()
            {
                AccountYearId = accountYearId,
                AccountYear = accountYear,
                Authorisers = authorisers,
                Orders = allOrders,
                HasData = authorisers.Any()
            };

            ViewBag.Title = "Orders Grouped By Authoriser";
            return View("Reports/OrdersByAuthoriser", viewModel);
        }

        
        // GET: LibraryAdmin/OrderDetails/Delete/5
        public ActionResult Delete(int? id, string callingController = "orderdetails")
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderDetail orderDetail = _db.OrderDetails.Find(id);
            if (orderDetail == null)
            {
                return HttpNotFound();
            }
            if (orderDetail.Deleted)
            {
                return HttpNotFound();
            }

            var viewModel = new OrderDetailsDeleteViewModel
            {
                OrderID = orderDetail.OrderID,
                OrderDate = orderDetail.OrderDate,
                OrderNo = orderDetail.OrderNo,
                Supplier = orderDetail.Supplier.SupplierName,
                TitleID = orderDetail.TitleID,
                Title = orderDetail.Title.Title1,
                Expected = orderDetail.Expected,
                ReceivedDate = orderDetail.ReceivedDate,
                CallingController = callingController
            };

            ViewBag.Title = "Delete Order?";
            return PartialView(viewModel);
        }

        // POST: LibraryAdmin/OrderDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, OrderDetailsDeleteViewModel viewModel)
        {
            OrderDetail orderDetail = _db.OrderDetails.Find(id);
            _db.OrderDetails.Remove(orderDetail);
            _db.SaveChanges();

            //return RedirectToAction("Index");
            if (viewModel.CallingController == "orderdetails")
            {
                return RedirectToAction("Index");
            }
            return RedirectToAction("Edit", "Titles", new { id = viewModel.TitleID });
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

