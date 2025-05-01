using TestApp.DTO;

namespace TestApp.Interfaces
{
    public interface IExternalUserService
    {
        public Task<User> GetUserByIdAsync(int userId);

        public Task<IEnumerable<User>> GetAllUsersAsync();
    }
}
