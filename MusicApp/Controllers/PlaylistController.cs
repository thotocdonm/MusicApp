using MusicApp.Models;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MusicApp.Controllers
{
    public class PlaylistController : Controller
    {

        private DataClasses1DataContext db = new DataClasses1DataContext("Data Source=DESKTOP-1VP4FKU\\SQLEXPRESS;Initial Catalog=music;Integrated Security=True;TrustServerCertificate=True");
        // GET: Playlist
        public string GetAudioDuration(string filePath)
        {
            using (var audioFile = new AudioFileReader(filePath))
            {
                return audioFile.TotalTime.ToString(@"mm\:ss");
            }
        }
        public ActionResult Details(long id)
        {
            var singerSongs = from playlistSong in db.mic_song_playlists
                              join song in db.mic_songs on playlistSong.song_id equals song.song_id
                              join singer in db.mic_singers on song.singer_id equals singer.singer_id
                              where song.is_deleted == '0'
                              && playlistSong.is_deleted == '0'
                              && playlistSong.playlist_id == id
                              select new SingerSongViewModel
                              {
                                  SongId = song.song_id,
                                  SongTitle = song.name,
                                  SingerName = singer.name,
                                  SingerId = singer.singer_id,
                                  CreatedTime = song.created_time.GetValueOrDefault(),
                                  Views = (int)song.views,
                                  SongThumbnail = song.thumbnail,
                                  SongSrc = song.song_src,
                                  Duration = GetAudioDuration(Server.MapPath("~/Public/Songs/" + song.song_src))
                              };

            var playlist = db.mic_playlists
                       .Where(playlistData => playlistData.is_deleted == '0' && playlistData.playlist_id == id)
                       .Select(playlistData => new { playlistData.playlist_id, playlistData.name })
                       .SingleOrDefault();

            var viewModel = new MusicViewModel();



            viewModel = new MusicViewModel
            {
                Songs = singerSongs ?? Enumerable.Empty<SingerSongViewModel>(),
                playlistName = playlist.name,
                playlistId = playlist.playlist_id
            };


            return View(viewModel);
        }

        [HttpPost]
        public JsonResult AddToPlaylist(int playlistId, int songId)
        {
            try
            {
                var playlist = db.mic_playlists.FirstOrDefault(p => p.playlist_id == playlistId && p.is_deleted == '0');

                if (playlist == null)
                {
                    return Json(new { success = false, message = "Playlist not found" });
                }

                var existingSong = db.mic_song_playlists
         .FirstOrDefault(sp => sp.song_id == songId && sp.playlist_id == playlistId && sp.is_deleted == '0');

                if (existingSong != null)
                {
                    return Json(new { success = false, message = "Song already exists in the playlist." });
                }

                mic_song_playlist data = new mic_song_playlist();
                data.song_id = songId;
                data.is_deleted = '0';
                data.playlist_id = playlistId;
                data.created_time = DateTime.Now;
                data.created_by = Session["login_name"]?.ToString();

                db.mic_song_playlists.InsertOnSubmit(data);
                db.SubmitChanges();
                TempData["SuccessMessage"] = "song added playlist successfully!";

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public JsonResult GetPlaylists(string searchTerm = "")
        {
            try
            {
                if (Session["user_id"] == null)
                {
                    return Json(new { success = true, playlists = new List<object>() }, JsonRequestBehavior.AllowGet);
                }

                int userId = Convert.ToInt32(Session["user_id"]);
                var query = db.mic_playlists
                              .Where(p => p.user_id == userId && p.is_deleted == '0');

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query = query.Where(p => p.name.Contains(searchTerm));
                }

                var playlists = query.Select(p => new
                {
                    playlist_id = p.playlist_id,
                    name = p.name
                }).ToList();

                return Json(new { success = true, playlists = playlists }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult Create(string title)
        {
            try
            {

                mic_playlist playlist = new mic_playlist();
                playlist.name = title;
                playlist.user_id = Convert.ToInt32(Session["user_id"]);
                playlist.is_deleted = '0';
                DateTime currentTime = DateTime.Now;
                playlist.created_time = currentTime;
                playlist.created_by = Session["login_name"]?.ToString();
                db.mic_playlists.InsertOnSubmit(playlist);
                db.SubmitChanges();
                TempData["SuccessMessage"] = "Playlist added successfully!";

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Delete(long id)
        {
            try
            {
                if (Session["user_id"] == null)
                {
                    return Json(new { success = false, message = "Bạn chưa đăng nhập!" });
                }

                var playlist = db.mic_playlists.FirstOrDefault(p => p.playlist_id == id && p.is_deleted == '0');

                if (playlist == null)
                {
                    return Json(new { success = false, message = "Danh sách phát không tồn tại!" });
                }

                playlist.is_deleted = '1';
                db.SubmitChanges();

                return Json(new { success = true, message = "Danh sách phát đã được xóa thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeleteSong(long playlistId, long songId)
        {
            try
            {
                if (Session["user_id"] == null)
                {
                    return Json(new { success = false, message = "Bạn chưa đăng nhập!" });
                }

                var songPlaylist = db.mic_song_playlists.FirstOrDefault(p => p.playlist_id == playlistId && p.song_id == songId && p.is_deleted == '0');

                if (songPlaylist == null)
                {
                    return Json(new { success = false, message = "Nhạc không tồn tại!" });
                }

                songPlaylist.is_deleted = '1';
                db.SubmitChanges();

                return Json(new { success = true, message = "Nhạc đã được xóa thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        [HttpPost]
        public JsonResult Update(long id, string newTitle)
        {
            try
            {
                if (Session["user_id"] == null)
                {
                    return Json(new { success = false, message = "Bạn chưa đăng nhập!" });
                }

                var playlist = db.mic_playlists.FirstOrDefault(p => p.playlist_id == id && p.is_deleted == '0');

                if (playlist == null)
                {
                    return Json(new { success = false, message = "Danh sách phát không tồn tại!" });
                }

                playlist.name = newTitle;
                playlist.modified_time = DateTime.Now;
                playlist.modified_by = Session["login_name"]?.ToString();

                db.SubmitChanges();

                return Json(new { success = true, message = "Danh sách phát đã được cập nhật thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}