using Application.ProductFeatures.Queries;
using AutoFixture;
using Domain.Entites;
using Persistence.Context;

namespace MovieManager.Core.Application.ProductFeatures.Tests.Queries
{
    public class GetAllMoviesQueryTests
    {
        private readonly DbContextDecorator<MovieManagerContext> _dbContext;

        protected readonly CancellationTokenSource _cts = new();

        public GetAllMoviesQueryTests()
        {
            var options = Utilities.CreateInMemoryDbOptions<MovieManagerContext>();

            _dbContext = new DbContextDecorator<MovieManagerContext>(options);
        }

        [TearDown]
        public void Cleanup()
        {
            _dbContext.Clear();
        }

        [Test]
        public void DataSet_ReturnsEmpty_WhenNoMovies()
        {
            _dbContext.Assert(async context =>
            {
                // Arrange
                var sut = CreateSut(context);

                //Act
                var results = await sut.Handle(new GetAllMoviesQuery(), _cts.Token);

                //Assert
                Assert.That(results.Count(), Is.EqualTo(0));
                Assert.That(!results.Any());
            });
        }

        [Test]
        public void DataSet_ReturnsCorrectRows()
        {
            var fixture = new Fixture();
            var movie1 = fixture.Build<Movie>().With(x => x.Id, 1).Create();
            var movie2 = fixture.Build<Movie>().With(x => x.Id, 2).Create();
            _dbContext.AddAndSaveRange(new List<Movie> { movie1, movie2 });

            _dbContext.Assert(async context =>
            {
                // Arrange
                var sut = CreateSut(context);

                //Act
                var results = await sut.Handle(new GetAllMoviesQuery(), _cts.Token);

                //Assert
                Assert.That(results, Is.Not.Null);
                Assert.That(results.Count, Is.EqualTo(2));

                var lastMovie = results.Last();
                Assert.Multiple(() =>
                {
                    Assert.That(lastMovie.Id, Is.EqualTo(movie2.Id));
                    Assert.That(lastMovie.Title, Is.EqualTo(movie2.Title));
                    Assert.That(lastMovie.Description, Is.EqualTo(movie2.Description));
                    Assert.That(lastMovie.ReleaseDate, Is.EqualTo(movie2.ReleaseDate));
                });
            });
        }

        private static GetAllMoviesQuery.GetAllMovieQueryHandler CreateSut(MovieManagerContext context) => new(context);
    }
}
