// <copyright file="IncidentService.cs" company="Engage Squared">
// Copyright (c) Engage Squared. All rights reserved.
// </copyright>

namespace PoliceIncidents.Core.Services
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using PoliceIncidents.Core.DB;
    using PoliceIncidents.Core.DB.Entities;
    using PoliceIncidents.Core.Interfaces;
    using PoliceIncidents.Core.Models;

    public class IncidentService : IIncidentService
    {
        private readonly PoliceIncidentsDbContext dbContext;
        private readonly ILogger<IncidentService> logger;

        public IncidentService(
            PoliceIncidentsDbContext dbContext,
            ILogger<IncidentService> logger)
        {
            this.dbContext = dbContext;
            this.logger = logger;
        }

        public async Task CreateDistrict(Guid groupId, string teamName, string conversationId)
        {
            try
            {
                var district = this.dbContext.Districts.Where(v => v.TeamGroupId == groupId).FirstOrDefault();
                if (district == null)
                {
                    var newDistrict = new DistrictEntity()
                    {
                        RegionName = teamName,
                        TeamGroupId = groupId,
                        ConversationId = conversationId,
                    };
                    this.dbContext.Districts.Add(newDistrict);
                    await this.dbContext.SaveChangesAsync();
                } else if (district.ConversationId != conversationId)
                {
                    district.ConversationId = conversationId;
                    await this.dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Failed to create district for team {groupId} {teamName}");
                throw;
            }
        }

        public async Task<long> CreateIncident(IncidentInputModel model)
        {
            try
            {
                IncidentDetailsEntity newIncident = this.MapFromInputModel(model);
                var districtId = await this.GetDistrictByRegionAsync(model.Region);
                if (model.ManagerId.HasValue)
                {
                    var manager = await this.EnsureUserAsync(model.ManagerId.Value);
                    newIncident.Manager = manager;
                }

                if (model.Author.HasValue)
                {
                    var author = await this.EnsureUserAsync(model.Author.Value);
                    newIncident.CreatedById = model.Author;
                }

                newIncident.DistrictId = districtId;
                this.dbContext.IncidentDetails.Add(newIncident);
                await this.dbContext.SaveChangesAsync();
                return newIncident.Id;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Failed to create incident {model.ExternalId} {model.Title} {model.ExternalLink}");
                throw;
            }
        }

        public async Task UpdateIncidentConversationId(long incidentId, string conversationId)
        {
            try
            {
                var incident = this.dbContext.IncidentDetails.Where(v => v.Id == incidentId).FirstOrDefault();
                if (incident == null)
                {
                    this.logger.LogWarning($"No incident was found with '{incidentId}' Id.");
                    return;
                }

                incident.ChatConverstaionId = conversationId;
                await this.dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Failed to update incident {incidentId}");
                throw;
            }
        }

        public NewIncidentInfoModel GetIncident(long incidentId)
        {
            try
            {
                var incident = this.dbContext.IncidentDetails.Where(v => v.Id == incidentId).FirstOrDefault();
                NewIncidentInfoModel result = null;
                if (incident != null)
                {
                    result = this.MapFromEntity(incident);
                }

                return result;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Failed to get incident {incidentId} from database");
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

        /// <summary>
        /// Create if not exist district.
        /// </summary>
        /// <param name="region">Name of district.</param>
        /// <returns>District id.</returns>
        private async Task<long> GetDistrictByRegionAsync(string region, Guid? teamRegionId = null)
        {
            try
            {
                var districtIdQuery = this.dbContext.Districts.Where(v => v.RegionName == region).Select(v => v.Id);
                if (teamRegionId.HasValue)
                {
                    var tid = teamRegionId.Value;
                    districtIdQuery = this.dbContext.Districts.Where(v => v.TeamGroupId == tid).Select(v => v.Id);
                }

                var districts = districtIdQuery.ToList();
                var districtId = districts.FirstOrDefault();
                if (!districts.Any())
                {
                    this.logger.LogInformation($"No district was found with '{region}' name and/or {teamRegionId} team Id. Trying to return default district");
                    districts = this.dbContext.Districts.Where(v => v.IsDefault).Select(v => v.Id).ToList();
                    districtId = districts.FirstOrDefault();
                    if (!districts.Any())
                    {
                        this.logger.LogInformation($"No default district was found.");
                        this.logger.LogInformation($"Trying to create draft district for incident.");
                        var newDistrict = new DistrictEntity()
                        {
                            RegionName = region,
                            TeamGroupId = teamRegionId,
                        };
                        this.dbContext.Districts.Add(newDistrict);
                        await this.dbContext.SaveChangesAsync();
                        districtId = newDistrict.Id;
                    }
                }

                return districtId;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Failed to ensure district with name: {region}");
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

        private NewIncidentInfoModel MapFromEntity(IncidentDetailsEntity incident)
        {
            return new NewIncidentInfoModel()
            {
                Title = incident.Title,
                Description = incident.Description,
                Link = incident.ExternalLink,
            };
        }

        private IncidentDetailsEntity MapFromInputModel(IncidentInputModel model)
        {
            var newIncident = new IncidentDetailsEntity();
            newIncident.Title = model.Title;
            newIncident.ExternalId = model.ExternalId;
            newIncident.Location = model.Location;
            newIncident.Description = model.Description;
            newIncident.Status = IncidentStatus.New;
            newIncident.CreatedUtc = DateTime.UtcNow;
            newIncident.ExternalLink = model.ExternalLink;
            return newIncident;
        }

    }
}
