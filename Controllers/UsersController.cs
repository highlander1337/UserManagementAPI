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

    // GET api/users?pageNumber=1&pageSize=10
    [HttpGet]
    [ProducesResponseType(typeof(UserPagedResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            var result = await _repo.GetAllAsync(pageNumber, pageSize);
            if (result.TotalCount == 0) return NotFound(new { Message = "No users found in the system." });
            return Ok(result);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An unexpected error occurred." });
        }
    }

    // GET api/users/{id}
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(UserDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var user = await _repo.GetByIdAsync(id);
            if (user == null) return NotFound(new { Message = $"No user found with Id {id}." });
            return Ok(user);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An unexpected error occurred." });
        }
    }

    // POST api/users
    [HttpPost]
    [ProducesResponseType(typeof(UserCreateResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] UserCreateRequest dto)
    {
        try
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _repo.CreateAsync(dto);
            if (result == null) return BadRequest(new { Message = "Failed to create user. Email may already exist." });
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An unexpected error occurred." });
        }
    }

    // PUT api/users/{id}
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(UserUpdateResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UserUpdateRequest dto)
    {
        try
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updated = await _repo.UpdateAsync(id, dto);
            if (updated == null) return BadRequest(new { Message = "Failed to update user. User may not exist or email is already in use." });
            return Ok(updated);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An unexpected error occurred." });
        }
    }

    // DELETE api/users/{id}
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var deleted = await _repo.DeleteAsync(id);
            if (!deleted) return BadRequest(new { Message = "Failed to delete user." });
            return NoContent();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An unexpected error occurred." });
        }
    }
}
