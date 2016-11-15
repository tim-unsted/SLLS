using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using slls.Models;

namespace slls.ViewModels
{
    public class DashboardViewModel
    {
        public bool ShowWelcomeMessage { get; set; }
        public string WelcomeHeader { get; set; }
        public string WelcomeMessage { get; set; }
        public bool HasTitles { get; set; }
        public List<NewTitlesSimpleViewModel> NewTitles { get; set; }
        public List<Title> RecentTitles { get; set; }
        public List<OrderDetail> OverdueOrders { get; set; }
        public List<Copy> IssuesExpected { get; set; }
        public List<Notification> Notifications { get; set; }
        public bool LibraryStaff { get; set; }
    }
}