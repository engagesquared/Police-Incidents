namespace PoliceIncidents.Tab.Models
{
    using System;

    public class GraphTokenModel
    {
        public string Token { get; set; }

        public DateTimeOffset ExpiresOn { get; set; }
    }
}
