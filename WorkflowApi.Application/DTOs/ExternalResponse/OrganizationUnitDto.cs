using WorkflowApi.Application.DTOs.Common;

namespace WorkflowApi.Application.DTOs.ExternalResponse
{
    public class OrganizationUnitDto : AuditDto
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string NameLocal { get; set; } = string.Empty;
        public int? ParentId { get; set; }
        public string? ParentName { get; set; }
        public int Level { get; set; }
        public string LevelName { get; set; } = string.Empty;  // "Company", "Subsidiary", etc.
        public string HierarchyPath { get; set; } = string.Empty;
        public DateTime EffectiveDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool IsEffective { get; set; }
        public bool IsActive { get; set; }
    }
}