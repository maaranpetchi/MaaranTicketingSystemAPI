﻿using MaaranTicketingSystemAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MaaranTicketingSystemAPI.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Department> Department { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("users");
            modelBuilder.Entity<Department>().ToTable("Department");

        }

    }
}