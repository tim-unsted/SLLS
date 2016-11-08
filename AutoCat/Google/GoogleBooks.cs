using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using AutoCat.ViewModels;
using Newtonsoft.Json.Linq;

namespace AutoCat.Google
{
    public class GoogleBooks
    {
        public AutoCatNewTitle GetFieldData(string isbn = "")
        {
            if (string.IsNullOrEmpty(isbn))
            {
                return null;
            }

            var doc = GetJsonObject(isbn);

            if (doc == null)
            {
                return null;
            }
            
            if (doc.HasValues)
            {
                var viewModel = new AutoCatNewTitle();
                
                var bookDetails = doc.GetValue("items");

                if (bookDetails == null)
                {
                    return null;
                }

                string title = "";
                string subTitle = "";

                JToken token = bookDetails[0]["volumeInfo"]["title"];
                if (token != null) title = token.ToString();

                token = bookDetails[0]["volumeInfo"]["subtitle"];
                if (token != null)
                {
                    subTitle = token.ToString();
                    subTitle = string.IsNullOrEmpty(subTitle) ? null : " : " + subTitle;
                }
                
                viewModel.Title = title + subTitle;

                //Get just the year from the published date ...
                token = bookDetails[0]["volumeInfo"]["publishedDate"];
                if (token != null)
                {
                    var pubDate = token.ToString();
                    var dateParts = pubDate.Split('-');
                    viewModel.Year = dateParts[0];
                }

                token = bookDetails[0]["volumeInfo"]["printType"];
                if (token != null) viewModel.Media = token.ToString();

                token = bookDetails[0]["volumeInfo"]["description"];
                if (token != null) viewModel.Description = token.ToString();

                token = bookDetails[0]["volumeInfo"]["language"];
                if (token != null) viewModel.Language = token.ToString();

                token = bookDetails[0]["volumeInfo"]["publisher"];
                if (token != null) viewModel.Publisher = token.ToString();

                token = bookDetails[0]["volumeInfo"]["publisher"];
                if (token != null) viewModel.Publisher = token.ToString();

                token = bookDetails[0]["volumeInfo"]["authors"];
                if (token != null)
                {
                    JArray authors = (JArray)token;
                    var authorList = new List<String>();
                    foreach (var author in authors)
                    {
                        authorList.Add(author.ToString());
                    }
                    viewModel.Author = authorList;
                }

                //Get image link ...
                token = bookDetails[0]["volumeInfo"]["imageLinks"];
                if (token != null)
                {
                    var imageToken = bookDetails[0]["volumeInfo"]["imageLinks"]["small"];
                    if (imageToken == null)
                    {
                        imageToken = bookDetails[0]["volumeInfo"]["imageLinks"]["thumbnail"];
                    }
                    if (imageToken == null)
                    {
                        imageToken = bookDetails[0]["volumeInfo"]["imageLinks"]["medium"];
                    }
                    if (imageToken == null)
                    {
                        imageToken = bookDetails[0]["volumeInfo"]["imageLinks"]["large"];
                    }
                    if (imageToken != null) viewModel.ImageUrl = imageToken.ToString();
                }
                

                //Get ISBN and ISBN-13
                token = bookDetails[0]["volumeInfo"]["industryIdentifiers"];
                if (token != null)
                {
                    JArray identifiers = (JArray)token;
                    foreach (var identifier in identifiers)
                    {
                        var identifierType = identifier["type"];
                        if (identifierType.ToString() == "ISBN_13")
                        {
                            viewModel.ISBN13 = identifier["identifier"].ToString();
                        }
                        if (identifierType.ToString() == "ISBN_10")
                        {
                            viewModel.ISBN10 = identifier["identifier"].ToString();
                        }
                        if (identifierType.ToString() == "ISSN")
                        {
                            viewModel.ISSN = identifier["identifier"].ToString();
                        }
                    }
                    
                }

                //Get some links ...
                var links = new Dictionary<string,string>();

                token = bookDetails[0]["volumeInfo"]["previewLink"];
                if (token != null) links.Add("Preview", token.ToString());

                token = bookDetails[0]["volumeInfo"]["infoLink"];
                if (token != null) links.Add("More info", token.ToString());

                if (links.Any())
                {
                    viewModel.Links = links;
                }
                
                return viewModel;
            }
            return null;
        }

        public JObject GetJsonObject(string isbn = "")
        {
            if (string.IsNullOrEmpty(isbn))
            {
                return null;
            }

            var baseUrl = ConfigurationManager.AppSettings["GoogleBooksUrl"] ?? "https://www.googleapis.com/books/v1/volumes?q=";
            var apiKey = ConfigurationManager.AppSettings["GoogleApiKey"] ?? "AIzaSyAHbSCwgEgKIBuzPTAW9EdtgNUgnENTSbU";
            var googleBooksUrl = "";

            if (isbn.Length == 8) //Looks for an ISSN ...
            {
                googleBooksUrl = baseUrl + "issn" + isbn + "&projection=full&key=" + apiKey;
            }
            else // Otherwise look for an ISBN ...
            {
                googleBooksUrl = baseUrl + "isbn" + isbn + "&projection=full&key=" + apiKey;
            }
            

            try
            {
                WebRequest request = WebRequest.Create(googleBooksUrl);
                WebResponse response = request.GetResponse();
                StreamReader sreader = new StreamReader(response.GetResponseStream());

                var jsonObject = JObject.Parse(sreader.ReadToEnd());
                return jsonObject;
            }
            catch (Exception e)
            {
                return null;
            }
        }
 
    }
}