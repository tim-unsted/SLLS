using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using slls.Localization;
using slls.Models;

namespace slls.ViewModels
{
    
    public class SupplierPeopleListViewModel
    {
        [Key]
        public int ContactID { get; set; }

        public int AddressID { get; set; }

        public IEnumerable<SupplierPeople> Contacts { get; set; }

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
        
        [DisplayName("Deleted?")]
        public bool Deleted { get; set; }

        //Stuff to handle the alphabetical paging links on the index view ...
        public List<string> FirstLetters { get; set; }
        public string SelectedLetter { get; set; }

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

        public int SupplierID { get; set; }
        
        [LocalDisplayName("Suppliers.Supplier_Name", "FieldDisplayName")]
        public string SupplierName { get; set; }

        public virtual ICollection<SupplierPeopleComm> SupplierPeopleComms { get; set; }
    }
    
    public class SupplierPeopleAddViewModel
    {
        [Key]
        public int ContactID { get; set; }

        [LocalDisplayName("Suppliers.Supplier", "FieldDisplayName")]
        [Required(ErrorMessage = "Please select a {0}")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a {0}")]
        public int SupplierID { get; set; }

        [LocalDisplayName("SupplierAddress.Address", "FieldDisplayName")]
        [Required(ErrorMessage = "Please select an {0}")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select an {0}")]
        public int AddressID { get; set; }

        public string AddressDescription { get; set; }

        public string SupplierName { get; set; }

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

        [StringLength(255)]
        [LocalDisplayName("SupplierPeople.Phone", "FieldDisplayName")]
        public string Phone { get; set; }

        public string CallingController { get; set; }

        public string CallingAction { get; set; }
    }

    public class SupplierPeopleEditViewModel
    {
        [Key]
        public int ContactID { get; set; }

        public int SupplierID { get; set; }

        [Required]
        [LocalDisplayName("SupplierAddress.Address", "FieldDisplayName")]
        public int AddressID { get; set; }

        public string AddressDescription { get; set; }

        [LocalDisplayName("Suppliers.Supplier_Name", "FieldDisplayName")]
        public string SupplierName { get; set; }

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

        public string CallingController { get; set; }

        public string CallingAction { get; set; }
    }

    public class SupplierPeopleCommsViewModel
    {
        [Key]
        public int CommID { get; set; }

        public int ContactID { get; set; }

        [Required]
        [Range(1, 1000)]
        [LocalDisplayName("CommunicationTypes.Method", "FieldDisplayName")]
        public int MethodID { get; set; }

        [Required]
        [LocalDisplayName("CommunicationTypes.Detail", "FieldDisplayName")]
        public string Detail { get; set; }

        [LocalDisplayName("SupplierPeople.Contact", "FieldDisplayName")]
        public string ContactName { get; set; } 
    }
}