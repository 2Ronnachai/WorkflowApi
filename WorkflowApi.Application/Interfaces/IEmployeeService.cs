namespace WorkflowApi.Application.Interfaces
{
    public interface IEmployeeService
    {
        Task<EmployeeDto?> GetByNIdAsync(string nId, CancellationToken cancellationToken = default);
        Task<List<EmployeeDto>> GetByPositionAndOuAsync(int positionId, int ouId, CancellationToken cancellationToken = default);
        Task<int> GetEmployeeOuIdAsync(string nId, CancellationToken cancellationToken = default);
    }

    public class EmployeeDto
    {
        public string NId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }    
}