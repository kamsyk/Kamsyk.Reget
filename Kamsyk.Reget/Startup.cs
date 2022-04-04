using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Kamsyk.Reget.Startup))]
namespace Kamsyk.Reget
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
