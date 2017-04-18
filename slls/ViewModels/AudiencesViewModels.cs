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
    public class AudienceAddViewModel
    {
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [LocalDisplayName("Audiences.Audience", "FieldDisplayName")]
        public string Audience { get; set; }

        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; }
    }

    public class AudienceEditViewModel
    {
        public int AudienceId { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [LocalDisplayName("Audiences.Audience", "FieldDisplayName")]
        public string Audience { get; set; }

        [DisplayName("Can update?")]
        public bool CanUpdate { get; set; }

        [DisplayName("Can delete?")]
        public bool CanDelete { get; set; }
    }
}