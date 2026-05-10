using SafeCity.IRCM.DTOs;
using SafeCity_IRCMDB.Data;
using SafeCity_IRCMDB.Enum;

namespace SafeCity.IRCM.Repositories
{
    public class IncidentRepository : IIncidentRepository
    {
        private readonly SafeCityDbContext _context;
        public IncidentRepository(SafeCityDbContext context)
        {
            _context = context;
        }
        public async Task<IncidentCreateResponse> IncidentCreate(IncidentCreateRequest request)
        {
            //check if the request is null or not
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            else
            {
                //saving incident to the database
                var incidentDetails = request.ToIncidentCreateRequest();
                await _context.Incidents.AddAsync(incidentDetails);
                await _context.SaveChangesAsync();
                return IncidentCreateResponseExtension.ToIncidentCreateResponse(incidentDetails);
            }
        }

        public async Task IncidentStatusUpdate(int IncidentID, int option)
        {
            var incidentDetails = await _context.Incidents.FindAsync(IncidentID);
            if (incidentDetails == null)
            {
                throw new ArgumentNullException(nameof(incidentDetails));
            }
            else
            {
                if (option == 1)
                {
                    incidentDetails.Status = IncidentStatusOption.InProgress;
                    _context.Incidents.Update(incidentDetails);
                    await _context.SaveChangesAsync();
                }
                if (option == 2)
                {
                    incidentDetails.Status = IncidentStatusOption.Resolved;
                    _context.Incidents.Update(incidentDetails);
                    await _context.SaveChangesAsync();
                }
            }
        }
    }
}
