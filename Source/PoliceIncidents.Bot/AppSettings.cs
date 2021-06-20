// <copyright file="AppSettings.cs" company="Engage Squared">
// Copyright (c) Engage Squared. All rights reserved.
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

        public string TabAppId { get; private set; }

        public string TabBaseUrl { get; private set; }

        private void Setup()
        {
            this.BotAppId = this.configuration["MicrosoftAppId"];
            this.TenantId = this.configuration["TenantId"];
            this.TabAppId = this.configuration["TabAppId"];
            this.TabBaseUrl = this.configuration["TabBaseUrl"];
        }
    }
}
