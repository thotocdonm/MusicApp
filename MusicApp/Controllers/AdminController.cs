using MusicApp.Models;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace MusicApp.Controllers
{
    public class AdminController : Controller
    {
        private DataClasses1DataContext db = new DataClasses1DataContext("Data Source=DESKTOP-H3FAVBH;Initial Catalog=music;Integrated Security=True;TrustServerCertificate=True");
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
            {
                var singerSongs = from singer in db.mic_singers
                                  join song in db.mic_songs on singer.singer_id equals song.singer_id
                                  where song.is_deleted == '0'
                                  select new SingerSongViewModel
                                  {
                                      SongId = song.song_id,
                                      SongTitle = song.name,
                                      SingerName = singer.name,
                                      CreatedTime = song.created_time.GetValueOrDefault()
                                  };
                var singers = from singer in db.mic_singers
                              select new Singer
                              {
                                  SingerId = singer.singer_id,
                                  SingerName = singer.name,
                              };
                var viewModel = new MusicViewModel
                {
                    Songs = singerSongs,
                    Singers = singers
                };


                return View(viewModel);
            }
      
        }

        public ActionResult Dashboard()
        {
            return View();
        }

        public ActionResult Artist()
        {
            return View();
        }
        


    }
}