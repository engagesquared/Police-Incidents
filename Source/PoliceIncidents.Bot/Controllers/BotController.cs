// <copyright file="BotController.cs" company="Engage Squared">
// Copyright (c) Engage Squared. All rights reserved.
// </copyright>

namespace PoliceIncidents.Bot.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Bot.Builder.Integration.AspNet.Core;
    using PoliceIncidents.Bot.Bots;

    [Route("api/messages")]
    [ApiController]
    public class BotController : ControllerBase
    {
        private readonly IBotFrameworkHttpAdapter adapter;
        private readonly PoliceIncidentsBot bot;

        public BotController(IBotFrameworkHttpAdapter adapter, PoliceIncidentsBot bot)
        {
            this.adapter = adapter;
            this.bot = bot;
        }

        [HttpPost]
        public async Task PostAsync()
        {
            // Delegate the processing of the HTTP POST to the adapter.
            // The adapter will invoke the bot.
            await this.adapter.ProcessAsync(this.Request, this.Response, this.bot);
        }
    }
}
