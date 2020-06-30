using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Bookpool_Management.Startup))]
namespace Bookpool_Management
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
