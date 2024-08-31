using Airdrops.Mint.Scheduler;
using Airdrops.Mint.Scheduler.Settings;
using Polly.Extensions.Http;
using Polly;
using Quartz;
using Serilog;
using Airdrops.Mint.Scheduler.Infrastructure.Abstractions;
using Refit;

Log.Logger = Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {NewLine}{Exception}")
    .CreateLogger();

try
{
    var builder = Host.CreateApplicationBuilder(args);
    builder.Services.AddSingleton(builder.Configuration.GetSection(nameof(UserSettings)).Get<UserSettings>());

    builder.Logging.ClearProviders();
    builder.Logging.AddSerilog();

    // Add services to the container
    builder.Services.AddQuartz(q =>
    {
        q.UseSimpleTypeLoader();
        q.UseInMemoryStore();

        // Register the job
        q.ScheduleJob<MintchainJob>(trigger => trigger
            .WithIdentity(nameof(MintchainJob))
            .StartNow()
            .WithSimpleSchedule(x => x
                .WithIntervalInHours(6)
                .RepeatForever())
        );
    });

    builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
    builder.Services.AddRefitClient<IMintchainApiClient>()
        .ConfigureHttpClient((ctx, c) =>
        {
            var configuration = ctx.GetService<IConfiguration>();
            var mintchainApiBaseUrl = configuration!["Endpoints:MintchainApiBaseUrl"];

            ArgumentException.ThrowIfNullOrWhiteSpace(mintchainApiBaseUrl, nameof(mintchainApiBaseUrl));

            c.BaseAddress = new Uri(mintchainApiBaseUrl);
        })
        .AddPolicyHandler(GetPolicy);

    var app = builder.Build();

    var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();

    lifetime.ApplicationStarted.Register(() =>
    {
        Log.Logger.Information("Application has started.");
    });

    lifetime.ApplicationStopped.Register(() =>
    {
        Log.Logger.Information("Application has stopped.");
    });

    app.Run();
}
catch (Exception e)
{
    Log.Fatal(e, "Cannot initialize Nike.CatalogAdapter.Api");
    throw;
}

static IAsyncPolicy<HttpResponseMessage> GetPolicy(IServiceProvider serviceProvider, HttpRequestMessage requestMessage)
{
    var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
    var logger = loggerFactory.CreateLogger("RetryPolicy");
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .Or<OperationCanceledException>()
        .OrResult(response => (int)response.StatusCode == 429 || (int)response.StatusCode > 500)
        .WaitAndRetryAsync(5,
            retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
            (outcome, timespan, retryAttempt, context) =>
            {
                logger.LogWarning("Retry {RetryAttempt} encountered an error. Waiting {Timespan} before next retry. Outcome: {StatusCode}", retryAttempt, timespan, outcome.Result.StatusCode);
            });
}
