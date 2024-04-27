using Application.ProductFeatures.Commands;
using AutoFixture;
using Domain.Entites;
using Persistence.Context;

namespace MovieManager.Core.Application.ProductFeatures.Tests.Commands
{
    public class UpdateMovieCommandTests
    {
        private readonly DbContextDecorator<MovieManagerContext> _dbContext;

        protected readonly CancellationTokenSource _cts = new();

        public UpdateMovieCommandTests()
        {
            var options = Utilities.CreateInMemoryDbOptions<MovieManagerContext>();

            _dbContext = new DbContextDecorator<MovieManagerContext>(options);
        }

        [SetUp]
        public void Init()
        {
            var fixture = new Fixture();
            var movie = fixture.Build<Movie>().With(x => x.Id, 1).Create();
            _dbContext.AddAndSave(movie);
        }

        [TearDown]
        public void Cleanup()
        {
            _dbContext.Clear();
        }

        [Test]
        public void Command_UpdatesMovie()
        {
            var fixture = new Fixture();
            var command = fixture.Build<UpdateMovieCommand>().With(x => x.Id, 1).Create();

            _dbContext.Assert(async context =>
            {
                //Arrange
                var sut = CreateSut(context);

                //Act
                var movieId = await sut.Handle(command, _cts.Token);

                // Assert
                _dbContext.Assert(context =>
                {
                    var movie = context.Movies.FirstOrDefault(x => x.Id == movieId);
                    Assert.That(movie, Is.Not.Null);
                    Assert.That(movie.Title, Is.EqualTo(command.Title));
                    Assert.That(movie.Description, Is.EqualTo(command.Description));
                    Assert.That(movie.ReleaseDate, Is.EqualTo(command.ReleaseDate));
                });
            });
        }

        private static UpdateMovieCommand.UpdateMovieCommandHandler CreateSut(MovieManagerContext context) => new(context);
    }
}
