﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeddingSite
{
    public class WeddingContext(DbContextOptions<WeddingContext> options) : DbContext(options)
    {
        public required DbSet<Guest> Guests { get; set; }
        public required DbSet<RSVP> RSVPs { get; set; }
        public required DbSet<SeatingInfo> SeatingInfo { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Guest>()
                .Property(g => g.GuestId).ValueGeneratedOnAdd();

            modelBuilder.Entity<RSVP>()
                .Property(r => r.RSVPId).ValueGeneratedOnAdd();

            modelBuilder.Entity<SeatingInfo>()
                .Property(s => s.SeatingId).ValueGeneratedOnAdd();
        }
    }

    public class Guest
    {
        public int GuestId { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string? Email { get; set; }
        public string? PlusOne { get; set; }
        public required string Address { get; set; }
        public required string PostCode { get; set; }
        public required string PostalCity { get; set; }
        public required string Country { get; set; }
        public string? PhoneNumber { get; set; }
        public bool ReceivedPreInvite { get; set; }
        public bool ReceivedInvite { get; set; }
        public required string InviteCode { get; set; }
        public bool HasLoggedIn { get; set; }
        public DateTimeOffset? FirstLogin { get; set; }
        public DateTimeOffset? LastLogin { get; set; }
        public virtual RSVP? RSVPData { get; set; }
    }

    public class RSVP
    {
        public int RSVPId { get; set; }
        public int GuestId { get; set; }
        public bool? Attending { get; set; }
        public bool? PlusOneAttending { get; set; }
        public DateTimeOffset? RSVPDate { get; set; }
        public int NumberOfGuests { get; set; }
        public string? DietaryRequirements { get; set; }
        public string? DietaryOptions { get; set; }
        public string? Message { get; set; }
        public bool OwnTransport { get; set; }
    }

    [Table("SeatingInfo")]
    [PrimaryKey("SeatingId")]
    public class SeatingInfo
    {
        public int SeatingId { get; set; }
        public int GuestId { get; set; }
        public int TableNumber { get; set; }
        public int SeatNumber { get; set; }

        public virtual Guest GuestInfo { get; set; } = null!;
    }
}
