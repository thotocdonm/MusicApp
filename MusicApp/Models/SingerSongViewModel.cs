using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicApp.Models
{
    public class SingerSongViewModel
    {

        public DateTime? created_time { get; set; }
        public string SingerName { get; set; }
        public int SongId { get; set; }
        public string SongTitle { get; set; }


    }
}