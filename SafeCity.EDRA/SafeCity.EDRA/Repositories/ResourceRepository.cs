using Microsoft.EntityFrameworkCore;
using SafeCity_EDRADB.Enums;
using SafeCity_EDRADB.Data;
using SafeCity.EDRA.DTOs;
using SafeCity.EDRA.HttpClients;

namespace SafeCity.EDRA.Repositories
{
    public class ResourceRepository : IResourceRepository
    {
        // dependency injection
        private readonly SafeCityDbContext _context;
        private readonly IIncidentService _incidentService;

        public ResourceRepository(SafeCityDbContext context, IIncidentService incidentService)
        {
            _context = context;
            _incidentService = incidentService;
        }

        public async Task<ResourceResponse> AddResource(ResourceRequest resourceRequest)
        {
            if (resourceRequest == null)
            {
                throw new ArgumentNullException(nameof(resourceRequest));
            }
            else
            {
                var resourceDetails = resourceRequest.ToResourceRequest();
                var response = await _context.Resources.AddAsync(resourceDetails);
                await _context.SaveChangesAsync();
                return ResourceResponseExtension.ToResourceResponse(resourceDetails);
            }
        }

        public async Task<DispatchResponse> AssignResource(DispatchRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Resource ko check karein ki wo Available hai ya nahi
                var findResource = await _context.Resources.FindAsync(request.ResourceID);
                if (findResource == null) throw new Exception("Resource not found");

                if (findResource.Availability != ResourceAvailabilityOption.Available)
                    throw new Exception("Resource is already busy or under maintenance");

                // Dispatch record create karein
                var dispatchDetails = request.ToDispatchRequest();
                dispatchDetails.Status = DispatchStatusOption.Assigned;
                await _context.Dispatches.AddAsync(dispatchDetails);

                // Resource status update karein
                findResource.Availability = ResourceAvailabilityOption.OnTask;
                _context.Resources.Update(findResource);

                await _incidentService.UpdateIncidentStatusAsync(request.IncidentID, 1);

                // Sab sahi hai toh save aur commit karein
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return DispatchResponseExtension.ToDispatchResponse(dispatchDetails);
            }
            catch (Exception)
            {
                // Kuch galat hua toh sab wapas pehle jaisa
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<List<ResourceResponse>> GetAllResource()
        {
            var response = await _context.Resources.ToListAsync();
            var availableResource = response.Where(temp => temp.Availability == 0);
            return availableResource.Select(i => ResourceResponseExtension.ToResourceResponse(i)).ToList();
        }

        public async Task<ResourceResponse> UpdateResource(int id, ResourceRequest request)
        {
            var resource = await _context.Resources.FindAsync(id);
            if (resource == null)
                throw new Exception("Resource not found");

            resource.Type = request.Type;
            resource.Availability = request.Availability;
            resource.Location = request.Location;
            resource.UnitName = request.UnitName;

            await _context.SaveChangesAsync();
            return ResourceResponseExtension.ToResourceResponse(resource);
        }

        public async Task<List<DispatchResponse>> GetAllDispatches()
        {
            var dispatches = await _context.Dispatches.ToListAsync();
            return dispatches.Select(d => DispatchResponseExtension.ToDispatchResponse(d)).ToList();
        }

        public async Task<bool> UpdateDispatchStatusAsync(int dispatchId, DispatchStatusOption status)
        {
            var dispatch = await _context.Dispatches.FindAsync(dispatchId);
            if (dispatch == null)
            {
                throw new Exception("Dispatch record not found.");
            }

            // Status update logic Assigned -> EnRoute -> OnSite
            dispatch.Status = status;

            _context.Dispatches.Update(dispatch);
            return await _context.SaveChangesAsync() > 0;
        }


        public async Task<bool> CompleteDispatchAsync(int dispatchId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Dispatch record dhoondo
                var dispatch = await _context.Dispatches.FindAsync(dispatchId);
                if (dispatch == null) throw new Exception("Dispatch record not found.");

                // Dispatch status ko Resolved mark karo
                dispatch.Status = DispatchStatusOption.Resolved;

                // Linked Resource ko wapas Available karo
                var resource = await _context.Resources.FindAsync(dispatch.ResourceID);
                if (resource != null)
                {
                    resource.Availability = ResourceAvailabilityOption.Available;
                    _context.Resources.Update(resource);
                }

                await _incidentService.UpdateIncidentStatusAsync(dispatch.IncidentID, 2);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {
                // Error aaya toh purana data wapas le aao
                await transaction.RollbackAsync();
                throw;
            }
        }
    }

}
