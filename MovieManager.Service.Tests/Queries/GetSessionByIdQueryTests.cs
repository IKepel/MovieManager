using Application.ProductFeatures.Queries;
using AutoFixture;
using Domain.Entites;
using Persistence.Context;

namespace MovieManager.Core.Application.ProductFeatures.Tests.Queries
{
    public class GetSessionByIdQueryTests
    {
        private readonly DbContextDecorator<MovieManagerContext> _dbContext;

        protected readonly CancellationTokenSource _cts = new();

        public GetSessionByIdQueryTests()
        {
            var options = Utilities.CreateInMemoryDbOptions<MovieManagerContext>();

            _dbContext = new DbContextDecorator<MovieManagerContext>(options);
        }

        [Test]
        public void Query_ReturnsNull_WhenSessionDoesNotExist()
        {
            _dbContext.Assert(async context =>
            {
                // Arrange
                var sut = CreateSut(context);
                var query = new GetSessionByIdQuery { Id = 999 };

                // Act
                var result = await sut.Handle(query, _cts.Token);

                // Assert
                Assert.That(result, Is.Null);
            });
        }

        [Test]
        public void Query_ReturnsCorrectSession()
        {
            var fixture = new Fixture();
            var session1 = fixture.Build<Session>().With(x => x.Id, 1).Create();
            var session2 = fixture.Build<Session>().With(x => x.Id, 2).Create();
            _dbContext.AddAndSaveRange(new List<Session> { session1, session2 });

            _dbContext.Assert(async context =>
            {
                // Arrange
                var sut = CreateSut(context);
                var query = new GetSessionByIdQuery { Id = 2 };

                // Act
                var result = await sut.Handle(query, _cts.Token);

                // Assert
                Assert.That(result, Is.Not.Null);
                Assert.Multiple(() =>
                {
                    Assert.That(result.Id, Is.EqualTo(session2.Id));
                    Assert.That(result.MovieId, Is.EqualTo(session2.MovieId));
                    Assert.That(result.RoomName, Is.EqualTo(session2.RoomName));
                    Assert.That(result.StartDateTime, Is.EqualTo(session2.StartDateTime));
                    Assert.That(result.Movie.Id, Is.EqualTo(session2.Movie.Id));
                    Assert.That(result.Movie.Title, Is.EqualTo(session2.Movie.Title));
                    Assert.That(result.Movie.Description, Is.EqualTo(session2.Movie.Description));
                    Assert.That(result.Movie.ReleaseDate, Is.EqualTo(session2.Movie.ReleaseDate));
                });
            });
        }

        private static GetSessionByIdQuery.GetSessionByIdQueryHandler CreateSut(MovieManagerContext context) => new(context);
    }
}
