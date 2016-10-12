using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using slls.Localization;
using slls.Models;

namespace slls.ViewModels
{
    public class NotificationsViewModel
    {
        [Key]
        public int NotificationID { get; set; }

        [LocalDisplayName("Notifications.Headline", "FieldDisplayName")]
        public string Headline { get; set; }

        [LocalDisplayName("Notifications.Scope", "FieldDisplayName")]
        public string Scope { get; set; }

        [LocalDisplayName("Notifications.Text", "FieldDisplayName")]
        [DataType(DataType.MultilineText)]
        public string Text { get; set; }

        [LocalDisplayName("Notifications.Expiry_Date", "FieldDisplayName")]
        [Column(TypeName = "smalldatetime")]
        public DateTime? ExpireDate { get; set; }

        [DisplayName("Position")]
        public int Position { get; set; }

        [DisplayName("Visible?")]
        public bool Visible { get; set; }
    }

    public class NotificationIndexViewModel
    {
        public IEnumerable<Notification> Notifications { get; set; }
        public string Scope { get; set; }
    }
}