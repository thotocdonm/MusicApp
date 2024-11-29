using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MusicApp.Controllers
{
    public class PlaylistController : Controller
    {
        // GET: Playlist
        public ActionResult Details(string id)
        {
            ViewData["id"] = id;
            return View();
        }
    }
}