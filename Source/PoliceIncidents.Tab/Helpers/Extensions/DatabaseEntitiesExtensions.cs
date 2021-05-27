// <copyright file="DatabaseEntitiesExtensions.cs" company="Engage Squared">
// Copyright (c) Engage Squared. All rights reserved.
// </copyright>

namespace PoliceIncidents.Tab.Helpers.Extensions
{
    using System.Linq;
    using PoliceIncidents.Core.DB.Entities;
    using PoliceIncidents.Tab.Models;

    public static class DatabaseEntitiesExtensions
    {
        public static IncidentUpdateModel ToIncidentUpdateModel(this IncidentUpdateEntity incidentUpdateEntity)
        {
            var result = new IncidentUpdateModel();
            result.Id = incidentUpdateEntity.Id;
            result.Title = incidentUpdateEntity.Title;
            result.Body = incidentUpdateEntity.Body;
            result.CreatedAt = incidentUpdateEntity.CreatedAt;
            result.CreatedById = incidentUpdateEntity.CreatedById;
            result.UpdateType = incidentUpdateEntity.UpdateType;

            return result;
        }

        public static IncidentModel ToIncidentModel(this IncidentDetailsEntity incidentEntity, int limitOfUpdates)
        {
            var result = new IncidentModel();
            result.Title = incidentEntity.Title;
            result.Description = incidentEntity.Description;
            result.Id = incidentEntity.Id;
            result.IncidentManagerId = incidentEntity.IncidentManagerId;
            result.Description = incidentEntity.Description;
            result.IncidentRaised = incidentEntity.IncidentRaised;
            result.Location = incidentEntity.Location;
            result.Status = incidentEntity.Status;
            result.WebEOCLink = incidentEntity.WebEOCLink;
            result.IncidentUpdates = incidentEntity.IncidentUpdates
                .OrderBy(u => u.CreatedAt)
                .Take(limitOfUpdates)
                .Select(v => v.ToIncidentUpdateModel())
                .ToList();
            return result;
        }
    }
}
