using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace slls.ViewModels
{
    public class AlphabeticalPagingViewModel
    {
        public List<string> ProductNames { get; set; }
        public List<string> FirstLetters { get; set; }
        public string SelectedLetter { get; set; }
    }
}