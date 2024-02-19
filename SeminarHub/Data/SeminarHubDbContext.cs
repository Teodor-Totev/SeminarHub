﻿namespace SeminarHub.Data
{
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;

    using SeminarHub.Data.Models;

    public class SeminarHubDbContext : IdentityDbContext
	{
		public SeminarHubDbContext(DbContextOptions<SeminarHubDbContext> options)
			: base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
			builder.Entity<SeminarParticipant>()
				.HasKey(sp => new { sp.SeminarId, sp.ParticipantId });

			builder.Entity<SeminarParticipant>()
				.HasOne(sp => sp.Participant)
				.WithMany()
				.HasForeignKey(sp => sp.ParticipantId)
				.OnDelete(DeleteBehavior.NoAction);

			builder
			   .Entity<Category>()
			   .HasData(new Category()
			   {
				   Id = 1,
				   Name = "Technology & Innovation"
			   },
			   new Category()
			   {
				   Id = 2,
				   Name = "Business & Entrepreneurship"
			   },
			   new Category()
			   {
				   Id = 3,
				   Name = "Science & Research"
			   },
			   new Category()
			   {
				   Id = 4,
				   Name = "Arts & Culture"
			   });

			base.OnModelCreating(builder);
		}

        public DbSet<Seminar> Seminars { get; set; } = null!;

        public DbSet<SeminarParticipant> SeminarsParticipants { get; set; } = null!;

        public DbSet<Category> Categories { get; set; } = null!;
    }
}