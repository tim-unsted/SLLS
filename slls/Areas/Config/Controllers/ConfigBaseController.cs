using System.Web.Mvc;
using slls.Controllers;
using slls.Utils.Helpers;

namespace slls.Areas.Config
{
    //Only allow Admin role to access this functionality ...
    [AuthorizeRoles(Roles.Administrator, Roles.BsAdmin)]
    public class ConfigBaseController : sllsBaseController
    {
    }
}