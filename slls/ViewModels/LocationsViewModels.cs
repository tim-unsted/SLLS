using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using slls.Localization;

namespace slls.ViewModels
{
    public class LocationsAddViewModel
    {
        [LocalDisplayName("Locations.Parent_Location", "FieldDisplayName")]
        public int? ParentLocationID { get; set; }

        [LocalDisplayName("Locations.Location", "FieldDisplayName")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Location { get; set; }

        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; }
    }

    public class LocationsEditViewModel
    {
        public int LocationID { get; set; }

        [LocalDisplayName("Locations.Parent_Location", "FieldDisplayName")]
        public int? ParentLocationID { get; set; }

        [LocalDisplayName("Locations.Location", "FieldDisplayName")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Location { get; set; }

        [DisplayName("Can update?")]
        public bool CanUpdate { get; set; }

        [DisplayName("Can delete?")]
        public bool CanDelete { get; set; }
    }
}