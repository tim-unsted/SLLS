using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using slls.DAO;
using slls.Models;
using slls.ViewModels;
using Westwind.Globalization;

namespace slls.Areas.LibraryAdmin
{
    public class ExportDataController : AdminBaseController
    {
        private readonly DbEntities _db = new DbEntities();

        public Dictionary<string, string> GetFormats()
        {
            return new Dictionary<string, string>
            {
                {"new", "Excel 2007 onwards (*.xlsx)"},    
                {"old", "Excel 97-2003 (*.xls)"}
            };
        }

        public Dictionary<string, string> GetCatalogueExportTypes()
        {
            return new Dictionary<string, string>
            {
                {"titles", "Titles only"},    
                {"titlescopies", "Titles & " + DbRes.T("Copies.Copy", "FieldDisplayName") + " details"},
                {"titlescopiesvolumes", "Titles, " + DbRes.T("Copies", "EntityType") + " & " + DbRes.T("Copies.Copy_Items", "FieldDisplayName").ToLower() + " (Volume) details"}
            };
        }

        public Dictionary<string, string> GetFinanceExportTypes()
        {
            return new Dictionary<string, string>
            {
                {"allorders", "All Orders"},
                {"outstanding", "All Outstanding (current)"},
                {"overdue", "All Overdue (current)"},
                {"received", "All Received"},
                {"onapproval", "All On Approval"},
                {"accepted", "All Accepted"},
                {"cancelled", "All Cancelled"},
                {"chased", "All Chased"},
            };
        }

        public ActionResult ExportCatalogue()
        {
            return RedirectToAction("ExportData", new { exportWhat = "catalogue" });
        }

        public ActionResult ExportFinance()
        {
            return RedirectToAction("ExportData", new { exportWhat = "finance" });
        }

        // GET: LibraryAdmin/ExportData
        public ActionResult ExportData(string exportWhat = "catalogue")
        {
            var viewModel = new ExportDataViewModel()
            {
                ConfirmButtonText = "Export",
                ExportWhat = exportWhat
            };

            if (exportWhat == "finance")
            {
                viewModel.AccountYears = CacheProvider.GetAll<AccountYear>("accountyears");
                viewModel.BudgetCodes = CacheProvider.GetAll<BudgetCode>("budgetcodes");
                viewModel.OrderCategories = CacheProvider.GetAll<OrderCategory>("ordercategories");
                viewModel.Suppliers = CacheProvider.GetAll<Supplier>("suppliers");
                viewModel.Requesters = _db.Users.Where(u => u.RequestedOrders.Any()).OrderBy(u => u.Lastname).ThenBy(u => u.Firstname).ToList();
                viewModel.Authorisers = _db.Users.Where(u => u.AuthorisedOrders.Any()).OrderBy(u => u.Lastname).ThenBy(u => u.Firstname).ToList();
                ViewBag.ExportTypes = GetFinanceExportTypes();
                viewModel.HeaderText = "Export Finance Data";
                viewModel.DetailsText =
                    "Use this tool to export finance data to Excel. Select the level of data you require and any filters you wish to apply.";
            }
            else
            {
                viewModel.StatusTypes = CacheProvider.GetAll<StatusType>("statustypes");
                viewModel.Languages = CacheProvider.GetAll<Language>("languages");
                viewModel.Locations = CacheProvider.GetAll<Location>("locations");
                viewModel.MediaTypes = CacheProvider.GetAll<MediaType>("mediatypes");
                viewModel.Publishers = CacheProvider.GetAll<Publisher>("publishers");
                viewModel.Classmarks = CacheProvider.GetAll<Classmark>("classmarks");
                ViewBag.ExportTypes = GetCatalogueExportTypes();
                viewModel.HeaderText = "Export Catalogue Data";
                viewModel.DetailsText =
                    "Use this tool to export catalogue data to Excel. Select the level of data you require and any filters you wish to apply.";
                viewModel.IncludeLinks = true;
                viewModel.IncludeSubjects = true;
                viewModel.IncludeTitleTexts = true;
            }

            ViewBag.ExportFormats = GetFormats();
            return PartialView(viewModel);
        }

        [HttpPost]
        public ActionResult ExportData(ExportDataViewModel viewModel)
        {
            switch (viewModel.ExportWhat)
            {
                case "finance":
                    {
                        var ordersList = new List<ExportFinanceViewModel>();
                        var allOrders = from o in _db.OrderDetails select o;

                        switch (viewModel.ExportType)
                        {
                            case "allorders":
                                {
                                   // allOrders = allOrders;
                                    break;
                                }
                            case "outstanding":
                                {
                                    allOrders = allOrders.Where(o => o.ReceivedDate == null);
                                    break;
                                }
                            case "overdue":
                                {
                                    allOrders = allOrders.Where(o => o.ReceivedDate == null && o.Expected < DateTime.Today);
                                    break;
                                }
                            case "received":
                                {
                                    allOrders = allOrders.Where(o => o.ReceivedDate != null);
                                    break;
                                }
                            case "onapproval":
                                {
                                    allOrders = allOrders.Where(o => o.ReceivedDate != null && o.OnApproval);
                                    break;
                                }
                            case "accepted":
                                {
                                    allOrders = allOrders.Where(o => o.Accepted);
                                    break;
                                }
                            case "chased":
                                {
                                    allOrders = allOrders.Where(o => o.ReceivedDate == null && o.Chased != null);
                                    break;
                                }
                            case "cancelled":
                                {
                                    allOrders = allOrders.Where(o => o.Cancelled != null);
                                    break;
                                }
                        }

                        foreach (var order in allOrders.ToList())
                        {
                            var anotherOrder = new ExportFinanceViewModel
                            {
                                Title = order.Title.Title1 ?? "",
                                Supplier = order.Supplier == null ? "" : order.Supplier.SupplierName,
                                AccountYear = order.AccountYear == null ? "" : order.AccountYear.AccountYear1,
                                BudgetCode = order.BudgetCode == null ? "" : order.BudgetCode.BudgetCode1,
                                OrderCategory = order.OrderCategory == null ? "" : order.OrderCategory.OrderCategory1,
                                ReceivedDate = string.Format("{0:yyyy-MM-dd}", order.ReceivedDate),
                                Price = order.Price ?? 0,
                                OrderNo = order.OrderNo ?? "",
                                Notes = order.Notes ?? "",
                                InvoiceRef = order.InvoiceRef ?? "",
                                Expected = string.Format("{0:yyyy-MM-dd}", order.Expected),
                                OrderDate = string.Format("{0:yyyy-MM-dd}", order.OrderDate),
                                Item = order.Item ?? "",
                                OnApproval = order.OnApproval,
                                Report = order.Report ?? "",
                                VAT = order.VAT ?? 0,
                                Chased = string.Format("{0:yyyy-MM-dd}", order.Chased),
                                Cancelled = string.Format("{0:yyyy-MM-dd}", order.Cancelled),
                                NumCopies = order.NumCopies ?? 0,
                                InvoiceDate = string.Format("{0:yyyy-MM-dd}", order.InvoiceDate),
                                ReturnedDate = string.Format("{0:yyyy-MM-dd}", order.ReturnedDate),
                                Passed = string.Format("{0:yyyy-MM-dd}", order.Passed),
                                Accepted = order.Accepted,
                                MonthSubDue = order.MonthSubDue ?? 0,
                                Link = order.Link ?? ""
                            };
                            ordersList.Add(anotherOrder);
                        }

                        switch (viewModel.ExportFormat)
                        {
                            case "old":
                                {
                                    DataExport.ExportExcel97(ordersList);
                                    return null;
                                }
                            default:
                                {
                                    DataExport.ExportExcel2007(ordersList);
                                    return null;
                                }
                        }
                    }
                default:
                    {
                        switch (viewModel.ExportType)
                        {
                            case "titles":
                                {
                                    //Create a new list for us to add stuff to later ...
                                    var titlesOnlyList = new List<ExportJustTitlesViewModel>();

                                    //Get a collection of titles to export ...
                                    var allTitles = from t in _db.Titles
                                                    where t.Deleted == false
                                                    select t;

                                    if (viewModel.MediaId > 0)
                                    {
                                        allTitles = allTitles.Where(x => x.MediaID == viewModel.MediaId);
                                    }
                                    if (viewModel.ClassmarkId > 0)
                                    {
                                        allTitles = allTitles.Where(x => x.ClassmarkID == viewModel.ClassmarkId);
                                    }
                                    if (viewModel.PublisherId > 0)
                                    {
                                        allTitles = allTitles.Where(x => x.PublisherID == viewModel.PublisherId);
                                    }
                                    if (viewModel.LanguageId > 0)
                                    {
                                        allTitles = allTitles.Where(x => x.LanguageID == viewModel.LanguageId);
                                    }
                                    if (viewModel.StatusId > 0)
                                    {
                                        allTitles = allTitles.Where(x => x.Copies.Any(c => c.StatusID == viewModel.StatusId));
                                    }
                                    if (viewModel.LocationId > 0)
                                    {
                                        allTitles = allTitles.Where(x => x.Copies.Any(c => c.LocationID == viewModel.LocationId || c.Location.ParentLocationID == viewModel.LocationId));
                                    }

                                    allTitles = allTitles.OrderBy(t => t.Title1);

                                    //Now loop through the collection and add each record to out newTitles list, created above ...
                                    foreach (var title in allTitles.ToList())
                                    {
                                        var anotherTitle = new ExportJustTitlesViewModel
                                        {
                                            TitleId = title.TitleID,
                                            Title = title.Title1 ?? "",
                                            Author = title.AuthorString ?? "",
                                            Edition = title.Edition ?? "",
                                            ISBN = title.Isbn ?? "",
                                            Publisher = title.Publisher.PublisherName ?? "",
                                            Year = title.Year ?? "",
                                            PlaceofPublication = title.PlaceofPublication ?? "",
                                            Media = title.MediaType.Media ?? "",
                                            Classmark = title.Classmark.Classmark1 ?? "",
                                            Language = title.Language.Language1 ?? "",
                                            Series = title.Series ?? "",
                                            Copies = title.Copies.Count(c => c.Deleted == false),
                                            DateCatalogued = string.Format("{0:yyyy-MM-dd}", title.DateCatalogued)
                                        };
                                        if (viewModel.IncludeLinks)
                                        {
                                            anotherTitle.Links = title.LinkString ?? "";
                                        }
                                        if (viewModel.IncludeSubjects)
                                        {
                                            anotherTitle.Keywords = title.KeywordString ?? "";
                                        }
                                        if (viewModel.IncludeTitleTexts)
                                        {
                                            anotherTitle.TitleTexts = title.TitleTextString ?? "";
                                        }
                                        titlesOnlyList.Add(anotherTitle);
                                    }
                                    //Check the format the use wants ...
                                    switch (viewModel.ExportFormat)
                                    {
                                        case "old":
                                            {
                                                DataExport.ExportExcel97(titlesOnlyList);
                                                return null;
                                            }
                                        default:
                                            {
                                                DataExport.ExportExcel2007(titlesOnlyList);
                                                return null;
                                            }
                                    }
                                }
                            case "titlescopies":
                                {
                                    //Create a new list for us to add stuff to later ...
                                    var titlesCopiesList = new List<ExportTitlesCopiesViewModel>();

                                    //Get a collection of titles to export ...
                                    var allCopies = from t in _db.Titles
                                                    join c in _db.Copies on t.TitleID equals c.TitleID
                                                    where t.Deleted == false && c.Deleted == false
                                                    select c;

                                    if (viewModel.MediaId > 0)
                                    {
                                        allCopies = allCopies.Where(x => x.Title.MediaID == viewModel.MediaId);
                                    }
                                    if (viewModel.ClassmarkId > 0)
                                    {
                                        allCopies = allCopies.Where(x => x.Title.ClassmarkID == viewModel.ClassmarkId);
                                    }
                                    if (viewModel.PublisherId > 0)
                                    {
                                        allCopies = allCopies.Where(x => x.Title.PublisherID == viewModel.PublisherId);
                                    }
                                    if (viewModel.LanguageId > 0)
                                    {
                                        allCopies = allCopies.Where(x => x.Title.LanguageID == viewModel.LanguageId);
                                    }
                                    if (viewModel.StatusId > 0)
                                    {
                                        allCopies = allCopies.Where(x => x.StatusID == viewModel.StatusId);
                                    }
                                    if (viewModel.LocationId > 0)
                                    {
                                        allCopies = allCopies.Where(x => x.LocationID == viewModel.LocationId || x.Location.ParentLocationID == viewModel.LocationId);
                                    }

                                    allCopies = allCopies.OrderBy(c => c.Title.Title1).ThenBy(c => c.CopyNumber);

                                    //Now loop through the collection and add each record to out newTitles list, created above ...
                                    foreach (var copy in allCopies)
                                    {
                                        var anotherCopy = new ExportTitlesCopiesViewModel
                                        {
                                            TitleId = copy.TitleID,
                                            Title = copy.Title.Title1 ?? "",
                                            Author = copy.Title.AuthorString ?? "",
                                            Edition = copy.Title.Edition ?? "",
                                            ISBN = copy.Title.Isbn ?? "",
                                            Publisher = copy.Title.Publisher.PublisherName ?? "",
                                            Year = copy.Title.Year ?? "",
                                            PlaceofPublication = copy.Title.PlaceofPublication ?? "",
                                            Media = copy.Title.MediaType.Media ?? "",
                                            Classmark = copy.Title.Classmark.Classmark1 ?? "",
                                            Language = copy.Title.Language.Language1 ?? "",
                                            Series = copy.Title.Series ?? "",
                                            //Keywords = copy.Title.KeywordString ?? "",
                                            //Links = copy.Title.LinkString ?? "",
                                            //TitleTexts = copy.Title.TitleTextString ?? "",
                                            Copy = copy.CopyNumber,
                                            Location = copy.Location.LocationString ?? "",
                                            Status = copy.StatusType == null ? "" : copy.StatusType.Status,
                                            Holdings = copy.Holdings ?? "",
                                            Volumes = copy.Volumes.Count(volume => volume.Deleted == false),
                                            DateCatalogued = string.Format("{0:yyyy-MM-dd}", copy.Title.DateCatalogued)
                                        };
                                        if (viewModel.IncludeLinks)
                                        {
                                            anotherCopy.Links = copy.Title.LinkString ?? "";
                                        }
                                        if (viewModel.IncludeSubjects)
                                        {
                                            anotherCopy.Keywords = copy.Title.KeywordString ?? "";
                                        }
                                        if (viewModel.IncludeTitleTexts)
                                        {
                                            anotherCopy.TitleTexts = copy.Title.TitleTextString ?? "";
                                        }
                                        titlesCopiesList.Add(anotherCopy);
                                    }
                                    //Check the format the use wants ...
                                    switch (viewModel.ExportFormat)
                                    {
                                        case "old":
                                            {
                                                DataExport.ExportExcel97(titlesCopiesList);
                                                return null;
                                            }
                                        default:
                                            {
                                                DataExport.ExportExcel2007(titlesCopiesList);
                                                return null;
                                            }
                                    }
                                }
                            case "titlescopiesvolumes":
                                //Create a new list for us to add stuff to later ...
                                var titlesCopiesVolumesList = new List<ExportTitlesCopiesVolumesViewModel>();

                                //Get a collection of titles to export ...
                                var allVolumes = from t in _db.Titles
                                                 join c in _db.Copies on t.TitleID equals c.TitleID
                                                 join v in _db.Volumes on c.CopyID equals v.CopyID
                                                 where t.Deleted == false && c.Deleted == false && v.Deleted == false
                                                 select v;

                                if (viewModel.MediaId > 0)
                                {
                                    allVolumes = allVolumes.Where(x => x.Copy.Title.MediaID == viewModel.MediaId);
                                }
                                if (viewModel.ClassmarkId > 0)
                                {
                                    allVolumes = allVolumes.Where(x => x.Copy.Title.ClassmarkID == viewModel.ClassmarkId);
                                }
                                if (viewModel.PublisherId > 0)
                                {
                                    allVolumes = allVolumes.Where(x => x.Copy.Title.PublisherID == viewModel.PublisherId);
                                }
                                if (viewModel.LanguageId > 0)
                                {
                                    allVolumes = allVolumes.Where(x => x.Copy.Title.LanguageID == viewModel.LanguageId);
                                }
                                if (viewModel.StatusId > 0)
                                {
                                    allVolumes = allVolumes.Where(x => x.Copy.StatusID == viewModel.StatusId);
                                }
                                if (viewModel.LocationId > 0)
                                {
                                    allVolumes = allVolumes.Where(x => x.Copy.LocationID == viewModel.LocationId || x.Copy.Location.ParentLocationID == viewModel.LocationId);
                                }

                                allVolumes = allVolumes.OrderBy(v => v.Copy.Title.Title1).ThenBy(v => v.Copy.CopyNumber).ThenBy(v => v.Barcode);

                                //Now loop through the collection and add each record to out newTitles list, created above ...
                                foreach (var volume in allVolumes)
                                {
                                    var anotherVolume = new ExportTitlesCopiesVolumesViewModel()
                                    {
                                        TitleId = volume.Copy.TitleID,
                                        Title = volume.Copy.Title.Title1 ?? "",
                                        Author = volume.Copy.Title.AuthorString ?? "",
                                        Edition = volume.Copy.Title.Edition ?? "",
                                        ISBN = volume.Copy.Title.Isbn ?? "",
                                        Publisher = volume.Copy.Title.Publisher.PublisherName ?? "",
                                        Year = volume.Copy.Title.Year ?? "",
                                        PlaceofPublication = volume.Copy.Title.PlaceofPublication ?? "",
                                        Media = volume.Copy.Title.MediaType.Media ?? "",
                                        Classmark = volume.Copy.Title.Classmark.Classmark1 ?? "",
                                        Language = volume.Copy.Title.Language.Language1 ?? "",
                                        Series = volume.Copy.Title.Series ?? "",
                                        //Keywords = volume.Copy.Title.KeywordString ?? "",
                                        //Links = volume.Copy.Title.LinkString ?? "",
                                        //TitleTexts = volume.Copy.Title.TitleTextString ?? "",
                                        Copy = volume.Copy.CopyNumber,
                                        Location = volume.Copy.Location.Location1 ?? "",
                                        //Status = volume.Copy.StatusType.Status,
                                        Status = volume.Copy.StatusType == null ? "" : volume.Copy.StatusType.Status,
                                        Holdings = volume.Copy.Holdings ?? "",
                                        Barcode = volume.Barcode ?? "",
                                        LabelText = volume.LabelText ?? "",
                                        LoanType = volume.LoanType.LoanTypeName ?? "",
                                        OnLoan = volume.OnLoan,
                                        DateCatalogued = string.Format("{0:yyyy-MM-dd}", volume.Copy.Title.DateCatalogued)
                                    };
                                    if (viewModel.IncludeLinks)
                                    {
                                        anotherVolume.Links = volume.Copy.Title.LinkString ?? "";
                                    }
                                    if (viewModel.IncludeSubjects)
                                    {
                                        anotherVolume.Keywords = volume.Copy.Title.KeywordString ?? "";
                                    }
                                    if (viewModel.IncludeTitleTexts)
                                    {
                                        anotherVolume.TitleTexts = volume.Copy.Title.TitleTextString ?? "";
                                    }
                                    titlesCopiesVolumesList.Add(anotherVolume);
                                }
                                //Check the format the use wants ...
                                switch (viewModel.ExportFormat)
                                {
                                    case "old":
                                        {
                                            DataExport.ExportExcel97(titlesCopiesVolumesList);
                                            return null;
                                        }
                                    default:
                                        {
                                            DataExport.ExportExcel2007(titlesCopiesVolumesList);
                                            return null;
                                        }
                                }

                            default:
                                {
                                    return null;
                                }
                        }
                    }
            }
        }
    }
}