using System.Text.Json;
using AutoMapper;
using WorkflowApi.Application.DTOs.Condition;
using WorkflowApi.Application.DTOs.WorkflowStep;
using WorkflowApi.Domain.Entities;

namespace WorkflowApi.Application.Mappings
{
    public class WorkflowStepMappingProfile : Profile
    {
        public WorkflowStepMappingProfile()
        {
            // Entity → Response
            CreateMap<WorkflowStep, WorkflowStepResponse>()
                .ForMember(dest => dest.ExecutionMode, opt => opt.MapFrom(src => src.ExecutionMode.ToString()))
                .ForMember(dest => dest.CompletionRule, opt => opt.MapFrom(src => src.CompletionRule.ToString()))
                .ForMember(dest => dest.ReturnBehavior, opt => opt.MapFrom(src => src.ReturnBehavior.ToString()))
                .ForMember(dest => dest.ReturnStepName, opt => opt.MapFrom(src => src.ReturnStep != null ? src.ReturnStep.StepName : null))
                .ForMember(dest => dest.NextStepCondition, opt => opt.MapFrom(src => DeserializeCondition(src.NextStepCondition)));

            // Request → Entity
            CreateMap<CreateWorkflowStepRequest, WorkflowStep>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Route, opt => opt.Ignore())
                .ForMember(dest => dest.ReturnStep, opt => opt.Ignore())
                .ForMember(dest => dest.Assignments, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.NextStepCondition, opt => opt.MapFrom(src => SerializeCondition(src.NextStepCondition)));
        }

        private static NextStepConditionDto? DeserializeCondition(string? json)
        {
            if (string.IsNullOrEmpty(json))
                return null;

            return JsonSerializer.Deserialize<NextStepConditionDto>(json);
        }

        private static string? SerializeCondition(NextStepConditionDto? condition)
        {
            if (condition == null)
                return null;

            return JsonSerializer.Serialize(condition);
        }
    }
}