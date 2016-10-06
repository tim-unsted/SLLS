using System.ComponentModel;
using slls.Localization;

namespace slls.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TitleAdditionalFieldDefs")]
    public partial class TitleAdditionalFieldDef
    {
        public TitleAdditionalFieldDef()
        {
            //TitleAdditionalFieldDatas = new HashSet<TitleAdditionalFieldData>();
        }

        [Key]
        public int FieldID { get; set; }

        [Required]
        [StringLength(128)]
        [LocalDisplayName("CustomTitleField.Field", "FieldDisplayName")]
        public string FieldName { get; set; }

        [DisplayName("Is Date?")]
        public bool IsDate { get; set; }

        [DisplayName("Is Numeric?")]
        public bool IsNumeric { get; set; }

        [DisplayName("Is True/False?")]
        public bool IsBoolean { get; set; }

        [DisplayName("Is Long Text?")]
        public bool IsLongText { get; set; }

        [DisplayName("Show field on OPAC?")]
        public bool ShowOnOPAC { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? InputDate { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? LastModified { get; set; }

        [DisplayName("Deleted?")]
        public bool Deleted { get; set; }

        [Column(TypeName = "timestamp")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [MaxLength(8)]
        public byte[] RowVersion { get; set; }

        public virtual ICollection<TitleAdditionalFieldData> TitleAdditionalFieldDatas { get; set; }
    }
}
