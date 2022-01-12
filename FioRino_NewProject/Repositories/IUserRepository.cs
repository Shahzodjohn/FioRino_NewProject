using FioRino_NewProject.Entities;
using System.Threading.Tasks;

namespace FioRino_NewProject.Repositories
{
    public interface IUserRepository
    {
        Task<DmUser> Create(DmUser user);
        Task<DmUser> GetByEmail(string email);
        Task<DmUser> GetUser(int Id);
        Task<DmUser> Delete(DmUser dmUser);
        DmUser UserRole(int roleId);
    }
}
