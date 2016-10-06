using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(slls.Startup))]
namespace slls
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            ConfigureSignalR(app);
        }
    }
}
