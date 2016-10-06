using System.ComponentModel;
using slls.Localization;

namespace slls.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SupplierPeopleComms")]
    public partial class SupplierPeopleComm
    {
        [Key]
        public int CommID { get; set; }

        public int ContactID { get; set; }

        [Required]
        [LocalDisplayName("CommunicationTypes.Method", "FieldDisplayName")]
        public int MethodID { get; set; }

        [Required]
        [LocalDisplayName("CommunicationTypes.Detail", "FieldDisplayName")]
        public string Detail { get; set; }
        
        [DisplayName("Deleted?")]
        public bool Deleted { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? InputDate { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? LastModified { get; set; }

        public virtual CommMethodType CommMethodType { get; set; }

        public virtual SupplierPeople SupplierPeople { get; set; }
    }
}
