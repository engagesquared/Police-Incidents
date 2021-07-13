// <copyright file="PoliceIncidentsBot.cs" company="Engage Squared">
// Copyright (c) Engage Squared. All rights reserved.
// </copyright>

namespace PoliceIncidents.Bot.Bots
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Teams;
    using Microsoft.Bot.Schema;
    using Microsoft.Extensions.Logging;
    using PoliceIncidents.Core.DB;
    using PoliceIncidents.Core.Interfaces;

    public class PoliceIncidentsBot : TeamsActivityHandler
    {
        private readonly ILogger<PoliceIncidentsBot> logger;
        private readonly IUserService userService;
        private readonly IIncidentService incidentService;
        private readonly PoliceIncidentsDbContext dbContext;

        public PoliceIncidentsBot(
             ILogger<PoliceIncidentsBot> logger,
             IUserService userService,
             IIncidentService incidentService,
             PoliceIncidentsDbContext dbContext)
        {
            this.logger = logger;
            this.userService = userService;
            this.incidentService = incidentService;
            this.dbContext = dbContext;
        }

        protected override async Task OnConversationUpdateActivityAsync(ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var conversationReference = turnContext.Activity.GetConversationReference();
            string botId = turnContext.Activity.Recipient.Id;

            // Bot added to the team channel
            if (turnContext.Activity.MembersAdded?.Any(v => v.Id == botId) == true && turnContext.Activity.Conversation?.ConversationType == "channel")
            {
                string conversationId = conversationReference.Conversation.Id;
                var team = turnContext.Activity.TeamsGetTeamInfo();
                string channelId = turnContext.Activity.ChannelId;
                await this.incidentService.CreateDistrict(new Guid(team.AadGroupId), team.Name, conversationId);
            }

            await this.EnsureConversaion(conversationReference);

            var serviceUrl = conversationReference.ServiceUrl.ToLowerInvariant().Trim();
            await this.EnsureServiceUrl(serviceUrl);
            await base.OnConversationUpdateActivityAsync(turnContext, cancellationToken);
        }

        private async Task EnsureConversaion(ConversationReference conversation)
        {
            await this.userService.EnsureUserAsync(conversation.User.AadObjectId, conversation.Conversation.Id, conversation.User.Id);
        }

        private async Task EnsureServiceUrl(string serviceUrl)
        {
            try
            {
                var existingService = this.dbContext.Config.Where(x => x.Key == Core.Common.Constants.BotServiceUrlConfigKey).FirstOrDefault();
                if (existingService == null)
                {
                    existingService = new Core.DB.Entities.ConfigEntity { Key = Core.Common.Constants.BotServiceUrlConfigKey, Value = serviceUrl };
                    this.dbContext.Add(existingService);
                    await this.dbContext.SaveChangesAsync();
                }
                else if (existingService.Value != serviceUrl)
                {
                    existingService.Value = serviceUrl;
                    await this.dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Failed to ensure Service URL");
                throw;
            }
        }
    }
}
