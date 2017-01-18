using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using slls.Localization;

namespace slls.ViewModels
{
    public class ClassmarksAddViewModel
    {
        [StringLength(255)]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [LocalDisplayName("Classmarks.Classmark", "FieldDisplayName")]
        public string Classmark { get; set; }

        [StringLength(50)]
        [LocalDisplayName("Classmarks.Short_Code", "FieldDisplayName")]
        public string Code { get; set; }

        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; }
    }

    public class ClassmarksEditViewModel
    {
        public int ClassmarkID { get; set; }

        [StringLength(255)]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [LocalDisplayName("Classmarks.Classmark", "FieldDisplayName")]
        public string Classmark { get; set; }

        [StringLength(50)]
        [LocalDisplayName("Classmarks.Short_Code", "FieldDisplayName")]
        public string Code { get; set; }

        [DisplayName("Can update?")]
        public bool CanUpdate { get; set; }

        [DisplayName("Can delete?")]
        public bool CanDelete { get; set; }
    }
}