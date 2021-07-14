// <copyright file="UserService.cs" company="Engage Squared">
// Copyright (c) Engage Squared. All rights reserved.
// </copyright>

namespace PoliceIncidents.Bot.Services
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using PoliceIncidents.Bot.Interfaces;
    using PoliceIncidents.Core.DB;
    using PoliceIncidents.Core.DB.Entities;

    public class UserService : IUserService
    {
        private readonly ILogger<UserService> logger;
        private readonly PoliceIncidentsDbContext dbContext;

        public UserService(
            ILogger<UserService> logger,
            PoliceIncidentsDbContext dbContext)
        {
            this.logger = logger;
            this.dbContext = dbContext;
        }

        public async Task<UserEntity> EnsureUserAsync(string aadObjectId, string conversationId, string botUserId)
        {
            try
            {
                var userId = new Guid(aadObjectId);
                var userQuery = this.dbContext.UserEntities.Where(x => x.AadUserId == userId);
                var user = userQuery.FirstOrDefault();
                if (user == null)
                {
                    user = new UserEntity
                    {
                        AadUserId = userId,
                        BotUserId = botUserId,
                        ConversationId = conversationId,
                    };

                    this.dbContext.Add(user);
                    await this.dbContext.SaveChangesAsync();
                }
                else if (!string.IsNullOrEmpty(conversationId) && !string.IsNullOrEmpty(botUserId))
                {
                    user.BotUserId = botUserId;
                    user.ConversationId = conversationId;
                    await this.dbContext.SaveChangesAsync();
                }

                return user;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Failed to ensure user");
                throw;
            }
        }
    }
}
