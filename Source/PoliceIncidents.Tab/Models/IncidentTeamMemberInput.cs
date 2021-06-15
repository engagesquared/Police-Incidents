// <copyright file="IncidentTeamMemberInput.cs" company="Engage Squared">
// Copyright (c) Engage Squared. All rights reserved.
// </copyright>

namespace PoliceIncidents.Tab.Models
{
    using System;
    using System.Collections.Generic;

    public class IncidentTeamMemberInput
    {
        public Guid? IncidentManager { get; set; }

        public List<Guid> SocLead { get; set; }

        public List<Guid> FieldOfficer { get; set; }

        public List<Guid> FamilyLiason { get; set; }

        public List<Guid> ExternalAgency { get; set; }
    }
}
