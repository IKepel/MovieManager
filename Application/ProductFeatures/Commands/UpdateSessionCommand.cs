using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.ProductFeatures.Commands
{
    public class UpdateSessionCommand : IRequest<int>
    {
        public int Id { get; set; }

        public int MovieId { get; set; }

        public string RoomName { get; set; }

        public DateTime StartDateTime { get; set; }

        public class UpdateSessionCommandHandler : IRequestHandler<UpdateSessionCommand, int>
        {
            private readonly IMovieManagerDbContext _context;

            public UpdateSessionCommandHandler(IMovieManagerDbContext context)
            {
                _context = context;
            }

            public async Task<int> Handle(UpdateSessionCommand command, CancellationToken cancellationToken)
            {
                var session = await _context.Sessions.Where(s => s.Id == command.Id).FirstOrDefaultAsync(cancellationToken) ?? throw new Exception("Session not found!");
                
                session.MovieId = command.MovieId;
                session.RoomName = command.RoomName;
                session.StartDateTime = command.StartDateTime;

                await _context.SaveChangesAsync(cancellationToken);

                return session.Id;
            }
        }
    }
}
