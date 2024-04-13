using Application.Interfaces;
using Domain.Entites;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.ProductFeatures.Queries
{
    public class GetAllSessionsQuery : IRequest<IEnumerable<Session>>
    {
        public class GetAllSessionsQueryHandler : IRequestHandler<GetAllSessionsQuery, IEnumerable<Session>>
        {
            private readonly IMovieManagerDbContext _context;

            public GetAllSessionsQueryHandler(IMovieManagerDbContext context)
            {
                _context = context;
            }

            public async Task<IEnumerable<Session>> Handle(GetAllSessionsQuery query, CancellationToken cancellationToken)
            {
                var sessionList = await _context.Sessions
                    .AsNoTracking()
                    .Include(x => x.Movie)
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
                    .ToListAsync(cancellationToken);

                return sessionList?.AsReadOnly();
            }
        }
    }
}
