using MediatR;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using OrderSystem.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using OrderSystem.Application.CQRS.Commands;

namespace OrderSystem.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/payments")]
    public class PaymentsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IValidator<ProcessPaymentRequestDto> _validator;

        public PaymentsController(IMediator mediator, IValidator<ProcessPaymentRequestDto> validator)
        {
            _mediator = mediator;
            _validator = validator;
        }

        [HttpPost("process-payment")]
        public async Task<IActionResult> ProcessPayment
            (
                ProcessPaymentRequestDto process,
                [FromServices] IValidator<ProcessPaymentRequestDto> validator
            )
        {
            var validationResult = await validator.ValidateAsync(process);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => new
                {
                    Field = e.PropertyName,
                    Error = e.ErrorMessage
                }));
            }
            var r = await _mediator.Send(new ProcessPaymentCommand(process.OrderId));
            return r ? Ok("Success Process") : BadRequest("Failed Process");
        }

        [HttpPost("process-stripe")]
        public async Task<IActionResult> ProcessStripe(
                ProcessPaymentRequestDto dto,
                [FromServices] IValidator<ProcessPaymentRequestDto> validator
            )
        {
            var validationResult = await validator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => new
                {
                    Field = e.PropertyName,
                    Error = e.ErrorMessage
                }));
            }
            var result = await _mediator.Send(new ProcessStripePaymentCommand(dto.OrderId));
            return result ? Ok("Success Process") : BadRequest("Failed Process");
        }
    }
}