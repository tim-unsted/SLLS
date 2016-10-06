using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using slls.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace slls.ViewModels
{
    public class ReportsGeneratorViewModel
    {
        public IEnumerable<SelectListItem> Reports { get; set; }
        public IEnumerable<SelectListItem> Classmarks { get; set; }
        public IEnumerable<SelectListItem> Locations { get; set; }
        public IEnumerable<SelectListItem> Offices { get; set; }
        public IEnumerable<SelectListItem> StatusTypes { get; set; }
        public IEnumerable<SelectListItem> MediaTypes { get; set; }
        public IEnumerable<SelectListItem> Publishers { get; set; }
        public IEnumerable<SelectListItem> Authors { get; set; }
        public IEnumerable<SelectListItem> Keywords { get; set; }
        public IEnumerable<SelectListItem> AccountYears { get; set; }
        public IEnumerable<SelectListItem> BudgetCodes { get; set; }
        public IEnumerable<SelectListItem> OrderCategories { get; set; }

        public IEnumerable<int> SelectedReport { get; set; }
        public IEnumerable<int> SelectedClassmark { get; set; }
        public IEnumerable<int> SelectedLocations { get; set; }
        public IEnumerable<int> SelectedOffice { get; set; }
        public IEnumerable<int> SelectedMediaType { get; set; }
        public IEnumerable<int> SelectedPublisher { get; set; }
        public IEnumerable<int> SelectedAuthor { get; set; }
        public IEnumerable<int> SelectedKeyword { get; set; }        
        public IEnumerable<int> SelectedStatusTypes { get; set; }

        public IEnumerable<int> SelectedBudgetCode { get; set; }
        public IEnumerable<int> SelectedAccountYear { get; set; }
        public bool sub { get; set; }
        public IEnumerable<int> SelectedOrderCategory { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public bool NotSelected { get; set; }
    }
}