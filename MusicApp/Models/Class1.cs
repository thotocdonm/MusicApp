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
    public class MusicViewModel
    {
        public IEnumerable<SingerSongViewModel> Songs { get; set; }
        public IEnumerable<Singer> Singers { get; set; }

        public String playlistName { get; set; }
    }

    public class Artist
    {
        public int SingerId { get; set; }
        public string SingerName { get; set; }
        public string avatar { get; set; }
    }
    public class ArtistViewModel
    {
        public IEnumerable<SingerSongViewModel> Songs { get; set; }
        public Artist Singers { get; set; }

        public String playlistName { get; set; }
    }

    public class SingerSongViewModel
    {
        public int SongId { get; set; }

        public string SongTitle { get; set; }

        public string SongGenre { get; set; }
        public string SongLanguage { get; set; }
        public int Views { get; set; }
        public int SingerId { get; set; }
        public string SingerName { get; set; }
        public string SingerUrl { get; set; }
        public string SongSrc { get; set; }
        public string SongUrl { get; set; }
        public string SingerThumbnail { get; set; }

        public string SongThumbnail { get; set; }
        public string Duration { get; set; }
        public DateTime? CreatedTime { get; set; }





    }

    public class HomeViewModel
    {
        public IEnumerable<SingerSongViewModel> PopularSongs { get; set; }
        public IEnumerable<SingerSongViewModel> VietnameseSongs { get; set; }
        public IEnumerable<SingerSongViewModel> JapaneseSongs { get; set; }
        public IEnumerable<SingerSongViewModel> KoreanSongs { get; set; }
    }

    public class DetailSong
    {
        public int SongId { get; set; }
        public string SongTitle { get; set; }
        public int Views { get; set; }
        public int SingerId { get; set; }
        public string SingerName { get; set; }

        public string SingerThumbnailSrc { get; set; }
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
