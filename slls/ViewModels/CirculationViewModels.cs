using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using slls.Models;

namespace slls.ViewModels
{
    public class CirculationListViewModel
    {
        [Key]
        public int CirculationId { get; set; }
        public IEnumerable<Circulation> CirculationList { get; set; }
        public IEnumerable<ApplicationUser> Recipients { get; set; }
        public string SelectCopy { get; set; }
    }

    public class CirculationListSummaryViewModel
    {
        [Key]
        public int CirculationId { get; set; }
        public string Fullname { get; set; }
    }

    public class CirculationByRecipientViewmodel
    {
        [Key]
        public int CirculationId { get; set; }
        public int CopyId { get; set; }
        public int UserId { get; set; }
        public int SortOrder { get; set; }
        public string Fullname { get; set; }
        public IEnumerable<Circulation> UserCirculationList { get; set; }
    }

    public class CirculationSlipViewModel
    {
        [Key]
        public int CopyId { get; set; }
        public string Title { get; set; }
        public int CopyNumber { get; set; }
        public DateTime DateReceived { get; set; }
        public IEnumerable<PartsReceived> PartsReceived { get; set; }
        public bool HasData { get; set; }
        public string NoDataTitle { get; set; }
        public string NoDataMsg { get; set; }
        public string NoDataOk { get; set; }

        public CirculationSlipViewModel()
        {
            NoDataTitle = "No Data!";
            NoDataMsg = "There are no pending cirulation slips ready to print.";
        }
    }

    public class CirculationSlipRecipientListViewModel
    {
        [Key]
        public int CopyId { get; set; }
        public string CirculateTo { get; set; }
        public string Signed { get; set; }
        public string DatePassed { get; set; }
        public IEnumerable<Circulation> CirculationList { get; set; }
        public bool HasData { get; set; }
        public string NoDataTitle { get; set; }
        public string NoDataMsg { get; set; }
        public string NoDataOk { get; set; }
    }

    public class CirculationReportsViewModel
    {
        public IEnumerable<Title> Titles { get; set; }
        public IEnumerable<Copy> Copies { get; set; }
        public IEnumerable<Classmark> Classmarks { get; set; }
        public IEnumerable<Location> Locations { get; set; }
        public IEnumerable<Keyword> Keywords { get; set; }
        public IEnumerable<Publisher> Publishers { get; set; }
        public IEnumerable<Supplier> Suppliers { get; set; }
        public IEnumerable<ApplicationUser> Recipients { get; set; }
        public bool HasData { get; set; }
        public string NoDataTitle { get; set; }
        public string NoDataMsg { get; set; }
        public string NoDataOk { get; set; }

        public CirculationReportsViewModel()
        {
            NoDataTitle = "No Data!";
            NoDataMsg = "There is no data available for this report.";
        }
    }

    public class SelectCirculatedCopy
    {
        public int CopyId { get; set; }
        public int TitleId { get; set; }
        public int CopyNumber { get; set; }
        public string Title { get; set; }
        public string Location { get; set; }
        //public int NonFilingChars { get; set; }
    }
}
