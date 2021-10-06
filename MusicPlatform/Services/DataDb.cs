﻿using Microsoft.EntityFrameworkCore;
using MusicPlatform.Model.Library;
using MusicPlatform.Model.User.Profile;

namespace MusicPlatform.Services
{
    public class DataDb :DbContext
    {
        public DataDb(DbContextOptions<DataDb> options):base(options)
        {

        }
        public DbSet<CreditModel> SongCredits { get; set; }
        public DbSet<SongModel> Songs { get; set; }
        public DbSet<SongImage> SongImages { get; set; }
        public DbSet<SongLibrary> Libraries { get; set; }
        public DbSet<UserImage> UserImages { get; set; }
        public DbSet<FollowingModel> Followings { get; set; }
        public DbSet<UserProfile> Profiles { get; set; }
    }
}
