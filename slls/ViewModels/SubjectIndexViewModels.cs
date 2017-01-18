using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace slls.ViewModels
{
    public class SubjectIndexAddViewModel
    {
        public IEnumerable<int> SelectedKeywords { get; set; }

        public IEnumerable<SelectListItem> AvailableKeywords { get; set; }

        public bool LargeData { get; set; }
        
        public int SubjectIndexId { get; set; }

        public int TitleId { get; set; }

        public int KeywordId { get; set; }

        public string Keyword { get; set; }

        public string  Title { get; set; } 
        
     
    }
}