using MusicApp.Models;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MusicApp.Controllers
{
    public class LikeSongController : Controller
    {
        // GET: LikeSong
        private DataClasses1DataContext db = new DataClasses1DataContext("Data Source=DESKTOP-H3FAVBH;Initial Catalog=music;Integrated Security=True;TrustServerCertificate=True");


        public string GetAudioDuration(string filePath)
        {
            using (var audioFile = new AudioFileReader(filePath))
            {
                return audioFile.TotalTime.ToString(@"mm\:ss");
            }
        }
        // GET: Artist
        public ActionResult SongLike()
        {
            var singerSongs = from singer in db.mic_singers
                              join song in db.mic_songs on singer.singer_id equals song.singer_id
                              join likesong in db.mic_favourite_songs on song.song_id equals likesong.song_id
                              join users in db.mic_users on likesong.user_id equals users.user_id
                              where song.is_deleted == '0' && users.user_id == int.Parse(Session["user_id"].ToString())
                              select new SingerSongViewModel
                              {
                                  SingerUrl = singer.singer_url,
                                  SongUrl = song.song_url,
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

            
            


            return View(singerSongs);
        }
        public ActionResult Index()
        {
            return View();
        }
    }
}