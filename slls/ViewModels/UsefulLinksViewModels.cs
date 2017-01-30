using System.ComponentModel.DataAnnotations;
using slls.Localization;

namespace slls.ViewModels
{
    public class UsefulLinksAddEditViewModel
    {
        [Key]
        public int LinkID { get; set; }

        [Required]
        [RegularExpression(@"^(http(s)?://|ftp://|file://|[\w]\:|\\).+$", ErrorMessage = "This URL does not appear to be valid. Are you missing a prefix (e.g. 'http://')?")]
        [LocalDisplayName("Useful_links.Link_Address", "FieldDisplayName")]
        public string Url { get; set; }

        [LocalDisplayName("Useful_links.Display_text", "FieldDisplayName")]
        public string DisplayText { get; set; }

        [LocalDisplayName("Useful_links.Hover_tip", "FieldDisplayName")]
        public string HoverTip { get; set; }

        [LocalDisplayName("Useful_links.Target", "FieldDisplayName")]
        public string Target { get; set; }

        public bool Enabled { get; set; }

        public int? FileId { get; set; }

        public string FileName { get; set; }

        public string InfoMsg { get; set; }
    }
}