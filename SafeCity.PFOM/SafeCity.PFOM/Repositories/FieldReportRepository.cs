using Microsoft.EntityFrameworkCore;
using SafeCity.PFOM.DTOs;
using SafeCity_PFOMDB.Data;
using SafeCity_PFOMDB.Enums;
namespace SafeCity.PFOM.Repositories
{
    public class FieldReportRepository : IFieldReportRepository
    {
        private readonly SafeCityDbContext _context;
        public FieldReportRepository(SafeCityDbContext context)
        {
            _context = context;
        }

        public async Task<List<FieldReportResponse>> GetAllFeildReport()
        {
            var response = await _context.FieldReports.ToListAsync();
            if (response == null)
            {
                throw new Exception("No Field Report was found");
            }
            return response.Select(x => FieldReportResponseExtension.ToFieldReportResponse(x)).ToList();
        }

        async Task<FieldReportResponse> IFieldReportRepository.FieldReport(FieldReportRequest request, int OfficerId)
        {
            int patrolId = request.PatrolId;
            var patrolDetails = await _context.Patrols.FindAsync(patrolId);
            if (patrolDetails == null) throw new Exception("Patrol record nahi mila!");
            if (patrolDetails.OfficerId != OfficerId)
            {
                throw new Exception("Ye aapka Patrol Details nahi hai");
            }
            else
            {
                if (patrolDetails.Status != PatrolStatus.OnPatrol)
                {
                    throw new Exception("Pahle Patrol Start Karo");
                }
                var details = request.ToFieldReportRequest();
                details.Status = FieldReportStatus.Submitted;
                await _context.FieldReports.AddAsync(details);
                await _context.SaveChangesAsync();
                return FieldReportResponseExtension.ToFieldReportResponse(details);

            }
        }


        public async Task<bool> ReviewFieldReportAsync(int reportId, FieldReportStatus newStatus)
        {
            var report = await _context.FieldReports.FindAsync(reportId);

            if (report == null)
            {
                return false; // Report nahi mili
            }

            // Status update karo Submitted -> Approved/Rejected
            report.Status = newStatus;

            _context.FieldReports.Update(report);
            await _context.SaveChangesAsync();

            return true;
        }



        public async Task<IEnumerable<FieldReportResponse>> GetMyReportHistoryAsync(int officerId)
        {
            var reports = await _context.FieldReports
                .Include(fr => fr.Patrol) // Patrol table se link karne ke liye
                .Where(fr => fr.Patrol.OfficerId == officerId) // Sirf us officer ki reports
                .OrderByDescending(fr => fr.Date) // Latest reports pehle dikhao
                .ToListAsync();

            // DTO mein map karke bhej do
            return reports.Select(r => FieldReportResponseExtension.ToFieldReportResponse(r));
        }
    }
}
