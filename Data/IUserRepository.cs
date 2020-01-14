using System.Threading.Tasks;
using JWTToken.API.Models;

namespace JWTToken.API.Data
{
    public interface IUserRepository
    {
         Task<User>GetUser(int id);

    }
}