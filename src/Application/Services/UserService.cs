using Application.DTOs;
using Core.Interfaces;

namespace Application.Services;

public class UserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<UserDTO>> GetAllUserAsync()
    {
        var users = await _userRepository.GetAllUsersAsync();
        var userDtos = users.Select(user => new UserDTO
        {
            Id = user.id,
            Name = user.name,
            Email = user.email
        });
        return userDtos;
    }
}