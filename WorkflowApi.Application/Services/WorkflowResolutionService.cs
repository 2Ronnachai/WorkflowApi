using System.Text.Json;
using WorkflowApi.Application.DTOs.Condition;
using WorkflowApi.Application.DTOs.Resolution;
using WorkflowApi.Application.Interfaces;
using WorkflowApi.Domain.Entities;
using WorkflowApi.Domain.Enums;
using WorkflowApi.Domain.Interfaces;

namespace WorkflowApi.Application.Services
{
    public class WorkflowResolutionService(
        IWorkflowRouteRepository routeRepository,
        IWorkflowStepRepository stepRepository) : IWorkflowResolutionService
    {
        private readonly IWorkflowRouteRepository _routeRepository = routeRepository;
        private readonly IWorkflowStepRepository _stepRepository = stepRepository;

        public async Task<ResolvedWorkflowResponse> ResolveWorkflowAsync(
            ResolveWorkflowRequest request,
            CancellationToken cancellationToken = default)
        {
            // 1. Find Route by RouteId
            var route = request.RouteId > 0
                ? await _routeRepository.GetCompleteRouteAsync(request.RouteId, cancellationToken)
                : await _routeRepository.GetCompleteRouteByDocumentTypeAsync(request.DocumentType, cancellationToken);

            if (route == null)
            {
                throw new KeyNotFoundException(
                    request.RouteId > 0
                        ? $"No route found for RouteId '{request.RouteId}'"
                        : $"No active workflow route found for DocumentType '{request.DocumentType}'"
                );
            }

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

        private async Task<ResolvedWorkflowStepResponse> ResolveStepAsync(
            WorkflowStep step,
            ResolveWorkflowRequest request,
            CancellationToken cancellationToken)
        {
            var resolvedStep = new ResolvedWorkflowStepResponse
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

            // Resolve Assignees
            foreach (var assignment in step.Assignments)
            {
                var assignees = await ResolveAssignmentAsync(
                    assignment, 
                    request, 
                    cancellationToken);
                
                resolvedStep.Assignees.AddRange(assignees);
            }

            // Evaluate Conditional Routing (if exists)
            if (!string.IsNullOrEmpty(step.NextStepCondition) && request.ConditionalData != null)
            {
                var conditionResult = EvaluateNextStepCondition(
                    step.NextStepCondition, 
                    request.ConditionalData);
                
                resolvedStep.NextStepId = conditionResult.NextStepId;
                resolvedStep.NextStepConditionResult = conditionResult.EvaluationDetails;
            }

            return resolvedStep;
        }

        private async Task<List<ResolvedAssigneeResponse>> ResolveAssignmentAsync(
            WorkflowStepAssignment assignment,
            ResolveWorkflowRequest request,
            CancellationToken cancellationToken)
        {
            var assignees = new List<ResolvedAssigneeResponse>();

            if (assignment.AssignmentType == AssignmentType.SpecificEmployee)
            {
                // ✅ Employee-based Assignment 
                var assignee = new ResolvedAssigneeResponse
                {
                    AssignmentId = assignment.Id,
                    AssignmentType = "SpecificEmployee",
                    NId = assignment.NId!,
                    ResolutionMethod = "DirectAssignment"
                };

                // TODO: Call external API to get employee details
                // assignee.EmployeeName = await GetEmployeeName(assignment.NId);
                // assignee.Email = await GetEmployeeEmail(assignment.NId);

                assignees.Add(assignee);
            }
            else if (assignment.AssignmentType == AssignmentType.Position)
            {
                // Position-based Assignment
                var targetOuIds = ResolveOrganizationalUnit(assignment, request);
                foreach(var ouId in targetOuIds)
                {
                    // 2. หาคนที่ PositionId + ouId
                    // TODO: var employees = await FindEmployeesByPositionAndOu(
                    //     assignment.PositionId!.Value, 
                    //     ouId);
                    
                    // 3. ถ้าเจอ → เพิ่มเข้า assignees
                    // foreach (var emp in employees)
                    // {
                    //     assignees.Add(new ResolvedAssigneeResponse
                    //     {
                    //         AssignmentId = assignment.Id,
                    //         AssignmentType = "Position",
                    //         PositionId = assignment.PositionId,
                    //         OrganizationalUnitId = ouId,
                    //         NId = emp.NId,
                    //         EmployeeName = emp.Name,
                    //         Email = emp.Email,
                    //         ResolutionMethod = GetResolutionMethod(assignment.OUResolutionMode)
                    //     });
                    // }
                }
            }

            return assignees;
        }

        private List<int> ResolveOrganizationalUnit(
            WorkflowStepAssignment assignment,
            ResolveWorkflowRequest request)
        {
            return assignment.OUResolutionMode switch
            {
                OUResolutionMode.FollowOrigin => request.OrganizationalUnitIds,
                
                OUResolutionMode.FollowInitiator => 
                    [GetInitiatorOuId(request.CreatedBy)],
                
                OUResolutionMode.Fixed => 
                    [assignment.OrganizationalUnitId!.Value],
        
                _ => throw new InvalidOperationException(
                    "Unsupported OUResolutionMode")
            };
        }

        private int GetInitiatorOuId(string createdBy)
        {
            // TODO: Call external API to get employee's OU
            // return await _organizationService.GetEmployeeOuId(createdBy);
            
            return 0; // Mock
        }

        public async Task<EvaluateConditionResponse> EvaluateConditionAsync(
            EvaluateConditionRequest request,
            CancellationToken cancellationToken = default)
        {
            // 1. Get Step
            var step = await _stepRepository.GetByIdAsync(request.StepId, cancellationToken) 
                ?? throw new KeyNotFoundException($"WorkflowStep with Id {request.StepId} not found");
                
            if (string.IsNullOrEmpty(step.NextStepCondition))
            {
                return new EvaluateConditionResponse
                {
                    ConditionMet = false,
                    EvaluationDetails = "No condition defined"
                };
            }

            // 2. Evaluate
            return EvaluateNextStepCondition(step.NextStepCondition, request.DocumentData);
        }

        private EvaluateConditionResponse EvaluateNextStepCondition(
            string conditionJson,
            Dictionary<string, object> documentData)
        {
            try
            {
                var condition = JsonSerializer.Deserialize<NextStepConditionDto>(conditionJson);
                if (condition == null)
                {
                    return new EvaluateConditionResponse
                    {
                        ConditionMet = false,
                        EvaluationDetails = "Invalid condition format"
                    };
                }

                // Evaluate each condition rule
                foreach (var rule in condition.Conditions)
                {
                    if (EvaluateRule(rule, documentData))
                    {
                        return new EvaluateConditionResponse
                        {
                            NextStepId = rule.NextStepId,
                            ConditionMet = true,
                            MatchedRule = $"{rule.Field} {rule.Operator} {rule.Value}",
                            EvaluationDetails = $"Condition met: {rule.Field} {rule.Operator} {rule.Value}"
                        };
                    }
                }

                // No condition met, use default
                return new EvaluateConditionResponse
                {
                    NextStepId = condition.DefaultNextStepId,
                    ConditionMet = false,
                    EvaluationDetails = "No condition met, using default next step"
                };
            }
            catch (Exception ex)
            {
                return new EvaluateConditionResponse
                {
                    ConditionMet = false,
                    EvaluationDetails = $"Error evaluating condition: {ex.Message}"
                };
            }
        }

        private static bool EvaluateRule(ConditionRuleDto rule, Dictionary<string, object> documentData)
        {
            if (!documentData.TryGetValue(rule.Field, out object? fieldValue))
                return false;

            var ruleValue = rule.Value;

            
            var dataType = rule.DataType ?? DetectDataType(fieldValue);

            return dataType.ToLower() switch
            {
                "decimal" or "int" or "double" or "numeric" => 
                    EvaluateNumeric(fieldValue, ruleValue, rule.Operator),
                
                "datetime" or "date" => 
                    EvaluateDateTime(fieldValue, ruleValue, rule.Operator),
                
                "boolean" or "bool" => 
                    EvaluateBoolean(fieldValue, ruleValue, rule.Operator),
                
                "string" or _ => 
                    EvaluateString(fieldValue, ruleValue, rule.Operator)
            };
        }

        private static string DetectDataType(object value)
        {
            return value switch
            {
                int or long or decimal or double or float => "Numeric",
                DateTime => "DateTime",
                bool => "Boolean",
                string str when DateTime.TryParse(str, out _) => "DateTime",
                string str when decimal.TryParse(str, out _) => "Numeric",
                string str when bool.TryParse(str, out _) => "Boolean",
                _ => "String"
            };
        }

        private static bool EvaluateNumeric(object fieldValue, object ruleValue, string op)
        {
            try
            {
                var field = Convert.ToDecimal(fieldValue);
                var rule = Convert.ToDecimal(ruleValue);

                return op switch
                {
                    ">" => field > rule,
                    ">=" => field >= rule,
                    "<" => field < rule,
                    "<=" => field <= rule,
                    "==" => field == rule,
                    "!=" => field != rule,
                    _ => false
                };
            }
            catch
            {
                return false;
            }
        }

        private static bool EvaluateDateTime(object fieldValue, object ruleValue, string op)
        {
            try
            {
                var field = Convert.ToDateTime(fieldValue);
                var rule = Convert.ToDateTime(ruleValue);

                return op switch
                {
                    ">" => field > rule,
                    ">=" => field >= rule,
                    "<" => field < rule,
                    "<=" => field <= rule,
                    "==" => field.Date == rule.Date,
                    "!=" => field.Date != rule.Date,
                    _ => false
                };
            }
            catch
            {
                return false;
            }
        }

        private static bool EvaluateBoolean(object fieldValue, object ruleValue, string op)
        {
            try
            {
                var field = Convert.ToBoolean(fieldValue);
                var rule = Convert.ToBoolean(ruleValue);

                return op switch
                {
                    "==" => field == rule,
                    "!=" => field != rule,
                    _ => false
                };
            }
            catch
            {
                return false;
            }
        }

        private static bool EvaluateString(object fieldValue, object ruleValue, string op)
        {
            var field = fieldValue?.ToString() ?? "";
            var rule = ruleValue?.ToString() ?? "";

            return op switch
            {
                "==" => field == rule,
                "!=" => field != rule,
                "contains" => field.Contains(rule, StringComparison.OrdinalIgnoreCase),
                "startsWith" => field.StartsWith(rule, StringComparison.OrdinalIgnoreCase),
                "endsWith" => field.EndsWith(rule, StringComparison.OrdinalIgnoreCase),
                _ => false
            };
        }
    }
}