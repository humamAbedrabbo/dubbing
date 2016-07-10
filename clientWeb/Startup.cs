using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(clientWeb.Startup))]
namespace clientWeb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
