using Backend.Domain;
using Backend.Persistence.DTO;
using Backend.Persistence.Records;
using Backend.Repositories;
using Backend.Persistence.Mappers;
using Backend.Services.GameCode;

namespace Backend.Persistence.Controllers;

[ApiController]
[Route("[controller]")]
public class PlayerController : ControllerBase
{
    private readonly PlayerRepository _repository;
    private readonly GameCodeService _codeService;

    public PlayerController(PlayerRepository repository, GameCodeService codeService)
    {
        _repository = repository;
        _codeService = codeService;
    }

    [HttpPost("JoinGame")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PlayerDto>> JoinGame([FromBody] JoinGameRequest request)
    {
        try
        {
            var gameId = _codeService.DecodeGameCode(request.Accesscode);
            
            if (gameId == null)
                return BadRequest("Nevalidan pristupni kod.");

            var createPlayerRequest = new CreatePlayerRequest(
                request.UserId,
                gameId.Value,
                request.Balance,
                request.Position,
                request.Color,
                request.IsInJail
            );
            var player = await _repository.CreatePlayerAsync(createPlayerRequest);
            
            var playerDto = PlayerMapper.ToDTO(player);
            playerDto.UserId = request.UserId;
            playerDto.Username = player?.Username ?? "";
            
            return Ok(playerDto);
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

    [HttpPost("CreatePlayer")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CreatePlayer([FromBody] CreatePlayerRequest request)
    {
        try
        {
            var player = await _repository.CreatePlayerAsync(request);
            var playerDto = PlayerMapper.ToDTO(player);
            playerDto.UserId = request.UserId;
            playerDto.Username = player?.Username ?? "";
            return Ok(playerDto);
        }
        catch (KeyNotFoundException knf)
        {
            return NotFound(knf.Message);
        }
        catch (InvalidOperationException ioe)
        {
            return BadRequest(ioe.Message);
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
    public async Task<ActionResult> GetById(int id)
    {
        try
        {
            var player = await _repository.GetByIdAsync(id);
            if (player == null) return NotFound($"Player sa ID {id} nije pronađen.");

            return Ok(PlayerMapper.ToDTO(player));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("GetAll")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> GetAll()
    {
        try
        {
            var players = await _repository.GetAllAsync();
            return Ok(players.Select(PlayerMapper.ToDTO).ToList());
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpDelete("Delete/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            await _repository.DeleteAsync(id);
            return Ok($"Player sa ID {id} je uspešno obrisan.");
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
}