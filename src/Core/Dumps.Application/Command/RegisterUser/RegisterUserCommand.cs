using Dumps.Application.DTO.Request.RegisterUser;
using Dumps.Domain.Common.Constants;
using Dumps.Domain.Entities;
using Dumps.Persistence.DbContext;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Dumps.Application.Command.RegisterUser
{
    public class RegisterUserCommand : RegisterUserRequest, IRequest<APIResponse<object>>
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
                    .MinimumLength(8).WithMessage("Password must be at least 8 characters long.");

            }
        }
    }
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, APIResponse<object>>
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

      

        public async Task<APIResponse<object>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {


            // Validating the request first
            var validator = new RegisterUserCommand.Validator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                return new APIResponse<object>(
                    null,
                    string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)),
                    false
                );

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
                        return new APIResponse<object>(
                           null,
                           string.Join("; ", result.Errors.Select(e => e.Description)),
                           false
                         );

                    }

                    //Adding role to the user
                    var roleResult = await _userManager.AddToRoleAsync(user, SD.Role_User);
                     if (!roleResult.Succeeded)
                     {
                        await transaction.RollbackAsync();
                        return new APIResponse<object>(
                          null,
                          string.Join("; ", roleResult.Errors.Select(e => e.Description)),
                          false
                         );
                    }

                    // Commit the transaction if user creation succeeds
                    await transaction.CommitAsync(cancellationToken);
                    return new APIResponse<object>(
                          new
                             {
                              User = new
                                     {
                                      user.Id,
                                      user.UserName,
                                      user.FirstName,
                                      user.LastName,
                                      user.Email
                                      }
                              },
                          "User registered successfully.",
                          true
                     );

                }
                catch (Exception ex)
                {
                    // Rollback the transaction in case of an error
                    await transaction.RollbackAsync(cancellationToken);

                    // Log the error
                    _logger.LogError(ex, "Error occurred while registering a new user.");

                    // Return a failed response with a general error message
                    return new APIResponse<object>(
                      null,
                      "An error occurred during user registration.",
                       false
                     );

                }
            }
          
        }

    }
}
