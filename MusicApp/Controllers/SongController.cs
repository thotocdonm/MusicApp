using MusicApp.Models;
using NAudio.Wave;
using System.Linq;
using System.Web.Mvc;

namespace MusicApp.Controllers

{
    public class SongController : Controller
    {


        private DataClasses1DataContext db = new DataClasses1DataContext("Data Source=DESKTOP-H3FAVBH;Initial Catalog=music;Integrated Security=True;TrustServerCertificate=True");



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



            var detailSongView = new DetailSongView
            {
                DetailSong = detailSong,
                PopularSongs = songsBySameSinger,
            };
            return View(detailSongView);
        }
    }
}