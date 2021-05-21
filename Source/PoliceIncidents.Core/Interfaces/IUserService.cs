// <copyright file="IUserService.cs" company="Engage Squared">
// Copyright (c) Engage Squared. All rights reserved.
// </copyright>

namespace PoliceIncidents.Core.Interfaces
{
    using System.Threading.Tasks;
    using PoliceIncidents.Core.DB.Entities;

    public interface IUserService
    {
        Task<UserEntity> EnsureUserAsync(string aadObjectId, string conversationId, string botUserId);
    }
}