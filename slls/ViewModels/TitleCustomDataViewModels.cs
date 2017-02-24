using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using slls.Localization;

namespace slls.ViewModels
{
    public class TitleCustomDataIndexViewModel
    {
        [Key]
        public int RecId { get; set; }

        public int FieldId { get; set; }

        public int TitleId { get; set; }

        public string FieldName { get; set; }

        public string FieldData { get; set; }
    }

    public class TitleCustomDataAddViewModel
    {
        [DisplayName("Type")]
        public int FieldId { get; set; }

        [DisplayName("Title")]
        public int TitleId { get; set; }

        [DisplayName("Title")]
        public string Title { get; set; }

        [DisplayName("Type")]
        public string FieldName { get; set; }

        [DisplayName("Text")]
        [AllowHtml]
        public string FieldData { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? InputDate { get; set; }
    }

    public class TitleCustomDataEditViewModel
    {
        [Key]
        public int RecId { get; set; }

        [DisplayName("Type")]
        public int FieldId { get; set; }

        [DisplayName("Title")]
        public int TitleId { get; set; }

        [LocalDisplayName("Titles.Title", "FieldDisplayName")]
        public string Title { get; set; }

        [DisplayName("Type")]
        public string FieldName { get; set; }

        [DisplayName("Text")]
        [AllowHtml]
        public string FieldData { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? LastModified { get; set; }

        [DisplayName("Is Date?")]
        public bool IsDate { get; set; }

        [DisplayName("Is Numeric?")]
        public bool IsNumeric { get; set; }

        [DisplayName("Is True/False?")]
        public bool IsBoolean { get; set; }

        [DisplayName("Is Long Text?")]
        public bool IsLongText { get; set; }
    }
}