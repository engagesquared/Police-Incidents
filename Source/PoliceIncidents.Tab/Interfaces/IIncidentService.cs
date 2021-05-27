// <copyright file="IIncidentService.cs" company="Engage Squared">
// Copyright (c) Engage Squared. All rights reserved.
// </copyright>

namespace PoliceIncidents.Tab.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using PoliceIncidents.Tab.Models;

    public interface IIncidentService
    {
        Task ChangeIncidentManager(long incidentId, Guid managerId);

        Task<IncidentModel> GetIncidentById(long id);

        Task<List<IncidentModel>> GetUserIncidents(Guid userId);
    }
}