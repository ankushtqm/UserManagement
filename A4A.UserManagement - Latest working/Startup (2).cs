using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(A4A.UM.Startup))]
namespace A4A.UM
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
