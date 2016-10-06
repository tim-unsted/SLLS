using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using slls.Localization;

namespace slls.ViewModels
{
    public class TitleAuthorAddViewModel
    {
        [Key]
        public int TitleAuthorId { get; set; }

        public int TitleId { get; set; }
        public int AuthorId { get; set; }

        [LocalDisplayName("TitleAuthors.Order_Sequence", "FieldDisplayName")]
        public int? OrderSeq { get; set; }

        public string Title { get; set; }
        public IEnumerable<int> SelectedAuthors { get; set; }
        public IEnumerable<SelectListItem> AvailableAuthors { get; set; }
    }

    public class TitleAuthorEditViewModel
    {
        [Key]
        public int TitleAuthorId { get; set; }

        public int TitleId { get; set; }
        public int AuthorId { get; set; }

        [LocalDisplayName("TitleAuthors.Order_Sequence", "FieldDisplayName")]
        public int? OrderSeq { get; set; }

        public string Title { get; set; }
    }
}