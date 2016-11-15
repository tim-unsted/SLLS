using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using slls.App_Settings;
using slls.DAO;
using slls.Models;
using slls.Utils.Helpers;
using slls.ViewModels;

namespace slls.Areas.Config
{
    public class SearchConfigController : ConfigBaseController
    {
        private readonly DbEntities _db = new DbEntities();
        
        // GET: Config/SearchConfig
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SearchOrderTypes()
        {
            var orderTypes = _db.SearchOrderTypes.Where(x => x.Scope == "o");
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("seeAlsoSearchingConfig", "SearchOrderTypes");
            ViewBag.Title = "Search Order Types";
            return View(orderTypes.ToList());
        }

        public ActionResult SearchFields()
        {
            var allSearchFields = CacheProvider.GetAll<SearchField>("searchfields");
            var searchFields = allSearchFields.Where(f => f.Scope == "opac");
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("seeAlsoSearchingConfig", "SearchFields");
            ViewBag.Title = "Search Fields";
            return View(searchFields.ToList());
        }

        public ActionResult Searchoptions(bool success = false)
        {
            var viewModel = new SearchOptionsViewModel()
            {
                DefaultSearchOrder = Settings.GetParameterValue("Searching.DefaultSortOrder", "title.asc", "Sets the default sort order for search results."),
                DefaultSearchOrderTip = "Sets the default sort order for search results.",
                DefaultNewTitlesOrder = Settings.GetParameterValue("Searching.DefaultNewTitlesSortOrder", "commenced.desc", "Sets the default sort order for the 'New Titles' list."),
                DefaultNewTitlesOrderTip = "Sets the default sort order for the 'New Titles' list.",
                SearchResultsPageSize = int.Parse(Settings.GetParameterValue("Searching.SearchResultSize", "10", "Sets the page size for OPAC simple search.")),
                SearchResultsPageSizeTip = "Sets the page size for OPAC simple search",
                NarrowByDefaultRecordCount = int.Parse(Settings.GetParameterValue("Searching.NarrowByDefaultRecordCount", "5", "Sets the number of initial narrow-by options for each narrow-by source (e.g. Media Types).")),
                NarrowByDefaultRecordCountTip = "Sets the number of initial narrow-by options for each narrow-by source (e.g. Media Types)."
            };

            if (success)
            {
                TempData["SuccessMsg"] = "Search options saved/updated successfully.";
            }

            ViewData["SeeAlso"] = MenuHelper.SeeAlso("seeAlsoSearchingConfig", "Searchoptions");
            ViewData["DefaultSearchOrder"] = SelectListHelper.OpacResultsOrderBy(viewModel.DefaultSearchOrder);
            ViewData["DefaultNewTitlesOrder"] = SelectListHelper.NewTitlesOrderBy(viewModel.DefaultNewTitlesOrder);
            ViewBag.Title = "Search Options";
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Searchoptions(SearchOptionsViewModel viewModel)
        {
            if (!string.IsNullOrEmpty(viewModel.DefaultSearchOrder))
            {
                Settings.UpdateParameter("Searching.DefaultSortOrder", viewModel.DefaultSearchOrder);
            }
            if (!string.IsNullOrEmpty(viewModel.DefaultNewTitlesOrder))
            {
                Settings.UpdateParameter("Searching.DefaultNewTitlesSortOrder", viewModel.DefaultNewTitlesOrder);
            }
            if (viewModel.SearchResultsPageSize != 0)
            {
                Settings.UpdateParameter("Searching.SearchResultSize", viewModel.SearchResultsPageSize.ToString());
            }
            if (viewModel.NarrowByDefaultRecordCount != 0)
            {
                Settings.UpdateParameter("Searching.NarrowByDefaultRecordCount", viewModel.NarrowByDefaultRecordCount.ToString());
            }
            
            return RedirectToAction("Searchoptions", new { success = true });
        }

        public ActionResult EditOrderType(int id = 0)
        {
            var orderType = _db.SearchOrderTypes.Find(id);
            if (orderType == null)
            {
                return HttpNotFound();
            }
            var viewModel = new SearchOrderTypeViewModel()
            {
                Scope = orderType.Scope,
                OrderTypeFriendly = orderType.OrderTypeFriendly,
                OrderTypeID = orderType.OrderTypeID,
                OrderType = orderType.OrderType,
                Display = orderType.Display
            };
            ViewBag.Title = "Edit Search Order Type";
            return PartialView(viewModel);
        }

        [HttpPost]
        public ActionResult EditOrderType(SearchOrderTypeViewModel viewModel)
        {
            var orderType = _db.SearchOrderTypes.Find(viewModel.OrderTypeID);
            if (orderType == null)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                orderType.Scope = viewModel.Scope;
                orderType.Display = viewModel.Display;
                orderType.OrderTypeFriendly = viewModel.OrderTypeFriendly;

                _db.Entry(orderType).State = EntityState.Modified;
                _db.SaveChanges();
                CacheProvider.RemoveCache("searchordertypes");
                return Json(new {success = true});
            }

            ViewBag.Title = "Edit Search Order Type";
            return PartialView(viewModel);
        }

        public ActionResult EditSearchField(int id = 0)
        {
            var searchField = _db.SearchFields.Find(id);
            if (searchField == null)
            {
                return HttpNotFound();
            }
            var viewModel = new SearchFieldViewModel()
            {
                FieldName = searchField.FieldName,
                Scope = searchField.Scope,
                DisplayName = searchField.DisplayName,
                RecId = searchField.RecId,
                Position = searchField.Position,
                Enabled = searchField.Enabled
            };
            ViewBag.Title = "Edit Search Fields";
            return PartialView(viewModel);
        }

        [HttpPost]
        public ActionResult EditSearchField(SearchFieldViewModel viewModel)
        {
            var searchField = _db.SearchFields.Find(viewModel.RecId);
            if (searchField == null)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                searchField.Scope = viewModel.Scope;
                searchField.DisplayName = viewModel.DisplayName;
                searchField.Position = viewModel.Position;
                searchField.Enabled = viewModel.Enabled;

                _db.Entry(searchField).State = EntityState.Modified;
                _db.SaveChanges();
                CacheProvider.RemoveCache("searchfields");
                return Json(new { success = true });
            }

            ViewBag.Title = "Edit Search Fields";
            return PartialView(viewModel);
        }

        public ActionResult MoveUp(int id)
        {
            var item = _db.SearchOrderTypes.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }

            var itemAbove = (from m in _db.SearchOrderTypes
                             where m.Display < item.Display
                             orderby m.Display descending
                             select m).FirstOrDefault();
            var oldPosition = item.Display;

            if (itemAbove != null)
            {
                var newPosition = itemAbove.Display;

                //Move the selected item up one place ...
                item.Display = newPosition;
                _db.Entry(item).State = EntityState.Modified;
                _db.SaveChanges();

                //Move the item just above the selected item down one place ...
                itemAbove.Display = oldPosition;
                _db.Entry(itemAbove).State = EntityState.Modified;
                _db.SaveChanges();

                CacheProvider.RemoveCache("searchordertypes");
            }

            return RedirectToAction("SearchOrderTypes");

        }

        public ActionResult MoveDown(int id)
        {
            var item = _db.SearchOrderTypes.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }

            var oldPosition = item.Display;

            var itemBelow = (from m in _db.SearchOrderTypes
                             where m.Display > item.Display
                             orderby m.Display ascending
                             select m).FirstOrDefault();

            if (itemBelow != null)
            {
                var newPosition = itemBelow.Display;

                //Move the selected item down one place ...
                item.Display = newPosition;
                _db.Entry(item).State = EntityState.Modified;
                _db.SaveChanges();

                //Move the item just below the selected item up one place ...
                itemBelow.Display = oldPosition;
                _db.Entry(itemBelow).State = EntityState.Modified;
                _db.SaveChanges();

                CacheProvider.RemoveCache("searchordertypes");
            }

            return RedirectToAction("SearchOrderTypes");
        }

        public ActionResult MoveSearchFieldUp(int id)
        {
            var item = _db.SearchFields.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }

            var itemAbove = (from x in _db.SearchFields
                             where x.Position < item.Position
                             orderby x.Position descending
                             select x).FirstOrDefault();
            var oldPosition = item.Position;

            if (itemAbove != null)
            {
                var newPosition = itemAbove.Position;

                //Move the selected item up one place ...
                item.Position = newPosition;
                _db.Entry(item).State = EntityState.Modified;
                _db.SaveChanges();

                //Move the item just above the selected item down one place ...
                itemAbove.Position = oldPosition;
                _db.Entry(itemAbove).State = EntityState.Modified;
                _db.SaveChanges();

                CacheProvider.RemoveCache("searchfields");
            }

            return RedirectToAction("SearchFields");

        }

        public ActionResult MoveSearchFieldDown(int id)
        {
            var item = _db.SearchFields.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }

            var oldPosition = item.Position;

            var itemBelow = (from x in _db.SearchFields
                             where x.Position > item.Position
                             orderby x.Position ascending
                             select x).FirstOrDefault();

            if (itemBelow != null)
            {
                var newPosition = itemBelow.Position;

                //Move the selected item down one place ...
                item.Position = newPosition;
                _db.Entry(item).State = EntityState.Modified;
                _db.SaveChanges();

                //Move the item just below the selected item up one place ...
                itemBelow.Position = oldPosition;
                _db.Entry(itemBelow).State = EntityState.Modified;
                _db.SaveChanges();

                CacheProvider.RemoveCache("searchfields");
            }

            return RedirectToAction("SearchFields");
        }
    }
}