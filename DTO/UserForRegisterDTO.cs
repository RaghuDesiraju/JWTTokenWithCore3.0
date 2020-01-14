using System.ComponentModel.DataAnnotations;

namespace JWTToken.API.DTO
{
    public class UserForRegisterDTO
    {
        [Required]
        [StringLength(8)]       
        public string UserName { get; set; }

         [Required]
         [StringLength(15, MinimumLength =8, ErrorMessage = "You must specify password between 8 and 15 characters")]       
        public string Password { get; set; }
    }
}