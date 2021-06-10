// <copyright file="IncidentTeamMemberInput.cs" company="Engage Squared">
// Copyright (c) Engage Squared. All rights reserved.
// </copyright>

namespace PoliceIncidents.Tab.Models
{
    using System;

    public class IncidentTeamMemberInput
    {
        public Guid? IncidentManager { get; set; }

        public Guid? SocLead { get; set; }

        public Guid? FieldOfficer { get; set; }

        public Guid? FamilyLiason { get; set; }

        public Guid? ExternalAgency { get; set; }
    }
}
