using Backend.Persistence.DTO;
using Backend.Persistence.Entities;

namespace Backend.Persistence.Mappers
{
    public static class LobbyPlayerMapperDE
    {
        public static LobbyPlayerDTO ToDto(LobbyPlayerEntity entity)
        {
            return new LobbyPlayerDTO
            {
                Id = entity.ID,
                UserId = entity.UserId,
                Username = entity.User?.Username ?? "Unknown",
                LobbyId = entity.LobbyId,
                Color = entity.Color,
                RolledNumber = entity.RolledNumber,
                IsReady = entity.IsReady
            };
        }

        public static LobbyPlayerEntity ToEntity(LobbyPlayerDTO dto)
        {
            return new LobbyPlayerEntity
            {
                ID = dto.Id,
                UserId = dto.UserId,
                LobbyId = dto.LobbyId,
                Color = dto.Color,
                RolledNumber = dto.RolledNumber,
                IsReady = dto.IsReady
            };
        }
    }
}
