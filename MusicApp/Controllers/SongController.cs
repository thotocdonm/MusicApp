using MusicApp.Models;
using NAudio.Wave;
using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MusicApp.Controllers

{
    public class SongController : Controller
    {


        private DataClasses1DataContext db = new DataClasses1DataContext("Data Source=DESKTOP-UOULN0V\\SQLEXPRESS;Initial Catalog=music;Integrated Security=True;TrustServerCertificate=True");



        public string GetAudioDuration(string filePath)
        {
            using (var audioFile = new AudioFileReader(filePath))
            {
                return audioFile.TotalTime.ToString(@"mm\:ss");
            }
        }



        // GET: Song
        [HttpPost]
        public JsonResult IncreaseView(int songId)
        {
            var song = db.mic_songs.FirstOrDefault(p => p.song_id == songId && p.is_deleted == '0');
            if (song == null)
            {
                return Json(new { success = false, message = "false" });
            }

            song.views = song.views + 1;

            db.SubmitChanges();

            return Json(new { success = true, message = "true" });
        }
        public ActionResult Details(string id)
        {
            var detailSong = (from song in db.mic_songs
                              join singer in db.mic_singers on song.singer_id equals singer.singer_id
                              where song.song_url == id && song.is_deleted == '0'
                              select new DetailSong
                              {
                                  SongId = song.song_id,
                                  SongSrc = song.song_src,
                                  SingerId = song.singer_id,
                                  SingerUrl = singer.singer_url,
                                  SingerName = singer.name,
                                  SingerThumbnailSrc = singer.avatar,
                                  SongTitle = song.name,
                                  Views = (int)song.views,
                                  SongThumbnail = song.thumbnail,
                                  Duration = GetAudioDuration(Server.MapPath("~/Public/Songs/" + song.song_src))
                              }).FirstOrDefault();
            if (detailSong == null)
            {
                return HttpNotFound();
            }

            // Fetch all songs by the same singer, ordered by views in descending order
            var songsBySameSinger = (from song in db.mic_songs
                                     where song.singer_id == detailSong.SingerId && song.is_deleted == '0'
                                     orderby song.views descending
                                     select new DetailSong
                                     {
                                         SongId = song.song_id,
                                         SongSrc = song.song_src,
                                         SingerId = song.singer_id,
                                         SingerName = detailSong.SingerName,
                                         SongTitle = song.name,
                                         Views = (int)song.views,
                                         SongThumbnail = song.thumbnail,
                                         Duration = GetAudioDuration(Server.MapPath("~/Public/Songs/" + song.song_src))
                                     }).ToList();


            //fetch favourite songs
            if (Session["user_id"] != null)
            {
                var userId = (int)Session["user_id"];
                var favouriteSongIds = db.mic_favourite_songs.Where(f => f.user_id == userId && f.is_deleted == '0').Select(f => f.song_id).ToList();
                ViewBag.FavouriteSongIds = favouriteSongIds;
            }


            var detailSongView = new DetailSongView
            {
                DetailSong = detailSong,
                PopularSongs = songsBySameSinger,
            };
            return View(detailSongView);
        }

        public ActionResult Download(string fileName)
        {
            // Path to the public folder
            var filePath = Server.MapPath("~/Public/Songs/" + fileName);

            // Check if the file exists
            if (!System.IO.File.Exists(filePath))
            {
                return HttpNotFound("The requested file does not exist.");
            }

            string sanitizedFileName = Path.GetFileName(fileName)
       .Replace(" ", "") // Replace spaces with hyphens
       .Replace("#", "")  // Remove special characters like #
       .Replace("&", "and");

            // Get the file's MIME type
            var mimeType = MimeMapping.GetMimeMapping(fileName);

            // Return the file as a downloadable response
            return File(filePath, mimeType, sanitizedFileName); // Optional: Rename the file in the third argument
        }

        [HttpPost]
        public ActionResult AddToFavorite(int songId)
        {
            // Get the current logged-in user
            var userId = (int)Session["user_id"]; // Replace with your authentication mechanism

            // Check if the song is already in the user's favorites
            var existingFavorite = db.mic_favourite_songs.FirstOrDefault(f => f.user_id == userId && f.song_id == songId);
            if (existingFavorite != null)
            {
                existingFavorite.is_deleted = '0';
                db.SubmitChanges();
                return Json(new { success = true, message = "Song added to favorites!", isFavorite = true });
            }

            // Add the song to the user's favorites
            var favorite = new mic_favourite_song
            {
                user_id = userId,
                song_id = songId,
                is_deleted = '0',
                created_time = DateTime.Now
            };

            db.mic_favourite_songs.InsertOnSubmit(favorite);
            db.SubmitChanges();

            return Json(new { success = true, message = "Song added to favorites!", isFavorite = true });
        }

        // Remove song from favorites
        [HttpPost]
        public ActionResult RemoveFromFavorite(int songId)
        {
            var userId = (int)Session["user_id"]; // Replace with your authentication mechanism

            // Find the favorite entry
            var favorite = db.mic_favourite_songs.FirstOrDefault(f => f.user_id == userId && f.song_id == songId);
            if (favorite == null)
            {
                return Json(new { success = false, message = "Song not found in favorites.", isFavorite = false });
            }

            favorite.is_deleted = '1';
            db.SubmitChanges();

            return Json(new { success = true, message = "Song removed from favorites!", isFavorite = false });
        }
    }
}