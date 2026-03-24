namespace Backend.Persistence.Entities
{
    public class LobbyEntity
    {
        [Key]
        public int ID {get; set;}

        public int HostUserId { get; set; }

        public List<LobbyPlayerEntity> Players { get; set; } = new List<LobbyPlayerEntity>();
    }
}