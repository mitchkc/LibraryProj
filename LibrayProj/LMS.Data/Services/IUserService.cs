
using LMS.Data.Entities;

namespace LMS.Data.Services
{

    // This interface describes the operations that a UserService class implementation should provide
    public interface IUserService
    {
        // Initialise the repository - only to be used during development 
        void Initialise();

        // ---------------- User Management --------------
        Task <List<User>> GetUsersListByIdAsync();
        Task <Paged<User>> GetUsersPagedAsync(int page=1, int size=20, string orderBy="id", string direction="asc");
        Task<User?> GetUserByIdAsync(int uid);
        Task <User?> GetUserByEmailAsync(string email);
        Task<bool> IsEmailAvailableAsync(string email, int userId);
        Task<User?> AddUserAsync(string forename, string surname, string password, Role role, DateOnly dateOfRegistration, string email, string address, string gender, DateTime dob, string contactNumber);
        Task<User?> UpdateUserByIdAsync(User user);
        Task<bool> DeleteUserByIdAsync(int uid);
        Task<User?> AuthenticateAsync(string email, string password);
        Task<string?> ForgotPasswordAsync(string email);
        Task<User?> ResetPasswordAsync(string email, string token, string password);
        Task<IList<string>> GetValidPasswordResetTokensAsync();
    }

}