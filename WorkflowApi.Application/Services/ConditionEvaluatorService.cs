using System.Text.Json;
using WorkflowApi.Application.DTOs.Condition;
using WorkflowApi.Application.Interfaces;

namespace WorkflowApi.Application.Services
{
    public class ConditionEvaluatorService : IConditionEvaluator
    {
        public EvaluateConditionResponse Evaluate(
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
                        EvaluationDetails = ResolutionConstants.InvalidConditionFormat
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
                    EvaluationDetails = ResolutionConstants.NoConditionMet
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