using UserManagementAPI.Entities;
using UserManagementAPI.Models.User;

namespace UserManagementAPI.Services;

public class InMemoryUserRepository : IUserRepository
{
    private readonly List<Users> _users = new();
    private readonly object _lock = new();

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
        lock (_lock)
        {
            var userList = _users.Select(u => new UserListResponse
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email
            }).ToList();

            return Task.FromResult<IEnumerable<UserListResponse>>(userList);
        }
    }

    public Task<UserDetailResponse?> GetByIdAsync(Guid id)
    {
        lock (_lock)
        {
            var user = _users.FirstOrDefault(u => u.Id == id);
            if (user == null) return Task.FromResult<UserDetailResponse?>(null);

            var userDetail = new UserDetailResponse
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email
            };
            return Task.FromResult<UserDetailResponse?>(userDetail);
        }
    }

    public Task<UserCreateResponse?> CreateAsync(UserCreateRequest user)
    {
        var newUser = new Users
        {
            Id = Guid.NewGuid(),
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email
        };

        lock (_lock)
        {
            // simple duplicate email check
            if (_users.Any(u => string.Equals(u.Email, newUser.Email, StringComparison.OrdinalIgnoreCase)))
                return Task.FromResult<UserCreateResponse?>(null);

            _users.Add(newUser);
        }

        var response = new UserCreateResponse
        {
            Id = newUser.Id,
            FirstName = newUser.FirstName,
            LastName = newUser.LastName,
            Email = newUser.Email
        };

        return Task.FromResult<UserCreateResponse?>(response);
    }

    public Task<UserUpdateResponse?> UpdateAsync(Guid id, UserUpdateRequest user)
    {
        lock (_lock)
        {
            var existing = _users.FirstOrDefault(u => u.Id == id);
            if (existing == null) return Task.FromResult<UserUpdateResponse?>(null);

            // check for email conflict
            if (_users.Any(u => u.Id != id && string.Equals(u.Email, user.Email, StringComparison.OrdinalIgnoreCase)))
                return Task.FromResult<UserUpdateResponse?>(null);

            existing.FirstName = user.FirstName;
            existing.LastName = user.LastName;
            existing.Email = user.Email;

            var userUpdated = new UserUpdateResponse
            {
                FirstName = existing.FirstName,
                LastName = existing.LastName,
                Email = existing.Email
            };

            return Task.FromResult<UserUpdateResponse?>(userUpdated);
        }
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        var existing = _users.FirstOrDefault(u => u.Id == id);
        if (existing == null) return Task.FromResult(false);
        _users.Remove(existing);
        return Task.FromResult(true);
    }
}
