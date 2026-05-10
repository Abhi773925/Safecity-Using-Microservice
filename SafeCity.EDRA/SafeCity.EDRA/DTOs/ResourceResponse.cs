using SafeCity_EDRADB.Entities;

namespace SafeCity.EDRA.DTOs
{
    public class ResourceResponse
    {
        public int ResourceID { get; set; }
        public string Type { get; set; }
        public string Availability { get; set; }
        public string Location { get; set; }
        public string UnitName { get; set; }

    }
    public static class ResourceResponseExtension
    {
        public static ResourceResponse ToResourceResponse(Resource response)
        {
            return new ResourceResponse
            {
                ResourceID = response.ResourceID,
                Type = response.Type.ToString(),
                Availability = response.Availability.ToString(),
                Location = response.Location,
                UnitName = response.UnitName,
            };
        }
    }
}
