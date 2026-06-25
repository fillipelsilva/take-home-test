using Fundo.Domain;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fundo.Application.Abstractions
{
    public interface IQuery<TResponse> : IRequest<Result<TResponse>>
    {
    }
}
