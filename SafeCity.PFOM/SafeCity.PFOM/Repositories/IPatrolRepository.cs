using SafeCity.PFOM.DTOs;
using SafeCity_PFOMDB.Enums;

namespace SafeCity.PFOM.Repositories
{
    public interface IPatrolRepository
    {
        public Task<PatrolScheduleResponse> PatrolSchedule(PatrolScheduleRequest patrolScheduleRequest);

        public Task<List<PatrolScheduleResponse>> MyPatrols(int userId);
        Task<string> UpdatePatrolStatusAsync(int patrolId, int loggedInOfficerId, PatrolStatus newStatus);

        Task<IEnumerable<PatrolScheduleResponse>> GetMyPatrolHistoryAsync(int officerId);
    }
}
