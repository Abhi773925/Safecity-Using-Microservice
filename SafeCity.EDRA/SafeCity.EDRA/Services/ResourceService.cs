using SafeCity_EDRADB.Enums;
using SafeCity.EDRA.DTOs;
using SafeCity.EDRA.Repositories;

namespace SafeCity.EDRA.Services
{
    public class ResourceService : IResourceService
    {
        // Dependency Injection Goes Here.
        private readonly IResourceRepository _repository;
        public ResourceService(IResourceRepository repository)
        {
            _repository = repository;
        }

        // Adding the Resource Logic Validation goes here.
        public async Task<ResourceResponse> AddResource(ResourceRequest resourceRequest)
        {
            if (resourceRequest == null)
            {
                throw new ArgumentNullException(nameof(resourceRequest));
            }
            // basic field validation check 
            var errorList = new List<string>();

            if (resourceRequest.Type < 0)
            {
                errorList.Add("Invalid Resource Type");
            }
            if (resourceRequest.Availability < 0)
            {
                errorList.Add("Invalid Resouce Availablity");
            }
            if (resourceRequest.Location == null)
            {
                errorList.Add("Location is missing");
            }
            if (resourceRequest.UnitName == null)
            {
                errorList.Add("Unit Name is missing");
            }

            // throwing if any validation fails
            if (errorList.Count > 0)
            {
                throw new Exception(string.Join(" | ", errorList));
            }
            else
            {
                var response = await _repository.AddResource(resourceRequest);
                return response;
            }
        }

        public async Task<DispatchResponse> AssignResource(DispatchRequest request)
        {
            if (request == null)
            {
                throw new Exception("request is null");
            }
            else
            {
                var response = await _repository.AssignResource(request);
                return response;
            }
        }

        public Task<List<ResourceResponse>> GetAllResource()
        {
            var response = _repository.GetAllResource();
            if (response == null)
            {
                throw new Exception("No Available Resource Found");
            }
            return response;
        }

        public async Task<List<ResourceResponse>> GetAllResources()
        {
            return await GetAllResource();
        }

        public async Task<ResourceResponse> UpdateResource(int id, ResourceRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));
            return await _repository.UpdateResource(id, request);
        }

        public async Task<List<DispatchResponse>> GetAllDispatches()
        {
            return await _repository.GetAllDispatches();
        }
        public async Task<bool> UpdateDispatchStatusAsync(int dispatchId, DispatchStatusOption status)
        {
            if (dispatchId <= 0) throw new Exception("Invalid Dispatch ID");

            // Yahan check kar sakte hain ki Resolved ya Cancelled status ko wapas EnRoute na kiya jaye
            if (status == DispatchStatusOption.Assigned)
                throw new Exception("Cannot set status back to Assigned once dispatched");

            return await _repository.UpdateDispatchStatusAsync(dispatchId, status);
        }

        public async Task<bool> CompleteDispatchAsync(int dispatchId)
        {
            if (dispatchId <= 0) throw new Exception("Invalid Dispatch ID");
            return await _repository.CompleteDispatchAsync(dispatchId);
        }
    }
}
