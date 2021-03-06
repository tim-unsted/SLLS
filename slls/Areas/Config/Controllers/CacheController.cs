﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using slls.DAO;
using slls.ViewModels;
using Westwind.Globalization;

namespace slls.Areas.Config
{
    public class CacheController : ConfigBaseController
    {
        // GET: Config/Cache
        public ActionResult Index()
        {
            //return View();
            return null;
        }

        // GET: Config/Cache/Details/5
        public ActionResult Details(int id)
        {
           // return View();
            return null;
        }

        // GET: Config/Cache/Delete/5
        public ActionResult Remove(int id)
        {
            //return View();
            return null;
        }

        // POST: Config/Cache/Delete/5
        [HttpPost]
        public ActionResult Remove(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return null;
            }
        }

        [HttpGet]
        public ActionResult RemoveAll()
        {
            var newTitlesList = DbRes.T("NewTitlesList", "EntityType");
            var gcvm = new GenericConfirmationViewModel
            {
                PostConfirmController = "Cache",
                PostConfirmAction = "DoRemoveAll",
                ConfirmationText = "You are about to remove all cached data. Are you sure you want to continue?",
                ConfirmButtonText = "Clear Cache",
                ConfirmButtonClass = "btn-danger",
                CancelButtonText = "Cancel",
                HeaderText = "Clear Cache?",
                Glyphicon = "glyphicon-remove"
            };
            return PartialView("_GenericConfirmation", gcvm);

        }

        [HttpPost]
        public ActionResult DoRemoveAll()
        {
            CacheProvider.RemoveAll();
            return Json(new { success = true });
            //return RedirectToAction("Index", "Home");
        }
    }
}
