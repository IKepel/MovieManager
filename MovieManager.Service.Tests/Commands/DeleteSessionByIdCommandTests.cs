using Application.ProductFeatures.Commands;
using AutoFixture;
using Domain.Entites;
using Persistence.Context;

namespace MovieManager.Core.Application.ProductFeatures.Tests.Commands
{
    public class DeleteSessionByIdCommandTests
    {
        private readonly DbContextDecorator<MovieManagerContext> _dbContext;

        protected readonly CancellationTokenSource _cts = new();

        public DeleteSessionByIdCommandTests()
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

        [Test]
        public void Command_DeletesSession()
        {
            _dbContext.Assert(async context =>
            {
                // Arrange
                var sut = CreateSut(context);
                var command = new DeleteSessionByIdCommand { Id = 1 };

                //Act
                var sessionId = await sut.Handle(command, _cts.Token);

                // Assert
                _dbContext.Assert(context =>
                {
                    var session = context.Sessions.FirstOrDefault(x => x.Id == sessionId);
                    Assert.That(session, Is.Null);
                });
            });
        }

        private static DeleteSessionByIdCommand.DeleteSessionByIdCommandHandler CreateSut(MovieManagerContext context) => new(context);
    }
}
