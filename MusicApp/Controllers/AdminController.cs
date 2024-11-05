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
        [HttpPost]
        public ActionResult SaveSong(HttpPostedFileBase musicFile, HttpPostedFileBase thumbnailFile, string song_name, string singerSelect, string newSingerName, string language, string genre)
        {
            int singerId;
            mic_song newSong = new mic_song();

            // Kiểm tra nếu người dùng nhập ca sĩ mới
            if (!string.IsNullOrEmpty(newSingerName))
            {
                // Thêm ca sĩ mới vào bảng mic_singer
                mic_singer newSinger = new mic_singer { name = newSingerName, is_deleted='0',created_time=DateTime.Now };
                db.mic_singers.InsertOnSubmit(newSinger);
                db.SubmitChanges();

                // Lấy Id của ca sĩ vừa thêm
                singerId = newSinger.singer_id;
            }
            else
            {
                // Nếu người dùng chọn ca sĩ có sẵn, lấy Id của ca sĩ đó
                singerId = int.Parse(singerSelect); // Giả sử singerSelect chứa Id của ca sĩ đã chọn
            }

            // Tạo đối tượng Song và gán các thuộc tính
            var song = new addSong
            {
                SongName = song_name,
                SingerId = singerId,
                Language = language,
                Genre = genre
            };

            // Lưu tên file nhạc vào CSDL và file vào thư mục
            if (musicFile != null && musicFile.ContentLength > 0)
            {
                var musicFileName = Path.GetFileName(musicFile.FileName);
                var musicPath = Path.Combine(Server.MapPath("~/Public/Songs"), musicFileName);
                musicFile.SaveAs(musicPath);
                song.MusicFileName = musicFileName;
            }

            // Lưu tên file ảnh vào CSDL và file vào thư mục
            if (thumbnailFile != null && thumbnailFile.ContentLength > 0)
            {
                var thumbnailFileName = Path.GetFileName(thumbnailFile.FileName);
                var thumbnailPath = Path.Combine(Server.MapPath("~/Public/Images"), thumbnailFileName);
                thumbnailFile.SaveAs(thumbnailPath);
                song.ThumbnailFileName = thumbnailFileName;
            }

            // Thêm đối tượng Song vào bảng mic_song
            newSong.name = song.SongName;
            newSong.thumbnail = song.ThumbnailFileName;
            newSong.created_time = DateTime.Now;
            newSong.language = language;
            newSong.type = genre;
            newSong.is_deleted = '0';
            newSong.singer_id = singerId;
            newSong.song_url = Guid.NewGuid().ToString();
            newSong.song_src = song.MusicFileName;
            db.mic_songs.InsertOnSubmit(newSong);
            db.SubmitChanges();


            return RedirectToAction("Music");
        }


    }
}