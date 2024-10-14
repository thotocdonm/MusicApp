using System.Web.Mvc;

namespace MusicApp.Controllers
{
    public class SongController : Controller
    {
        // GET: Song
        public ActionResult Details(string id)
        {
            ViewData["id"] = id;
            return View();
        }
    }
}