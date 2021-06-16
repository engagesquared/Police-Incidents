// <copyright file="Recipient.cs" company="Engage Squared">
// Copyright (c) Engage Squared. All rights reserved.
// </copyright>

namespace PoliceIncidents.Tab.Models
{
    using Newtonsoft.Json;

    public class Recipient
    {
        [JsonProperty("@odata.type")]
        public string OdataType { get; set; }

        public string UserId { get; set; }
    }
}
