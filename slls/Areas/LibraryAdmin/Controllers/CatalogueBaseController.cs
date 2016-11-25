using slls.Controllers;
using slls.Utils.Helpers;

namespace slls.Areas.LibraryAdmin
{
    //Only allow catalogue admin users to access this functionality ...
    [AuthorizeRoles(Roles.CatalogueAdmin,Roles.BaileyAdmin,Roles.FinanceAdmin)]
    public class CatalogueBaseController : AdminBaseController
    {
    }
}