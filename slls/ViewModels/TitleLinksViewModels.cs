using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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

        //[Required]
        [RegularExpression(@"^(http(s)?://|ftp://|file://|[\w]\:|\\).+$", ErrorMessage = "This URL does not appear to be valid. Are you missing a prefix (e.g. 'http://')?")]
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

        ////[Required]
        ////[NotMapped]
        //public HttpPostedFileBase File { get; set; }

        //public IEnumerable<HttpPostedFileBase> Files { get; set; }

        ////[Required(ErrorMessage = "An image file is required")]
        //public string FileName
        //{
        //    get
        //    {
        //        if (File != null)
        //            return File.FileName;
        //        return string.Empty;
        //    }
        //}

        public bool Success { get; set; }
    }

    public class TitleLinksEditViewModel
    {
        [Key]
        public int TitleLinkId { get; set; }

        public int TitleId { get; set; }

        [LocalDisplayName("Titles.Title", "FieldDisplayName")]
        public string Title { get; set; }

        [RegularExpression(@"^(http(s)?://|ftp://|file://|[\w]\:|\\).+$", ErrorMessage = "This URL does not appear to be valid. Are you missing a prefix (e.g. 'http://')?")]
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

        public int? FileId { get; set; }

        public string FileName { get; set; }    

        [LocalDisplayName("TitleLinks.Is_Valid", "FieldDisplayName")]
        public bool IsValid { get; set; }

        [StringLength(50)]
        [LocalDisplayName("TitleLinks.Link_Status", "FieldDisplayName")]
        public string LinkStatus { get; set; }

        [AllowHtml]
        public string InfoMsg { get; set; }
    }
}