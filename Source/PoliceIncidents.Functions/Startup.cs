// <copyright file="Startup.cs" company="Engage Squared">
// Copyright (c) Engage Squared. All rights reserved.
// </copyright>

using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PoliceIncidents.Core.DB;
using PoliceIncidents.Core.Interfaces;
using PoliceIncidents.Core.Services;

[assembly: FunctionsStartup(typeof(PoliceIncidents.Functions.Startup))]

namespace PoliceIncidents.Functions
{
    internal class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var services = builder.Services;
            services.AddLogging();
            services.AddScoped<IIncidentService, IncidentService>();
            services.AddDbContext<PoliceIncidentsDbContext>(p => p.UseSqlServer(WebConfig.DbConnectionString));
        }
    }
}
