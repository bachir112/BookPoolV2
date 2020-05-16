using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BookPoolV2.Startup))]
namespace BookPoolV2
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
