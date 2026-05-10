using SafeCity_PFOMDB.Enums;

namespace SafeCity.PFOM.DTOs
{
    public class UpdatePatrolStatusDto
    {
        public int PatrolId { get; set; }
        public PatrolStatus NewStatus { get; set; }
    }
}
