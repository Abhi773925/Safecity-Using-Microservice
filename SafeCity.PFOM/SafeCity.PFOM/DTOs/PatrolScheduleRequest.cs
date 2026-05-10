using SafeCity_PFOMDB.Entities;
using SafeCity_PFOMDB.Enums;

namespace SafeCity.PFOM.DTOs
{
    public class PatrolScheduleRequest
    {
        public int OfficerId { get; set; }
        public string Area { get; set; } = default!;
        public DateTime Date { get; set; }
        public PatrolStatus Status { get; set; } = PatrolStatus.Inactive;

        public Patrol ToPatrolScheduleRequest()
        {
            return new Patrol
            {
                OfficerId = OfficerId,
                Area = Area,
                Date = Date,
                Status = Status
            };
        }
    }
}
