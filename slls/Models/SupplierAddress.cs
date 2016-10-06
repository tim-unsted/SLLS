using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web.Security.AntiXss;
using slls.Localization;

namespace slls.Models
{
    [Table("SupplierAddresses")]
    public class SupplierAddress
    {
        [Key]
        public int AddressID { get; set; }

        public int? SupplierID { get; set; }

        [StringLength(255)]
        [LocalDisplayName("SupplierAddress.Division", "FieldDisplayName")]
        public string Division { get; set; }

        [StringLength(255)]
        [LocalDisplayName("SupplierAddress.Address_Line_1", "FieldDisplayName")]
        public string Address1 { get; set; }

        [StringLength(255)]
        [LocalDisplayName("SupplierAddress.Address_Line_2", "FieldDisplayName")]
        public string Address2 { get; set; }

        [Column("Town/City")]
        [LocalDisplayName("SupplierAddress.Town_City", "FieldDisplayName")]
        [StringLength(255)]
        public string Town_City { get; set; }

        [StringLength(255)]
        [LocalDisplayName("SupplierAddress.County", "FieldDisplayName")]
        public string County { get; set; }

        [StringLength(50)]
        [LocalDisplayName("SupplierAddress.Postcode", "FieldDisplayName")]
        public string Postcode { get; set; }

        [StringLength(255)]
        [LocalDisplayName("SupplierAddress.Country", "FieldDisplayName")]
        public string Country { get; set; }

        [StringLength(50)]
        [LocalDisplayName("SupplierAddress.DX", "FieldDisplayName")]
        public string DX { get; set; }

        [StringLength(255)]
        [LocalDisplayName("SupplierAddress.Main_Tel", "FieldDisplayName")]
        public string MainTel { get; set; }

        [StringLength(255)]
        [LocalDisplayName("SupplierAddress.Main_Fax", "FieldDisplayName")]
        public string MainFax { get; set; }

        [StringLength(255)]
        [LocalDisplayName("SupplierAddress.Account", "FieldDisplayName")]
        public string Account { get; set; }

        [LocalDisplayName("SupplierAddress.Activity_Code", "FieldDisplayName")]
        public int? ActivityCode { get; set; }

        [StringLength(1000)]
        [LocalDisplayName("SupplierAddress.Website", "FieldDisplayName")]
        public string URL { get; set; }

        [StringLength(255)]
        [LocalDisplayName("SupplierAddress.Web_Password", "FieldDisplayName")]
        public string WebPassword { get; set; }

        [LocalDisplayName("SupplierAddress.Notes", "FieldDisplayName")]
        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }

        [StringLength(255)]
        [LocalDisplayName("SupplierAddress.Email", "FieldDisplayName")]
        public string Email { get; set; }

        [StringLength(255)]
        [LocalDisplayName("SupplierAddress.OtherPhone", "FieldDisplayName")]
        public string Phone1 { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? InputDate { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? LastModified { get; set; }

        [DisplayName("Deleted?")]
        public bool Deleted { get; set; }

        [Column(TypeName = "timestamp")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [MaxLength(8)]
        public byte[] RowVersion { get; set; }
        
        public virtual Supplier Supplier { get; set; }

        [LocalDisplayName("SupplierAddress.Address", "FieldDisplayName")]
        public string FullAddress
        {
            get
            {
                string[] addressParts = {Address1, Address2, Town_City, County, Country, Postcode};
                return string.Join(", ", addressParts.Where(x => !string.IsNullOrEmpty(x)));

            }
        }
    }
}