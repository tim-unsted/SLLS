using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using slls.Localization;

namespace slls.ViewModels
{
    public class UserTypesAddViewModel
    {
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [LocalDisplayName("UserTypes.UserType", "FieldDisplayName")]
        public string UserType { get; set; }

        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; }
    }

    public class UserTypesEditViewModel
    {
        public int UserTypeId { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [LocalDisplayName("UserTypes.UserType", "FieldDisplayName")]
        public string UserType { get; set; }

        [DisplayName("Can update?")]
        public bool CanUpdate { get; set; }

        [DisplayName("Can delete?")]
        public bool CanDelete { get; set; }
    }
}