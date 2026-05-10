using SafeCity_EDRADB.Entities;
using SafeCity_EDRADB.Enums;
namespace SafeCity.EDRA.DTOs
{
    public class ResourceRequest
    {
        public ResourceTypeOption Type { get; set; } = ResourceTypeOption.Vehicle;
        public ResourceAvailabilityOption Availability { get; set; } = ResourceAvailabilityOption.Available;
        public string Location { get; set; }
        public string UnitName { get; set; }

        public Resource ToResourceRequest()
        {
            return new Resource
            {
                Type = Type,
                Availability = Availability,
                Location = Location,
                UnitName = UnitName
            };
        }
    }
}
