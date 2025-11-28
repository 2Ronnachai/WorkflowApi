using WorkflowApi.Application.DTOs.Condition;
using WorkflowApi.Application.DTOs.Resolution;
using WorkflowApi.Application.Interfaces;
using WorkflowApi.Domain.Entities;
using WorkflowApi.Domain.Interfaces;

namespace WorkflowApi.Application.Services
{
    public static class ResolutionConstants
    {
        // Resolution Method
        public const string DirectAssignment = "DirectAssignment";
        
        // Condition Messages
        public const string NoConditionDefined = "No condition defined";
        public const string InvalidConditionFormat = "Invalid condition format";
        public const string NoConditionMet = "No condition met, using default next step";

        // Error Messages
        public const string RouteNotFoundById = "No route found for RouteId '{0}'";
        public const string RouteNotFoundByDocType = "No active workflow route found for DocumentType '{0}'";
    }

    public class WorkflowResolutionService(
        IUnitOfWork unitOfWork,
        IConditionEvaluator conditionEvaluator,
        IDelegationResolver delegationResolver,
        IAssignmentResolver assignmentResolver) : IWorkflowResolutionService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IConditionEvaluator _conditionEvaluator = conditionEvaluator;
        private readonly IDelegationResolver _delegationResolver = delegationResolver;
        private readonly IAssignmentResolver _assignmentResolver = assignmentResolver;
        public async Task<ResolvedWorkflowResponse> ResolveWorkflowAsync(
            ResolveWorkflowRequest request,
            CancellationToken cancellationToken = default)
        {
            // 1. Find Route by RouteId
            var route = await GetRouteAsync(request, cancellationToken);

            // 2. Prepare Response
            var response = new ResolvedWorkflowResponse
            {
                RouteId = route.Id,
                RouteName = route.RouteName,
                DocumentType = route.DocumentType,
                Steps = []
            };

            // 3. Resolve each Step
            foreach (var step in route.Steps.OrderBy(s => s.SequenceNo))
            {
                var resolvedStep = await ResolveStepAsync(step, request, cancellationToken);
                response.Steps.Add(resolvedStep);
            }

            return response;
        }

        private async Task<WorkflowRoute> GetRouteAsync(ResolveWorkflowRequest request , CancellationToken cancellationToken)
        {
            WorkflowRoute? route = null;
            string errorMessage;

            if (request.RouteId > 0)
            {
                route = await _unitOfWork.WorkflowRoutes
                    .GetCompleteRouteAsync(request.RouteId, cancellationToken);
                
                errorMessage = string.Format(
                    ResolutionConstants.RouteNotFoundById, 
                    request.RouteId);
            }
            else
            {
                route = await _unitOfWork.WorkflowRoutes
                    .GetCompleteRouteByDocumentTypeAsync(request.DocumentType, cancellationToken);
                
                errorMessage = string.Format(
                    ResolutionConstants.RouteNotFoundByDocType, 
                    request.DocumentType);
            }

            if (route == null)
            {
                throw new KeyNotFoundException(errorMessage);
            }

            return route;
        }

        private async Task<ResolvedWorkflowStepResponse> ResolveStepAsync(
            WorkflowStep step,
            ResolveWorkflowRequest request,
            CancellationToken cancellationToken)
        {
            // 1. Map step to response
            var resolvedStep = MapToResolvedStep(step);

            // 2. Resolve Assignees
            foreach (var assignment in step.Assignments)
            {
                var assignees = await _assignmentResolver.ResolveAsync(
                    assignment,
                    request,
                    cancellationToken);
                
                resolvedStep.Assignees.AddRange(assignees);
            }

            // 3. Apply conditional routing
            ApplyConditionalRouting(resolvedStep, step, request);

            return resolvedStep;
        }

        private static ResolvedWorkflowStepResponse MapToResolvedStep(WorkflowStep step)
        {
            return new ResolvedWorkflowStepResponse
            {
                StepId = step.Id,
                SequenceNo = step.SequenceNo,
                StepName = step.StepName,
                ParallelGroupId = step.ParallelGroupId,
                ExecutionMode = step.ExecutionMode.ToString(),
                CompletionRule = step.CompletionRule.ToString(),
                AllowReturn = step.AllowReturn,
                ReturnBehavior = step.ReturnBehavior.ToString(),
                ReturnStepId = step.ReturnStepId,
                IsFinalStep = step.IsFinalStep,
                Assignees = []
            };
        }

        private void ApplyConditionalRouting(
            ResolvedWorkflowStepResponse resolvedStep,
            WorkflowStep step,
            ResolveWorkflowRequest request)
        {
            if (string.IsNullOrEmpty(step.NextStepCondition) || request.ConditionalData == null)
            {
                return;
            }

            var conditionResult = _conditionEvaluator.Evaluate(
                step.NextStepCondition,
                request.ConditionalData);

            resolvedStep.NextStepId = conditionResult.NextStepId;
            resolvedStep.NextStepConditionResult = conditionResult.EvaluationDetails;
        }

        public async Task<EvaluateConditionResponse> EvaluateConditionAsync(
            EvaluateConditionRequest request,
            CancellationToken cancellationToken = default)
        {
            // 1. Get Step
            var step = await _unitOfWork.WorkflowSteps.GetByIdAsync(request.StepId, cancellationToken)
                ?? throw new KeyNotFoundException($"WorkflowStep with Id {request.StepId} not found");
                
            if (string.IsNullOrEmpty(step.NextStepCondition))
            {
                return new EvaluateConditionResponse
                {
                    ConditionMet = false,
                    EvaluationDetails = ResolutionConstants.NoConditionDefined
                };
            }

            // 2. Evaluate
            return _conditionEvaluator.Evaluate(step.NextStepCondition, request.DocumentData);
        }
    }
}

