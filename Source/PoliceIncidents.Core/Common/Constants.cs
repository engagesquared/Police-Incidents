// <copyright file="Constants.cs" company="Engage Squared">
// Copyright (c) Engage Squared. All rights reserved.
// </copyright>

namespace PoliceIncidents.Core.Common
{
    public static class Constants
    {
        public const string BotServiceUrlConfigKey = "BotServiceUrl";
        public const string IncidentCreatedBotRoute = "/api/notify/incidentCreated/{incidentId}";
        public const string IncidentRolesBotRoute = "/api/notify/incidentRoles/{incidentId}";

        public const string IncidentIdToken = "{incidentId}";
    }
}
