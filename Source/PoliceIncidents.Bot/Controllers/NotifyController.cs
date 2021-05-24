// <copyright file="NotifyController.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PoliceIncidents.Bot.Controllers
{
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

        [HttpGet]
        [Route(Core.Common.Constants.IncidentCreatedBotRoute)]
        public async Task<StatusCodeResult> NewIncidentCreated(int id)
        {

            await this.bot.NotifyAboutIncidentCreated(id);

            // Let the caller know proactive messages have been sent
            return new OkResult();
        }
    }
}
