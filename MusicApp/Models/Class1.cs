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
        public string SongTitle { get; set; }
        public string SingerName { get; set; }
        public DateTime? CreatedTime { get; set; }
    }

    public class MusicViewModel
    {
        public IEnumerable<SingerSongViewModel> Songs { get; set; }
        public IEnumerable<Singer> Singers { get; set; }
    }



}
