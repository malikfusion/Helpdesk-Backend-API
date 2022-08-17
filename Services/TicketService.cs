using Helpdesk_Backend_API.DTOs;
using Helpdesk_Backend_API.Entities;
using Microsoft.EntityFrameworkCore;

namespace Helpdesk_Backend_API.Services
{
    public interface ITicketService
    {
        Task<ServiceResponse<Ticket>> AddTicket(AddTicket model, CancellationToken token);
        Task<ServiceResponse<Ticket>> GetTicket(string ticketId, CancellationToken token);
        IQueryable<Ticket> GetAll();
        IQueryable<Ticket> ListByOrganization(string organizationId);
        IQueryable<Ticket> ListByStaff(string staffId);
        Task<bool> CheckIfTicketIsAssignedToStaffOfOrganization(string staffId, string ticketId, CancellationToken token);
    }
    public class TicketService : ITicketService
    {
        private readonly IRepositoryService repositoryService;
        private readonly IOrganizationService organizationService;
        private readonly IStaffService staffService;

        public TicketService(IRepositoryService repositoryService, IOrganizationService organizationService, 
            IStaffService staffService)
        {
            this.repositoryService = repositoryService;
            this.organizationService = organizationService;
            this.staffService = staffService;
        }
        public async Task<ServiceResponse<Ticket>> AddTicket(AddTicket model, CancellationToken token)
        {
            if(await organizationService.IsExisting(model.OrganizationId, token) == false)
            {
                return new ServiceResponse<Ticket>()
                {
                    Data = null,
                    Message = "Organization not found",
                    ResponseType = ResponseType.Failed
                };
            }

            var addTicket = new Ticket()
            {
                Name = model.Name,
                OrganizationId = model.OrganizationId,
                Description = model.Description,
            };

            if(await repositoryService.AddAsync(addTicket, token) == false)
            {
                return new ServiceResponse<Ticket>()
                {
                    Data = null,
                    Message = "Failed to Create Ticket",
                    ResponseType = ResponseType.Failed
                };
            }
            else
            {
                return new ServiceResponse<Ticket>()
                {
                    Data = await GetTicketPrivate(addTicket.Id, token),
                    Message = "Success",
                    ResponseType = ResponseType.Success
                };
            }
        }

        private async Task<Ticket> GetTicketPrivate(string ticketId, CancellationToken token)
        {
            return await repositoryService.ListAll<Ticket>()
                .Include(t => t.Organization)
                .FirstOrDefaultAsync(t => t.Id == ticketId, token);
        }


        public async Task<ServiceResponse<Ticket>> GetTicket(string ticketId, CancellationToken token)
        {
            var ticket = await GetTicketPrivate(ticketId, token);
            if( ticket == null)
            {
                return new ServiceResponse<Ticket>()
                {
                    Data = null,
                    Message = "Ticket not found",
                    ResponseType = ResponseType.Failed
                };
            }
            else
            {
                return new ServiceResponse<Ticket>()
                {
                    Data = ticket,
                    Message = "Success",
                    ResponseType = ResponseType.Success
                };
            }
        }
        public IQueryable<Ticket> ListByOrganization(string organizationId)
        {
            return repositoryService.ListAll<Ticket>()
                .Include(t => t.StaffAssignedTo)
                .ThenInclude(t => t.Organization)
                .Where(t => t.OrganizationId == organizationId);
        }

        public IQueryable<Ticket> ListByStaff(string staffId)
        {
            return repositoryService.ListAll<Ticket>()
                .Include(t => t.StaffAssignedTo)
                .ThenInclude(t => t.Organization)
                .Where(t => staffId == t.StaffAssignedToId);
        }

        public IQueryable<Ticket> GetAll()
        {
            return repositoryService.ListAll<Ticket>()
                .Include(t => t.StaffAssignedTo.Organization);
        }

        public async Task<bool> CheckIfTicketIsAssignedToStaffOfOrganization(string staffId, string ticketId, CancellationToken token)
        {
            return await repositoryService.ListAll<Ticket>()
            .AnyAsync(t => t.Id == ticketId && t.StaffAssignedToId == staffId);
        }
    }
}
