using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using WebApi.Models;

namespace WebApi.Context
{
    public class WebAPIDbContext : DbContext
    {

        public WebAPIDbContext(DbContextOptions options) : base(options)
        { }

        public DbSet<UserModel> UserModels { get; set; }
        public DbSet<MessageModel> MessageModels { get; set; }
        public DbSet<TrackingModel> TrackingModels { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserModel>(entity =>
            {

                entity.HasKey(u => u.UserId);
                entity.HasIndex(u => u.UserName).IsUnique();

                // 初期データ
                entity.HasData(
                    new UserModel()
                    {
                        UserId = Guid.Parse("9e0957e4-fd0f-4241-ba12-67c9fa73d0ab"),
                        UserName = "admin",
                        PasswordHash = UserModel.getHashFromPassword("p@ssw0rd"),
                        Role = UserRoles.Admin
                    }
                );

            });
        }
    }
}
