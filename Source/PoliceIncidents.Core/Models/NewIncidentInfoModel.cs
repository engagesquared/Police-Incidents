// <copyright file="NewIncidentInfoModel.cs" company="Engage Squared">
// Copyright (c) Engage Squared. All rights reserved.
// </copyright>

using PoliceIncidents.Core.DB.Entities;
using System;

namespace PoliceIncidents.Core.Models
{
    public class NewIncidentInfoModel
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string Location { get; set; }

        public string PlannerLink { get; set; }

        public string IncidentID { get; set; }

        public string ChannelID { get; set; }

        public string GroupID { get; set; }

        public string ExternalId { get; set; }

        public IncidentStatus Status { get; set; }

        public DateTime CreatedUtc { get; set; }

        public string ExternalLink { get; set; }

        public string ChatConverstaionId { get; set; }
    }
}
