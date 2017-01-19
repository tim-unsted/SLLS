using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace slls.ViewModels
{
    public class ActivityTypesAddViewModel
    {
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Activity { get; set; }

        [DisplayName("Can update?")]
        public bool CanUpdate { get; set; }

        [DisplayName("Can delete?")]
        public bool CanDelete { get; set; }
    }

    public class ActivityTypesEditViewModel
    {
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public int ActivityCode { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Activity { get; set; }

        [DisplayName("Can update?")]
        public bool CanUpdate { get; set; }

        [DisplayName("Can delete?")]
        public bool CanDelete { get; set; }

    }
}