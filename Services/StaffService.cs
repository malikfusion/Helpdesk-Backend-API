using Helpdesk_Backend_API.DTOs;
using Helpdesk_Backend_API.Entities;
using Microsoft.EntityFrameworkCore;

namespace Helpdesk_Backend_API.Services
{
    public interface IStaffService
    {
        Task<ServiceResponse<Staff>> AddStaff(AddStaff model, CancellationToken token);
        Task<ServiceResponse<Staff>> GetStaff(string staffId, CancellationToken token);
        IQueryable<Staff> GetAll();
        IQueryable<Staff> ListByOrganization(string organizationId);
        Task<bool> CheckIfStaffBelongToOrganization(string organizationId, string staffId, CancellationToken token);
    }


    public class StaffService : IStaffService
    {
        private readonly IRepositoryService repositoryService;
        private readonly IOrganizationService organizationService;

        public StaffService(IRepositoryService repositoryService, IOrganizationService organizationService)
        {
            this.repositoryService = repositoryService;
            this.organizationService = organizationService;
        }
        public async Task<ServiceResponse<Staff>> AddStaff(AddStaff model, CancellationToken token)
        {
            if(await organizationService.IsExisting(model.OrganizationId, token) == false)
            {
                return new ServiceResponse<Staff>()
                {
                    Data = null,
                    Message = "Organization not found",
                    ResponseType = ResponseType.Failed
                };
            }
            var addStaff = new Staff()
            {
                OrganizationId = model.OrganizationId,
            };

            if (await repositoryService.AddAsync(addStaff, token) == false)
            {
                return new ServiceResponse<Staff>()
                {
                    Data = null,
                    Message = "Staff not found",
                    ResponseType = ResponseType.Failed
                };
            }
            else
            {
                return new ServiceResponse<Staff>()
                {
                    Data = await GetStaffPrivate(addStaff.Id, token),
                    Message = "Success",
                    ResponseType = ResponseType.Success
                };
            }
        }

        private async Task<Staff> GetStaffPrivate(string id, CancellationToken token)
        {
            return await repositoryService.ListAll<Staff>()
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == id, token);
        }

        public async Task<ServiceResponse<Staff>> GetStaff(string staffId, CancellationToken token)
        {
            var staff = await GetStaffPrivate(staffId, token);
            if(staff == null)
            {
                return new ServiceResponse<Staff>()
                {
                    Data = null,
                    Message = "Staff not found",
                    ResponseType = ResponseType.Failed
                };
            }
            else
            {
                return new ServiceResponse<Staff>()
                {
                    Data = staff,
                    Message = "Success",
                    ResponseType = ResponseType.Success
                };
            }
        }


        public IQueryable<Staff> ListByOrganization(string organizationId)
        {
            return repositoryService.ListAll<Staff>()
                .Include(s => s.User)
                .Where(s => s.OrganizationId == organizationId);
        }

        public async Task<bool> CheckIfStaffBelongToOrganization(string organizationId, string staffId, CancellationToken token)
        {
            return await repositoryService.ListAll<Staff>()
                .AnyAsync(s => s.Id == staffId && s.OrganizationId == organizationId, token);
        }

        public IQueryable<Staff> GetAll()
        {
            return repositoryService.ListAll<Staff>()
                .Include(s => s.User);
        }
    }
}