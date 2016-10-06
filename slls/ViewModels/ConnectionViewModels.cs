using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace slls.ViewModels
{
    public class ConnectionEditViewModel
    {
        [DisplayName("Database Name")]
        public string InitialCatalog { get; set; }

        [DisplayName("Server Name")]
        public string DataSource { get; set; }

        [DisplayName("SQL User ID")]
        public string UserId { get; set; }

        [DisplayName("Password")]
        public string Pwd { get; set; }
    }
}