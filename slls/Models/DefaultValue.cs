using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace slls.Models
{
     [Table("DefaultValues")]
    public class DefaultValue
    {
         [Key]
         public int DefaultId { get; set; }

         [DisplayName("Table name")]
         public string TableName { get; set; }

         [DisplayName("Field name")]
         public string FieldName { get; set; }

         [DisplayName("Affected data")]
         public string ChildTableName { get; set; }

         [DisplayName("Default Value")]
         public int DefaultValueId { get; set; }

         [Column(TypeName = "smalldatetime")]
         public DateTime? InputDate { get; set; }

         [Column(TypeName = "smalldatetime")]
         public DateTime? LastModified { get; set; }

    }
}