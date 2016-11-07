using System;
using System.IO;
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

            //var url = "https://www.googleapis.com/books/v1/volumes?q=isbn:" + isbn;
            
            //var response =  UrlFetchApp.fetch(url);
            //var results = JSON.parse(response);

            if (doc.HasValues)
            {
                var viewModel = new AutoCatNewTitle();
                
                var book = doc.GetValue("items");
                var title = book[0]["volumeInfo"]["title"].ToString();  //.(0).Item("volumeInfo").Item("title").ToString();
                var subTitle = book[0]["volumeInfo"]["subtitle"].ToString();
                subTitle = string.IsNullOrEmpty(subTitle) ? null : " : " + subTitle;
                viewModel.Title = title + subTitle;
                viewModel.Year = book[0]["volumeInfo"]["publishedDate"].ToString();
                viewModel.Media = book[0]["volumeInfo"]["printType"].ToString();
                viewModel.Description = book[0]["volumeInfo"]["description"].ToString();
                viewModel.Language = book[0]["volumeInfo"]["language"].ToString();

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

            var baseUrl = "https://www.googleapis.com/books/v1/volumes?q=isbn:"; //ConfigurationManager.AppSettings["GoogleBooksUrl"];
            var googleBooksUrl = baseUrl + isbn;

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