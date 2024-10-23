using Dumps.Application.DTO.Request.RegisterUser;
using Dumps.Domain.Entities;
using Dumps.Persistence.DbContext;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Dumps.Application.Command.RegisterUser
{
    public class RegisterUserCommand : RegisterUserRequest, IRequest<IdentityResult>
    {
        //  Validator for register user
        public class Validator : AbstractValidator<RegisterUserCommand>
        {
            public Validator()
            {
                RuleFor(x => x.Email)
                    .NotEmpty().WithMessage("Email is required.")
                    .EmailAddress().WithMessage("Invalid email format.");

                RuleFor(x => x.Password)
                    .NotEmpty().WithMessage("Password is required.")
                    .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");

                RuleFor(x => x.ConfirmPassword)
                    .Equal(x => x.Password).WithMessage("Passwords do not match.");
            }
        }
    }
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, IdentityResult>
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterUserCommandHandler> _logger;
        public RegisterUserCommandHandler(AppDbContext context, UserManager<ApplicationUser> userManager, ILogger<RegisterUserCommandHandler> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

      

        public async Task<IdentityResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {


            // Validating the request first
            var validator = new RegisterUserCommand.Validator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                return IdentityResult.Failed(validationResult.Errors
                    .Select(e => new IdentityError { Description = e.ErrorMessage })
                    .ToArray());
            }

            // Starting a  database transaction
            using (var transaction = await _context.Database.BeginTransactionAsync(cancellationToken))
            {
                try
                {    
                    // Create a new user instance
                    var user = new ApplicationUser
                    {
                        UserName = request.Email,
                        FirstName = request.FirstName,
                        LastName = request.LastName,
                        Email = request.Email
                    };

                    // Create the user using UserManager (this also hashes the password)
                    var result = await _userManager.CreateAsync(user, request.Password);

                    // If user creation fails, return the result with errors
                    if (!result.Succeeded)
                    {
                        return result;
                    }

                     //Adding role to the user
                     var roleResult = await _userManager.AddToRoleAsync(user, "User");
                     if (!roleResult.Succeeded)
                     {
                        await transaction.RollbackAsync();
                         return roleResult;
                     }

                    // Commit the transaction if user creation succeeds
                    await transaction.CommitAsync(cancellationToken);
                    return IdentityResult.Success;
                }
                catch (Exception ex)
                {
                    // Rollback the transaction in case of an error
                    await transaction.RollbackAsync(cancellationToken);

                    // Log the error
                    _logger.LogError(ex, "Error occurred while registering a new user.");

                    // Return a failed IdentityResult with a general error
                    return IdentityResult.Failed(new IdentityError { Description = "An error occurred during user registration." });
                }
            }
          
        }

    }
}
