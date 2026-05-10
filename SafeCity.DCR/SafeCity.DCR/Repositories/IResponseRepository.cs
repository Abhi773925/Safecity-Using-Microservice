using SafeCity.DCR.DTOs;

namespace SafeCity.DCR.Repositories
{
    public interface IResponseRepository
    {
        Task<DeploymentResponse> DeployTeamAsync(DeploymentRequest request);
        Task<IEnumerable<DeploymentResponse>> GetActiveDeploymentsAsync();
        Task<bool> ReassignTeamAsync(int responseId, int newTeamId);
        Task<bool> CancelDeploymentAsync(int id);
        Task<DeploymentResponse?> GetResponseByIdAsync(int id);

        Task UpdateResponseStatus(int responseId, int teamLeadId, UpdateProgressRequest updateProgressRequest);
        Task UpdateCloseMissionAsync(int responseId, CloseMissionRequest request);
    }
}