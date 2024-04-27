using Application.ProductFeatures.Queries;
using AutoFixture;
using Domain.Entites;
using Persistence.Context;

namespace MovieManager.Core.Application.ProductFeatures.Tests.Queries
{
    public class GetMovieByIdQueryTests
    {
        private readonly DbContextDecorator<MovieManagerContext> _dbContext;

        protected readonly CancellationTokenSource _cts = new();

        public GetMovieByIdQueryTests()
        {
            var options = Utilities.CreateInMemoryDbOptions<MovieManagerContext>();

            _dbContext = new DbContextDecorator<MovieManagerContext>(options);
        }

        [Test]
        public void Query_ReturnsCorrectMovie()
        {
            
            var fixture = new Fixture();
            var movie1 = fixture.Build<Movie>().With(x => x.Id, 1).Create();
            var movie2 = fixture.Build<Movie>().With(x => x.Id, 2).Create();
            _dbContext.AddAndSaveRange(new List<Movie> { movie1, movie2 });

            _dbContext.Assert(async context =>
            {
                // Arrange
                var sut = CreateSut(context);
                var query = new GetMovieByIdQuery { Id = 2 };

                // Act
                var result = await sut.Handle(query, _cts.Token);

                // Assert
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Id, Is.EqualTo(movie2.Id));
                Assert.That(result.Title, Is.EqualTo(movie2.Title));
                Assert.That(result.Description, Is.EqualTo(movie2.Description));
                Assert.That(result.ReleaseDate, Is.EqualTo(movie2.ReleaseDate));
            });
        }

        [Test]
        public void Query_ReturnsNull_WhenMovieDoesNotExist()
        {
            _dbContext.Assert(async context =>
            {
                // Arrange
                var sut = CreateSut(context);
                var query = new GetMovieByIdQuery { Id = 999 };

                // Act
                var result = await sut.Handle(query, _cts.Token);

                // Assert
                Assert.That(result, Is.Null);
            });
        }

        private static GetMovieByIdQuery.GetMovieByIdQueryHandler CreateSut(MovieManagerContext context) => new(context);
    }
}
