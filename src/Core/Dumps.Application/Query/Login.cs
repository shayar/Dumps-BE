using System.Net;
using Dumps.Application.DTO.Request.Auth;
using Dumps.Application.DTO.Response.Auth;
using Dumps.Application.Exceptions;
using Dumps.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Dumps.Application.Query;

public class Login
{
    public class LoginQuery: LoginRequest, IRequest<LoginResponse>
    {

    }

    public class LoginQueryValidator : AbstractValidator<LoginQuery>
    {
        public LoginQueryValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty();
        }
    }

    public class Handler : IRequestHandler<LoginQuery, LoginResponse>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public Handler(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<LoginResponse> Handle(LoginQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                throw new RestException(HttpStatusCode.Unauthorized, new { Error = "Invalid email or password" });
            }
            var result = await _signInManager.PasswordSignInAsync(user, request.Password, false, false);
            if (!result.Succeeded)
            {
                throw new RestException(HttpStatusCode.Unauthorized, new { Error = "Invalid email or password" });
            }

            return new LoginResponse(user);
        }
    }
}
