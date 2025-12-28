using DAL.Interfaces;
using DTO;
using Entities;
using Microsoft.AspNetCore.Identity;

namespace DAL.Repositories
{
    public class AccountServices(IGenericRepository<ApplicationUser> _userRepo, UserManager<ApplicationUser> _userManager) : IAccountServices
    {
        public async Task<SignupDTO> SignupUserAsync(SignupDTO model)
        {
            ApplicationUser user = new ApplicationUser { UserName = model.Email, Email = model.Email, PasswordHash = model.Password, PhoneNumber = model.PhoneNumber };

            IdentityResult result = await _userManager.CreateAsync(user, password: user.PasswordHash);

            if (!result.Succeeded)
                throw new Exception("User not successfully created.");

            return model;
        }
    }
}
