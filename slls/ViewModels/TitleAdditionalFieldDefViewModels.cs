using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace slls.ViewModels
{
    public class LongTextLabelAdd
    {
        [Required]
        [StringLength(128)]
        [DisplayName("Text Label")]
        public string TextLabel { get; set; }

        [DisplayName("Is Large Text?")]
        public bool IsLongText { get; set; }

        [DisplayName("Show field on OPAC?")]
        public bool ShowOnOpac { get; set; }
    }

    public class CustomFieldAdd
    {
        [Required]
        [StringLength(128)]
        [DisplayName("Field Name")]
        public string FieldName { get; set; }
        
        [DisplayName("Is Date?")]
        public bool IsDate { get; set; }

        [DisplayName("Is Numeric?")]
        public bool IsNumeric { get; set; }

        [DisplayName("Is True/False?")]
        public bool IsBoolean { get; set; }
        
        [DisplayName("Show field on OPAC?")]
        public bool ShowOnOpac { get; set; }
    }

    public class TitleAdditionalFieldDefEdit
    {
        [Key]
        public int FieldId { get; set; }

        [Required]
        [StringLength(128)]
        [DisplayName("Field Name")]
        public string FieldName { get; set; }

        [DisplayName("Is Date?")]
        public bool IsDate { get; set; }

        [DisplayName("Is Numeric?")]
        public bool IsNumeric { get; set; }

        [DisplayName("Is True/False?")]
        public bool IsBoolean { get; set; }

        [DisplayName("Is Large Text?")]
        public bool IsLongText { get; set; }

        [DisplayName("Show field on OPAC?")]
        public bool ShowOnOpac { get; set; }
    }
}