using System;
using System.Collections.Generic;

namespace MusicApp.Models
{

    public class Song
    {
        public int SongId { get; set; }
        public string SongTitle { get; set; }
        public DateTime CreatedTime { get; set; }
    }

    public class Singer
    {
        public int SingerId { get; set; }
        public string SingerName { get; set; }
    }


    public class SingerSongViewModel
    {
        public int SongId { get; set; }

        public int SingerId { get; set; }
        public string SongTitle { get; set; }
        public string SingerName { get; set; }
        public DateTime? CreatedTime { get; set; }
    }

    public class MusicViewModel
    {
        public IEnumerable<SingerSongViewModel> Songs { get; set; }
        public IEnumerable<Singer> Singers { get; set; }
    }

    public class DetailSong
    {
        public int SongId { get; set; }
        public string SongTitle { get; set; }
        public int Views { get; set; }
        public int SingerId { get; set; }
        public string SingerName { get; set; }
        public string SongSrc { get; set; }

        public string SongThumbnail { get; set; }
        public string Duration { get; set; }
    }

    public class DetailSongView
    {
        public DetailSong DetailSong { get; set; }
        public IEnumerable<DetailSong> PopularSongs { get; set; }
    }



}
