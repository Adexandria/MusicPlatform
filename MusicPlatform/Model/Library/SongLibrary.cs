using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MusicPlatform.Model.Library
{
    public class SongLibrary
    {
        [Key]
        public Guid LibraryId { get; set; }
        [ForeignKey("SongId")]
        public Guid SongId { get; set; }
        public virtual SongModel Song { get; set; }
    }
}
