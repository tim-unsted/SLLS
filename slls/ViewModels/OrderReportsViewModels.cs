using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using slls.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Services.Protocols;

namespace slls.ViewModels
{
    public class OrderReportsViewModel
    {
        public OrderReportsViewModel()
        {
            NoDataTitle = "No Report Data!";
            NoDataMsg = "Sorry, there is no data for this report that matches your chosen criteria. Please check and try again.";
            NoDataOk = "OK";
        }

        public IEnumerable<OrderDetail> Orders { get; set; }
        public IEnumerable<Supplier> Suppliers { get; set; }
        public IEnumerable<BudgetCode> BudgetCodes { get; set; }
        public IEnumerable<MediaType> MediaTypes { get; set; }
        public IEnumerable<OrderCategory> OrderCategories { get; set; }
        public IEnumerable<Title> Titles { get; set; }
        public IEnumerable<Copy> Copies { get; set; }
        public IEnumerable<ApplicationUser> AllUsers { get; set; }
        public IEnumerable<ApplicationUser> Requestors { get; set; } 
        public IEnumerable<ApplicationUser> Authorisers { get; set; }
        public string BudgetCodeId { get; set; }
        public int AccountYearId { get; set; }
        public string BudgetCode { get; set; }
        public string AccountYear { get; set; }
        public int OrderCategory { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool Sub { get; set; }
        public bool OneOff { get; set; }
        public bool HasData { get; set; }
        public string NoDataTitle { get; set; }
        public string NoDataMsg { get; set; }
        public string  NoDataOk { get; set; }
    }
}