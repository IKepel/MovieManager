using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.ProductFeatures.Commands
{
    public class DeleteSessionByIdCommand : IRequest<int>
    {
        public int Id { get; set; }

        public class DeleteSessionByIdCommandHandler : IRequestHandler<DeleteSessionByIdCommand, int>
        {
            private readonly IMovieManagerDbContext _context;

            public DeleteSessionByIdCommandHandler(IMovieManagerDbContext context)
            {
                _context = context;
            }

            public async Task<int> Handle(DeleteSessionByIdCommand command, CancellationToken cancellationToken)
            {
                var session = await _context.Sessions.Where(s => s.Id == command.Id).FirstOrDefaultAsync(cancellationToken) ?? throw new Exception("Session not found!");

                _context.Sessions.Remove(session);
                await _context.SaveChangesAsync(cancellationToken);

                return session.Id;
            }
        }
    }
}
