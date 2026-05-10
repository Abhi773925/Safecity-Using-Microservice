using Microsoft.EntityFrameworkCore;
using SafeCity.DCR.DTOs;
using SafeCity_DCRDB.Data;
using SafeCity_DCRDB.Entities;
using SafeCity_DCRDB.Enums;

namespace SafeCity.DCR.Repositories
{
    public class ResponseRepository : IResponseRepository
    {
        private readonly SafeCityDbContext _context;
        public ResponseRepository(SafeCityDbContext context) => _context = context;

        // DEPLOY TEAM 
        public async Task<DeploymentResponse> DeployTeamAsync(DeploymentRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Team availability check 
                var team = await _context.Teams.FindAsync(request.TeamId)
                           ?? throw new Exception("Team not found!");

                if (team.Status != TeamStatus.Active)
                    throw new Exception("Team is currently Inactive/Busy!");

                //  Crisis Check
                var crisis = await _context.Crises.FindAsync(request.CrisisId)
                             ?? throw new Exception("Crisis not found!");

                if (crisis.Status == CrisisStatus.Resolved)
                    throw new Exception("Cannot deploy to a Resolved crisis.");

                // Create Response Record
                var response = new Response
                {
                    CrisisID = request.CrisisId,
                    TeamID = request.TeamId,
                    Actions = request.SpecialInstructions,
                    Date = DateTime.UtcNow,
                    Status = ResponseStatus.Pending,
                    CrisisIdNavigation = crisis,
                    TeamIdNavigation = team
                };

                //  Update Team Status Active -> Inactive
                team.Status = TeamStatus.Inactive;

                // Update Crisis Status
                if (crisis.Status == CrisisStatus.Pending)
                    crisis.Status = CrisisStatus.Active;

                await _context.Responses.AddAsync(response);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return MapToDto(response, team, crisis);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // REASSIGN TEAM
        public async Task<bool> ReassignTeamAsync(int responseId, int newTeamId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var response = await _context.Responses
                    .Include(r => r.TeamIdNavigation)
                    .FirstOrDefaultAsync(r => r.ResponseID == responseId);

                if (response == null) return false;

                var newTeam = await _context.Teams.FindAsync(newTeamId);
                if (newTeam == null || newTeam.Status != TeamStatus.Active) return false;

                // Old team free (Active), New team busy (Inactive)
                response.TeamIdNavigation.Status = TeamStatus.Active;
                newTeam.Status = TeamStatus.Inactive;

                response.TeamID = newTeamId;
                response.Actions += $" | Reassigned on {DateTime.UtcNow}";

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch { await transaction.RollbackAsync(); return false; }
        }

        // CANCEL DEPLOYMENT
        public async Task<bool> CancelDeploymentAsync(int id)
        {
            var response = await _context.Responses
                .Include(r => r.TeamIdNavigation)
                .FirstOrDefaultAsync(r => r.ResponseID == id);

            if (response == null) return false;

            // Release team back to Active status
            response.TeamIdNavigation.Status = TeamStatus.Active;
            response.Status = ResponseStatus.Cancelled;

            _context.Responses.Remove(response);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<DeploymentResponse>> GetActiveDeploymentsAsync()
        {
            var data = await _context.Responses
                .Include(r => r.CrisisIdNavigation)
                .Include(r => r.TeamIdNavigation)
                .Where(r => r.Status != ResponseStatus.Closed && r.Status != ResponseStatus.Resolved)
                .ToListAsync();

            return data.Select(r => MapToDto(r, r.TeamIdNavigation, r.CrisisIdNavigation));
        }

        private static DeploymentResponse MapToDto(Response r, Team t, Crisis c) => new()
        {
            ResponseId = r.ResponseID,
            CrisisId = c.CrisisID,
            CrisisLocation = c.Location,
            TeamName = t.TeamName,
            TeamLead = t.TeamLeadID,
            Status = r.Status.ToString(),
            DeployedAt = r.Date,
            Instructions = r.Actions
        };

        public async Task<DeploymentResponse?> GetResponseByIdAsync(int id)
        {
            var r = await _context.Responses
                .Include(r => r.CrisisIdNavigation)
                .Include(r => r.TeamIdNavigation)
                .FirstOrDefaultAsync(x => x.ResponseID == id);
            return r != null ? MapToDto(r, r.TeamIdNavigation, r.CrisisIdNavigation) : null;
        }


        public async Task UpdateResponseStatus(int responseId, int teamLeadId, UpdateProgressRequest updateProgressRequest)
        {
            // Fetch Response with Crisis
            var responseDetails = await _context.Responses
                .Include(r => r.CrisisIdNavigation)
                .FirstOrDefaultAsync(r => r.ResponseID == responseId);

            if (responseDetails == null) throw new Exception("No Response Details Found");

            // Debugging version of your logic
            var teamDetails = await _context.Teams.FirstOrDefaultAsync(temp => temp.TeamLeadID == teamLeadId);

            if (teamDetails == null)
            {
                throw new Exception($"System ko Team Lead {teamLeadId} ki koi team hi nahi mili!");
            }

            // Dono IDs ko compare karke dekho
            if (responseDetails.TeamID != teamDetails.TeamID)
            {
                // Yahan hum exact bata rahe hain ki kaunsi ID mismatch hui
                throw new Exception($"ID Mismatch! Mission Assigned to Team: {responseDetails.TeamID}, But You belong to Team: {teamDetails.TeamID}");
            }

            responseDetails.Status = updateProgressRequest.NewStatus;
            string timestamp = DateTime.Now.ToString("dd-MM-yyyy HH:mm");
            responseDetails.Actions += $"\n[{timestamp}]: {updateProgressRequest.UpdateNote}";

            // Sync Crisis Status
            if (updateProgressRequest.NewStatus == ResponseStatus.Stabilized)
            {
                responseDetails.CrisisIdNavigation.Status = CrisisStatus.Stabilized;
            }

            await _context.SaveChangesAsync();
        }


        public async Task UpdateCloseMissionAsync(int responseId, CloseMissionRequest request)
        {

            var response = await _context.Responses
                .Include(r => r.CrisisIdNavigation)
                .Include(r => r.TeamIdNavigation)
                .FirstOrDefaultAsync(r => r.ResponseID == responseId);

            if (response == null) throw new Exception("Mission not found!");

            // Status Updates
            response.Status = ResponseStatus.Resolved;
            response.CrisisIdNavigation.Status = CrisisStatus.Resolved;

            // RESOURCE RELEASE
            response.TeamIdNavigation.Status = TeamStatus.Active;

            // Final Timeline Entry in Actions
            string timestamp = DateTime.Now.ToString("dd-MM-yyyy HH:mm");
            response.Actions += $"\n[{timestamp}]: MISSION CLOSED - {request.FinalClosingNote}";

            await _context.SaveChangesAsync();
        }
    }
}