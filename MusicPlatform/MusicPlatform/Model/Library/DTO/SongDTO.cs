using System;

namespace MusicPlatform.Model.User.Profile.DTO
{
    public class SongDTO
    {
        public string SongName { get; set; }
        public string SongURL { get; set; }
        public string ImageUrl { get; set; }
        public  DateTime ReleasedDate { get; set; }
        public CreditModelDTO Credit { get; set; }
        public int Download { get; set; }
    }
}
