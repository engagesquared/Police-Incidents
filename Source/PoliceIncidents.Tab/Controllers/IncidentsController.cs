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
    using PoliceIncidents.Tab.Models;
    using PoliceIncidents.Tab.Services;

    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class IncidentsController : BaseController
    {
        private readonly ILogger<UserController> logger;
        private readonly PoliceIncidentsDbContext dbContext;

        public IncidentsController(
            IOptions<AzureAdOptions> azureAdOptions,
            ILogger<UserController> logger,
            IConfidentialClientApplication confidentialClientApp, 
            PoliceIncidentsDbContext dbContext)
            : base(confidentialClientApp, azureAdOptions, logger)
        {
            this.logger = logger;
            this.dbContext = dbContext;
        }

        [HttpGet("UserIncidents")]
        public async Task<List<IncidentModel>> GetUserIncidents()
        {
            try
            {
                var userId = new Guid(this.UserObjectId);
                return await new FakeIncidentService().GetUserIncidents(userId);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"An error occurred in GetToken: {ex.Message}");
                throw;
            }
        }
    }
}