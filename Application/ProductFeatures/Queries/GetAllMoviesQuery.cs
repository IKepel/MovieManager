using Application.Interfaces;
using Domain.Entites;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.ProductFeatures.Queries
{
    public class GetAllMoviesQuery : IRequest<IEnumerable<Movie>>
    {
        public class GetAllMovieQueryHandler : IRequestHandler<GetAllMoviesQuery, IEnumerable<Movie>>
        {
            private readonly IMovieManagerDbContext _context;

            public GetAllMovieQueryHandler(IMovieManagerDbContext context)
            {
                _context = context;
            }

            public async Task<IEnumerable<Movie>> Handle(GetAllMoviesQuery query, CancellationToken cancellationToken)
            {
                var moviesList = await _context.Movies.AsNoTracking().ToListAsync(cancellationToken);

                return moviesList?.AsReadOnly();
            }
        }
    }
}
