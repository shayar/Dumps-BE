using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dumps.Application.DTO.Request.RegisterUser;
using Dumps.Domain.Entities;
using Dumps.Persistence.DbContext;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Dumps.Application.Command.RegisterUser
{
    public class RegisterUserCommand : RegisterUserRequest, IRequest<IdentityResult>
    {

    }
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, IdentityResult>
    {
        private readonly AppDbContext _context;
        private readonly IPasswordHasher<ApplicationUser> _passwordHasher;
        public RegisterUserCommandHandler(AppDbContext context, IPasswordHasher<ApplicationUser> passwordHasher)
        {
            _passwordHasher = passwordHasher;
            _context = context;
        }
        public async Task<IdentityResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {

            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (existingUser != null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Email is already taken." });
            }

            // Create new user
            var user = new ApplicationUser
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
            };

            // Hash the password
            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

            // Add user to the database
            _context.Users.Add(user);
            await _context.SaveChangesAsync(cancellationToken);

            return IdentityResult.Success;

        }
    }
}
