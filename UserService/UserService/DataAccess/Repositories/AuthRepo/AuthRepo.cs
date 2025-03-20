using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace UserService.DataAccess.Repositories.AuthRepo
{
    public class AuthRepo : IAuthRepo
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthRepo(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
    }
}
