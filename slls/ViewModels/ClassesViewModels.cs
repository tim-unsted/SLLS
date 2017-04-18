using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using slls.Localization;

namespace slls.ViewModels
{
    public class ClassesAddViewModel
    {
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [LocalDisplayName("Classes.Class", "FieldDisplayName")]
        public string Class { get; set; }

        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; }
    }

    public class ClassesEditViewModel
    {
        public int ClassId { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [LocalDisplayName("Classes.Class", "FieldDisplayName")]
        public string Class { get; set; }

        [DisplayName("Can update?")]
        public bool CanUpdate { get; set; }

        [DisplayName("Can delete?")]
        public bool CanDelete { get; set; }
    }
}