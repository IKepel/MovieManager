using Domain.Entites;
using Microsoft.EntityFrameworkCore;

namespace Application.Interfaces
{
    public interface IMovieManagerDbContext
    {
        DbSet<Movie> Movies { get; set; }

        DbSet<Session> Sessions { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
