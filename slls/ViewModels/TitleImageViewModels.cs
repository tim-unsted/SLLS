using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using slls.Localization;

namespace slls.ViewModels
{
    public class TitleImageAddViewModel
    {
        public int TitleId { get; set; }

        public string Title { get; set; }

        public string Url { get; set; }

        [LocalDisplayName("Titles.ISBN_ISSN", "FieldDisplayName")]
        public string Isbn { get; set; }

        //[Required(ErrorMessage = "An image file is required")]
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

        public bool HasAutocat { get; set; }

        public bool IsPrimary { get; set; }

        public bool Success { get; set; }

        public string ErrorMessage { get; set; }

        public List<string> Sources { get; set; }
    }

    public class TitleImageEditViewModel
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
    }

    public class TitleImageListViewmodel
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