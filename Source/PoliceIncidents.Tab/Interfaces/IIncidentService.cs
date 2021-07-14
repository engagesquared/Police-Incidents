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

        Task<bool> UpdateTeamMember(long id, IncidentTeamMemberInput teamMemberInput);

        Task<IncidentModel> GetIncidentById(long id);

        DistrictEntity GetDistricForIncident(long incidentId);

        Task ChangeIncidentFileReportUrl(long incidentId, string fileReportUrl);

        Task<List<IncidentModel>> GetUserIncidents(Guid userId, int pagenumber);

        Task<List<IncidentModel>> GetUserManagedIncidents(Guid userId, int pagenumber);

        Task<List<IncidentModel>> GetTeamIncidents(Guid teamId, int pagenumber);

        Task<List<IncidentModel>> GetClosedTeamIncidents(Guid teamId, int pagenumber);

        Task<long> CreateIncident(IncidentInputModel incident, Guid authorId);

        Task<bool> ReAssignIncident(List<ReAssignIncidentInput> incidentManagerArray);

        Task UpdateDistrictFolder(long districtId, string folderPath);
    }
}