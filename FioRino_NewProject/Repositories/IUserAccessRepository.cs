using FioRino_NewProject.Entities;
using System.Threading.Tasks;

namespace FioRino_NewProject.Repositories
{
    public interface IUserAccessRepository
    {
        Task<DmUsersAccess> GetById(int Id);
    }
}
