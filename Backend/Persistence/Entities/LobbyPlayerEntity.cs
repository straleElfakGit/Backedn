using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Backend.Domain;

namespace Backend.Persistence.Entities
{
    public class LobbyPlayerEntity
    {
        [Key]
        public int ID {get; set;}

        [ForeignKey(nameof(Lobby))]
        public int LobbyId { get; set; }
        [JsonIgnore]
        public LobbyEntity Lobby { get; set; } = null!;

        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        [JsonIgnore]
        public UserEntity User { get; set; } = null!;
        [Required]
        public Color Color { get; set; }
        
        public int RolledNumber { get; set; }
        
        public bool IsReady { get; set; }
    }
}