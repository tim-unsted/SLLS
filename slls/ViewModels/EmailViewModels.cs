using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace slls.ViewModels
{
    public class NewEmailViewModel
    {
        public NewEmailViewModel()
        {
            ShowBcc = false;
            ShowCc = false;
            Important = false;
            ShowImportant = false;
            Title = "Ask a Question";
            InternalMsg = true;
            ShowCaptcha = true;
        }
        
        [Key]
        public int EmailID { get; set; }

        [Required]
        public string To { get; set; }

        [Required]
        public string From { get; set; }

        public string Cc { get; set; }

        public string Bcc { get; set; }

        [Required]
        public string Subject { get; set; }

        public bool Important { get; set; }

        [Required]
        [AllowHtml]
        [DataType(DataType.MultilineText)]
        public string Message { get; set; }

        public string RedirectController { get; set; }

        public string RedirectAction { get; set; }

        public string Title { get; set; }

        public bool ShowCc { get; set; }

        public bool ShowBcc { get; set; }

        public bool ShowImportant { get; set; }

        public bool InternalMsg { get; set; }

        public bool ShowCaptcha { get; set; }

    }
}