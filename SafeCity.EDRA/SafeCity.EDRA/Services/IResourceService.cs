using SafeCity_EDRADB.Enums;
using SafeCity.EDRA.DTOs;

namespace SafeCity.EDRA.Services
{
    public interface IResourceService
    {
        public Task<ResourceResponse> AddResource(ResourceRequest resourceRequest);
        public Task<List<ResourceResponse>> GetAllResource();
        public Task<List<ResourceResponse>> GetAllResources();
        public Task<ResourceResponse> UpdateResource(int id, ResourceRequest request);
        public Task<List<DispatchResponse>> GetAllDispatches();
        public Task<DispatchResponse> AssignResource(DispatchRequest request);
        Task<bool> UpdateDispatchStatusAsync(int dispatchId, DispatchStatusOption status);
        Task<bool> CompleteDispatchAsync(int dispatchId);
    }
}
