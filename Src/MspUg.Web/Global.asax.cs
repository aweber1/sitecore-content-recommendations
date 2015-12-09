using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MspUg.Web
{
    public class Global : Sitecore.Web.Application
    {
        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
           AreaRegistration.RegisterAllAreas();
           RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}