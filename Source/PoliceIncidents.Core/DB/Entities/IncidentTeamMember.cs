namespace PoliceIncidents.Core.DB.Entities
{
    public class IncidentTeamMember
    {
        public long Id { get; set; }
        public UserEntity TeamMember { get; set; }
        public IncidentDetails Incident { get; set; }
    }
}
