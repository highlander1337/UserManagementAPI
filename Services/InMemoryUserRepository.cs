using UserManagementAPI.Entities;
using UserManagementAPI.Models.User;

namespace UserManagementAPI.Services;

public class InMemoryUserRepository : IUserRepository
{
    private readonly List<Users> _users = new();

    public InMemoryUserRepository()
    {
        // Seed with a sample user
        _users.Add(new Users
        {
            Id = Guid.NewGuid(),
            FirstName = "Admin",
            LastName = "User",
            Email = "admin@example.com"
        });
    }

    public Task<IEnumerable<UserListResponse>> GetAllAsync()
    {
        var userList = _users.Select(u => new UserListResponse
        {
            Id = u.Id,
            FirstName = u.FirstName,
            LastName = u.LastName,
            Email = u.Email
        });

        return Task.FromResult(userList);
    }

    public Task<UserDetailResponse?> GetByIdAsync(Guid id)
    {
        var user = _users.FirstOrDefault(u => u.Id == id);

        var userDetail = user == null ? null : new UserDetailResponse
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email
        };

        return Task.FromResult(userDetail);
    }

    public Task<UserCreateResonse> CreateAsync(UserCreateRequest user)
    {
        var newUser = new Users
        {
            Id = Guid.NewGuid(),
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email
        };

        _users.Add(newUser);

        var response = new UserCreateResonse
        {
            Id = newUser.Id,
            FirstName = newUser.FirstName,
            LastName = newUser.LastName,
            Email = newUser.Email
        };

        return Task.FromResult(response);
    }

    public Task<UserUpdateResponse?> UpdateAsync(Guid id, UserUpdateRequest user)
    {
        var existing = _users.FirstOrDefault(u => u.Id == id);

        var userUpdated = existing == null ? null : new UserUpdateResponse
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email
        };
        return Task.FromResult(userUpdated);
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        var existing = _users.FirstOrDefault(u => u.Id == id);
        if (existing == null) return Task.FromResult(false);
        _users.Remove(existing);
        return Task.FromResult(true);
    }
}
