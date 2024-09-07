using Dumps.Application.DTO.Request.ContactUs;
using Dumps.Domain.Entities;
using Dumps.Persistence.DbContext;
using MediatR;

namespace Dumps.Application.Command
{
    public class CreateContactUsCommand : ContactUsRequest, IRequest<int>
    {
    }

    public class Handler : IRequestHandler<CreateContactUsCommand, int>
    {
        private readonly AppDbContext _context;
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
