using Application.ProductFeatures.Queries;
using AutoFixture;
using Domain.Entites;
using Persistence.Context;

namespace MovieManager.Core.Application.ProductFeatures.Tests.Queries
{
    public class GetAllSessionsQueryTests
    {
        private readonly DbContextDecorator<MovieManagerContext> _dbContext;

        protected readonly CancellationTokenSource _cts = new();

        public GetAllSessionsQueryTests()
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
        public void DataSet_ReturnsEmpty_WhenNoSessions()
        {
            _dbContext.Assert(async context =>
            {
                // Arrange
                var sut = CreateSut(context);

                //Act
                var results = await sut.Handle(new GetAllSessionsQuery(), _cts.Token);

                //Assert
                Assert.That(results, Is.Not.Null);
                Assert.That(!results.Any());
            });
        }

        [Test]
        public void DataSet_ReturnsCorrectRows()
        {
            var fixture = new Fixture();
            var movie1 = fixture.Build<Movie>().With(x => x.Id, 1).Create();
            var movie2 = fixture.Build<Movie>().With(x => x.Id, 2).Create();

            var session1 = new Session { Id = 1, MovieId = 1, RoomName = "Room 1", StartDateTime = DateTime.Now };
            var session2 = new Session { Id = 2, MovieId = 2, RoomName = "Room 2", StartDateTime = DateTime.Now };

            _dbContext.AddAndSaveRange(new List<Movie> { movie1, movie2 });
            _dbContext.AddAndSaveRange(new List<Session> { session1, session2 });

            _dbContext.Assert(async context =>
            {
                // Arrange
                var sut = CreateSut(context);

                //Act
                var results = await sut.Handle(new GetAllSessionsQuery(), _cts.Token);

                //Assert
                Assert.That(results, Is.Not.Null);
                Assert.That(results.Count(), Is.EqualTo(2));

                var lastSession = results.Last();
                Assert.Multiple(() =>
                {
                    Assert.That(lastSession.Id, Is.EqualTo(session2.Id));
                    Assert.That(lastSession.RoomName, Is.EqualTo(session2.RoomName));
                    Assert.That(lastSession.StartDateTime, Is.EqualTo(session2.StartDateTime));
                    Assert.That(lastSession.Movie, Is.Not.Null);
                });
                Assert.Multiple(() =>
                {
                    Assert.That(lastSession.Movie.Id, Is.EqualTo(movie2.Id));
                    Assert.That(lastSession.Movie.Title, Is.EqualTo(movie2.Title));
                    Assert.That(lastSession.Movie.Description, Is.EqualTo(movie2.Description));
                    Assert.That(lastSession.Movie.ReleaseDate, Is.EqualTo(movie2.ReleaseDate));
                });
            });
        }

        private static GetAllSessionsQuery.GetAllSessionsQueryHandler CreateSut(MovieManagerContext context) => new(context);
    }
}
