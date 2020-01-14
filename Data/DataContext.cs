
using JWTToken.API.Models;
using Microsoft.EntityFrameworkCore;
namespace JWTToken.API.Data
{
    //dotnet add package Microsoft.EntityFrameworkCore
    public class DataContext:DbContext
    {
        public DataContext(DbContextOptions<DataContext>options):base(options)
        {            
        }
         public DbSet<User>Users{get;set;}
    }
}