using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using slls.Models;
using slls.Utils;
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
            var param5 = new SqlParameter("@UserId", HttpContext.Current.Session["currentUserId"] ?? "");
            var titleIds = db.Database.SqlQuery<int>("DoFtSearch @SearchTerm, @IgnoreTerm, @Field, @Table, @UserId", param1, param2, param3, param4, param5).ToList();
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

        public static List<SelectAuthor> SelectAuthors(string searchTerm, int take = 100, bool inUseOnly = false, bool opac = false)
        {
            var db = new DbEntities();
            var param1 = new SqlParameter("@SearchTerm", searchTerm);
            var param2 = new SqlParameter("@Take", take);
            var param3 = new SqlParameter("@InUseOnly", inUseOnly);
            var authors = db.Database.SqlQuery<SelectAuthor>("DoAuthorSearch @SearchTerm, @Take, @InUseOnly", param1, param2, param3).ToList();
            var result = authors.ToList();
            return result;
        }

        public static List<SelectKeyword> SelectKeywords(string searchTerm, int take = 100, bool inUseOnly = false, bool opac = false)
        {
            var db = new DbEntities();
            var param1 = new SqlParameter("@SearchTerm", searchTerm);
            var param2 = new SqlParameter("@Take", take);
            var param3 = new SqlParameter("@InUseOnly", inUseOnly);
            var keywords = db.Database.SqlQuery<SelectKeyword>("DoKeywordSearch @SearchTerm, @Take, @InUseOnly", param1, param2, param3).ToList();
            var result = keywords.ToList();
            return result;
        }

        public static List<SelectTitles> SelectTitles(string searchTerm, int take = 100)
        {
            var db = new DbEntities();
            var param1 = new SqlParameter("@SearchTerm", searchTerm);
            var param2 = new SqlParameter("@Take", take);
            var titles = db.Database.SqlQuery<SelectTitles>("SelectTitle @SearchTerm, @Take", param1, param2).ToList();
            var result = titles.ToList();
            return result;
        }

        public static List<SelectCopy> SelectCopies(string searchTerm, int take = 100)
        {
            var db = new DbEntities();
            var param1 = new SqlParameter("@SearchTerm", searchTerm);
            var param2 = new SqlParameter("@Take", take);
            var titles = db.Database.SqlQuery<SelectCopy>("SelectCopy @SearchTerm, @Take", param1, param2).ToList();
            var result = titles.ToList();
            return result;
        }

        public static IEnumerable<SelectOrder> SelectOrders(string searchTerm, int take = 100)
        {
            var db = new DbEntities();
            var param1 = new SqlParameter("@SearchTerm", searchTerm);
            var param2 = new SqlParameter("@Take", take);
            var titles = db.Database.SqlQuery<SelectOrder>("SelectOrder @SearchTerm, @Take", param1, param2).ToList();
            var result = titles.ToList();
            return result;
        }

        public static IEnumerable<SearchTerms> GetSearchTerms(string searchTerm, int take = 100)
        {
            var db = new DbEntities();
            var param1 = new SqlParameter("@SearchTerm", searchTerm);
            var param2 = new SqlParameter("@Take", take);
            var searchTerms = db.Database.SqlQuery<SearchTerms>("GetSearchTerms @SearchTerm, @Take", param1, param2).ToList();
            var result = searchTerms.ToList();
            return result;
        }
    }


}