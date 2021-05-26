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

        public async Task CreateDistrict(string channelId, string channelName, string conversationId)
        {
            try
            {
                var districts = this.dbContext.Districts.Where(v => v.TeamGroupId == channelId).Select(v => v.Id).ToList();
                if (!districts.Any())
                {
                    var newDistrict = new DistrictEntity()
                    {
                        TeamGroupId = channelId,
                        TeamGroupName = channelName,
                        ConversationId = conversationId,
                    };
                    this.dbContext.Districts.Add(newDistrict);
                    await this.dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Failed to create district for channel {channelId} {channelName}");
                throw;
            }
        }

        public async Task<long> CreateIncident(IncidentInputModel model)
        {
            try
            {
                IncidentDetailsEntity newIncident = this.MapFromInputModel(model);
                var districtId = await this.GetDistrictByRegionAsync(model.Region);
                newIncident.DistrictId = districtId;
                this.dbContext.IncidentDetails.Add(newIncident);
                await this.dbContext.SaveChangesAsync();
                return newIncident.Id;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Failed to create incident {model.Id} {model.Title} {model.WebEOCLink}");
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
        private async Task<long> GetDistrictByRegionAsync(string region)
        {
            try
            {
                var districts = this.dbContext.Districts.Where(v => v.RegionName == region).Select(v => v.Id).ToList();
                var districtId = districts.FirstOrDefault();
                if (!districts.Any())
                {
                    this.logger.LogInformation($"No district was found with '{region}' name. Trying to return default district");
                    districts = this.dbContext.Districts.Where(v => v.IsDefault).Select(v => v.Id).ToList();
                    districtId = districts.FirstOrDefault();
                    if (!districts.Any())
                    {
                        this.logger.LogInformation($"No default district was found.");
                        this.logger.LogInformation($"Trying to create draft district for incident.");
                        var newDistrict = new DistrictEntity()
                        {
                            RegionName = region,
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

        private NewIncidentInfoModel MapFromEntity(IncidentDetailsEntity incident)
        {
            return new NewIncidentInfoModel()
            {
                Title = incident.Title,
                Description = incident.Description,
                Link = incident.WebEOCLink,
            };
        }

        private IncidentDetailsEntity MapFromInputModel(IncidentInputModel model)
        {
            var newIncident = new IncidentDetailsEntity();
            newIncident.Title = model.Title;
            newIncident.IncidentLegacyId = model.Id;
            newIncident.Location = model.Location;
            newIncident.Description = model.Description;
            newIncident.Status = IncidentStatus.New;
            newIncident.IncidentRaised = DateTime.Now.ToUniversalTime();
            newIncident.WebEOCLink = model.WebEOCLink;
            return newIncident;
        }
    }
}
