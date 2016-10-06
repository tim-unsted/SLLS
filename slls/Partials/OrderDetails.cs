using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace slls.Partials
{
    public partial class OrderDetail
    {
        public DateTime? OrderDate { get; set; }
        
        public string OrderDateSortable
        {
            get { return string.Format("{0:yyyy-MM-dd}", OrderDate); }
        }
    }
}