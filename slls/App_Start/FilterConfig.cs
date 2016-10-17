using System.Web;
using System.Web.Mvc;
using slls.Filters;

namespace slls
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            //filters.Add(new AuthorizeAttribute());
            filters.Add(new RecaptchaFilter());
        }
    }
}
