using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using slls.Models;

namespace slls.ViewModels
{
    public class DataImportViewModel
    {
        public DataImportViewModel()
        {
            Title = "Select File ...";
            ButtonText = "Open";
            Glyphicon = "glyphicon glyphicon-folder-open";
            Tip = "Browse for and select the file you require, then click the 'Open' button.";
            AcceptedFileTypes = ".txt, .TXT, .xml, .XML, .csv, .CSV, .xls, .XLS, .xlsx, .XLSX";
        }
        
        [Required(ErrorMessage = "Please select a file.")]
        public HttpPostedFileBase File { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public int Rows { get; set; }
        public string AcceptedFileTypes { get; set; }
        public string Title { get; set; }
        public string Tip { get; set; }
        public string ButtonText { get; set; }
        public string Glyphicon { get; set; }   
    }

    public class UserImportViewModel
    {
        public List<ApplicationUser> Users { get; set; }
        public int Count { get; set; }
    }
}