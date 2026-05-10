using SafeCity.DCR.DTOs;

namespace SafeCity.DCR.Services
{
    public interface ITeamService
    {
        public Task<TeamResponse> CreateTeam(TeamRequest request);
        Task<List<TeamResponse>> GetAllTeam();
        public Task<List<TeamResponse>> GetActiveTeamDetails();
        public Task UpdateTeamStatus(int newStatus, int id);

    }
}
