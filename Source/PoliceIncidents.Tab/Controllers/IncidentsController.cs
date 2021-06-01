// <copyright file="IncidentsController.cs" company="Engage Squared">
// Copyright (c) Engage Squared. All rights reserved.
// </copyright>

namespace PoliceIncidents.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.Identity.Client;
    using PoliceIncidents.Helpers;
    using PoliceIncidents.Models;
    using PoliceIncidents.Tab;
    using PoliceIncidents.Tab.Interfaces;
    using PoliceIncidents.Tab.Models;

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

        public IncidentsController(
            IOptions<AzureAdOptions> azureAdOptions,
            ILogger<UserController> logger,
            IIncidentService incidentService,
            IIncidentUpdateService incidentUpdateService,
            IConfidentialClientApplication confidentialClientApp,
            HttpClient httpClient,
            AppSettings appSettings)
            : base(confidentialClientApp, azureAdOptions, logger)
        {
            this.logger = logger;
            this.incidentService = incidentService;
            this.incidentUpdateService = incidentUpdateService;
            this.httpClient = httpClient;
            this.appSettings = appSettings;
        }

        [HttpPost("")]
        public async Task<long> CreateIncident(IncidentInputModel incident)
        {
            try
            {
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

        [HttpGet("user/all")]
        public async Task<List<IncidentModel>> GetAllUserIncidents()
        {
            try
            {
                var userId = new Guid(this.UserObjectId);
                return await this.incidentService.GetUserIncidents(userId);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"An error occurred in GetAllUserIncidents: {ex.Message}");
                throw;
            }
        }

        [HttpGet("user/managed")]
        public async Task<List<IncidentModel>> GetManagedUserIncidents()
        {
            try
            {
                var userId = new Guid(this.UserObjectId);
                return await this.incidentService.GetUserIncidents(userId);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"An error occurred in GetManagedUserIncidents: {ex.Message}");
                throw;
            }
        }

        [HttpGet("team/{teamId}/active")]
        public async Task<List<IncidentModel>> GetActiveTeamIncidents(Guid teamId)
        {
            try
            {
                return await this.incidentService.GetTeamIncidents(teamId);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"An error occurred in GetActiveTeamIncidents: {ex.Message}");
                throw;
            }
        }

        [HttpGet("team/{teamId}/closed")]
        public async Task<List<IncidentModel>> GetClosedTeamIncidents(Guid teamId)
        {
            try
            {
                return await this.incidentService.GetTeamIncidents(teamId);
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

                userIds.AddRange(incident.Members);
                userIds = userIds.Distinct().ToList();
                var upns = new List<string>();
                if (userIds.Any()) {
                    var token = await this.GetAccessTokenAsync();
                    var graphHelper = new GraphUtilityHelper(token);
                    upns = await graphHelper.GetUserUpns(userIds);
                }

                return $"https://teams.microsoft.com/l/meeting/new?subject={Uri.EscapeDataString(incident.Title)}&attendees={string.Join(',', upns)}";
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"An error occurred in GetNewMeetigLink {id}: {ex.Message}");
                throw;
            }
        }
    }
}