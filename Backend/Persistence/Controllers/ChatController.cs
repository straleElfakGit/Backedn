using Backend.Domain;
using Srbopoly.Services.Messaging;
using Backend.Persistence.DTO;
using System.Data.Common;
using System.Data;

namespace Backend.Persistence.Controllers;

[ApiController]
[Route("chat")]
public class ChatController : ControllerBase
{
    private readonly IChatPublisher _chatPublisher;

    public ChatController(IChatPublisher chatPublisher)
    {
        _chatPublisher = chatPublisher;
    }

    [HttpPost("{gameId}")]
    public async Task<IActionResult> SendMessage(
        int gameId, 
        [FromBody] SendChatMessageRequest request)
    {
        if( string.IsNullOrWhiteSpace(request.Username) || 
            string.IsNullOrWhiteSpace(request.Text))
        {
            return BadRequest("Potrebno je da budu postavljeni i tekst i korisničko ime");
        }   
        var message = new ChatMessage
        {
            Username = request.Username,
            Text = request.Text,
            GameId = gameId,
            SentAt = DateTime.UtcNow
        };

        await _chatPublisher.PublishAsync(gameId, message);

        return NoContent();
    }
}