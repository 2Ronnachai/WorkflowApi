using AutoMapper;
using WorkflowApi.Application.DTOs.WorkflowDelegation;
using WorkflowApi.Domain.Entities;

namespace WorkflowApi.Application.Mappings
{
    public class DelegationMappingProfile : Profile
    {
        public DelegationMappingProfile()
        {
           CreateMap<WorkflowDelegation, DelegationResponse>()
                .ForMember(dest => dest.Scope, opt => opt.MapFrom(src => src.Scope.ToString()))
                .ForMember(dest => dest.RouteName, opt => opt.MapFrom(src => src.Route != null ? src.Route.RouteName : null))
                .ForMember(dest => dest.StepName, opt => opt.MapFrom(src => src.Step != null ? src.Step.StepName : null));
        }
    }
}