// <copyright file="IncidentsController.cs" company="Engage Squared">
// Copyright (c) Engage Squared. All rights reserved.
// </copyright>

namespace PoliceIncidents.Controllers
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.Identity.Client;
    using PoliceIncidents.Models;

    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class IncidentsController : BaseController
    {
        private readonly ILogger<UserController> logger;

        public IncidentsController(
            IOptions<AzureAdOptions> azureAdOptions,
            ILogger<UserController> logger,
            IConfidentialClientApplication confidentialClientApp)
            : base(confidentialClientApp, azureAdOptions, logger)
        {
            this.logger = logger;
        }

        [HttpGet("UserIncidents")]
        public async Task<IActionResult> GetUserIncidents()
        {
            try
            {
                string accessToken = await this.GetAccessTokenAsync();
                return this.Ok(accessToken);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"An error occurred in GetToken: {ex.Message}");
                throw;
            }
        }
    }
}