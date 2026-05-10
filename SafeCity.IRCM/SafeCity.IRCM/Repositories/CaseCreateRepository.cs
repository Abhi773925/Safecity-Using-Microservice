using Microsoft.EntityFrameworkCore;
using SafeCity.IRCM.DTOs;
using SafeCity_IRCMDB.Data;
using SafeCity_IRCMDB.Entity;

namespace SafeCity.IRCM.Repositories
{
    public class CaseCreateRepository : ICaseCreateRepository
    {
        // dependency injection
        private readonly SafeCityDbContext _context;
        public CaseCreateRepository(SafeCityDbContext context)
        {
            _context = context;
        }

        // case creation Logic goes here..
        public async Task<CaseCreateResponse> CaseCreate(CaseCreateRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            else
            {
                // creating the case and saving to the case Tables.
                var caseDetails = request.ToCaseCreateRequest();
                var response = await _context.Cases.AddAsync(caseDetails);
                await _context.SaveChangesAsync();

                return CaseCreateResponseExtension.ToCaseCreateResponse(caseDetails);
            }
        }

        public async Task<List<Case>> GetAllCase()
        {
            // Data fetch karein
            var response = await _context.Cases.ToListAsync();

            // Check karein ki data mila ya nahi
            if (response == null || response.Count == 0)
            {

                throw new Exception("No Case Found in the database.");
            }
            return response;
        }

        public async Task<List<Case>> GetCaseByCitizenId(int citizenId)
        {
            if (citizenId <= 0)
            {
                throw new Exception("Invalid Citizen Id.");
            }

            var response = await _context.Cases
                .Where(temp => temp.Incident != null && temp.Incident.CitizenID == citizenId)
                .ToListAsync();

            if (response == null || response.Count == 0)
            {
                throw new Exception("No Case Found for this citizen.");
            }

            return response;
        }
    }
}
