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

        public DistrictEntity GetDistricForIncident(long incidentId)
        {
            try
            {
                var disctrict = this.dbContext.IncidentDetails.Where(v => v.Id == incidentId).Select(v => v.District).FirstOrDefault();
                return disctrict;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Failed to get district for incident {incidentId} from database");
                throw;
            }
        }

        public async Task<List<IncidentModel>> GetUserIncidents(Guid userId, int pagenumber)
        {
            try
            {
                var userIncidentsQuery = this.dbContext.IncidentDetails
                    .Where(v => v.Status != IncidentStatus.Closed)
                    .Where(v => v.Participants.Any(p => p.TeamMemberId == userId) || v.ManagerId == userId)
                    .Skip((pagenumber - 1) * 10).Take(10)
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

        public async Task<List<IncidentModel>> GetUserManagedIncidents(Guid userId, int pagenumber)
        {
            try
            {
                var userIncidentsQuery = this.dbContext.IncidentDetails
                    .Where(v => v.Status != IncidentStatus.Closed)
                    .Where(v => v.ManagerId == userId).Skip((pagenumber - 1) * 10).Take(10)
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

        public async Task<List<IncidentModel>> GetTeamIncidents(Guid teamId, int pagenumber)
        {
            try
            {
                var incidentsQuery = this.dbContext.IncidentDetails
                    .Where(v => v.Status != IncidentStatus.Closed)
                    .Where(v => v.District.TeamGroupId == teamId)
                    .Skip((pagenumber - 1) * 10).Take(10)
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

        public async Task<List<IncidentModel>> GetClosedTeamIncidents(Guid teamId, int pagenumber)
        {
            try
            {
                var incidentsQuery = this.dbContext.IncidentDetails
                    .Where(v => v.Status == IncidentStatus.Closed)
                    .Where(v => v.District.TeamGroupId == teamId).Skip((pagenumber - 1) * 10).Take(10)
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

        public async Task ChangeLocation(long incidentId, string location)
        {
            try
            {
                var incidentQuery = this.dbContext.IncidentDetails.Where(v => v.Id == incidentId);
                var incident = await incidentQuery.FirstOrDefaultAsync();
                if (incident != null)
                {
                    incident.Location = location;
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
                this.logger.LogError(ex, $"Failed to change location. Incident id: {incidentId} Location: {location}");
                throw;
            }
        }

        public async Task<bool> CloseIncident(long incidentId)
        {
            try
            {
                var incidentQuery = this.dbContext.IncidentDetails.Where(v => v.Id == incidentId);
                var incident = await incidentQuery.FirstOrDefaultAsync();
                if (incident != null)
                {
                    incident.Status = IncidentStatus.Closed;
                    this.dbContext.Update(incident);
                    await this.dbContext.SaveChangesAsync();
                    return true;
                }
                else
                {
                    this.logger.LogError($"Failed to find incident by id: {incidentId}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Failed to close incident. Incident id: {incidentId}");
                throw;
            }
        }

        public async Task<List<ReAssignIncidentInput>> ReAssignIncidents(List<ReAssignIncidentInput> incidents)
        {
            try
            {
                var managersToNotify = new List<ReAssignIncidentInput>();
                var incidentsToUpdateIds = incidents.Select(x => x.IncidentId).Distinct().ToArray();
                var incidentsToUpdate = this.dbContext.IncidentDetails.Where(v => incidentsToUpdateIds.Contains(v.Id)).Include(x => x.Manager).ToList();
                var users = await this.EnsureUsersAsync(incidents.Select(x => x.IncidentManagerId).ToArray());
                foreach (var incidentToUpdate in incidentsToUpdate)
                {
                    var request = incidents.First(x => x.IncidentId == incidentToUpdate.Id);
                    var newManager = users.First(x => x.AadUserId == request.IncidentManagerId);
                    if (incidentToUpdate.Manager?.AadUserId != newManager.AadUserId)
                    {
                        incidentToUpdate.Manager = newManager;
                        this.dbContext.Update(incidentToUpdate);
                        managersToNotify.Add(request);
                    }
                }

                await this.dbContext.SaveChangesAsync();
                return managersToNotify;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Failed to Re Assign Incident");
                throw;
            }
        }

        public async Task<List<UserEntity>> UpdateTeamMembers(long incidentId, List<IncidentMemberInput> teamMembersInput)
        {
            var usersToNotify = new List<UserEntity>();
            try
            {
                var incident = this.dbContext.IncidentDetails.Where(v => v.Id == incidentId).Include(x => x.Participants).FirstOrDefault();
                if (incident != null)
                {
                    var users = await this.EnsureUsersAsync(teamMembersInput.Select(x => x.UserId).Distinct().ToArray());

                    var assignmentsToDelete = incident.Participants.Where(x => !users.Any(u => u.AadUserId == x.TeamMemberId)).ToList();
                    this.dbContext.IncidentTeamMembers.RemoveRange(assignmentsToDelete);

                    foreach (var user in users)
                    {
                        var teamMemberInput = teamMembersInput.First(x => x.UserId == user.AadUserId);
                        var participant = incident.Participants.FirstOrDefault(x => x.TeamMemberId == user.AadUserId);
                        if (participant == null)
                        {
                            participant = new IncidentTeamMemberEntity { TeamMember = user };
                            incident.Participants.Add(participant);
                        }

                        if (participant.UserRoleId != teamMemberInput.RoleId)
                        {
                            participant.UserRoleId = teamMemberInput.RoleId;
                            usersToNotify.Add(user);
                        }
                    }

                    this.dbContext.Update(incident);
                    await this.dbContext.SaveChangesAsync();
                }
                else
                {
                    this.logger.LogError($"Failed to find incident by id: {incidentId}");
                }

                return usersToNotify;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Failed to Update Team Member. Incident id: {incidentId}");
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

                var defaultRole = this.dbContext.UserRoles.FirstOrDefault(x => x.IsDefault == true);
                if (defaultRole == null)
                {
                    defaultRole = this.dbContext.UserRoles.FirstOrDefault();
                }

                if (incident.MemberIds?.Count > 0)
                {
                    incident.MemberIds = incident.MemberIds.Where(x => manager.AadUserId != x).Distinct().ToList();
                    foreach (var userId in incident.MemberIds)
                    {
                        var user = await this.EnsureUserAsync(userId);
                        newIncident.Participants.Add(new IncidentTeamMemberEntity { TeamMember = user, UserRole = defaultRole });
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
            return (await this.EnsureUsersAsync(new[] { userId })).FirstOrDefault();
        }

        private async Task<List<UserEntity>> EnsureUsersAsync(Guid[] userIds)
        {
            var users = this.dbContext.UserEntities.Where(x => userIds.Contains(x.AadUserId)).ToList();
            var usersToEnsure = userIds.Where(x => users.All(u => u.AadUserId != x));

            if (usersToEnsure.Any())
            {
                foreach (var userId in usersToEnsure)
                {
                    var newUser = new UserEntity
                    {
                        AadUserId = userId,
                    };

                    this.dbContext.Add(newUser);
                    users.Add(newUser);
                }

                await this.dbContext.SaveChangesAsync();
            }

            return users;
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
