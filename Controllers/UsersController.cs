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
    [ProducesResponseType(typeof(UserListResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAll()
    {
        var users = await _repo.GetAllAsync();
        if (!users.Any()) return NotFound(new { Message = "No users found in the system." });
        return Ok(users);
    }

    // GET api/users/{id}
    [HttpGet("{id:guid}")]

    // how to use swagger to show the possible responses for this endpoint? for example, if the user is not found, it should return a 404 with a message. If the user is found, it should return a 200 with the user details. How can I document this in swagger so that it's clear to API consumers? 
    // To document the possible responses for this endpoint in Swagger, you can use the [ProducesResponseType] attribute to specify the different HTTP status codes and their corresponding response types. In this case, you can indicate that a 200 OK response will return a user detail object, while a 404 Not Found response will return an error message. Here's how you can do it: 
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
    [ProducesResponseType(typeof(UserCreateResonse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] UserCreateRequest dto)
    {

        var result = await _repo.CreateAsync(dto);
        if (result == null) return BadRequest(new {Message = "Failed to create user." });
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, dto);
    }

    // PUT api/users/{id}
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(UserUpdateResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UserUpdateRequest dto)
    {
        var updated = await _repo.UpdateAsync(id, dto);
        if (updated == null) return BadRequest(new { Message = "Failed to update user." });
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
