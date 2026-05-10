using SafeCity_PFOMDB.Entities;
using SafeCity_PFOMDB.Enums;

namespace SafeCity.PFOM.DTOs
{
    public class FieldReportRequest
    {
        public int PatrolId { get; set; }
        public string Notes { get; set; } = default!;
        public DateTime Date { get; set; }
        public FieldReportStatus Status { get; set; }

        public FieldReport ToFieldReportRequest()
        {
            return new FieldReport
            {
                PatrolId = PatrolId,
                Notes = Notes,
                Date = Date,
                Status = Status
            };
        }
    }

    public class FieldReportResponse
    {
        public int ReportId { get; set; }
        public int PatrolId { get; set; }
        public string Notes { get; set; } = default!;
        public DateTime Date { get; set; }
        public string Status { get; set; } = default!;

    }
    public static class FieldReportResponseExtension
    {
        public static FieldReportResponse ToFieldReportResponse(FieldReport report)
        {
            return new FieldReportResponse
            {
                ReportId = report.ReportId,
                PatrolId = report.PatrolId,
                Notes = report.Notes,
                Date = report.Date,
                Status = report.Status.ToString()
            };
        }
    }
}
