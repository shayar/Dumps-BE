using System.Net;
using Dumps.Application.DTO.Request.Account;
using Dumps.Application.Exceptions;
using Dumps.Application.ServicesInterfaces;
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
        private readonly IEmailService _emailService;

        public ForgotPasswordCommandHandler(UserManager<ApplicationUser> userManager, ILogger<ForgotPasswordCommandHandler> logger, IEmailService emailService)
        {
            _userManager = userManager;
            _logger = logger;
            _emailService = emailService;
        }

        public async Task<APIResponse<string>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                throw new RestException(HttpStatusCode.NotFound, "Email not found");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            // Send via email — implement your own EmailService
            _logger.LogInformation($"Generated token for {request.Email}: {token}");

            var resetUrl = $"https://yourfrontend.com/reset-password?email={WebUtility.UrlEncode(request.Email)}&token={WebUtility.UrlEncode(token)}";

            string content = $@"
            <p>Hello,</p>
            <p>Please reset your password by clicking on the link below:</p>
            <p><a href='{resetUrl}'>Reset Password</a></p>
            <p>If you didn't request this, please ignore this email.</p>";

            await _emailService.SendEmailAsync(request.Email, "Reset Password", content);

            return new APIResponse<string>("Reset token generated and email sent.", "Email Sent");
        }
    }

}
