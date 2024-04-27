using Application.ProductFeatures.Commands;
using AutoFixture;
using Domain.Entites;
using Persistence.Context;

namespace MovieManager.Core.Application.ProductFeatures.Tests.Commands
{
    public class UpdateSessionCommandTests
    {
        private readonly DbContextDecorator<MovieManagerContext> _dbContext;
        protected readonly CancellationTokenSource _cts = new();

        public UpdateSessionCommandTests()
        {
            var options = Utilities.CreateInMemoryDbOptions<MovieManagerContext>();

            _dbContext = new DbContextDecorator<MovieManagerContext>(options);
        }

        [SetUp]
        public void Init()
        {
            var fixture = new Fixture();
            var session = fixture.Build<Session>().With(x => x.Id, 1).Create();
            _dbContext.AddAndSave(session);
        }

        [TearDown]
        public void Cleanup()
        {
            _dbContext.Clear();
        }

        [Test]
        public void Command_UpdateSession()
        {
            var fixture = new Fixture();
            var command = fixture.Build<UpdateSessionCommand>().With(x => x.Id, 1).Create();

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

        private static UpdateSessionCommand.UpdateSessionCommandHandler CreateSut(MovieManagerContext context) => new(context);
    }
}
