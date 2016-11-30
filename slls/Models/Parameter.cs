using System.ComponentModel;
using System.Linq;

namespace slls.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Parameters")]
    public partial class Parameter
    {
        [Key]
        public int RecID { get; set; }

        [StringLength(255)]
        [Required]
        [DisplayName("Name")]
        public string ParameterID{ get; set; }

        [DisplayName("Value")]
        public string ParameterValue { get; set; }

        [StringLength(500)]
        [DisplayName("Comments")]
        [DataType(DataType.MultilineText)]
        public string ParamUsage { get; set; }

        public string Roles { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime InputDate { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? LastModified { get; set; }

        [DisplayName("Can update?")]
        public bool CanUpdate { get; set; }

        [DisplayName("Can delete?")]
        public bool CanDelete { get; set; }

        [DisplayName("Deleted?")]
        public bool Deleted { get; set; }

        public string Packages { get; set; }

        [NotMapped]
        public int ListPos { get; set; }
        
        [Column(TypeName = "timestamp")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [MaxLength(8)]
        public byte[] RowVersion { get; set; }

        public string ParameterArea
        {
            get
            {
                var parmameterstring = ParameterID.Split('.');
                return parmameterstring[0];
            }
        }

        public string ParameterName
        {
            get
            {
                char[] charSeparators = new char[] {'.'};
                var parmameterstring = ParameterID.Split(charSeparators);
                parmameterstring = parmameterstring.Skip(1).ToArray();
                return string.Join(".", parmameterstring);
            }
        }
    }
}
