using Application.Interfaces;
using Domain.Entites;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.ProductFeatures.Queries
{
    public class GetMovieByIdQuery : IRequest<Movie>
    {
        public int Id { get; set; }

        public class GetMovieByIdQueryHandler : IRequestHandler<GetMovieByIdQuery, Movie>
        {
            private readonly IMovieManagerDbContext _context;

            public GetMovieByIdQueryHandler(IMovieManagerDbContext context)
            {
                _context = context;
            }

            public async Task<Movie> Handle(GetMovieByIdQuery query, CancellationToken cancellationToken)
            {
                return await _context.Movies.AsNoTracking().Where(m => m.Id == query.Id).FirstOrDefaultAsync(cancellationToken);
            }
        }
    }
}
