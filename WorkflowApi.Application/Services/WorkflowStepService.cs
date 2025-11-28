using AutoMapper;
using System.Text.Json;
using WorkflowApi.Application.DTOs.WorkflowStep;
using WorkflowApi.Application.Exceptions;
using WorkflowApi.Application.Interfaces;
using WorkflowApi.Domain.Entities;
using WorkflowApi.Domain.Interfaces;

namespace WorkflowApi.Application.Services;

public class WorkflowStepService(
    IUnitOfWork unitOfWork,
    IMapper mapper) : IWorkflowStepService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;

    public async Task<WorkflowStepResponse> CreateAsync(
        CreateWorkflowStepRequest request,
        CancellationToken cancellationToken = default)
    {
        // 1. Check if route exists
        var route = await _unitOfWork.WorkflowRoutes.GetByIdAsync(request.RouteId, cancellationToken) 
            ?? throw new NotFoundException(nameof(WorkflowRoute), request.RouteId);

        // 2. Check duplicate sequence number
        var exists = await _unitOfWork.WorkflowSteps.IsSequenceNoExistsAsync(
            request.RouteId,
            request.SequenceNo,
            cancellationToken: cancellationToken);

        if (exists)
        {
            throw new InvalidOperationException(
                $"SequenceNo {request.SequenceNo} already exists in Route {request.RouteId}");
        }

        // 3. Map to Entity
        var step = _mapper.Map<WorkflowStep>(request);

        // 4. Validate business rules
        step.ValidateReturnBehavior();
        step.ValidateParallelGroup();

        // 5. Save
        await _unitOfWork.WorkflowSteps.AddAsync(step, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 6. Return response
        return _mapper.Map<WorkflowStepResponse>(step);
    }

    public async Task<WorkflowStepResponse> UpdateAsync(
        int id,
        UpdateWorkflowStepRequest request,
        CancellationToken cancellationToken = default)
    {
        // 1. Find existing
        var step = await _unitOfWork.WorkflowSteps.GetByIdAsync(id, cancellationToken) 
            ?? throw new KeyNotFoundException($"WorkflowStep with Id {id} not found");

        // 2. Update properties
        step.StepName = request.StepName;
        step.ParallelGroupId = request.ParallelGroupId;
        step.ExecutionMode = request.ExecutionMode;
        step.CompletionRule = request.CompletionRule;
        step.AllowReturn = request.AllowReturn;
        step.ReturnBehavior = request.ReturnBehavior;
        step.ReturnStepId = request.ReturnStepId;
        step.IsFinalStep = request.IsFinalStep;
        
        // Serialize NextStepCondition
        step.NextStepCondition = request.NextStepCondition != null
            ? JsonSerializer.Serialize(request.NextStepCondition)
            : null;

        // 3. Validate business rules
        step.ValidateReturnBehavior();
        step.ValidateParallelGroup();

        // 4. Save
        await _unitOfWork.WorkflowSteps.UpdateAsync(step, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 5. Return response
        return _mapper.Map<WorkflowStepResponse>(step);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        // 1. Find existing
        var step = await _unitOfWork.WorkflowSteps.GetByIdAsync(id, cancellationToken);
        if (step == null)
        {
            throw new KeyNotFoundException($"WorkflowStep with Id {id} not found");
        }

        // 2. Check if it's referenced by other steps (ReturnStepId)
        var referencingSteps = await _unitOfWork.WorkflowSteps.FindAsync(
            s => s.ReturnStepId == id,
            cancellationToken);

        if (referencingSteps.Any())
        {
            throw new InvalidOperationException(
                $"Cannot delete step {id} because it's referenced by other steps");
        }

        // 3. Delete
        await _unitOfWork.WorkflowSteps.DeleteAsync(step, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<WorkflowStepResponse?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var step = await _unitOfWork.WorkflowSteps.GetByIdAsync(id, cancellationToken);
        return step == null ? null : _mapper.Map<WorkflowStepResponse>(step);
    }

    public async Task<WorkflowStepResponse?> GetWithAssignmentsAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var step = await _unitOfWork.WorkflowSteps.GetStepWithAssignmentsAsync(id, cancellationToken);
        return step == null ? null : _mapper.Map<WorkflowStepResponse>(step);
    }

    public async Task<IEnumerable<WorkflowStepResponse>> GetByRouteIdAsync(
        int routeId,
        CancellationToken cancellationToken = default)
    {
        var steps = await _unitOfWork.WorkflowSteps.GetStepsByRouteIdAsync(routeId, cancellationToken);
        return _mapper.Map<IEnumerable<WorkflowStepResponse>>(steps);
    }

    public async Task<IEnumerable<WorkflowStepResponse>> GetParallelStepsAsync(
        int routeId,
        int parallelGroupId,
        CancellationToken cancellationToken = default)
    {
        var steps = await _unitOfWork.WorkflowSteps.GetParallelStepsAsync(routeId, parallelGroupId, cancellationToken);
        return _mapper.Map<IEnumerable<WorkflowStepResponse>>(steps);
    }

    public async Task<WorkflowStepResponse?> GetNextStepAsync(
        int routeId,
        int currentSequenceNo,
        CancellationToken cancellationToken = default)
    {
        var step = await _unitOfWork.WorkflowSteps.GetNextStepAsync(routeId, currentSequenceNo, cancellationToken);
        return step == null ? null : _mapper.Map<WorkflowStepResponse>(step);
    }

    public async Task<WorkflowStepResponse?> GetFinalStepAsync(
        int routeId,
        CancellationToken cancellationToken = default)
    {
        var step = await _unitOfWork.WorkflowSteps.GetFinalStepAsync(routeId, cancellationToken);
        return step == null ? null : _mapper.Map<WorkflowStepResponse>(step);
    }
}
