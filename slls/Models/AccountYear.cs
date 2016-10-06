using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using slls.Localization;

namespace slls.Models
{
    [Table("AccountYears")]
    public class AccountYear
    {
        public AccountYear()
        {
            //OrderDetails = new HashSet<OrderDetail>();
        }

        public int AccountYearID { get; set; }

        [Column("AccountYear")]
        [Required]
        [StringLength(255)]
        [LocalDisplayName("AccountYears.Account_Year", "FieldDisplayName")]
        public string AccountYear1 { get; set; }

        [Column(TypeName = "smalldatetime")]
        [Required]
        [LocalDisplayName("AccountYears.Year_Start_Date", "FieldDisplayName")]
        public DateTime? YearStartDate { get; set; }

        [Column(TypeName = "smalldatetime")]
        [Required]
        [LocalDisplayName("AccountYears.Year_End_Date", "FieldDisplayName")]
        public DateTime? YearEndDate { get; set; }

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