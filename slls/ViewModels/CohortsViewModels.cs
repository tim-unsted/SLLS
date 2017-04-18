using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using slls.Localization;

namespace slls.ViewModels
{
    public class CohortsAddViewModel
    {
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [LocalDisplayName("Cohorts.Cohort", "FieldDisplayName")]
        public string Cohort { get; set; }

        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; }
    }

    public class CohortsEditViewModel
    {
        public int CohortId { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [LocalDisplayName("Cohorts.Cohort", "FieldDisplayName")]
        public string Cohort { get; set; }

        [DisplayName("Can update?")]
        public bool CanUpdate { get; set; }

        [DisplayName("Can delete?")]
        public bool CanDelete { get; set; }
    }
}