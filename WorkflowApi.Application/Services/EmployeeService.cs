using WorkflowApi.Application.Interfaces;

namespace WorkflowApi.Application.Services
{
    public class EmployeeService : IEmployeeService
    {
        public Task<EmployeeDto?> GetByNIdAsync(string nId, CancellationToken cancellationToken = default)
        {
            // TODO: Call external API
            return Task.FromResult<EmployeeDto?>(new EmployeeDto
            {
                NId = nId,
                Name = "Mock Employee",
                Email = "mock@example.com"
            });
        }

        public Task<List<EmployeeDto>> GetByPositionAndOuAsync(int positionId, int ouId, CancellationToken cancellationToken = default)
        {
            // TODO: Call external API
            var employees = new List<EmployeeDto>
            {
                new() { NId = "E12345", Name = "John Doe", Email = "john.doe@example.com" },
                new() { NId = "E67890", Name = "Jane Smith", Email = "jane.smith@example.com" }
            };
            return Task.FromResult(employees);
        }

        public Task<int> GetEmployeeOuIdAsync(string nId, CancellationToken cancellationToken = default)
        {
            // TODO: Call external API
            return Task.FromResult(1); // Mock OU ID
        }
    }
}
