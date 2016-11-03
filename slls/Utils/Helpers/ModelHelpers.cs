using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using slls.Models;

namespace slls.Utils.Helpers
{
    public static class Extensions
    {
        public static Expression<Func<Title, string>> AuthorString()
        {
            return t => t.AuthorString;
        }
    }
}