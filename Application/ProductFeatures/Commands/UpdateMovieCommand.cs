using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.ProductFeatures.Commands
{
    public class UpdateMovieCommand : IRequest<int>
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime ReleaseDate { get; set; }

        public class UpdateMovieCommandHandler : IRequestHandler<UpdateMovieCommand, int>
        {
            private readonly IMovieManagerDbContext _context;

            public UpdateMovieCommandHandler(IMovieManagerDbContext context)
            {
                _context = context;
            }

            public async Task<int> Handle(UpdateMovieCommand command, CancellationToken cancellationToken)
            {
                var movie = await _context.Movies.Where(m => m.Id == command.Id).FirstOrDefaultAsync(cancellationToken) ?? throw new Exception("Movie not found!");

                movie.Title = command.Title;
                movie.Description = command.Description;
                movie.ReleaseDate = command.ReleaseDate;

                await _context.SaveChangesAsync();

                return movie.Id;
            }
        }
    }
}
