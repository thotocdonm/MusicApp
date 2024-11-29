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
                                  join song in db.mic_songs on singer.singer_id equals song.singer_id
                                  where song.is_deleted == '0'
                                  select new SingerSongViewModel
                                  {
                                      SongId = song.song_id,
                                      SongTitle = song.name,
                                      SingerName = singer.name,
                                      SingerId = singer.singer_id,
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

        [HttpPost]
        public ActionResult UploadSong(HttpPostedFileBase musicFile, HttpPostedFileBase thumbnailFile, String song_name, int? singerSelect, string newSingerName, string language, string genre)
        {
            long unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            string id = Guid.NewGuid().ToString();
            // Check if the music file is uploaded

            string musicFilePath = null;
            string musicFileName = null;
            if (musicFile != null && musicFile.ContentLength > 0)
            {
                musicFileName = $"{Path.GetFileNameWithoutExtension(musicFile.FileName)}_{unixTimestamp}{Path.GetExtension(musicFile.FileName)}";
                musicFileName = musicFileName.Replace(" ", "_");
                musicFilePath = Path.Combine(Server.MapPath("~/Public/Songs"), musicFileName);
                musicFile.SaveAs(musicFilePath);
            }

            // Save the thumbnail image with the timestamp
            string imagePath = null;
            string imageFileName = null;
            if (thumbnailFile != null && thumbnailFile.ContentLength > 0)
            {
                imageFileName = $"{Path.GetFileNameWithoutExtension(thumbnailFile.FileName)}_{unixTimestamp}{Path.GetExtension(thumbnailFile.FileName)}";
                imageFileName = imageFileName.Replace(" ", "_");
                imagePath = Path.Combine(Server.MapPath("~/Public/Images"), imageFileName);
                thumbnailFile.SaveAs(imagePath);
            }

            int singerId;
            if (!string.IsNullOrEmpty(newSingerName))
            {
                // Check if singer exists
                var existingSinger = db.mic_singers.FirstOrDefault(s => s.name == newSingerName);
                if (existingSinger != null)
                {
                    singerId = existingSinger.singer_id;
                }
                else
                {
                    // Create a new singer
                    var newSinger = new mic_singer { name = newSingerName, is_deleted = '0', created_time = DateTime.Now };
                    db.mic_singers.InsertOnSubmit(newSinger);
                    db.SubmitChanges();
                    singerId = newSinger.singer_id;
                }
            }
            else if (singerSelect.HasValue)
            {
                singerId = singerSelect.Value;
            }
            else
            {
                ModelState.AddModelError("", "You must select or create a singer.");
                return View("Music");
            }

            var song = new mic_song
            {
                name = song_name,
                singer_id = singerId,
                is_deleted = '0',
                views = 0,
                thumbnail = imageFileName,
                song_src = musicFileName,
                song_url = id,
                created_time = DateTime.Now,
                language = language,
                type = genre
            };

            db.mic_songs.InsertOnSubmit(song);
            db.SubmitChanges();

            return RedirectToAction("Music");
        }

        [HttpPost]
        public ActionResult DeleteSong(int songId)
        {
            // Find the song in the database
            var song = db.mic_songs.FirstOrDefault(s => s.song_id == songId);
            if (song != null)
            {
                // Mark the song as deleted (soft delete)
                song.is_deleted = '1'; // Assuming '1' indicates deleted
                db.SubmitChanges();

                // Delete associated files if they exist
                string musicFilePath = Path.Combine(Server.MapPath("~/Public/Songs"), song.song_src);
                string thumbnailFilePath = Path.Combine(Server.MapPath("~/Public/Images"), song.thumbnail);

                // Remove music file if it exists
                if (System.IO.File.Exists(musicFilePath))
                {
                    System.IO.File.Delete(musicFilePath);
                }

                // Remove thumbnail file if it exists
                if (System.IO.File.Exists(thumbnailFilePath))
                {
                    System.IO.File.Delete(thumbnailFilePath);
                }
            }

            // Redirect to the action that fetches the song list
            return RedirectToAction("Music"); // Replace with your actual action name
        }

        [HttpPost]
        public ActionResult EditSong(int songId, string songTitle, HttpPostedFileBase musicFile, HttpPostedFileBase thumbnailFile, int? singerId, string editNewSingerName, string language, string genre)
        {
            var song = db.mic_songs.FirstOrDefault(s => s.song_id == songId);
            if (song == null)
            {
                return HttpNotFound();
            }

            if (singerId.HasValue)
            {
                song.singer_id = singerId.Value;
            }


            else if (!string.IsNullOrEmpty(editNewSingerName))
            {
                // Check if singer exists
                var existingSinger = db.mic_singers.FirstOrDefault(s => s.name == editNewSingerName);
                if (existingSinger != null)
                {
                    singerId = existingSinger.singer_id;
                }
                else
                {
                    // Create a new singer
                    var newSinger = new mic_singer { name = editNewSingerName, is_deleted = '0', created_time = DateTime.Now };
                    db.mic_singers.InsertOnSubmit(newSinger);
                    db.SubmitChanges();
                    song.singer_id = newSinger.singer_id;
                }
            }




            // Update song details
            song.name = songTitle;
            song.modified_time = DateTime.Now;
            song.type = genre;
            song.language = language;

            // If new music file is uploaded, delete old file and save new one
            if (musicFile != null && musicFile.ContentLength > 0)
            {
                var oldMusicFilePath = Server.MapPath("~/Public/Songs/" + song.song_src);
                if (System.IO.File.Exists(oldMusicFilePath))
                {
                    System.IO.File.Delete(oldMusicFilePath);
                }

                var musicFileName = Path.GetFileName(musicFile.FileName);
                var musicFilePath = Path.Combine(Server.MapPath("~/Public/Songs"), musicFileName);
                musicFile.SaveAs(musicFilePath);
                song.song_src = musicFileName;
            }

            // If new thumbnail is uploaded, delete old file and save new one
            if (thumbnailFile != null && thumbnailFile.ContentLength > 0)
            {
                var oldThumbnailFilePath = Server.MapPath("~/Public/Images/" + song.thumbnail);
                if (System.IO.File.Exists(oldThumbnailFilePath))
                {
                    System.IO.File.Delete(oldThumbnailFilePath);
                }

            }

            db.SubmitChanges();
            return RedirectToAction("Music");
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