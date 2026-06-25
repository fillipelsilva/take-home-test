using Fundo.Domain;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fundo.Application.Abstractions
{
    public interface ICommand : IRequest<Result>, IBaseCommand
    {
    }

    public interface ICommand<TResponse> : IRequest<Result<TResponse>>, IBaseCommand
    {
    }

}
