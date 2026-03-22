using UserManagementAPI.Models.User;

namespace UserManagementAPI.Services;

public class UserPagedResult
{
    public IEnumerable<UserListResponse> Items { get; set; } = Enumerable.Empty<UserListResponse>();
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
}
