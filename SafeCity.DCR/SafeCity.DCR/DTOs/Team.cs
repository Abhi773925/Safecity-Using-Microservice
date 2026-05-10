using SafeCity_DCRDB.Entities;
using SafeCity_DCRDB.Enums;
using System.ComponentModel.DataAnnotations;

namespace SafeCity.DCR.DTOs
{
    public class TeamRequest
    {
        [Required(ErrorMessage = "Team Name is required")]
        public string TeamName { get; set; } = default!;
        [Required(ErrorMessage = "Team lead is required")]
        public int TeamLeadID { get; set; }
        public TeamStatus Status { get; set; } = TeamStatus.Active;

        public Team ToTeamRequest()
        {
            return new Team
            {
                TeamName = TeamName,
                TeamLeadID = TeamLeadID,
                Status = Status
            };
        }
    }

    public class TeamResponse
    {
        public int TeamID { get; set; }
        public string TeamName { get; set; } = default!;
        public int TeamLeadID { get; set; }
        public string Status { get; set; } = default!;
    }

    public class UpdateTeamStatusRequest
    {
        [Required(ErrorMessage = "New status is required")]
        public int NewStatus { get; set; }
    }

    public static class TeamResponseExtension
    {
        public static TeamResponse ToTeamResponse(Team team)
        {
            return new TeamResponse
            {
                TeamID = team.TeamID,
                TeamName = team.TeamName,
                TeamLeadID = team.TeamLeadID,
                Status = team.Status.ToString()
            };
        }
    }
}
