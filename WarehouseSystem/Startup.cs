using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WarehouseSystem.Startup))]
namespace WarehouseSystem
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
