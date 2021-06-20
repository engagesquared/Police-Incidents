// <copyright file="IncidentMemberNotificationCardData.cs" company="Engage Squared">
// Copyright (c) Engage Squared. All rights reserved.
// </copyright>

namespace PoliceIncidents.Bot.Models
{
    public class IncidentMemberNotificationCardData
    {
        public string IncidentName { get; set; }

        public string IncidentLocation { get; set; }

        public string IncidentLogUrl { get; set; }

        public string ChatThreadUrl { get; set; }

        public string RoleType { get; set; }
    }
}
