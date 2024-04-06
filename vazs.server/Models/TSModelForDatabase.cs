namespace vazs.server.Models
{
    public class TSModelForDatabase
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime Deadline { get; set; }
        public int Budget { get; set; }
        public string? DocumentExt { get; set; }
        public string ClientID { get; set; }
        public string DepartmentName { get; set; }
    }
}
