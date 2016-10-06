using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using slls.Localization;
using slls.Models;

namespace slls.ViewModels
{
    public class AuthorCreateViewModel
    {
        [LocalDisplayName("Authors.Title", "FieldDisplayName")]
        public string Title { get; set; }

        [LocalDisplayName("Authors.Display_Name", "FieldDisplayName")]
        public string DisplayName { get; set; }

        [LocalDisplayName("Authors.Firstnames", "FieldDisplayName")]
        public string Firstnames { get; set; }

        [LocalDisplayName("Authors.Lastnames", "FieldDisplayName")]
        public string Lastnames { get; set; }

        [LocalDisplayName("Authors.Author_Type", "FieldDisplayName")]
        public string AuthType { get; set; }

        [LocalDisplayName("Authors.Notes", "FieldDisplayName")]
        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }
    }

    public class AuthorIndexViewModel
    {
        public int AuthorID { get; set; }

        [LocalDisplayName("Authors.Title", "FieldDisplayName")]
        public string Title { get; set; }

        [LocalDisplayName("Authors.Display_Name", "FieldDisplayName")]
        public string DisplayName { get; set; }

        [LocalDisplayName("Authors.Author_Type", "FieldDisplayName")]
        public string AuthType { get; set; }

        [LocalDisplayName("Authors.Author_Titles", "FieldDisplayName")]
        public virtual ICollection<TitleAuthor> TitleAuthors { get; set; }

        [LocalDisplayName("Authors.Editor_Titles", "FieldDisplayName")]
        public virtual ICollection<TitleEditor> TitleEditors { get; set; }

        [DisplayName("Can update?")]
        public bool CanUpdate { get; set; }

        [DisplayName("Can delete?")]
        public bool CanDelete { get; set; }

        //public AuthorIndexViewModel()
        //{
        //    TitleAuthors = new HashSet<TitleAuthor>();
        //    TitleEditors = new HashSet<TitleEditor>();
        //}
    }
}