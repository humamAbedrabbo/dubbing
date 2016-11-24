using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(voiceLibrary.Startup))]
namespace voiceLibrary
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
