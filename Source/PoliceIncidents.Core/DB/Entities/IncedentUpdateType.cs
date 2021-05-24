// <copyright file="IncedentUpdateType.cs" company="Engage Squared">
// Copyright (c) Engage Squared. All rights reserved.
// </copyright>

namespace PoliceIncidents.Core.DB.Entities
{
    public enum IncedentUpdateType : byte
    {
        /// <summary>
        /// Manual
        /// </summary>
        Manual = 1,

        /// <summary>
        /// WebEOC
        /// </summary>
        WebEOC = 2,

        /// <summary>
        /// Critical
        /// </summary>
        Critical = 3,
    }
}
