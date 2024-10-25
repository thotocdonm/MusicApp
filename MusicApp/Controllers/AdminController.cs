﻿using MusicApp.Models;
using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MusicApp.Controllers
{
    public class AdminController : Controller
    {
        private DataClasses1DataContext db = new DataClasses1DataContext("Data Source=DESKTOP-UOULN0V\\SQLEXPRESS;Initial Catalog=music;Integrated Security=True;Connect Timeout=30;Encrypt=True;TrustServerCertificate=True");
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
        public ActionResult UploadSong(HttpPostedFileBase musicFile, HttpPostedFileBase thumbnailFile, String song_name, int? singerSelect, string newSingerName)
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
                is_deleted = '0',
                thumbnail = imageFileName,
                song_src = musicFileName,
                song_url = id,
                created_time = DateTime.Now,
            };

            db.mic_songs.InsertOnSubmit(song);
            db.SubmitChanges();

            var songSinger = new mic_song_singer
            {
                song_id = song.song_id,
                singer_id = singerId,
                is_deleted = '0',
                created_time = DateTime.Now,
            };
            db.mic_song_singers.InsertOnSubmit(songSinger);
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