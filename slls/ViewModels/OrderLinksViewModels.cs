using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using slls.Localization;

namespace slls.ViewModels
{
    public class OrderLinksAddViewModel
    {
        [Key]
        public int OrderLinkId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int OrderId { get; set; }

        [LocalDisplayName("Orders.Order", "FieldDisplayName")]
        public string OrderDetail { get; set; }
        
        //[Required]
        [RegularExpression(@"^(http(s)?://|ftp://|file://|[\w]\:|\\).+$", ErrorMessage = "This URL does not appear to be valid. Are you missing a prefix (e.g. 'http://')?")]
        [LocalDisplayName("OrderLinks.URL_Path", "FieldDisplayName")]
        public string Url { get; set; }

        [StringLength(1000)]
        [LocalDisplayName("OrderLinks.Hover_Tip_Text", "FieldDisplayName")]
        public string HoverTip { get; set; }

        [StringLength(255)]
        [LocalDisplayName("OrderLinks.Display_Text", "FieldDisplayName")]
        public string DisplayText { get; set; }
        
        public bool Success { get; set; }
    }

    public class OrderLinksEditViewModel
    {
        [Key]
        public int OrderLinkId { get; set; }

        public int OrderId { get; set; }

        [LocalDisplayName("Orders.Order", "FieldDisplayName")]
        public string OrderDetail { get; set; }
        
        [RegularExpression(@"^(http(s)?://|ftp://|file://|[\w]\:|\\).+$", ErrorMessage = "This URL does not appear to be valid. Are you missing a prefix (e.g. 'http://')?")]
        [LocalDisplayName("Links.URL_Path", "FieldDisplayName")]
        public string Url { get; set; }

        [StringLength(1000)]
        [LocalDisplayName("Links.Hover_Tip_Text", "FieldDisplayName")]
        public string HoverTip { get; set; }

        [StringLength(255)]
        [LocalDisplayName("Links.Display_Text", "FieldDisplayName")]
        public string DisplayText { get; set; }
        
        public int? FileId { get; set; }

        public string FileName { get; set; }    

        [LocalDisplayName("Links.Is_Valid", "FieldDisplayName")]
        public bool IsValid { get; set; }

        [StringLength(50)]
        [LocalDisplayName("Links.Link_Status", "FieldDisplayName")]
        public string LinkStatus { get; set; }

        [AllowHtml]
        public string InfoMsg { get; set; }
    }
}