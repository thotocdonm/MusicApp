﻿using System.Web.Mvc;
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
                url: "song/{id}",
                defaults: new { controller = "Song", action = "Details", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "Artist",
                url: "Artist/{id}",
                defaults: new { controller = "Artist", action = "DetailArtist", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
