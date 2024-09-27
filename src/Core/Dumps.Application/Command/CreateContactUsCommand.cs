using Dumps.Application.DTO.Request.ContactUs;
using Dumps.Domain.Entities;
using Dumps.Persistence.DbContext;
using FluentValidation;
using MediatR;

namespace Dumps.Application.Command
{
    public class CreateContactUsCommand : ContactUsRequest, IRequest<int>
    {
    }

    public class Handler : IRequestHandler<CreateContactUsCommand, int>
    {
        private readonly AppDbContext _context;

        public class CreateContactUsCommandValidator : AbstractValidator<CreateContactUsCommand>
        {
            public CreateContactUsCommandValidator()
            {
                RuleFor(x => x.Name).NotEmpty();
                RuleFor(x => x.Email).NotEmpty().EmailAddress();
                RuleFor(x => x.Message).NotEmpty();
            }
        }
        public Handler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(CreateContactUsCommand command, CancellationToken cancellationToken)
        {
            var contactUs = new ContactUs
            {
                Name = command.Name,
                Email = command.Email,
                Message = command.Message
            };

            _context.ContactUsMessages.Add(contactUs);
            await _context.SaveChangesAsync(cancellationToken);
            return contactUs.Id;
        }
    }
}
