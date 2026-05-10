using SafeCity.PFOM.DTOs;

namespace SafeCity.PFOM.Services
{
    public interface IPatrolService
    {
        public Task<PatrolScheduleResponse> PatrolSchedule(PatrolScheduleRequest patrolScheduleRequest);
        public Task<List<PatrolScheduleResponse>> MyPatrols(int userId);
        public Task<IEnumerable<PatrolScheduleResponse>> GetMyPatrolHistoryService(int officerId);
    }
}
