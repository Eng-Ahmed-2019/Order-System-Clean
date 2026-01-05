using MediatR;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using OrderSystem.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using OrderSystem.Application.CQRS.Commands;

namespace OrderSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IValidator<RegisterRequestDto> _registerValidator;
        private readonly IValidator<LoginRequestDto> _loginValidator;

        public AuthController(IMediator mediator, IValidator<RegisterRequestDto> registerValidator, IValidator<LoginRequestDto> loginValidator)
        {
            _mediator = mediator;
            _registerValidator = registerValidator;
            _loginValidator = loginValidator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(
            RegisterRequestDto dto,
            [FromServices] IValidator<RegisterRequestDto> validator)
        {
            var validationResult = await validator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => new {
                    Field = e.PropertyName,
                    Error = e.ErrorMessage
                }));
            }
            await _mediator.Send(new RegisterUserCommand(
                dto.FullName,
                dto.NationalId,
                dto.Password,
                dto.Email
            ));
            return Ok(new { Message = "User Registered Successfully" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(
        [FromBody] LoginRequestDto dto,
        [FromServices] IValidator<LoginRequestDto> validator)
        {
            var validationResult = await validator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => new {
                    Field = e.PropertyName,
                    Error = e.ErrorMessage
                }));
            }
            var result = await _mediator.Send(new LoginUserCommand(dto.Email, dto.Password));
            return Ok(new
            {
                Success = true,
                Data = result
            });
        }

        [Authorize]
        [HttpPost("log-out")]
        public async Task<IActionResult> LogOut()
        {
            var sidClaim = User.FindFirst("sid");
            if (sidClaim == null)
            {
                return Unauthorized(new { Success = false, Message = "Session ID not found" });
            }
            await _mediator.Send(new LogoutUserCommand(Guid.Parse(sidClaim.Value)));
            return Ok(new
            {
                Success = true,
                Message = "Logged out successfully"
            });
        }
    }
}