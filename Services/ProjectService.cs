using Helpdesk_Backend_API.DTOs;
using Helpdesk_Backend_API.Entities;
using Microsoft.EntityFrameworkCore;

namespace Helpdesk_Backend_API.Services
{
    public interface IProjectService
    {
        Task<ServiceResponse<Project>> AddProject(AddProject model, CancellationToken token);

        Task<ServiceResponse<Project>> GetProject(string projectId, CancellationToken token);

        IQueryable<Project> GetAll();

        IQueryable<Project> ListByOrganization(string organizationId);

        Task<bool> CheckIfProjectBelongsToOrganization(string organizationId, string projectId, CancellationToken token);
    }


    public class ProjectService : IProjectService
    {
        private readonly IRepositoryService repository;
        private readonly IOrganizationService organizationService;

        public ProjectService(IRepositoryService repository, IOrganizationService organizationService)
        {
            this.repository = repository;
            this.organizationService = organizationService;
        }

        public async Task<ServiceResponse<Project>> AddProject(AddProject model, CancellationToken token)
        {
            if(await organizationService.IsExisting(model.OrganizationId, token) == false)
            {
                return new ServiceResponse<Project>()
                {
                    Data = null,
                    Message = "Organization not found",
                    ResponseType = ResponseType.Failed
                };
            }


            var projectToAdd = new Project()
            {
                Name = model.Name,
                OrganizationId = model.OrganizationId,
                Description = model.Description
            };

            if(await repository.AddAsync(projectToAdd, token) is false)
            {
                return new ServiceResponse<Project>()
                {
                    Data = null,
                    Message = "Failed to create project",
                    ResponseType = ResponseType.Failed
                };
            }
            else
            {
                return new ServiceResponse<Project>()
                {
                    Data = await GetProjectPrivate(projectToAdd.Id, token),
                    Message = "success",
                    ResponseType = ResponseType.Success
                };
            }
        }

        public async Task<bool> CheckIfProjectBelongsToOrganization(string organizationId, string projectId, CancellationToken token)
        {
            return await repository.ListAll<Project>()
                .AnyAsync(c => c.Id == projectId && c.OrganizationId == organizationId, token);
        }

        public IQueryable<Project> GetAll()
        {
            return repository.ListAll<Project>()
                .Include(c => c.Organization);
        }

        public async Task<ServiceResponse<Project>> GetProject(string projectId, CancellationToken token)
        {
            var project = await GetProjectPrivate(projectId, token);

            if(project is null)
            {
                return new ServiceResponse<Project>()
                {
                    Data = null,
                    Message = "Not found",
                    ResponseType = ResponseType.NotFound
                };
            }
            else
            {
                return new ServiceResponse<Project>()
                {
                    Data = project,
                    Message = "Success",
                    ResponseType = ResponseType.Success
                };
            }
        }

        public IQueryable<Project> ListByOrganization(string organizationId)
        {
            return repository.ListAll<Project>()
                .Include(c => c.Organization)
                .Where(c => c.OrganizationId == organizationId);
        }

        private async Task<Project> GetProjectPrivate(string projectId, CancellationToken token)
        {
            return await repository.ListAll<Project>()
                .Include(c => c.Organization)
                .FirstOrDefaultAsync(c => c.Id == projectId, token);
        }
    }
}
