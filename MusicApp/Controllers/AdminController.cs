using MusicApp.Models;
using System;
using System.Linq;
using System.Reflection;
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




        public ActionResult DeleteUser()

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
        public ActionResult EditUser(EditUserModel user)
        {
            if (ModelState.IsValid)
            {
                var existingUser = db.mic_users.FirstOrDefault(u => u.user_id == user.id);
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
        [HttpPost]
        public ActionResult AddUser(AddUser user)
        {
            if (ModelState.IsValid)
            {
                mic_user NewUser = new mic_user();
                NewUser.login_name = user.new_login_name;
                NewUser.full_name = user.new_full_name;
                /*                NewUser.created_by = user.new_created_by;*/
                int cost = 10;
                NewUser.password = BCrypt.Net.BCrypt.HashPassword(user.new_password);
                NewUser.email = user.new_email;
                NewUser.mobile_number = user.new_mobile_number;
                NewUser.role = user.new_role;
                NewUser.is_deleted = '0';
                DateTime currentTime = DateTime.Now;
                NewUser.created_time = currentTime;
                db.mic_users.InsertOnSubmit(NewUser);
                db.SubmitChanges();
                TempData["SuccessMessage"] = "User added successfully!";


                return RedirectToAction("User", "Admin");
            }
            return View("User");
        }
        public ActionResult Music()
        {
            {
                var singerSongs = from singer in db.mic_singers
                                  join songSinger in db.mic_song_singers on singer.singer_id equals songSinger.singer_id
                                  join song in db.mic_songs on songSinger.song_id equals song.song_id
                                  where song.is_deleted == '0'
                                  select new SingerSongViewModel
                                  {
                                      SongId = song.song_id,
                                      SongTitle = song.name,
                                      SingerName = singer.name,
                                      CreatedTime = song.created_time.GetValueOrDefault()
                                  };


                return View(singerSongs.ToList());
            }
      
        }

        public ActionResult Dashboard()
        {
            return View();
        }
    }
}