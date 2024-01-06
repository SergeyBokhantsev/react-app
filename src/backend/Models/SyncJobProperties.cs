using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApp.Models
{
    public class SyncJobProperties
    {
        public string? JobId { get; set; }
        public string? TenantUrl { get; set; }
        public string? JobLogUrl { get; set; }
        public string? TenantId { get; set; }
        public string? IsCustomizationUpgrade { get; set; }
        public string? OrganizationId { get; set; }
        public string? EmployeeId { get; set; }
        public string? UserEmail { get; set; }
        public string? SlaveConnector { get; set; }
        public string? ExchangeVersion { get; set; }
    }
}
