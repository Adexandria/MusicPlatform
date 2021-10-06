using Microsoft.EntityFrameworkCore;
using MusicPlatform.Model.User.Profile;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicPlatform.Model.Library
{
    public class SongImage 
    {
        [Key]
        public Guid SongImageid { get; set; }
        [ForeignKey("SongId")]
        public Guid SongId { get; set; }
        public string ImageUrl { get; set; }
         public virtual SongModel Song { get; set; }
    }
}
