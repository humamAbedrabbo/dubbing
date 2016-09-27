using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace dubbingApp
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "dubbingRoute",
                url: "dubbing/Index"
            );
            routes.MapRoute(
                name: "dubbingMonitorRoute",
                url: "dubbingMonitor/Index"
            );
            routes.MapRoute(
                name: "executives",
                url: "executiveSummary/Index"
            );
        }
    }
}
