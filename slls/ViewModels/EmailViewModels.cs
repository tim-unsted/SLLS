using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

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
        [DataType(DataType.MultilineText)]
        public string Message { get; set; }

        public string RedirectController { get; set; }

        public string RedirectAction { get; set; }

        public string Title { get; set; }

        public bool ShowCc { get; set; }

        public bool ShowBcc { get; set; }

        public bool ShowImportant { get; set; }

    }
}