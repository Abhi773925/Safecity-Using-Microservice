using Microsoft.EntityFrameworkCore;
using SafeCity.IRCM.DTOs;
using SafeCity_IRCMDB.Data;

namespace SafeCity.IRCM.Repositories
{
    public class IncidentRetrivalRepository : IIncidentRetrivalRepository
    {

        private readonly SafeCityDbContext _context;
        public IncidentRetrivalRepository(SafeCityDbContext context)
        {
            _context = context;
        }
        // fetching all the pending incident created by the citizen only Police Officer and Dispatcher is able to see all the records
        public async Task<List<IncidentCreateResponse>> IncidentRetrival()
        {
            // Listing all the incident with the pending state
            var incidents = await _context.Incidents
        .Where(temp => temp.Status == 0) // Ya Status.Pending agar Enum hai
        .ToListAsync();

            return incidents.Select(i => IncidentCreateResponseExtension.ToIncidentCreateResponse(i)).ToList();

        }

        public async Task<List<IncidentCreateResponse>> IncidentRetrivalAll()
        {
            var incidents = await _context.Incidents.ToListAsync();

            return incidents.Select(i => IncidentCreateResponseExtension.ToIncidentCreateResponse(i)).ToList();
        }

        public async Task<List<IncidentCreateResponse>> IncidentRetrivalByCitizenId(int citizenId)
        {
            var incidents = await _context.Incidents
                .Where(temp => temp.CitizenID == citizenId)
                .ToListAsync();

            return incidents.Select(i => IncidentCreateResponseExtension.ToIncidentCreateResponse(i)).ToList();
        }
    }
}
