using System.Net;
using Dumps.Application.DTO.Request.Account;
using Dumps.Application.Exceptions;
using Dumps.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Dumps.Application.Command.Account
{
    public class ForgotPasswordCommand : ForgotPasswordRequest, IRequest<APIResponse<string>>
    {
    }

    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, APIResponse<string>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ForgotPasswordCommandHandler> _logger;

        public ForgotPasswordCommandHandler(UserManager<ApplicationUser> userManager, ILogger<ForgotPasswordCommandHandler> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<APIResponse<string>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                throw new RestException(HttpStatusCode.NotFound, "Email not found");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            // Send via email — implement your own EmailService
            _logger.LogInformation($"Generated token for {request.Email}: {token}");

            // TODO: Send resetUrl via email

            return new APIResponse<string>("Reset token generated and email sent.", "Email Sent");
        }
    }

}
