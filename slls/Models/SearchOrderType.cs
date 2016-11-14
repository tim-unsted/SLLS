using System.ComponentModel;
using slls.Localization;

namespace slls.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SearchOrderTypes")]
    public partial class SearchOrderType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
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