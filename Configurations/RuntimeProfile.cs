using AutoMapper;
using Helpdesk_Backend_API.DTOs;
using Helpdesk_Backend_API.Entities;

namespace Helpdesk_Backend_API.Configurations
{
    public  class RuntimeProfile : Profile
    {
        public RuntimeProfile()
        {
            #region Organization
            CreateMap<Organization, GetOrganization>();
            #endregion


            #region Fusion Admin
            CreateMap<FusionAdmin, GetFusionAdminDto>()
                .ForMember(dest => dest.FirstName, option => option
                .MapFrom(src => src.User.FirstName))
                .ForMember(dest => dest.LastName, option => option
                .MapFrom(src => src.User.LastName))
                .ForMember(dest => dest.Email, option => option
                .MapFrom(src => src.User.Email))
                .ForMember(dest => dest.PhoneNumber, option => option
                .MapFrom(src => src.User.PhoneNumber));

            CreateMap<UpdateFusionAdminDto, FusionAdmin>();
            #endregion


            #region Product
            CreateMap<Product, GetProduct>()
                .ForPath(c => c.Project.Id, option => option
                .MapFrom(c => c.Project.Id))
                .ForPath(c => c.Project.Name, option => option
                .MapFrom(c => c.Project.Name))
                .ForPath(c => c.Project.Description, option => option
                .MapFrom(c => c.Project.Description))
                .ForPath(c => c.Project.DateCreated, option => option
                .MapFrom(c => c.Project.DateCreated))

                .ForPath(c => c.Project.Organization.Id, option => option
                .MapFrom(c => c.Project.Organization.Id))
                .ForPath(c => c.Project.Organization.Name, option => option
                .MapFrom(c => c.Project.Organization.Name))
                .ForPath(c => c.Project.Organization.Description, option => option
                .MapFrom(c => c.Project.Organization.Description))
                .ForPath(c => c.Project.Organization.DateCreated, option => option
                .MapFrom(c => c.Project.Organization.DateCreated));

            #endregion


            #region Project
            CreateMap<Project, GetProject>()
                .ForPath(c => c.Organization.Id, option => option
                .MapFrom(c => c.Organization.Id))
                .ForPath(c => c.Organization.Name, option => option
                .MapFrom(c => c.Organization.Name))
                .ForPath(c => c.Organization.Description, option => option
                .MapFrom(c => c.Organization.Description))
                .ForPath(c => c.Organization.DateCreated, option => option
                .MapFrom(c => c.Organization.DateCreated));

            #endregion

            #region Ticket
            CreateMap<Ticket, GetTicket>();

            #endregion

            #region Staff
            CreateMap<Staff, GetStaff>()
                .ForMember(c => c.FirstName, option => option
                .MapFrom(src => src.User.FirstName))
                .ForMember(dest => dest.LastName, option => option
                .MapFrom(src => src.User.LastName))
                .ForMember(dest => dest.Email, option => option
                .MapFrom(src => src.User.Email))
                .ForMember(dest => dest.PhoneNumber, option => option
                .MapFrom(src => src.User.PhoneNumber));

            #endregion
        }
    }
}
