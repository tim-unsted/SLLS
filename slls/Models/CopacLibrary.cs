using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using slls.Localization;

namespace slls.Models
{
    [Table("CopacLibraries")]
    public class CopacLibrary
    {
        public CopacLibrary()
        {
        }

        [Key]
        public int LibraryID { get; set; }

        [Column("Library")]
        [Required]
        [StringLength(255)]
        public string Library { get; set; }
    }
}