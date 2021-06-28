using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BusReservationSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<BusReservationSystem.Models.Bus> Bus { get; set; }

        public DbSet<BusReservationSystem.Models.Line> Line { get; set; }
        public DbSet<BusReservationSystem.Models.Bookings> Bookings { get; set; }
        public DbSet<BusReservationSystem.Models.BusBookingsModel> BusBookings { get; set; }
    }
}
