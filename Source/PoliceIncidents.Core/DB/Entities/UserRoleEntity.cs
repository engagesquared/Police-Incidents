// <copyright file="UserRoleEntity.cs" company="Engage Squared">
// Copyright (c) Engage Squared. All rights reserved.
// </copyright>

namespace PoliceIncidents.Core.DB.Entities
{
    public class UserRoleEntity
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public bool? IsDefault { get; set; }
    }
}
