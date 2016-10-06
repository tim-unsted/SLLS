using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using slls.Localization;

namespace slls.ViewModels
{
    public class FrequencyAddViewModel
    {
        [LocalDisplayName("Frequency.Frequency", "FieldDisplayName")]
        [StringLength(255)]
        public string Frequency { get; set; }

        [DisplayName("Days Between")]
        [LocalDisplayName("Frequency.DaysBetween", "FieldDisplayName")]
        [RegularExpression("([0-9]+)", ErrorMessage = "Please enter valid number, either 0 (zero) or greater")]
        [Required(ErrorMessage = "Please enter a value for Days Between.")]
        [Range(0, 9999)]
        public int? Days { get; set; }

        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; }
    }

    public class FrequencyEditViewModel
    {
        public int FrequencyID { get; set; }

        [LocalDisplayName("Frequency.Frequency", "FieldDisplayName")]
        [StringLength(255)]
        public string Frequency { get; set; }

        [DisplayName("Days Between")]
        [LocalDisplayName("Frequency.DaysBetween", "FieldDisplayName")]
        [RegularExpression("([0-9]+)", ErrorMessage = "Please enter valid number, either 0 (zero) or greater")]
        [Required(ErrorMessage = "Please enter a value for Days Between.")]
        [Range(0, 9999)]
        public int? Days { get; set; }

        [DisplayName("Can update?")]
        public bool CanUpdate { get; set; }

        [DisplayName("Can delete?")]
        public bool CanDelete { get; set; }
    }
}