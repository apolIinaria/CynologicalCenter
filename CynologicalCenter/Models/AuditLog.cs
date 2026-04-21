using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CynologicalCenter.Models
{
    public class AuditLog
    {
        public int LogId { get; set; }
        public string? TableName { get; set; }
        public int? RecordId { get; set; }
        public string? Action { get; set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public DateTime ChangedAt { get; set; }
        public string? ChangedBy { get; set; }
        public string? Description { get; set; }
        public string? OperationSource { get; set; }
    }
}
