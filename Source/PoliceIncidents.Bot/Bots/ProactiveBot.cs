// <copyright file="ProactiveBot.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
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
                var conversationRefs = this.dbContext.UserEntities.Where(x => x.ConversationId != null || !x.ConversationId.Equals(string.Empty)).Select(v => v.ConversationId).ToList();
                foreach (var conversationRef in conversationRefs)
                {
                    var message = this.GeIncidentCreatedMessage(incidentId);
                    await this.SendMessageAsync(message, conversationRef);
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Can't sent NotifyAboutIncidentCreated notification.");
            }
        }

        private async Task SendMessageAsync(IActivity message, string conversationReference)
        {
            var conversation = this.CreateConversationReferencePersonal(conversationReference);
            async Task Conversationcallback(ITurnContext turnContext, CancellationToken cancellationToken)
            {
                await turnContext.SendActivityAsync(message, cancellationToken);
            }

            await ((BotAdapter)this.adapter).ContinueConversationAsync(this.appSettings.BotAppId, conversation, Conversationcallback, default);
        }

        private Activity GeIncidentCreatedMessage(long incidentId)
        {
            var incident = this.incidentService.Get(incidentId);
            if (incident != null)
            {
                var text = Strings.IncidentCreatedTemplate
                    .Replace("{title}", incident.Title)
                    .Replace("{description}", incident.Description)
                    .Replace("{link}", incident.Link.Replace(" ", "%20"));
                var message = MessageFactory.Text(text);
                message.TextFormat = "markdown";
                return message;
            }

            return null;
        }

        private ConversationReference CreateConversationReferencePersonal(string conversationId)
        {
            return this.CreateConversationReference(conversationId, "personal");
        }

        private ConversationReference CreateConversationReference(string conversationId, string conversationType)
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
                    ConversationType = conversationType,
                    TenantId = this.appSettings.TenantId,
                },
                ServiceUrl = serviceUrl.Value,
            };
            return conversation;
        }
    }
}
