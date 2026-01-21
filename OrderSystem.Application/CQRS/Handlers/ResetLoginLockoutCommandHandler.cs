using MediatR;
using OrderSystem.Application.Interfaces;
using OrderSystem.Application.CQRS.Commands;

namespace OrderSystem.Application.CQRS.Handlers
{
    public class ResetLoginLockoutCommandHandler
    : IRequestHandler<ResetLoginLockoutCommand>
    {
        private readonly ILoginLockoutRepository _repo;

        public ResetLoginLockoutCommandHandler(ILoginLockoutRepository repo)
        {
            _repo = repo;
        }

        public async Task Handle(
            ResetLoginLockoutCommand request,
            CancellationToken cancellationToken
        )
        {
            await _repo.ResetAsync(request.Key);
        }
    }
}