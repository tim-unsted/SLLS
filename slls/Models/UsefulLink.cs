using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using slls.Localization;

namespace slls.Models
{
    [Table("UsefulLinks")]
    public class UsefulLink
    {
        [Key]
        public int LinkID { get; set; }

        [Column("FileID")]
        public int? FileId { get; set; }

        [LocalDisplayName("Useful_links.Link_Address", "FieldDisplayName")]
        [Column("LinkAddress")]
        public string Url { get; set; }

        [LocalDisplayName("Useful_links.Display_text", "FieldDisplayName")]
        public string DisplayText { get; set; }

        [LocalDisplayName("Useful_links.Hover_tip", "FieldDisplayName")]
        [Column("ToolTip")]
        public string HoverTip { get; set; }

        [LocalDisplayName("Useful_links.Target", "FieldDisplayName")]
        public string Target { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? InputDate { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? LastModified { get; set; }

        public bool Deleted { get; set; }

        public bool Enabled { get; set; }

        public virtual HostedFile HostedFiles { get; set; }
    }
}