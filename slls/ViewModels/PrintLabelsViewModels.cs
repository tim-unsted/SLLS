using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace slls.ViewModels
{
    public class PrintLabelsViewModel
    {
        public bool LabelsPending { get; set; }

        public string AlertMsg { get; set; }
        
        public string PostSelectAction { get; set; }
        
        public string Title { get; set; }

        public bool ShowOrderBy { get; set; }

        [DisplayName("Order By")]
        public string OrderBy { get; set; }

        [DisplayName("Location")]
        public int LocationID { get; set; }

        public IEnumerable<SelectListItem> Locations { get; set; }

        [DisplayName("Starting row")]
        public int StartPositionRow { get; set; }

        [DisplayName("Starting column")]
        public int StartPositioncolumn { get; set; }

        [DisplayName("Top margin")]
        public float topMargin { get; set; }

        [DisplayName("Bottom margin")]
        public float bottomMargin { get; set; }

        [DisplayName("Left margin")]
        public float leftMargin { get; set; }

        [DisplayName("Right margin")]
        public float rightMargin { get; set; }

        [DisplayName("No. labels across")]
        public int labelsAcross { get; set; }

        [DisplayName("No. labels down")]
        public int labelsDown { get; set; }

    }
}