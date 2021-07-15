// <copyright file="NotifyController.cs" company="Engage Squared">
// Copyright (c) Engage Squared. All rights reserved.
// </copyright>

namespace PoliceIncidents.Bot.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using PoliceIncidents.Bot.Bots;

    [ApiController]
    public class NotifyController
    {
        private readonly ProactiveBot bot;

        public NotifyController(ProactiveBot bot)
        {
            this.bot = bot;
        }

        [HttpPost]
        [Route(Core.Common.Constants.IncidentCreatedBotRoute)]
        public async Task<StatusCodeResult> NewIncidentCreated(int incidentId)
        {
            await this.bot.NotifyAboutIncidentCreated(incidentId);

            // Let the caller know proactive messages have been sent
            return new OkResult();
        }

        [HttpPost]
        [Route(Core.Common.Constants.IncidentRolesBotRoute)]
        public async Task<StatusCodeResult> NotifyNewRoles(int incidentId, List<Guid> usersToNotifyIds)
        {
            await this.bot.NotifyAboutIncidentRoles(incidentId, usersToNotifyIds);

            // Let the caller know proactive messages have been sent
            return new OkResult();
        }
    }
}
