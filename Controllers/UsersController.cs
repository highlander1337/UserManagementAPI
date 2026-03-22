using Microsoft.AspNetCore.Mvc;
using UserManagementAPI.Models.User;
using UserManagementAPI.Services;

namespace UserManagementAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _repo;

    public UsersController(IUserRepository repo)
    {
        _repo = repo;
    }

    // GET api/users
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UserListResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAll()
    {
        var users = await _repo.GetAllAsync();
        if (!users.Any()) return NotFound(new { Message = "No users found in the system." });
        return Ok(users);
    }

    // GET api/users/{id}
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(UserDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var user = await _repo.GetByIdAsync(id);
        if (user == null) return NotFound(new { Message = $"No user found with Id {id}." });
        return Ok(user);
    }

    // POST api/users
    [HttpPost]
    [ProducesResponseType(typeof(UserCreateResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] UserCreateRequest dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _repo.CreateAsync(dto);
        if (result == null) return BadRequest(new { Message = "Failed to create user. Email may already exist." });
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    // PUT api/users/{id}
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(UserUpdateResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UserUpdateRequest dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var updated = await _repo.UpdateAsync(id, dto);
        if (updated == null) return BadRequest(new { Message = "Failed to update user. User may not exist or email is already in use." });
        return Ok(updated);
    }

    // DELETE api/users/{id}
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _repo.DeleteAsync(id);
        if (!deleted) return BadRequest(new { Message = "Failed to delete user." });
        return NoContent();
    }
}
