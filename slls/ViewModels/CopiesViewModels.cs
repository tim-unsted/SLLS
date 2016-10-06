using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using slls.Localization;
using slls.Models;

namespace slls.ViewModels
{
    public class CopiesListViewModel
    {
        public int CopyID { get; set; }
        public int CopyNumber { get; set; }
        public string Location { get; set; }
        public string Status { get; set; }
        public string Holdings { get; set; }
        public List<Volume> Volumes { get; set; }
        public List<string> BarcodeLinks { get; set; }
        public string BarcodeString { get; set; }
    }
    
    public class CopiesAddViewModel
    {
        [LocalDisplayName("Titles.Title", "FieldDisplayName")]
        [Required(ErrorMessage = "Please select a Title!")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a Title!")]
        public int TitleID { get; set; }

        [LocalDisplayName("Titles.Title", "FieldDisplayName")]
        public string Title { get; set; }

        [LocalDisplayName("Copies.Copy_Number", "FieldDisplayName")]
        [Required(ErrorMessage = "Please provide a Copy Number")]
        //[Remote("CopyNumberExists", "Copies", HttpMethod = "POST", ErrorMessage = "This copy number is already in use for this title!")]
        public int CopyNumber { get; set; }

        [StringLength(50)]
        [LocalDisplayName("Copies.Acquisitions_No", "FieldDisplayName")]
        public string AcquisitionsNo { get; set; }

        public int? LocationId { get; set; }
        public int? StatusId { get; set; }

        [LocalDisplayName("Titles.New_Titles", "FieldDisplayName")]
        public bool AcquisitionsList { get; set; }

        [LocalDisplayName("Copies.Print_Label", "FieldDisplayName")]
        public bool PrintLabel { get; set; }

        [LocalDisplayName("Copies.Bind", "FieldDisplayName")]
        public bool Bind { get; set; }

        [LocalDisplayName("Copies.Holdings", "FieldDisplayName")]
        [DataType(DataType.MultilineText)]
        public string Holdings { get; set; }

        [LocalDisplayName("Copies.Notes", "FieldDisplayName")]
        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }

        public int Step { get; set; }

        //[StringLength(255)]
        ////[Required]
        //[LocalDisplayName("CopyItems.Label_Text", "FieldDisplayName")]
        //public string LabelText { get; set; }

        //[StringLength(50)]
        ////[Required]
        //[LocalDisplayName("CopyItems.Barcode", "FieldDisplayName")]
        //public string Barcode { get; set; }
        
        //[LocalDisplayName("CopyItems.Is_Ref_Only", "FieldDisplayName")]
        //public bool RefOnly { get; set; }

    }

    public class CopyDetailsEditViewModel
    {
        public int CopyId { get; set; }
        
        public int TitleId { get; set; }

        [LocalDisplayName("Titles.Title", "FieldDisplayName")]
        [StringLength(450)]
        public string Title { get; set; }
        
        [LocalDisplayName("Copies.Copy_Number", "FieldDisplayName")]
        public int CopyNumber { get; set; }

        [StringLength(50)]
        [LocalDisplayName("Copies.Acquisitions_No", "FieldDisplayName")]
        public string AcquisitionsNo { get; set; }

        [LocalDisplayName("Locations.Location", "FieldDisplayName")]
        public int? LocationId { get; set; }

        [LocalDisplayName("Copies.Status", "FieldDisplayName")]
        public int? StatusId { get; set; }

        [LocalDisplayName("Titles.New_Titles", "FieldDisplayName")]
        public bool AcquisitionsList { get; set; }

        [LocalDisplayName("Copies.Print_Label", "FieldDisplayName")]
        public bool PrintLabel { get; set; }

        [LocalDisplayName("Copies.Holdings", "FieldDisplayName")]
        [DataType(DataType.MultilineText)]
        public string Holdings { get; set; }

        [LocalDisplayName("Copies.Bind", "FieldDisplayName")]
        public bool Bind { get; set; }

        //[Column(TypeName = "smalldatetime")]
        [LocalDisplayName("Copies.Date_Commenced", "FieldDisplayName")]
        public DateTime? Commenced { get; set; }

        [LocalDisplayName("Copies.Cancelled_By", "FieldDisplayName")]
        public virtual ApplicationUser CancelledByUser { get; set; }

        //[Column(TypeName = "smalldatetime")]
        [LocalDisplayName("Copies.Date_Cancelled", "FieldDisplayName")]
        public DateTime? Cancellation { get; set; }

        [LocalDisplayName("Copies.Cancelled_Account_Year", "FieldDisplayName")]
        public int? AccountYearId { get; set; }

        [Column(TypeName = "money")]
        [LocalDisplayName("Copies.Amount_Saved", "FieldDisplayName")]
        public decimal? Saving { get; set; }

        [LocalDisplayName("Copies.Notes", "FieldDisplayName")]
        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }

        [LocalDisplayName("Copies.Is_Circulated", "FieldDisplayName")]
        public bool Circulated { get; set; }

        [LocalDisplayName("Copies.Date_Added_To_New_Titles_List", "FieldDisplayName")]
        public DateTime? AddedToAcquisitions { get; set; }

        [LocalDisplayName("Circulation.Circulation_Slip_Message", "FieldDisplayName")]
        public int? CirculationMsgId { get; set; }

        public virtual ICollection<Circulation> Circulations { get; set; }

        public virtual CirculationMessage CirculationMessage { get; set; }
        
    }

    public class AddToBindingViewModel
    {
        public IEnumerable<int> SelectedCopies { get; set; }

        [LocalDisplayName("Copies.Copies_to_bind", "FieldDisplayName")]
        public IEnumerable<SelectListItem> CopiesToBind { get; set; }
    }

    public class CopySummaryViewModel
    {
        [Key]
        public int CopyId { get; set; }

        public int TitleId { get; set; }

        [LocalDisplayName("Titles.Title", "FieldDisplayName")]
        [StringLength(450)]
        public string Title { get; set; }

        [LocalDisplayName("Copies.Copy_Number", "FieldDisplayName")]
        public int CopyNumber { get; set; }

        [LocalDisplayName("MediaTypes.Media_Type", "FieldDisplayName")]
        public string Media { get; set; }

        [LocalDisplayName("Locations.Location", "FieldDisplayName")]
        public string Location { get; set; }

        [LocalDisplayName("StatusTypes.Status_Type", "FieldDisplayName")]
        public string Status { get; set; }

        [LocalDisplayName("Circulation.Circulation_Slip_Message", "FieldDisplayName")]
        public int? CirculationMsgID { get; set; }

        [LocalDisplayName("Copies.Holdings", "FieldDisplayName")]
        [DataType(DataType.MultilineText)]
        public string Holdings { get; set; }

        [LocalDisplayName("Copies.Notes", "FieldDisplayName")]
        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }

    }

    public class EditHoldingsNotesViewModel
    {
        [Key]
        public int CopyId { get; set; }

        [LocalDisplayName("Titles.Title", "FieldDisplayName")]
        public string Title { get; set; }

        [LocalDisplayName("Copies.Copy_Number", "FieldDisplayName")]
        public int CopyNumber { get; set; }

        [LocalDisplayName("Copies.Holdings", "FieldDisplayName")]
        [DataType(DataType.MultilineText)]
        public string Holdings { get; set; }

        [LocalDisplayName("Copies.Notes", "FieldDisplayName")]
        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }
    }
 
}