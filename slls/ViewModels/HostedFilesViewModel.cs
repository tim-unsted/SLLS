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
    public class HostedFilesViewModel
    {
        [Key]
        public int FileId { get; set; }

        [LocalDisplayName("Hosted_Files.FileName", "FieldDisplayName")]
        public string FileName { get; set; }

        [LocalDisplayName("Hosted_Files.Content_Type", "FieldDisplayName")]
        public string ContentType { get; set; }

        [LocalDisplayName("Hosted_Files.Extension", "FieldDisplayName")]
        public string FileExtension { get; set; }

        [LocalDisplayName("Hosted_Files.Original_Path", "FieldDisplayName")]
        public string Path { get; set; }

        [DisplayName("Data")]
        public byte[] Data { get; set; }

        [LocalDisplayName("Hosted_Files.Compressed", "FieldDisplayName")]
        public bool Compressed { get; set; }

        [LocalDisplayName("Hosted_Files.Date_Uploaded", "FieldDisplayName")]
        [Column(TypeName = "smalldatetime")]
        public DateTime? InputDate { get; set; }

        [LocalDisplayName("Hosted_Files.File_Created", "FieldDisplayName")]
        public DateTime? CreateDate { get; set; }

        [LocalDisplayName("Hosted_Files.File_Last_Updated", "FieldDisplayName")]
        public DateTime? LastUpdateDate { get; set; }

        [LocalDisplayName("TitleLinks", "EntityType")]
        public int TitleLinks { get; set; }
    }

    public class UploadFileViewModel
    {
        public HttpPostedFileBase File { get; set; }

        public IEnumerable<HttpPostedFileBase> Files { get; set; }

    }
}