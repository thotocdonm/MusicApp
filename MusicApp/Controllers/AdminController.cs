using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MusicApp.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult User()
        {
            return View();
        }

        public ActionResult Music()
        {
            return View();
        }

        public ActionResult Dashboard()
        {
            return View();
        }
    }
}