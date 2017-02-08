using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using slls.Models;

namespace slls.Controllers
{
    public class FilesController : sllsBaseController
    {
        public static int DownloadImageFromUrl(string url = "")
        {
            using (var db = new DbEntities())
            {
                using (var client = new WebClient())
                {
                    try
                    {
                        var image = client.DownloadData(url);

                        var img = new CoverImage
                        {
                            Image = image,
                            Name = url,
                            Source = url,
                            Size = image.Length,
                            Type = image.GetType().ToString(),
                            InputDate = DateTime.Now
                        };

                        db.Images.Add(img);
                        db.SaveChanges();

                        return img.ImageId;
                    }
                    catch (Exception e)
                    {
                        return 0;
                    }
                }
            }
        }

        public static int UploadImage(Stream fileStream, string name, string type, string ext, string source)
        {
            using (var db = new DbEntities())
            {
                try
                {
                    var existingFile = db.Images.FirstOrDefault(f => f.Source == source);
                    if (existingFile != null)
                    {
                        return existingFile.ImageId;
                    }
                    else
                    {
                        var compressedBytes = new byte[fileStream.Length];

                        //Check the name of the file - append [n] for each duplicate ...
                        var i = 1;
                        var existingFileName = db.Images.FirstOrDefault(f => f.Source == name);
                        while (existingFileName != null)
                        {
                            name = name.Replace(ext, "");
                            name = name + "[" + i + "]";
                            name = name + ext;
                            existingFileName = db.Images.FirstOrDefault(f => f.Source == name);
                            i++;
                        }

                        //Save the file (binary data) to the database ...
                        fileStream.Read(compressedBytes, 0, compressedBytes.Length);
                        var image = new CoverImage()
                        {
                            Image = compressedBytes,
                            Source = source,
                            Name = name,
                            Type = type,
                            Size = compressedBytes.Length / 1024,
                            InputDate = DateTime.Now
                        };
                        db.Images.Add(image);
                        db.SaveChanges();

                        //Get the Id of the newly inserted file ...
                        return image.ImageId;
                    }
                }
                catch (Exception e)
                {
                    return 0;
                }
            }
        }

        public static int UploadFile(Stream fileStream, string name, string type, string ext, string path)
        {
            try
            {
                using (var db = new DbEntities())
                {
                    var existingFile = db.HostedFiles.FirstOrDefault(f => f.Path == path);
                    if (existingFile != null)
                    {
                        return existingFile.FileId;
                    }
                    else
                    {
                        var compressedBytes = Compress(fileStream);
                        bool compressed;
                        if (compressedBytes != null)
                        {
                            compressed = true;
                        }
                        else
                        {
                            compressedBytes = new byte[fileStream.Length];
                            compressed = false;
                        }

                        //Check the name of the file - append [n] for each duplicate ...
                        var i = 1;
                        var existingFileName = db.HostedFiles.FirstOrDefault(f => f.FileName == name);
                        while (existingFileName != null)
                        {
                            name = name.Replace(ext, "");
                            name = name + "[" + i + "]";
                            name = name + ext;
                            existingFileName = db.HostedFiles.FirstOrDefault(f => f.FileName == name);
                            i++;
                        }

                        //Save the file (binary data) to the database ...
                        fileStream.Read(compressedBytes, 0, compressedBytes.Length);
                        var file = new HostedFile
                        {
                            Data = compressedBytes,
                            FileName = name,
                            FileExtension = ext,
                            Compressed = compressed,
                            SizeStored = compressedBytes.Length / 1024,
                            Path = path,
                            InputDate = DateTime.Now
                        };
                        db.HostedFiles.Add(file);
                        db.SaveChanges();

                        //Get the Id of the newly inserted file ...
                        var fileId = file.FileId;
                        return fileId;
                    }
                }
            }
            catch (Exception e)
            {
                return 0;
            }
        }

        public static byte[] Compress(Stream input)
        {
            try
            {
                using (var compressStream = new MemoryStream())
                using (var compressor = new DeflateStream(compressStream, CompressionMode.Compress))
                {
                    input.CopyTo(compressor);
                    compressor.Close();
                    return compressStream.ToArray();
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }

        private static MemoryStream Decompress(byte[] input)
        {
            var output = new MemoryStream();

            using (var compressStream = new MemoryStream(input))
            using (var decompressor = new DeflateStream(compressStream, CompressionMode.Decompress))
                decompressor.CopyTo(output);

            output.Position = 0;
            return output;
        }


        public FileContentResult FileDownload(int id)
        {
            using (var db = new DbEntities())
            {
                //using LINQ expression to get record from database for given id (FileID) value
                var file = (from p in db.HostedFiles
                    where p.FileId == id
                    select p).First();

                //only one record will be returned from database as expression uses condition on primary field
                //so get first record from returned values and retrive file content (binary) and filename 
                var fileBytes = (byte[]) file.Data.ToArray();

                //var memoryStream = new MemoryStream();

                var memoryStream = file.Compressed ? Decompress(fileBytes) : new MemoryStream(fileBytes);

                var fileName = file.FileName;
                var contentType = MimeMapping.GetMimeMapping(fileName);

                //Finally, return file and provide byte file content and file name
                return File(memoryStream.GetBuffer(), contentType, fileName);
            }
        }

        public ActionResult RenderImage(int? id)
        {
            using (var db = new DbEntities())
            {
                var coverImage = db.Images.Find(id);
                var buffer = coverImage.Image;
                return File(buffer, "image/jpg", string.Format("{0}.jpg", id));
            }
        }

        [OutputCache(Duration = 3600, VaryByParam = "imageId")]
        public ActionResult RenderSiteLogo(int imageId = 0)
        {
            using (var db = new DbEntities())
            {
                var siteLogo = db.Images.Find(imageId);
                var buffer = siteLogo.Image;
                return File(buffer, "image/jpg", string.Format("{0}.jpg", imageId));
            }
        }
    }
}