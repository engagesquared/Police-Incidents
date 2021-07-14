namespace PoliceIncidents.Bot.Services
{
    using System;
    using System.IO;
    using System.Text.RegularExpressions;
    using AdaptiveCards.Templating;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Schema;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using PoliceIncidents.Bot.Models;
    using PoliceIncidents.Bot.Resources;
    using PoliceIncidents.Core.DB.Entities;
    using PoliceIncidents.Core.Services;

    public class AdaptiveCardService
    {
        private readonly string userNotificationCardPath = "Resources/AdaptiveCards/IncidentAssignmentUserNotification.json";

        private readonly AppSettings appSettings;
        private readonly DeepLinksService deepLinksService;

        public AdaptiveCardService(AppSettings appSettings, DeepLinksService deepLinksService)
        {
            this.appSettings = appSettings;
            this.deepLinksService = deepLinksService;
        }

        public IMessageActivity GeIncidentMemberNotificationMessage(IncidentDetailsEntity incident, string roleName)
        {
            var district = incident.District;

            var chatThreadUrl = this.deepLinksService.GetChatMessageLink(district.TeamGroupId, district.ConversationId, incident.ChatConverstaionId);
            var incidentLogUrl = this.deepLinksService.GetTeamIncidentLink(district.TeamGroupId, this.appSettings.TabAppId, incident.Title, district.ConversationId, incident.Id);
            var data = new IncidentMemberNotificationCardData
            {
                IncidentName = incident.Title,
                IncidentLocation = incident.Location,
                IncidentLogUrl = incidentLogUrl,
                ChatThreadUrl = chatThreadUrl,
                RoleType = roleName,
            };
            var adaptiveCardText = File.ReadAllText(this.userNotificationCardPath);
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

        public IMessageActivity GeIncidentCreatedMessage(IncidentDetailsEntity incident)
        {
            var incidentDeepLink = this.deepLinksService.GetTeamIncidentLink(incident.District.TeamGroupId, this.appSettings.TabAppId, incident.Title, incident.District.ConversationId, incident.Id);
            var location = Escape(incident.Location);
            var locationUrlescaped = Uri.EscapeDataString(incident.Location);
            var text = Strings.IncidentCreatedTemplate
                .Replace("{title}", incident.Title)
                .Replace("{locationEscaped}", location)
                .Replace("{locationUrlEscaped}", locationUrlescaped)
                .Replace("{description}", incident.Description)
                .Replace("{plannerLink}", incident.PlannerLink)
                .Replace("{incidentLink}", incidentDeepLink);
            var message = MessageFactory.Text(text);
            message.TextFormat = "markdown";
            return message;
        }

        private string Escape(string link)
        {
            var regex = new Regex("(?<!\\\\)([\\(\\)\\[\\]\\*\\{\\}\\!\\+\\-])");
            return regex.Replace(link ?? string.Empty, "\\$1");
        }
    }
}
