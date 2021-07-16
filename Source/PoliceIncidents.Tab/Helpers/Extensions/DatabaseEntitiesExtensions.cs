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
    }
}
