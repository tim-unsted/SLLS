namespace slls.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("EmailLogAttachment")]
    public partial class EmailLogAttachment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int EmailID { get; set; }

        [StringLength(1000)]
        public string AttachmentName { get; set; }

        public byte[] AttachmentFile { get; set; }
    }
}
