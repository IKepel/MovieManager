using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.ProductFeatures.Commands
{
    public class DeleteMovieByIdCommand : IRequest<int>
    {
        public int Id { get; set; }

        public class DeleteMovieByIdCommandHandler : IRequestHandler<DeleteMovieByIdCommand, int>
        {
            private readonly IMovieManagerDbContext _context;

            public DeleteMovieByIdCommandHandler(IMovieManagerDbContext context)
            {
                _context = context;
            }

            public async Task<int> Handle(DeleteMovieByIdCommand command, CancellationToken cancellationToken)
            {
               var movie = await _context.Movies.Where(m => m.Id == command.Id).FirstOrDefaultAsync(cancellationToken) ?? throw new Exception("Movie not found!");

                _context.Movies.Remove(movie);
                await _context.SaveChangesAsync(cancellationToken);

                return movie.Id;
            }
        }
    }
}
