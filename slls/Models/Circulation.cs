using System.ComponentModel;

namespace slls.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Circulation")]
    public partial class Circulation
    {
        //public Circulation()
        //{
        //    RecipientUser = new ApplicationUser();
        //}
        
        [Key]
        public int CirculationID { get; set; }

        public int? CopyID { get; set; }
        
        //[Column("UserID")]
        //public string UserID { get; set; }
        
        public int? SortOrder { get; set; }

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

        public virtual ApplicationUser RecipientUser { get; set; }
        public virtual Copy Copy { get; set; }

        
    }
}
