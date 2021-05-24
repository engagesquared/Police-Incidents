// <copyright file="AppSettings.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PoliceIncidents.Bot
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

        public string BotAppId { get; private set; }

        public string TenantId { get; private set; }

        private void Setup()
        {
            this.BotAppId = this.configuration["MicrosoftAppId"];
            this.TenantId = this.configuration["TenantId"];
        }
    }
}
