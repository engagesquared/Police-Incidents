// <copyright file="DatabaseEntitiesExtensions.cs" company="Engage Squared">
// Copyright (c) Engage Squared. All rights reserved.
// </copyright>

namespace PoliceIncidents.Tab.Helpers.Extensions
{
    using System;
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
            result.CreatedAt = DateTime.SpecifyKind(incidentUpdateEntity.CreatedAt, DateTimeKind.Utc);
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
            result.Created = DateTime.SpecifyKind(incidentEntity.CreatedUtc, DateTimeKind.Utc);
            result.Location = incidentEntity.Location;
            result.Status = incidentEntity.Status;
            result.ExternalLink = incidentEntity.ExternalLink;
            result.ReportsFolderPath = incidentEntity.District?.RootFolderPath + incidentEntity.FileReportFolderName;
            result.ReportsFolderName = incidentEntity.FileReportFolderName;
            result.ChatConverstaionId = incidentEntity.ChatConverstaionId;
            result.ChatThreadLink = incidentEntity.District == null
                ? string.Empty : $"https://teams.microsoft.com/l/message/{incidentEntity.District.ConversationId}/{incidentEntity.ChatConverstaionId}";
            result.PlannerLink = incidentEntity.PlannerLink;
            result.Members = incidentEntity.Participants?.Select(v => new Tuple<Guid, int>(v.TeamMemberId, v.UserRoleId)).ToList();
            result.IncidentUpdates = incidentEntity.Updates
                .OrderByDescending(u => u.CreatedAt)
                .Take(limitOfUpdates)
                .Select(v => v.ToIncidentUpdateModel())
                .ToList();
            return result;
        }
    }
}
