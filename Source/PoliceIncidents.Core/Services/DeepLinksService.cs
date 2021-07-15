// <copyright file="DeepLinksService.cs" company="Engage Squared">
// Copyright (c) Engage Squared. All rights reserved.
// </copyright>

namespace PoliceIncidents.Core.Services
{
    using System;

    public class DeepLinksService
    {
        private const string TeamsDeepLink = "https://teams.microsoft.com/l";

        public string GetChatMessageLink(Guid? teamId, string chatId, string messageId)
        {
            var result = $"{TeamsDeepLink}/message/{chatId}/{messageId}{(teamId.HasValue ? $"?groupId={teamId}" : string.Empty)}";
            return result;
        }

        public string GetTeamIncidentLink(Guid? teamId, string tabAppId, string label, string channelId, long incidentId)
        {
            var ctx = $"{{ \"subEntityId\": \"incident/{incidentId}\", \"channelId\": \"{channelId}\" }}";
            var result = $"{TeamsDeepLink}/entity/{tabAppId}/Home?label={Uri.EscapeDataString(label)}{(teamId.HasValue ? $"&groupId={teamId}" : string.Empty)}&context={Uri.EscapeDataString(ctx)}";
            return result;
        }

        public string GetNewMeetingLink(string incidentTitle, string[] upns)
        {
            return $"{TeamsDeepLink}/meeting/new?subject={Uri.EscapeDataString(incidentTitle)}&attendees={string.Join(',', upns)}";
        }
    }
}
