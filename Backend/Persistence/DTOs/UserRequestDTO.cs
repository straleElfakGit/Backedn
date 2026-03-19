namespace Backend.Persistence.DTO;

public class UserRequestDTO
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class CreateUserRequestDTO : UserRequestDTO { }

public class LoginUserRequestDTO : UserRequestDTO { }