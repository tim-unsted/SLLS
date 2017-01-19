using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using slls.Localization;
using slls.Models;

namespace slls.ViewModels
{
    public class SuppliersListViewModel
    {
        public IEnumerable<Supplier> Suppliers { get; set; }

        //Stuff to handle the alphabetical paging links on the index view ...
        public List<string> FirstLetters { get; set; }
        public string SelectedLetter { get; set; }
    }
    
    public class SuppliersAddViewModel
    {
        [StringLength(255)]
        [Required]
        [LocalDisplayName("Suppliers.Supplier_Name", "FieldDisplayName")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string SupplierName { get; set; }

        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }

        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; }
    }

    public class SuppliersEditViewModel
    {
        [Key]
        public int SupplierID { get; set; }

        [Required]
        [StringLength(255)]
        [LocalDisplayName("Suppliers.Supplier_Name", "FieldDisplayName")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string SupplierName { get; set; }

        [LocalDisplayName("Suppliers.Notes", "FieldDisplayName")]
        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }

        [DisplayName("Can update?")]
        public bool CanUpdate { get; set; }

        [DisplayName("Can delete?")]
        public bool CanDelete { get; set; }
    }
}