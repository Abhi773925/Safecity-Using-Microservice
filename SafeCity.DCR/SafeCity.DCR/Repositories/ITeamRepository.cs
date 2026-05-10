using SafeCity.DCR.DTOs;

namespace SafeCity.DCR.Repositories
{
    public interface ITeamRepository
    {
        public Task<TeamResponse> CreateTeam(TeamRequest request);
        public Task<List<TeamResponse>> GetAllTeam();
        public Task<List<TeamResponse>> GetActiveTeamDetails();
        public Task UpdateTeamStatus(int newStatus, int id);
    }
}
