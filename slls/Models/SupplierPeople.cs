using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.RegularExpressions;
using slls.Localization;

namespace slls.Models
{
    [Table("SupplierPeople")]
    public class SupplierPeople
    {
        private readonly DbEntities _db = new DbEntities();
        
        public SupplierPeople()
        {
            //SupplierPeopleComms = new HashSet<SupplierPeopleComm>();
        }

        [Key]
        public int ContactID { get; set; }

        [LocalDisplayName("SupplierAddress.Address", "FieldDisplayName")]
        [Required(ErrorMessage = "Please select an {0}")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select an {0}")]
        public int AddressID { get; set; }

        [StringLength(50)]
        [LocalDisplayName("SupplierPeople.Title", "FieldDisplayName")]
        public string Title { get; set; }

        [StringLength(50)]
        [LocalDisplayName("SupplierPeople.Initials", "FieldDisplayName")]
        public string Initials { get; set; }

        [StringLength(50)]
        [LocalDisplayName("SupplierPeople.Lastnames", "FieldDisplayName")]
        public string Surname { get; set; }

        [StringLength(50)]
        [LocalDisplayName("SupplierPeople.Firstnames", "FieldDisplayName")]
        public string Firstname { get; set; }

        [StringLength(50)]
        [LocalDisplayName("SupplierPeople.Position", "FieldDisplayName")]
        public string Position { get; set; }

        [StringLength(255)]
        [LocalDisplayName("SupplierPeople.Email", "FieldDisplayName")]
        public string Email { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? InputDate { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? LastModified { get; set; }

        [DisplayName("Deleted?")]
        public bool Deleted { get; set; }

        [LocalDisplayName("SupplierPeople.Fullname", "FieldDisplayName")]
        public string Fullname
        {
            get
            {
                return string.Format("{0} {1}", Firstname, Surname);
            }
        }

        [LocalDisplayName("SupplierPeople.Fullname", "FieldDisplayName")]
        public string FullnameRev
        {
            get
            {
                if ((string.IsNullOrEmpty(Surname)) || (string.IsNullOrEmpty(Firstname)))
                {
                    return string.Format("{0}{1}", Surname, Firstname);
                }
                return string.Format("{0}, {1}", Surname, Firstname);
            }
        }
        
        [LocalDisplayName("CommunicationTypes.Methods", "FieldDisplayName")]
        public string CommTypeDetails
        {
            get
            {
                List<String> commTypeDetails =
                    SupplierPeopleComms.OrderBy(c => c.CommMethodType.Method)
                        .Select(c => c.CommMethodType.Method + ": " + c.Detail)
                        .ToList();
                return string.Join("<br />", commTypeDetails);
            }
        }

        public int SupplierID
        {
            get
            {
               return _db.SupplierAddresses.Find(AddressID).SupplierID.Value;
            }
        }

        public string SupplierName
        {
            get
            {
                return _db.Suppliers.Find(SupplierID).SupplierName;
            }
        }

        [Column(TypeName = "timestamp")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [MaxLength(8)]
        public byte[] RowVersion { get; set; }

        public virtual ICollection<SupplierPeopleComm> SupplierPeopleComms { get; set; }
    }
}