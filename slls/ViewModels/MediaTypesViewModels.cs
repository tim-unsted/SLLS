using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using slls.Localization;

namespace slls.ViewModels
{
    public class MediaTypesAddViewModel
    {
        [Required (ErrorMessage = "Please enter a Media Type!")]
        [StringLength(255)]
        [LocalDisplayName("MediaTypes.Media_Type", "FieldDisplayName")]
        public string Media { get; set; }

        [LocalDisplayName("MediaTypes.Loan_Type", "FieldDisplayName")]
        public int LoanTypeID { get; set; }

        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; }
    }

    public class MediaTypesEditViewModel
    {
        public int MediaID { get; set; }

        [Required]
        [StringLength(255)]
        [LocalDisplayName("MediaTypes.Media_Type", "FieldDisplayName")]
        public string Media { get; set; }

        [LocalDisplayName("MediaTypes.Loan_Type", "FieldDisplayName")]
        public int LoanTypeID { get; set; }

        [DisplayName("Can update?")]
        public bool CanUpdate { get; set; }

        [DisplayName("Can delete?")]
        public bool CanDelete { get; set; }
    }
}