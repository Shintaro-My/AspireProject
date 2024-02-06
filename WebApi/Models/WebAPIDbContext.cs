using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;

namespace WebApi.Models
{
    public class WebAPIDbContext: DbContext
    {

        public WebAPIDbContext(DbContextOptions options) : base(options)
        {  }

        public DbSet<UserModel> UserModels { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserModel>().HasData(
                new UserModel() {
                    UserId = Guid.Parse("9e0957e4-fd0f-4241-ba12-67c9fa73d0ab"),
                    UserName = "admin",
                    PasswordHash = "a075d17f3d453073853f813838c15b8023b8c487038436354fe599c3942e1f95", // WebAPIDbContext.getHashFromPassword("p@ssw0rd")
                    Role = UserRoles.Admin
                }
            );
        }
    }
}
