using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicPlatform.Model.Library
{
    public class UserLibrary
    {
        [Key]
        public Guid LibraryId { get; set; }
        [ForeignKey("SongModel")]
        public Guid SongId { get; set; }
        public SongModel SongModel { get; set; }

    }
}
