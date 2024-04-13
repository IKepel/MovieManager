using Application.Interfaces;
using Domain.Entites;
using MediatR;

namespace Application.ProductFeatures.Commands
{
    public class CreateSessionCommand : IRequest<int>
    {
        public int MovieId { get; set; }

        public string RoomName { get; set; }

        public DateTime StartDateTime { get; set; }

        public class CreateSessionCommandHandler : IRequestHandler<CreateSessionCommand, int>
        {
            private readonly IMovieManagerDbContext _context;

            public CreateSessionCommandHandler(IMovieManagerDbContext context)
            {
                _context = context;
            }

            public async Task<int> Handle(CreateSessionCommand command, CancellationToken cancellationToken)
            {
                var session = new Session
                {
                    MovieId = command.MovieId,
                    RoomName = command.RoomName,
                    StartDateTime = command.StartDateTime
                };

                _context.Sessions.Add(session);
                await _context.SaveChangesAsync(cancellationToken);

                return session.Id;
            }
        }
    }
}
