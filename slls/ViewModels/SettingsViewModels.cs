using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace slls.ViewModels
{
    public class SettingsViewModel
    {
        [Key]
        [DisplayName("Name")]
        public string ParameterID { get; set; }

        [DisplayName("Value")]
        public string ParameterValue { get; set; }

        [StringLength(500)]
        [DataType(DataType.MultilineText)]
        [DisplayName("Comments")]
        public string ParamUsage { get; set; }
    }
}