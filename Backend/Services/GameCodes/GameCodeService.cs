using HashidsNet;

namespace Backend.Services.GameCode;

public class GameCodeService
{
    private readonly Hashids _hashids;

    public GameCodeService(IConfiguration configuration)
    {
        var salt = configuration["GameSettings:HashIdSalt"];

        if (string.IsNullOrEmpty(salt) || salt == "PLACEHOLDER_SALT")
        {
            throw new InvalidOperationException("HashIdSalt nije ispravno konfigurisan u User Secrets ili appsettings.json!");
        }

        _hashids = new Hashids(salt, 6, "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789");
    }

    public string EncodeGameId(int gameId)
    {
        return _hashids.Encode(gameId);
    }
    public int? DecodeGameCode(string gameCode)
    {
        var decoded = _hashids.Decode(gameCode);
        if (decoded.Length > 0)
        {
            return decoded[0];
        }
        return null;
    }
}