namespace slls.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("EmailLog")]
    public partial class EmailLog
    {
        [Key]
        public int EmailID { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? DateSent { get; set; }

        [StringLength(100)]
        public string From { get; set; }

        [StringLength(100)]
        public string To { get; set; }

        [StringLength(100)]
        public string CC { get; set; }

        [StringLength(100)]
        public string BCC { get; set; }

        [StringLength(250)]
        public string Subject { get; set; }

        public string Body { get; set; }
    }
}
