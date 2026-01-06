using DTO;

namespace DAL.Interfaces
{
    public interface IAccountServices
    {
        Task<SignupDTO> SignupUserAsync(SignupDTO model);

        Task<TokenResponseModel> IsUserExists(SignInDTO signInDTO);

        Task<TokenResponseModel> RefreshTokenAsync(string refreshToken);
    }
}
