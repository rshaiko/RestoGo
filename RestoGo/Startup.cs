using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(RestoGo.Startup))]
namespace RestoGo
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
