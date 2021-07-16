// <copyright file="IncidentsController.cs" company="Engage Squared">
// Copyright (c) Engage Squared. All rights reserved.
// </copyright>

namespace PoliceIncidents.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using PoliceIncidents.Core.Services;
    using PoliceIncidents.Models;
    using PoliceIncidents.Tab;
    using PoliceIncidents.Tab.Common;
    using PoliceIncidents.Tab.Helpers;
    using PoliceIncidents.Tab.Interfaces;
    using PoliceIncidents.Tab.Models;
    using PoliceIncidents.Tab.Services;

    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class IncidentsController : BaseController
    {
        private readonly ILogger<UserController> logger;
        private readonly IIncidentService incidentService;
        private readonly IIncidentUpdateService incidentUpdateService;
        private readonly AppSettings appSettings;
        private readonly GraphApiService graphApiService;
        private readonly BotNotificationsService botNotificationsService;
        private readonly DeepLinksService deepLinksService;
        private readonly PdfService pdfService;

        public IncidentsController(
            ILogger<UserController> logger,
            IIncidentService incidentService,
            IIncidentUpdateService incidentUpdateService,
            AppSettings appSettings,
            GraphApiService graphApiService,
            BotNotificationsService botNotificationsService,
            DeepLinksService deepLinksService,
            PdfService pdfService)
        {
            this.logger = logger;
            this.incidentService = incidentService;
            this.incidentUpdateService = incidentUpdateService;
            this.appSettings = appSettings;
            this.graphApiService = graphApiService;
            this.botNotificationsService = botNotificationsService;
            this.deepLinksService = deepLinksService;
            this.pdfService = pdfService;
        }

        [HttpPost("")]
        public async Task<long> CreateIncident(IncidentInputModel incident)
        {
            try
            {
                incident.MemberIds = incident.MemberIds ?? new List<Guid>();

                if (incident.GroupIds?.Length > 0)
                {
                    foreach (var groupId in incident.GroupIds)
                    {
                        try
                        {
                            var memberIds = await this.graphApiService.GetUsersByGroupId(groupId.ToString());
                            incident.MemberIds.AddRange(memberIds);
                        }
                        catch (Exception ex)
                        {
                            this.logger.LogError(ex, $"Can't retrieve owners and members from group '{groupId}'");
                        }
                    }

                    incident.MemberIds = incident.MemberIds.Distinct().ToList();
                }

                try
                {
                    incident.PlannerLink = await this.graphApiService.CreateCopyOfPlanner(this.appSettings.PlannerId, incident);
                }
                catch (Exception ex)
                {
                    this.logger.LogError(ex, "Planner creating error");
                }

                var newIncidentId = await this.incidentService.CreateIncident(incident, new Guid(this.UserObjectId));

                // Send async to increase system's response time
                _ = this.botNotificationsService.SendNewIncidentChannelNotification(newIncidentId);
                return newIncidentId;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"An error occurred in CreateIncident {incident.Title}: {ex.Message}");
                throw;
            }
        }

        [HttpGet("user/all/{pageNumber}")]
        public async Task<List<IncidentModel>> GetAllUserIncidents(int pageNumber, [FromQuery]int pageSize = Constants.DefaultPageSize)
        {
            try
            {
                var userId = new Guid(this.UserObjectId);
                return await this.incidentService.GetUserIncidents(userId, pageNumber, pageSize);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"An error occurred in GetAllUserIncidents: {ex.Message}");
                throw;
            }
        }

        [HttpGet("user/managed/{pageNumber}")]
        public async Task<List<IncidentModel>> GetManagedUserIncidents(int pageNumber, [FromQuery]int pageSize = Constants.DefaultPageSize)
        {
            try
            {
                var userId = new Guid(this.UserObjectId);
                return await this.incidentService.GetUserManagedIncidents(userId, pageNumber, pageSize);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"An error occurred in GetManagedUserIncidents: {ex.Message}");
                throw;
            }
        }

        [HttpGet("team/{teamId}/active/{pageNumber}")]
        public async Task<List<IncidentModel>> GetActiveTeamIncidents(Guid teamId, int pageNumber, [FromQuery]int pageSize = Constants.DefaultPageSize)
        {
            try
            {
                return await this.incidentService.GetTeamIncidents(teamId, pageNumber, pageSize);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"An error occurred in GetActiveTeamIncidents: {ex.Message}");
                throw;
            }
        }

        [HttpGet("team/{teamId}/closed/{pageNumber}")]
        public async Task<List<IncidentModel>> GetClosedTeamIncidents(Guid teamId, int pageNumber, [FromQuery]int pageSize = Constants.DefaultPageSize)
        {
            try
            {
                return await this.incidentService.GetClosedTeamIncidents(teamId, pageNumber, pageSize);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"An error occurred in GetClosedTeamIncidents: {ex.Message}");
                throw;
            }
        }

        [HttpGet("{id}")]
        public async Task<IncidentModel> GetIncidentById(long id)
        {
            try
            {
                return await this.incidentService.GetIncidentById(id);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"An error occurred in GetIncidentById: {ex.Message}");
                throw;
            }
        }

        [HttpPost("{id}/manager")]
        public async Task SetIncidentManager(long id, Guid managerId)
        {
            try
            {
                await this.incidentService.ChangeIncidentManager(id, managerId);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"An error occurred in SetIncidentManager {id} {managerId}: {ex.Message}");
                throw;
            }
        }

        [HttpPost("{id}/location")]
        public async Task SetIncidentLocation(long id, IncidentLocationUpdateModel location)
        {
            try
            {
                await this.incidentService.ChangeLocation(id, location.Location);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"An error occurred in SetIncidentLocation {id} {location}: {ex.Message}");
                throw;
            }
        }

        [HttpPost("{id}/members")]
        public async Task AddUpdateTeamMember(long id, List<IncidentMemberInput> members)
        {
            try
            {
                var usersToNotify = await this.incidentService.UpdateTeamMembers(id, members);
                if (usersToNotify.Any())
                {
                    // async for responsibility
                    _ = this.botNotificationsService.SendIncidentRolesPrivateNotification(id, usersToNotify.Select(x => x.AadUserId).ToList());
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"An error occured in UpdateTeamMember {id} ");
                throw;
            }
        }

        [HttpGet("{id}/close")]
        public async Task<bool> CloseIncident(long id)
        {
            try
            {
                await this.incidentService.CloseIncident(id);
                await this.pdfService.GenerateAndUploadPdf(id, true);
                return true;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"An error occurred in CloseIncident: {ex.Message}");
                throw;
            }
        }

        [HttpPost("{id}/generatePdf")]
        public async Task<string> GeneratePdf(long id)
        {
            try
            {
                var pdfDocUrl = await this.pdfService.GenerateAndUploadPdf(id);
                return pdfDocUrl;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"An error occurred in GeneratePdf: {ex.Message}");
                throw;
            }
        }

        [HttpGet("{id}/updates")]
        public async Task<List<IncidentUpdateModel>> GetIncidentUpdatesById(long id)
        {
            try
            {
                return await this.incidentUpdateService.GetIncidentUpdates(id);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"An error occurred in GetIncidentById: {ex.Message}");
                throw;
            }
        }

        [HttpPost("{id}/updates")]
        public async Task<IncidentUpdateModel> AddIncidentUpdate(long id, IncidentUpdateInputModel incidentUpdate)
        {
            try
            {
                incidentUpdate.ParentIncidentId = id;
                incidentUpdate.CreatedByUserId = new Guid(this.UserObjectId);
                return await this.incidentUpdateService.AddIncidentUpdate(incidentUpdate);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"An error occurred in AddIncidentUpdate {id}: {ex.Message}");
                throw;
            }
        }

        [HttpGet("{id}/newMeetingLink")]
        public async Task<string> GetNewMeetigLink(long id)
        {
            try
            {
                var incident = await this.incidentService.GetIncidentById(id);
                var userIds = new List<Guid>();
                if (incident.ManagerId.HasValue)
                {
                    userIds.Add(incident.ManagerId.Value);
                }

                userIds.AddRange(incident.Members.Select(t => t.UserId).ToList());
                userIds = userIds.Distinct().ToList();
                var upns = new List<string>();
                if (userIds.Any())
                {
                    upns = await this.graphApiService.GetUserUpns(userIds);
                }

                return this.deepLinksService.GetNewMeetingLink(incident.Title, upns.ToArray());
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"An error occurred in GetNewMeetigLink {id}: {ex.Message}");
                throw;
            }
        }

        [HttpPost("reassign")]
        public async Task<bool> ReAssignIncidents(List<ReAssignIncidentInput> incidentManagerArray)
        {
            try
            {
                var usersToNotify = await this.incidentService.ReAssignIncidents(incidentManagerArray);
                if (usersToNotify.Any())
                {
                    foreach (var userToNotify in usersToNotify)
                    {
                        // async for responsibility
                        _ = this.botNotificationsService.SendIncidentRolesPrivateNotification(userToNotify.IncidentId, new List<Guid>() { userToNotify.IncidentManagerId });
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"An error occured in ReAssignIncidents");
                throw;
            }
        }
    }
}