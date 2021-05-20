using PoliceIncidents.Core.DB;
using PoliceIncidents.Core.DB.Entities;
using PoliceIncidents.Core.Interfaces;
using PoliceIncidents.Core.Models;
using System;

namespace PoliceIncidents.Core.Services
{
    public class IncidentService : IIncidentService
    {

        public void CreateIncident(IncidentInputModel model, PoliceIncidentsDbContext dbContext)
        {
            IncidentDetails newIncident = Map(model);
            dbContext.IncidentDetails.Add(newIncident);
            dbContext.SaveChanges();
        }

        private static IncidentDetails Map(IncidentInputModel model)
        {
            var newIncident = new IncidentDetails();
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
