using HotelReservationsManager.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelReservationsManager.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }
        public ApplicationDbContext()
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reservations)
                .HasForeignKey(u => u.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ClientReservation>()
                .HasKey(cr => new { cr.ClientId, cr.ReservationId });

            modelBuilder.Entity<ClientReservation>()
                .HasOne(cr => cr.Client)
                .WithMany(c => c.ClientReservations)
                .HasForeignKey(cr => cr.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ClientReservation>()
                .HasOne(cr => cr.Reservation)
                .WithMany(r => r.ClientReservations)
                .HasForeignKey(cr => cr.ReservationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Room>()
                .HasOne(r => r.Reservation)
                .WithOne(re => re.ReservedRoom)
                .HasForeignKey<Reservation>(re => re.ReservedRoomId)
                .OnDelete(DeleteBehavior.Restrict);


            base.OnModelCreating(modelBuilder);
        }

        public virtual DbSet<Room> Rooms { get; set; }
        public virtual DbSet<Reservation> Reservations { get; set; }
        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<ClientReservation> ClientReservations { get; set; }

    }
}
