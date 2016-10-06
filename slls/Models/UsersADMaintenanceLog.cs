namespace slls.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("UsersADMaintenanceLog")]
    public partial class UsersADMaintenanceLog
    {
        [Key]
        public int RecID { get; set; }

        public int UserID { get; set; }

        public bool Inserted { get; set; }

        public bool Updated { get; set; }

        public bool SetLive { get; set; }

        public bool SetNonLive { get; set; }

        [StringLength(255)]
        public string Domain { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? TransDate { get; set; }
    }
}
