// <copyright file="DistrictEntity.cs" company="Engage Squared">
// Copyright (c) Engage Squared. All rights reserved.
// </copyright>

namespace PoliceIncidents.Core.DB.Entities
{
    using System;

    public class DistrictEntity
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public Guid TeamGroupId { get; set; }
    }
}
