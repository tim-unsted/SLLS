using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using slls.Models;
using slls.DAO;

//The CONFIG home Contrller

namespace slls.Areas.Config
{
    public class HomeController : ConfigBaseController
    {
        public HomeController()
        {
            ViewBag.Title = "System Configuration Home Page";
        }
        
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult NotFound()
        {
            return View();
        }
        
    }
}