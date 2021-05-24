// <copyright file="PoliceIncidentsDbContext.cs" company="Engage Squared">
// Copyright (c) Engage Squared. All rights reserved.
// </copyright>

namespace PoliceIncidents.Core.DB
{
    using Microsoft.EntityFrameworkCore;
    using PoliceIncidents.Core.DB.Entities;

    public class PoliceIncidentsDbContext : DbContext
    {
        private static bool isEnsured;

        public PoliceIncidentsDbContext(DbContextOptions<PoliceIncidentsDbContext> options)
        : base(options)
        {
            // hack to ensure once per application run, becauser Azure Function DI doesn't provide such option.
            if (!isEnsured)
            {
                this.Database.EnsureCreated();
                isEnsured = true;
            }
        }

        public DbSet<UserEntity> IncidentTeams { get; protected set; }

        public DbSet<IncidentDetailsEntity> IncidentDetails { get; protected set; }

        public DbSet<IncidentUpdateEntity> IncidentUpdates { get; protected set; }

        public DbSet<IncidentTeamMemberEntity> IncidentTeamMembers { get; protected set; }

        public DbSet<UserEntity> UserEntities { get; protected set; }

        public DbSet<ConfigEntity> Config { get; protected set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserEntity>(e =>
            {
                e.Property(x => x.AadUserId).IsRequired();
                e.Property(x => x.BotUserId).HasMaxLength(500);
                e.Property(x => x.ConversationId).HasMaxLength(500);
                e.HasKey(x => x.AadUserId);
                e.HasMany(x => x.IncidentTeamMembers).WithOne(x => x.TeamMember).HasForeignKey(x => x.TeamMemberId).OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<IncidentDetailsEntity>(e =>
            {
                e.Property(e => e.Id).ValueGeneratedOnAdd();
                e.Property(e => e.Description);
                e.Property(e => e.IncidentLegacyId);
                e.Property(e => e.IncidentRaised);
                e.Property(e => e.Location);
                e.Property(e => e.Status);
                e.Property(e => e.ThreadLink);
                e.Property(e => e.Title);
                e.Property(e => e.WebEOCLink);

                e.HasKey(x => x.Id);
                e.HasOne(x => x.IncidentManager).WithMany(x => x.IncidentsManagedByUser).HasForeignKey(x => x.IncidentManagerId).OnDelete(DeleteBehavior.NoAction);
                e.HasMany(x => x.IncidentUpdates).WithOne(x => x.ParentIncident).HasForeignKey(x => x.ParentIncidentId).OnDelete(DeleteBehavior.NoAction);
                e.HasMany(x => x.Participants).WithOne(x => x.Incident).HasForeignKey(x => x.IncidentId).OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<ConfigEntity>(e =>
            {
                e.Property(e => e.Key).HasMaxLength(25);
                e.Property(e => e.Value);
                e.HasKey(x => x.Key);
            });

            modelBuilder.Entity<IncidentUpdateEntity>(e =>
            {
                e.Property(e => e.Id).ValueGeneratedOnAdd();
                e.Property(e => e.Title);
                e.Property(e => e.Body);
                e.Property(e => e.CreatedAt);
                e.Property(e => e.UpdateType);

                e.HasOne(x => x.CreatedBy).WithMany(x => x.IncidentUpdatesCreatedByUser).HasForeignKey(x => x.CreatedById).OnDelete(DeleteBehavior.NoAction);
                e.HasOne(x => x.ParentIncident).WithMany(x => x.IncidentUpdates).HasForeignKey(x => x.ParentIncidentId).OnDelete(DeleteBehavior.NoAction);

                e.HasKey(x => x.Id);
            });

            modelBuilder.Entity<IncidentTeamMemberEntity>(e =>
            {
                e.Property(e => e.Id).ValueGeneratedOnAdd();
                e.HasOne(x => x.Incident).WithMany(x => x.Participants).HasForeignKey(x => x.IncidentId).OnDelete(DeleteBehavior.NoAction);
                e.HasOne(x => x.TeamMember).WithMany(x => x.IncidentTeamMembers).HasForeignKey(x => x.TeamMemberId).OnDelete(DeleteBehavior.NoAction);
                e.HasKey(x => x.Id);
            });
        }
    }
}
