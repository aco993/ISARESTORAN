using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Restoran2016.Startup))]
namespace Restoran2016
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
