// <copyright file="ProactiveBot.cs" company="Engage Squared">
// Copyright (c) Engage Squared. All rights reserved.
// </copyright>

namespace PoliceIncidents.Bot.Bots
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Integration.AspNet.Core;
    using Microsoft.Bot.Schema;
    using Microsoft.Extensions.Logging;
    using PoliceIncidents.Bot.Interfaces;
    using PoliceIncidents.Bot.Resources;
    using PoliceIncidents.Bot.Services;
    using PoliceIncidents.Core.DB;

    public class ProactiveBot
    {
        private readonly AppSettings appSettings;
        private readonly ILogger<ProactiveBot> logger;
        private readonly IBotFrameworkHttpAdapter adapter;
        private readonly IIncidentService incidentService;
        private readonly PoliceIncidentsDbContext dbContext;
        private readonly AdaptiveCardService adaptiveCardService;

        public ProactiveBot(
            AppSettings appSettings,
            ILogger<ProactiveBot> logger,
            IBotFrameworkHttpAdapter adapter,
            IIncidentService incidentService,
            PoliceIncidentsDbContext dbContext,
            AdaptiveCardService adaptiveCardService)
        {
            this.appSettings = appSettings;
            this.logger = logger;
            this.adapter = adapter;
            this.incidentService = incidentService;
            this.dbContext = dbContext;
            this.adaptiveCardService = adaptiveCardService;
        }

        public async Task NotifyAboutIncidentCreated(long incidentId)
        {
            try
            {
                this.logger.LogInformation($"NotifyAboutIncidentCreated: incident with {incidentId} id.");
                var incident = this.incidentService.GetIncident(incidentId);
                if (incident == null)
                {
                    this.logger.LogError($"NotifyAboutIncidentCreated: Can't find incident with {incidentId} id");
                    return;
                }

                if (string.IsNullOrEmpty(incident.District?.ConversationId))
                {
                    this.logger.LogError($"NotifyAboutIncidentCreated: District (Team) {incident.District?.TeamGroupId} is not resolved. Bot was not added to the target team correctly");
                }
                else
                {
                    var message = this.adaptiveCardService.GeIncidentCreatedMessage(incident);
                    await this.SendProactiveMessageAsync(message, incident.District.ConversationId);
                    await this.incidentService.UpdateIncidentChatMessageId(incidentId, message.Id);
                }

                await this.NotifyAboutIncidentRoles(incidentId, null);

            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Can't sent NotifyAboutIncidentCreated notification.");
                throw;
            }
        }

        public async Task NotifyAboutIncidentRoles(long incidentId, List<Guid> usersToNotify)
        {
            try
            {
                var notifyAll = usersToNotify == null || usersToNotify.Count == 0;
                this.logger.LogInformation($"NotifyAboutIncidentRoles: incident with {incidentId} id.");
                var incident = this.incidentService.GetIncident(incidentId);
                if (incident == null)
                {
                    this.logger.LogError($"NotifyAboutIncidentRoles: Can't find incident with {incidentId} id");
                    return;
                }

                if (string.IsNullOrEmpty(incident.District?.ConversationId))
                {
                    this.logger.LogError($"NotifyAboutIncidentCreated: District (Team) {incident.District?.TeamGroupId} is not resolved. Bot was not added to the target team correctly");
                }

                if (notifyAll || usersToNotify.Any(x => x == incident.Manager.AadUserId))
                {
                    if (string.IsNullOrEmpty(incident.Manager.ConversationId))
                    {
                        this.logger.LogWarning($"Can't send notification to user {incident.Manager.AadUserId}. Bot is not installed for them.");
                    }
                    else
                    {
                        var notification = this.adaptiveCardService.GeIncidentMemberNotificationMessage(incident, Strings.ManagerRoleName);
                        await this.SendProactiveMessageAsync(notification, incident.Manager.ConversationId);
                    }
                }

                var members = this.incidentService.GetIncidentTeamMembers(incidentId);
                foreach (var member in members)
                {
                    if (notifyAll || usersToNotify.Any(x => x == member.TeamMember.AadUserId))
                    {
                        if (string.IsNullOrEmpty(member.TeamMember.ConversationId))
                        {
                            this.logger.LogWarning($"Can't send notification to user {member.TeamMember.AadUserId}. Bot is not installed for them");
                        }
                        else
                        {
                            var notification = this.adaptiveCardService.GeIncidentMemberNotificationMessage(incident, member.UserRole.Title);
                            await this.SendProactiveMessageAsync(notification, member.TeamMember.ConversationId);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Can't sent NotifyAboutIncidentRoles notification.");
                throw;
            }
        }

        private async Task SendProactiveMessageAsync(IMessageActivity message, string conversationId)
        {
            var conversation = this.CreateConversationReference(conversationId);
            async Task Conversationcallback(ITurnContext turnContext, CancellationToken cancellationToken)
            {
                await turnContext.SendActivityAsync(message, cancellationToken);
            }

            await ((BotAdapter)this.adapter).ContinueConversationAsync(this.appSettings.BotAppId, conversation, Conversationcallback, default);
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
