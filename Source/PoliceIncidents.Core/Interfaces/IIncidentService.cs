// <copyright file="IIncidentService.cs" company="Engage Squared">
// Copyright (c) Engage Squared. All rights reserved.
// </copyright>

namespace PoliceIncidents.Core.Interfaces
{
    using PoliceIncidents.Core.DB;
    using PoliceIncidents.Core.Models;

    public interface IIncidentService
    {
        void CreateIncident(IncidentInputModel model, PoliceIncidentsDbContext dbContext);
    }
}