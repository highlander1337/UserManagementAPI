using UserManagementAPI.Models.User;

namespace UserManagementAPI.Services;

public interface IUserRepository
{
    Task<UserPagedResult> GetAllAsync(int pageNumber = 1, int pageSize = 10);
    Task<UserDetailResponse?> GetByIdAsync(Guid id);
    Task<UserCreateResponse?> CreateAsync(UserCreateRequest user);
    Task<UserUpdateResponse?> UpdateAsync(Guid id, UserUpdateRequest user);
    Task<bool> DeleteAsync(Guid id);
}
