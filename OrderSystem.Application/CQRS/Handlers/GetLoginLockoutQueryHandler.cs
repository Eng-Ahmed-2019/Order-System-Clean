using MediatR;
using OrderSystem.Domain.Entities;
using OrderSystem.Application.Interfaces;
using OrderSystem.Application.CQRS.Queries;

namespace OrderSystem.Application.CQRS.Handlers
{
    public class GetLoginLockoutQueryHandler
    : IRequestHandler<GetLoginLockoutQuery, LoginLockout?>
    {
        private readonly ILoginLockoutRepository _repo;

        public GetLoginLockoutQueryHandler(ILoginLockoutRepository repo)
        {
            _repo = repo;
        }

        public Task<LoginLockout?> Handle(
            GetLoginLockoutQuery request,
            CancellationToken cancellationToken
        )
        {
            return _repo.GetAsync(request.Key);
        }
    }
}