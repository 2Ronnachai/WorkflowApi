// WorkflowStepService.cs
using AutoMapper;
using System.Text.Json;
using WorkflowApi.Application.DTOs.WorkflowStep;
using WorkflowApi.Application.Interfaces;
using WorkflowApi.Domain.Entities;
using WorkflowApi.Domain.Interfaces;

namespace WorkflowApi.Application.Services;

public class WorkflowStepService(
    IWorkflowStepRepository stepRepository,
    IWorkflowRouteRepository routeRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper) : IWorkflowStepService
{
    private readonly IWorkflowStepRepository _stepRepository = stepRepository;
    private readonly IWorkflowRouteRepository _routeRepository = routeRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;

    public async Task<WorkflowStepResponse> CreateAsync(
        CreateWorkflowStepRequest request,
        CancellationToken cancellationToken = default)
    {
        // 1. Check if route exists
        var route = await _routeRepository.GetByIdAsync(request.RouteId, cancellationToken) 
            ?? throw new KeyNotFoundException($"WorkflowRoute with Id {request.RouteId} not found");

        // 2. Check duplicate sequence number
        var exists = await _stepRepository.IsSequenceNoExistsAsync(
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
        await _stepRepository.AddAsync(step, cancellationToken);
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
        var step = await _stepRepository.GetByIdAsync(id, cancellationToken) 
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
        await _stepRepository.UpdateAsync(step, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 5. Return response
        return _mapper.Map<WorkflowStepResponse>(step);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        // 1. Find existing
        var step = await _stepRepository.GetByIdAsync(id, cancellationToken);
        if (step == null)
        {
            throw new KeyNotFoundException($"WorkflowStep with Id {id} not found");
        }

        // 2. Check if it's referenced by other steps (ReturnStepId)
        var referencingSteps = await _stepRepository.FindAsync(
            s => s.ReturnStepId == id,
            cancellationToken);

        if (referencingSteps.Any())
        {
            throw new InvalidOperationException(
                $"Cannot delete step {id} because it's referenced by other steps");
        }

        // 3. Delete
        await _stepRepository.DeleteAsync(step, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<WorkflowStepResponse?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var step = await _stepRepository.GetByIdAsync(id, cancellationToken);
        return step == null ? null : _mapper.Map<WorkflowStepResponse>(step);
    }

    public async Task<WorkflowStepResponse?> GetWithAssignmentsAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var step = await _stepRepository.GetStepWithAssignmentsAsync(id, cancellationToken);
        return step == null ? null : _mapper.Map<WorkflowStepResponse>(step);
    }

    public async Task<IEnumerable<WorkflowStepResponse>> GetByRouteIdAsync(
        int routeId,
        CancellationToken cancellationToken = default)
    {
        var steps = await _stepRepository.GetStepsByRouteIdAsync(routeId, cancellationToken);
        return _mapper.Map<IEnumerable<WorkflowStepResponse>>(steps);
    }

    public async Task<IEnumerable<WorkflowStepResponse>> GetParallelStepsAsync(
        int routeId,
        int parallelGroupId,
        CancellationToken cancellationToken = default)
    {
        var steps = await _stepRepository.GetParallelStepsAsync(routeId, parallelGroupId, cancellationToken);
        return _mapper.Map<IEnumerable<WorkflowStepResponse>>(steps);
    }

    public async Task<WorkflowStepResponse?> GetNextStepAsync(
        int routeId,
        int currentSequenceNo,
        CancellationToken cancellationToken = default)
    {
        var step = await _stepRepository.GetNextStepAsync(routeId, currentSequenceNo, cancellationToken);
        return step == null ? null : _mapper.Map<WorkflowStepResponse>(step);
    }

    public async Task<WorkflowStepResponse?> GetFinalStepAsync(
        int routeId,
        CancellationToken cancellationToken = default)
    {
        var step = await _stepRepository.GetFinalStepAsync(routeId, cancellationToken);
        return step == null ? null : _mapper.Map<WorkflowStepResponse>(step);
    }
}
