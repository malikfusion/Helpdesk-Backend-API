using Helpdesk_Backend_API.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Helpdesk_Backend_API.Services
{
    public interface IFusionAdminService
    {
        Task<FusionAdmin> Create(FusionAdmin FusionAdmin, CancellationToken token);

        Task<FusionAdmin> GetFusionAdmin(string FusionAdminId, CancellationToken token);

        IQueryable<FusionAdmin> ListAllFusionAdmins();

        Task<FusionAdmin> Update(FusionAdmin FusionAdmin, CancellationToken token);

        Task<FusionAdmin> Toggle(FusionAdmin FusionAdmin, CancellationToken token);

        Task<bool> IsUserNameTaken(string username, CancellationToken token);
        
    }

    public class FusionAdminService : IFusionAdminService
    {
        private readonly IRepositoryService repositoryService;

        public FusionAdminService(IRepositoryService repositoryService)
        {
            this.repositoryService = repositoryService;
        }

        public async Task<FusionAdmin> Create(FusionAdmin FusionAdmin, CancellationToken token)
        {
            if(FusionAdmin is null)
            {
                return null;
            }
            else
            {
                if(await repositoryService.AddAsync(FusionAdmin, token) is false)
                {
                    return null;
                }
                else
                {
                    return await GetFusionAdmin(FusionAdmin.Id, token);
                }
            }
        }     

        public async Task<FusionAdmin> GetFusionAdmin(string FusionAdminId, CancellationToken token)
        {
            return await repositoryService.ListAll<FusionAdmin>()
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == FusionAdminId);
        }

        public async Task<bool> IsUserNameTaken(string username, CancellationToken token)
        {
            return await repositoryService.ListAll<FusionAdmin>()
                .AnyAsync(c => c.User.UserName == username || c.User.Email == username, token);
        }

        public IQueryable<FusionAdmin> ListAllFusionAdmins()
        {
            return repositoryService.ListAll<FusionAdmin>().Include(c => c.User);
        }

        public async Task<FusionAdmin> Toggle(FusionAdmin FusionAdmin, CancellationToken token)
        {
            if(FusionAdmin is null)
            {
                return null;
            }

            FusionAdmin.IsActive = !FusionAdmin.IsActive;
            return await Update(FusionAdmin, token);
        }

        public async Task<FusionAdmin> Update(FusionAdmin FusionAdmin, CancellationToken token)
        {
            if(FusionAdmin is null)
            {
                return null;
            }

            FusionAdmin.DateModified = DateTime.UtcNow;

            if (await repositoryService.ModifyAsync(FusionAdmin, token) is false)
            {
                return null;
            }

            return await GetFusionAdmin(FusionAdmin.Id, token);
        }
    }
}
