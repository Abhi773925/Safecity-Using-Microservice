using Microsoft.EntityFrameworkCore;
using SafeCity.DCR.DTOs;
using SafeCity_DCRDB.Data;
using SafeCity_DCRDB.Entities;
using SafeCity_DCRDB.Enums;

namespace SafeCity.DCR.Repositories
{
    public class CrisisRepository : ICrisisRepository
    {
        private readonly SafeCityDbContext _safeCityDbContext;
        public CrisisRepository(SafeCityDbContext safeCityDbContext)
        {
            _safeCityDbContext = safeCityDbContext;
        }


        public async Task<Crisis?> GetCrisisById(int id)
        {
            // Hum Include isliye use kar rahe hain taaki agar us Crisis par 
            // koi Teams Responses kaam kar rahi hon, toh wo bhi dikh jayein.
            return await _safeCityDbContext.Crises
                .Include(c => c.Responses)
                .FirstOrDefaultAsync(c => c.CrisisID == id);
        }

        //  CREATE With Duplicate Check 
        public async Task<CrisisResponse> CreateCrisis(CrisisRequest crisisRequest)
        {
            var exists = await _safeCityDbContext.Crises.AnyAsync(c =>
                c.Location.ToLower() == crisisRequest.Location.ToLower() &&
                c.Type == crisisRequest.Type &&
                c.Status != CrisisStatus.Resolved);

            if (exists) throw new Exception("Alert: This crisis is already reported and active at this location!");

            var crisisDetails = crisisRequest.ToCrisisRequest();

            crisisDetails.Date = DateTime.UtcNow;
            crisisDetails.Status = CrisisStatus.Pending;

            await _safeCityDbContext.Crises.AddAsync(crisisDetails);
            await _safeCityDbContext.SaveChangesAsync();

            return CrisisResponseExtension.ToCrisisResponse(crisisDetails);
        }

        //  READ ALL Crisis
        public async Task<IEnumerable<CrisisResponse>> GetAllCrises()
        {
            var crises = await _safeCityDbContext.Crises.ToListAsync();
            return crises.Select(c => CrisisResponseExtension.ToCrisisResponse(c));
        }

        //  READ ACTIVE War Room View
        public async Task<IEnumerable<CrisisResponse>> GetActiveCrises()
        {
            var active = await _safeCityDbContext.Crises
                .Where(c => c.Status == CrisisStatus.Pending || c.Status == CrisisStatus.Active)
                .OrderByDescending(c => c.Date)
                .ToListAsync();
            return active.Select(c => CrisisResponseExtension.ToCrisisResponse(c));
        }

        //  UPDATE Live Escalation the Crisis Severity
        public async Task<bool> UpdateCrisis(int id, CrisisStatus? status, CrisisSeverity? severity)
        {
            var crisis = await _safeCityDbContext.Crises.FindAsync(id);
            if (crisis == null) return false;

            if (status.HasValue) crisis.Status = status.Value;
            if (severity.HasValue) crisis.Severity = severity.Value;

            await _safeCityDbContext.SaveChangesAsync();
            return true;
        }
    }
}