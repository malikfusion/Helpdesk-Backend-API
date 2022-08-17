using Helpdesk_Backend_API.DTOs;
using Helpdesk_Backend_API.Entities;
using Helpdesk_Backend_API.Utilities;
using Microsoft.EntityFrameworkCore;

namespace Helpdesk_Backend_API.Services
{
    public interface IOrganizationService
    {
        Task<ServiceResponse<Organization>> AddOrganization(CreateOrganization model, CancellationToken token);

        Task<ServiceResponse<Organization>> GetOrganization(string organizationId, CancellationToken token);

        IQueryable<Organization> GetAll();

        Task<bool> IsExisting(string organizationId, CancellationToken token);
    }


    public class OrganizationService : IOrganizationService
    {
        private readonly IRepositoryService repository;

        public OrganizationService(IRepositoryService repository)
        {
            this.repository = repository;
        }

        public async Task<ServiceResponse<Organization>> AddOrganization(CreateOrganization model, CancellationToken token)
        {
             if(await repository.ListAll<Organization>()
                 .FindNameByRegex(model.Name))
            {
                return new ServiceResponse<Organization>()
                {
                    ResponseType = ResponseType.Duplicate,
                    Message = "Name already taken",
                    Data = null
                };
            }

            var organizationToAdd = new Organization()
            {
                Name = model.Name,
                Description = model.Description
            };

            if(await repository.AddAsync(organizationToAdd, token) is false)
            {
                return new ServiceResponse<Organization>()
                {
                    Data = null,
                    Message = "Failed to create organization",
                    ResponseType = ResponseType.Failed
                };
            }
            else
            {
                return new ServiceResponse<Organization>()
                {
                    Data = await GetOrganizationPrivate(organizationToAdd.Id, token),
                    Message = "success",
                    ResponseType = ResponseType.Success
                };
            }
        }

        public IQueryable<Organization> GetAll()
        {
            return repository.ListAll<Organization>();
        }

        public async Task<ServiceResponse<Organization>> GetOrganization(string organizationId, CancellationToken token)
        {
            var organization = await GetOrganizationPrivate(organizationId, token);

            if(organization is null)
            {
                return new ServiceResponse<Organization>()
                {
                    Data = null,
                    Message = "Organization not found",
                    ResponseType = ResponseType.NotFound
                };
            }
            else
            {
                return new ServiceResponse<Organization>()
                {
                    Data = organization,
                    Message = "success",
                    ResponseType = ResponseType.Success
                };
            }
        }

        public async Task<Organization> GetOrganizationPrivate(string organizationId, CancellationToken token)
        {
            return await repository.ListAll<Organization>()
                .FirstOrDefaultAsync(c => c.Id == organizationId);
        }

        public async Task<bool> IsExisting(string organizationId, CancellationToken token)
        {
            return await repository.ListAll<Organization>()
                .AnyAsync(c => c.Id == organizationId, token);
        }
    }
}
