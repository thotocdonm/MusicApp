using MusicApp.Models;
using System.Linq;
using System.Web.Mvc;

namespace MusicApp.Controllers
{
    public class AdminController : Controller
    {
        private DataClasses1DataContext db = new DataClasses1DataContext("Data Source=LAPTOP-1FVGCU0O\\MINHQUANG2;Initial Catalog=music;Integrated Security=True;TrustServerCertificate=True");
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

        // GET: Admin/Artist
        public ActionResult Artist()
        {
            var singers = db.mic_singers.Where(singer => singer.is_deleted == '0').ToList();
            return View(singers);
        }

        public ActionResult SingerDelete()
        {
            int id = int.Parse(Request.QueryString["ID"]);
            var singer = db.mic_singers.FirstOrDefault(x => x.singer_id == id);

            if (singer != null)
            {
                singer.is_deleted = '1';
                db.SubmitChanges();
            }

            // Tải lại danh sách ca sĩ đã được lọc
            var singers = db.mic_singers.Where(x => x.is_deleted == '0').ToList();
            return View("Artist", singers);
        }

        public ActionResult SingerEdit()
        {
            var singers = db.mic_singers.ToList();
            return View(singers);
        }

        [HttpPost]
        public ActionResult SingerEdit(mic_singer singer)
        {
            if (ModelState.IsValid)
            {
                var existingSinger = db.mic_singers.FirstOrDefault(u => u.singer_id == singer.singer_id);

                if (existingSinger != null)
                {
                    existingSinger.name = singer.name;
                    // Uncomment if you have an avatar field to update
                    // existingSinger.avatar = singer.avatar;

                    db.SubmitChanges();
                }

                return RedirectToAction("Artist", "Admin");
            }

            return View(singer);
        }



        public ActionResult Delete()
        {
            int id = int.Parse(Request.QueryString["ID"]);
            mic_user mic_User = db.mic_users.FirstOrDefault(x => x.user_id == id);

            if (mic_User != null)
            {
                mic_User.is_deleted = '1';
                db.SubmitChanges();
            }
            var user = db.mic_users.Where(x => x.is_deleted == '0').ToList();
            return View("User", user);
        }
        public ActionResult Edit()
        {
            var user = db.mic_users.ToList();
            return View(user);
        }
        [HttpPost]
        public ActionResult Edit(mic_user user)
        {
            if (ModelState.IsValid)
            {
                var existingUser = db.mic_users.FirstOrDefault(u => u.user_id == user.user_id);
                if (existingUser != null)
                {
                    existingUser.login_name = user.login_name;
                    existingUser.email = user.email;
                    existingUser.mobile_number = user.mobile_number;
                    existingUser.role = user.role;

                    db.SubmitChanges();
                }
                return RedirectToAction("User", "Admin");
            }
            return View(user);
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