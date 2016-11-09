using System.ComponentModel;

namespace slls.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Menu")]
    public partial class Menu
    {
        public Menu()
        {
            //Menu1 = new HashSet<Menu>();
        }

        public int ID { get; set; }

        [StringLength(255)]
        public string Key { get; set; }

        public int? ParentID { get; set; }

        [StringLength(255)]
        public string Title { get; set; }

        [StringLength(50)]
        public string MenuArea { get; set; }

        [StringLength(50)]
        public string Controller { get; set; }

        [StringLength(50)]
        public string Action { get; set; }

        [StringLength(50)]
        public string LinkArea { get; set; }

        [StringLength(255)]
        public string URL { get; set; }

        [StringLength(255)]
        public string ToolTip { get; set; }

        [StringLength(50)]
        public string Class { get; set; }

        [StringLength(50)]
        public string DataToggle { get; set; }

        [StringLength(50)]
        public string DataTarget { get; set; }

        [StringLength(50)]
        public string Target { get; set; }

        public bool IsSelectable { get; set; }

        public bool IsEnabled { get; set; }

        public bool IsVisible { get; set; }

        public bool CanDelete { get; set; }

        public bool Deleted { get; set; }

        public int? SortOrder { get; set; }
        
        [StringLength(1000)]
        public string Roles { get; set; }

        [StringLength(1000)]
        public string Groups { get; set; }

        public string Description { get; set; }

        public string HoverTip
        {
            get { return string.Format("{0}", string.IsNullOrEmpty(ToolTip) ? Title: ToolTip); }
        }

        public virtual ICollection<Menu> Menu1 { get; set; }

        public virtual Menu Menu2 { get; set; }
    }
}