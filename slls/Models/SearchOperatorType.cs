namespace slls.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SearchOperatorTypes")]
    public partial class SearchOperatorType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int RecID { get; set; }

        [Required]
        [StringLength(50)]
        public string EnglishPhrase { get; set; }

        [StringLength(50)]
        public string Operator { get; set; }

        [StringLength(50)]
        public string Prefix { get; set; }

        [StringLength(5)]
        public string Suffix { get; set; }
    }
}
