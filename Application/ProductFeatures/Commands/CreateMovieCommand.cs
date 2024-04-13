using Application.Interfaces;
using Domain.Entites;
using MediatR;

namespace Application.ProductFeatures.Commands
{
    public class CreateMovieCommand : IRequest<int>
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime ReleaseDate { get; set; }

        public class CreateMovieCommandHandler : IRequestHandler<CreateMovieCommand, int>
        {
            private readonly IMovieManagerDbContext _context;

            public CreateMovieCommandHandler(IMovieManagerDbContext context)
            {
                _context = context;
            }

            public async Task<int> Handle(CreateMovieCommand command, CancellationToken cancellationToken)
            {
                var movie = new Movie
                {
                    Title = command.Title,
                    Description = command.Description,
                    ReleaseDate = command.ReleaseDate
                };

                _context.Movies.Add(movie);
                await _context.SaveChangesAsync(cancellationToken);

                return movie.Id;
            }
        }
    }
}
