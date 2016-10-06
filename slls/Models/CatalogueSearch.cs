namespace slls.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CatalogueSearches")]
    public partial class CatalogueSearch
    {
        [Key]
        public int SearchID { get; set; }

        [StringLength(255)]
        public string SearchCriteria { get; set; }

        [StringLength(50)]
        public string UserName { get; set; }
    }
}
