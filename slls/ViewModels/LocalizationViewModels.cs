using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using slls.Localization;

namespace slls.ViewModels

{
    public class LocalizationIndexViewModel
    {
        public IEnumerable<Models.Localization> localizations { get; set; }
        
        [Key]
        public int pk { get; set; }

        [StringLength(1024)]
        [LocalDisplayName("Localization.Resource", "FieldDisplayName")]
        public string ResourceId { get; set; }

        [Required]
        [LocalDisplayName("Localization.Value", "FieldDisplayName")]
        public string Value { get; set; }

        [StringLength(10)]
        [LocalDisplayName("Localization.Locale", "FieldDisplayName")]
        public string LocaleId { get; set; }

        [StringLength(512)]
        [LocalDisplayName("Localization.ResourceSet", "FieldDisplayName")]
        public string ResourceSet { get; set; }



    }

    public class LocalizationEditViewModel
    {
        public Models.Localization _localization { get; set; }

        [Key]
        public int pk { get; set; }

        [StringLength(1024)]
        [LocalDisplayName("Localization.Resource", "FieldDisplayName")]
        public string ResourceId { get; set; }
        
        [Required]
        [LocalDisplayName("Localization.Value", "FieldDisplayName")]
        public string Value { get; set; }

        [StringLength(10)]
        [LocalDisplayName("Localization.Locale", "FieldDisplayName")]
        public string LocaleId { get; set; }

        [StringLength(512)]
        [LocalDisplayName("Localization.ResourceSet", "FieldDisplayName")]
        public string ResourceSet { get; set; }

        public LocalizationEditViewModel()
        {
            
        }
        public LocalizationEditViewModel(Models.Localization localization)
        {
            this._localization = localization;
            this.ResourceId = _localization.ResourceId;
            this.Value = _localization.Value;
            this.LocaleId = _localization.LocaleId;
            this.ResourceSet = _localization.ResourceSet;
        }
    }

    public class LocalizationDetailsViewModel
    {
        public Models.Localization _localization { get; set; }

        [Key]
        public int pk { get; set; }

        [LocalDisplayName("Localization.Resource", "FieldDisplayName")]
        public string ResourceId { get; set; }

        [Required]
        [LocalDisplayName("Localization.Value", "FieldDisplayName")]
        public string Value { get; set; }

        [LocalDisplayName("Localization.Locale", "FieldDisplayName")]
        public string LocaleId { get; set; }

        [LocalDisplayName("Localization.ResourceSet", "FieldDisplayName")]
        public string ResourceSet { get; set; }
        
        public LocalizationDetailsViewModel(Models.Localization localization)
        {
            this._localization = localization;
            ResourceId = _localization.ResourceId;
            Value = _localization.Value;
            LocaleId = _localization.LocaleId;
            ResourceSet = _localization.ResourceSet;
        }
    }



    
}