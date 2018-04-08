﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SocialNetwork.Interface.Models.Entities;

namespace SocialNetwork.DAL
{
    public class SocialNetworkDbContext : IdentityDbContext<User>
    {
        public SocialNetworkDbContext(DbContextOptions<SocialNetworkDbContext> builder) : base(builder)
        {
        }

        public DbSet<Post> Posts { get; set; }
    }
}
