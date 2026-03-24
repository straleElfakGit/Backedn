using Backend.Persistence.DTO;
using Backend.Persistence.Entities;

namespace Backend.Persistence.Mappers
{
    public static class LobbyMapperDE
    {
        public static LobbyDTO ToDto(LobbyEntity entity, string accessCode)
        {
            return new LobbyDTO
            {
                Id = entity.ID,
                HostUserId = entity.HostUserId,
                AccessCode = accessCode,
                Players = entity.Players != null 
                ? entity.Players.Select(p => LobbyPlayerMapperDE.ToDto(p)).ToList() 
                    : new List<LobbyPlayerDTO>()
            };
        }

        public static LobbyEntity ToEntity(LobbyDTO dto)
        {
            return new LobbyEntity
            {
                ID = dto.Id,
                HostUserId = dto.HostUserId,
                Players = new List<LobbyPlayerEntity>()
            };
        }
    }
}
