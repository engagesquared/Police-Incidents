// <copyright file="FakeIncidentService.cs" company="Engage Squared">
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

    public class FakeIncidentService : IIncidentService
    {
        public FakeIncidentService()
        {
        }

        public async Task ChangeIncidentManager(long incidentId, Guid managerId)
        {
            return;
        }

        public async Task<IncidentModel> GetIncidentById(long id)
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

            var incident = new IncidentModel()
            {
                Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
                IncidentRaised = DateTime.Now.AddHours(-3 * 1),
                Id = 1,
                Title = $"Police incident #{id}",
                IncidentUpdates = updates,
                Location = "Perth, Corner of Hay Street and Barrack Street",
                Status = IncidentStatus.Active,
                WebEOCLink = "https://google.com",
            };
            return incident;
        }

        public async Task<List<IncidentModel>> GetUserIncidents(Guid userId)
        {
            var result = new List<IncidentModel>();
            var updates = new List<IncidentUpdateModel>();
            for (int i = 1; i <= 8; i++)
            {
                var update = new IncidentUpdateModel()
                {
                    Body = "Sed ut perspiciatis unde omnis iste natus error sit voluptatem accusantium doloremque laudantium, totam rem aperiam, eaque ipsa quae ab illo inventore veritatis et quasi architecto beatae vitae dicta sunt explicabo.",
                    CreatedAt = DateTime.Now.AddHours(-3 * i),
                    CreatedById = userId,
                    Id = i,
                    Title = $"Update {i}",
                    UpdateType = (IncedentUpdateType)((i % 3) + 1),
                };
                updates.Add(update);
            }

            for (int i = 1; i <= 8; i++)
            {
                var incident = new IncidentModel()
                {
                    Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
                    IncidentRaised = DateTime.Now.AddHours(-3 * i),
                    IncidentManagerId = userId,
                    Id = i,
                    Title = $"Police incident #{i}",
                    IncidentUpdates = updates,
                    Location = "Perth, Corner of Hay Street and Barrack Street",
                    Status = IncidentStatus.Active,
                    WebEOCLink = "https://google.com",
                };
                result.Add(incident);
            }

            return result;
        }
    }
}
