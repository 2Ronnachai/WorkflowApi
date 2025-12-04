using WorkflowApi.Application.DTOs.ExternalResponse;

namespace WorkflowApi.Application.Interfaces
{
    public interface IEmployeeService
    {
        Task<EmployeeDto?> GetByNIdAsync(string nId, CancellationToken cancellationToken = default);
        Task<List<EmployeeDto>> GetByPositionAndOuAsync(int positionId, int ouId, CancellationToken cancellationToken = default);
        Task<List<int>> GetEmployeeOuIdsAsync(string nId, CancellationToken cancellationToken = default);
    }   
}