using AutoMapper;
using WorkflowApi.Application.DTOs.WorkflowRoute;
using WorkflowApi.Domain.Entities;

namespace WorkflowApi.Application.Mappings
{
    public class WorkflowRouteMappingProfile : Profile
    {
        public WorkflowRouteMappingProfile()
        {
            // Entity → Response
            CreateMap<WorkflowRoute, WorkflowRouteResponse>();

            // Entity → DetailResponse (with Steps)
            CreateMap<WorkflowRoute, WorkflowRouteDetailResponse>();

            // Request → Entity
            CreateMap<CreateWorkflowRouteRequest, WorkflowRoute>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Steps, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());
        }
    }
}