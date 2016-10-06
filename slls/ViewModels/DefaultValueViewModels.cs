using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using slls.Models;

namespace slls.ViewModels
{
    public class DefaultValueEditViewModel
    {
        public int DefaultId { get; set; }

        [DisplayName("Table name")]
        public string TableName { get; set; }

        [DisplayName("Field name")]
        public string FieldName { get; set; }

        [DisplayName("Affected data")]
        public string ChildTableName { get; set; }

        [DisplayName("Default Value")]
        public int DefaultValueId { get; set; }

        [DisplayName("Default Value")]
        public string DefaultValue { get; set; }
    }

    public class DefaultValueIndexViewModel
    {
        public List<DefaultValueEditViewModel> DefaultValues { get; set; }
    }
}