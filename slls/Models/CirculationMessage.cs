using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using slls.Localization;

namespace slls.Models
{
    [Table("CirculationMessages")]
    public class CirculationMessage
    {
        public CirculationMessage()
        {
            //Copies = new HashSet<Copy>();
        }

        [Key]
        public int CirculationMsgID { get; set; }

        [LocalDisplayName("Circulation.Circulation_Slip_Message", "FieldDisplayName")]
        [DataType(DataType.MultilineText)]
        public string CirculationMsg { get; set; }

        [DisplayName("Can update?")]
        public bool CanUpdate { get; set; }

        [DisplayName("Can delete?")]
        public bool CanDelete { get; set; }

        [DisplayName("Deleted?")]
        public bool Deleted { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? InputDate { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? LastModified { get; set; }

        public int ListPos { get; set; }

        public virtual ICollection<Copy> Copies { get; set; }
    }
}