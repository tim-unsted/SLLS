using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using slls.Localization;

namespace slls.Models
{
    [Table("Suppliers")]
    public class Supplier
    {
        public Supplier()
        {
            //SupplierAddresses = new HashSet<SupplierAddress>();
            //OrderDetails = new HashSet<OrderDetail>();
        }

        public int SupplierID { get; set; }

        [StringLength(255)]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [LocalDisplayName("Suppliers.Supplier_Name", "FieldDisplayName")]
        public string SupplierName { get; set; }

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

        [LocalDisplayName("Suppliers.Notes", "FieldDisplayName")]
        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }

        public virtual ICollection<SupplierAddress> SupplierAddresses { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }

        [LocalDisplayName("SupplierAddress.Address", "FieldDisplayName")]
        public string AddressDetails
        {
            get
            {
                List<String> addressDetails =
                    SupplierAddresses
                        .Select(x => x.FullAddress)
                        .ToList();
                return string.Join("<br />", addressDetails);
            }
        }

        [LocalDisplayName("SupplierAddress.Account", "FieldDisplayName")]
        public string Accounts
        {
            get
            {
                List<String> accounts =
                    SupplierAddresses
                        .Select(x => x.Account)
                        .ToList();
                return string.Join("<br />", accounts);
            }
        }

        [LocalDisplayName("SupplierAddress.Main_Tel", "FieldDisplayName")]
        public string TelephoneNumbers
        {
            get
            {
                List<String> telephoneNumbers =
                    SupplierAddresses.OrderBy(x => x.Division)
                        .Select(x => x.MainTel)
                        .ToList();
                return string.Join("<br />", telephoneNumbers);
            }
        }

    }
}