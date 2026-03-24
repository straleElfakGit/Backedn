using Backend.Domain;
using Backend.Persistence.Entities;
using Backend.Persistence.DTO;
using Backend.Persistence.Mappers;
using Backend.Services.GameCode;

namespace Backend.Repository;

public class LobbyPlayerRepository
{
    private readonly SrbopolyContext _context;
    private readonly GameCodeService _codeService;

    public LobbyPlayerRepository(SrbopolyContext context, GameCodeService codeService)
    {
        _context = context;
        _codeService = codeService;
    }

    public async Task<LobbyPlayerDTO> JoinLobbyAsync(string accessCode, int userId)
    {
        var lobby = await _context.Lobbies
            .Include(l => l.Players)
            .FirstOrDefaultAsync(l => l.ID == _codeService.DecodeGameCode(accessCode));

        if (lobby == null)
            throw new KeyNotFoundException("Lobi sa tim kodom ne postoji.");

        if (lobby.Players.Count >= 4)
            throw new InvalidOperationException("Lobi je već popunjen (maksimalno 4 igrača).");

        if (lobby.Players.Any(p => p.UserId == userId))
            throw new InvalidOperationException("Korisnik je već u ovom lobiju.");

        var takenColors = lobby.Players.Select(p => p.Color).ToList();
        var assignedColor = GetFirstAvailableColor(takenColors);

        var newPlayer = new LobbyPlayerEntity
        {
            LobbyId = lobby.ID,
            UserId = userId,
            Color = assignedColor,
            RolledNumber = 0,
            IsReady = false
        };

        _context.LobbyPlayers.Add(newPlayer);
        await _context.SaveChangesAsync();
        await _context.Entry(newPlayer).Reference(p => p.User).LoadAsync();

        return LobbyPlayerMapperDE.ToDto(newPlayer);
    }

    public async Task DeletePlayerAsync(int lobbyPlayerId)
    {
        var player = await _context.LobbyPlayers
            .Include(p => p.Lobby)
            .FirstOrDefaultAsync(p => p.ID == lobbyPlayerId);
        if (player == null) throw new KeyNotFoundException($"Igrac sa id-jem {lobbyPlayerId} ne postoji.");;

        var lobby = player.Lobby;
        var departingUserId = player.UserId;

        _context.LobbyPlayers.Remove(player);
        await _context.SaveChangesAsync();

        var remainingPlayers = await _context.LobbyPlayers
            .Where(p => p.LobbyId == lobby.ID)
            .ToListAsync();

        if (remainingPlayers.Count == 0)
        {
            _context.Lobbies.Remove(lobby);
            await _context.SaveChangesAsync();
        }
        else
        {
            if (lobby.HostUserId == departingUserId)
            {
                var newHost = remainingPlayers.First();
                lobby.HostUserId = newHost.UserId;
                
                await _context.SaveChangesAsync();
            }
        }
    }

    public async Task<LobbyPlayerDTO?> GetByIdAsync(int id)
    {
        var entity = await _context.LobbyPlayers
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.ID == id);

        if (entity == null)
            return null;

        return LobbyPlayerMapperDE.ToDto(entity);
    }

    private Color GetFirstAvailableColor(List<Color> takenColors)
    {
        foreach (Color color in Enum.GetValues(typeof(Color)))
        {
            if (!takenColors.Contains(color))
            {
                return color;
            }
        }
        throw new InvalidOperationException("Nema slobodnih boja (što ne bi trebalo da se desi zbog provere broja igrača).");
    }
}