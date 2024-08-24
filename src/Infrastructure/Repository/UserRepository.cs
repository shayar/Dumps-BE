using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.Repository;

public class UserRepository: IUserRepository
{
    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        var users = new List<User>();
        users.Add(new User
        {
            id = Guid.NewGuid(),
            name = "John Doe",
            email = "johndoe@gmail.com",
        });
        return users;
    }
}