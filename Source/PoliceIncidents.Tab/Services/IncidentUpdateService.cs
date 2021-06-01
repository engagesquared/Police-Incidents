// <copyright file="IncidentUpdateService.cs" company="Engage Squared">
// Copyright (c) Engage Squared. All rights reserved.
// </copyright>

namespace PoliceIncidents.Tab.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using PoliceIncidents.Core.DB;
    using PoliceIncidents.Core.DB.Entities;
    using PoliceIncidents.Tab.Helpers.Extensions;
    using PoliceIncidents.Tab.Interfaces;
    using PoliceIncidents.Tab.Models;

    public class IncidentUpdateService : IIncidentUpdateService
    {
        private readonly ILogger<IncidentUpdateService> logger;
        private readonly PoliceIncidentsDbContext dbContext;

        public IncidentUpdateService(
            ILogger<IncidentUpdateService> logger,
            PoliceIncidentsDbContext dbContext)
        {
            this.logger = logger;
            this.dbContext = dbContext;
        }

        public async Task<IncidentUpdateModel> AddIncidentUpdate(IncidentUpdateInputModel incidentUpdate)
        {
            try
            {
                var user = await this.EnsureUserAsync(incidentUpdate.CreatedByUserId);

                var newIncidentUpdate = new IncidentUpdateEntity();

                newIncidentUpdate.CreatedBy = user;
                newIncidentUpdate.CreatedAt = DateTime.UtcNow;
                newIncidentUpdate.Title = incidentUpdate.Title;
                newIncidentUpdate.Body = incidentUpdate.Body;
                newIncidentUpdate.ParentIncidentId = incidentUpdate.ParentIncidentId;
                newIncidentUpdate.UpdateType = incidentUpdate.UpdateType;

                await this.dbContext.IncidentUpdates.AddAsync(newIncidentUpdate);
                await this.dbContext.SaveChangesAsync();
                return newIncidentUpdate.ToIncidentUpdateModel();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Failed to create incident update for incident id {incidentUpdate?.ParentIncidentId}. Update title: {incidentUpdate.Title}");
                throw;
            }
        }

        public async Task<List<IncidentUpdateModel>> GetIncidentUpdates(long incidentId)
        {
            try
            {
                var query = this.dbContext.IncidentUpdates.Where(v => v.ParentIncidentId == incidentId);
                var updates = await query.Select(v => v.ToIncidentUpdateModel()).ToListAsync();
                return updates;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Failed to get incident update for incident id {incidentId}");
                throw;
            }
        }

        private async Task<UserEntity> EnsureUserAsync(Guid userId)
        {
            var userQuery = this.dbContext.UserEntities.Where(x => x.AadUserId == userId);
            var user = userQuery.FirstOrDefault();
            if (user == null)
            {
                user = new UserEntity
                {
                    AadUserId = userId,
                };

                this.dbContext.Add(user);
                await this.dbContext.SaveChangesAsync();
            }

            return user;
        }
    }
}
