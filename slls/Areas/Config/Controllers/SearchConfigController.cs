using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using slls.DAO;
using slls.Models;
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
            ViewBag.Title = "Search Order Types";
            return View(orderTypes.ToList());
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
    }
}