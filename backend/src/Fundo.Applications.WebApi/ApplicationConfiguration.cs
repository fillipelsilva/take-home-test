using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using MediatR;
using Fundo.Application.Behaviors;

namespace Fundo.Applications.WebApi
{
    public static class ApplicationConfiguration
    {
        public static IServiceCollection AddApplication(
           this IServiceCollection services,
           Assembly[] moduleAssemblies)
        {

            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssemblies(moduleAssemblies);
            });

            services.AddValidatorsFromAssemblies(moduleAssemblies, includeInternalTypes: true);

            return services;
        }
    }
}
