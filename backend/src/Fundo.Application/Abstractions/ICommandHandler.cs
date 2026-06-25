using Fundo.Domain;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fundo.Application.Abstractions
{
    public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand, Result>
    where TCommand : ICommand
    {
    }

    public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, Result<TResponse>>
        where TCommand : ICommand<TResponse>
    {
    }
}
