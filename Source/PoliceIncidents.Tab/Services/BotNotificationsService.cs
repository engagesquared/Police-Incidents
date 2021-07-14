namespace PoliceIncidents.Tab.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    public class BotNotificationsService
    {
        private readonly HttpClient httpClient;
        private readonly AppSettings appSettings;
        private readonly ILogger<BotNotificationsService> logger;

        public BotNotificationsService(HttpClient httpClient, AppSettings appSettings, ILogger<BotNotificationsService> logger)
        {
            this.httpClient = httpClient;
            this.appSettings = appSettings;
            this.logger = logger;
        }

        public async Task SendNewIncidentChannelNotification(long incidentId)
        {
            try
            {
                var botNotifyIncidentCreatedPath = Core.Common.Constants.IncidentCreatedBotRoute.Replace(Core.Common.Constants.IncidentIdToken, incidentId.ToString());
                await this.httpClient.PostAsync(this.appSettings.BotBaseUrl + botNotifyIncidentCreatedPath, null);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"SendNewIncidentChannelNotification {incidentId}");
            }
        }

        public async Task SendIncidentRolesPrivateNotification(long incidentId, List<Guid> userToNotify)
        {
            try
            {
                if (userToNotify == null)
                {
                    userToNotify = new List<Guid>();
                }

                var content = new StringContent(JsonConvert.SerializeObject(userToNotify), Encoding.UTF8, "application/json");
                var botNotifyRolesPath = Core.Common.Constants.IncidentRolesBotRoute.Replace(Core.Common.Constants.IncidentIdToken, incidentId.ToString());
                await this.httpClient.PostAsync(this.appSettings.BotBaseUrl + botNotifyRolesPath, content);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"SendIncidentRolesPrivateNotification {incidentId}, users: {string.Join(", ", userToNotify)}");
            }
        }
    }
}
