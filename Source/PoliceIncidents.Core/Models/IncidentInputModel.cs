namespace PoliceIncidents.Core.Models
{
    public class IncidentInputModel
    {
        /// <summary>
        /// WebEOC  ID of the incident 
        /// </summary>
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string WebEOCLink { get; set; }
        public string Status { get; set; }
        public string Location { get; set; }
    }
}
