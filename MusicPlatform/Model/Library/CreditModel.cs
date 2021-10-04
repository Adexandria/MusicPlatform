using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicPlatform.Model.Library
{
    public class CreditModel
    {
        public string Producer { get; set; }
        public string Writer { get; set; }
        public string Performer { get; set; }
        public string RecordLabel { get; set; }

        [ForeignKey("SongModel")]
        public Guid SongId { get; set; }
    }
}