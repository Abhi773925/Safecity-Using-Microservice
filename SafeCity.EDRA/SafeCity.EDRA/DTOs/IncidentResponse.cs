namespace SafeCity.EDRA.DTOs
{
    public class IncidentResponse
    {
        public int IncidentID { get; set; }
        public int CitizenID { get; set; }
        public string Type { get; set; } = default!;
        public string Location { get; set; } = default!;
        public DateTime Date { get; set; }
        public string Status { get; set; } = default!;
    }

    public class IncidentServiceWrapper
    {
        public string Message { get; set; } = default!;
        public List<IncidentResponse> Data { get; set; } = default!;
    }
}