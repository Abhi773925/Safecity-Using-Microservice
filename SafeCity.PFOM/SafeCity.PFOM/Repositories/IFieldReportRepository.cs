using SafeCity.PFOM.DTOs;
using SafeCity_PFOMDB.Enums;

namespace SafeCity.PFOM.Repositories
{
    public interface IFieldReportRepository
    {
        public Task<FieldReportResponse> FieldReport(FieldReportRequest request, int OfficerId);

        public Task<List<FieldReportResponse>> GetAllFeildReport();
        Task<bool> ReviewFieldReportAsync(int reportId, FieldReportStatus newStatus);
        Task<IEnumerable<FieldReportResponse>> GetMyReportHistoryAsync(int officerId);
    }
}
