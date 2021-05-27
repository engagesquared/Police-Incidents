// <copyright file="FakeIncidentUpdateService.cs" company="Engage Squared">
// Copyright (c) Engage Squared. All rights reserved.
// </copyright>

namespace PoliceIncidents.Tab.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using PoliceIncidents.Core.DB.Entities;
    using PoliceIncidents.Tab.Interfaces;
    using PoliceIncidents.Tab.Models;

    public class FakeIncidentUpdateService : IIncidentUpdateService
    {

        public FakeIncidentUpdateService()
        {
        }

        public async Task AddIncidentUpdate(IncidentUpdateInputModel incidentUpdate)
        {
            return;
        }

        public async Task<List<IncidentUpdateModel>> GetIncidentUpdates(long incidentId)
        {
            var updates = new List<IncidentUpdateModel>();
            for (int i = 1; i <= 8; i++)
            {
                var update = new IncidentUpdateModel()
                {
                    Body = "Sed ut perspiciatis unde omnis iste natus error sit voluptatem accusantium doloremque laudantium, totam rem aperiam, eaque ipsa quae ab illo inventore veritatis et quasi architecto beatae vitae dicta sunt explicabo.",
                    CreatedAt = DateTime.Now.AddHours(-3 * 1),
                    Id = i,
                    Title = $"Update {i}",
                    UpdateType = (IncedentUpdateType)((i % 3) + 1),
                };
                updates.Add(update);
            }

            return updates;
        }
    }
}
