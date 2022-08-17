using Helpdesk_Backend_API.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Helpdesk_Backend_API.Data
{

    public class AppDbContext : IdentityDbContext<HelpDeskUser>
    {

        public DbSet<HelpDeskUser> HelpDeskUsers { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<FusionAdmin> FusionAdmins { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TicketComment> TicketComments { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<HelpDeskUser>().HasIndex(c => c.Email).IsUnique(true);
            builder.Entity<HelpDeskUser>().HasIndex(c => c.PhoneNumber).IsUnique(true);
            base.OnModelCreating(builder);
        }
    }
}
