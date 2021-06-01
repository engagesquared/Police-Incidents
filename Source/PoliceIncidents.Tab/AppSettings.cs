// <copyright file="AppSettings.cs" company="Engage Squared">
// Copyright (c) Engage Squared. All rights reserved.
// </copyright>

namespace PoliceIncidents.Tab
{
    using Microsoft.Extensions.Configuration;

    public class AppSettings
    {
        private readonly IConfiguration configuration;

        public AppSettings(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.Setup();
        }

        public string BotBaseUrl { get; private set; }

        private void Setup()
        {
            this.BotBaseUrl = this.configuration["BotBaseUrl"];
        }
    }
}
