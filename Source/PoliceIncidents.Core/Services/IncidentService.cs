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

        public async Task<long> CreateIncident(IncidentInputModel model)
        {
            IncidentDetailsEntity newIncident = this.MapFromInputModel(model);
            this.dbContext.IncidentDetails.Add(newIncident);
            await this.dbContext.SaveChangesAsync();
            return newIncident.Id;
        }

        public NewIncidentInfoModel Get(long incidentId)
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
