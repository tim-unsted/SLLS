using slls.Controllers;
using slls.Utils.Helpers;

namespace slls.Areas.LibraryAdmin
{
    //Only allow admin role to access this functionality ...
    [AuthorizeRoles(Roles.Administrator, Roles.Staff, Roles.BsAdmin)]
    public class AdminBaseController : sllsBaseController
    {
    }
}