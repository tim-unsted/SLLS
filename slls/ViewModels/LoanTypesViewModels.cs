using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using slls.Localization;

namespace slls.ViewModels
{
    public class LoanTypesAddEditViewModel
    {
        [LocalDisplayName("LoanTypes.Loan_Type", "FieldDisplayName")]
        public int LoanTypeID { get; set; }

        [Required]
        [LocalDisplayName("LoanTypes.Loan_Type", "FieldDisplayName")]
        public string LoanTypeName { get; set; }

        [LocalDisplayName("LoanTypes.Ref_Only", "FieldDisplayName")]
        public bool RefOnly { get; set; }
        
        [LocalDisplayName("LoanTypes.Daily_Fine", "FieldDisplayName")]
        public decimal? DailyFine { get; set; }

        [Required]
        [LocalDisplayName("LoanTypes.Loan_Length", "FieldDisplayName")]
        public int LengthDays { get; set; }

        [Required]
        [LocalDisplayName("LoanTypes.Max_Items", "FieldDisplayName")]
        public int MaxItems { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? InputDate { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? LastModified { get; set; }
    }
}