using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ProyectoIngenieria.Startup))]
namespace ProyectoIngenieria
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
