using Fundo.Domain;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fundo.Application.Abstractions
{
    public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>
    {
    }
}
