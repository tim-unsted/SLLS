using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using slls.Models;

namespace slls.ViewModels
{
    public class MenuViewModel
    {
        public MenuViewModel()
        {
           // Menu1 = new HashSet<Menu>();
        }
        
        [Key]
        public int ID { get; set; }

        [DisplayName("Key")]
        public string Key { get; set; }

        [DisplayName("Parent Item")]
        public int? ParentID { get; set; }

        [DisplayName("Parent Item")]
        public string Parent { get; set; }

        [StringLength(255)]
        [DisplayName("Displayed Text")]
        public string Title { get; set; }

        [StringLength(50)]
        [DisplayName("Menu Area")]
        public string MenuArea { get; set; }

        [StringLength(50)]
        public string Controller { get; set; }

        [StringLength(50)]
        public string Action { get; set; }

        public string NameSpace { get; set; }

        [StringLength(50)]
        [DisplayName("Link Destination Area")]
        public string LinkArea { get; set; }

        [StringLength(255)]
        [DisplayName("Navigate To / URL")]
        public string Url { get; set; }

        [StringLength(255)]
        [DisplayName("Hover Tip")]
        public string ToolTip { get; set; }

        [StringLength(50)]
        [DisplayName("Styling Class")]
        public string Class { get; set; }

        [StringLength(50)]
        [DisplayName("Data Toggle")]
        public string DataToggle { get; set; }

        [StringLength(50)]
        [DisplayName("Data Target")]
        public string DataTarget { get; set; }

        [StringLength(50)]
        [DisplayName("Link Behaviour")]
        public string Target { get; set; }

        [DisplayName("Selectable?")]
        public bool IsSelectable { get; set; }

        [DisplayName("Enabled?")]
        public bool IsEnabled { get; set; }

        [DisplayName("Visible?")]
        public bool IsVisible { get; set; }

        [DisplayName("Divider?")]
        public bool IsDivider { get; set; }

        [DisplayName("Can Delete?")]
        public bool CanDelete { get; set; }

        [DisplayName("Deleted?")]
        public bool Deleted { get; set; }
        
        [DisplayName("Sort Order/Position")]
        public int? SortOrder { get; set; }

        [DisplayName("Roles/Permissions")]
        public string Roles { get; set; }

        [DisplayName("Groups")]
        public string Groups { get; set; }

        [DisplayName("Description")]
        public string Description { get; set; }

        public bool IsBsAdmin { get; set; }

        public virtual ICollection<Menu> Menu1 { get; set; }

        public virtual Menu Menu2 { get; set; }

        public List<SelectListItem> RolesList { get; set; }

        public List<SelectListItem> Actions { get; set; }
    }
}