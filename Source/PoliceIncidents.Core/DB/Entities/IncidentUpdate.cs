using System;

namespace PoliceIncidents.Core.DB.Entities
{
    public class IncidentUpdate
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public IncedentUpdateType UpdateType { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public IncidentDetails ParentIncident { get; set; }
    }
}
