using WorkflowApi.Application.DTOs.Condition;
using WorkflowApi.Application.Interfaces;
using WorkflowApi.Domain.Entities;
using WorkflowApi.Domain.Enums;
using WorkflowApi.Domain.Interfaces;

namespace WorkflowApi.Application.Services
{
    public class DelegationResolverService(IUnitOfWork unitOfWork) : IDelegationResolver
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<string> ResolveAsync(
            string nId,
            int routeId,
            string documentType,
            int stepId,
            CancellationToken cancellationToken = default)
        {
            var result = await ResolveWithDetailsAsync(
                nId, 
                routeId, 
                documentType, 
                stepId, 
                cancellationToken);
            
            return result.FinalNId;
        }

        public async Task<DelegationResult> ResolveWithDetailsAsync(
            string nId,
            int routeId,
            string documentType,
            int stepId,
            CancellationToken cancellationToken = default)
        {
            var visited = new HashSet<string>();
            var delegationChain = new List<string> { nId };
            var currentNId = nId;
            string? delegationReason = null;

            while (visited.Add(currentNId))
            {
                var delegations = await _unitOfWork.WorkflowDelegations.GetActiveDelegationsAsync(
                    currentNId,
                    DateTime.Now,
                    cancellationToken);

                var matchedDelegation = FindMatchingDelegation(
                    delegations, 
                    routeId, 
                    documentType, 
                    stepId);

                if (matchedDelegation == null)
                    break;

                currentNId = matchedDelegation.Delegatee;
                delegationReason = matchedDelegation.Reason;
                delegationChain.Add(currentNId);
            }

            return new DelegationResult
            {
                OriginalNId = nId,
                FinalNId = currentNId,
                IsDelegated = currentNId != nId,
                DelegationReason = delegationReason,
                DelegationChain = delegationChain
            };
        }

        private static WorkflowDelegation? FindMatchingDelegation(
            List<WorkflowDelegation> delegations,
            int routeId,
            string documentType,
            int stepId)
        {
            // Priority 1: Specific Step
            var stepDelegation = delegations
                .FirstOrDefault(d => d.Scope == DelegationScope.SpecificStep && d.StepId == stepId);
            if (stepDelegation != null) return stepDelegation;

            // Priority 2: Specific Route
            var routeDelegation = delegations
                .FirstOrDefault(d => d.Scope == DelegationScope.SpecificRoute && d.RouteId == routeId);
            if (routeDelegation != null) return routeDelegation;

            // Priority 3: Specific Document Type
            var docTypeDelegation = delegations
                .FirstOrDefault(d => d.Scope == DelegationScope.SpecificDocType && d.DocumentType == documentType);
            if (docTypeDelegation != null) return docTypeDelegation;

            // Priority 4: All
            var allDelegation = delegations
                .FirstOrDefault(d => d.Scope == DelegationScope.All);

            return allDelegation;
        }
    }
}