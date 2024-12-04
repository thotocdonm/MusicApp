
using MusicApp.Models;
using NAudio.Wave;
using System.Linq;
using System.Web.Mvc;

namespace MusicApp.Controllers
{
    public class ArtistController : Controller
    {

        private DataClasses1DataContext db = new DataClasses1DataContext("Data Source=DESKTOP-H3FAVBH;Initial Catalog=music;Integrated Security=True;TrustServerCertificate=True");

        public string GetAudioDuration(string filePath)
        {
            using (var audioFile = new AudioFileReader(filePath))
            {
                return audioFile.TotalTime.ToString(@"mm\:ss");
            }
        }
        // GET: Artist
        public ActionResult DetailArtist(int id)
        {
            var singerSongs = from singer in db.mic_singers
                              join song in db.mic_songs on singer.singer_id equals song.singer_id
                              where song.is_deleted == '0' && singer.singer_id == id
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

            var singers = from singer in db.mic_singers
                          select singer.name;
            var viewModel = new MusicViewModel
            {
                Songs = singerSongs,
                Singers = singers.ToString(),
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
