using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(HomeFinder.Startup))]
namespace HomeFinder
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
        }
    }
}
