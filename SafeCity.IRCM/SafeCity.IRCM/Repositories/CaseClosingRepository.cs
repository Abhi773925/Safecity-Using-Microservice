using SafeCity.IRCM.DTOs;
using SafeCity_IRCMDB.Data;

namespace SafeCity.IRCM.Repositories
{
    public class CaseClosingRepository : ICaseClosingRepository
    {
        // dependency injection
        private readonly SafeCityDbContext _context;
        public CaseClosingRepository(SafeCityDbContext context)
        {
            _context = context;
        }

        public async Task CaseClosing(CaseClosingRequest request, int CaseID)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            if (CaseID <= 0)
            {
                throw new Exception("Invalid Case Id.");
            }
            else
            {

                // closing the case details
                var caseDetails = await _context.Cases.FindAsync(CaseID);
                if (caseDetails == null)
                {
                    throw new Exception("No Such Case Found.");
                }
                caseDetails.Description = request.Description;
                caseDetails.ResolutionDate = request.ResolutionDate;
                caseDetails.Status = request.Status;

                _context.Cases.Update(caseDetails);
                await _context.SaveChangesAsync();

            }
        }
    }
}
