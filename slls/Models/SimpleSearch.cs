namespace slls.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SimpleSearch")]
    public partial class SimpleSearch
    {
        [Key]
        public int SearchID { get; set; }

        [StringLength(800)]
        public string EnglishPhrase { get; set; }

        [StringLength(50)]
        public string ViewName { get; set; }

        [StringLength(50)]
        public string Fieldname { get; set; }

        [StringLength(50)]
        public string OperatorType { get; set; }

        public string SQLString { get; set; }

        [StringLength(50)]
        public string SearchType { get; set; }
    }
}
