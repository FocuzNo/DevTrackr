using System.Diagnostics;
using DevTrackr.Observability.Configuration;
using DevTrackr.Observability.ExceptionHandling;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Context;

namespace DevTrackr.Observability.Extensions;

public static class ObservabilityExtensions
{
    public static WebApplicationBuilder AddDevTrackrObservability(
        this WebApplicationBuilder builder,
        string serviceName)
    {
        builder.Services.Configure<SeqOptions>(builder.Configuration.GetSection(SeqOptions.SectionName));
        builder.Services.Configure<OpenTelemetryOptions>(builder.Configuration.GetSection(OpenTelemetryOptions.SectionName));

        builder.Host.UseSerilog((context, services, loggerConfiguration) =>
        {
            var seqUrl = context.Configuration[$"{SeqOptions.SectionName}:ServerUrl"];

            loggerConfiguration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithProperty("Application", "DevTrackr")
                .Enrich.WithProperty("ServiceName", serviceName)
                .Enrich.WithProperty("EnvironmentName", context.HostingEnvironment.EnvironmentName)
                .WriteTo.Console();

            if (!string.IsNullOrWhiteSpace(seqUrl))
            {
                loggerConfiguration.WriteTo.Seq(seqUrl);
            }
        });

        builder.Services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Extensions["traceId"] =
                    Activity.Current?.TraceId.ToString() ?? context.HttpContext.TraceIdentifier;
            };
        });
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(serviceName: serviceName))
            .WithTracing(tracing =>
            {
                tracing
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddSource("MassTransit");

                var endpoint = builder.Configuration[$"{OpenTelemetryOptions.SectionName}:OtlpEndpoint"];
                if (!string.IsNullOrWhiteSpace(endpoint))
                {
                    tracing.AddOtlpExporter(options => options.Endpoint = new Uri(endpoint));
                }
            })
            .WithMetrics(metrics =>
            {
                metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation();

                var endpoint = builder.Configuration[$"{OpenTelemetryOptions.SectionName}:OtlpEndpoint"];
                if (!string.IsNullOrWhiteSpace(endpoint))
                {
                    metrics.AddOtlpExporter(options => options.Endpoint = new Uri(endpoint));
                }
            });

        return builder;
    }

    public static WebApplication UseDevTrackrObservability(
        this WebApplication app,
        string serviceName)
    {
        app.UseExceptionHandler();
        app.Use(
            async (context, next) =>
            {
                using (LogContext.PushProperty("TraceId", Activity.Current?.TraceId.ToString() ?? context.TraceIdentifier))
                using (LogContext.PushProperty("SpanId", Activity.Current?.SpanId.ToString() ?? string.Empty))
                using (LogContext.PushProperty("ServiceName", serviceName))
                {
                    await next();
                }
            });
        app.UseSerilogRequestLogging(options =>
        {
            options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
                diagnosticContext.Set("ServiceName", serviceName);
                diagnosticContext.Set("TraceId", Activity.Current?.TraceId.ToString() ?? httpContext.TraceIdentifier);
                diagnosticContext.Set("SpanId", Activity.Current?.SpanId.ToString() ?? string.Empty);
            };
        });

        return app;
    }
}
