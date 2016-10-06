using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using slls.Models;

namespace slls.ViewModels
{
    public class TitlesReportsViewModel
    {
        public TitlesReportsViewModel()
        {
            NoDataTitle = "No Report Data!";
            NoDataMsg = "Sorry, there is no data for this report that matches your chosen criteria. Please check and try again.";
            NoDataOk = "OK";
        }
        
        public IEnumerable<Title> Titles { get; set; }
        public IEnumerable<MediaType> MediaTypes { get; set; }
        public IEnumerable<Classmark> Classmarks { get; set; }
        public IEnumerable<Keyword> Keywords { get; set; }
        public IEnumerable<Author> Authors { get; set; }
        public IEnumerable<Location> Locations { get; set; }
        public IEnumerable<Location> Offices { get; set; }
        public IEnumerable<Publisher> Publishers { get; set; }
        public IEnumerable<StatusType> StatusTypesList { get; set; }

        public Location Office { get; set; }
        public Location Location { get; set; }
        public Publisher Publisher { get; set; }
        public MediaType MediaType { get; set; }
        public Keyword Keyword { get; set; }

        public int[] StatusTypes { get; set; }

        public int TitlesCount { get; set; }
        public int CopiesCount { get; set; }
        public int VolumesCount { get; set; }
        public int VolumesOnLoanCount { get; set; }
        public int VolumesAvailableCount { get; set; }
        public int RefOnlyCount { get; set; }
        public int TitlesNoMediaCount { get; set; }
        public int CopiesNoStatusCount { get; set; }
        public int CopiesNoOfficeCount { get; set; }
        public int CopiesNoLocationCount { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public bool HasData { get; set; }
        public string NoDataTitle { get; set; }
        public string NoDataMsg { get; set; }
        public string NoDataOk { get; set; }
    }
    
    public class NewAcquisitionsReportViewModel
    {
        public NewAcquisitionsReportViewModel()
        {
            NoDataTitle = "No Report Data!";
            NoDataMsg = "Sorry, there is no data for this report that matches your chosen criteria. Please check and try again.";
        }
        
        public IEnumerable<Classmark> Classmarks { get; set; }
        public IEnumerable<Title> NewTitles { get; set; }
        public int[] StatusTypes { get; set; }
        public bool HasData { get; set; }
        public string NoDataTitle { get; set; }
        public string NoDataMsg { get; set; }
    }

    public class AuthorTitleListReportViewModel
    {
        public AuthorTitleListReportViewModel()
        {
            NoDataTitle = "No Report Data!";
            NoDataMsg = "Sorry, there is no data for this report that matches your chosen criteria. Please check and try again.";
        }
        
        public IEnumerable<Title> Titles { get; set; }
        public int[] StatusTypes { get; set; }
        public bool HasData { get; set; }
        public string NoDataTitle { get; set; }
        public string NoDataMsg { get; set; }
    }

    public class CLAAuditReportViewModel
    {
        public CLAAuditReportViewModel()
        {
            NoDataTitle = "No Report Data!";
            NoDataMsg = "Sorry, there is no data for this report that matches your chosen criteria. Please check and try again.";
        }
        
        public List<SelectListItem> ExpenditureTypes { get; set; }
        public List<SelectListItem> ReportTypes { get; set; }
        public IEnumerable<SelectListItem> MediaTypes { get; set; }
        public IEnumerable<SelectListItem> StatusTypes { get; set; }
        public IEnumerable<Title> Titles { get; set; }

        [Required]
        public int ReportType { get; set; }
        public int ExpenditureType { get; set; }
        public IEnumerable<int> SelectedMediaTypes { get; set; }
        public IEnumerable<int> SelectedStatusTypes { get; set; }

        public int[] ReportStatusTypes { get; set; }

        public bool UseDates { get; set; }
        public string HeldOrPurch { get; set; }
        public bool Sub { get; set; }   
        
        //[Range(typeof(DateTime), "01/01/1900", "01/01/2079", ErrorMessage = "Date is out of Range")]
        public DateTime StartDate { get; set; }

        [Required]
        //[Range(typeof(DateTime), "01/01/1900", "01/01/2079", ErrorMessage = "Date is out of Range")]
        public DateTime EndDate { get; set; }

        public string FriendlyName { get; set; }
        public string LabelReportType { get; set; }
        public string LabelNoCopies { get; set; }
        public string LabelBetween { get; set; }
        public string Category { get; set; }
        public string ReportSource { get; set; }

        public bool HasData { get; set; }
        public string NoDataTitle { get; set; }
        public string NoDataMsg { get; set; }
       
    }
}