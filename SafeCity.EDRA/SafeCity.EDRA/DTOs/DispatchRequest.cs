using SafeCity_EDRADB.Entities;

namespace SafeCity.EDRA.DTOs
{
    public class DispatchRequest
    {
        public int IncidentID { get; set; }
        public int ResourceID { get; set; }
        public int DispatcherID { get; set; }

        public Dispatch ToDispatchRequest()
        {
            return new Dispatch { IncidentID = IncidentID, ResourceID = ResourceID, DispatcherID = DispatcherID };
        }
    }
}