using Microsoft.AspNetCore.Mvc;
using WorkflowApi.Application.DTOs.WorkflowStepAssignment;
using WorkflowApi.Application.Interfaces;

namespace WorkflowApi.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkflowStepAssignmentsController(
    IWorkflowStepAssignmentService assignmentService) : ControllerBase
{
    private readonly IWorkflowStepAssignmentService _assignmentService = assignmentService;

    /// <summary>
    /// Get assignment by ID
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(WorkflowStepAssignmentResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var assignment = await _assignmentService.GetByIdAsync(id, cancellationToken);
        return Ok(assignment);
    }

    /// <summary>
    /// Get all assignments for a specific step
    /// </summary>
    [HttpGet("by-step/{stepId:int}")]
    [ProducesResponseType(typeof(IEnumerable<WorkflowStepAssignmentResponse>), 200)]
    public async Task<IActionResult> GetByStepId(int stepId, CancellationToken cancellationToken)
    {
        var assignments = await _assignmentService.GetByStepIdAsync(stepId, cancellationToken);
        return Ok(assignments);
    }

    /// <summary>
    /// Get position-based assignments for a step
    /// </summary>
    [HttpGet("position-based")]
    [ProducesResponseType(typeof(IEnumerable<WorkflowStepAssignmentResponse>), 200)]
    public async Task<IActionResult> GetPositionBasedAssignments(
        [FromQuery] int stepId,
        CancellationToken cancellationToken)
    {
        var assignments = await _assignmentService.GetPositionBasedAssignmentsAsync(
            stepId, 
            cancellationToken);
        return Ok(assignments);
    }

    /// <summary>
    /// Get employee-based assignments for a step
    /// </summary>
    [HttpGet("employee-based")]
    [ProducesResponseType(typeof(IEnumerable<WorkflowStepAssignmentResponse>), 200)]
    public async Task<IActionResult> GetEmployeeBasedAssignments(
        [FromQuery] int stepId,
        CancellationToken cancellationToken)
    {
        var assignments = await _assignmentService.GetEmployeeBasedAssignmentsAsync(
            stepId, 
            cancellationToken);
        return Ok(assignments);
    }

    /// <summary>
    /// Get assignment by NId (employee ID)
    /// </summary>
    [HttpGet("by-nid")]
    [ProducesResponseType(typeof(WorkflowStepAssignmentResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetByNId(
        [FromQuery] int stepId,
        [FromQuery] string nId,
        CancellationToken cancellationToken)
    {
        var assignment = await _assignmentService.GetByNIdAsync(stepId, nId, cancellationToken);
        return Ok(assignment);
    }

    /// <summary>
    /// Create new assignment
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(WorkflowStepAssignmentResponse), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Create(
        [FromBody] CreateWorkflowStepAssignmentRequest request,
        CancellationToken cancellationToken)
    {
        var assignment = await _assignmentService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = assignment.Id }, assignment);
    }

    /// <summary>
    /// Update assignment
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(WorkflowStepAssignmentResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateWorkflowStepAssignmentRequest request,
        CancellationToken cancellationToken)
    {
        var assignment = await _assignmentService.UpdateAsync(id, request, cancellationToken);
        return Ok(assignment);
    }

    /// <summary>
    /// Delete assignment
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _assignmentService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
