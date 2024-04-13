using Application.Interfaces;
using Domain.Entites;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Context
{
    public class MovieManagerContext : DbContext, IMovieManagerDbContext
    {
        public MovieManagerContext(DbContextOptions<MovieManagerContext> options) : base(options) { }

        public DbSet<Movie> Movies { get; set; }

        public DbSet<Session> Sessions { get; set; }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
