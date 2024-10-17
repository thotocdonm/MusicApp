using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MusicApp.Models;
using System.Configuration;

namespace MusicApp.Controllers
{
    public class AdminController : Controller
    {
        DataClasses1DataContext db = new DataClasses1DataContext(ConfigurationManager.ConnectionStrings["musicConnectionString1"].ConnectionString);
        public ActionResult Music()
        {
            var singerSongs = from singer in db.mic_singers
                              join songSinger in db.mic_song_singers on singer.singer_id equals songSinger.singer_id
                              join song in db.mic_songs on songSinger.song_id equals song.song_id
                              select new SingerSongViewModel
                              {
                                  created_time = (DateTime)songSinger.created_time,
                                  SingerName = singer.name,
                                  SongId = song.song_id,
                                  SongTitle = song.name
                              };
                              

            return View(singerSongs.ToList());
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult User()
        {
            return View();
        }

        

        public ActionResult Dashboard()
        {
            return View();
        }
    }
}