using Microsoft.AspNetCore.Mvc;
using WorkflowApi.Application.DTOs.WorkflowRoute;
using WorkflowApi.Application.Interfaces;

namespace WorkflowApi.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkflowRoutesController(IWorkflowRouteService workflowRouteService) : ControllerBase
{
    private readonly IWorkflowRouteService _workflowRouteService = workflowRouteService;

    /// <summary>
    /// Get all workflow routes
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<WorkflowRouteResponse>), 200)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var routes = await _workflowRouteService.GetAllAsync(cancellationToken);
        return Ok(routes);
    }

    /// <summary>
    /// Get active workflow routes only
    /// </summary>
    [HttpGet("active")]
    [ProducesResponseType(typeof(IEnumerable<WorkflowRouteResponse>), 200)]
    public async Task<IActionResult> GetActiveRoutes(CancellationToken cancellationToken)
    {
        var routes = await _workflowRouteService.GetActiveRoutesAsync(cancellationToken);
        return Ok(routes);
    }

    /// <summary>
    /// Get workflow route by ID
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(WorkflowRouteResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var route = await _workflowRouteService.GetByIdAsync(id, cancellationToken);
        return Ok(route);
    }

    /// <summary>
    /// Get workflow route with complete details (including steps and assignments)
    /// </summary>
    [HttpGet("{id:int}/detail")]
    [ProducesResponseType(typeof(WorkflowRouteDetailResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetDetailById(int id, CancellationToken cancellationToken)
    {
        var route = await _workflowRouteService.GetDetailByIdAsync(id, cancellationToken);
        return Ok(route);
    }

    /// <summary>
    /// Get workflow route by document type
    /// </summary>
    [HttpGet("by-document-type/{documentType}")]
    [ProducesResponseType(typeof(WorkflowRouteResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetByDocumentType(
        string documentType, 
        CancellationToken cancellationToken)
    {
        var route = await _workflowRouteService.GetByDocumentTypeAsync(documentType, cancellationToken);
        return Ok(route);
    }

    /// <summary>
    /// Create new workflow route
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(WorkflowRouteResponse), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(409)]
    public async Task<IActionResult> Create(
        [FromBody] CreateWorkflowRouteRequest request,
        CancellationToken cancellationToken)
    {
        var route = await _workflowRouteService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = route.Id }, route);
    }

    /// <summary>
    /// Update workflow route
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(WorkflowRouteResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(409)]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateWorkflowRouteRequest request,
        CancellationToken cancellationToken)
    {
        var route = await _workflowRouteService.UpdateAsync(id, request, cancellationToken);
        return Ok(route);
    }

    /// <summary>
    /// Delete workflow route (soft delete)
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _workflowRouteService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
