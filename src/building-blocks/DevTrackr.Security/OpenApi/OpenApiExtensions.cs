using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;
using System.Collections.Generic;

namespace DevTrackr.Security.OpenApi;

public static class OpenApiExtensions
{
    private const string BearerSchemeName = "Bearer";

    public static IServiceCollection AddDevTrackrOpenApi(this IServiceCollection services)
    {
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer(AddBearerSecuritySchemeAsync);
            options.AddOperationTransformer(RequireBearerSecurityAsync);
        });

        return services;
    }

    private static Task AddBearerSecuritySchemeAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext _,
        CancellationToken __)
    {
        var components = document.Components;
        if (components is null)
        {
            components = new OpenApiComponents();
            document.Components = components;
        }

        var securitySchemes = components.SecuritySchemes;
        if (securitySchemes is null)
        {
            securitySchemes = new Dictionary<string, IOpenApiSecurityScheme>();
            components.SecuritySchemes = securitySchemes;
        }

        securitySchemes[BearerSchemeName] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Name = "Authorization",
            Description = "Paste the JWT access token. Scalar will send it as 'Bearer {token}'."
        };

        return Task.CompletedTask;
    }

    private static Task RequireBearerSecurityAsync(
        OpenApiOperation operation,
        OpenApiOperationTransformerContext context,
        CancellationToken _)
    {
        var metadata = context.Description.ActionDescriptor.EndpointMetadata;
        var allowsAnonymous = metadata.OfType<IAllowAnonymous>().Any();
        var requiresAuthorization = metadata.OfType<IAuthorizeData>().Any();

        if (allowsAnonymous || !requiresAuthorization)
        {
            return Task.CompletedTask;
        }

        operation.Security ??= [];
        operation.Security.Add(
            new OpenApiSecurityRequirement
            {
                [
                    new OpenApiSecuritySchemeReference(
                        BearerSchemeName,
                        context.Document,
                        null)
                ] = []
            });

        return Task.CompletedTask;
    }
}
