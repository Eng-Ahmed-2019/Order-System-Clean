using MediatR;
using Serilog;
using OrderSystem.Application.Interfaces;
using OrderSystem.Application.CQRS.Commands;

namespace OrderSystem.Application.CQRS.Handlers
{
    public class LogoutUserHandler : IRequestHandler<LogoutUserCommand>
    {
        private readonly ISessionRepository _sessionRepository;

        public LogoutUserHandler(ISessionRepository sessionRepository)
        {
            _sessionRepository = sessionRepository;
        }

        public async Task Handle(LogoutUserCommand command,CancellationToken cancellation)
        {
            Log.Information(
                "Logout request for SessionId {SessionId}",
                command.sessionId
            );
            await _sessionRepository.DeleteAsync(command.sessionId);
            Log.Information(
                "Session deleted successfully for SessionId {SessionId}",
                command.sessionId
            );
        }
    }
}