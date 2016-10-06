using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using slls.Localization;

namespace slls.Models
{
    [Table("Notifications")]
    public class Notification
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
        
        [Column(TypeName = "smalldatetime")]
        public DateTime? InputDate { get; set; }

        [LocalDisplayName("Notifications.Expiry_Date", "FieldDisplayName")]
        [Column(TypeName = "smalldatetime")]
        public DateTime? ExpireDate { get; set; }

        [LocalDisplayName("Notifications.Input_By", "FieldDisplayName")]
        public string InputUser { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? LastModified { get; set; }
 
    }
}