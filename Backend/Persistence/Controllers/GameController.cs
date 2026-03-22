using Backend.Persistence.Entities;
using Backend.Persistence.DTO;
using Backend.Domain;
using Backend.Repository;
using Backend.Persistence.Mappers;
using Backend.Services.GameCode;

namespace Backend.Persistence.Controllers;

[ApiController]
[Route("[controller]")]
public class GameController : ControllerBase
{
    private readonly IGameRepository _repository;
    private readonly GameCodeService _codeService;

    public GameController(IGameRepository repository, GameCodeService codeService)
    {
        _repository = repository;
        _codeService = codeService;
    }

    [HttpPost("CreateGame")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<GameDto>> CreateGame([FromBody] int maxTurns)
    {
        try
        {
            var game = new Game
            {
                Status = GameStatus.Paused,
                MaxTurns = maxTurns,
                CurrentTurn = 0,
                CurrentPlayerIndex = 0,
                GameBoard = new Board()
            };

            var createdGame = await _repository.CreateAsync(game);
            game.ID = createdGame.ID;

            var gameDto = GameMapper.ToEntity(createdGame);
            gameDto.AccessCode = _codeService.EncodeGameId(createdGame.ID);

            return Ok(gameDto);
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
    public async Task<ActionResult<GameDto>> GetById(int id)
    {
        try
        {
            var game = await _repository.GetByIdAsync(id);
            if (game == null)
                return NotFound($"Game sa ID {id} nije pronađen.");

            return Ok(GameMapper.ToEntity(game));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("GetAll")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<GameDto>>> GetAll()
    {
        try
        {
            var games = await _repository.GetAllAsync();
            return Ok(games.Select(GameMapper.ToEntity).ToList());
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
            var game = await _repository.GetByIdAsync(id);
            if (game == null)
                return NotFound($"Game sa ID {id} nije pronađen.");

            await _repository.DeleteAsync(game);
            return Ok($"Game sa ID {id} je uspešno obrisan.");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("GetByUserId/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<GameDto>>> GetByUserId(int userId)
    {
        try
        {
            var games = await _repository.GetGamesByUserIdAsync(userId);
            
            if (games == null || !games.Any())
                return NotFound($"Nisu pronađene igre za korisnika sa ID {userId}.");

            return Ok(games.Select(GameMapper.ToEntity).ToList());
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
    public async Task<ActionResult<GameDto>> GetByAccessCode(string accessCode)
    {
        try
        {
            var gameId = _codeService.DecodeGameCode(accessCode);
            if (gameId == null)
            {
                return BadRequest("Nevalidan pristupni kod.");
            }

            var game = await _repository.GetByIdAsync(gameId.Value);            
            if (game == null)
                return NotFound($"Igra sa pristupnim kodom {accessCode} nije pronađena.");

            var gameDto = GameMapper.ToEntity(game);
            gameDto.AccessCode = accessCode;

            return Ok(gameDto);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}