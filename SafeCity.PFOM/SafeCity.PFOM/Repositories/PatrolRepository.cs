using Microsoft.EntityFrameworkCore;
using SafeCity.PFOM.HttpClients;
using SafeCity.PFOM.DTOs;
using SafeCity_PFOMDB.Data;
using SafeCity_PFOMDB.Enums;

namespace SafeCity.PFOM.Repositories
{
    public class PatrolRepository : IPatrolRepository
    {
        private readonly SafeCityDbContext _context;
        private readonly IIdentityService _identityService;

        public PatrolRepository(SafeCityDbContext context, IIdentityService identityService)
        {
            _context = context;
            _identityService = identityService;
        }

        // My Patrol Logic goes here
        public async Task<List<PatrolScheduleResponse>> MyPatrols(int userId)
        {
            var response = await _context.Patrols.ToListAsync();
            var myResponse = response.Where(temp => temp.OfficerId == userId).ToList();

            return myResponse.Select(i => PatrolScheduleExtension.ToPatrolScheduleResponse(i)).ToList();


        }

        // Patrol Schedule logic 
        public async Task<PatrolScheduleResponse> PatrolSchedule(PatrolScheduleRequest patrolScheduleRequest)
        {
            int officerId = patrolScheduleRequest.OfficerId;

            // valid officer id check
            var isOfficerValid = await _identityService.IsOfficerValidAsync(officerId);

            if (!isOfficerValid)
            {
                throw new Exception("Invalid User Id");
            }


            // Check karo ki us specific officer ki us din koi duty to nahi hai
            var checkExistingDuty = await _context.Patrols
                .FirstOrDefaultAsync(p => p.OfficerId == patrolScheduleRequest.OfficerId
                                       && p.Date.Date == patrolScheduleRequest.Date.Date);

            if (checkExistingDuty != null)
            {
                throw new Exception("Duty is already scheduled for this officer on the selected day.");
            }

            // ab database me add krskte hai
            var patrolScheduleDetails = patrolScheduleRequest.ToPatrolScheduleRequest();
            await _context.Patrols.AddAsync(patrolScheduleDetails);
            await _context.SaveChangesAsync();

            return PatrolScheduleExtension.ToPatrolScheduleResponse(patrolScheduleDetails);

        }




        public async Task<string> UpdatePatrolStatusAsync(int patrolId, int loggedInOfficerId, PatrolStatus newStatus)
        {
            var patrol = await _context.Patrols.FindAsync(patrolId);

            if (patrol == null) return "Patrol record nahi mila.";

            // Check karo ki wahi officer hai jiski duty hai
            if (patrol.OfficerId != loggedInOfficerId)
                return "Ye aapki assigned duty nahi hai!";

            // Start Patrol Logic Inactive -> OnPatrol
            if (newStatus == PatrolStatus.OnPatrol)
            {
                if (patrol.Status != PatrolStatus.Inactive)
                    return "Patrol pehle se active hai ya khatam ho chuki hai.";

                patrol.Status = PatrolStatus.OnPatrol;
            }
            //  End Patrol Logic OnPatrol -> Inactive
            else if (newStatus == PatrolStatus.Inactive)
            {
                if (patrol.Status != PatrolStatus.OnPatrol)
                    return "Pehle patrol start toh karo!";

                patrol.Status = PatrolStatus.Inactive;
                // Yahan ActualEndTime update kar sakte ho agar table mein column hai
                patrol.Date = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return "Success";
        }


        public async Task<IEnumerable<PatrolScheduleResponse>> GetMyPatrolHistoryAsync(int officerId)
        {
            // Database se us officer ki saari purani duties fetch karna
            var patrols = await _context.Patrols
                .Where(p => p.OfficerId == officerId)
                .OrderByDescending(p => p.Date) // Nayi duty sabse upar
                .ToListAsync();

            // DTO mein map karke return karna
            return patrols.Select(p => PatrolScheduleExtension.ToPatrolScheduleResponse(p));
        }
    }
}
