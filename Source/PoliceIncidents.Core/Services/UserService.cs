// <copyright file="UserService.cs" company="Engage Squared">
// Copyright (c) Engage Squared. All rights reserved.
// </copyright>

namespace PoliceIncidents.Core.Services
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using PoliceIncidents.Core.DB;
    using PoliceIncidents.Core.DB.Entities;
    using PoliceIncidents.Core.Interfaces;

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
                else
                {
                    user.BotUserId = botUserId;
                    user.ConversationId = conversationId;
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
