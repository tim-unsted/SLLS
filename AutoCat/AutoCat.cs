using System;
using AutoCat.Amazon;
using AutoCat.Copac;
using AutoCat.Hammicks;
using AutoCat.ViewModels;
using AutoCat.Wildys;

namespace AutoCat
{
    public class AutoCat
    {
        private static string CleanIsbn(string isbn)
        {
            var cleanedIsbn = isbn.Trim();
            cleanedIsbn = cleanedIsbn.Replace("(", "");
            cleanedIsbn = cleanedIsbn.Replace(")", "");
            cleanedIsbn = cleanedIsbn.Replace("-", "");
            cleanedIsbn = cleanedIsbn.Replace(" ", "");
            return cleanedIsbn;
        }

        
        public static string GetImageUrl(string source, string isbn = "")
        {
            string url = "";
            isbn = CleanIsbn(isbn);

            switch (source)
            {
                case "Amazon":
                    {
                        var amazon = new AmazonWebServices();
                        url = amazon.GetImageUrl(isbn);
                        break;
                    }
                case "Hammicks":
                    {
                        var hammicks = new HammicksWebServices();
                        url = hammicks.GetImageUrl(isbn);
                        break;
                    }
                case "Wildys":
                    {
                        var wildys = new WildysWebServices();
                        url = wildys.GetImageUrl(isbn);
                        break;
                    }
            }

            //if (url != null)
            if(!string.IsNullOrEmpty(url))
            {
                if (CheckImageUrl(url))
                {
                    return url;
                }
                return null;
            }
            return null;
        }


        public static bool CheckImageUrl(string url)
        {
            return url.IndexOf("Error", StringComparison.Ordinal) == -1;
        }


        //Get new book data ...
        public AutoCatNewTitle GetIsbnData(string source, string isbn)
        {
            isbn = CleanIsbn(isbn);
            
            var data = new AutoCatNewTitle();
            switch (source)
            {
                case "Amazon":
                    {
                        var amazon = new AmazonWebServices();
                        data = amazon.GetFieldData(isbn);
                        break;
                    }
                case "Hammicks":
                    {
                        var hammicks = new HammicksWebServices();
                        data = hammicks.GetFieldData(isbn);
                        break;
                    }
                case "Wildys":
                    {
                        var wildys = new WildysWebServices();
                        data = wildys.GetFieldData(isbn);
                        break;
                    }
                case "Copac":
                    {
                        var copac = new CopacWebServices();
                        data = copac.GetFieldData(isbn);
                        break;
                    }
            }

            return data;
        }

        public CopacSearchResults GetCopacSearchResults(string searchCriteria)
        {
            var copac = new CopacWebServices();
            var searchResults = copac.GetSearchResults(searchCriteria);
            return searchResults;
        }

    }


}