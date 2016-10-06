using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using slls.Localization;

namespace slls.ViewModels
{
    public class DepartmentsAddViewModel
    {
        [LocalDisplayName("Departments.Department", "FieldDisplayName")]
        [StringLength(255)]
        public string Department { get; set; }

        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; }
    }

    public class DepartmentsEditViewModel
    {
        public int DepartmentID { get; set; }

        [LocalDisplayName("Departments.Department", "FieldDisplayName")]
        [StringLength(255)]
        public string Department { get; set; }

        [DisplayName("Can update?")]
        public bool CanUpdate { get; set; }

        [DisplayName("Can delete?")]
        public bool CanDelete { get; set; }
    }
}