using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ECommerceMVC.Web.Startup))]
namespace ECommerceMVC.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
