using Microsoft.EntityFrameworkCore;
using SafeCity.IRCM.DTOs;
using SafeCity.IRCM.Repositories;
using SafeCity_IRCMDB.Data;

namespace SafeCity.IRCM.Services
{
    public class CaseClosingService : ICaseClosingService
    {
        // depedency injection
        private readonly ICaseClosingRepository _repository;
        private readonly IIncidentService _incidentService;
        private readonly SafeCityDbContext _dbContext;

        public CaseClosingService(ICaseClosingRepository repository, IIncidentService incidentService, SafeCityDbContext dbContext)
        {
            _repository = repository;
            _incidentService = incidentService;
            _dbContext = dbContext;
        }

        public async Task CaseClosing(CaseClosingRequest request, int CaseID)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            if (CaseID <= 0)
            {
                throw new Exception("Invalid Case Id");
            }
            else
            {

                await _repository.CaseClosing(request, CaseID);

                // incident id find kro
                var caseDetails = await _dbContext.Cases.FirstOrDefaultAsync(temp => temp.CaseID == CaseID);
                await _incidentService.IncidentStatusUpdate(caseDetails.IncidentID, 2);
            }

        }
    }
}
