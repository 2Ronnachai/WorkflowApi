using AutoMapper;
using WorkflowApi.Application.DTOs.WorkflowRoute;
using WorkflowApi.Application.Exceptions;
using WorkflowApi.Application.Interfaces;
using WorkflowApi.Domain.Entities;
using WorkflowApi.Domain.Interfaces;

namespace WorkflowApi.Application.Services;

public class WorkflowRouteService(
    IUnitOfWork unitOfWork,
    IMapper mapper) : IWorkflowRouteService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;

    public async Task<WorkflowRouteResponse> CreateAsync(
        CreateWorkflowRouteRequest request, 
        CancellationToken cancellationToken = default)
    {
        // 1. Check duplicate
        var exists = await _unitOfWork.WorkflowRoutes.IsRouteNameExistsAsync(
            request.RouteName, 
            request.DocumentType, 
            cancellationToken: cancellationToken);
        
        if (exists)
        {
            throw new DuplicateException(
                $"Route '{request.RouteName}' for DocumentType '{request.DocumentType}' already exists");
        }

        // 2. Map to Entity
        var route = _mapper.Map<WorkflowRoute>(request);

        // 3. Save
        await _unitOfWork.WorkflowRoutes.AddAsync(route, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 4. Return response
        return _mapper.Map<WorkflowRouteResponse>(route);
    }

    public async Task<WorkflowRouteResponse> UpdateAsync(
        int id, 
        UpdateWorkflowRouteRequest request, 
        CancellationToken cancellationToken = default)
    {
        // 1. Find existing
        var route = await _unitOfWork.WorkflowRoutes.GetByIdAsync(id, cancellationToken) 
            ?? throw new NotFoundException(nameof(WorkflowRoute), id);

        // 2. Check duplicate (exclude current)
        var exists = await _unitOfWork.WorkflowRoutes.IsRouteNameExistsAsync(
            request.RouteName,
            route.DocumentType,
            excludeId: id,
            cancellationToken: cancellationToken);

        if (exists)
        {
            throw new InvalidOperationException(
                $"Route '{request.RouteName}' already exists");
        }

        // 3. Update properties
        route.RouteName = request.RouteName;
        route.DocumentType = request.DocumentType;
        route.Description = request.Description;
        route.IsActive = request.IsActive;

        // 4. Save
        await _unitOfWork.WorkflowRoutes.UpdateAsync(route, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 5. Return response
        return _mapper.Map<WorkflowRouteResponse>(route);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        // 1. Find existing
        var route = await _unitOfWork.WorkflowRoutes.GetByIdAsync(id, cancellationToken) 
            ?? throw new KeyNotFoundException($"WorkflowRoute with Id {id} not found");

        // 2. Soft delete
        await _unitOfWork.WorkflowRoutes.DeleteAsync(route, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<WorkflowRouteResponse?> GetByIdAsync(
        int id, 
        CancellationToken cancellationToken = default)
    {
        var route = await _unitOfWork.WorkflowRoutes.GetByIdAsync(id, cancellationToken);
        return route == null ? null : _mapper.Map<WorkflowRouteResponse>(route);
    }

    public async Task<WorkflowRouteDetailResponse?> GetDetailByIdAsync(
        int id, 
        CancellationToken cancellationToken = default)
    {
        var route = await _unitOfWork.WorkflowRoutes.GetCompleteRouteAsync(id, cancellationToken);
        return route == null ? null : _mapper.Map<WorkflowRouteDetailResponse>(route);
    }

    public async Task<WorkflowRouteResponse?> GetByDocumentTypeAsync(
        string documentType, 
        CancellationToken cancellationToken = default)
    {
        var route = await _unitOfWork.WorkflowRoutes.GetByDocumentTypeAsync(documentType, cancellationToken);
        return route == null ? null : _mapper.Map<WorkflowRouteResponse>(route);
    }

    public async Task<IEnumerable<WorkflowRouteResponse>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var routes = await _unitOfWork.WorkflowRoutes.GetAllAsync(cancellationToken);
        return _mapper.Map<IEnumerable<WorkflowRouteResponse>>(routes);
    }

    public async Task<IEnumerable<WorkflowRouteResponse>> GetActiveRoutesAsync(
        CancellationToken cancellationToken = default)
    {
        var routes = await _unitOfWork.WorkflowRoutes.GetActiveRoutesAsync(cancellationToken);
        return _mapper.Map<IEnumerable<WorkflowRouteResponse>>(routes);
    }
}
