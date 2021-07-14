// <copyright file="IncidentService.cs" company="Engage Squared">
// Copyright (c) Engage Squared. All rights reserved.
// </copyright>

namespace PoliceIncidents.Bot.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using PoliceIncidents.Bot.Interfaces;
    using PoliceIncidents.Core.DB;
    using PoliceIncidents.Core.DB.Entities;

    public class IncidentService : IIncidentService
    {
        private readonly PoliceIncidentsDbContext dbContext;
        private readonly ILogger<IncidentService> logger;

        public IncidentService(
            PoliceIncidentsDbContext dbContext,
            ILogger<IncidentService> logger)
        {
            this.dbContext = dbContext;
            this.logger = logger;
        }

        public async Task EnsureDistrict(Guid groupId, string teamName, string conversationId)
        {
            try
            {
                var district = this.dbContext.Districts.Where(v => v.TeamGroupId == groupId).FirstOrDefault();
                if (district == null)
                {
                    var newDistrict = new DistrictEntity()
                    {
                        RegionName = teamName,
                        TeamGroupId = groupId,
                        ConversationId = conversationId,
                    };
                    this.dbContext.Districts.Add(newDistrict);
                    await this.dbContext.SaveChangesAsync();
                }

                if (district.ConversationId != conversationId)
                {
                    district.ConversationId = conversationId;
                    await this.dbContext.SaveChangesAsync();
                }

                if (string.IsNullOrWhiteSpace(district.RegionName) && !string.IsNullOrWhiteSpace(teamName))
                {
                    district.RegionName = teamName;
                    await this.dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Failed to ensure district for team {groupId} - {teamName}");
                throw;
            }
        }

        public async Task UpdateIncidentChatMessageId(long incidentId, string messageId)
        {
            try
            {
                var incident = this.dbContext.IncidentDetails.Where(v => v.Id == incidentId).FirstOrDefault();
                if (incident == null)
                {
                    this.logger.LogError($"No incident was found with '{incidentId}' id to update ChatConverstaionId.");
                    return;
                }

                incident.ChatConverstaionId = messageId;
                await this.dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Failed to update ChatConverstaionId for incident {incidentId}");
                throw;
            }
        }

        public IncidentDetailsEntity GetIncident(long incidentId)
        {
            try
            {
                var incident = this.dbContext.IncidentDetails.Where(v => v.Id == incidentId)
                    .Include(x => x.District)
                    .Include(x => x.Manager)
                    .FirstOrDefault();

                return incident;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Failed to get incident {incidentId} from database");
                throw;
            }
        }

        public List<IncidentTeamMemberEntity> GetIncidentTeamMembers(long incidentId)
        {
            try
            {
                var members = this.dbContext.IncidentTeamMembers.Where(v => v.IncidentId == incidentId).Include(x => x.TeamMember).Include(x => x.UserRole).ToList();
                return members;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Failed to get team members for incident {incidentId} from database");
                throw;
            }
        }
    }
}
