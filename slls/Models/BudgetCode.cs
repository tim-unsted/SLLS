using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using slls.Localization;

namespace slls.Models
{
    [Table("BudgetCodes")]
    public class BudgetCode
    {
        public BudgetCode()
        {
            //OrderDetails = new HashSet<OrderDetail>();
        }

        public int BudgetCodeID { get; set; }

        [Column("BudgetCode")]
        [LocalDisplayName("BudgetCode.Budget_Code", "FieldDisplayName")]
        [StringLength(255)]
        public string BudgetCode1 { get; set; }

        [Column(TypeName = "money")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        [LocalDisplayName("BudgetCode.Allocation_Subs", "FieldDisplayName")]
        public decimal AllocationSubs { get; set; }

        [Column(TypeName = "money")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        [LocalDisplayName("BudgetCode.Allocation_OneOffs", "FieldDisplayName")]
        public decimal AllocationOneOffs { get; set; }

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

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}