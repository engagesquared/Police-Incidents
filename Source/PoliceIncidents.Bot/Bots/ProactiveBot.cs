// <copyright file="ProactiveBot.cs" company="Engage Squared">
// Copyright (c) Engage Squared. All rights reserved.
// </copyright>

namespace PoliceIncidents.Bot.Bots
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Integration.AspNet.Core;
    using Microsoft.Bot.Schema;
    using Microsoft.Extensions.Logging;
    using PoliceIncidents.Bot.Resources;
    using PoliceIncidents.Core.DB;
    using PoliceIncidents.Core.Interfaces;

    public class ProactiveBot
    {
        private readonly AppSettings appSettings;
        private readonly ILogger<ProactiveBot> logger;
        private readonly IBotFrameworkHttpAdapter adapter;
        private readonly IIncidentService incidentService;
        private readonly PoliceIncidentsDbContext dbContext;

        public ProactiveBot(
            AppSettings appSettings,
            ILogger<ProactiveBot> logger,
            IBotFrameworkHttpAdapter adapter,
            IIncidentService incidentService,
            PoliceIncidentsDbContext dbContext)
        {
            this.appSettings = appSettings;
            this.logger = logger;
            this.adapter = adapter;
            this.incidentService = incidentService;
            this.dbContext = dbContext;
        }

        public async Task NotifyAboutIncidentCreated(long incidentId)
        {
            try
            {
                var district = this.incidentService.GetDistricForIncident(incidentId);
                if (district == null)
                {
                    this.logger.LogError($"No distric found for IncidentId: {incidentId}");
                }
                else
                {
                    var message = this.GeIncidentCreatedMessage(incidentId);
                    await this.SendChannelMessageAsync(message, district.ConversationId);
                    await this.incidentService.UpdateIncidentConversationId(incidentId, message.Id);
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Can't sent NotifyAboutIncidentCreated notification.");
            }
        }

        private async Task SendChannelMessageAsync(Activity message, string conversationId)
        {
            var conversation = this.CreateConversationReference(conversationId);
            async Task Conversationcallback(ITurnContext turnContext, CancellationToken cancellationToken)
            {
                await turnContext.SendActivityAsync(message, cancellationToken);
            }

            await ((BotAdapter)this.adapter).ContinueConversationAsync(this.appSettings.BotAppId, conversation, Conversationcallback, default);
        }

        private async Task SendPrivateMessageAsync(IActivity message, string conversationReference)
        {
            var conversation = this.CreateConversationReference(conversationReference);
            async Task Conversationcallback(ITurnContext turnContext, CancellationToken cancellationToken)
            {
                await turnContext.SendActivityAsync(message, cancellationToken);
            }

            await ((BotAdapter)this.adapter).ContinueConversationAsync(this.appSettings.BotAppId, conversation, Conversationcallback, default);
        }

        private Activity GeIncidentCreatedMessage(long incidentId)
        {
            var incident = this.incidentService.GetIncident(incidentId);
            if (incident != null)
            {
                var text = Strings.IncidentCreatedTemplate
                    .Replace("{title}", incident.Title)
                    .Replace("{location}", incident.Location)
                    .Replace("{description}", incident.Description);
                var message = MessageFactory.Text(text);
                message.TextFormat = "markdown";
                return message;
            }

            return null;
        }

        private ConversationReference CreateConversationReference(string conversationId)
        {
            var serviceUrl = this.dbContext.Config.FirstOrDefault(x => x.Key == Core.Common.Constants.BotServiceUrlConfigKey);
            if (serviceUrl == null)
            {
                throw new InvalidOperationException($"Can't find {Core.Common.Constants.BotServiceUrlConfigKey} config value in DB");
            }

            ConversationReference conversation = new ConversationReference()
            {
                ChannelId = "msteams",
                Conversation = new ConversationAccount()
                {
                    Id = conversationId,
                    TenantId = this.appSettings.TenantId,
                },
                ServiceUrl = serviceUrl.Value,
            };
            return conversation;
        }
    }
}
