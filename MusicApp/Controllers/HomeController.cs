﻿using MusicApp.Models;
using System.Linq;
using System.Web.Mvc;

namespace MusicApp.Controllers
{
    public class HomeController : Controller
    {

        private DataClasses1DataContext db = new DataClasses1DataContext("Data Source=DESKTOP-H3FAVBH;Initial Catalog=music;Integrated Security=True;TrustServerCertificate=True");
        public ActionResult Index()
        {
            var popularSongs = (from song in db.mic_songs
                                join singer in db.mic_singers on song.singer_id equals singer.singer_id
                                where song.is_deleted == '0'
                                orderby song.views descending
                                select new SingerSongViewModel
                                {
                                    SongId = song.song_id,
                                    SongSrc = song.song_src,
                                    SongUrl = song.song_url,
                                    SingerId = song.singer_id,
                                    SingerName = singer.name,
                                    SongTitle = song.name,
                                    SingerUrl = singer.singer_url,
                                    SongThumbnail = song.thumbnail,

                                }).Take(5).ToList();

            var japaneseSongs = (from song in db.mic_songs
                                 join singer in db.mic_singers on song.singer_id equals singer.singer_id
                                 where song.is_deleted == '0' && song.language == "Japanese"
                                 orderby song.views descending
                                 select new SingerSongViewModel
                                 {
                                     SongId = song.song_id,
                                     SongSrc = song.song_src,
                                     SongUrl = song.song_url,
                                     SingerId = song.singer_id,
                                     SingerName = singer.name,
                                     SongTitle = song.name,
                                     SingerUrl = singer.singer_url,
                                     SongThumbnail = song.thumbnail,

                                 }).Take(5).ToList();

            var vietnameseSongs = (from song in db.mic_songs
                                   join singer in db.mic_singers on song.singer_id equals singer.singer_id
                                   where song.is_deleted == '0' && song.language == "Vietnamese"
                                   orderby song.views descending
                                   select new SingerSongViewModel
                                   {
                                       SongId = song.song_id,
                                       SongSrc = song.song_src,
                                       SongUrl = song.song_url,
                                       SingerId = song.singer_id,
                                       SingerName = singer.name,
                                       SongTitle = song.name,
                                       SingerUrl = singer.singer_url,
                                       SongThumbnail = song.thumbnail,

                                   }).Take(5).ToList();

            var koreanSongs = (from song in db.mic_songs
                               join singer in db.mic_singers on song.singer_id equals singer.singer_id
                               where song.is_deleted == '0' && song.language == "Korean"
                               orderby song.views descending
                               select new SingerSongViewModel
                               {
                                   SongId = song.song_id,
                                   SongSrc = song.song_src,
                                   SongUrl = song.song_url,
                                   SingerId = song.singer_id,
                                   SingerName = singer.name,
                                   SongTitle = song.name,
                                   SingerUrl = singer.singer_url,
                                   SongThumbnail = song.thumbnail,

                               }).Take(5).ToList();

            var viewModel = new HomeViewModel();



            viewModel = new HomeViewModel
            {
                PopularSongs = popularSongs ?? Enumerable.Empty<SingerSongViewModel>(),
                JapaneseSongs = japaneseSongs ?? Enumerable.Empty<SingerSongViewModel>(),
                VietnameseSongs = vietnameseSongs ?? Enumerable.Empty<SingerSongViewModel>(),
                KoreanSongs = koreanSongs ?? Enumerable.Empty<SingerSongViewModel>(),
            };

            return View(viewModel);
        }


        public ActionResult AccessDenied()
        {
            return View();
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public JsonResult Search(string keyword)
        {
            var resultSongs = (from song in db.mic_songs
                               join singer in db.mic_singers on song.singer_id equals singer.singer_id
                               where song.is_deleted == '0' && song.name.Contains(keyword)
                               select new SingerSongViewModel
                               {
                                   SongId = song.song_id,
                                   SongSrc = song.song_src,
                                   SongUrl = song.song_url,
                                   SingerId = song.singer_id,
                                   SingerName = singer.name,
                                   SongTitle = song.name,
                                   SingerUrl = singer.singer_url,
                                   SongThumbnail = song.thumbnail,
                               }).ToList();

            var resultSingers = (from singer in db.mic_singers 
                          where singer.name.Contains(keyword)
                          select new SingerSongViewModel
                          {
                              SingerId = singer.singer_id,
                              SingerName = singer.name,
                              SingerThumbnail = singer.avatar,
                              SingerUrl = singer.singer_url,
                          }).ToList();

            return Json(new {Songs = resultSongs, Singers = resultSingers}, JsonRequestBehavior.AllowGet);
        }

    }
}