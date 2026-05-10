using SafeCity.PFOM.DTOs;
using SafeCity.PFOM.Repositories;
using SafeCity_PFOMDB.Enums;

namespace SafeCity.PFOM.Services
{
    public class PatrolService : IPatrolService
    {
        private readonly IPatrolRepository _repository;
        public PatrolService(IPatrolRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<PatrolScheduleResponse>> MyPatrols(int userId)
        {
            var response = await _repository.MyPatrols(userId);
            return response;
        }

        public async Task<PatrolScheduleResponse> PatrolSchedule(PatrolScheduleRequest request)
        {
            if (request == null)
            {
                throw new Exception(nameof(request));
            }

            // basic rule check

            List<string> errorList = new List<string>();

            // OfficerId Check
            if (request.OfficerId <= 0)
            {
                errorList.Add("Officer ID must be a positive number.");
            }

            // Area Check
            if (string.IsNullOrWhiteSpace(request.Area))
            {
                errorList.Add("Area name is required.");
            }
            else if (request.Area.Length < 3 || request.Area.Length > 100)
            {
                errorList.Add("Area name must be between 3 and 100 characters.");
            }

            // Date Check (Past date allow nahi karni)
            if (request.Date < DateTime.Now.Date)
            {
                errorList.Add("Patrol date cannot be in the past.");
            }

            // Status Check
            if (!Enum.IsDefined(typeof(PatrolStatus), request.Status))
            {
                errorList.Add("Invalid Patrol Status value.");
            }

            // if any validation fail
            if (errorList.Any())
            {
                throw new Exception(string.Join(" | ", errorList));
            }

            else
            {
                var response = await _repository.PatrolSchedule(request);
                return response;
            }
        }


        public async Task<IEnumerable<PatrolScheduleResponse>> GetMyPatrolHistoryService(int officerId)
        {
            // Yahan tum chaho toh check kar sakte ho ki officerId valid hai ya nahi
            return await _repository.GetMyPatrolHistoryAsync(officerId);
        }
    }
}
