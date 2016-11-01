using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace slls.ViewModels
{
    public class MergeAuthorityListViewModel
    {
        //public IEnumerable<SelectListItem> AuthorityLists { get; set; }

        [DisplayName("Authority List")]
        [Required(ErrorMessage = "Please select the Authority List you wish to edit.")]
        public string AuthorityList { get; set; }

        
        public IEnumerable<SelectListItem> AvailableItems { get; set; }

        [DisplayName("Current values to move/merge")]
        public List<int> SelectedIds { get; set; }

        [DisplayName("Current value to accept move/merger")]
        public int NewId { get; set; }

        [DisplayName("New value to accept move/merger")]
        public string NewValue { get; set; }
    }
}