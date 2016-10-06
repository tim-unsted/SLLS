using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using slls.Localization;

namespace slls.ViewModels
{
    public class TitleEditorAddViewModel
    {
        [Key]
        public int TitleEditorId { get; set; }

        public int TitleId { get; set; }
        public int AuthorId { get; set; }

        [LocalDisplayName("TitleEditors.Order_Sequence", "FieldDisplayName")]
        public int? OrderSeq { get; set; }

        public string Title { get; set; }
        public IEnumerable<int> SelectedEditors { get; set; }
        public IEnumerable<SelectListItem> AvailableEditors { get; set; }
    }

    public class TitleEditorEditViewModel
    {
        [Key]
        public int TitleEditorId { get; set; }

        public int TitleId { get; set; }
        public int AuthorId { get; set; }

        [LocalDisplayName("TitleEditors.Order_Sequence", "FieldDisplayName")]
        public int? OrderSeq { get; set; }

        public string Title { get; set; }
    }
}