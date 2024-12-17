
using MusicApp.Models;
using NAudio.Wave;
using System.Linq;
using System.Web.Mvc;

namespace MusicApp.Controllers
{
    public class ArtistController : Controller
    {


        private DataClasses1DataContext db = new DataClasses1DataContext("Data Source=DESKTOP-1VP4FKU\\SQLEXPRESS;Initial Catalog=music;Integrated Security=True;TrustServerCertificate=True");


        public string GetAudioDuration(string filePath)
        {
            using (var audioFile = new AudioFileReader(filePath))
            {
                return audioFile.TotalTime.ToString(@"mm\:ss");
            }
        }
        // GET: Artist
        public ActionResult DetailArtist(string id)
        {
            var singerSongs = from singer in db.mic_singers
                              join song in db.mic_songs on singer.singer_id equals song.singer_id
                              where song.is_deleted == '0' && singer.singer_url == id
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

            var singers = (from s in db.mic_singers
                           where s.singer_url == id
                           select new Artist
                           {
                               SingerName = s.name,
                               avatar = s.avatar,
                           }).FirstOrDefault();
            var viewModel = new ArtistViewModel
            {
                Songs = singerSongs,
                Singers = singers,
            };


            return View(viewModel);
        }

        public ActionResult Test()
        {

            // Your logic here
            return View();
        }
    }
}
