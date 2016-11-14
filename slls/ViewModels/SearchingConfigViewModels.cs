using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using slls.Localization;

namespace slls.ViewModels
{
    public class SearchOrderTypeViewModel
    {
        public int OrderTypeID { get; set; }

        [Required]
        [StringLength(255)]
        public string OrderType { get; set; }

        [StringLength(255)]
        [LocalDisplayName("Searching.Order_By", "FieldDisplayName")]
        public string OrderTypeFriendly { get; set; }

        [StringLength(10)]
        public string Scope { get; set; }

        [DisplayName("Display Position")]
        public int Display { get; set; }
    }
}