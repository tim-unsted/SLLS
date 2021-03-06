﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using slls.Localization;

namespace slls.Models
{
    [Table("LibraryUserBookmarks")]
    public class LibraryUserBookmark
    {
        [Key]
        public int BookmarkID { get; set; }

        public int TitleID { get; set; }
        public string UserID { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }

        [Column(TypeName = "smalldatetime")]
        [LocalDisplayName("Bookmarks.Date_Bookmarked", "FieldDisplayName")]
        public DateTime? InputDate { get; set; }

        //public virtual LibraryUser LibraryUser { get; set; }
        public virtual Title Title { get; set; }

        [NotMapped]
        public string InputDateSortable
        {
            get { return string.Format("{0:yyyy-MM-dd}", InputDate); }
        }
    }
}