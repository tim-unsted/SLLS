using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using slls.Localization;
using slls.Models;

namespace slls.ViewModels
{
    public class LinkedFileAddViewModel
    {
        public LinkedFileAddViewModel()
        {
            ExistingFile = 1;
            PostAction = "PostCreate";
            PostController = "TitleLinks";
            AlertText = "Use the form to add a new link to the current item.";
        }
        
        public int TitleId { get; set; }
        public int OrderId { get; set; }

        [LocalDisplayName("Titles.Title", "FieldDisplayName")]
        public string Title { get; set; }

        [LocalDisplayName("Orders.Order", "FieldDisplayName")]
        public string OrderDetails { get; set; }

        //[Required]
        [RegularExpression(@"^(http(s)?://|ftp://|file://|[\w]\:|\\).+$", ErrorMessage = "This URL does not appear to be valid. Are you missing a prefix (e.g. 'http://')?")]
        [LocalDisplayName("Links.URL_Path", "FieldDisplayName")]
        public string Url { get; set; }

        [StringLength(1000)]
        [LocalDisplayName("Links.Hover_Tip_Text", "FieldDisplayName")]
        public string HoverTip { get; set; }

        [StringLength(255)]
        [LocalDisplayName("Links.Display_Text", "FieldDisplayName")]
        public string DisplayText { get; set; }

        [StringLength(70)]
        public string Login { get; set; }

        [StringLength(20)]
        public string Password { get; set; }

        [LocalDisplayName("Titles.ISBN_ISSN", "FieldDisplayName")]
        public string Isbn { get; set; }

        public HttpPostedFileBase File { get; set; }

        public IEnumerable<HttpPostedFileBase> Files { get; set; }

        //[Required(ErrorMessage = "An image file is required")]
        public string FileName
        {
            get
            {
                if (File != null)
                    return File.FileName;
                return string.Empty;
            }
        }

        public int ExistingFile { get; set; }

        public int ExistingImage { get; set; }

        public IEnumerable<SelectListItem> ExistingFiles { get; set; }

        public IEnumerable<SelectListItem> ExistingImages { get; set; }

        public bool HasAutocat { get; set; }

        public bool IsPrimary { get; set; }

        public bool Success { get; set; }

        public string ErrorMessage { get; set; }

        public List<string> Sources { get; set; }

        public string SupplierName { get; set; }

        public string PostController { get; set; }

        public string PostAction { get; set; }

        [AllowHtml]
        public string AlertText { get; set; }
    }

    public class LinkedFileEditViewModel
    {
        [Key]
        public int TitleImageId { get; set; }

        public int ImageId { get; set; }

        public int TitleId { get; set; }

        public string Title { get; set; }

        [LocalDisplayName("Images.Alt", "FieldDisplayName")]
        public string Alt { get; set; }

        [LocalDisplayName("Images.HoverText", "FieldDisplayName")]
        [DataType(DataType.MultilineText)]
        public string HoverText { get; set; }

        [LocalDisplayName("Images.IsPrimary", "FieldDisplayName")]
        public bool IsPrimary { get; set; }

        public bool Success { get; set; }
        
        //[Key]
        //public int TitleImageId { get; set; }
        //public int ImageId { get; set; }

        //public int TitleLinkId { get; set; }
        //public int TitleId { get; set; }

        //public int OrderLinkId { get; set; }
        //public int OrderId { get; set; }

        //[LocalDisplayName("Titles.Title", "FieldDisplayName")]
        //public string Title { get; set; }

        //[LocalDisplayName("Orders.Order", "FieldDisplayName")]
        //public string OrderDetails { get; set; }

        ////[Required]
        //[RegularExpression(@"^(http(s)?://|ftp://|file://|[\w]\:|\\).+$", ErrorMessage = "This URL does not appear to be valid. Are you missing a prefix (e.g. 'http://')?")]
        //[LocalDisplayName("Links.URL_Path", "FieldDisplayName")]
        //public string Url { get; set; }

        //[LocalDisplayName("Links.Hover_Tip_Text", "FieldDisplayName")]
        //public string HoverTip { get; set; }

        //[LocalDisplayName("Links.Hover_Tip_Text", "FieldDisplayName")]
        //public string HoverText { get; set; }

        //[StringLength(255)]
        //[LocalDisplayName("Links.Display_Text", "FieldDisplayName")]
        //public string DisplayText { get; set; }

        //public string Alt { get; set; }

        //[StringLength(70)]
        //public string Login { get; set; }

        //[StringLength(20)]
        //public string Password { get; set; }

        //[LocalDisplayName("Titles.ISBN_ISSN", "FieldDisplayName")]
        //public string Isbn { get; set; }

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

        //public int ExistingFile { get; set; }

        //public IEnumerable<SelectListItem> ExistingFiles { get; set; }

        //public bool HasAutocat { get; set; }

        //public bool IsPrimary { get; set; }

        //public bool Success { get; set; }

        //public string ErrorMessage { get; set; }

        //public List<string> Sources { get; set; }

        //public string SupplierName { get; set; }

        //public string PostController { get; set; }

        //public string PostAction { get; set; }

        //[AllowHtml]
        //public string AlertText { get; set; }
    }

    public class LinkedFileListViewmodel
    {
        public int TitleImageId { get; set; }
        
        public int TitleId { get; set; }

        public int ImageId { get; set; }

        public string ImageToShow { get; set; }

        public Byte[] ImageBytes { get; set; }

        public string Alt { get; set; }

        public string HoverText { get; set; }

        public bool IsPrimary { get; set; }
    }

    public class DownloadImagesViewmodel
    {
        public int CountAvailable { get; set; }
        public int CountNoImage { get; set; }
        public int CountNoIsbn { get; set; }
        public int CountWithIsbn { get; set; }
        public string Who { get; set; }
        //public string Sources { get; set; }
        public IEnumerable<SelectListItem> Sources { get; set; }

        [Required(ErrorMessage = "Please choose at least one source!")]
        public IEnumerable<string> SelectedSources { get; set; }
    }

}