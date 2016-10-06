using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using slls.Utils.Helpers;

namespace slls.ViewModels
{
    public class ParametersAddEditViewModel
    {
        public Models.Parameter _parameters { get; set; }

        public ParametersAddEditViewModel()
        {
            
        }
        public ParametersAddEditViewModel(Models.Parameter parameter)
        {
            this._parameters = parameter;
            this.ParameterID = _parameters.ParameterID;
            this.ParameterValue = _parameters.ParameterValue;
            this.ParamUsage = _parameters.ParamUsage;
        }
        
        [Key]
        public int RecID { get; set; }
        
        [DisplayName("Name")]
        public string ParameterID { get; set; }

        [DisplayName("Value")]
        public string ParameterValue { get; set; }

        [DisplayName("Area")]
        public string ParameterArea { get; set; }

        [DisplayName("Name")]
        public string ParameterName { get; set; }

        [StringLength(500)]
        [DisplayName("Comments")]
        [DataType(DataType.MultilineText)]
        public string ParamUsage { get; set; }

        public string Roles { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime InputDate { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? LastModified { get; set; }
    }

    public class ParameterIndexViewModel
    {
        public IEnumerable<Models.Parameter> Parameters { get; set; }

        public bool IsBaileyAdmin { get; set; }

        public ParameterIndexViewModel()
        {
            this.IsBaileyAdmin = Roles.IsBaileyAdmin();
        }
    }
}