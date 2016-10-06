using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using slls.Localization;


namespace slls.Models
{
    [Table("SearchFields")]
    public class SearchField
    {
        [Key]
        public string FieldId { get; set; }

        [DisplayName("Field Name")]
        public string FieldName { get; set; }

        [DisplayName("Area")]
        public string Scope { get; set; }

        [DisplayName("Position in list")]
        public int Position { get; set; }

        [DisplayName("Enabled?")]
        public bool Enabled { get; set; }
    }
}