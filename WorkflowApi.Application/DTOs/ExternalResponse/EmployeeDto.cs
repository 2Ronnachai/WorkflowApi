namespace WorkflowApi.Application.DTOs.ExternalResponse
{
     public class EmployeeDto
    {
        public string NId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}";
    }
}