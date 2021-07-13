// <copyright file="UserController.cs" company="Engage Squared">
// Copyright (c) Engage Squared. All rights reserved.
// </copyright>

namespace PoliceIncidents.Controllers
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Identity.Client;
    using PoliceIncidents.Tab.Authentication;
    using PoliceIncidents.Tab.Models;

    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseController
    {
        private readonly ILogger<UserController> logger;
        private readonly TokenAcquisitionService tokenAcquisitionService;

        public UserController(
            ILogger<UserController> logger,
            TokenAcquisitionService tokenAcquisitionService)
        {
            this.logger = logger;
            this.tokenAcquisitionService = tokenAcquisitionService;
        }

        [HttpGet("GetToken")]
        public async Task<GraphTokenModel> GetToken()
        {
            try
            {
                var accessToken = await this.tokenAcquisitionService.GetAccessTokenAsync();
                return new GraphTokenModel
                {
                    Token = accessToken.AccessToken,
                    ExpiresOn = accessToken.ExpiresOn,
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"An error occurred in GetToken: {ex.Message}");
                throw;
            }
        }
    }
}