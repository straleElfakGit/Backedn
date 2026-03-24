using Backend.Persistence.DTO;
using Backend.Repository;
using Backend.Persistence.Records;

namespace Backend.Persistence.Controllers;

[ApiController]
[Route("[controller]")]
public class LobbyPlayerController : ControllerBase
{
    private readonly LobbyPlayerRepository _repository;

    public LobbyPlayerController(LobbyPlayerRepository repository)
    {
        _repository = repository;
    }

    [HttpPost("Join")]
    public async Task<ActionResult<LobbyPlayerDTO>> Join([FromBody] JoinRequest request)
    {
        try
        {
            var player = await _repository.JoinLobbyAsync(request.AccessCode, request.UserId);
            return Ok(player);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpDelete("Leave/{id}")]
    public async Task<ActionResult> Leave(int id)
    {
        try
        {
            await _repository.DeletePlayerAsync(id);
            return Ok("Igrač je napustio lobi.");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("GetById/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LobbyPlayerDTO>> GetById(int id)
    {
        try
        {
            var player = await _repository.GetByIdAsync(id);
            
            if (player == null)
                return NotFound($"Igrač u lobiju sa ID {id} nije pronađen.");

            return Ok(player);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}