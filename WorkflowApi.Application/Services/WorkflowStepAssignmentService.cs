using AutoMapper;
using WorkflowApi.Application.DTOs.WorkflowStepAssignment;
using WorkflowApi.Application.Interfaces;
using WorkflowApi.Domain.Entities;
using WorkflowApi.Domain.Interfaces;

namespace WorkflowApi.Application.Services
{
    public class WorkflowStepAssignmentService(
        IWorkflowStepAssignmentRepository assignmentRepository,
        IWorkflowStepRepository stepRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper) : IWorkflowStepAssignmentService
    {
        private readonly IWorkflowStepAssignmentRepository _assignmentRepository = assignmentRepository;
        private readonly IWorkflowStepRepository _stepRepository = stepRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;

        public async Task<WorkflowStepAssignmentResponse> CreateAsync(
            CreateWorkflowStepAssignmentRequest request,
            CancellationToken cancellationToken = default)
        {
            // 1. Check if step exists
            var step = await _stepRepository.GetByIdAsync(request.StepId, cancellationToken) 
                ?? throw new KeyNotFoundException($"WorkflowStep with Id {request.StepId} not found");

            // 2. Map to Entity
            var assignment = _mapper.Map<WorkflowStepAssignment>(request);

            // 3. Validate business rules
            assignment.Validate();

            // 4. Save
            await _assignmentRepository.AddAsync(assignment, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 5. Return response
            return _mapper.Map<WorkflowStepAssignmentResponse>(assignment);
        }

        public async Task<WorkflowStepAssignmentResponse> UpdateAsync(
            int id,
            UpdateWorkflowStepAssignmentRequest request,
            CancellationToken cancellationToken = default)
        {
            // 1. Find existing
            var assignment = await _assignmentRepository.GetByIdAsync(id, cancellationToken);
            if (assignment == null)
            {
                throw new KeyNotFoundException($"WorkflowStepAssignment with Id {id} not found");
            }

            // 2. Update properties
            assignment.AssignmentType = request.AssignmentType;
            assignment.PositionId = request.PositionId;
            assignment.OUResolutionMode = request.OUResolutionMode;
            assignment.OrganizationalUnitId = request.OrganizationalUnitId;
            assignment.NId = request.NId;

            // 3. Validate business rules
            assignment.Validate();

            // 4. Save
            await _assignmentRepository.UpdateAsync(assignment, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 5. Return response
            return _mapper.Map<WorkflowStepAssignmentResponse>(assignment);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            // 1. Find existing
            var assignment = await _assignmentRepository.GetByIdAsync(id, cancellationToken) 
                ?? throw new KeyNotFoundException($"WorkflowStepAssignment with Id {id} not found");

            // 2. Delete
            await _assignmentRepository.DeleteAsync(assignment, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task<WorkflowStepAssignmentResponse?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            var assignment = await _assignmentRepository.GetByIdAsync(id, cancellationToken);
            return assignment == null ? null : _mapper.Map<WorkflowStepAssignmentResponse>(assignment);
        }

        public async Task<IEnumerable<WorkflowStepAssignmentResponse>> GetByStepIdAsync(
            int stepId,
            CancellationToken cancellationToken = default)
        {
            var assignments = await _assignmentRepository.GetAssignmentsByStepIdAsync(stepId, cancellationToken);
            return _mapper.Map<IEnumerable<WorkflowStepAssignmentResponse>>(assignments);
        }

        public async Task<IEnumerable<WorkflowStepAssignmentResponse>> GetPositionBasedAssignmentsAsync(
            int stepId,
            CancellationToken cancellationToken = default)
        {
            var assignments = await _assignmentRepository.GetPositionBasedAssignmentsAsync(stepId, cancellationToken);
            return _mapper.Map<IEnumerable<WorkflowStepAssignmentResponse>>(assignments);
        }

        public async Task<IEnumerable<WorkflowStepAssignmentResponse>> GetEmployeeBasedAssignmentsAsync(
            int stepId,
            CancellationToken cancellationToken = default)
        {
            var assignments = await _assignmentRepository.GetEmployeeBasedAssignmentsAsync(stepId, cancellationToken);
            return _mapper.Map<IEnumerable<WorkflowStepAssignmentResponse>>(assignments);
        }

        public async Task<WorkflowStepAssignmentResponse?> GetByNIdAsync(
            int stepId,
            string nId,
            CancellationToken cancellationToken = default)
        {
            var assignment = await _assignmentRepository.GetAssignmentByNIdAsync(stepId, nId, cancellationToken);
            return assignment == null ? null : _mapper.Map<WorkflowStepAssignmentResponse>(assignment);
        }
    }
}