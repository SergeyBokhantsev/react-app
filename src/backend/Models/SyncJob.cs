namespace FunctionApp.Models
{
    public class SyncJob
    {
        public required string Id { get; set; }
        public required string EmployeeId { get; set; }
        public bool IsTarget { get; set; }
        public DateTime Time { get; set; }
        public required string Status { get; set; }
        public string? AppRoleInstance { get; set; }
        public SyncJobProperties? Properties { get; set; }
        public string? TenantId { get; set; }
        public string? OperationName { get; set; }
        public string? AppVersion { get; set; }
    }
}
