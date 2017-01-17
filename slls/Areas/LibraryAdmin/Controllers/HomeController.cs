using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls.Expressions;
using Microsoft.AspNet.Identity;
using slls.DAO;
using slls.Models;
using slls.Utils.Helpers;
using slls.ViewModels;
using Westwind.Globalization;
using slls.App_Settings;

//The LIBRARY ADMIN home Controller

namespace slls.Areas.LibraryAdmin
{
    public class HomeController : AdminBaseController
    {
        private readonly DbEntities _db = new DbEntities();
        private readonly string _customerPackage = App_Settings.GlobalVariables.Package;

        //Get the available gadgets from the database or cache ...
        private readonly IEnumerable<DashboardGadget> _allGadgets = CacheProvider.DashboardGadgets().Where(x => x.Area == "Admin");
        
        public HomeController()
        {
            ViewBag.Title = "Library Admin Dashboard";
        }

        //[OutputCache(Duration = 300, VaryByParam = "none")] //cached for 300 seconds  
        public ActionResult Index()
        {
            TempData["LoansSelectorViewModel"] = null;
            var viewModel = new DashboardViewModel()
            {
                ShowWelcomeMessage = App_Settings.Settings.GetParameterValue("Admin.ShowWelcomeMessage", "true", dataType: "bool") == "true",
                WelcomeHeader = App_Settings.Settings.GetParameterValue("Admin.WelcomeHeader", "Library Admin", dataType: "longtext"),
                WelcomeMessage = App_Settings.Settings.GetParameterValue("Admin.WelcomeMessage", "Simple Little Library System Admin Dashboard", dataType: "longtext")
            };

            using (var db = new DbEntities())
            {
                viewModel.HasTitles = db.Titles.Any();
            }

            return View(viewModel);
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Bailey Solutions Ltd";
            return View();
        }

        public ActionResult NotFound()
        {
            return View();
        }

        [ActionOutputCache(300)] // Caches for n seconds
        public ActionResult RenderGadget(int col = 0, int row = 0, bool hasTitles = false)
        {
            //Get the current user's ID ...
            var userId = Utils.PublicFunctions.GetUserId();
            var gadgetAction =
                _allGadgets.Where(x => x.Row == row && x.Column == col).Select(g => g.Name).FirstOrDefault();

            if (gadgetAction == null)
            {
                return null;
            }

            switch (gadgetAction.ToLower())
            {
                
                case "autocat":
                    {
                        //Get a list of all data soures (i.e. Amazon;Hammick;Wildys; etc.)
                        var allSources = ConfigurationManager.AppSettings["AutoCatDataSources"];
                        var sources = allSources.Split(',').ToList();

                        var viewModel = new AddTitleWithAutoCatViewModel
                        {
                            Sources = sources
                        };

                        ViewBag.Title = "AutoCat";
                        return PartialView("_AutoCat", viewModel);
                    }
                case "alerts":
                    {
                        var viewModel = new AdminAlertsViewModel()
                        {
                            Package = _customerPackage
                        };

                        //All packages have titles ...
                        viewModel.HasTitles = _db.Titles.Any(x => x.Deleted == false);
                        viewModel.HasAuthors = _db.Authors.Any(x => x.Deleted == false);
                        viewModel.HasClassmarks = _db.Classmarks.Any(x => x.Deleted == false && x.CanDelete);
                        viewModel.HasFrequencies = _db.Frequencies.Any(x => x.Deleted == false && x.CanDelete);
                        viewModel.HasKeywords = _db.Keywords.Any(x => x.Deleted == false && x.CanDelete);
                        viewModel.HasLanguages = _db.Languages.Any(x => x.Deleted == false && x.CanDelete);
                        viewModel.HasLocations = _db.Locations.Any(x => x.Deleted == false && x.CanDelete);
                        viewModel.HasMediaTypes = _db.MediaTypes.Any(x => x.Deleted == false && x.CanDelete);
                        viewModel.HasPublishers = _db.Publishers.Any(x => x.Deleted == false && x.CanDelete);
                        viewModel.HasStatusTypes = _db.StatusTypes.Any(x => x.Deleted == false && x.CanDelete);

                        //All packages except sharing has loans and users
                        if (_customerPackage != "collectors")
                        {
                            viewModel.HasLoans = _db.Borrowings.Any(x => x.Deleted == false);
                            viewModel.HasDepartments = _db.Departments.Any(x => x.Deleted == false && x.CanDelete);
                            viewModel.HasLoanTypes = _db.LoanTypes.Any(x => x.CanDelete);
                        }
                        
                        //Only expert and super packages have serials and finance
                        if (_customerPackage != "collectors" && _customerPackage != "sharing")
                        {
                            viewModel.HasOrders = _db.OrderDetails.Any(x => x.Deleted == false);
                            viewModel.HasAccountYears = _db.AccountYears.Any(x => x.Deleted == false && x.CanDelete);
                            viewModel.HasActivityTypes = _db.ActivityTypes.Any(x => x.Deleted == false && x.CanDelete);
                            viewModel.HasBudgetCodes = _db.BudgetCodes.Any(x => x.Deleted == false && x.CanDelete);
                            viewModel.HasSuppliers = _db.Suppliers.Any(x => x.Deleted == false && x.CanDelete);
                            viewModel.HasOrderCategories = _db.OrderCategories.Any(x => x.Deleted == false && x.CanDelete);
                        }

                        if (hasTitles)
                        {
                            viewModel.TitlesNoCopies = _db.Titles.Count(t => t.Copies.Count == 0);
                            viewModel.CopiesNoVolumes = _db.Copies.Count(c => c.Volumes.Count == 0);
                        }

                        if (viewModel.HasOrders)
                        {
                            viewModel.OutstandingOrders = (from o in _db.OrderDetails
                                                           where o.ReceivedDate == null && o.Cancelled == null
                                                           select o).Count();
                            viewModel.OverdueOrders = (from o in _db.OrderDetails
                                                       where o.ReceivedDate == null && o.Cancelled == null && o.Expected < DateTime.Today
                                                       select o).Count();
                            viewModel.OrdersOnApproval = (from o in _db.OrderDetails
                                                          where o.ReceivedDate != null && o.Cancelled == null && o.OnApproval
                                                          select o).Count();

                            var today = DateTime.Today;
                            var futureDay = DateTime.Today.AddDays(0);
                            viewModel.IssuesExpected = _db.Copies.Include(c => c.Title).Include(c => c.PartsReceived)
                                .Where(c => c.Cancellation == null && c.PartsReceived.Any() && c.Title.Frequency.Days > 0 && DbFunctions.AddDays(c.PartsReceived.OrderByDescending(p => p.DateReceived).FirstOrDefault().DateReceived.Value, c.Title.Frequency.Days) >= today && DbFunctions.AddDays(c.PartsReceived.OrderByDescending(p => p.DateReceived).FirstOrDefault().DateReceived.Value, c.Title.Frequency.Days) <= futureDay)
                                .OrderBy(c => c.Title.Title1).ThenBy(c => c.CopyNumber).Count();

                            viewModel.OverdueIssues = _db.Copies
                                .Include(
                                    c =>
                                        c.PartsReceived).Count(c => c.Cancellation == null && c.PartsReceived.Any() && c.Title.Frequency.Days > 0 &&
                                        DbFunctions.DiffDays(
                                            c.PartsReceived.OrderByDescending(p => p.DateReceived).FirstOrDefault().DateReceived.Value,
                                            DateTime.Now) > c.Title.Frequency.Days);
                        }

                        if (viewModel.HasLoans)
                        {
                            viewModel.ReservationRequests = 0;
                            viewModel.OverdueLoans = _db.Borrowings.Count(b => b.ReturnDue < DateTime.Today && b.Returned == null);
                        }

                        ViewBag.Title = "Alerts";
                        return PartialView("_Alerts", viewModel);
                    }

                case "notifications":
                    {
                        var allNotifications = CacheProvider.GetAll<Notification>("notifications").ToList();
                        var opacNotifications = allNotifications.Where(n => n.Scope == "A" && n.Visible && (n.ExpireDate == null || n.ExpireDate > DateTime.Today)).OrderBy(n => n.Position).Take(5).ToList();

                        if (!opacNotifications.Any())
                        {
                            return null;
                        }

                        var viewModel = new DashboardViewModel { Notifications = opacNotifications };
                        var count = opacNotifications.Count();
                        var countLabel = DbRes.T("Notifications.Notification", "FieldDisplayName");
                        if (count > 1)
                        {
                            countLabel = countLabel + "s";
                        }
                        ViewBag.Title = DbRes.T("Notifications", "EntityType");
                        ViewBag.Count = count;
                        ViewBag.CountLabel = countLabel;
                        ViewBag.Showing = opacNotifications.Count() < 5 ? opacNotifications.Count() : 5;
                        return PartialView("_Notifications", viewModel);
                    }
                case "quickcat":
                    {
                        var viewModel = new TitleAddViewModel()
                        {
                            Year = DateTime.Now.ToString("yyyy"),
                            Step = 1
                        };

                        ViewBag.Title = "Quick Catalogue";
                        return PartialView("_QuickCat", viewModel);
                    }
                case "recentlycatalogued":
                    {
                        if (!hasTitles)
                        {
                            return null;
                        }

                        var viewModel = new DashboardViewModel();

                        var recentTitles = (from t in _db.Titles
                                            orderby t.DateCatalogued descending, t.Title1.Substring(t.NonFilingChars) ascending
                                            select t).Take(10);

                        if (!recentTitles.Any())
                        {
                            return null;
                        }

                        viewModel.RecentTitles = recentTitles.ToList();
                        ViewBag.Title = "Recently Catalogued";
                        ViewBag.Count = recentTitles.Count();
                        ViewBag.Showing = recentTitles.Count() < 10 ? recentTitles.Count() : 10;
                        return PartialView("_RecentlyCatalogued", viewModel);
                    }

                case "newtitles":
                    {
                        if (!hasTitles)
                        {
                            return null;
                        }

                        //Get a collection of items on the 'New Titles' list ...
                        var allnewtitles = CacheProvider.NewTitles().ToList();

                        if (!allnewtitles.Any())
                        {
                            return null;
                        }

                        var viewModel = new DashboardViewModel();

                        var newTitlesSortOrder = Settings.GetParameterValue("Searching.DefaultNewTitlesSortOrder",
                            "commenced.desc", "Sets the default sort order for the 'New Titles' list.", dataType: "text");
                        if (newTitlesSortOrder == "title.asc")
                        {
                            var newTitles = allnewtitles.OrderBy(t => t.Title.Substring(t.NonFilingChars)).GroupBy(x => x.TitleId).Select(t => t.First()).ToList();
                            viewModel.NewTitles = newTitles.Take(10).ToList();
                            ViewBag.Count = newTitles.Count();
                        }
                        else
                        {
                            var newTitles = allnewtitles.OrderByDescending(t => t.Commenced).GroupBy(x => x.TitleId).Select(t => t.First()).ToList();
                            viewModel.NewTitles = newTitles.Take(10).ToList();
                            ViewBag.Count = newTitles.Count();
                        }

                        //viewModel.NewTitles = allnewtitles.OrderBy(t => t.Title1.Substring(t.NonFilingChars)).Take(10).ToList();
                        //viewModel.NewTitles = allnewtitles.OrderBy(t => t.Title.Substring(t.NonFilingChars)).Take(10).ToList();
                        ViewBag.Title = DbRes.T("Titles.New_Titles", "FieldDisplayName");
                        ViewBag.Showing = allnewtitles.Count() < 10 ? allnewtitles.Count() : 10;
                        return PartialView("_NewTitles", viewModel);
                    }

                case "issuesexpectedtoday":
                    {
                        if (!hasTitles)
                        {
                            return null;
                        }

                        //Get a list of parts exected today or in the future ...
                        var today = DateTime.Today;
                        var futureDay = DateTime.Today.AddDays(0);
                        var viewModel = new DashboardViewModel();

                        var partsExpected = _db.Copies.Include(c => c.Title).Include(c => c.PartsReceived)
                            .Where(c => c.Cancellation == null && c.PartsReceived.Any() && c.Title.Frequency.Days > 0 && DbFunctions.AddDays(c.PartsReceived.OrderByDescending(p => p.DateReceived).FirstOrDefault().DateReceived.Value, c.Title.Frequency.Days) >= today && DbFunctions.AddDays(c.PartsReceived.OrderByDescending(p => p.DateReceived).FirstOrDefault().DateReceived.Value, c.Title.Frequency.Days) <= futureDay)
                            .OrderBy(c => c.Title.Title1).ThenBy(c => c.CopyNumber);

                        if (!partsExpected.Any())
                        {
                            return null;
                        }

                        //Calculate the data that the next part is expected:
                        foreach (var copy in partsExpected)
                        {
                            var lastOrDefault = copy.PartsReceived.LastOrDefault();
                            if (lastOrDefault != null)
                                if (lastOrDefault.DateReceived != null)
                                    if (copy.Title.Frequency.Days != null)
                                        copy.NextPartExpected =
                                            lastOrDefault.DateReceived.Value.AddDays((int)copy.Title.Frequency.Days);
                        }

                        viewModel.IssuesExpected = partsExpected.ToList();
                        ViewBag.Title = DbRes.T("Serials.Parts", "FieldDisplayName") + " Expected Today";
                        ViewBag.Count = partsExpected.Count();
                        ViewBag.Showing = partsExpected.Count() < 10 ? partsExpected.Count() : 10;
                        return PartialView("_IssuesExpected", viewModel);
                    }

                case "partsoverdue":
                    {
                        if (!hasTitles)
                        {
                            return null;
                        }

                        var viewModel = new DashboardViewModel();

                        //Get a list of overdue items:
                        var partsOverdue = _db.Copies.Include(c => c.Title).Include(c => c.PartsReceived)
                            .Where(
                                c =>
                                    c.Cancellation == null && c.PartsReceived.Any() && c.Title.Frequency.Days > 0 &&
                                    DbFunctions.DiffDays(
                                        c.PartsReceived.OrderByDescending(p => p.DateReceived).FirstOrDefault().DateReceived.Value,
                                        DateTime.Now) > c.Title.Frequency.Days);


                        if (!partsOverdue.Any())
                        {
                            return null;
                        }

                        //Calculate when the next part was expected:
                        foreach (var copy in partsOverdue)
                        {
                            var lastOrDefault = copy.PartsReceived.LastOrDefault();
                            if (lastOrDefault != null)
                                if (lastOrDefault.DateReceived != null)
                                    if (copy.Title.Frequency.Days != null)
                                        copy.NextPartExpected =
                                            lastOrDefault.DateReceived.Value.AddDays((int)copy.Title.Frequency.Days);
                        }

                        viewModel.IssuesExpected = partsOverdue.ToList(); //(from p in partsOverdue orderby p.NextPartExpected select p).Take(10);
                        ViewBag.Title = "Overdue " + DbRes.T("Serials.Parts", "FieldDisplayName");// "Parts/Issues Overdue";
                        ViewBag.Count = partsOverdue.Count();
                        ViewBag.Showing = partsOverdue.Count() < 10 ? partsOverdue.Count() : 10;
                        return PartialView("_PartsOverdue", viewModel);
                    }

                case "overdueorders":
                    {
                        if (!hasTitles)
                        {
                            return null;
                        }

                        var viewModel = new DashboardViewModel();
                        var overdueOrders = from o in _db.OrderDetails
                                            where o.ReceivedDate == null && o.Cancelled == null && o.Expected < DateTime.Today
                                            select o;

                        if (!overdueOrders.Any())
                        {
                            return null;
                        }

                        viewModel.OverdueOrders = overdueOrders.ToList();
                        ViewBag.Title = "Overdue  " + DbRes.T("Orders", "EntityType");
                        ViewBag.Count = overdueOrders.Count();
                        ViewBag.Showing = overdueOrders.Count() < 10 ? overdueOrders.Count() : 10;
                        return PartialView("_OverdueOrders", viewModel);
                    }
                case "searchgadget":
                    {
                        if (!hasTitles)
                        {
                            return null;
                        }

                        ViewData["SearchField"] = SelectListHelper.SearchFieldsList();
                        ViewBag.Title = DbRes.T("Search", "Terminology");
                        return PartialView("_SearchGadget");
                    }
                case "barcodesearch":
                    {
                        if (!hasTitles)
                        {
                            return null;
                        }

                        ViewBag.Title = DbRes.T("CopyItems.Barcode", "FieldDisplayName") + " Look-Up";
                        return PartialView("_BarcodeSearch");
                    }
            }

            return null;
        }
    }
}