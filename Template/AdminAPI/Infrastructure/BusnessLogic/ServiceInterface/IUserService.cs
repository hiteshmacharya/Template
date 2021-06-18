using System.Threading.Tasks;
using Upscript.Services.Admin.API.Model;

namespace Upscript.Services.Admin.API.Infrastructure.BusnessLogic.ServiceInterface
{
    public interface IUserService
    {
        Task<User> CreateUserAsync(User user);
    }
}
