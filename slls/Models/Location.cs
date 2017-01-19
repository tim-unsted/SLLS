using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using slls.Localization;

namespace slls.Models
{
    [Table("Locations")]
    public class Location
    {
        public Location()
        {
            //Copies = new HashSet<Copy>();
            //LibraryUsers = new HashSet<ApplicationUser>();
            //SubLocations = new HashSet<Location>();
        }

        [Key]
        [Column("LocationID")]
        public int LocationID { get; set; }

        [Column("ParentLocationID")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [LocalDisplayName("Locations.Parent_Location", "FieldDisplayName")]
        public int? ParentLocationID { get; set; }

        [Column("Location")]
        [LocalDisplayName("Locations.Location", "FieldDisplayName")]
        public string Location1 { get; set; }

        [LocalDisplayName("Locations.Location", "FieldDisplayName")]
        public string LocationName
        {
            get
            {
                return string.IsNullOrEmpty(Location1) ? "<no name>" : Location1;
            }
        }

        [LocalDisplayName("Locations.Location", "FieldDisplayName")]
        public string LocationString
        {
            get
            {
                return ParentLocation != null ? ParentLocation.LocationName + " - " + LocationName : LocationName;
            }
        }

        [StringLength(128)]
        public string LocationHier { get; set; }

        [DisplayName("Can update?")]
        public bool CanUpdate { get; set; }

        [DisplayName("Can delete?")]
        public bool CanDelete { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? InputDate { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? LastModified { get; set; }

        [DisplayName("Deleted?")]
        public bool Deleted { get; set; }

        public int ListPos { get; set; }

        [Column(TypeName = "timestamp")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [MaxLength(8)]
        public byte[] RowVersion { get; set; }

        [LocalDisplayName("Locations.Copies", "FieldDisplayName")]
        public virtual ICollection<Copy> Copies { get; set; }

        [LocalDisplayName("Locations.Users", "FieldDisplayName")]
        public virtual ICollection<ApplicationUser> LibraryUsers { get; set; }

        public virtual ICollection<Location> SubLocations { get; set; }
        public virtual Location ParentLocation { get; set; }
    }
}