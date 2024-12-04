using System.Web.Mvc;
using System.Web.Routing;

namespace MusicApp
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Song",
                url: "song/{action}/{id}",
                defaults: new { controller = "Song", action = "Details", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Artist",
                url: "Artist/{action}/{id}",
                defaults: new { controller = "Artist", action = "DetailArtist", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
             name: "StreamMp3",
             url: "Public/Songs/{fileName}",
              defaults: new { controller = "Audio", action = "StreamMp3", fileName = UrlParameter.Optional }
            );

            routes.MapRoute(
            name: "StreamMp3test",
            url: "Audio/StreamMp3/{fileName}",
             defaults: new { controller = "Audio", action = "StreamMp3", fileName = UrlParameter.Optional }
           );

        }
    }
}
