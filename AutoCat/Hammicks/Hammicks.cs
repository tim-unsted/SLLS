using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml;
using AutoCat.ViewModels;

namespace AutoCat.Hammicks
{
    public class HammicksWebServices
    {
        public string GetImageUrl(string isbn = "")
        {
            try
            {
                var hammicksImageUrl = ConfigurationManager.AppSettings["HammicksImageUrl"] ?? "http://www.hammickslegal.com/live/ProductImage/";
                var imageUrl = hammicksImageUrl + isbn;

                //Test what we get returned from this URL ...
                using (var client = new WebClient())
                {
                    try
                    {
                        string webpage = client.DownloadString(imageUrl);
                        if (webpage.IndexOf("403", StringComparison.Ordinal) != -1 ||
                            webpage.IndexOf("404", StringComparison.Ordinal) != -1 ||
                            webpage.IndexOf("Error", StringComparison.Ordinal) != -1 ||
                            webpage.IndexOf("Invalid", StringComparison.Ordinal) != -1)
                        {
                            return null;
                        }
                    }
                    catch (WebException ex)
                    {
                        if (ex.Status == WebExceptionStatus.ProtocolError && ex.Response != null)
                        {
                            var resp = (HttpWebResponse) ex.Response;
                            if (resp.StatusCode == HttpStatusCode.NotFound) // HTTP 404
                            {
                                return null;
                            }
                        }
                        //throw any other exception - this should not occur!
                        return null;
                    }
                }

                //Url seems fine, so return it now ...
                return imageUrl;
            }
            catch (Exception)
            {
                return null;
            }
        }


        public XmlDocument GetXmlDocument(string isbn = "")
        {
            if (string.IsNullOrEmpty(isbn))
            {
                return null;
            }

            var baseXmlUrl = ConfigurationManager.AppSettings["HammicksXmlUrl"];
            var hammicksXmlUrl = baseXmlUrl + isbn;

            try
            {
                WebRequest request = WebRequest.Create(hammicksXmlUrl);
                WebResponse response = request.GetResponse();
                XmlDocument doc = new XmlDocument();
                doc.Load(response.GetResponseStream());
                return doc;
            }
            catch (Exception e)
            {
                return null;
            }
        }


        public AutoCatNewTitle GetFieldData(string isbn = "")
        {
            if (string.IsNullOrEmpty(isbn))
            {
                return null;
            }
            
            var doc = GetXmlDocument(isbn);

            if (doc == null)
            {
                return null;
            }

            var viewModel = new AutoCatNewTitle();

            var myXmlNode = doc.SelectSingleNode("//product/title");
            if (myXmlNode != null) viewModel.Title = myXmlNode.InnerText;

            myXmlNode = doc.SelectSingleNode("//product/ean");
            if (myXmlNode != null) viewModel.ISBN13 = myXmlNode.InnerText;

            myXmlNode = doc.SelectSingleNode("//product/isbn10");
            if (myXmlNode != null) viewModel.ISBN10 = myXmlNode.InnerText;

            myXmlNode = doc.SelectSingleNode("//product/edition");
            if (myXmlNode != null) viewModel.Edition = myXmlNode.InnerText;
            
            myXmlNode = doc.SelectSingleNode("//product/publisher");
            if (myXmlNode != null) viewModel.Publisher = myXmlNode.InnerText;

            myXmlNode = doc.SelectSingleNode("//product/country_of_publication");
            if (myXmlNode != null) viewModel.PlaceofPublication = myXmlNode.InnerText;

            myXmlNode = doc.SelectSingleNode("//product/format_text");
            if (myXmlNode != null) viewModel.Media = myXmlNode.InnerText;

            myXmlNode = doc.SelectSingleNode("//product/series");
            if (myXmlNode != null) viewModel.Series = myXmlNode.InnerText;

            myXmlNode = doc.SelectSingleNode("//product/language_text");
            if (myXmlNode != null) viewModel.Language = myXmlNode.InnerText;

            myXmlNode = doc.SelectSingleNode("//product/description");
            if (myXmlNode != null) viewModel.Description = myXmlNode.InnerText;

            myXmlNode = doc.SelectSingleNode("//product/contents");
            if (myXmlNode != null) viewModel.Contents = myXmlNode.InnerText;

            myXmlNode = doc.SelectSingleNode("//product/reviews");
            if (myXmlNode != null) viewModel.Reviews = myXmlNode.InnerText;

            myXmlNode = doc.SelectSingleNode("//product/authors");
            if (myXmlNode != null)
            {
                viewModel.Author = myXmlNode.InnerText.Split(';').ToList();
            }

            myXmlNode = doc.SelectSingleNode("//product/bic_subjects");
            if (myXmlNode != null)
            {
                var pattern = ";";
                var input = myXmlNode.InnerText.Replace("&amp;","&");

                viewModel.Keywords = Regex.Split(input, pattern).ToList();
            }

            myXmlNode = doc.SelectSingleNode("//product/publication_date");
            if (myXmlNode != null)
            {
                var pubDate = myXmlNode.InnerText;
                viewModel.PublicationDate = pubDate;
                var arrPubDate = new string[3];

                if (pubDate.IndexOf("/", StringComparison.Ordinal) != -1)
                {
                    arrPubDate = pubDate.Split('/');
                }
                else if (pubDate.IndexOf("-", StringComparison.Ordinal) != -1)
                {
                    arrPubDate = pubDate.Split('-');
                }
                else if (pubDate.IndexOf(" ", StringComparison.Ordinal) != -1)
                {
                    arrPubDate = pubDate.Split(' ');
                }
                else if (pubDate.IndexOf(".", StringComparison.Ordinal) != -1)
                {
                    arrPubDate = pubDate.Split('.');
                }
                else
                {
                    arrPubDate[0] = pubDate;
                }
                viewModel.Year = arrPubDate[2];
            }

            var url = GetImageUrl(isbn);
            viewModel.ImageUrl = url.IndexOf("Error", StringComparison.Ordinal) == -1 ? url : null;
            
            return viewModel;
        }
    }
}