using SafeCity.PFOM.DTOs;

namespace SafeCity.PFOM.Services
{
    public interface IFieldReportService
    {
        public Task<FieldReportResponse> FieldReport(FieldReportRequest request, int OfficerId);

        public Task<List<FieldReportResponse>> GetAllFeildReport();
        public Task<string> ReviewFieldReportService(int reportId, int newStatus);
        Task<IEnumerable<FieldReportResponse>> GetMyReportHistoryService(int officerId);
    }
}
