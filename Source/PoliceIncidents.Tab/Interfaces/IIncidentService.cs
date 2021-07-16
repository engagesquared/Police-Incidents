// <copyright file="IIncidentService.cs" company="Engage Squared">
// Copyright (c) Engage Squared. All rights reserved.
// </copyright>

namespace PoliceIncidents.Tab.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using PoliceIncidents.Core.DB.Entities;
    using PoliceIncidents.Tab.Models;

    public interface IIncidentService
    {
        Task ChangeLocation(long incidentId, string location);

        Task ChangeIncidentManager(long incidentId, Guid managerId);

        Task<bool> CloseIncident(long incidentId);

        Task<List<UserEntity>> UpdateTeamMembers(long id, List<IncidentMemberInput> teamMembersInput);

        Task<IncidentModel> GetIncidentById(long id);

        Task<List<IncidentModel>> GetUserIncidents(Guid userId, int pagenumber, int pageSize);

        Task<List<IncidentModel>> GetUserManagedIncidents(Guid userId, int pagenumber, int pageSize);

        Task<List<IncidentModel>> GetTeamIncidents(Guid teamId, int pagenumber, int pageSize);

        Task<List<IncidentModel>> GetClosedTeamIncidents(Guid teamId, int pagenumber, int pageSize);

        Task<long> CreateIncident(IncidentInputModel incident, Guid authorId);

        Task<List<ReAssignIncidentInput>> ReAssignIncidents(List<ReAssignIncidentInput> incidents);
    }
}