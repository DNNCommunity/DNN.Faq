using System;
using System.Web.Routing;
using DotNetNuke.Web.Api;

namespace DotNetNuke.Modules.FAQs.Services
{
    public class RouteMapper : IServiceRouteMapper
    {
        public void RegisterRoutes(IMapRoute mapRouteManager)
        {
            mapRouteManager.MapHttpRoute("FAQs",
                                         "default",
                                         "{controller}/{action}/{output}",
                                         new RouteValueDictionary {{"output", "default"}},
                                         new RouteValueDictionary {{"output", "xml|json|rss|atom|default"}},
                                         new[] {"DotNetNuke.Modules.FAQs.Services"});
        }
    }
}
