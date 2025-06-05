using ErrorOr;
using FastEndpoints;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sample.Api.Common;
using System.Runtime.CompilerServices;

namespace Sample.Api;

public static class ServiceExtensions
{
    public static IServiceCollection AddSampleApiServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddFastEndpoints(opt =>
            {
                opt.Assemblies = [typeof(ServiceExtensions).Assembly];
            });

        // Use validators if need
        // services.AddTransient<IValidator<CreateProcessRequest>, CreateProcessRequestValidator>();
        // services.AddAutoMapper(typeof(CreateProcessMapper));

        return services;
    }

    public static T ValueOrThrow<T>(
        this IErrorOr<T> errorOr,
        [CallerMemberName] string memberName = "",
        [CallerLineNumber] int sourceLineNumber = 0)
    {
        if (errorOr.IsError)
        {
            throw new ErrorOrException(errorOr, memberName, sourceLineNumber);
        }

        return errorOr.Value;
    }
}
