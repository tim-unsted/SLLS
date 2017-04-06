using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Expressions;
using System.Web.UI.WebControls.Expressions;
using System.Web.WebSockets;
using Microsoft.AspNet.Identity.Owin;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using slls.Models;
using slls.Utils;
using slls.Utils.Helpers;
using slls.ViewModels;
using Westwind.Globalization;

namespace slls.Areas.LibraryAdmin
{
    //[RouteArea("LibraryAdmin", AreaPrefix = "Admin")]
    public class OrderDetailsController : FinanceBaseController
    {
        private readonly DbEntities _db = new DbEntities();
        private ApplicationUserManager _userManager;
        private readonly string _entityName = DbRes.T("Orders.Order", "FieldDisplayName");
        private readonly string _entityType = DbRes.T("Orders", "EntityType");

        private ApplicationUserManager UserManager
        {
            get { return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            set { _userManager = value; }
        }

        public OrderDetailsController()
        {
            ViewBag.Title = DbRes.T("Orders", "EntityType");
        }

        // GET: Admin/Orders/ViewAll
        public ActionResult Index(int listSupplier = 0, int year = 0)
        {
            var viewModel = new OrderDetailsListViewModel()
            {
                Year = year
            };

            var allOrders =
                from o in _db.OrderDetails
                select o;

            IEnumerable<OrderDetail> filteredOrders;
            IEnumerable<OrderDetail> supplierOrders;
            IEnumerable<OrderDetail> yearOrders;

            //Initialise some objects to hold years and months ...
            var years = new Dictionary<int, string> { { -1, "All Years" } };

            if (listSupplier > 0 && year == 0)
            {
                year = -1;
                viewModel.Year = -1;
            }
            if (listSupplier == 0 && year > 0)
            {
                listSupplier = -1;
            }

            if (year > 0 && listSupplier == -1)
            {
                filteredOrders = from o in allOrders where o.OrderDate.Value.Year == year select o;
                supplierOrders = filteredOrders;
                yearOrders = allOrders;
            }
            else if (year == -1 && listSupplier > 0)
            {
                filteredOrders = from o in allOrders where o.SupplierID == listSupplier select o;
                supplierOrders = allOrders;
                yearOrders = from o in allOrders where o.SupplierID == listSupplier select o;
            }
            else if (year == -1 && listSupplier == -1)
            {
                filteredOrders = from o in allOrders  select o;
                supplierOrders = from o in allOrders  select o;
                yearOrders = from o in allOrders select o;
            }
            else if (year > 0 && listSupplier > 0)
            {
                filteredOrders = from o in allOrders where o.OrderDate.Value.Year == year && o.SupplierID == listSupplier select o;
                supplierOrders = from o in allOrders where o.OrderDate.Value.Year == year select o;
                yearOrders = from o in allOrders where o.SupplierID == listSupplier select o;
            }
            else
            {
                filteredOrders = from o in allOrders where o.OrderDate.Value.Year == -1 && o.SupplierID == -1 select o;
                supplierOrders = allOrders;
                yearOrders = allOrders;
            }

            viewModel.Orders = filteredOrders;

            var suppliers = (from x in _db.Suppliers
                             join o in supplierOrders on x.SupplierID equals o.SupplierID
                             select
                                 new SupplierList
                                 {
                                     SupplierId = x.SupplierID,
                                     SupplierName = x.SupplierName,
                                     Count = supplierOrders.Count(s => s.SupplierID == x.SupplierID)
                                 })
                .Distinct().OrderBy(x => x.SupplierName);
            ViewData["ListSupplier"] = SelectListHelper.SuppliersListCustom(suppliers, addDefault: false);


            var orderYears = (from o in yearOrders select new { Year = o.OrderDate.Value.Year }).Distinct();
            foreach (var item in orderYears.OrderBy(y => y.Year))
            {
                years.Add(item.Year, item.Year.ToString());
            }

            ViewData["Years"] = years;
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("ordersSeeAlso", "index");
            ViewBag.InfoMsg =
                "To limit the list to just those " + _entityType.ToLower() + " you want to see, use the Year and Supplier drop-down lists below:";
            ViewBag.Title = "All " + DbRes.T("Orders", "EntityType") + " (Historical)";
            return View(viewModel);
        }

        // GET: Admin/Invoices/
        public ActionResult AllInvoices(int listSupplier = 0, int year = 0)
        {
            var viewModel = new OrderDetailsListViewModel()
            {
                Year = year
            };

            var allInvoices =
                from o in _db.OrderDetails
                where o.InvoiceDate != null
                select o;

            IEnumerable<OrderDetail> filteredOrders;
            IEnumerable<OrderDetail> supplierOrders;
            IEnumerable<OrderDetail> yearOrders;

            //Initialise some objects to hold years and months ...
            var years = new Dictionary<int, string> { { -1, "All Years" } };

            if (listSupplier > 0 && year == 0)
            {
                year = -1;
                viewModel.Year = -1;
            }
            if (listSupplier == 0 && year > 0)
            {
                listSupplier = -1;
            }

            if (year > 0 && listSupplier == -1)
            {
                filteredOrders = from o in allInvoices where o.InvoiceDate.Value.Year == year select o;
                supplierOrders = filteredOrders;
                yearOrders = allInvoices;
            }
            else if (year == -1 && listSupplier > 0)
            {
                filteredOrders = from o in allInvoices where o.SupplierID == listSupplier select o;
                supplierOrders = allInvoices;
                yearOrders = from o in allInvoices where o.SupplierID == listSupplier select o;
            }
            else if (year == -1 && listSupplier == -1)
            {
                filteredOrders = from o in allInvoices select o;
                supplierOrders = from o in allInvoices select o;
                yearOrders = from o in allInvoices select o;
            }
            else if (year > 0 && listSupplier > 0)
            {
                filteredOrders = from o in allInvoices where o.InvoiceDate.Value.Year == year && o.SupplierID == listSupplier select o;
                supplierOrders = from o in allInvoices where o.InvoiceDate.Value.Year == year select o;
                yearOrders = from o in allInvoices where o.SupplierID == listSupplier select o;
            }
            else
            {
                filteredOrders = from o in allInvoices where o.InvoiceDate.Value.Year == -1 && o.SupplierID == -1 select o;
                supplierOrders = allInvoices;
                yearOrders = allInvoices;
            }

            viewModel.Orders = filteredOrders;

            var suppliers = (from x in _db.Suppliers
                             join o in supplierOrders on x.SupplierID equals o.SupplierID
                             select
                                 new SupplierList
                                 {
                                     SupplierId = x.SupplierID,
                                     SupplierName = x.SupplierName,
                                     Count = supplierOrders.Count(s => s.SupplierID == x.SupplierID)
                                 })
                .Distinct().OrderBy(x => x.SupplierName);
            ViewData["ListSupplier"] = SelectListHelper.SuppliersListCustom(suppliers, addDefault: false);
            
            var orderYears = (from o in yearOrders select new { Year = o.InvoiceDate.Value.Year }).Distinct();
            foreach (var item in orderYears.OrderBy(y => y.Year))
            {
                years.Add(item.Year, item.Year.ToString());
            }

            ViewData["Years"] = years;
            ViewBag.InfoMsg =
                "To limit the list to just those invoices you want to see, use the Year and Supplier drop-down lists below:";
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("ordersSeeAlso", "AllInvoices");
            ViewBag.Title = "All " + DbRes.T("Invoices", "EntityType") + " (Historical)";
            return View(viewModel);
        }

        // GET: Admin/Orders/AllReceived
        public ActionResult AllReceived(int listSupplier = 0, int year = 0)
        {
            var viewModel = new OrderDetailsListViewModel()
            {
                Year = year
            };

            var allReceivedOrders =
                from o in _db.OrderDetails
                where o.ReceivedDate != null
                select o;

            IEnumerable<OrderDetail> filteredOrders;
            IEnumerable<OrderDetail> supplierOrders;
            IEnumerable<OrderDetail> yearOrders;

            //Initialise some objects to hold years and months ...
            var years = new Dictionary<int, string> { { -1, "All Years" } };

            if (listSupplier > 0 && year == 0)
            {
                year = -1;
                viewModel.Year = -1;
            }
            if (listSupplier == 0 && year > 0)
            {
                listSupplier = -1;
            }

            if (year > 0 && listSupplier == -1)
            {
                filteredOrders = from o in allReceivedOrders where o.ReceivedDate.Value.Year == year select o;
                supplierOrders = filteredOrders;
                yearOrders = allReceivedOrders;
            }
            else if (year == -1 && listSupplier > 0)
            {
                filteredOrders = from o in allReceivedOrders where o.SupplierID == listSupplier select o;
                supplierOrders = allReceivedOrders;
                yearOrders = from o in allReceivedOrders where o.SupplierID == listSupplier select o;
            }
            else if (year == -1 && listSupplier == -1)
            {
                filteredOrders = from o in allReceivedOrders select o;
                supplierOrders = from o in allReceivedOrders select o;
                yearOrders = from o in allReceivedOrders select o;
            }
            else if (year > 0 && listSupplier > 0)
            {
                filteredOrders = from o in allReceivedOrders where o.ReceivedDate.Value.Year == year && o.SupplierID == listSupplier select o;
                supplierOrders = from o in allReceivedOrders where o.ReceivedDate.Value.Year == year select o;
                yearOrders = from o in allReceivedOrders where o.SupplierID == listSupplier select o;
            }
            else
            {
                filteredOrders = from o in allReceivedOrders where o.ReceivedDate.Value.Year == -1 && o.SupplierID == -1 select o;
                supplierOrders = allReceivedOrders;
                yearOrders = allReceivedOrders;
            }

            viewModel.Orders = filteredOrders;

            var suppliers = (from x in _db.Suppliers
                             join o in supplierOrders on x.SupplierID equals o.SupplierID
                             select
                                 new SupplierList
                                 {
                                     SupplierId = x.SupplierID,
                                     SupplierName = x.SupplierName,
                                     Count = supplierOrders.Count(s => s.SupplierID == x.SupplierID)
                                 })
                .Distinct().OrderBy(x => x.SupplierName);
            ViewData["ListSupplier"] = SelectListHelper.SuppliersListCustom(suppliers, addDefault: false);

            var orderYears = (from o in yearOrders select new { Year = o.ReceivedDate.Value.Year }).Distinct();
            foreach (var item in orderYears.OrderBy(y => y.Year))
            {
                years.Add(item.Year, item.Year.ToString());
            }

            ViewData["Years"] = years;;
            ViewBag.InfoMsg =
                "This page lists all " + _entityType.ToLower() + " that have been marked as received. To limit the list to just those " + _entityType.ToLower() + " you want to see, use the Year and Supplier drop-down lists below:";
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("ordersSeeAlso", "AllReceived");
            ViewBag.Title = "Received " + DbRes.T("Orders", "EntityType");
            return View(viewModel);
        }

        // GET: Admin/Orders/AllUnassigned
        public ActionResult AllUnassigned(int listSupplier = 0)
        {
            //Get the list of suppliers with unassigned orders ...
            var suppliers = (from x in _db.Suppliers
                             join o in _db.OrderDetails on x.SupplierID equals o.SupplierID
                             where (o.BudgetCodeID == null || o.AccountYearID == null)
                             select new SupplierList { SupplierId = x.SupplierID, SupplierName = string.IsNullOrEmpty(x.SupplierName) ? "[Blank Supplier]" : x.SupplierName, Count = x.OrderDetails.Count(y => y.BudgetCode == null || y.AccountYearID == null) })
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

            ViewBag.InfoMsg =
                "This page lists any " + _entityType.ToLower() + " that have no Budget Code or Account Year assigned to them. To limit the list to just those " + _entityType.ToLower() + " from a particular Supplier, use the drop-down list below:";
            ViewData["ListSupplier"] = SelectListHelper.SuppliersListCustom(suppliers, addDefault: false, showAll: false);
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("ordersSeeAlso", "AlUnassigned");
            ViewBag.Title = "Unallocated " + DbRes.T("Orders", "EntityType");
            return View(viewModel);
        }

        // GET: Admin/Orders/OutstandingOrders
        public ActionResult AllOutstanding(int listSupplier = 0)
        {
            //Get the list of suppliers with outstanding orders ...
            var suppliers = (from x in _db.Suppliers
                             join o in _db.OrderDetails on x.SupplierID equals o.SupplierID
                             where (o.ReceivedDate == null)
                             select new SupplierList { SupplierId = x.SupplierID, SupplierName = string.IsNullOrEmpty(x.SupplierName) ? "[Blank Supplier]" : x.SupplierName, Count = x.OrderDetails.Count(y => y.ReceivedDate == null) })
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

            ViewBag.InfoMsg =
                "This page lists all " + _entityType.ToLower() + " that are still outstanding (i.e. have not yet been marked as received). To limit the list to just those " + _entityType.ToLower() + " from a particular Supplier, use the drop-down list below:";
            ViewData["ListSupplier"] = SelectListHelper.SuppliersListCustom(suppliers, addDefault: false, showAll: false);
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("ordersSeeAlso", "AllOutstanding");
            ViewBag.Title = "Current (Outstanding) " + DbRes.T("Orders", "EntityType");
            return View(viewModel);
        }

        // GET: Admin/Orders/OverdueOrders
        public ActionResult AllOverdue(int listSupplier = 0)
        {
            //Get the list of suppliers with overdue orders ...
            var today = DateTime.Today; //Today at 00:00:00
            var suppliers = (from x in _db.Suppliers
                             join o in _db.OrderDetails on x.SupplierID equals o.SupplierID
                             where (o.ReceivedDate == null && o.Expected <= today)
                             select new SupplierList { SupplierId = x.SupplierID, SupplierName = string.IsNullOrEmpty(x.SupplierName) ? "[Blank Supplier]" : x.SupplierName, Count = x.OrderDetails.Count(y => y.ReceivedDate == null && y.Expected <= today) })
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

            ViewBag.InfoMsg =
                "This page lists all " + _entityType.ToLower() + " that are outstanding and overdue (i.e. the date expected has passed). To limit the list to just those " + _entityType.ToLower() + " from a particular Supplier, use the drop-down list below:";
            ViewData["ListSupplier"] = SelectListHelper.SuppliersListCustom(suppliers, addDefault: false, showAll: false);
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("ordersSeeAlso", "AllOverdue");
            ViewBag.Title = "Overdue " + DbRes.T("Orders", "EntityType");
            return View(viewModel);
        }

        // GET: Admin/Orders/AllOnApproval
        public ActionResult AllOnApproval(int listSupplier = 0)
        {
            //Get the list of suppliers with outstanding orders ...
            var suppliers = (from x in _db.Suppliers
                             join o in _db.OrderDetails on x.SupplierID equals o.SupplierID
                             where (o.OnApproval)
                             select new SupplierList { SupplierId = x.SupplierID, SupplierName = string.IsNullOrEmpty(x.SupplierName) ? "[Blank Supplier]" : x.SupplierName, Count = x.OrderDetails.Count(y => y.OnApproval) })
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

            ViewBag.InfoMsg =
                "To limit the list to just those " + _entityType.ToLower() + " from a particular Supplier, use the drop-down list below:";
            ViewData["ListSupplier"] = SelectListHelper.SuppliersListCustom(suppliers, addDefault: false, showAll: false);
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("ordersSeeAlso", "AllOnApproval");
            ViewBag.Title = DbRes.T("Orders", "EntityType") + " On Approval";
            return View(viewModel);
        }

        // GET: Admin/Orders/ReceivedOrdersNoInvoice
        public ActionResult ReceivedOrdersNoInvoice(int listSupplier = 0)
        {
            //Get the list of suppliers with received orders but no invoice ...
            var suppliers = (from x in _db.Suppliers
                             join o in _db.OrderDetails on x.SupplierID equals o.SupplierID
                             where (o.ReceivedDate != null && o.InvoiceRef == null && o.InvoiceDate == null)
                             select new SupplierList { SupplierId = x.SupplierID, SupplierName = string.IsNullOrEmpty(x.SupplierName) ? "[Blank Supplier]" : x.SupplierName, Count = x.OrderDetails.Count(y => y.ReceivedDate == null) })
                .Distinct().OrderBy(x => x.SupplierName);

            var receivedOrders = _db.OrderDetails
                .Where(o => o.ReceivedDate != null && o.InvoiceRef == null && o.InvoiceDate == null);
            if (listSupplier > 0)
            {
                receivedOrders = receivedOrders.Where(o => o.SupplierID == listSupplier);
            }

            var viewModel = new OrderDetailsListViewModel()
            {
                Orders = receivedOrders
            };

            ViewBag.InfoMsg =
                "This page lists all " + _entityType.ToLower() + " that have been marked as received but no invoice details have been logged. To limit the list to just those " + _entityType.ToLower() + " from a particular Supplier, use the drop-down list below:";
            ViewData["ListSupplier"] = SelectListHelper.SuppliersListCustom(suppliers, addDefault: false, showAll: false);
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("ordersSeeAlso", "AllOutstanding");
            ViewBag.Title = "Received " + _entityType + " With No Invoice";
            return View(viewModel);
        }

        // GET: Admin/Orders/ReceivedOrdersNoInvoice
        public ActionResult OutstandingOrdersWithInvoice(int listSupplier = 0)
        {
            //Get the list of suppliers with received orders but no invoice ...
            var suppliers = (from x in _db.Suppliers
                             join o in _db.OrderDetails on x.SupplierID equals o.SupplierID
                             where (o.ReceivedDate == null && (o.InvoiceRef != null && o.InvoiceDate != null))
                             select new SupplierList { SupplierId = x.SupplierID, SupplierName = string.IsNullOrEmpty(x.SupplierName) ? "[Blank Supplier]" : x.SupplierName, Count = x.OrderDetails.Count(y => y.ReceivedDate == null) })
                .Distinct().OrderBy(x => x.SupplierName);

            var receivedOrders = _db.OrderDetails
                .Where(o => o.ReceivedDate == null && (o.InvoiceRef != null || o.InvoiceDate != null));
            if (listSupplier > 0)
            {
                receivedOrders = receivedOrders.Where(o => o.SupplierID == listSupplier);
            }

            var viewModel = new OrderDetailsListViewModel()
            {
                Orders = receivedOrders
            };

            ViewBag.InfoMsg =
                "This page lists all outstanding " + _entityType.ToLower() + " (i.e. not yet marked as received) that have invoice details logged against them. To limit the list to just those " + _entityType.ToLower() + " from a particular Supplier, use the drop-down list below:";
            ViewData["ListSupplier"] = SelectListHelper.SuppliersListCustom(suppliers, addDefault: false, showAll: false);
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("ordersSeeAlso", "AllOutstanding");
            ViewBag.Title = "Invoices With Outstanding " + _entityType;
            return View(viewModel);
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult AutoComplete(string term)
        {
            var orders = SearchService.SelectOrders(term, 100);

            var ordersList = (from o in orders
                          orderby o.Title.Substring(o.NonFilingChars) ascending ,o.OrderId descending 
                          select new { Title = o.Title, OrderId = o.OrderId, Year = o.Year, Edition = o.Edition, OrderNo = o.OrderNo, InvoiceRef = o.InvoiceRef, OrderDate = o.OrderDate, ReceivedDate = o.ReceivedDate, InvoiceDate = o.InvoiceDate, SupplierName = o.SupplierName });

            return Json(ordersList, JsonRequestBehavior.AllowGet);
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult AutoCompleteNoInvoice(string term)
        {
            var orders = SearchService.SelectOrders(term, 100);

            var ordersList = (from o in orders
                          where o._InvoiceDate == null && o._InvoiceRef == null
                          orderby o.Title.Substring(o.NonFilingChars) ascending, o.OrderId descending
                          select new { Title = o.Title, OrderId = o.OrderId, Year = o.Year, Edition = o.Edition, OrderNo = o.OrderNo, InvoiceRef = o.InvoiceRef, OrderDate = o.OrderDate, ReceivedDate = o.ReceivedDate, InvoiceDate = o.InvoiceDate, SupplierName = o.SupplierName });

            return Json(ordersList, JsonRequestBehavior.AllowGet);
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult AutoCompleteOutstanding(string term)
        {
            var orders = SearchService.SelectOrders(term, 100);

            var ordersList = (from o in orders
                          where o._ReceivedDate == null
                          orderby o.Title.Substring(o.NonFilingChars) ascending, o.OrderId descending
                          select new { Title = o.Title, OrderId = o.OrderId, Year = o.Year, Edition = o.Edition, OrderNo = o.OrderNo, InvoiceRef = o.InvoiceRef, OrderDate = o.OrderDate, ReceivedDate = o.ReceivedDate, InvoiceDate = o.InvoiceDate, SupplierName = o.SupplierName });

            return Json(ordersList, JsonRequestBehavior.AllowGet);
        }


        //GET: Select an order ...
        public ActionResult Select(string callingAction = "edit", string filter = "")
        {
            var viewModel = new SelectOrderViewmodel();

            ViewData["SeeAlso"] = MenuHelper.SeeAlso("ordersSeeAlso");
            switch (callingAction)
            {
                case "edit":
                    {
                        ViewBag.Title = "Edit/Update " + _entityName;
                        viewModel.BtnText = "Edit/Update " + _entityName;
                        viewModel.Message = "Enter an " + _entityName.ToLower() + " to edit/update";
                        viewModel.HelpText = "Start typing the name of the ordered item and then select the " + _entityName.ToLower() + " you wish to edit/update in the box below.";
                        viewModel.ReturnAction = "Edit";
                        viewModel.AutoCompleteSource = "AutoComplete";
                        break;
                    }

                case "print":
                    {
                        ViewBag.Title = "Print " + _entityName;
                        viewModel.BtnText = "Print Order";
                        viewModel.Message = "Enter an " + _entityName.ToLower() + " to print";
                        viewModel.HelpText = "Start typing the name of the ordered item and then select the " + _entityName.ToLower() + " you wish to print in the box below.";
                        viewModel.ReturnAction = "PrintOrder";
                        viewModel.AutoCompleteSource = "AutoComplete";
                        break;
                    }

                case "reprint":
                    {
                        ViewBag.Title = "Reprint " + _entityName;
                        viewModel.BtnText = "Reprint " + _entityName;
                        viewModel.Message = "Enter an " + _entityName.ToLower() + " to Reprint";
                        viewModel.HelpText = "Start typing the name of the ordered item and then select the " + _entityName.ToLower() + " you wish to reprint in the box below.";
                        viewModel.ReturnAction = "PrintOrder";
                        viewModel.AutoCompleteSource = "AutoComplete";
                        break;
                    }

                case "duplicate":
                    {
                        ViewBag.Title = "Duplicate " + _entityName;
                        viewModel.BtnText = "Duplicate " + _entityName;
                        viewModel.Message = "Specify an " + _entityName.ToLower() + " to Duplicate";
                        viewModel.HelpText = "Start typing the name of the ordered item and then select the " + _entityName.ToLower() + " you wish to duplicate in the box below.";
                        viewModel.ReturnAction = "DuplicateOrder";
                        viewModel.AutoCompleteSource = "AutoComplete";
                        break;
                    }

                case "addInvoice":
                    {
                        ViewBag.Title = "Add Invoice";
                        viewModel.BtnText = "Add Invoice";
                        viewModel.Message = "Enter an " + _entityName.ToLower() + " to add an Invoice to";
                        viewModel.HelpText = "<strong>Note: </strong>The list below <emp>only</emp> shows " + _entityType.ToLower() + " with no invoice. To see all " + _entityType.ToLower() + ", choose another option.";
                        viewModel.ReturnAction = "AddInvoice";
                        viewModel.Tab = "#invoice";
                        viewModel.AutoCompleteSource = "AutoCompleteNoInvoice";
                        break;
                    }

                default:
                    {
                        ViewBag.Title = "Select  " + _entityName;
                        viewModel.BtnText = "Ok";
                        viewModel.Message = "Select an " + _entityName;
                        viewModel.HelpText = "Select an " + _entityName.ToLower() + " from the dropdown list of available " + _entityType.ToLower() + " below.";
                        viewModel.ReturnAction = "Edit";
                        viewModel.AutoCompleteSource = "AutoComplete";
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
            TempData["GoToTab"] = viewModel.Tab;
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
        public ActionResult OrdersBySupplier(int listSupplier = 0)
        {
            //Get the list of suppliers with orders ...
            var suppliers = (from x in _db.Suppliers
                             join o in _db.OrderDetails on x.SupplierID equals o.SupplierID
                             select new SupplierList { SupplierId = x.SupplierID, SupplierName = string.IsNullOrEmpty(x.SupplierName) ? "[Blank Supplier]" : x.SupplierName, Count = x.OrderDetails.Count() })
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

            ViewData["ListSupplier"] = SelectListHelper.SuppliersListCustom(suppliers, addDefault: true, showAll: false);
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("ordersSeeAlso", "OrdersBySupplier");
            ViewBag.Title = DbRes.T("Orders", "EntityType") + " By " +
                            DbRes.T("Suppliers.Supplier", "FieldDisplayName");
            return View(viewModel);
        }


        // GET: Admin/Orders/ByRequester
        public ActionResult OrdersByRequester(string listRequesters = "")
        {
            //Get the list of requesters with orders ...
            //Get the list of authorisers with orders ...
            IEnumerable<SelectListItem> requestersList =
                UserManager.Users.Where(u => u.AuthorisedOrders.Count > 0).OrderBy(u => u.Lastname).ThenBy(u => u.Firstname)
                    .Select(
                        x =>
                            new SelectListItem
                            {
                                Value = x.Id,
                                Text = x.Lastname + ", " + x.Firstname + " (" + x.RequestedOrders.Count + ")"
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
        public ActionResult OrdersByAuthoriser(string listAuthorisers = "")
        {
            //Get the list of authorisers with orders ...
            IEnumerable<SelectListItem> authorisersList =
                UserManager.Users.Where(u => u.AuthorisedOrders.Count > 0).OrderBy(u => u.Lastname).ThenBy(u => u.Firstname)
                    .Select(
                        x =>
                            new SelectListItem
                            {
                                Value = x.Id,
                                Text = x.Lastname + ", " + x.Firstname + " (" + x.RequestedOrders.Count + ")"
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
        public ActionResult OrdersByTitle(int listTitles = 0)
        {
            //Get the list of ordered titles  ...
            var titles = (from x in _db.Titles
                          where x.OrderDetails.Count > 0
                          select
                              new { TitleID = x.TitleID, Title = x.Title1, OrderCount = x.OrderDetails.Count, x.NonFilingChars })
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
            foreach (var item in titles)
            {
                titlesList.Add(new SelectListItem
                {
                    Text = StringHelper.Truncate(item.Title, 100) + " (" + item.OrderCount + ")",
                    Value = item.TitleID.ToString()
                });
            }


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
                Suppliers = SelectListHelper.SupplierList(PublicFunctions.GetDefaultValue("OrderDetails", "SupplierID")),
                OrderCategories = SelectListHelper.OrderCategoryList(PublicFunctions.GetDefaultValue("OrderDetails", "OrderCategoryID")),
                BudgetCodes = SelectListHelper.BudgetCodesList(PublicFunctions.GetDefaultValue("OrderDetails", "BudgetCodeID"), "Select a ", true, false),
                AccountYears = SelectListHelper.AccountYearsList(id: GetCurrentAccountYearId(), addNew: false),
                RequestUsers = SelectListHelper.SelectUsersByLastname(liveOnly: true),
                AuthorityUsers = SelectListHelper.SelectUsersByLastname(liveOnly: true),
                //AuthorityUsers = new SelectList(UserManager.Users, "Id", "FullnameRev"),
                CallingAction = "Edit",
                CallingController = "Titles"
            };

            ViewBag.Title = "Add " + DbRes.T("Orders.New_Order", "FieldDisplayName");
            return PartialView(viewModel);
        }

        // GET: LibraryAdmin/OrderDetails/Create
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
                Titles = SelectListHelper.TitlesList(titleid),
                Suppliers = SelectListHelper.SupplierList(PublicFunctions.GetDefaultValue("OrderDetails", "SupplierID")),
                OrderCategories = SelectListHelper.OrderCategoryList(PublicFunctions.GetDefaultValue("OrderDetails", "OrderCategoryID")),
                AccountYears = SelectListHelper.AccountYearsList(id:GetCurrentAccountYearId(), addNew:false),
                BudgetCodes = SelectListHelper.BudgetCodesList(PublicFunctions.GetDefaultValue("OrderDetails", "BudgetCodeID"), "Select a ", true, false),
                RequestUsers = SelectListHelper.SelectUsersByLastname(liveOnly: true),
                AuthorityUsers = SelectListHelper.SelectUsersByLastname(liveOnly: true)
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
                BudgetCodeID = viewModel.BudgetCodeID,
                AccountYearID = viewModel.AccountYearID,
                OrderDate = viewModel.OrderDate ?? DateTime.Now,
                OrderNo = viewModel.OrderNo ?? "",
                Expected = viewModel.Expected ?? DateTime.Now.AddDays(28),
                RequesterUser = _db.Users.Find(viewModel.RequestedBy),
                AuthoriserUser = _db.Users.Find(viewModel.Authority),
                NumCopies = viewModel.NumCopies ?? 1,
                Price = viewModel.Price ?? 0,
                VAT = viewModel.VAT ?? 0,
                SupplierID = viewModel.SupplierID == 0 ? 1 : viewModel.SupplierID,
                Notes = viewModel.Notes,
                ReceivedDate = viewModel.ReceivedDate,
                Passed = viewModel.Passed,
                InvoiceDate = viewModel.InvoiceDate,
                InvoiceRef = viewModel.InvoiceRef,
                Link = viewModel.Link
            };

            if (ModelState.IsValid)
            {
                _db.OrderDetails.Add(newOrder);
                _db.SaveChanges();

                //Check that we have an Order Number; if not, add noe now based on the OrderID ...
                if(string.IsNullOrEmpty(viewModel.OrderNo))
                {
                    newOrder.OrderNo = newOrder.OrderID.ToString();
                    _db.Entry(newOrder).State = EntityState.Modified;
                    _db.SaveChanges();
                }

                if (viewModel.CallingController == "Titles")
                {
                    return RedirectToAction(viewModel.CallingAction, "Titles", new { id = viewModel.TitleID });
                }
                return RedirectToAction("Edit", new { id = newOrder.OrderID });
            }
            return RedirectToAction("Add");
        }

        public ActionResult AddInvoice(int id = 0, bool success = false)
        {
            if (id == 0)
            {
                return RedirectToAction("Select", new { callingAction = "addInvoice" });
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
                OrderLinks = orderDetail.OrderLinks,
                Notes = orderDetail.Notes,
                Price = orderDetail.Price,
                Passed = orderDetail.Passed,
                Report = orderDetail.Report,
                SupplierID = orderDetail.SupplierID,
                VAT = orderDetail.VAT,
                Titles = SelectListHelper.TitlesList(orderDetail.TitleID),
                Suppliers = SelectListHelper.SupplierList(id: orderDetail.SupplierID ?? 0),
                RequestUsers = SelectListHelper.SelectUsersByLastname(),
                AuthorityUsers = SelectListHelper.SelectUsersByLastname(),
                CallingAction = "AddInvoice",
                SelectedTab = "#invoice",
                PlaceHolderText = "To view/edit another order, start typing part of the name of the ordered item ...",
                SelectOrder = _db.vwSelectJustTitles.Find(orderDetail.TitleID).Title, //orderDetail.Title.Title1,
                AutoCompleteSource = "AutoCompleteNoInvoice"
                
            };

            if (success && viewModel.InvoiceDate != null)
            {
                TempData["SuccessMsg"] = "Invoice added successfully.";
                ViewBag.Title = _entityName + " - Full Details";
                ViewData["selectOrder"] = SelectListHelper.OrdersList(id: viewModel.OrderID);
            }
            else
            {
                ViewBag.Title = "Add Invoice";
                //ViewData["selectOrder"] = SelectListHelper.OrdersList(id: viewModel.OrderID, filter: "noinvoice");
            }

            //Check some values and issue info or warning messages if appropriate ...
            if (orderDetail.ReceivedDate == null && (orderDetail.InvoiceDate != null || orderDetail.InvoiceRef != null))
            {
                viewModel.WarningMsg =
                    "<strong>Info: </strong>An invoice has been entered for this " + _entityName.ToLower() + ", but the " + _entityName.ToLower() + " has not been marked as received and is still outstanding. You can enter a received date on the 'Invoice' tab'.";
            }
            if (orderDetail.ReceivedDate != null && (orderDetail.InvoiceDate == null && orderDetail.InvoiceRef == null))
            {
                viewModel.WarningMsg =
                    "<strong>Info: </strong>This  " + _entityName.ToLower() + " has been marked as received but no invoice has been entered. You can add an invoice on the 'Invoice' tab'.";
            }

            //ViewData["selectOrder"] = SelectListHelper.OrdersList(id: viewModel.OrderID, filter: "noinvoice");
            ViewData["AccountYearID"] = new SelectList(_db.AccountYears, "AccountYearID", "AccountYear1", orderDetail.AccountYearID);
            ViewData["BudgetCodeID"] = new SelectList(_db.BudgetCodes, "BudgetCodeID", "BudgetCode1", orderDetail.BudgetCodeID);
            ViewData["OrderCategoryID"] = new SelectList(_db.OrderCategories, "OrderCategoryID", "OrderCategory1", orderDetail.OrderCategoryID);
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("ordersSeeAlso", "AddInvoice");
            return View("AddInvoice", viewModel);
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
        public ActionResult EditTitle(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return RedirectToAction("Edit", "Titles", new { id });
        }

        // GET: LibraryAdmin/OrderDetails/Edit/5
        public ActionResult Edit(int? id, bool success = false)
        {
            if (id == 0)
            {
                return RedirectToAction("Select", new { callingAction = "edit" });
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
                OrderLinks = orderDetail.OrderLinks,
                Notes = orderDetail.Notes,
                Price = orderDetail.Price,
                Passed = orderDetail.Passed,
                Report = orderDetail.Report,
                SupplierID = orderDetail.SupplierID,
                VAT = orderDetail.VAT,
                Titles = SelectListHelper.TitlesList(orderDetail.TitleID),
                Suppliers = SelectListHelper.SupplierList(id:orderDetail.SupplierID ?? 0),
                RequestUsers = SelectListHelper.SelectUsersByLastname(liveOnly:false),
                AuthorityUsers = SelectListHelper.SelectUsersByLastname(liveOnly: false),
                CallingAction = "Edit",
                PlaceHolderText = "To view/edit another order, start typing part of the name of the ordered item ...",
                SelectOrder = _db.vwSelectJustTitles.Find(orderDetail.TitleID).Title, //orderDetail.Title.Title1,
                AutoCompleteSource = "AutoComplete"
            };

            if (success)
            {
                TempData["SuccessMsg"] = _entityName + " details have been updated successfully.";
            }

            //Check some values and issue info or warning messages if appropriate ...
            if (orderDetail.ReceivedDate == null && orderDetail.InvoiceDate == null && orderDetail.InvoiceRef == null)
            {
                viewModel.WarningMsg =
                    "<strong>Info: </strong>This " + _entityName.ToLower() + " is still outstanding. You can add a receipt and invoice on the 'Invoice' tab'.";
            }
            if (orderDetail.ReceivedDate == null && (orderDetail.InvoiceDate != null || orderDetail.InvoiceRef != null))
            {
                viewModel.WarningMsg =
                    "<strong>Info: </strong>An invoice has been entered for this " + _entityName.ToLower() + ", but the order has not been marked as received and is still outstanding. You can enter a received date on the 'Invoice' tab'.";
            }
            if (orderDetail.ReceivedDate != null && (orderDetail.InvoiceDate == null && orderDetail.InvoiceRef == null))
            {
                viewModel.WarningMsg =
                    "<strong>Info: </strong>This " + _entityName.ToLower() + " has been marked as received but no invoice has been entered. You can add an invoice on the 'Invoice' tab'.";
            }

            //ViewData["selectOrder"] = SelectListHelper.OrdersList(id: viewModel.OrderID);
            ViewData["AccountYearID"] = SelectListHelper.AccountYearsList(id: orderDetail.AccountYearID ?? 0, addNew: false);
            ViewData["BudgetCodeID"] = SelectListHelper.BudgetCodesList(id: orderDetail.BudgetCodeID ?? 0, addNew: false);
            ViewData["OrderCategoryID"] = SelectListHelper.OrderCategoriesList(id: orderDetail.OrderCategoryID ?? 0, addNew: false);
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("ordersSeeAlso", "Edit");
            ViewBag.Title = "Edit/Update Order";
            return View(viewModel);
            ;
        }

        // POST: LibraryAdmin/OrderDetails/Update/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(OrderDetailsEditViewModel viewModel)
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
                orderDetail.AccountYearID = viewModel.AccountYearID == 0 ? null : viewModel.AccountYearID;
                orderDetail.Accepted = viewModel.Accepted;
                orderDetail.AuthoriserUser = _db.Users.Find(viewModel.Authority);
                orderDetail.OnApproval = viewModel.OnApproval;
                orderDetail.BudgetCodeID = viewModel.BudgetCodeID == 0 ? null : viewModel.BudgetCodeID;
                orderDetail.RequesterUser = _db.Users.Find(viewModel.RequestedBy);
                orderDetail.Chased = viewModel.Chased;
                orderDetail.Cancelled = viewModel.Cancelled;
                orderDetail.NumCopies = viewModel.NumCopies;
                orderDetail.OrderCategoryID = viewModel.OrderCategoryID == 0 ? null : viewModel.OrderCategoryID;
                orderDetail.InvoiceDate = viewModel.InvoiceDate;
                orderDetail.MonthSubDue = viewModel.MonthSubDue;
                orderDetail.OrderDate = viewModel.OrderDate;
                orderDetail.ReceivedDate = viewModel.ReceivedDate;
                orderDetail.ReturnedDate = viewModel.ReturnedDate;
                orderDetail.Expected = viewModel.Expected;
                orderDetail.InvoiceRef = viewModel.InvoiceRef;
                orderDetail.Item = viewModel.Item;
                orderDetail.TitleID = viewModel.TitleID;
                orderDetail.SupplierID = viewModel.SupplierID == 0 ? null : viewModel.SupplierID;
                orderDetail.Link = viewModel.Link;
                orderDetail.Notes = viewModel.Notes;
                orderDetail.OrderNo = viewModel.OrderNo;
                orderDetail.Price = viewModel.Price;
                orderDetail.Passed = viewModel.Passed;
                orderDetail.Report = viewModel.Report;
                orderDetail.VAT = viewModel.VAT;

                _db.Entry(orderDetail).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction(viewModel.CallingAction, "OrderDetails", new { id = viewModel.OrderID, success = true });
            }

            ViewData["AccountYearID"] = SelectListHelper.AccountYearsList(id: viewModel.AccountYearID ?? 0, addNew: false);
            ViewData["BudgetCodeID"] = SelectListHelper.BudgetCodesList(id: viewModel.BudgetCodeID ?? 0, addNew: false);
            ViewData["OrderCategoryID"] = SelectListHelper.OrderCategoriesList(id: viewModel.OrderCategoryID ?? 0, addNew: false);
            ViewData["Authority"] =SelectListHelper.SelectUsersByLastname(liveOnly: false, id: viewModel.Authority);
            ViewData["RequestedBy"] = SelectListHelper.SelectUsersByLastname(liveOnly: false, id: viewModel.RequestedBy);
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("ordersSeeAlso", "Edit");

            switch (viewModel.CallingAction)
            {
                case "Edit":
                    {
                        ViewData["selectOrder"] = SelectListHelper.OrdersList(id: viewModel.OrderID);
                        ViewBag.Title = "Edit/Update " + _entityName;
                        break;
                    }
                case "AddInvoice":
                    {
                        ViewData["selectOrder"] = SelectListHelper.OrdersList(id: viewModel.OrderID, filter: "noinvoice");
                        ViewBag.Title = "Add Invoice";
                        break;
                    }
                default:
                    {
                        ViewData["selectOrder"] = SelectListHelper.OrdersList(id: viewModel.OrderID);
                        ViewBag.Title = _entityName + " Details";
                        break;
                    }
            }

            return View(viewModel.CallingAction, viewModel);
        }

        // GET: LibraryAdmin/OrderDetails/Edit/5
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

            if (orderDetail.InvoiceDate == null || orderDetail.ReceivedDate == null || orderDetail.Passed == null)
            {
                viewModel.DateWarningMsg =
                    "<p><strong>Note: </strong>The dates highlighted in <span style=\"color: #3c763d;\"><strong>green</strong></span> have been auto-filled for you.  Please check to ensure that the dates are correct for your invoice.</p>";
            }

            ViewData["AccountYearID"] = new SelectList(_db.AccountYears, "AccountYearID", "AccountYear1", orderDetail.AccountYearID);
            ViewData["BudgetCodeID"] = new SelectList(_db.BudgetCodes, "BudgetCodeID", "BudgetCode1", orderDetail.BudgetCodeID);
            ViewData["OrderCategoryID"] = new SelectList(_db.OrderCategories, "OrderCategoryID", "OrderCategory1", orderDetail.OrderCategoryID);
            ViewBag.Title = "Add Invoice";
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

                return Json(new { success = true });

            }
            return PartialView("AddReceipt", viewModel);
        }

        // GET: LibraryAdmin/OrderDetails/Edit/5
        public ActionResult QuickReceipt(int? id)
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
                OrderNo = orderDetail.OrderNo,
                ReceivedDate = orderDetail.ReceivedDate ?? DateTime.Now,
                Title = orderDetail.Title.Title1
            };

            if (orderDetail.ReceivedDate == null)
            {
                viewModel.DateWarningMsg =
                    "<p><strong>Note: </strong>The received date (highlighted in <span style=\"color: #3c763d;\"><strong>green</strong></span>) has been auto-filled for you.  Please check to ensure that this date is correct for your " + _entityName.ToLower() + ".</p>";
            }

            ViewBag.Title = "Quick Receipt";
            return PartialView(viewModel);
        }

        public ActionResult PostQuickReceipt([Bind(Include = "OrderID,ReceivedDate")] OrderReceiptsViewModel viewModel)
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
                _db.Entry(orderDetail).State = EntityState.Modified;
                _db.SaveChanges();

                return Json(new { success = true });
            }
            return PartialView("QuickReceipt", viewModel);
        }

        //[Route("ReportGenerator")]
        public ActionResult ReportGenerator()
        {
            if (TempData["NoReportSelected"] != null)
            {
                if ((bool)TempData["NoReportSelected"] == true)
                {
                    TempData["ErrorMsg"] = "No report selected! Please select a report from the list.";
                    //ModelState.AddModelError("NotSelected", "No report selected! Please select a report from the list.");
                }
            }

            if (TempData["NoData"] != null)
            {
                if ((bool)TempData["NoData"] == true)
                {
                    TempData["NoDataMsg"] = "There is no data for this report! Please select some alternative criteria and try again.";
                    //ModelState.AddModelError("NoData", "There is no data for this report! Please select some alternative criteria and try again.");
                }
            }

            var viewModel = new ReportsGeneratorViewModel
            {
                Reports = SelectListHelper.FinanceReportsList(),
                BudgetCodes = SelectListHelper.BudgetCodesList(addDefault: true, addNew: false, msg: "All "),
                AccountYears = SelectListHelper.AccountYearsList(addDefault: true, addNew: false),
                OrderCategories = SelectListHelper.OrderCategoriesList(addDefault: true, addNew: false, msg: "All "),
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

        public ActionResult Cancellations_Report(string caption, int accountYearId, DateTime startDate, DateTime endDate)
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

            var accountYear = _db.AccountYears.Find(accountYearId);

            var viewModel = new OrderReportsViewModel
            {
                TimeFrameDesc = accountYear == null ? "Received between: " + startDate.ToShortDateString() + " and " + endDate.ToShortDateString() : "Account Year: " + accountYear.AccountYear1,
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

            var accountYear = _db.AccountYears.Find(accountYearId);

            var viewModel = new OrderReportsViewModel
            {
                OrderCategories = orderCategories,
                AccountYearId = accountYearId,
                TimeFrameDesc = accountYear == null ? "Received between: " + startDate.ToShortDateString() + " and " + endDate.ToShortDateString() : "Account Year: " + accountYear.AccountYear1,
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

            var accountYear = _db.AccountYears.Find(accountYearId);

            var viewModel = new OrderReportsViewModel
            {
                Orders = allOrders,
                BudgetCodes = allBudgetCodes,
                AccountYearId = accountYearId,
                TimeFrameDesc = accountYear == null ? "Received between: " + startDate.ToShortDateString() + " and " + endDate.ToShortDateString() : "Account Year: " + accountYear.AccountYear1,
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

            var accountYear = _db.AccountYears.Find(accountYearId);

            var viewModel = new OrderReportsViewModel
            {
                Orders = allOrders,
                BudgetCodes = allBudgetCodes,
                AccountYearId = accountYearId,
                TimeFrameDesc = accountYear == null ? "Received between: " + startDate.ToShortDateString() + " and " + endDate.ToShortDateString() : "Account Year: " + accountYear.AccountYear1,
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

            var accountYear = _db.AccountYears.Find(accountYearId);

            var viewModel = new OrderReportsViewModel
            {
                Orders = allOrders,
                BudgetCodes = allBudgetCodes,
                AccountYearId = accountYearId,
                TimeFrameDesc = accountYear == null ? "Received between: " + startDate.ToShortDateString() + " and " + endDate.ToShortDateString() : "Account Year: " + accountYear.AccountYear1,
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

            var accountYear = _db.AccountYears.Find(accountYearId);

            var viewModel = new OrderReportsViewModel
            {
                Orders = allOrders,
                BudgetCodes = allBudgetCodes,
                AccountYearId = accountYearId,
                TimeFrameDesc = accountYear == null ? "Received between: " + startDate.ToShortDateString() + " and " + endDate.ToShortDateString() : "Account Year: " + accountYear.AccountYear1,
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

            var accountYear = _db.AccountYears.Find(accountYearId);

            var viewModel = new OrderReportsViewModel
            {
                Orders = allOrders,
                Titles = allTitles,
                MediaTypes = allMediaTypes,
                TimeFrameDesc = accountYear == null ? "Received between: " + startDate.ToShortDateString() + " and " + endDate.ToShortDateString() : "Account Year: " + accountYear.AccountYear1,
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

            var accountYear = _db.AccountYears.Find(accountYearId);

            var viewModel = new OrderReportsViewModel
            {
                Orders = allOrders,
                Titles = allTitles,
                Suppliers = allSuppliers,
                TimeFrameDesc = accountYear == null ? "Received between: " + startDate.ToShortDateString() + " and " + endDate.ToShortDateString() : "Account Year: " + accountYear.AccountYear1,
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

            var accountYear = _db.AccountYears.Find(accountYearId);

            var viewModel = new OrderReportsViewModel
            {
                Orders = allOrders,
                BudgetCodes = allBudgetCodes,
                AccountYearId = accountYearId,
                TimeFrameDesc = accountYear == null ? "Received between: " + startDate.ToShortDateString() + " and " + endDate.ToShortDateString() : "Account Year: " + accountYear.AccountYear1,
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

            var accountYear = _db.AccountYears.Find(accountYearId);

            var viewModel = new OrderReportsViewModel
            {
                Orders = allOrders,
                BudgetCodes = allBudgetCodes,
                AccountYearId = accountYearId,
                TimeFrameDesc = accountYear == null ? "Received between: " + startDate.ToShortDateString() + " and " + endDate.ToShortDateString() : "Account Year: " + accountYear.AccountYear1,
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

            var budgetCodes = (from o in orders
                               where o.BudgetCodeID != null
                               select o.BudgetCode).Distinct();

            var accountYear = _db.AccountYears.Find(accountYearId);

            var viewModel = new OrderReportsViewModel
            {
                BudgetCodes = budgetCodes,
                AccountYearId = accountYearId,
                TimeFrameDesc = accountYear == null ? "Received between: " + startDate.ToShortDateString() + " and " + endDate.ToShortDateString() : "Account Year: " + accountYear.AccountYear1,
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

            var allTitles = (from o in allOrders
                             select o.Title).Distinct();

            var allMediaTypes = (from t in allTitles
                                 select t.MediaType).Distinct();

            var accountYear = _db.AccountYears.Find(accountYearId);

            var viewModel = new OrderReportsViewModel
            {
                Orders = allOrders,
                Titles = allTitles,
                MediaTypes = allMediaTypes,
                AccountYearId = accountYearId,
                TimeFrameDesc = accountYear == null ? "Received between: " + startDate.ToShortDateString() + " and " + endDate.ToShortDateString() : "Account Year: " + accountYear.AccountYear1,
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
            
            var allTitles = (from o in allOrders
                             select o.Title).Distinct();

            var allMediaTypes = (from t in allTitles
                                 select t.MediaType).Distinct();

            var accountYear = _db.AccountYears.Find(accountYearId);

            var viewModel = new OrderReportsViewModel
            {
                Orders = allOrders,
                Titles = allTitles,
                MediaTypes = allMediaTypes,
                AccountYearId = accountYearId,
                TimeFrameDesc = accountYear == null ? "Received between: " + startDate.ToShortDateString() + " and " + endDate.ToShortDateString() : "Account Year: " + accountYear.AccountYear1,
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
            
            var allTitles = (from o in allOrders
                             select o.Title).Distinct();

            var allMediaTypes = (from t in allTitles
                                 select t.MediaType).Distinct();

            var accountYear = _db.AccountYears.Find(accountYearId);

            var viewModel = new OrderReportsViewModel
            {
                Orders = allOrders,
                Titles = allTitles,
                MediaTypes = allMediaTypes,
                AccountYearId = accountYearId,
                TimeFrameDesc = accountYear == null ? "Received between: " + startDate.ToShortDateString() + " and " + endDate.ToShortDateString() : "Account Year: " + accountYear.AccountYear1,
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

            var allTitles = (from o in allOrders
                             select o.Title).Distinct();

            var allMediaTypes = (from t in allTitles
                                 select t.MediaType).Distinct();

            var accountYear = _db.AccountYears.Find(accountYearId);

            var viewModel = new OrderReportsViewModel
            {
                Orders = allOrders,
                Titles = allTitles,
                MediaTypes = allMediaTypes,
                AccountYearId = accountYearId,
                TimeFrameDesc = accountYear == null ? "Received between: " + startDate.ToShortDateString() + " and " + endDate.ToShortDateString() : "Account Year: " + accountYear.AccountYear1,
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

            var allSuppliers = (from o in allOrders
                                select o.Supplier).Distinct();

            var accountYear = _db.AccountYears.Find(accountYearId);

            var viewModel = new OrderReportsViewModel
            {
                Orders = allOrders,
                Suppliers = allSuppliers,
                AccountYearId = accountYearId,
                TimeFrameDesc = accountYear == null ? "Received between: " + startDate.ToShortDateString() + " and " + endDate.ToShortDateString() : "Account Year: " + accountYear.AccountYear1,
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

            var allSuppliers = (from o in allOrders
                                select o.Supplier).Distinct();

            var accountYear = _db.AccountYears.Find(accountYearId);

            var viewModel = new OrderReportsViewModel
            {
                Orders = allOrders,
                Suppliers = allSuppliers,
                AccountYearId = accountYearId,
                TimeFrameDesc = accountYear == null ? "Received between: " + startDate.ToShortDateString() + " and " + endDate.ToShortDateString() : "Account Year: " + accountYear.AccountYear1,
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

            var allSuppliers = (from o in allOrders
                                select o.Supplier).Distinct();

            var accountYear = _db.AccountYears.Find(accountYearId);

            var viewModel = new OrderReportsViewModel
            {
                Orders = allOrders,
                Suppliers = allSuppliers,
                AccountYearId = accountYearId,
                TimeFrameDesc = accountYear == null ? "Received between: " + startDate.ToShortDateString() + " and " + endDate.ToShortDateString() : "Account Year: " + accountYear.AccountYear1,
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

            var allSuppliers = (from o in allOrders
                                select o.Supplier).Distinct();

            var accountYear = _db.AccountYears.Find(accountYearId);

            var viewModel = new OrderReportsViewModel
            {
                Orders = allOrders,
                Suppliers = allSuppliers,
                AccountYearId = accountYearId,
                TimeFrameDesc = accountYear == null ? "Received between: " + startDate.ToShortDateString() + " and " + endDate.ToShortDateString() : "Account Year: " + accountYear.AccountYear1,
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
            var accountYear = _db.AccountYears.Find(accountYearId);

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
                TimeFrameDesc = accountYear.AccountYear1,
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
            var accountYear = _db.AccountYears.Find(accountYearId);

            var allOrders = (from o in _db.OrderDetails
                             where o.AccountYearID == accountYearId && o.AuthoriserUser != null
                             select o).AsEnumerable();

            var users = UserManager.Users.AsEnumerable();

            var authorisers = (from o in allOrders
                               select o.AuthoriserUser).Distinct();

            var viewModel = new OrderReportsViewModel()
            {
                AccountYearId = accountYearId,
                TimeFrameDesc = accountYear.AccountYear1,
                Authorisers = authorisers,
                Orders = allOrders,
                HasData = authorisers.Any()
            };

            ViewBag.Title = "Orders Grouped By Authoriser";
            return View("Reports/OrdersByAuthoriser", viewModel);
        }

        public int GetCurrentAccountYearId()
        {
            var accountYear =
                (from y in _db.AccountYears
                where y.YearStartDate >= DateTime.Today && y.YearEndDate <= DateTime.Today
                select y).FirstOrDefault();
            if (accountYear == null)
            {
                return _db.AccountYears.Max(y => y.AccountYearID);
            }
            return accountYear.AccountYearID;
        }


        //Method used to supply a JSON list of Volumes when selecting a Copy (Ajax stuf)
        public JsonResult GetOrderList(int orderId = 0, string filter = "")
        {
            var allOrders = SelectListHelper.OrdersList(id: orderId, filter: filter);

            return Json(new
            {
                success = true,
                AllOrders = allOrders
            });
        }


        // GET: LibraryAdmin/OrderDetails/AddLink
        public ActionResult AddLink(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var order = _db.OrderDetails.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }

            //Get a list of any hosted (i.e. stored in database) files
            var existingFiles = _db.HostedFiles.OrderBy(f => f.FileName).ToList();

            var viewModel = new LinkedFileAddViewModel
            {
                OrderId = order.OrderID,
                Title = order.Title.Title1,
                SupplierName = order.Supplier.SupplierName
            };

            ViewBag.ExistingFile = new SelectList(existingFiles, "FileId", "FileName");
            ViewBag.ExistingFileCount = existingFiles.Count();
            ViewBag.Title = "Add New " + DbRes.T("Orders.Link_URL", "FieldDisplayName");
            return PartialView(viewModel);
        }

        // POST: LibraryAdmin/OrderDetails/PostAddLink
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult PostAddLink(LinkedFileAddViewModel viewModel)
        {
            //Check if we've been passed a URL ...
            if (!string.IsNullOrEmpty(viewModel.Url))
            {
                var orderDetail = _db.OrderDetails.Find(viewModel.OrderId);
                if (orderDetail == null)
                {
                    return null;
                }

                orderDetail.Link = viewModel.Url;
                _db.Entry(orderDetail).State = EntityState.Modified;
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
                                var type = file.ContentType;
                                var ext = Path.GetExtension(file.FileName);
                                var path = Path.GetFullPath(file.FileName);

                                viewModel.Success = HandleUpload(fileStream: file.InputStream, name: name, type: type, path: path, titleId: titleId, ext: ext,
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

                var orderDetail = _db.OrderDetails.Find(viewModel.OrderId);
                if (orderDetail == null)
                {
                    return null;
                }

                orderDetail.Link = viewModel.Url;
                _db.Entry(orderDetail).State = EntityState.Modified;
                _db.SaveChanges();

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

            return PartialView("AddLink", viewModel);
        }

        private bool HandleUpload(Stream fileStream, string name, string type, string ext, string path, int titleId, string displayText, string hoverTip, string login, string password)
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


        // GET: LibraryAdmin/OrderDetails/Delete/5
        public ActionResult Delete(int? id, string callingAction = "")
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
                CallingAction = callingAction
            };

            ViewBag.Title = "Delete " + _entityName + "?";
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

            if (viewModel.CallingAction == "edit")
            {
                UrlHelper urlHelper = new UrlHelper(HttpContext.Request.RequestContext);
                string actionUrl = urlHelper.Action("AllOutstanding", "OrderDetails");
                return Json(new {success = true, redirectTo = actionUrl});
            }
            return Json(new { success = true });
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

