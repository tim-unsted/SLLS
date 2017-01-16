using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace slls.ViewModels
{
    public class IpAddressAddEditViewModel
    {
        public IpAddressAddEditViewModel()
        {
            Blocked = false;
            AllowPassThrough = true;
            CanDelete = true;
            CanUpdate = true;
        }
        
        [Key]
        [Column("RecID")]
        public int RecId { get; set; }

        [Column("IPAddress")]
        [DisplayName("IP Address")]
        public string IpAddress1 { get; set; }

        [DisplayName("Blocked?")]
        public bool Blocked { get; set; }

        [DisplayName("Allowed?")]
        public bool AllowPassThrough { get; set; }

        [DisplayName("Can Update?")]
        public bool CanUpdate { get; set; }

        [DisplayName("Can Delete?")]
        public bool CanDelete { get; set; }

        [DisplayName("Date Added")]
        public DateTime InputDate { get; set; } 
    }
}