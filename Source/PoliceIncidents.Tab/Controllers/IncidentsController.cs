// <copyright file="IncidentsController.cs" company="Engage Squared">
// Copyright (c) Engage Squared. All rights reserved.
// </copyright>

namespace PoliceIncidents.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.Identity.Client;
    using PoliceIncidents.Core.DB;
    using PoliceIncidents.Models;
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

        public IncidentsController(
            IOptions<AzureAdOptions> azureAdOptions,
            ILogger<UserController> logger,
            IIncidentService incidentService,
            IIncidentUpdateService incidentUpdateService,
            IConfidentialClientApplication confidentialClientApp)
            : base(confidentialClientApp, azureAdOptions, logger)
        {
            this.logger = logger;
            this.incidentService = incidentService;
            this.incidentUpdateService = incidentUpdateService;
        }

        [HttpGet("UserIncidents")]
        public async Task<List<IncidentModel>> GetUserIncidents()
        {
            try
            {
                var userId = new Guid(this.UserObjectId);
                return await this.incidentService.GetUserIncidents(userId);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"An error occurred in GetUserIncidents: {ex.Message}");
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

        [HttpPost("{id}/SetManager")]
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

        [HttpGet("{id}/Updates")]
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

        [HttpPost("{id}/AddUpdate")]
        public async Task AddIncidentUpdate(long id, IncidentUpdateInputModel incidentUpdate)
        {
            try
            {
                incidentUpdate.ParentIncidentId = id;
                await this.incidentUpdateService.AddIncidentUpdate(incidentUpdate);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"An error occurred in AddIncidentUpdate {id}: {ex.Message}");
                throw;
            }
        }
    }
}