using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using slls.Models;

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
        
    }
}