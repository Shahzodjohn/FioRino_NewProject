using FioRino_NewProject.DataTransferObjects;
using System.Threading.Tasks;

namespace FioRino_NewProject.Services
{
    public interface IMailService
    {
        Task SendEmailAsync(MailRequestDTO mailRequest);
    }
}
