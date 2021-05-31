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
            result.ManagerId = incidentEntity.ManagerId;
            result.Description = incidentEntity.Description;
            result.Created = incidentEntity.CreatedUtc;
            result.Location = incidentEntity.Location;
            result.Status = incidentEntity.Status;
            result.ExternalLink = incidentEntity.ExternalLink;
            result.Members = incidentEntity.Participants?.Select(x => x.TeamMemberId).ToList();
            result.IncidentUpdates = incidentEntity.Updates
                .OrderBy(u => u.CreatedAt)
                .Take(limitOfUpdates)
                .Select(v => v.ToIncidentUpdateModel())
                .ToList();
            return result;
        }
    }
}
