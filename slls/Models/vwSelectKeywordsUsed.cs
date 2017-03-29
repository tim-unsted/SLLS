using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace slls.Models
{
    [Table("vwSelectKeywordsUsed")]
    public class vwSelectKeywordUsed
    {
        [Key]
        [Column("KeywordID")]
        public int KeywordId { get; set; }

        [Column("KeywordTerm")]
        public string KeywordTerm { get; set; }
        
    }
}