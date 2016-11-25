using slls.Controllers;
using slls.Utils.Helpers;

namespace slls.Areas.LibraryAdmin
{
    //Only allow library admin roles to access this functionality ...
    [AuthorizeRoles(Roles.CatalogueAdmin, Roles.UsersAdmin, Roles.FinanceAdmin, Roles.LoansAdmin, Roles.SerialsAdmin, Roles.OpacAdmin, Roles.BaileyAdmin)]
    public class AdminBaseController : sllsBaseController
    {
    }
}