using System.Net;
using Dumps.Application.DTO.Request.Account;
using Dumps.Application.Exceptions;
using Dumps.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Dumps.Application.Command.Account
{
    public class ResetPasswordCommand : ResetPasswordRequest, IRequest<APIResponse<string>>
    {
    }

    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, APIResponse<string>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ResetPasswordCommandHandler> _logger;

        public ResetPasswordCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<APIResponse<string>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                throw new RestException(HttpStatusCode.NotFound, "User not found");

            var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);

            if (!result.Succeeded)
            {
                var errorMsg = string.Join("; ", result.Errors.Select(e => e.Description));
                _logger.LogInformation($"Reset Password Failed for {request.Email}: {errorMsg}");
                throw new RestException(HttpStatusCode.BadRequest, errorMsg);
            }

            return new APIResponse<string>("Password reset successful.", "Password changed");
        }
    }

}
