using System;
using System.Linq;
using System.Net;
using System.Xml;
using AutoCat.ViewModels;

namespace AutoCat.Copac
{
    public class CopacWebServices
    {
        //Get the data for just one item via it's ISBN ...
        public AutoCatNewTitle GetFieldData(string isbn = "")
        {
            if (string.IsNullOrEmpty(isbn))
            {
                return null;
            }

            var searchString = "&isn=" + isbn.Replace((char) 32, (char) 43);

            var doc = GetXmlDocument(searchString);

            if (doc == null)
            {
                return null;
            }

            var autoCatNewTitle = new AutoCatNewTitle();

            var nsmgr = new XmlNamespaceManager(doc.NameTable);
            nsmgr.AddNamespace("loc", "http://www.loc.gov/mods/v3");

            var oNode = doc.SelectSingleNode("//loc:modsCollection", nsmgr);
            if (oNode == null)
            {
                return null;
            }

            var nodeList = oNode.ChildNodes;
            var mod = nodeList[0];

            foreach (XmlElement modElement in mod.ChildNodes)
            {
                if (modElement == null) continue;
                switch (modElement.Name)
                {
                    case "originInfo":
                        {
                            foreach (XmlElement field in modElement.ChildNodes)
                            {
                                if (field == null) continue;
                                switch (field.Name)
                                {
                                    case "edition":
                                        {
                                            autoCatNewTitle.Edition = field.InnerText.Replace("[", "").Replace("]", "");
                                            break;
                                        }
                                    case "publisher":
                                        {
                                            autoCatNewTitle.Publisher = field.InnerText.Replace("[", "").Replace("]", "");
                                            break;
                                        }
                                    case "dateIssued":
                                        {
                                            autoCatNewTitle.Year = field.InnerText.Replace("[", "").Replace("]", "");
                                            break;
                                        }
                                    case "place":
                                        {
                                            autoCatNewTitle.PlaceofPublication = field.InnerText.Replace("[", "").Replace("]", "");
                                            break;
                                        }
                                }
                            }
                            break;
                        }

                    case "titleInfo":
                        {
                            foreach (XmlElement field in modElement.ChildNodes)
                            {
                                switch (field.Name)
                                {
                                    case "title":
                                        {
                                            autoCatNewTitle.Title = field.InnerText;
                                            break;
                                        }
                                }
                            }
                            break;
                        }

                    case "relatedItem":
                        {
                            if (modElement.GetAttribute("type") == "series")
                            {
                                autoCatNewTitle.Series = modElement.ChildNodes[0].InnerText;
                            }
                            break;
                        }

                    case "identifier":
                        {
                            if (modElement.GetAttribute("type") == "isbn")
                            {
                                var tempIsbn = modElement.ChildNodes[0].InnerText;
                                var isbnParts = tempIsbn.Split(' ');
                                var Isbn = isbnParts[0];
                                if (Isbn.Substring(1, 3) == "978")
                                {
                                    autoCatNewTitle.ISBN13 = isbn;
                                }
                                else
                                {
                                    autoCatNewTitle.ISBN10 = isbn;
                                }
                                autoCatNewTitle.Isbn = Isbn; //modElement.ChildNodes[0].InnerText;
                            }
                            break;
                        }

                    case "name":
                        {
                            if (modElement.GetAttribute("type") == "personal")
                            {
                                var authorString = modElement.ChildNodes[0].InnerText;
                                autoCatNewTitle.Author = authorString.Split(';').ToList();
                            }
                            break;
                        }
                }
            }

            return autoCatNewTitle;
        }

        //Get a set of search results for a given set of search criteria ...
        public CopacSearchResults GetSearchResults(string searchCriteria = "")
        {
            //Create something to save our results ...
            var copacSearchResults = new CopacSearchResults();
            
            if (string.IsNullOrEmpty(searchCriteria))
            {
                copacSearchResults.HasErrors = true;
                return copacSearchResults;
            }

            var doc = GetXmlDocument(searchCriteria);

            if (doc == null)
            {
                copacSearchResults.HasErrors = true;
                return copacSearchResults;
            }
            
            var nsmgr = new XmlNamespaceManager(doc.NameTable);
            nsmgr.AddNamespace("loc", "http://www.loc.gov/mods/v3");

            var oNode = doc.SelectSingleNode("//loc:modsCollection", nsmgr);
            if (oNode == null)
            {
                copacSearchResults.HasErrors = true;
                return copacSearchResults;
            }

            var nodeList = oNode.ChildNodes;
            copacSearchResults.ResultsCount = nodeList.Count;

            if (nodeList.Count <= 0) return copacSearchResults;
            if (nodeList.Count > 100) return copacSearchResults;

            var i = 1;

            foreach (XmlElement mod in nodeList)
            {
                if (mod == null) continue;
                var copacRecord = new CopacRecordViewModel(i);
                i++;

                foreach (XmlElement modElement in mod.ChildNodes)
                {
                    if (modElement == null) continue;
                    switch (modElement.Name)
                    {
                        case "originInfo":
                        {
                            foreach (XmlElement field in modElement.ChildNodes)
                            {
                                if (field == null) continue;
                                switch (field.Name)
                                {
                                    case "edition":
                                    {
                                        copacRecord.Edition = field.InnerText.Replace("[", "").Replace("]", "");
                                        break;
                                    }
                                    case "publisher":
                                    {
                                        copacRecord.Publisher = field.InnerText.Replace("[", "").Replace("]", "");
                                        break;
                                    }
                                    case "dateIssued":
                                    {
                                        copacRecord.PubYear = field.InnerText.Replace("[", "").Replace("]", "");
                                        break;
                                    }
                                    case "place":
                                    {
                                        copacRecord.Place = field.InnerText.Replace("[", "").Replace("]", "");
                                        break;
                                    }
                                }
                            }
                            break;
                        }

                        case "titleInfo":
                        {
                            foreach (XmlElement field in modElement.ChildNodes)
                            {
                                switch (field.Name)
                                {
                                    case "title":
                                    {
                                        copacRecord.Title = field.InnerText;
                                        break;
                                    }
                                    case "subTitle":
                                    {
                                        copacRecord.SubTitle = field.InnerText;
                                        break;
                                    }
                                }
                            }
                            break;
                        }

                        case "relatedItem":
                        {
                            if (modElement.GetAttribute("type") == "series")
                            {
                                copacRecord.Series = modElement.ChildNodes[0].InnerText;
                            }
                            break;
                        }

                        case "identifier":
                        {
                            if (modElement.GetAttribute("type") == "isbn")
                            {
                                var tempIsbn = modElement.ChildNodes[0].InnerText;
                                var isbnParts = tempIsbn.Split(' ');
                                var isbn = isbnParts[0];
                                if (isbn.Substring(1, 3) == "978")
                                {
                                    copacRecord.Isbn13 = isbn;
                                }
                                else
                                {
                                    copacRecord.Isbn10 = isbn;
                                }
                                copacRecord.Isbn = isbn; //modElement.ChildNodes[0].InnerText;
                            }
                            break;
                        }

                        case "name":
                        {
                            if (modElement.GetAttribute("type") == "personal")
                            {
                                copacRecord.Author = modElement.ChildNodes[0].InnerText;
                            }
                            break;
                        }
                    }
                }

                copacSearchResults.CopacRecords.Add(copacRecord);
            }

            return copacSearchResults;
        }


        public XmlDocument GetXmlDocument(string searchCriteria = "")
        {
            if (string.IsNullOrEmpty(searchCriteria))
            {
                return null;
            }

            var baseXmlUrl = "http://copac.ac.uk/search?format=XML+-+MODS&sort-order=ti"; //ConfigurationManager.AppSettings["CopacUrl"];
            //var baseXmlUrl = "http://copac.jisc.ac.uk/search?&format=XML+-+MODS&sort-order=ti";
            var copacXmlUrl = baseXmlUrl + searchCriteria;

            try
            {
                WebRequest request = WebRequest.Create(copacXmlUrl);
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
    }
}