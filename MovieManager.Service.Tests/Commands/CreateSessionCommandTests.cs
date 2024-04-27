using Application.ProductFeatures.Commands;
using AutoFixture;
using Domain.Entites;
using Persistence.Context;

namespace MovieManager.Core.Application.ProductFeatures.Tests.Commands
{
    public class CreateSessionCommandTests
    {
        private readonly DbContextDecorator<MovieManagerContext> _dbContext;

        protected readonly CancellationTokenSource _cts = new();

        public CreateSessionCommandTests()
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
        public void Command_CreateNewSession()
        {
            var fixture = new Fixture();
            var command = fixture.Build<CreateSessionCommand>().With(x => x.MovieId, 1).Create();

            _dbContext.Assert(async context =>
            {
                //Arrange
                var sut = CreateSut(context);

                //Act
                var sessionId = await sut.Handle(command, _cts.Token);

                //Assert
                _dbContext.Assert(context =>
                {
                    var session = context.Sessions.FirstOrDefault(x => x.Id == sessionId);
                    Assert.That(session, Is.Not.Null);
                    Assert.That(session.MovieId, Is.EqualTo(command.MovieId));
                    Assert.That(session.RoomName, Is.EqualTo(command.RoomName));
                    Assert.That(session.StartDateTime, Is.EqualTo(command.StartDateTime));
                });
            });
        }

        [Test]
        public void Command_DoesNotCreateDuplicates()
        {
            var fixture = new Fixture();
            var command = fixture.Build<CreateSessionCommand>().Create();

            _dbContext.Assert(async context =>
            {
                //Arrange
                var sut = CreateSut(context);
                
                //Act
                var sessionId1 = await sut.Handle(command, _cts.Token);
                var sessionId2 = await sut.Handle(command, _cts.Token);

                //Assert
                Assert.That(sessionId1, Is.Not.EqualTo(sessionId2));
            });
        }

        private static CreateSessionCommand.CreateSessionCommandHandler CreateSut(MovieManagerContext context) => new(context);
    }
}
