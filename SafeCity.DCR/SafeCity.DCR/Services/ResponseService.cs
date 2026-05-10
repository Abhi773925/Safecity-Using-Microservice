using SafeCity.DCR.DTOs;
using SafeCity.DCR.Repositories;
using SafeCity.DCR.Services;

public class ResponseService : IResponseService
{
    private readonly IResponseRepository _repo;
    public ResponseService(IResponseRepository repo) => _repo = repo;

    public async Task<DeploymentResponse> DeployAsync(DeploymentRequest request)
        => await _repo.DeployTeamAsync(request);

    public async Task<IEnumerable<DeploymentResponse>> GetAllActive()
        => await _repo.GetActiveDeploymentsAsync();

    public async Task<bool> CancelAsync(int id)
        => await _repo.CancelDeploymentAsync(id);

    public async Task<bool> ReassignAsync(int responseId, int newTeamId)
        => await _repo.ReassignTeamAsync(responseId, newTeamId);

    public async Task UpdateResponseStatus(int responseId, int teamLeadId, UpdateProgressRequest updateProgressRequest)
    {
        await _repo.UpdateResponseStatus(responseId, teamLeadId, updateProgressRequest);
    }

    public async Task CloseMission(int id, CloseMissionRequest request)
    {
        await _repo.UpdateCloseMissionAsync(id, request);
    }
}