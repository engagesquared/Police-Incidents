// <copyright file="ProactiveBot.cs" company="Engage Squared">
// Copyright (c) Engage Squared. All rights reserved.
// </copyright>

namespace PoliceIncidents.Bot.Bots
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using AdaptiveCards.Templating;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Integration.AspNet.Core;
    using Microsoft.Bot.Schema;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using PoliceIncidents.Bot.Resources;
    using PoliceIncidents.Bot.Models;
    using PoliceIncidents.Core.DB;
    using PoliceIncidents.Core.Interfaces;
    using System.Web;

    public class ProactiveBot
    {
        private readonly AppSettings appSettings;
        private readonly ILogger<ProactiveBot> logger;
        private readonly IBotFrameworkHttpAdapter adapter;
        private readonly IIncidentService incidentService;
        private readonly PoliceIncidentsDbContext dbContext;
        private readonly string UserNotificationCardPath = "Resources/AdaptiveCards/IncidentAssignmentUserNotification.json";

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
                var members = this.incidentService.GetIncidentTeamMembers(incidentId);
                if (district == null)
                {
                    this.logger.LogError($"No distric found for IncidentId: {incidentId}");
                }
                else if (district.ConversationId != null)
                {
                    var message = this.GeIncidentCreatedMessage(incidentId);
                    await this.SendChannelMessageAsync(message, district.ConversationId);
                    await this.incidentService.UpdateIncidentConversationId(incidentId, message.Id);
                }

                foreach (var member in members)
                {
                    if (member.ConversationId != null)
                    {
                        var roleEntity = this.dbContext.IncidentTeamMembers.Where(v => v.IncidentId == incidentId && v.TeamMemberId == member.AadUserId).Select(x => x.UserRole).FirstOrDefault();
                        var notification = this.GeIncidentMemberNotificationMessage(incidentId, roleEntity?.Title ?? "Manager");
                        await this.SendChannelMessageAsync(notification, member.ConversationId);
                    }
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Can't sent NotifyAboutIncidentCreated notification.");
            }
        }

        private async Task SendChannelMessageAsync(IMessageActivity message, string conversationId)
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

        private IMessageActivity GeIncidentCreatedMessage(long incidentId)
        {
            var incident = this.incidentService.GetIncident(incidentId);
            if (incident != null)
            {
                var text = Strings.IncidentCreatedTemplate
                    .Replace("{title}", incident.Title)
                    .Replace("{location}", incident.Location)
                    .Replace("{description}", incident.Description)
                    .Replace("{plannerLink}", incident.PlannerLink)
                    .Replace("{incidentID}", incident.IncidentID)
                    .Replace("{channelID}", incident.ChannelID)
                    .Replace("{groupID}", incident.GroupID);
                var message = MessageFactory.Text(text);
                message.TextFormat = "markdown";
                return message;
            }

            return null;
        }

        private IMessageActivity GeIncidentMemberNotificationMessage(long incidentId, string role)
        {
            var incident = this.incidentService.GetIncident(incidentId);
            var district = this.incidentService.GetDistricForIncident(incidentId);

            var chatThreadUrl = $"https://teams.microsoft.com/l/message/{district.ConversationId}/{incident.ChatConverstaionId}?tenantId={this.appSettings.TenantId}&groupId={district.TeamGroupId}&parentMessageId={incident.ChatConverstaionId}";
            var incidentLogUrl = $"https://teams.microsoft.com/l/entity/{this.appSettings.TabAppId}/Home?webUrl={HttpUtility.UrlEncode($"{this.appSettings.TabBaseUrl}/incident/{incidentId}")}&label={HttpUtility.UrlEncode(incident.Title)}&context={HttpUtility.UrlEncode($"{{ \"subEntityId\": \"incident/{incidentId}\", \"channelId\": \"{district.ConversationId}\" }}")}";
            var data = new IncidentMemberNotificationCardData
            {
                IncidentName = incident.Title,
                IncidentLocation = incident.Location,
                IncidentLogUrl = incidentLogUrl,
                ChatThreadUrl = chatThreadUrl,
                RoleType = role,
            };
            var adaptiveCardText = File.ReadAllText(this.UserNotificationCardPath);
            AdaptiveCardTemplate template = new AdaptiveCardTemplate(adaptiveCardText);
            string processedCardText = template.Expand(data);
            var cardJson = JsonConvert.DeserializeObject<JObject>(processedCardText);
            var message = MessageFactory.Attachment(new Attachment()
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = cardJson,
            });

            return message;
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
