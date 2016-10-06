using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace slls.ViewModels
{
    public class ReleaseNotesAddEditViewModel
    {
        [Key]
        public int ReleaseNoteId { get; set; }

        [Required]
        [DisplayName("Release Header ID")]
        public int ReleaseId { get; set; }

        [Required]
        [DisplayName("Item")]
        public int SequenceNo { get; set; }

        [DataType(DataType.MultilineText)]
        public string Detail { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? InputDate { get; set; }
    }
}