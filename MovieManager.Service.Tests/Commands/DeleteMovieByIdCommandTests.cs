using Application.ProductFeatures.Commands;
using AutoFixture;
using Domain.Entites;
using Persistence.Context;

namespace MovieManager.Core.Application.ProductFeatures.Tests.Commands
{
    public class DeleteMovieByIdCommandTests
    {
        private readonly DbContextDecorator<MovieManagerContext> _dbContext;

        protected readonly CancellationTokenSource _cts = new();

        public DeleteMovieByIdCommandTests()
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

        [Test]
        public void Command_DeletesMovie()
        {
            _dbContext.Assert(async context =>
            {
                //Arrange
                var sut = CreateSut(context);
                var command = new DeleteMovieByIdCommand { Id = 1 };

                //Act
                var movieId = await sut.Handle(command, _cts.Token);

                // Assert
                _dbContext.Assert(context =>
                {
                    var movie = context.Movies.FirstOrDefault(x => x.Id == movieId);
                    Assert.That(movie, Is.Null);
                });
            });
        }

        private static DeleteMovieByIdCommand.DeleteMovieByIdCommandHandler CreateSut(MovieManagerContext context) => new(context);
    }
}
