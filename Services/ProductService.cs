using Helpdesk_Backend_API.DTOs;
using Helpdesk_Backend_API.Entities;
using Microsoft.EntityFrameworkCore;

namespace Helpdesk_Backend_API.Services
{
    public interface IProductService
    {
        Task<ServiceResponse<Product>> AddProduct(AddProduct model, CancellationToken token);

        Task<ServiceResponse<Product>> GetProduct(string productId, CancellationToken token);

        IQueryable<Product> GetAll();

        IQueryable<Product> ListByProject(string projectId);

        IQueryable<Product> ListByOrganization(string organizationId);
    }


    public class ProductService : IProductService
    {

        private readonly IRepositoryService repositoryService;
        private readonly IProjectService projectService;
        private readonly IOrganizationService organizationService;

        public ProductService(IProjectService projectService, IRepositoryService repositoryService,
            IOrganizationService organizationService)
        {
            this.projectService = projectService;
            this.repositoryService = repositoryService;
            this.organizationService = organizationService;
        }

        public async Task<ServiceResponse<Product>> AddProduct(AddProduct model, CancellationToken token)
        {
            if(await organizationService.IsExisting(model.OrganizationId, token) is false)
            {
                return new ServiceResponse<Product>()
                {
                    Data = null,
                    Message = "Organization not found",
                    ResponseType = ResponseType.Failed
                };
            }

            if(await projectService.CheckIfProjectBelongsToOrganization(model.OrganizationId, model.ProjectId, token) is false)
            {
                return new ServiceResponse<Product>()
                {
                    Message = "Project does not belong to the specified organization",
                    ResponseType = ResponseType.Failed
                };
            }

            var productToAdd = new Product()
            {
                ProjectId = model.ProjectId,
                OrganizationId = model.OrganizationId,
                Name = model.Name,
                Description = model.Description
            };

            if(await repositoryService.AddAsync(productToAdd, token) is false)
            {
                return new ServiceResponse<Product>()
                {
                    Message = "Failed to add Product",
                    ResponseType = ResponseType.Failed
                };
            }
            else
            {
                return new ServiceResponse<Product>()
                {
                    Data = await GetProductPrivate(productToAdd.Id, token),
                    Message = "success",
                    ResponseType = ResponseType.Success
                };
            }
        }

        public IQueryable<Product> GetAll()
        {
            return repositoryService.ListAll<Product>()
                .Include(c => c.Project.Organization);
        }

        public async Task<ServiceResponse<Product>> GetProduct(string productId, CancellationToken token)
        {
            var product = await GetProductPrivate(productId, token);

            if(product is null)
            {
                return new ServiceResponse<Product>()
                {
                    Message = "Product not found",
                    ResponseType = ResponseType.NotFound
                };
            }
            else
            {
                return new ServiceResponse<Product>()
                {
                    Data = product,
                    ResponseType = ResponseType.Success,
                    Message = "success"
                };
            }
        }

        public IQueryable<Product> ListByOrganization(string organizationId)
        {
            return repositoryService.ListAll<Product>()
                .Include(c => c.Project)
                .ThenInclude(c => c.Organization)
                .Where(c => c.OrganizationId == organizationId);
        }

        public IQueryable<Product> ListByProject(string projectId)
        {
            return repositoryService.ListAll<Product>()
                .Include(c => c.Project)
                .ThenInclude(c => c.Organization)
                .Where(c => c.ProjectId == projectId);
        }

        private async Task<Product> GetProductPrivate(string productId, CancellationToken token)
        {
            return await repositoryService.ListAll<Product>()
                .Include(c => c.Project)
                .ThenInclude(c => c.Organization)
                .FirstOrDefaultAsync(c => c.Id == productId, token);
        }
    }
}
