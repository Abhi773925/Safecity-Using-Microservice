using SafeCity_EDRADB.Entities;

namespace SafeCity.EDRA.DTOs
{
    public class DispatchResponse
    {
        public int DispatchID { get; set; }
        public int IncidentID { get; set; }
        public int DispatcherID { get; set; }
        public int? ResourceID { get; set; }
        public string Status { get; set; }
    }
    public static class DispatchResponseExtension
    {
        public static DispatchResponse ToDispatchResponse(Dispatch dispatch)
        {
            return new DispatchResponse
            {
                DispatchID = dispatch.DispatchID,
                IncidentID = dispatch.IncidentID,
                DispatcherID = dispatch.DispatcherID,
                ResourceID = dispatch.ResourceID,
                Status = dispatch.Status.ToString(),
            };
        }
    }
}
