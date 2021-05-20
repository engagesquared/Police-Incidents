using PoliceIncidents.Core.DB;
using PoliceIncidents.Core.Models;

namespace PoliceIncidents.Core.Interfaces
{
    public interface IIncidentService
    {
        void CreateIncident(IncidentInputModel model, PoliceIncidentsDbContext dbContext);
    }
}