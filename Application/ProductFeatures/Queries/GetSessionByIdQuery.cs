using Application.Interfaces;
using Domain.Entites;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.ProductFeatures.Queries
{
    public class GetSessionByIdQuery : IRequest<Session>
    {
        public int Id { get; set; }

        public class GetSessionByIdQueryHandler : IRequestHandler<GetSessionByIdQuery, Session>
        {
            private readonly IMovieManagerDbContext _context;

            public GetSessionByIdQueryHandler(IMovieManagerDbContext context)
            {
                _context = context;
            }

            public async Task<Session> Handle(GetSessionByIdQuery query, CancellationToken cancellationToken)
            {
                return await _context.Sessions
                    .AsNoTracking()
                    .Include(x => x.Movie)
                    .Where(s => s.Id == query.Id)
                    .Select(x => new Session
                    {
                        Id = x.Id,
                        MovieId = x.MovieId,
                        RoomName = x.RoomName,
                        StartDateTime = x.StartDateTime,
                        Movie = new Movie
                        {
                            Id = x.Movie.Id,
                            Title = x.Movie.Title,
                            Description = x.Movie.Description,
                            ReleaseDate = x.Movie.ReleaseDate
                        }
                    })
                    .FirstOrDefaultAsync(cancellationToken) ?? throw new Exception("Session not found!");
            }
        }
    }
}
