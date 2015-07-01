using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SyrianPoundRates.Startup))]
namespace SyrianPoundRates
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
