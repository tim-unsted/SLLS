using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using slls.Localization;

namespace slls.ViewModels
{
    public class StatusTypesAddViewModel
    {
        [StringLength(255)]
        [LocalDisplayName("StatusTypes.Status_Type", "FieldDisplayName")]
        public string Status { get; set; }

        [LocalDisplayName("StatusTypes.Opac", "FieldDisplayName")]
        public bool Opac { get; set; }
        
        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; }
    }

    public class StatusTypesEditViewModel
    {
        public int StatusID { get; set; }

        [StringLength(255)]
        [LocalDisplayName("StatusTypes.Status_Type", "FieldDisplayName")]
        public string Status { get; set; }

        [LocalDisplayName("StatusTypes.Opac", "FieldDisplayName")]
        public bool Opac { get; set; }

        [DisplayName("Can update?")]
        public bool CanUpdate { get; set; }

        [DisplayName("Can delete?")]
        public bool CanDelete { get; set; }
    }
}