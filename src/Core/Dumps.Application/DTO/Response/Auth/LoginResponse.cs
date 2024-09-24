using Dumps.Domain.Entities;

namespace Dumps.Application.DTO.Response.Auth;

public class LoginResponse
{
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Username { get; set; }

    public string Email { get; set; }

    public LoginResponse(ApplicationUser user)
    {
        Id = user.Id;
        FirstName = user.FirstName;
        LastName = user.LastName;
        Username = user.UserName;
        Email = user.Email;
    }
}
