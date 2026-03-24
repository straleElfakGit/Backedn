using Backend.Persistence.DTO;
using Backend.Repository;

namespace Backend.Persistence.Controllers;

[ApiController]
[Route("[controller]")]
public class LobbyController : ControllerBase
{
    private readonly LobbyRepository _repository;

    public LobbyController(LobbyRepository repository)
    {
        _repository = repository;
    }

    [HttpPost("CreateLobby")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LobbyDTO>> CreateLobby([FromBody] int hostUserId)
    {
        try
        {
            var lobbyDto = await _repository.CreateLobbyAsync(hostUserId);
            return Ok(lobbyDto);
        }
        catch (KeyNotFoundException knf)
        {
            return NotFound(knf.Message);
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
    public async Task<ActionResult<LobbyDTO>> GetById(int id)
    {
        try
        {
            var lobby = await _repository.GetByIdAsync(id);
            if (lobby == null)
                return NotFound($"Lobi sa ID {id} nije pronađen.");

            return Ok(lobby);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("GetByAccessCode/{accessCode}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LobbyDTO>> GetByAccessCode(string accessCode)
    {
        try
        {
            var lobby = await _repository.GetByAccessCodeAsync(accessCode);
            if (lobby == null)
                return NotFound($"Lobi sa pristupnim kodom {accessCode} nije pronađen.");

            return Ok(lobby);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("GetAll")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<LobbyDTO>>> GetAll()
    {
        try
        {
            var lobbies = await _repository.GetAllAsync();
            return Ok(lobbies);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpDelete("Delete/{accessCode}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Delete(string accessCode)
    {
        try
        {
            await _repository.DeleteAsync(accessCode);
            return Ok($"Lobi sa kodom {accessCode} je uspešno obrisan.");
        }
        catch (ArgumentException ae)
        {
            return BadRequest(ae.Message);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}