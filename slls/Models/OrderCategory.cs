using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using slls.Localization;

namespace slls.Models
{
    [Table("OrderCategories")]
    public partial class OrderCategory
    {
        public OrderCategory()
        {
            //OrderDetails = new HashSet<OrderDetail>();
        }
        
        public int OrderCategoryID { get; set; }

        [Column("OrderCategory")]
        [LocalDisplayName("OrderCategories.Order_Category", "FieldDisplayName")]
        [StringLength(255)]
        public string OrderCategory1 { get; set; }

        [LocalDisplayName("OrderCategories.Annual", "FieldDisplayName")]
        public bool Annual { get; set; }

        [LocalDisplayName("OrderCategories.Sub", "FieldDisplayName")]
        public bool Sub { get; set; }

        [DisplayName("Can update?")]
        public bool CanUpdate { get; set; }

        [DisplayName("Can delete?")]
        public bool CanDelete { get; set; }

        [DisplayName("Deleted?")]
        public bool Deleted { get; set; }

        public int ListPos { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? InputDate { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? LastModified { get; set; }

        [Column(TypeName = "timestamp")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [MaxLength(8)]
        public byte[] RowVersion { get; set; }

        [LocalDisplayName("OrderCategories.Orders", "FieldDisplayName")]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}