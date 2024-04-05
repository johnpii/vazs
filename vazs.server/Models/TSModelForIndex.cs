namespace vazs.server.Models
{
    public class TSModelForIndex
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime Deadline { get; set; }
        public int Budget { get; set; }
        public string DepartmentName { get; set; }
        public string DownloadUrl { get; set; }
    }
}
