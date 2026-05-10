namespace SafeCity_IRCMDB.Entity
{
    public class User
    {
        public int UserID { get; set; }
        public string Name { get; set; } = string.Empty;
        public int RoleID { get; set; }
        public virtual ICollection<Incident> Incidents { get; set; } = new List<Incident>();
        public virtual ICollection<Case> AssignedCases { get; set; } = new List<Case>();
    }
}