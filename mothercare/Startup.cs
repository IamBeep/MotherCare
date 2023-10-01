using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(mothercare.Startup))]
namespace mothercare
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
