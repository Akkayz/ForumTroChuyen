using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(WebTroChuyen.Startup))]

namespace WebTroChuyen
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}