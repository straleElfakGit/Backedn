using Backend.Domain;

namespace Backend.Persistence.DTO;

public class LobbyDTO
{
    public int Id { get; set; }
    public int HostUserId { get; set; }
    public string AccessCode { get; set; } = string.Empty;
    public List<LobbyPlayerDTO> Players { get; set; } = new List<LobbyPlayerDTO>();
}