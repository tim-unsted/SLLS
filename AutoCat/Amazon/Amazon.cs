using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Xml;
using AutoCat.ViewModels;

namespace AutoCat.Amazon
{
    public class AmazonWebServices
    {
        //private const string MY_AWS_ACCESS_KEY_ID = "AKIAJZIAIBEOY25NRGIQ";
        //private const string MY_AWS_SECRET_KEY = "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";

        public string GetRequestUrl(string isbn = "")
        {
            var awsDestination = ConfigurationManager.AppSettings["AmazonAwsDestination"];
            var awsAccessKey = ConfigurationManager.AppSettings["AmazonAwsAccessKeyID"];
            var awsSecretKey = ConfigurationManager.AppSettings["AmazonAwsSecretKey"];
            var awsService = ConfigurationManager.AppSettings["AmazonAwsService"];
            var awsVersion = ConfigurationManager.AppSettings["AmazonAwsVersion"];
            var awsAssociateTag = ConfigurationManager.AppSettings["AmazonAwsAssociateTag"];
            var awsOperation = ConfigurationManager.AppSettings["AmazonAwsOperation"];
            var awsIdType = ConfigurationManager.AppSettings["AmazonAwsIdType"];
            var awsResponseGroup = ConfigurationManager.AppSettings["AmazonAwsResponseGroup"];
            var awsSearchIndex = ConfigurationManager.AppSettings["AmazonAwsSearchIndex"];

            var helper = new SignedRequestHelper(awsAccessKey, awsSecretKey, awsDestination);

            /*** Request is stored as a dictionary .... ***/
            IDictionary<string, string> r1 = new Dictionary<string, string>();
            r1["Service"] = awsService;
            r1["Version"] = awsVersion;
            r1["AssociateTag"] = awsAssociateTag;
            r1["Operation"] = awsOperation;
            r1["ItemId"] = isbn;
            r1["IdType"] = awsIdType;
            r1["ResponseGroup"] = awsResponseGroup;
            r1["SearchIndex"] = awsSearchIndex;

            return helper.Sign(r1);
        }

        public XmlDocument GetXmlDocument(string isbn = "")
        {
            if (string.IsNullOrEmpty(isbn))
            {
                return null;
            }

            var requestUrl = GetRequestUrl(isbn);

            if (requestUrl == null)
            {
                return null;
            }

            try
            {
                ////log the url so we can try to troubleshoot errors...
                //var filename = AppDomain.CurrentDomain.BaseDirectory + "Logs\\" + "logErrors.txt";
                //var sw = new System.IO.StreamWriter(filename, true);
                //sw.WriteLine(DateTime.Now.ToString() + " " + requestUrl);
                //sw.Close();

                WebRequest request = WebRequest.Create(requestUrl);
                WebResponse response = request.GetResponse();
                XmlDocument doc = new XmlDocument();
                doc.Load(response.GetResponseStream());
                return doc;
            }
            catch (Exception e)
            {
                return null;
            }
            return null;
        }

        public string GetImageUrl(string isbn = "")
        {
            if (string.IsNullOrEmpty(isbn))
            {
                return null;
            }

            var awsNamespace = ConfigurationManager.AppSettings["AmazonAwsNamespace"] ?? "http://webservices.amazon.com/AWSECommerceService/2011-08-01";
            var awsImageSize = ConfigurationManager.AppSettings["AmazonAwsImageSize"] ?? "MediumImage";

            XmlDocument doc = GetXmlDocument(isbn);

            try
            {
                var errorMessageNodes = doc.GetElementsByTagName("Message");
                if (errorMessageNodes.Count > 0)
                {
                    //var xmlNode = errorMessageNodes.Item(0);
                    //if (xmlNode != null)
                    //{
                    //    string message = xmlNode.InnerText;
                    //    return "Error: " + message + " (but signature worked)";
                    //}
                    return null;
                }

                XmlNamespaceManager ns = new XmlNamespaceManager(doc.NameTable);
                ns.AddNamespace("aws", awsNamespace);

                XmlNode imageNode = doc.GetElementsByTagName(awsImageSize).Item(0);

                if (imageNode == null) return null;
                var selectSingleNode = imageNode.SelectSingleNode("aws:URL", ns);
                if (selectSingleNode != null)
                {
                    string imageUrl = selectSingleNode.InnerText;
                    return imageUrl;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                return null;
            }
            return null;
        }


        public AutoCatNewTitle GetFieldData(string isbn = "")
        {
            if (string.IsNullOrEmpty(isbn))
            {
                return null;
            }

            XmlNode myXmlChildNode;
            XmlDocument doc = GetXmlDocument(isbn);
            var awsNamespace = ConfigurationManager.AppSettings["AmazonAwsNamespace"];
            var awsImageSize = ConfigurationManager.AppSettings["AmazonAwsImageSize"];
            var viewModel = new AutoCatNewTitle();

            try
            {
                var errorMessageNodes = doc.GetElementsByTagName("Message");
                if (errorMessageNodes.Count > 0)
                {
                    var xmlNode = errorMessageNodes.Item(0);
                    if (xmlNode != null)
                    {
                        string message = xmlNode.InnerText;
                        viewModel.ErrorMessage = "Error: " + message + " (but signature worked)";
                        return viewModel;
                    }
                }

                XmlNamespaceManager nsm = new XmlNamespaceManager(doc.NameTable);
                nsm.AddNamespace("aws", awsNamespace);

                var myXmlNodeList = doc.SelectNodes("aws:ItemLookupResponse/aws:Items/aws:Item", nsm);

                if (myXmlNodeList.Count == 0)
                {
                    viewModel.ErrorMessage = "Item not found.";
                }
                else
                {       
                    foreach (XmlNode itemXmlNode in myXmlNodeList)
                    {
                        var myXmlNode = itemXmlNode.SelectSingleNode("aws:ItemAttributes/aws:Title", nsm);
                        if (myXmlNode != null) viewModel.Title = myXmlNode.InnerText;

                        myXmlNode = itemXmlNode.SelectSingleNode("aws:ItemAttributes/aws:Edition", nsm);
                        if (myXmlNode != null) viewModel.Edition = myXmlNode.InnerText;

                        myXmlNode = itemXmlNode.SelectSingleNode("aws:ItemAttributes/aws:Languages/aws:Language/aws:Name", nsm);
                        if (myXmlNode != null) viewModel.Language = myXmlNode.InnerText;

                        var myXmlChildNodeList = itemXmlNode.SelectNodes("aws:ItemAttributes/aws:Author", nsm);
                        if (myXmlChildNodeList != null)
                        {
                            viewModel.Author = new List<string>();
                            foreach (XmlNode ChildNode in myXmlChildNodeList)
                            {
                                if (ChildNode != null)
                                {

                                    viewModel.Author.Add(ChildNode.InnerText);
                                }
                            }
                        }

                        myXmlNode = itemXmlNode.SelectSingleNode("aws:ItemAttributes/aws:Publisher", nsm);
                        if (myXmlNode != null) viewModel.Publisher = myXmlNode.InnerText;

                        myXmlNode = itemXmlNode.SelectSingleNode("aws:ItemAttributes/aws:PublicationDate", nsm);
                        if (myXmlNode != null)
                        {
                            var pubDate = myXmlNode.InnerText;
                            viewModel.PublicationDate = pubDate;
                            var arrPubDate = new string[3];

                            if(pubDate.IndexOf("/", StringComparison.Ordinal) != -1)
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
                            viewModel.Year = arrPubDate[0];
                        }

                        myXmlNode = itemXmlNode.SelectSingleNode("aws:ItemAttributes/aws:Series", nsm);
                        if (myXmlNode != null) viewModel.Series = myXmlNode.InnerText;

                        myXmlNode = itemXmlNode.SelectSingleNode("aws:ItemAttributes/aws:Binding", nsm);
                        if (myXmlNode != null) viewModel.Description = myXmlNode.InnerText;

                        myXmlNode = itemXmlNode.SelectSingleNode("aws:ItemAttributes/aws:ISBN", nsm);
                        if (myXmlNode != null) viewModel.ISBN10 = myXmlNode.InnerText;

                        myXmlNode = itemXmlNode.SelectSingleNode("aws:ItemAttributes/aws:EAN", nsm);
                        if (myXmlNode != null) viewModel.ISBN13 = myXmlNode.InnerText;
                    }

                    //Finally, get the image Url ...
                    var imageNode = doc.GetElementsByTagName(awsImageSize).Item(0);
                    if (imageNode != null)
                    {
                        var selectSingleNode = imageNode.SelectSingleNode("aws:URL", nsm);
                        if (selectSingleNode != null)
                        {
                            viewModel.ImageUrl = selectSingleNode.InnerText;
                        }
                    }

                    return viewModel;
                }
                return null;
            }
            catch (Exception e)
            {
                return null;
            }
            return null;
        }
    }
}