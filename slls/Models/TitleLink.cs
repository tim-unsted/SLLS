using System.ComponentModel;
using slls.Localization;

namespace slls.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TitleLinks")]
    public partial class TitleLink
    {
        public int TitleLinkID { get; set; }

        public int TitleID { get; set; }

        //[Required]
        [LocalDisplayName("TitleLinks.URL_Path", "FieldDisplayName")]
        public string URL { get; set; }

        public int FileId { get; set; }

        [StringLength(1000)]
        [LocalDisplayName("TitleLinks.Hover_Tip_Text", "FieldDisplayName")]
        public string HoverTip { get; set; }

        [StringLength(255)]
        [LocalDisplayName("TitleLinks.Display_Text", "FieldDisplayName")]
        public string DisplayText { get; set; }

        [StringLength(70)]
        public string Login { get; set; }

        [StringLength(20)]
        public string Password { get; set; }

        [LocalDisplayName("TitleLinks.Is_Valid", "FieldDisplayName")]
        public bool IsValid { get; set; }

        [StringLength(50)]
        [LocalDisplayName("TitleLinks.Link_Status", "FieldDisplayName")]
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

        public virtual Title Title { get; set; }

        public virtual HostedFile HostedFiles { get; set; }
    }
}
