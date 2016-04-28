using System.Web;
using System.Web.Routing;
using System.Web.Mvc;


namespace Demo.Web.Application
{


public class DemoWebApplication
    : HttpApplication
{


protected void Application_Start()
{
    RouteTable.Routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

    RouteTable.Routes.MapRoute(
        "Root",
        "",
        new { controller = "Root", action = "Index" });

    RouteTable.Routes.MapRoute(
        "Courses",
        "courses",
        new { controller = "Courses", action = "Index" });

    RouteTable.Routes.MapRoute(
        "Course",
        "course/{courseName}",
        new { controller = "Courses", action = "Course" });

    RouteTable.Routes.MapRoute(
        "Module",
        "courses/{courseName}/modules/{moduleName}",
        new { controller = "Courses", action = "Module" });

    RouteTable.Routes.MapRoute(
        "CreateCourse",
        "createcourse",
        new { controller = "Courses", action = "Course" });

    RouteTable.Routes.MapRoute(
        "CreateModule",
        "courses/{courseName}/createmodule",
        new { controller = "Courses", action = "Module" });

    /*
    RouteTable.Routes.MapRoute(
        name: "Default",
        url: "{controller}/{action}/{id}",
        defaults: new {
            controller = "Home",
            action = "Index",
            id = UrlParameter.Optional }
    );
    AreaRegistration.RegisterAllAreas();
    FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
    RouteConfig.RegisterRoutes(RouteTable.Routes);
    BundleConfig.RegisterBundles(BundleTable.Bundles);
    */
}


}
}
