using AutoMapper;
using WorkflowApi.Application.DTOs.WorkflowStepAssignment;
using WorkflowApi.Domain.Entities;

namespace WorkflowApi.Application.Mappings;

public class WorkflowStepAssignmentMappingProfile : Profile
{
    public WorkflowStepAssignmentMappingProfile()
    {
        // Entity → Response
        CreateMap<WorkflowStepAssignment, WorkflowStepAssignmentResponse>()
            .ForMember(dest => dest.AssignmentType, opt => opt.MapFrom(src => src.AssignmentType.ToString()))
            .ForMember(dest => dest.OUResolutionMode, opt => opt.MapFrom(src => src.OUResolutionMode.ToString()))
            .ForMember(dest => dest.PositionName, opt => opt.Ignore()) // To be populated separately
            .ForMember(dest => dest.OrganizationalUnitName, opt => opt.Ignore())
            .ForMember(dest => dest.EmployeeName, opt => opt.Ignore());

        // Request → Entity
        CreateMap<CreateWorkflowStepAssignmentRequest, WorkflowStepAssignment>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Step, opt => opt.Ignore());

        CreateMap<UpdateWorkflowStepAssignmentRequest, WorkflowStepAssignment>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.StepId, opt => opt.Ignore())
            .ForMember(dest => dest.Step, opt => opt.Ignore());
    }
}
