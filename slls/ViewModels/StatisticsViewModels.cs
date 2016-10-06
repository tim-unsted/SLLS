using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Westwind.Globalization;

namespace slls.ViewModels
{
    public class AuthorityStatisticsViewModel
    {
        public IEnumerable<string> AuthorityLists { get; set; }
        public int AccountYearsCount { get; set; }
        public int ActivityTypesCount { get; set; }
        public int AuthorsCount { get; set; }
        public int BudgetCodesCount { get; set; }
        public int ClassmarksCount { get; set; }
        public int DepartmentsCount { get; set; }
        public int FrequenciesCount { get; set; }
        public int KeywordsCount { get; set; }
        public int LanguagesCount { get; set; }
        public int LocationsCount { get; set; }
        public int MediaTypesCount { get; set; }
        public int PublishersCount { get; set; }
        public int StatusTypesCount { get; set; }
        
    }

    public class AdminAlertsViewModel
    {
        //Alerts
        public bool show { get; set; }
        public int OutstandingOrders { get; set; }
        public int OverdueOrders { get; set; }
        public int IssuesExpected { get; set; }
        public int OverdueIssues { get; set; }
        public int OrdersOnApproval { get; set; }
        public int ReservationRequests { get; set; }
        public int OverdueLoans { get; set; }
        public int TitlesNoCopies { get; set; }
        public int CopiesNoVolumes { get; set; }
        public bool HasTitles { get; set; }
        public bool HasAccountYears { get; set; }
        public bool HasActivityTypes { get; set; }
        public bool HasAuthors { get; set; }
        public bool HasBudgetCodes { get; set; }
        public bool HasOrderCategories { get; set; }
        public bool HasClassmarks { get; set; }
        public bool HasDepartments { get; set; }
        public bool HasFrequencies { get; set; }
        public bool HasKeywords { get; set; }
        public bool HasLanguages { get; set; }
        public bool HasLocations { get; set; }
        public bool HasMediaTypes { get; set; }
        public bool HasPublishers { get; set; }
        public bool HasStatusTypes { get; set; }
        public bool HasSuppliers { get; set; }
        public bool HasOrders { get; set; }
        public bool HasLoans { get; set; }
        public bool HasReservations { get; set; }
        public bool HasLoanTypes { get; set; }

    }
}