using HAUM_BackEnd.Entities;

namespace HAUM_BackEnd.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<UserDTO>?> GetUsersAsync();
        Task<UserDTO?> GetUserByIdAsync(Guid userId);
        Task<bool> AddUserAsync(UserDTO userDTO);
        Task<bool> UpdateUserAsync(UserDTO userDTO);
        Task<bool> DeleteUserAsync(Guid userId);
        Task<Guid> LoginAsync(UserDTO userDTO);
    }
}
