using System.Web.Mvc;
using System.Web.Routing;
namespace ShoppingCart.Mvc5;
public class MvcApplication : System.Web.HttpApplication
{
    protected void Application_Start() { AreaRegistration.RegisterAllAreas(); RouteTable.Routes.MapMvcAttributeRoutes(); RouteTable.Routes.MapRoute("Default", "{controller}/{action}/{id}", new { controller = "Home", action = "Index", id = UrlParameter.Optional }); }
}
