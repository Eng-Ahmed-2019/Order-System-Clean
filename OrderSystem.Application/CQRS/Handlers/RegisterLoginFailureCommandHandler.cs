using MediatR;
using OrderSystem.Application.Interfaces;
using OrderSystem.Application.CQRS.Commands;

namespace OrderSystem.Application.CQRS.Handlers
{
    public class RegisterLoginFailureCommandHandler
    : IRequestHandler<RegisterLoginFailureCommand>
    {
        private readonly ILoginLockoutRepository _repo;

        public RegisterLoginFailureCommandHandler(ILoginLockoutRepository repo)
        {
            _repo = repo;
        }

        public async Task Handle(
            RegisterLoginFailureCommand request,
            CancellationToken cancellationToken
        )
        {
            await _repo.RegisterFailureAsync(request.Key);
        }
    }
}