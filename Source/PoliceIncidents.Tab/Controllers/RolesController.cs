// <copyright file="UserController.cs" company="Engage Squared">
// Copyright (c) Engage Squared. All rights reserved.
// </copyright>

namespace PoliceIncidents.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using PoliceIncidents.Core.DB;
    using PoliceIncidents.Models;

    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : BaseController
    {
        private readonly ILogger<UserController> logger;
        private readonly PoliceIncidentsDbContext dbContext;

        public RolesController(
            ILogger<UserController> logger, PoliceIncidentsDbContext dbContext)
        {
            this.logger = logger;
            this.dbContext = dbContext;
        }

        [HttpGet("")]
        public List<UserRoleModel> GetAll()
        {
            try
            {
                var roles = this.dbContext.UserRoles.ToList();
                return roles.Select(x => new UserRoleModel
                {
                    Id = x.Id,
                    Name = x.Title,
                }).ToList();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"An error occurred in GetAll: {ex.Message}");
                throw;
            }
        }
    }
}