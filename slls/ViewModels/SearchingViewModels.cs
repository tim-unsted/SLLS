using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using slls.App_Settings;
using slls.DAO;
using slls.Localization;
using slls.Models;
using slls.Utils.Helpers;

namespace slls.ViewModels
{
    public class SimpleSearchingViewModel
    {
        //Is the current user an admin user
        public bool LibraryStaff { get; set; }

        //Are the results being used in an actual search, or in a Browse By page?
        public bool IsActualSearch { get; set; }
        
        public string Area { get; set; }

        [LocalDisplayName("Searching.SearchTerm", "FieldDisplayName")]
        [Required (ErrorMessage = "Please enter a word or phrase to search for.")]
        public string SearchString { get; set; }

        [LocalDisplayName("Searching.SearchWhere", "FieldDisplayName")]
        public string SearchField { get; set; }

        [LocalDisplayName("Searching.Order_By", "FieldDisplayName")]
        public string OrderBy { get; set; }

        public IEnumerable<SelectListItem> SearchFields { get; set; }

        public List<Title> Titles { get; set; }
        public List<Title> Results { get; set; }
        public JsonResult JsonData { get; set; }
        public List<Title> ResultsBeforeFilter { get; set; }
        public List<Title> OpacTitles { get; set; }

        public List<string> Filters { get; set; }
        public List<int> TitleIDs { get; set; }

        public List<SelectClassmarkEditorViewModel> ClassmarksFilter { get; set; }
        public List<SelectMediaEditorViewModel> MediaFilter { get; set; }
        public List<SelectPublisherEditorViewModel> PublisherFilter { get; set; }
        public List<SelectLanguageEditorViewModel> LanguageFilter { get; set; }
        public List<SelectKeywordEditorViewModel> KeywordFilter { get; set; }
        public List<SelectAuthorEditorViewModel> AuthorFilter { get; set; }

        public int NarrowByDefaultRecordCount { get; set; }
        public int SearchResultSize { get; set; }
        public string SearchStyle { get; set; }
        public string SelectItem { get; set; }
        public int SelectedId { get; set; }

        public bool AutoSuggestEnabled { get; set; }
        
        public SimpleSearchingViewModel()
        {
            this.ClassmarksFilter = new List<SelectClassmarkEditorViewModel>();
            this.MediaFilter = new List<SelectMediaEditorViewModel>();
            this.PublisherFilter = new List<SelectPublisherEditorViewModel>();
            this.LanguageFilter = new List<SelectLanguageEditorViewModel>();
            this.KeywordFilter = new List<SelectKeywordEditorViewModel>();
            this.AuthorFilter = new List<SelectAuthorEditorViewModel>();
            this.Filters = new List<string>();
            this.ResultsBeforeFilter = new List<Title>();
            this.NarrowByDefaultRecordCount = int.Parse(Settings.GetParameterValue("Searching.NarrowByDefaultRecordCount", "5", dataType: "int"));
            this.SearchResultSize = int.Parse(Settings.GetParameterValue("Searching.SearchResultSize", "10", dataType: "int"));
            this.SearchStyle = "prefix";
            this.OrderBy = "title";
            this.LibraryStaff = Roles.IsUserInRole("Catalogue Admin");
            this.SearchField = "all";
            this.AutoSuggestEnabled =
                Settings.GetParameterValue("Searching.EnableAutoSuggest", "true",
                    "When enabled, displays other similar searched terms as suggestions.", dataType: "bool") == "true";
        }

        public IEnumerable<int> GetSelectedClassmarkIds()
        {
            // Return an Enumerable containing the Id's of the selected classmarks:
            return (from x in this.ClassmarksFilter where x.Selected select x.Id).ToList();
        }
        public IEnumerable<int> GetSelectedMediaIds()
        {
            // Return an Enumerable containing the Id's of the selected media types:
            return (from x in this.MediaFilter where x.Selected select x.Id).ToList();
        }
        public IEnumerable<int> GetSelectedPublisherIds()
        {
            // Return an Enumerable containing the Id's of the selected publishers:
            return (from x in this.PublisherFilter where x.Selected select x.Id).ToList();
        }
        public IEnumerable<int> GetSelectedLanguageIds()
        {
            // Return an Enumerable containing the Id's of the selected languages:
            return (from x in this.LanguageFilter where x.Selected select x.Id).ToList();
        }
        public IEnumerable<int> GetSelectedKeywordIds()
        {
            // Return an Enumerable containing the Id's of the selected keywords:
            return (from x in this.KeywordFilter where x.Selected select x.Id).ToList();
        }
        public IEnumerable<int> GetSelectedAuthorIds()
        {
            // Return an Enumerable containing the Id's of the selected authors:
            return (from x in this.AuthorFilter where x.Selected select x.Id).ToList();
        }
    }

    public class FinanceSearchingViewModel
    {
        //Is the current user an admin user
        public bool LibraryStaff { get; set; }

        public string Area { get; set; }

        [LocalDisplayName("Searching.SearchTerm", "FieldDisplayName")]
        public string SearchString { get; set; }

        [LocalDisplayName("Searching.SearchWhere", "FieldDisplayName")]
        public string SearchField { get; set; }

        public IEnumerable<SelectListItem> SearchFields { get; set; }

        public List<OrderDetail> Results { get; set; }
        public List<OrderDetail> ResultsBeforeFilter { get; set; }
        public List<OrderDetail> AllOrders { get; set; }

        public List<string> Filters { get; set; }
        public List<int> OrderIDs { get; set; }

        public List<SelectAccountYearEditorViewModel> AccountYearsFilter { get; set; }
        public List<SelectBudgetCodeEditorViewModel> BudgetCodesFilter { get; set; }
        public List<SelectOrderCategoryEditorViewModel> OrderCategoriesFilter { get; set; }
        public List<SelectSupplierEditorViewModel> SuppliersFilter { get; set; }
        public List<SelectRequesterEditorViewModel> RequestersFilter { get; set; }

        public int NarrowByDefaultRecordCount { get; set; }
        public int SearchResultSize { get; set; }

        public FinanceSearchingViewModel()
        {
            this.OrderCategoriesFilter = new List<SelectOrderCategoryEditorViewModel>();
            this.BudgetCodesFilter = new List<SelectBudgetCodeEditorViewModel>();
            this.AccountYearsFilter = new List<SelectAccountYearEditorViewModel>();
            this.SuppliersFilter = new List<SelectSupplierEditorViewModel>();
            this.RequestersFilter = new List<SelectRequesterEditorViewModel>();
            this.Filters = new List<string>();
            this.ResultsBeforeFilter = new List<OrderDetail>();
            this.NarrowByDefaultRecordCount = int.Parse(Settings.GetParameterValue("Searching.NarrowByDefaultRecordCount", "5", dataType: "int"));
            this.SearchResultSize = int.Parse(Settings.GetParameterValue("Searching.SearchResultSize", "10", dataType: "int"));
        }

        public IEnumerable<int> GetSelectedAccountYearIds()
        {
            // Return an Enumerable containing the Id's of the selected account years:
            return (from x in this.AccountYearsFilter where x.Selected select x.Id).ToList();
        }

        public IEnumerable<int> GetSelectedOrderCategoryIds()
        {
            // Return an Enumerable containing the Id's of the selected order categories:
            return (from x in this.OrderCategoriesFilter where x.Selected select x.Id).ToList();
        }

        public IEnumerable<int> GetSelectedBudgetCodeIds()
        {
            // Return an Enumerable containing the Id's of the selected budget codes:
            return (from x in this.BudgetCodesFilter where x.Selected select x.Id).ToList();
        }

        public IEnumerable<int> GetSelectedSupplierIds()
        {
            // Return an Enumerable containing the Id's of the selected suppliers:
            return (from x in this.SuppliersFilter where x.Selected select x.Id).ToList();
        }

        public IEnumerable<string> GetSelectedRequesterIds()
        {
            // Return an Enumerable containing the Id's of the selected requesters:
            return (from x in this.RequestersFilter where x.Selected select x.Id).ToList();
        }
    }

    public class SelectClassmarkEditorViewModel
    {
        public bool Selected { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public int TitleCount { get; set; }
    }

    public class SelectMediaEditorViewModel
    {
        public bool Selected { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public int TitleCount { get; set; }
    }

    public class SelectAuthorEditorViewModel
    {
        public bool Selected { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public int TitleCount { get; set; }
    }

    public class SelectLanguageEditorViewModel
    {
        public bool Selected { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public int TitleCount { get; set; }
    }

    public class SelectKeywordEditorViewModel
    {
        public bool Selected { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public int TitleCount { get; set; }
    }

    public class SelectPublisherEditorViewModel
    {
        public bool Selected { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public int TitleCount { get; set; }
    }

    public class SelectAccountYearEditorViewModel
    {
        public bool Selected { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public int OrderCount { get; set; }
    }

    public class SelectBudgetCodeEditorViewModel
    {
        public bool Selected { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public int OrderCount { get; set; }
    }

    public class SelectOrderCategoryEditorViewModel
    {
        public bool Selected { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public int OrderCount { get; set; }
    }

    public class SelectSupplierEditorViewModel
    {
        public bool Selected { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public int OrderCount { get; set; }
    }

    public class SelectRequesterEditorViewModel
    {
        public bool Selected { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public int OrderCount { get; set; }
    }

    public class SearchTerms
    {
        public string SearchTerm { get; set; }
    }

}