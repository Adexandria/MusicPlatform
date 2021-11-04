using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicPlatform.Model.Library.DTO
{
    public class SongUpdate
    {
        public IFormFile Song { get; set; }
        public DateTime ReleasedDate { get; set; }
    }
}
