using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MVC_12H_N04.Startup))]
namespace MVC_12H_N04
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //ConfigureAuth(app);
        }
    }
}
