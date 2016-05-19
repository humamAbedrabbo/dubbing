using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(dubbingApp.Startup))]
namespace dubbingApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
