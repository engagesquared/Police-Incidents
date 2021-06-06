// <copyright file="IncidentService.cs" company="Engage Squared">
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

    public class IncidentService : IIncidentService
    {
        private readonly ILogger<IncidentService> logger;
        private readonly PoliceIncidentsDbContext dbContext;

        public IncidentService(
             ILogger<IncidentService> logger,
             PoliceIncidentsDbContext dbContext)
        {
            this.logger = logger;
            this.dbContext = dbContext;
        }

        public async Task<IncidentModel> GetIncidentById(long id)
        {
            try
            {
                var userIncidentsQuery = this.dbContext.IncidentDetails
                    .Where(v => v.Id == id)
                    .Include(v => v.Updates).Include(v => v.Participants).Include(x => x.District);
                var incidents = await userIncidentsQuery
                    .Select(v => v.ToIncidentModel(Common.Constants.MaxUpdatesInIncedentItem))
                    .FirstOrDefaultAsync();
                return incidents;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Failed to get inciden by id {id}.");
                throw;
            }
        }

        public async Task<List<IncidentModel>> GetUserIncidents(Guid userId)
        {
            try
            {
                var userIncidentsQuery = this.dbContext.IncidentDetails
                    .Where(v => v.Status != IncidentStatus.Closed)
                    .Where(v => v.Participants.Any(p => p.TeamMemberId == userId))
                    .Include(v => v.Updates).Include(v => v.Participants).Include(x => x.District);
                var incidents = await userIncidentsQuery
                    .Select(v => v.ToIncidentModel(Common.Constants.MaxUpdatesInIncedentList))
                    .ToListAsync();
                return incidents;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Failed to get user incidents. User id: {userId}");
                throw;
            }
        }

        public async Task<List<IncidentModel>> GetTeamIncidents(Guid teamId)
        {
            try
            {
                var incidentsQuery = this.dbContext.IncidentDetails
                    .Where(v => v.Status != IncidentStatus.Closed)
                    .Where(v => v.District.TeamGroupId == teamId)
                    .Include(v => v.Updates).Include(x => x.District);
                var incidents = await incidentsQuery
                    .Select(v => v.ToIncidentModel(Common.Constants.MaxUpdatesInIncedentList))
                    .ToListAsync();
                return incidents;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Failed to get team incidents. Team id: {teamId}");
                throw;
            }
        }

        public async Task ChangeIncidentManager(long incidentId, Guid managerId)
        {
            try
            {
                var incidentQuery = this.dbContext.IncidentDetails.Where(v => v.Id == incidentId);
                var incident = await incidentQuery.FirstOrDefaultAsync();
                if (incident != null)
                {
                    incident.ManagerId = managerId;
                    this.dbContext.Update(incident);
                    await this.dbContext.SaveChangesAsync();
                }
                else
                {
                    this.logger.LogError($"Failed to find incident by id: {incidentId}");
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Failed to change incident manager. Incident id: {incidentId} ManagerId: {managerId}");
                throw;
            }
        }

        public async Task<long> CreateIncident(IncidentInputModel incident, Guid authorId)
        {
            try
            {
                var newIncident = this.MapFromInputModel(incident);
                var districtId = await this.GetDistrictByRegionAsync(incident.RegionId);

                var manager = await this.EnsureUserAsync(incident.ManagerId);
                newIncident.Manager = manager;

                var author = await this.EnsureUserAsync(authorId);
                newIncident.CreatedById = authorId;

                if (incident.Members != null && incident.Members.Any())
                {
                    foreach (var memberId in incident.Members)
                    {
                        var member = await this.EnsureUserAsync(memberId);
                        newIncident.Participants.Add(new IncidentTeamMemberEntity { TeamMember = member, UserRoleId = 3 });
                    }
                }

                newIncident.DistrictId = districtId;
                this.dbContext.IncidentDetails.Add(newIncident);
                await this.dbContext.SaveChangesAsync();
                return newIncident.Id;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Failed to create incident {incident.Title} ");
                throw;
            }
        }

        private async Task<long> GetDistrictByRegionAsync(Guid teamRegionId)
        {
            var district = this.dbContext.Districts.FirstOrDefault(v => v.TeamGroupId == teamRegionId);
            if (district == null)
            {
                this.logger.LogInformation($"District with group Id ${teamRegionId} doesn't exist. Creating one.");
                district = new DistrictEntity
                {
                    TeamGroupId = teamRegionId,
                };
                await this.dbContext.Districts.AddAsync(district);
                await this.dbContext.SaveChangesAsync();
            }

            return district.Id;
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

        private IncidentDetailsEntity MapFromInputModel(IncidentInputModel model)
        {
            var newIncident = new IncidentDetailsEntity();
            newIncident.Title = model.Title;
            newIncident.PlannerLink = model.PlannerLink;
            newIncident.Location = model.Location;
            newIncident.Description = model.Description;
            newIncident.Status = IncidentStatus.New;
            newIncident.CreatedUtc = DateTime.UtcNow;
            return newIncident;
        }
    }
}
