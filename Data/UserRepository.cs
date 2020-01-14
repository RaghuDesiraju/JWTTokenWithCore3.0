using System.Threading.Tasks;
using JWTToken.API.Models;
using Microsoft.EntityFrameworkCore;

namespace JWTToken.API.Data
{
    public class UserRepository:IUserRepository
    {
         DataContext _context;
        public UserRepository(DataContext context)
        {
            _context = context;
        }

         public async Task<User> GetUser(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u=>u.Id == id);
            return user;
        }

    }
}