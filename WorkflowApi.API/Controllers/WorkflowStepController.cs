using Microsoft.AspNetCore.Mvc;
using WorkflowApi.Application.DTOs.WorkflowStep;
using WorkflowApi.Application.Interfaces;

namespace WorkflowApi.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkflowStepController(IWorkflowStepService workflowStepService) : ControllerBase
{
    private readonly IWorkflowStepService _workflowStepService = workflowStepService;

    /// <summary>
    /// Get workflow step by ID
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(WorkflowStepResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var step = await _workflowStepService.GetByIdAsync(id, cancellationToken);
        return Ok(step);
    }

    /// <summary>
    /// Get workflow step with assignments
    /// </summary>
    [HttpGet("{id:int}/with-assignments")]
    [ProducesResponseType(typeof(WorkflowStepResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetWithAssignments(int id, CancellationToken cancellationToken)
    {
        var step = await _workflowStepService.GetWithAssignmentsAsync(id, cancellationToken);
        return Ok(step);
    }

    /// <summary>
    /// Get all steps for a specific route
    /// </summary>
    [HttpGet("by-route/{routeId:int}")]
    [ProducesResponseType(typeof(IEnumerable<WorkflowStepResponse>), 200)]
    public async Task<IActionResult> GetByRouteId(int routeId, CancellationToken cancellationToken)
    {
        var steps = await _workflowStepService.GetByRouteIdAsync(routeId, cancellationToken);
        return Ok(steps);
    }

    /// <summary>
    /// Get parallel steps in a route
    /// </summary>
    [HttpGet("parallel")]
    [ProducesResponseType(typeof(IEnumerable<WorkflowStepResponse>), 200)]
    public async Task<IActionResult> GetParallelSteps(
        [FromQuery] int routeId,
        [FromQuery] int parallelGroupId,
        CancellationToken cancellationToken)
    {
        var steps = await _workflowStepService.GetParallelStepsAsync(
            routeId, 
            parallelGroupId, 
            cancellationToken);
        return Ok(steps);
    }

    /// <summary>
    /// Get next step based on current sequence number
    /// </summary>
    [HttpGet("next")]
    [ProducesResponseType(typeof(WorkflowStepResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetNextStep(
        [FromQuery] int routeId,
        [FromQuery] int currentSequenceNo,
        CancellationToken cancellationToken)
    {
        var step = await _workflowStepService.GetNextStepAsync(
            routeId, 
            currentSequenceNo, 
            cancellationToken);
        return Ok(step);
    }

    /// <summary>
    /// Get final step of a route
    /// </summary>
    [HttpGet("final")]
    [ProducesResponseType(typeof(WorkflowStepResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetFinalStep(
        [FromQuery] int routeId,
        CancellationToken cancellationToken)
    {
        var step = await _workflowStepService.GetFinalStepAsync(routeId, cancellationToken);
        return Ok(step);
    }

    /// <summary>
    /// Create new workflow step
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(WorkflowStepResponse), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(409)]
    public async Task<IActionResult> Create(
        [FromBody] CreateWorkflowStepRequest request,
        CancellationToken cancellationToken)
    {
        var step = await _workflowStepService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = step.Id }, step);
    }

    /// <summary>
    /// Update workflow step
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(WorkflowStepResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateWorkflowStepRequest request,
        CancellationToken cancellationToken)
    {
        var step = await _workflowStepService.UpdateAsync(id, request, cancellationToken);
        return Ok(step);
    }

    /// <summary>
    /// Delete workflow step
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _workflowStepService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
