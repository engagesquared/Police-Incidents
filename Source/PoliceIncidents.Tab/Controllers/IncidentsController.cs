// <copyright file="IncidentsController.cs" company="Engage Squared">
// Copyright (c) Engage Squared. All rights reserved.
// </copyright>

namespace PoliceIncidents.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.Identity.Client;
    using PoliceIncidents.Helpers;
    using PoliceIncidents.Models;
    using PoliceIncidents.Tab;
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
        private readonly HttpClient httpClient;
        private readonly IIncidentUpdateService incidentUpdateService;
        private readonly AppSettings appSettings;
        private readonly GraphApiService graphApiService;

        public IncidentsController(
            IOptions<AzureAdOptions> azureAdOptions,
            ILogger<UserController> logger,
            IIncidentService incidentService,
            IIncidentUpdateService incidentUpdateService,
            IConfidentialClientApplication confidentialClientApp,
            HttpClient httpClient,
            AppSettings appSettings,
            GraphApiService graphApiService)
        {
            this.logger = logger;
            this.incidentService = incidentService;
            this.incidentUpdateService = incidentUpdateService;
            this.httpClient = httpClient;
            this.appSettings = appSettings;
            this.graphApiService = graphApiService;
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
                try
                {
                    var botNotifyPath = Core.Common.Constants.IncidentCreatedBotRoute.Replace("{id}", newIncidentId.ToString());
                    await this.httpClient.GetAsync(this.appSettings.BotBaseUrl.Trim(new[] { '/' }) + botNotifyPath);
                }
                catch (Exception ex)
                {
                    this.logger.LogError(ex, "Notifying bot error.");
                }

                return newIncidentId;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"An error occurred in CreateIncident {incident.Title}: {ex.Message}");
                throw;
            }
        }

        [HttpGet("user/all/{pagenumber}")]
        public async Task<List<IncidentModel>> GetAllUserIncidents(int pagenumber)
        {
            try
            {
                var userId = new Guid(this.UserObjectId);
                return await this.incidentService.GetUserIncidents(userId, pagenumber);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"An error occurred in GetAllUserIncidents: {ex.Message}");
                throw;
            }
        }

        [HttpGet("user/managed/{pagenumber}")]
        public async Task<List<IncidentModel>> GetManagedUserIncidents(int pagenumber)
        {
            try
            {
                var userId = new Guid(this.UserObjectId);
                return await this.incidentService.GetUserManagedIncidents(userId, pagenumber);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"An error occurred in GetManagedUserIncidents: {ex.Message}");
                throw;
            }
        }

        [HttpGet("team/{teamId}/active/{pagenumber}")]
        public async Task<List<IncidentModel>> GetActiveTeamIncidents(Guid teamId, int pagenumber)
        {
            try
            {
                return await this.incidentService.GetTeamIncidents(teamId, pagenumber);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"An error occurred in GetActiveTeamIncidents: {ex.Message}");
                throw;
            }
        }

        [HttpGet("team/{teamId}/closed/{pagenumber}")]
        public async Task<List<IncidentModel>> GetClosedTeamIncidents(Guid teamId, int pagenumber)
        {
            try
            {
                return await this.incidentService.GetClosedTeamIncidents(teamId, pagenumber);
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
        public async Task SetIncidentManager(long id, string managerId)
        {
            try
            {
                await this.incidentService.ChangeIncidentManager(id, Guid.Parse(managerId));
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

        [HttpPost("{id}/updatemember")]
        public async Task<bool> AddUpdateTeamMember(long id, IncidentTeamMemberInput teamMemberInput)
        {
            try
            {
                return await this.incidentService.UpdateTeamMember(id, teamMemberInput);
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
                return await this.incidentService.CloseIncident(id);
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
                var incident = await this.incidentService.GetIncidentById(id);
                var district = this.incidentService.GetDistricForIncident(id);
                if (string.IsNullOrEmpty(district.RootFolderPath))
                {
                    var rfolder = await this.graphApiService.GetRootDriveUrl(district.TeamGroupId?.ToString());
                    await this.incidentService.UpdateDistrictFolder(district.Id, rfolder);
                }

                var chatMessage = await this.graphApiService.GetChatMessage(district.TeamGroupId.ToString(), district.ConversationId, incident.ChatConverstaionId);
                var chatMessageReplies = await this.graphApiService.GetChatMessageReplies(district.TeamGroupId.ToString(), district.ConversationId, incident.ChatConverstaionId);
                var pdfBytes = PdfGenerator.PrepareDocument(chatMessage, chatMessageReplies);
                MemoryStream stream = new MemoryStream(pdfBytes);
                var folder = incident.ReportsFolderName ?? $"/IncidentReports/{incident.Id}-{new Regex("[\"*:<>?\\/\\|]").Replace(incident.Title, string.Empty)}";
                if (folder.Length > 80)
                {
                    folder = folder.Substring(0, 80);
                }

                if (!folder.Equals(incident.ReportsFolderName))
                {
                    await this.incidentService.ChangeIncidentFileReportUrl(id, folder);
                }

                var pdfDocUrl = await this.graphApiService.UploadFileToTeams(district.TeamGroupId.ToString(), $"{folder}/Report-{DateTime.Now.ToString("yyyyMMdd_HHmm")}.pdf", stream);
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

                userIds.AddRange(incident.Members.Select(t => t.Item1).ToList());
                userIds = userIds.Distinct().ToList();
                var upns = new List<string>();
                if (userIds.Any())
                {
                    upns = await this.graphApiService.GetUserUpns(userIds);
                }

                return $"https://teams.microsoft.com/l/meeting/new?subject={Uri.EscapeDataString(incident.Title)}&attendees={string.Join(',', upns)}";
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"An error occurred in GetNewMeetigLink {id}: {ex.Message}");
                throw;
            }
        }

        [HttpPost("reassignincident")]
        public async Task<bool> ReAssignIncident(List<ReAssignInput> incidentManagerArray)
        {
            try
            {
                return await this.incidentService.ReAssignIncident(incidentManagerArray);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"An error occured in ReAssignIncident");
                throw;
            }
        }
    }
}