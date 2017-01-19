using System.ComponentModel;
using slls.Localization;

namespace slls.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CommMethodTypes")]
    public partial class CommMethodType
    {
        public CommMethodType()
        {
            //SupplierPeopleComms = new HashSet<SupplierPeopleComm>();
        }

        [Key]
        public int MethodID { get; set; }

        [StringLength(255)]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [LocalDisplayName("SupplierCommTypes.Communication_Type", "FieldDisplayName")]
        public string Method { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? InputDate { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? LastModified { get; set; }

        [DisplayName("Can update?")]
        public bool CanUpdate { get; set; }

        [DisplayName("Can delete?")]
        public bool CanDelete { get; set; }

        [DisplayName("Deleted?")]
        public bool Deleted { get; set; }

        public int ListPos { get; set; }

        [Column(TypeName = "timestamp")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [MaxLength(8)]
        public byte[] RowVersion { get; set; }

        public virtual ICollection<SupplierPeopleComm> SupplierPeopleComms { get; set; }
    }
}
