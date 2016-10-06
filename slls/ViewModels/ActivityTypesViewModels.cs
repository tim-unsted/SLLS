using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace slls.ViewModels
{
    public class ActivityTypesAddViewModel
    {
        public string Activity { get; set; }

        [DisplayName("Can update?")]
        public bool CanUpdate { get; set; }

        [DisplayName("Can delete?")]
        public bool CanDelete { get; set; }
    }

    public class ActivityTypesEditViewModel
    {
        public int ActivityCode { get; set; }

        public string Activity { get; set; }

        [DisplayName("Can update?")]
        public bool CanUpdate { get; set; }

        [DisplayName("Can delete?")]
        public bool CanDelete { get; set; }

    }
}