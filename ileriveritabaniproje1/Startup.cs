using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ileriveritabaniproje1.Startup))]
namespace ileriveritabaniproje1
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
