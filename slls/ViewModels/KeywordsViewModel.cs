using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using slls.Localization;
using slls.Models;

namespace slls.ViewModels
{
    public class KeywordsIndexViewModel
    {
        public KeywordsIndexViewModel()
        {
            //Keyword1 = new HashSet<Keyword>();
        }

        public int KeywordID { get; set; }

        [LocalDisplayName("Keywords.Keyword", "FieldDisplayName")]
        public string KeywordTerm { get; set; }

        [LocalDisplayName("Keywords.Parent_Keyword", "FieldDisplayName")]
        public int? ParentKeywordID { get; set; }

        public IEnumerable<Models.Keyword> keywords { get; set; }

        //Stuff to handle the alphabetical paging links on the index view ...
        public List<string> FirstLetters { get; set; }
        public string SelectedLetter { get; set; }

        //Stuff to display the parent keyword ...
        public virtual ICollection<Keyword> Keyword1 { get; set; }
        public virtual Keyword Keyword2 { get; set; }
    }

    public class KeywordsCreateViewModel
    {
        [Required]
        [LocalDisplayName("Keywords.Keyword", "FieldDisplayName")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string KeywordTerm { get; set; }

        [Required]
        [LocalDisplayName("Keywords.Parent_Keyword", "FieldDisplayName")]
        public int? ParentKeywordID { get; set; }
    }

    public class KeywordsEditViewModel
    {
        public Models.Keyword _keyword { get; set; }
        
        public int KeywordID { get; set; }
        
        [LocalDisplayName("Keywords.Keyword", "FieldDisplayName")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string KeywordTerm { get; set; }

        [Required]
        [LocalDisplayName("Keywords.Parent_Keyword", "FieldDisplayName")]
        public int? ParentKeywordID { get; set; }

        public KeywordsEditViewModel()
        {
            
        }
        public KeywordsEditViewModel(Models.Keyword keyword)
        {
            this._keyword = keyword;
            this.KeywordID = _keyword.KeywordID;
            this.KeywordTerm = _keyword.KeywordTerm;
            this.ParentKeywordID = _keyword.ParentKeywordID;
        }

        [DisplayName("Can update?")]
        public bool CanUpdate { get; set; }

        [DisplayName("Can delete?")]
        public bool CanDelete { get; set; }
    }
}