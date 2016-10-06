using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using slls.ViewModels;

namespace slls.Areas.Config
{
    public class ConnectionController : ConfigBaseController
    {
        public struct ConnectionRec
        {
            public string InitialCatalog;
            public string DataSource;
            public string UserId;
            public string Pwd;
        }

        public ActionResult Details()
        {
            var viewModel = new ConnectionEditViewModel();
            var strConn = ConfigurationManager.ConnectionStrings["SLLS"].ConnectionString;

            var arrConn = strConn.Split(';');

            foreach (var connPart in arrConn)
            {
                var arrConnPart = connPart.Split('=');
                if (arrConnPart[0].IndexOf("initial catalog", StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    viewModel.InitialCatalog = arrConnPart[1];
                }
                if (arrConnPart[0].IndexOf("data source", StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    viewModel.DataSource = arrConnPart[1];
                }
                if (arrConnPart[0].IndexOf("user id", StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    viewModel.UserId = arrConnPart[1];
                }
                if (arrConnPart[0].IndexOf("password", StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    viewModel.Pwd = arrConnPart[1];
                }
            }

            ViewBag.Title = "Database Connection Details";
            return PartialView(viewModel);
        }

        [HttpGet]
        public ActionResult Edit()
        {
            var viewModel = new ConnectionEditViewModel();
            var strConn = ConfigurationManager.ConnectionStrings["SLLS"].ConnectionString;

            var arrConn = strConn.Split(';');

            foreach (var connPart in arrConn)
            {
                var arrConnPart = connPart.Split('=');
                if (arrConnPart[0].IndexOf("initial catalog", StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    viewModel.InitialCatalog = arrConnPart[1];
                }
                if (arrConnPart[0].IndexOf("data source", StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    viewModel.DataSource = arrConnPart[1];
                }
                if (arrConnPart[0].IndexOf("user id", StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    viewModel.UserId = arrConnPart[1];
                }
                if (arrConnPart[0].IndexOf("password", StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    viewModel.Pwd = arrConnPart[1];
                }
            }

            ViewBag.Title = "Edit Database Connection";
            return PartialView(viewModel);
        }

        [HttpPost]
        public ActionResult Edit(ConnectionEditViewModel viewModel)
        {
            string[] arrConn = new string[11];

            //Get the component parts of the connection string into an array
            arrConn[0] = "Data source=" + viewModel.DataSource;
            arrConn[1] = "Initial catalog=" + viewModel.InitialCatalog;
            arrConn[2] = "User ID=" + viewModel.UserId;
            arrConn[3] = "Password=" + viewModel.Pwd;

            //the following are hard-coded for now
            arrConn[4] = "Connection Timeout=30";
            arrConn[5] = "Pooling=True";
            arrConn[6] = "Min Pool Size=5";
            arrConn[7] = "Max Pool Size=400";
            arrConn[8] = "persist security info=True";
            arrConn[9] = "MultipleActiveResultSets=True";
            arrConn[10] = "App=EntityFramework";

            //create a string from the array, delimited with ";"
            var connectionString = string.Join(";", arrConn);

            Configuration config = WebConfigurationManager.OpenWebConfiguration("~");
            ConnectionStringsSection section = config.GetSection("connectionStrings") as ConnectionStringsSection;
            if (section != null)
            {
                section.ConnectionStrings["SLLS"].ConnectionString = connectionString;
                config.Save();
            }
            
            return Json(new { success = true });
        }
        
    }
}