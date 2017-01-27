using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using slls.Localization;

namespace slls.Models
{
    [Table("OrderLinks")]
    public partial class OrderLink
    {
        public int OrderLinkID { get; set; }

        public int OrderID { get; set; }

        [LocalDisplayName("OrderLinks.URL_Path", "FieldDisplayName")]
        public string URL { get; set; }

        public int? FileId { get; set; }

        [StringLength(1000)]
        [LocalDisplayName("Links.Hover_Tip_Text", "FieldDisplayName")]
        public string HoverTip { get; set; }

        [StringLength(255)]
        [LocalDisplayName("Links.Display_Text", "FieldDisplayName")]
        public string DisplayText { get; set; }
        
        [LocalDisplayName("Links.Is_Valid", "FieldDisplayName")]
        public bool IsValid { get; set; }

        [StringLength(50)]
        [LocalDisplayName("Links.Link_Status", "FieldDisplayName")]
        public string LinkStatus { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? InputDate { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? LastModified { get; set; }

        [DisplayName("Deleted?")]
        public bool Deleted { get; set; }

        [Column(TypeName = "timestamp")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [MaxLength(8)]
        public byte[] RowVersion { get; set; }

        public virtual OrderDetail Order { get; set; }

        public virtual HostedFile HostedFiles { get; set; }
    }
}
