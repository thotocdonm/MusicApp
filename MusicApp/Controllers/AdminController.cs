using MusicApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MusicApp.Controllers
{
    public class AdminController : Controller
    {
        private DataClasses1DataContext db = new DataClasses1DataContext("Data Source=Dienmoi\\MSSQLSERVER01;Initial Catalog=music;Integrated Security=True;TrustServerCertificate=True");
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult User()
        {
            var x = db.mic_users.Where(user => user.is_deleted == '0').ToList();
            return View(x);
        }
        public ActionResult Delete()
        {
            int id = int.Parse(Request.QueryString["ID"]);
            mic_user mic_User = db.mic_users.FirstOrDefault(x =>x.user_id == id);

            if (mic_User != null)
            {
                mic_User.is_deleted = '1';
                db.SubmitChanges();
            }
            var user = db.mic_users.Where(x => x.is_deleted == '0').ToList();
            return View("User",user); 
        }
        public ActionResult Edit(int id, string login_name, string email, string mobile_number, string role)
        {
            var user = db.mic_users.Where(x => x.user_id == id);
            if (user != null) {
                
            }
            return View("User",x);
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