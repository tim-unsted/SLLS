using System.ComponentModel.DataAnnotations;
using slls.Localization;

namespace slls.ViewModels
{
    public class TitleLinksAddViewModel
    {
        [Key]
        public int TitleLinkId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int TitleId { get; set; }

        [LocalDisplayName("Titles.Title", "FieldDisplayName")]
        public string Title { get; set; }

        [Required]
        [LocalDisplayName("TitleLinks.URL_Path", "FieldDisplayName")]
        public string Url { get; set; }

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
    }

    public class TitleLinksEditViewModel
    {
        [Key]
        public int TitleLinkId { get; set; }

        public int TitleId { get; set; }

        [LocalDisplayName("Titles.Title", "FieldDisplayName")]
        public string Title { get; set; }

        [LocalDisplayName("TitleLinks.URL_Path", "FieldDisplayName")]
        public string Url { get; set; }

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
    }
}