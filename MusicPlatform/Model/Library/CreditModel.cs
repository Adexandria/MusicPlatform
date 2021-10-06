using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicPlatform.Model.Library
{
    public class CreditModel
    { 
        [Key]
        public Guid CreditId { get; set; }
        [ForeignKey("SongModel")]
        public Guid SongId { get; set; }
        public virtual SongModel Song { get; set; }
        public string Producer { get; set; }
        public string Writer { get; set; }
        public string Performer { get; set; }
        public string RecordLabel { get; set; }

       
    }
}