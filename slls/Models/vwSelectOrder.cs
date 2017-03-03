using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using slls.Utils.Helpers;

namespace slls.Models
{
    [Table("vwSelectOrder")]
    public class vwSelectOrder
    {
        [Column("OrderID")]
        [Key]
        public int OrderId { get; set; }

        public string OrderNo { get; set; }

        public string Title { get; set; }

        public string SupplierName { get; set; }

        public DateTime OrderDate { get; set; }

        public DateTime? ReceivedDate { get; set; }

        public DateTime? InvoiceDate { get; set; }

        public string InvoiceRef { get; set; }

        public int NonFilingChars { get; set; }

        [NotMapped]
        public string SelectOrder
        {
            get
            {
                var orderNumber = OrderNo ?? OrderId.ToString();
                var title = StringHelper.Truncate(Title, 45);
                var supplier = SupplierName == null ? "".PadRight(35, '\u00A0') : StringHelper.Truncate(SupplierName, 30);
                var dateOrdered = string.Format("{0:dd-MMM-yyyy}", OrderDate);

                title = title.PadRight(50, '\u00A0');
                supplier = supplier.PadRight(40, '\u00A0');
                orderNumber = orderNumber.PadRight(20, '\u00A0');

                return string.Format("{0} {1} {2} {3}", title, supplier, orderNumber, dateOrdered);
            }
        }

    }
}