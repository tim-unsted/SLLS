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
    public class AccountYearsAddViewModel
    {
        [StringLength(255)]
        [Required]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [LocalDisplayName("AccountYears.Account_Year", "FieldDisplayName")]
        public string AccountYear { get; set; }

        [Required]
        [Column(TypeName = "smalldatetime")]
        [LocalDisplayName("AccountYears.Year_Start_Date", "FieldDisplayName")]
        //[RegularExpression(@"^[0-9]{4}/[0-9]{2}/[0-9]{2} ((20|21|22|23)|([0-1][0-9])):[0-5][0-9]:[0-5][0-9]$", ErrorMessage = "The {0} must be a valid date")]
        [RegularExpression(@"^(?:(?:31(\/|-|\.)(?:0?[13578]|1[02]|(?:Jan|Mar|May|Jul|Aug|Oct|Dec)))\1|(?:(?:29|30)(\/|-|\.)(?:0?[1,3-9]|1[0-2]|(?:Jan|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec))\2))(?:(?:1[6-9]|[2-9]\d)?\d{2})$|^(?:29(\/|-|\.)(?:0?2|(?:Feb))\3(?:(?:(?:1[6-9]|[2-9]\d)?(?:0[48]|[2468][048]|[13579][26])|(?:(?:16|[2468][048]|[3579][26])00))))$|^(?:0?[1-9]|1\d|2[0-8])(\/|-|\.)(?:(?:0?[1-9]|(?:Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep))|(?:1[0-2]|(?:Oct|Nov|Dec)))\4(?:(?:1[6-9]|[2-9]\d)?\d{2})$", ErrorMessage = "The {0} must be a valid date")]
        public DateTime? YearStartDate { get; set; }

        [Required]
        [Column(TypeName = "smalldatetime")]
        [LocalDisplayName("AccountYears.Year_End_Date", "FieldDisplayName")]
        //[RegularExpression(@"^[0-9]{4}/[0-9]{2}/[0-9]{2} ((20|21|22|23)|([0-1][0-9])):[0-5][0-9]:[0-5][0-9]$", ErrorMessage = "The {0} must be a valid date")]
        [RegularExpression(@"^(?:(?:31(\/|-|\.)(?:0?[13578]|1[02]|(?:Jan|Mar|May|Jul|Aug|Oct|Dec)))\1|(?:(?:29|30)(\/|-|\.)(?:0?[1,3-9]|1[0-2]|(?:Jan|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec))\2))(?:(?:1[6-9]|[2-9]\d)?\d{2})$|^(?:29(\/|-|\.)(?:0?2|(?:Feb))\3(?:(?:(?:1[6-9]|[2-9]\d)?(?:0[48]|[2468][048]|[13579][26])|(?:(?:16|[2468][048]|[3579][26])00))))$|^(?:0?[1-9]|1\d|2[0-8])(\/|-|\.)(?:(?:0?[1-9]|(?:Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep))|(?:1[0-2]|(?:Oct|Nov|Dec)))\4(?:(?:1[6-9]|[2-9]\d)?\d{2})$", ErrorMessage = "The {0} must be a valid date")]
        public DateTime? YearEndDate { get; set; }

        [DisplayName("Can update?")]
        public bool CanUpdate { get; set; }

        [DisplayName("Can delete?")]
        public bool CanDelete { get; set; }
    }

    public class AccountYearsEditViewModel
    {
        public int AccountYearID { get; set; }
        
        [StringLength(255)]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [LocalDisplayName("AccountYears.Account_Year", "FieldDisplayName")]
        [Required]
        public string AccountYear { get; set; }

        [Required]
        [Column(TypeName = "smalldatetime")]
        [LocalDisplayName("AccountYears.Year_Start_Date", "FieldDisplayName")]
        [RegularExpression(@"^(?:(?:31(\/|-|\.)(?:0?[13578]|1[02]|(?:Jan|Mar|May|Jul|Aug|Oct|Dec)))\1|(?:(?:29|30)(\/|-|\.)(?:0?[1,3-9]|1[0-2]|(?:Jan|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec))\2))(?:(?:1[6-9]|[2-9]\d)?\d{2})$|^(?:29(\/|-|\.)(?:0?2|(?:Feb))\3(?:(?:(?:1[6-9]|[2-9]\d)?(?:0[48]|[2468][048]|[13579][26])|(?:(?:16|[2468][048]|[3579][26])00))))$|^(?:0?[1-9]|1\d|2[0-8])(\/|-|\.)(?:(?:0?[1-9]|(?:Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep))|(?:1[0-2]|(?:Oct|Nov|Dec)))\4(?:(?:1[6-9]|[2-9]\d)?\d{2})$", ErrorMessage = "The {0} must be a valid date")]
        public DateTime? YearStartDate { get; set; }

        [Required]
        [Column(TypeName = "smalldatetime")]
        [LocalDisplayName("AccountYears.Year_End_Date", "FieldDisplayName")]
        [RegularExpression(@"^(?:(?:31(\/|-|\.)(?:0?[13578]|1[02]|(?:Jan|Mar|May|Jul|Aug|Oct|Dec)))\1|(?:(?:29|30)(\/|-|\.)(?:0?[1,3-9]|1[0-2]|(?:Jan|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec))\2))(?:(?:1[6-9]|[2-9]\d)?\d{2})$|^(?:29(\/|-|\.)(?:0?2|(?:Feb))\3(?:(?:(?:1[6-9]|[2-9]\d)?(?:0[48]|[2468][048]|[13579][26])|(?:(?:16|[2468][048]|[3579][26])00))))$|^(?:0?[1-9]|1\d|2[0-8])(\/|-|\.)(?:(?:0?[1-9]|(?:Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep))|(?:1[0-2]|(?:Oct|Nov|Dec)))\4(?:(?:1[6-9]|[2-9]\d)?\d{2})$", ErrorMessage = "The {0} must be a valid date")]
        public DateTime? YearEndDate { get; set; }

        [DisplayName("Can update?")]
        public bool CanUpdate { get; set; }

        [DisplayName("Can delete?")]
        public bool CanDelete { get; set; }
    }
}