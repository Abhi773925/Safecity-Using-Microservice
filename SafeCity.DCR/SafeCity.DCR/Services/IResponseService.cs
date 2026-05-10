using SafeCity.DCR.DTOs;

namespace SafeCity.DCR.Services
{
    public interface IResponseService
    {
        Task<DeploymentResponse> DeployAsync(DeploymentRequest request);
        Task<IEnumerable<DeploymentResponse>> GetAllActive();
        Task<bool> CancelAsync(int id);
        public Task<bool> ReassignAsync(int responseId, int newTeamId);
        Task UpdateResponseStatus(int responseId, int teamLeadId, UpdateProgressRequest updateProgressRequest);

        Task CloseMission(int id, CloseMissionRequest request);
    }
}
