﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using slls.Localization;

namespace slls.ViewModels
{
    public class GendersAddViewModel
    {
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [LocalDisplayName("Genders.Gender", "FieldDisplayName")]
        public string Gender { get; set; }

        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; }
    }

    public class GendersEditViewModel
    {
        public int GenderId { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [LocalDisplayName("Genders.Gender", "FieldDisplayName")]
        public string Gender { get; set; }

        [DisplayName("Can update?")]
        public bool CanUpdate { get; set; }

        [DisplayName("Can delete?")]
        public bool CanDelete { get; set; }
    }
}