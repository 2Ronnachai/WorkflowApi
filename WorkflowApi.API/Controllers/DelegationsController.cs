using Microsoft.AspNetCore.Mvc;
using WorkflowApi.Application.DTOs.WorkflowDelegation;
using WorkflowApi.Application.Interfaces;

namespace WorkflowApi.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DelegationsController(IWorkflowDelegationService service) : ControllerBase
    {
        private readonly IWorkflowDelegationService _service = service;

        [HttpPost("simple")]
        public async Task<ActionResult<DelegationResponse>> CreateSimple(
            [FromBody] SimpleDelegationRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _service.CreateSimpleAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPost("advanced")]
        public async Task<ActionResult<DelegationResponse>> CreateAdvanced(
            [FromBody] CreateDelegationRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _service.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<DelegationResponse>> Update(
            int id,
            [FromBody] UpdateDelegationRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _service.UpdateAsync(id, request, cancellationToken);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DelegationResponse>> GetById(
            int id,
            CancellationToken cancellationToken)
        {
            var result = await _service.GetByIdAsync(id, cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpGet("delegator/{delegatorNId}")]
        public async Task<ActionResult<List<DelegationResponse>>> GetByDelegator(
            string delegatorNId,
            CancellationToken cancellationToken)
        {
            var result = await _service.GetByDelegatorAsync(delegatorNId, cancellationToken);
            return Ok(result);
        }

        [HttpGet("delegate/{delegateNId}")]
        public async Task<ActionResult<List<DelegationResponse>>> GetByDelegate(
            string delegateNId,
            CancellationToken cancellationToken)
        {
            var result = await _service.GetByDelegateAsync(delegateNId, cancellationToken);
            return Ok(result);
        }

        [HttpGet("active/{delegatorNId}")]
        public async Task<ActionResult<string>> GetActiveDelegate(
            string delegatorNId,
            [FromQuery] int? routeId = null,
            [FromQuery] string? documentType = null,
            [FromQuery] int? stepId = null,
            CancellationToken cancellationToken = default)
        {
            var result = await _service.GetActiveDelegateAsync(
                delegatorNId, routeId, documentType, stepId, cancellationToken);
            
            return result == null ? NotFound() : Ok(new { DelegateNId = result });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
    }
}