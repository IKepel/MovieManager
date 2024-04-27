using Application.ProductFeatures.Commands;
using AutoFixture;
using Persistence.Context;

namespace MovieManager.Core.Application.ProductFeatures.Tests.Commands
{
    public class CreateMovieCommandTests
    {
        private readonly DbContextDecorator<MovieManagerContext> _dbContext;

        protected readonly CancellationTokenSource _cts = new();

        public CreateMovieCommandTests()
        {
            var options = Utilities.CreateInMemoryDbOptions<MovieManagerContext>();

            _dbContext = new DbContextDecorator<MovieManagerContext>(options);
        }

        [Test]
        public void Command_CreateMovie()
        {
            var fixture = new Fixture();
            var movieCommand = fixture.Build<CreateMovieCommand>().Create();

            _dbContext.Assert(async context =>
            {
                //Arrange
                var sut = CreateSut(context);

                //Act
                var movieId = await sut.Handle(movieCommand, _cts.Token);

                //Assert
                _dbContext.Assert(context =>
                {
                    var movie = context.Movies.FirstOrDefault(x => x.Id == movieId);
                    Assert.That(movie, Is.Not.Null);
                    Assert.That(movie.Title, Is.EqualTo(movieCommand.Title));
                    Assert.That(movie.Description, Is.EqualTo(movieCommand.Description));
                    Assert.That(movie.ReleaseDate, Is.EqualTo(movieCommand.ReleaseDate));
                });
            });
        }

        [Test]
        public void Command_DoesNotCreateDuplicates()
        {
            var fixture = new Fixture();
            var command = fixture.Build<CreateMovieCommand>().Create();

            _dbContext.Assert(async context =>
            {
                //Arrange
                var sut = CreateSut(context);

                //Act
                var movieId1 = await sut.Handle(command, _cts.Token);
                var movieId2 = await sut.Handle(command, _cts.Token);

                //Assert
                Assert.That(movieId1, Is.Not.EqualTo(movieId2));
            });
        }

        private static CreateMovieCommand.CreateMovieCommandHandler CreateSut(MovieManagerContext context) => new(context);
    }
}
