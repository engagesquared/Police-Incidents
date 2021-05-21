// <copyright file="ConfigEntity.cs" company="Engage Squared">
// Copyright (c) Engage Squared. All rights reserved.
// </copyright>

namespace PoliceIncidents.Core.DB.Entities
{
    public class ConfigEntity
    {
        public long Id { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }
    }
}
