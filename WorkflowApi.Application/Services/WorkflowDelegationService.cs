using AutoMapper;
using WorkflowApi.Application.DTOs.WorkflowDelegation;
using WorkflowApi.Application.Interfaces;
using WorkflowApi.Domain.Entities;
using WorkflowApi.Domain.Enums;
using WorkflowApi.Domain.Interfaces;

namespace WorkflowApi.Application.Services
{
    public class WorkflowDelegationService(
        IUnitOfWork unitOfWork,
        IMapper mapper) : IWorkflowDelegationService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;

        public async Task<DelegationResponse> CreateSimpleAsync(
            SimpleDelegationRequest request,
            CancellationToken cancellationToken = default)
        {
            var delegation = new WorkflowDelegation
            {
                Delegator = request.Delegator,
                Delegatee = request.Delegatee,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Scope = DelegationScope.All,
                Reason = request.Reason,
                IsActive = true
            };

            var created = await _unitOfWork.WorkflowDelegations.AddAsync(delegation, cancellationToken);
            return _mapper.Map<DelegationResponse>(created);
        }

        public async Task<DelegationResponse> CreateAsync(
            CreateDelegationRequest request,
            CancellationToken cancellationToken = default)
        {
            var delegation = new WorkflowDelegation
            {
                Delegator = request.Delegator,
                Delegatee = request.Delegatee,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Scope = (DelegationScope)request.Scope,
                RouteId = request.RouteId,
                DocumentType = request.DocumentType,
                StepId = request.StepId,
                Reason = request.Reason,
                IsActive = true
            };

            var created = await _unitOfWork.WorkflowDelegations.AddAsync(delegation, cancellationToken);
            return _mapper.Map<DelegationResponse>(created);
        }

        public async Task<DelegationResponse> UpdateAsync(
            int id,
            UpdateDelegationRequest request,
            CancellationToken cancellationToken = default)
        {
            var delegation = await _unitOfWork.WorkflowDelegations.GetByIdAsync(id, cancellationToken)
                ?? throw new KeyNotFoundException($"Delegation with Id {id} not found");

            delegation.Delegatee = request.Delegatee;
            delegation.StartDate = request.StartDate;
            delegation.EndDate = request.EndDate;
            delegation.Scope = (DelegationScope)request.Scope;
            delegation.RouteId = request.RouteId;
            delegation.DocumentType = request.DocumentType;
            delegation.StepId = request.StepId;
            delegation.Reason = request.Reason;
            delegation.IsActive = request.IsActive;

            await _unitOfWork.WorkflowDelegations.UpdateAsync(delegation, cancellationToken);
            return _mapper.Map<DelegationResponse>(delegation);
        }

        public async Task<DelegationResponse?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            var delegation = await _unitOfWork.WorkflowDelegations.GetByIdAsync(id, cancellationToken);
            return delegation == null ? null : _mapper.Map<DelegationResponse>(delegation);
        }

        public async Task<List<DelegationResponse>> GetByDelegatorAsync(
            string delegatorNId,
            CancellationToken cancellationToken = default)
        {
            var delegations = await _unitOfWork.WorkflowDelegations.GetByDelegatorAsync(delegatorNId, cancellationToken);
            return _mapper.Map<List<DelegationResponse>>(delegations);
        }

        public async Task<List<DelegationResponse>> GetByDelegateAsync(
            string delegateNId,
            CancellationToken cancellationToken = default)
        {
            var delegations = await _unitOfWork.WorkflowDelegations.GetByDelegateAsync(delegateNId, cancellationToken);
            return _mapper.Map<List<DelegationResponse>>(delegations);
        }

        public async Task<string?> GetActiveDelegateAsync(
            string delegatorNId,
            int? routeId = null,
            string? documentType = null,
            int? stepId = null,
            CancellationToken cancellationToken = default)
        {
            var now = DateTime.Now;
            WorkflowDelegation? delegation = null;

            // Priority: Step > Route > DocumentType > All
            if (stepId.HasValue)
            {
                delegation = await _unitOfWork.WorkflowDelegations.GetActiveDelegationForStepAsync(
                    delegatorNId, stepId.Value, now, cancellationToken);
            }

            if (delegation == null && routeId.HasValue)
            {
                delegation = await _unitOfWork.WorkflowDelegations.GetActiveDelegationForRouteAsync(
                    delegatorNId, routeId.Value, now, cancellationToken);
            }

            if (delegation == null && !string.IsNullOrEmpty(documentType))
            {
                delegation = await _unitOfWork.WorkflowDelegations.GetActiveDelegationForDocumentTypeAsync(
                    delegatorNId, documentType, now, cancellationToken);
            }

            if (delegation == null)
            {
                delegation = await _unitOfWork.WorkflowDelegations.GetActiveDelegationAsync(
                    delegatorNId, now, cancellationToken);
            }

            return delegation?.Delegatee;
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var delegation = await _unitOfWork.WorkflowDelegations.GetByIdAsync(id, cancellationToken)
                ?? throw new KeyNotFoundException($"Delegation with Id {id} not found");

            await _unitOfWork.WorkflowDelegations.DeleteAsync(delegation, cancellationToken);
        }
    }
}