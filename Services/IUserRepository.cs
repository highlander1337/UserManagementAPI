using UserManagementAPI.Models.User;

namespace UserManagementAPI.Services;

public interface IUserRepository
{
    Task<IEnumerable<UserListResponse>> GetAllAsync();
    Task<UserDetailResponse?> GetByIdAsync(Guid id);
    Task<UserCreateResonse> CreateAsync(UserCreateRequest user);
    Task<UserUpdateResponse?> UpdateAsync(Guid id, UserUpdateRequest user);
    Task<bool> DeleteAsync(Guid id);
}
