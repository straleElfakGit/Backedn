using Backend.Domain;
using Backend.Persistence.Entities;
using Backend.Persistence.Mappers;
using Backend.Persistence.DTO;
using Backend.Services.GameCode;

namespace Backend.Repository;

public class LobbyRepository
{
    private readonly SrbopolyContext _context;
    private readonly GameCodeService _codeService;

    public LobbyRepository(SrbopolyContext context, GameCodeService codeService)
    {
        _context = context;
        _codeService = codeService;
    }

    public async Task<LobbyDTO> CreateLobbyAsync(int hostUserId)
    {
        var userEntity = await _context.Users.FindAsync(hostUserId);
        if (userEntity == null)
            throw new KeyNotFoundException($"User sa ID {hostUserId} ne postoji.");

        var lobby = new LobbyEntity { HostUserId = hostUserId };
        _context.Lobbies.Add(lobby);
        await _context.SaveChangesAsync();

        var lobbyDTO = LobbyMapperDE.ToDto(lobby, _codeService.EncodeGameId(lobby.ID));
        return lobbyDTO;
    }

    public async Task<LobbyDTO?> GetByIdAsync(int id)
    {
        var entity = await _context.Lobbies
            .FirstOrDefaultAsync(l => l.ID == id);

        if (entity == null)
            return null;

        var dto = LobbyMapperDE.ToDto(entity, _codeService.EncodeGameId(id));

        return dto;
    }

    public async Task<LobbyDTO?> GetByAccessCodeAsync(string accessCode)
    {
        var id = _codeService.DecodeGameCode(accessCode);
        if (id == null) 
            return null;

        var entity = await _context.Lobbies
            .Include(l => l.Players)
                .ThenInclude(p => p.User)
            .FirstOrDefaultAsync(l => l.ID == id);

        if (entity == null) 
            return null;

        return LobbyMapperDE.ToDto(entity, accessCode);
    }

    public async Task<List<LobbyDTO>> GetAllAsync()
    {
        var entities = await _context.Lobbies
            .ToListAsync();

        return entities.Select(e => 
            LobbyMapperDE.ToDto(e, _codeService.EncodeGameId(e.ID))
        ).ToList();
    }

    public async Task DeleteAsync(string accessCode)
    {
        var id = _codeService.DecodeGameCode(accessCode);
        if (id == null) throw new ArgumentException("Nevalidan kod.");

        var lobby = await _context.Lobbies.FindAsync(id.Value);
        if (lobby != null)
        {
            _context.Lobbies.Remove(lobby);
            await _context.SaveChangesAsync();
        }
    }
}