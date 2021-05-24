// <copyright file="IncidentStatus.cs" company="Engage Squared">
// Copyright (c) Engage Squared. All rights reserved.
// </copyright>

namespace PoliceIncidents.Core.DB.Entities
{
    public enum IncidentStatus : byte
    {
        /// <summary>
        /// New
        /// </summary>
        New = 1,

        /// <summary>
        /// Active
        /// </summary>
        Active = 2,

        /// <summary>
        /// Closed
        /// </summary>
        Closed = 3,
    }
}
