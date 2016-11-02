using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml;
using AutoCat.ViewModels;

namespace AutoCat.Wildys
{
    public class WildysWebServices
    {
        public string GetImageUrl(string isbn = "")
        {
            try
            {
                var doc = GetXmlDocument(isbn);

                if (doc == null)
                {
                    return null;
                }

                var myXmlNode = doc.SelectSingleNode("//product/thumbnail");
                if (myXmlNode != null)
                {
                    var imageurl = myXmlNode.InnerText;
                    return imageurl.Replace("_sml", "");
                }
                return null;
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

            var wildysBaseUrl = ConfigurationManager.AppSettings["WildysUrl"] ?? "http://www.wildy.com/isbn/";
            var wildysToken = ConfigurationManager.AppSettings["WildysToken"] ?? "3E6W2A2M";
            var wildysXmlUrl = wildysBaseUrl + isbn + ".xml?id=" + wildysToken;

            try
            {
                WebRequest request = WebRequest.Create(wildysXmlUrl);
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
            var doc = GetXmlDocument(isbn);

            if (doc == null)
            {
                return null;
            }

            var viewModel = new AutoCatNewTitle();

            var myXmlNode = doc.SelectSingleNode("//product/title");
            if (myXmlNode != null) viewModel.Title = myXmlNode.InnerText;

            myXmlNode = doc.SelectSingleNode("//product/isbn13");
            if (myXmlNode != null) viewModel.ISBN13 = myXmlNode.InnerText;

            myXmlNode = doc.SelectSingleNode("//product/isbn10");
            if (myXmlNode != null) viewModel.ISBN10 = myXmlNode.InnerText;

            myXmlNode = doc.SelectSingleNode("//product/edition");
            if (myXmlNode != null) viewModel.Edition = myXmlNode.InnerText;

            myXmlNode = doc.SelectSingleNode("//product/language");
            if (myXmlNode != null) viewModel.Language = myXmlNode.InnerText;

            myXmlNode = doc.SelectSingleNode("//product/publisher");
            if (myXmlNode != null) viewModel.Publisher = myXmlNode.InnerText;

            myXmlNode = doc.SelectSingleNode("//product/country_of_publication");
            if (myXmlNode != null) viewModel.PlaceofPublication = myXmlNode.InnerText;

            myXmlNode = doc.SelectSingleNode("//product/format");
            if (myXmlNode != null) viewModel.Media = myXmlNode.InnerText;

            myXmlNode = doc.SelectSingleNode("//product/series");
            if (myXmlNode != null) viewModel.Series = myXmlNode.InnerText;
            
            myXmlNode = doc.SelectSingleNode("//product/description");
            if (myXmlNode != null) viewModel.Description = myXmlNode.InnerText;

            myXmlNode = doc.SelectSingleNode("//product/contents");
            if (myXmlNode != null) viewModel.Contents = myXmlNode.InnerText;

            myXmlNode = doc.SelectSingleNode("//product/authors");
            if (myXmlNode != null)
            {
                viewModel.Author = myXmlNode.InnerText.Split(';').ToList();
            }

            myXmlNode = doc.SelectSingleNode("//product/subjects");
            if (myXmlNode != null)
            {
                var pattern = ";";
                var input = myXmlNode.InnerText.Replace("&amp;", "&");

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

            myXmlNode = doc.SelectSingleNode("//product/thumbnail");
            if (myXmlNode != null)
            {
                var imageurl = myXmlNode.InnerText;
                viewModel.ImageUrl = imageurl.Replace("_sml", "");
            }


            
            //viewModel.ImageUrl = url.IndexOf("Error", StringComparison.Ordinal) == -1 ? url : null;

            return viewModel;


        }
    }
}