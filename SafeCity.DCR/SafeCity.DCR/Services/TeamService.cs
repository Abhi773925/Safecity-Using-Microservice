using SafeCity.DCR.DTOs;
using SafeCity.DCR.Repositories;

namespace SafeCity.DCR.Services
{
    public class TeamService : ITeamService
    {
        private readonly ITeamRepository _teamRepository;
        public TeamService(ITeamRepository teamRepository)
        {
            _teamRepository = teamRepository;
        }
        public async Task<TeamResponse> CreateTeam(TeamRequest request)
        {

            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            else
            {
                var response = await _teamRepository.CreateTeam(request);
                return response;
            }
        }

        public async Task<List<TeamResponse>> GetActiveTeamDetails()
        {
            var response = await _teamRepository.GetActiveTeamDetails();
            return response;
        }

        public async Task<List<TeamResponse>> GetAllTeam()
        {
            var response = await _teamRepository.GetAllTeam();
            return response;
        }

        public async Task UpdateTeamStatus(int newStatus, int id)
        {
            await _teamRepository.UpdateTeamStatus(newStatus, id);
        }
    }
}
