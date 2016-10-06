using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using slls.Localization;

namespace slls.ViewModels
{
    public class BudgetCodeAddViewModel
    {
        [LocalDisplayName("BudgetCode.Budget_Code", "FieldDisplayName")]
        [Required(ErrorMessage = "A budget code is required")]
        [StringLength(255)]
        public string BudgetCode { get; set; }

        [Column(TypeName = "money")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        [LocalDisplayName("BudgetCode.Allocation_Subs", "FieldDisplayName")]
        public decimal AllocationSubs { get; set; }

        [Column(TypeName = "money")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        [LocalDisplayName("BudgetCode.Allocation_OneOffs", "FieldDisplayName")]
        public decimal AllocationOneOffs { get; set; }

        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; }
    }

    public class BudgetCodeEditViewModel
    {
        public int BudgetCodeID { get; set; }
        
        [LocalDisplayName("BudgetCode.Budget_Code", "FieldDisplayName")]
        [StringLength(255)]
        public string BudgetCode { get; set; }

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
    }
}