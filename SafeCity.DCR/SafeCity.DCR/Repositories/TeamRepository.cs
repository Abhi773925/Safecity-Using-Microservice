using Microsoft.EntityFrameworkCore;
using SafeCity.DCR.DTOs;
using SafeCity.DCR.HttpClients;
using SafeCity_DCRDB.Data;
using SafeCity_DCRDB.Enums;
namespace SafeCity.DCR.Repositories
{
    public class TeamRepository : ITeamRepository
    {
        private readonly SafeCityDbContext _context;
        private readonly IIdentityService _identityService;

        public TeamRepository(SafeCityDbContext context, IIdentityService identityService)
        {
            _context = context;
            _identityService = identityService;
        }

        // Team creation logic goes here
        public async Task<TeamResponse> CreateTeam(TeamRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            // check kro ki teamlead id ye ek police wala hai kya
            int teamLeadId = request.TeamLeadID;

            // wo user toh hai aur police role ka hona chahiye
            var isPoliceOfficer = await _identityService.IsPoliceOfficerAsync(teamLeadId);

            if (!isPoliceOfficer)
            {
                throw new Exception("TeamLead Id is not a police officer");
            }

            // ab check kro ki wo user ke pass ek hi team hai
            var teamDetails = await _context.Teams.FirstOrDefaultAsync(temp => temp.TeamLeadID == request.TeamLeadID);
            if (teamDetails != null)
            {
                throw new Exception("This Police Officer has already assigned to some different team");
            }
            // agar sab kuch sahi rha tab

            var entryDetails = request.ToTeamRequest();
            await _context.Teams.AddAsync(entryDetails);
            await _context.SaveChangesAsync();

            return TeamResponseExtension.ToTeamResponse(entryDetails);

        }

        // logic to get all the active team member details
        public async Task<List<TeamResponse>> GetActiveTeamDetails()
        {
            var response = await _context.Teams.ToListAsync();
            if (response == null)
            {
                throw new Exception($"{nameof(GetAllTeam)}");
            }
            var filteredResponse = response.Where(temp => temp.Status == TeamStatus.Active).ToList();
            return filteredResponse.Select(i => TeamResponseExtension.ToTeamResponse(i)).ToList();
        }

        // logic to get all the team
        public async Task<List<TeamResponse>> GetAllTeam()
        {
            var response = await _context.Teams.ToListAsync();
            if (response == null)
            {
                throw new Exception($"{nameof(GetAllTeam)}");
            }

            return response.Select(i => TeamResponseExtension.ToTeamResponse(i)).ToList();
        }

        public async Task UpdateTeamStatus(int newStatus, int id)
        {
            var teamDetails = await _context.Teams.FindAsync(id);
            if (teamDetails == null)
            {
                throw new Exception("Team Details not found");
            }
            teamDetails.Status = (TeamStatus)newStatus;
            _context.Teams.Update(teamDetails);
            await _context.SaveChangesAsync();
        }
    }
}
