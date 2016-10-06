using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using slls.Localization;
using slls.Models;

namespace slls.ViewModels
{
    public class PartsReceivedIndexViewModel
    {
        [Key]
        public int PartID { get; set; }

        public int? CopyID { get; set; }

        [LocalDisplayName("Copies.Copy_Number", "FieldDisplayName")]
        public int? CopyNumber { get; set; }
        
        public int? TitleID { get; set; }

        [LocalDisplayName("Titles.Title", "FieldDisplayName")]
        public string Title { get; set; }

        [StringLength(255)]
        [LocalDisplayName("CheckIn.Part_Issue_Desc", "FieldDisplayName")]
        public string PartReceived { get; set; }

        [Column(TypeName = "smalldatetime")]
        [LocalDisplayName("CheckIn.Date_Received", "FieldDisplayName")]
        public DateTime? DateReceived { get; set; }

        [LocalDisplayName("CheckIn.Print_Circulation_List", "FieldDisplayName")]
        public bool PrintList { get; set; }

        [LocalDisplayName("CheckIn.Returned", "FieldDisplayName")]
        public bool Returned { get; set; }

        [DisplayName("Deleted?")]
        public bool Deleted { get; set; }

        public IEnumerable<PartsReceived> PartsReceivedList { get; set; }

        public IEnumerable<Copy> PartsExpectedList { get; set; }
    }

    public class PartsReceivedEditViewModel
    {
        [Key]
        public int PartID { get; set; }
        
        [LocalDisplayName("Copies.Copy_Number", "FieldDisplayName")]
        public int? CopyNumber { get; set; }

        [LocalDisplayName("Titles.Title", "FieldDisplayName")]
        public string Title { get; set; }

        [StringLength(255)]
        [LocalDisplayName("CheckIn.Part_Issue_Desc", "FieldDisplayName")]
        public string PartReceived { get; set; }

        [Column(TypeName = "smalldatetime")]
        [LocalDisplayName("CheckIn.Date_Received", "FieldDisplayName")]
        public DateTime? DateReceived { get; set; }

        [LocalDisplayName("CheckIn.Print_Circulation_List", "FieldDisplayName")]
        public bool PrintList { get; set; }

        [LocalDisplayName("CheckIn.Returned", "FieldDisplayName")]
        public bool Returned { get; set; }
    }

    public class PartsReceivedAddViewModel
    {
        [Key]
        public int PartID { get; set; }

        [LocalDisplayName("Copies.Copy_Number", "FieldDisplayName")]
        public int CopyID { get; set; }
        
        [StringLength(255)]
        [LocalDisplayName("CheckIn.Part_Issue_Desc", "FieldDisplayName")]
        public string PartReceived { get; set; }

        [Column(TypeName = "smalldatetime")]
        [LocalDisplayName("CheckIn.Date_Received", "FieldDisplayName")]
        public DateTime? DateReceived { get; set; }

        [LocalDisplayName("CheckIn.Print_Circulation_List", "FieldDisplayName")]
        public bool PrintList { get; set; }

        [LocalDisplayName("CheckIn.Returned", "FieldDisplayName")]
        public bool Returned { get; set; }
    }

    public class PartsReceivedSubFormViewModel
    {
        [Key]
        public int PartID { get; set; }

        [LocalDisplayName("Copies.Copy_Number", "FieldDisplayName")]
        public int CopyID { get; set; }

        [StringLength(255)]
        [LocalDisplayName("CheckIn.Part_Issue_Desc", "FieldDisplayName")]
        public string PartReceived { get; set; }

        [Column(TypeName = "smalldatetime")]
        [LocalDisplayName("CheckIn.Date_Received", "FieldDisplayName")]
        public DateTime? DateReceived { get; set; }

        [LocalDisplayName("CheckIn.Print_Circulation_List", "FieldDisplayName")]
        public bool PrintList { get; set; }

        public IEnumerable<PartsReceived> PartsReceivedList { get; set; }
    }


    public class PartsOverdueViewModel
    {
        [Key]
        public int PartID { get; set; }

        [LocalDisplayName("Copies.Copy_Number", "FieldDisplayName")]
        public int CopyID { get; set; }

        [LocalDisplayName("Titles.Title", "FieldDisplayName")]
        public string Title { get; set; }

        [LocalDisplayName("Copies.Copy_Number", "FieldDisplayName")]
        public int? CopyNumber { get; set; }

        [StringLength(255)]
        [LocalDisplayName("CheckIn.Part_Issue_Desc", "FieldDisplayName")]
        public string PartReceived { get; set; }

        [Column(TypeName = "smalldatetime")]
        [LocalDisplayName("CheckIn.Date_Received", "FieldDisplayName")]
        public DateTime? DateReceived { get; set; }

        public IEnumerable<Copy> PartsOverdue { get; set; }
    }

}