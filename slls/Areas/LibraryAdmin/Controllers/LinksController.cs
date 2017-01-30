using slls.Utils.Helpers;

namespace slls.Areas.LibraryAdmin
{
    [AuthorizeRoles(Roles.CatalogueAdmin, Roles.UsersAdmin, Roles.FinanceAdmin, Roles.LoansAdmin, Roles.SerialsAdmin, Roles.OpacAdmin, Roles.BaileyAdmin, Roles.SystemAdmin, Roles.BaileyAdmin)]
    public class LinksController : AdminBaseController
    {
        
    }
}