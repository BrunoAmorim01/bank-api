namespace api.Presentation.Controllers.Dtos.Users;

public class RegisterUserDto
{
    public required string Name { get; set; }
    public required string Password { get; set; }
    public required string PasswordConfirmation { get; set; }
    public required string Email { get; set; }
}
