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
        Task ChangeLocation(long incidentId, string location);

        Task ChangeIncidentManager(long incidentId, Guid managerId);

        Task<bool> CloseIncident(long incidentId);

        Task<bool> UpdateTeamMember(long id, IncidentTeamMemberInput teamMemberInput);

        Task<IncidentModel> GetIncidentById(long id);

        Task<List<IncidentModel>> GetUserIncidents(Guid userId);

        Task<List<IncidentModel>> GetUserManagedIncidents(Guid userId);

        Task<List<IncidentModel>> GetTeamIncidents(Guid teamId);

        Task<List<IncidentModel>> GetClosedTeamIncidents(Guid teamId);

        Task<long> CreateIncident(IncidentInputModel incident, Guid authorId);
    }
}