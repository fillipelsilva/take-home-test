using Fundo.Application.Commands.CreateLoan;
using Fundo.Application.Commands.DeleteLoan;
using Fundo.Application.Commands.RegisterLoanPayment;
using Fundo.Application.Commands.UpdateLoan;
using Fundo.Application.DTOs;
using Fundo.Application.Queries.GetLoanById;
using Fundo.Application.Queries.GetLoans;
using Fundo.Applications.WebApi.Extensions;
using Fundo.Domain;
using Fundo.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Fundo.Applications.WebApi.Controllers
{
    [ApiController]
    [Route("/loans")]
    [Authorize]
    public sealed class LoanManagementController : Controller
    {
        private readonly ISender _sender;

        public LoanManagementController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost]
        [Authorize(Policy = "LoansWrite")]
        public async Task<IActionResult> CreateLoan(
        [FromBody] CreateLoanRequest request,
        CancellationToken cancellationToken)
        {
            var command = new CreateLoanCommand(request.Amount, request.ApplicantName);
            Result<Guid> result = await _sender.Send(command, cancellationToken);

            return result.ToCreatedActionResult(
                this,
                routeName: nameof(GetLoanById),
                routeValues: new { id = result.IsSuccess ? result.Value : Guid.Empty });
        }

        [HttpGet("{id:guid}", Name = nameof(GetLoanById))]
        [Authorize(Policy = "LoansRead")]
        public async Task<IActionResult> GetLoanById(Guid id, CancellationToken cancellationToken)
        {
            var query = new GetLoanByIdQuery(id);
            Domain.Result<LoanDto> result = await _sender.Send(query, cancellationToken);

            return result.ToActionResult(this);
        }

        [HttpGet]
        [Authorize(Policy = "LoansRead")]
        public async Task<IActionResult> GetLoans(CancellationToken cancellationToken)
        {
            var query = new GetLoansQuery();
            Domain.Result<IReadOnlyList<LoanDto>> result = await _sender.Send(query, cancellationToken);

            return result.ToActionResult(this);
        }

        [HttpPost("{id:guid}/payment")]
        [Authorize(Policy = "LoansWrite")]
        public async Task<IActionResult> RegisterPayment(
            Guid id,
            [FromBody] RegisterPaymentRequest request,
            CancellationToken cancellationToken)
        {
            var command = new RegisterLoanPaymentCommand(id, request.Amount);
            Domain.Result result = await _sender.Send(command, cancellationToken);

            return result.ToActionResult(this);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Policy = "LoansWrite")]
        public async Task<IActionResult> UpdateLoan(
            Guid id,
            [FromBody] UpdateLoanRequest request,
            CancellationToken cancellationToken)
        {
            var command = new UpdateLoanCommand(
                id,
                request.Amount,
                request.ApplicantName,
                request.CurrentBalance,
                request.Status);

            Domain.Result result = await _sender.Send(command, cancellationToken);

            return result.ToActionResult(this);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Policy = "LoansWrite")]
        public async Task<IActionResult> DeleteLoan(Guid id, CancellationToken cancellationToken)
        {
            var command = new DeleteLoanCommand(id);
            Domain.Result result = await _sender.Send(command, cancellationToken);

            return result.ToActionResult(this);
        }

        public sealed record CreateLoanRequest(decimal Amount, string ApplicantName);

        public sealed record UpdateLoanRequest(
            decimal Amount,
            string ApplicantName,
            decimal CurrentBalance,
            LoanStatus Status);

        public sealed record RegisterPaymentRequest(decimal Amount);
    }
}