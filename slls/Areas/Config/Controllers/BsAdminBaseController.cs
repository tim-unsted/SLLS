using System.Web.Mvc;
using slls.Controllers;
using slls.Utils.Helpers;

namespace slls.Areas.Config
{
    //Only allow BS Admin role to access this functionality ...
    [AuthorizeRoles(Roles.BsAdmin)]
    public class BsAdminBaseController : sllsBaseController
    {
    }
}