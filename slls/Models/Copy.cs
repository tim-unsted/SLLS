using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using slls.Localization;

namespace slls.Models
{
    [Table("Copies")]
    public partial class Copy
    {
        public Copy()
        {
            //Circulations = new HashSet<Circulation>();
            //PartsReceived = new HashSet<PartsReceived>();
            //Volumes = new HashSet<Volume>();
            //VolumesWithLoans = new HashSet<vwVolumesWithLoans>();
            //CancelledByUser = new ApplicationUser();
            //ItemsOnLoan = new HashSet<vwItemsOnLoan>();
        }

        public int CopyID { get; set; }

        public int TitleID { get; set; }

        [LocalDisplayName("Copies.Copy_Number", "FieldDisplayName")]
        public int CopyNumber { get; set; }

        [StringLength(50)]
        [LocalDisplayName("Copies.Acquisitions_No", "FieldDisplayName")]
        public string AcquisitionsNo { get; set; }

        [LocalDisplayName("Copies.Location", "FieldDisplayName")]
        public int? LocationID { get; set; }

        [LocalDisplayName("Copies.Status", "FieldDisplayName")]
        public int? StatusID { get; set; }

        [LocalDisplayName("Titles.New_Titles", "FieldDisplayName")]
        public bool AcquisitionsList { get; set; }

        [LocalDisplayName("Copies.Print_Label", "FieldDisplayName")]
        public bool PrintLabel { get; set; }

        [LocalDisplayName("Copies.Holdings", "FieldDisplayName")]
        [DataType(DataType.MultilineText)]
        public string Holdings { get; set; }

        [LocalDisplayName("Copies.Bind", "FieldDisplayName")]
        public bool Bind { get; set; }

        [Column(TypeName = "smalldatetime")]
        [LocalDisplayName("Copies.Date_Commenced", "FieldDisplayName")]
        public DateTime? Commenced { get; set; }

        [NotMapped]
        public string CommencedSortable
        {
            get { return string.Format("{0:yyyy-MM-dd HH:mm:ss}", Commenced); }
        }
        
        [Column(TypeName = "smalldatetime")]
        public DateTime? Cancellation { get; set; }

        [LocalDisplayName("Copies.Cancelled_Account_Year", "FieldDisplayName")]
        public int? AccountYearID { get; set; }

        [Column(TypeName = "money")]
        [LocalDisplayName("Copies.Amount_Saved", "FieldDisplayName")]
        public decimal? Saving { get; set; }

        [LocalDisplayName("Copies.Notes", "FieldDisplayName")]
        public string Notes { get; set; }

        [LocalDisplayName("Copies.Is_On_Loan", "FieldDisplayName")]
        public bool OnLoan { get; set; }

        [LocalDisplayName("Copies.Is_Circulated", "FieldDisplayName")]
        public bool Circulated { get; set; }

        [LocalDisplayName("Copies.Is_StandingOrder", "FieldDisplayName")]
        public bool StandingOrder { get; set; }

        public int? CirculationMsgID { get; set; }

        [Column(TypeName = "smalldatetime")]
        [LocalDisplayName("Copies.Date_Added_To_New_Titles_List", "FieldDisplayName")]
        public DateTime? AddedToAcquisitions { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? InputDate { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? LastModified { get; set; }

        [DisplayName("Deleted?")]
        public bool Deleted { get; set; }

        [NotMapped]
        public string CopyLocationStatus
        {
            get
            {
                var copyLocationStatus = string.Format("Copy {0} - {1} {2}", CopyNumber, string.IsNullOrEmpty(Location.Location1) ? "" : Location.ParentLocation.Location1 + ": " + Location.Location1, StatusID == null ? "" : " - " + StatusType.Status);
                return copyLocationStatus.Trim();
            }
        }

        [NotMapped]
        public bool Opac
        {
            get { return StatusType.Opac && Deleted == false && Volumes.Any(); }
        }

        [NotMapped]
        public DateTime? NextPartExpected { get; set; }

        [NotMapped]
        public string NextPartExpectedSortable
        {
            get { return string.Format("{0:yyyy-MM-dd}", NextPartExpected); }
        }


        [Column(TypeName = "timestamp")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [MaxLength(8)]
        public byte[] RowVersion { get; set; }
        
        public virtual AccountYear AccountYear { get; set; }

        public virtual ICollection<Circulation> Circulations { get; set; }

        public virtual ICollection<PartsReceived> PartsReceived { get; set; }

        public virtual CirculationMessage CirculationMessage { get; set; }

        public virtual Location Location { get; set; }

        public virtual StatusType StatusType { get; set; }

        public virtual Title Title { get; set; }

        public virtual ICollection<Volume> Volumes { get; set; }

        public virtual ICollection<vwVolumesWithLoans> VolumesWithLoans { get; set; }

        public virtual ApplicationUser CancelledByUser { get; set; }
        
    }
}
