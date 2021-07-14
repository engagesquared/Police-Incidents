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

        public async Task ChangeIncidentFileReportUrl(long incidentId, string fileReportUrl)
        {
            try
            {
                var incidentQuery = this.dbContext.IncidentDetails.Where(v => v.Id == incidentId);
                var incident = await incidentQuery.FirstOrDefaultAsync();
                if (incident != null)
                {
                    incident.FileReportFolderName = fileReportUrl;
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
                this.logger.LogError(ex, $"Failed to change incident file report url. Incident id: {incidentId} FileReportUrl: {fileReportUrl}");
                throw;
            }
        }

        public async Task UpdateDistrictFolder(long districtId, string folderPath)
        {
            try
            {
                var district = this.dbContext.Districts.FirstOrDefault(x => x.Id == districtId);
                if (district == null)
                {
                    this.logger.LogWarning($"No district was found with '{districtId}' Id.");
                    return;
                }

                district.RootFolderPath = folderPath;
                await this.dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Failed to update incident {districtId}");
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


        public async Task<bool> ReAssignIncident(List<ReAssignIncidentInput> incidentManagerArray)
        {
            try
            {
                foreach (ReAssignIncidentInput incidentManager in incidentManagerArray)
                {
                    var incidentQuery = this.dbContext.IncidentDetails.Where(v => v.Id == incidentManager.IncidentId).Include(x => x.District);
                    var incident = await incidentQuery.FirstOrDefaultAsync();
                    if (incident != null)
                    {
                        var manager = await this.EnsureUserAsync(incidentManager.IncidentManagerId);
                        incident.Manager = manager;
                        this.dbContext.Update(incident);
                        await this.dbContext.SaveChangesAsync();
                        //TODO: 
                        //await this.SendTeamsNotification(accessToken, incident.District.TeamGroupId.ToString(), incidentManager.IncidentManagerId.ToString(), incident.Title);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Failed to Re Assign Incident");
                throw;
            }
        }

        public async Task<bool> UpdateTeamMember(long incidentId, IncidentTeamMemberInput teamMembers)
        {
            try
            {
                var incidentQuery = this.dbContext.IncidentDetails.Where(v => v.Id == incidentId);
                var incident = await incidentQuery.FirstOrDefaultAsync();
                if (incident != null)
                {
                    incident.ManagerId = teamMembers.IncidentManager;

                    var incidentTeams = this.dbContext.IncidentTeamMembers.Where(t => t.IncidentId == incidentId);
                    var incidentTeamMembers = await incidentTeams.ToListAsync();
                    this.dbContext.IncidentTeamMembers.RemoveRange(incidentTeamMembers);
                    if (teamMembers.FieldOfficer != null)
                    {
                        foreach (Guid temp in teamMembers.FieldOfficer)
                        {
                            var fieldOfficer = await this.EnsureUserAsync(temp);
                            incident.Participants.Add(new IncidentTeamMemberEntity { TeamMember = fieldOfficer, UserRoleId = 1 });
                        }
                    }

                    if (teamMembers.ExternalAgency != null)
                    {
                        foreach (Guid temp in teamMembers.ExternalAgency)
                        {
                            var fieldOfficer = await this.EnsureUserAsync(temp);
                            incident.Participants.Add(new IncidentTeamMemberEntity { TeamMember = fieldOfficer, UserRoleId = 2 });
                        }
                    }

                    if (teamMembers.SocLead != null)
                    {
                        foreach (Guid temp in teamMembers.SocLead)
                        {
                            var fieldOfficer = await this.EnsureUserAsync(temp);
                            incident.Participants.Add(new IncidentTeamMemberEntity { TeamMember = fieldOfficer, UserRoleId = 3 });
                        }
                    }

                    if (teamMembers.FamilyLiason != null)
                    {
                        foreach (Guid temp in teamMembers.FamilyLiason)
                        {

                            var fieldOfficer = await this.EnsureUserAsync(temp);
                            incident.Participants.Add(new IncidentTeamMemberEntity { TeamMember = fieldOfficer, UserRoleId = 4 });
                        }
                    }

                    this.dbContext.Update(incident);

                    // this.dbContext.Update(incidentTeamMember);
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

                if (incident.MemberIds?.Count > 0)
                {
                    incident.MemberIds = incident.MemberIds.Distinct().ToList();
                    foreach (var userId in incident.MemberIds)
                    {
                        var user = await this.EnsureUserAsync(userId);
                        newIncident.Participants.Add(new IncidentTeamMemberEntity { TeamMember = user, UserRoleId = 1 });
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
