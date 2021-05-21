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

        public DbSet<IncidentDetails> IncidentDetails { get; protected set; }

        public DbSet<IncidentUpdate> IncidentUpdates { get; protected set; }

        public DbSet<IncidentTeamMember> IncidentTeamMembers { get; protected set; }

        public DbSet<UserEntity> UserEntities { get; protected set; }

        public DbSet<ConfigEntity> Config { get; protected set; }
    }
}
