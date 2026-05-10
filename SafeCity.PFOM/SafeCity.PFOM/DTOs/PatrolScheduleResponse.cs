using SafeCity_PFOMDB.Entities;

namespace SafeCity.PFOM.DTOs
{
    public class PatrolScheduleResponse
    {
        public int PatrolId { get; set; }
        public int OfficerId { get; set; }
        public string Area { get; set; } = default!;
        public DateTime Date { get; set; }
        public string Status { get; set; } = default!;
    }
    public static class PatrolScheduleExtension
    {
        public static PatrolScheduleResponse ToPatrolScheduleResponse(Patrol patrol)
        {
            return new PatrolScheduleResponse
            {
                PatrolId = patrol.PatrolId,
                OfficerId = patrol.OfficerId,
                Area = patrol.Area,
                Date = patrol.Date,
                Status = patrol.Status.ToString(),
            };
        }
    }
}
