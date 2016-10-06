using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using slls.Localization;

namespace slls.Models
{
    [Table("LoanTypes")]
    public class LoanType
    {
        public LoanType()
        {
            //Volumes = new HashSet<Volume>();
            //MediaTypes = new HashSet<MediaType>();
        }

        
        [LocalDisplayName("LoanTypes.Loan_Type", "FieldDisplayName")]
        public int LoanTypeID { get; set; }

        [LocalDisplayName("LoanTypes.Loan_Type", "FieldDisplayName")]
        public string LoanTypeName { get; set; }

        [LocalDisplayName("LoanTypes.Ref_Only", "FieldDisplayName")]
        public bool RefOnly { get; set; }

        [LocalDisplayName("LoanTypes.Daily_Fine", "FieldDisplayName")]
        public decimal? DailyFine { get; set; }

        [LocalDisplayName("LoanTypes.Loan_Length", "FieldDisplayName")]
        public int LengthDays { get; set; }

        [LocalDisplayName("LoanTypes.Max_Items", "FieldDisplayName")]
        public int MaxItems { get; set; }

        public bool CanUpdate { get; set; }

        public bool CanDelete { get; set; }

        public int ListPos { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? InputDate { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? LastModified { get; set; }

        [LocalDisplayName("LoanTypes.CopyItems", "FieldDisplayName")]
        public virtual ICollection<Volume> Volumes { get; set; }

        [LocalDisplayName("LoanTypes.MediaTypes", "FieldDisplayName")]
        public virtual ICollection<MediaType> MediaTypes { get; set; }
    }
}