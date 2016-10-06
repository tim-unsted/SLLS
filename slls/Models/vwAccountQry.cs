using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace slls.Models
{
    [Table("dbo.vwAccountQry")]
    public partial class vwAccountQry
    {
        [Key]
        public int OrderID { get; set; }
        public int AccountYearID { get; set; }
        public string AccountYear { get; set; }
        public string OrderCategory { get; set; }
        public string Media { get; set; }
        public string Title { get; set; }
        public double SumOfPrice { get; set; }
        public string BudgetCode { get; set; }
        public double AllocationSubs { get; set; }
        public double AllocationOneOffs { get; set; }
        public double AllocationTotal { get; set; }
    }
}