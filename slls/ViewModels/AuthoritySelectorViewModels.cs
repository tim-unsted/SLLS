using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Westwind.Globalization;

namespace slls.ViewModels
{
    public class AuthoritySelectorViewModel
    {
        public AuthoritySelectorViewModel()
        {
            ButtonText = DbRes.T("Buttons.Ok", "Terminology");
        }

        public string HeaderText { get; set; }
        public string DetailsText { get; set; }
        public string ButtonText { get; set; }
        public string LabelText { get; set; }
        public string PostAction { get; set; }
        public string PostController { get; set; }
        public IEnumerable<SelectListItem> AuthorityList { get; set; }
        public IEnumerable<int> SelectedValue { get; set; }
    }
}