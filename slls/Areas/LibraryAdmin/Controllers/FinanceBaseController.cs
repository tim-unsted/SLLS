using slls.Controllers;
using slls.Utils.Helpers;

namespace slls.Areas.LibraryAdmin
{
    //Only allow finance admin users to access this functionality ...
    [AuthorizeRoles(Roles.FinanceAdmin,Roles.BaileyAdmin)]
    public class FinanceBaseController : AdminBaseController
    {
    }
}