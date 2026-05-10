using SafeCity.PFOM.DTOs;
using SafeCity.PFOM.Repositories;
using SafeCity_PFOMDB.Enums;

namespace SafeCity.PFOM.Services
{
    public class FieldReportService : IFieldReportService
    {
        private readonly IFieldReportRepository _fieldReportRepository;

        public FieldReportService(IFieldReportRepository fieldReportRepository)
        {
            _fieldReportRepository = fieldReportRepository;
        }

        public async Task<FieldReportResponse> FieldReport(FieldReportRequest request, int OfficerId)
        {

            // PatrolId check Zero ya Negative nahi hona chahiye
            if (request.PatrolId <= 0)
            {
                throw new ArgumentException("Valid Patrol ID dena zaroori hai.");
            }

            if (string.IsNullOrWhiteSpace(request.Notes))
            {
                throw new ArgumentException("Observation notes khali nahi ho sakte.");
            }

            if (request.Notes.Length < 10)
            {
                throw new ArgumentException("Notes thode detail mein likho (Min 10 characters).");
            }

            // Future ki date mein report nahi dal sakte
            if (request.Date > DateTime.Now)
            {
                throw new ArgumentException("Future ki date mein report submit nahi ki ja sakti.");
            }

            // Agar sab sahi hai, tabhi Repository call hogi
            return await _fieldReportRepository.FieldReport(request, OfficerId);
        }

        public async Task<List<FieldReportResponse>> GetAllFeildReport()
        {
            var response = await _fieldReportRepository.GetAllFeildReport();
            return response;
        }

        public async Task<string> ReviewFieldReportService(int reportId, int newStatus)
        {
            // Check karo ki status valid Enum range mein hai
            if (!Enum.IsDefined(typeof(FieldReportStatus), newStatus))
            {
                return "Invalid Status value bhenji gayi hai.";
            }

            // Repository ko call karo
            var result = await _fieldReportRepository.ReviewFieldReportAsync(reportId, (FieldReportStatus)newStatus);

            if (!result)
            {
                return "Report ID galat hai, record nahi mila.";
            }

            return "Success";
        }


        public async Task<IEnumerable<FieldReportResponse>> GetMyReportHistoryService(int officerId)
        {
            // Yahan agar tum chaho toh date-range filter bhi laga sakte ho
            return await _fieldReportRepository.GetMyReportHistoryAsync(officerId);
        }
    }
}