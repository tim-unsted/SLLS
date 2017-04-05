using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using slls.Models;
using slls.ViewModels;

namespace slls
{
    public class SearchService
    {
        public static List<Title> DoFullTextSearch(string searchTerm, string ignoreTerm = "", string field = "*", string table = "OpacFullTextSearch")
        {
            var db = new DbEntities();
            var param1 = new SqlParameter("@SearchTerm", searchTerm);
            var param2 = new SqlParameter("@IgnoreTerm", ignoreTerm);
            var param3 = new SqlParameter("@Field", field);
            var param4 = new SqlParameter("@Table", table);
            var titleIds = db.Database.SqlQuery<int>("DoFtSearch @SearchTerm, @IgnoreTerm, @Field, @Table", param1, param2, param3, param4).ToList();
            var result = db.Titles.Where(t => titleIds.Contains(t.TitleID)).ToList();
            return result;
        }

        public static bool ReIndexAll()
        {
            var db = new DbEntities();
            try
            {
                db.Database.ExecuteSqlCommand("EXEC dbo.UpdateOpacSearch");
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public static bool IndexPending()
        {
            var db = new DbEntities();
            try
            {
                db.Database.ExecuteSqlCommand("EXEC dbo.DoIndexing");
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public static List<SelectAuthor> SelectAuthors(string searchTerm)
        {
            var db = new DbEntities();
            var param1 = new SqlParameter("@SearchTerm", searchTerm);
            var authors = db.Database.SqlQuery<SelectAuthor>("DoAuthorSearch @SearchTerm", param1).ToList();
            var result = authors.ToList();
            return result;
        }
    }


}