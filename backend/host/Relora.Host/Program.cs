using System.Threading.RateLimiting;

using Amazon.Runtime;
using Amazon.S3;

using Relora.Admin.API;
using Relora.Admin.Application.Queries;
using Relora.Auctions.API.Controllers;
using Relora.Auctions.Application.Interfaces;
using Relora.Auctions.Application.Validators;
using Relora.Auctions.Infrastructure.Repository;
using Relora.Bids.API.Controllers;
using Relora.Bids.Application.Interfaces;
using Relora.Bids.Application.Validators;
using Relora.Bids.Infrastructure.Repository;
using Relora.Admin.Application.Validators;
using Relora.Host.BackgroundJobs;
using Relora.Host.Middleware;
using Relora.Identity.API.Controllers;
using Relora.Identity.Application.Interfaces;
using Relora.Identity.Application.Validators;
using Relora.Identity.Infrastructure;
using Relora.Identity.Infrastructure.Cookies;
using Relora.Identity.Infrastructure.Repository;
using Relora.Items.API.Controllers;
using Relora.Items.Application.Interfaces;
using Relora.Items.Application.Validators;
using Relora.Items.Infrastructure.Repository;
using Relora.Orders.API.Controllers;
using Relora.Orders.Application.Interfaces;
using Relora.Orders.Application.Service;
using Relora.Orders.Application.Validators;
using Relora.Orders.Infrastructure.Options;
using Relora.Orders.Infrastructure.Repository;
using Relora.Payments.API;
using Relora.Payments.Application.Interfaces;
using Relora.Payments.Application.Service;
using Relora.Payments.Domain;
using Relora.Payments.Infrastructure.Repository;
using Relora.Persistance;
using Relora.Realtime.Extensions;
using Relora.Shared.Domain.Time;
using Relora.Shared.Domain.Persistence;
using Relora.Shared.Infrastructure.Dispatcher;
using Relora.Shared.Infrastructure.Interfaces;
using Relora.Shared.Infrastructure.Media;
using Relora.Shared.Infrastructure.Pipelines;
using Relora.Shared.Infrastructure.Time;
using Relora.Shared.Application.Emails;
using Relora.Shared.Application.Payments;
using Relora.Shared.Infrastructure.Emails;
using Relora.Support.API.Controllers;
using Relora.Support.Application.Interfaces;
using Relora.Support.Application.Validators;
using Relora.Support.Infrastructure.Repository;

using FluentValidation;
using FluentValidation.AspNetCore;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MediaOptions>(
    builder.Configuration.GetSection(MediaOptions.SectionName));

var mediaOptions = builder.Configuration
    .GetSection(MediaOptions.SectionName)
    .Get<MediaOptions>() ?? throw new InvalidOperationException("R2 configuration is missing.");

builder.Services.AddSingleton<IAmazonS3>(_ =>
{
    var credentials = new BasicAWSCredentials(
        mediaOptions.AccessKeyId,
        mediaOptions.SecretAccessKey);

    var config = new AmazonS3Config
    {
        ServiceURL = mediaOptions.ServiceUrl,
        ForcePathStyle = true
    };

    return new AmazonS3Client(credentials, config);
});

builder.Services.AddControllers()
    .AddApplicationPart(typeof(AuctionsController).Assembly)
    .AddApplicationPart(typeof(EngineController).Assembly)
    .AddApplicationPart(typeof(ItemController).Assembly)
    .AddApplicationPart(typeof(MediaController).Assembly)
    .AddApplicationPart(typeof(BidsController).Assembly)
    .AddApplicationPart(typeof(AdminController).Assembly)
    .AddApplicationPart(typeof(OrdersController).Assembly)
    .AddApplicationPart(typeof(AuthController).Assembly)
    .AddApplicationPart(typeof(PaymentsController).Assembly)
    .AddApplicationPart(typeof(SellerStripeController).Assembly)
    .AddApplicationPart(typeof(SupportController).Assembly);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var problemDetails = new ValidationProblemDetails(context.ModelState)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Validation failed",
            Type = $"https://httpstatuses.com/{StatusCodes.Status400BadRequest}",
            Detail = "One or more validation errors occurred.",
            Instance = context.HttpContext.Request.Path
        };

        problemDetails.Extensions["message"] = "One or more validation errors occurred.";
        problemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier;

        return new BadRequestObjectResult(problemDetails);
    };
});

builder.Services.AddReloraRealtime();
builder.Services.AddIdentityModule(builder.Configuration);
builder.Services.AddMedia(builder.Configuration);

builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddValidatorsFromAssembly(typeof(RegisterCommandValidator).Assembly);
builder.Services.AddValidatorsFromAssembly(typeof(LoginCommandValidator).Assembly);
builder.Services.AddValidatorsFromAssembly(typeof(CreateLotCommandValidator).Assembly);
builder.Services.AddValidatorsFromAssembly(typeof(CreateSupportRequestCommandValidator).Assembly);
builder.Services.AddValidatorsFromAssembly(typeof(CreateAuctionCommandValidator).Assembly);
builder.Services.AddValidatorsFromAssembly(typeof(PlaceBidCommandValidator).Assembly);
builder.Services.AddValidatorsFromAssembly(typeof(ConfirmOrderReceivedCommandValidator).Assembly);
builder.Services.AddValidatorsFromAssembly(typeof(AcceptLotCommandValidator).Assembly);

builder.Services.AddTransient(
    typeof(IPipelineBehavior<,>),
    typeof(ValidationBehavior<,>)
);

var corsOrigins = builder.Environment.IsDevelopment()
    ? builder.Configuration.GetSection("Cors:DevOrigins").Get<string[]>()
    : builder.Configuration.GetSection("Cors:ProdOrigins").Get<string[]>();

if (corsOrigins is null || corsOrigins.Length == 0)
{
    throw new InvalidOperationException($"No CORS origins configured for environment '{builder.Environment.EnvironmentName}'.");
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("RealtimeCors", policy =>
    {
        policy
            .WithOrigins(corsOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddPolicy("AuthLoginPolicy", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
                AutoReplenishment = true
            }));

    options.AddPolicy("AuthRegisterPolicy", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(10),
                QueueLimit = 0,
                AutoReplenishment = true
            }));

    options.AddPolicy("AuthRefreshPolicy", httpContext =>
    {
        var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: $"refresh:{ip}",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 30,
                Window = TimeSpan.FromSeconds(30),
                QueueLimit = 0,
                AutoReplenishment = true
            });
    });

    options.AddPolicy("AuthMePolicy", httpContext =>
    {
        var userId = httpContext.User.FindFirst("sub")?.Value
            ?? httpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value
            ?? httpContext.Connection.RemoteIpAddress?.ToString()
            ?? "unknown";

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: $"me:{userId}",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 60,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
                AutoReplenishment = true
            });
    });

    options.AddPolicy("BidsPolicy", httpContext =>
    {
        var userId = httpContext.User.FindFirst("sub")?.Value
            ?? httpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
        var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var partition = userId is null ? $"ip:{ip}" : $"user:{userId}";

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: partition,
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 6,
                Window = TimeSpan.FromSeconds(10),
                QueueLimit = 0,
                AutoReplenishment = true
            });
    });

    options.AddPolicy("SupportPolicy", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(10),
                QueueLimit = 0,
                AutoReplenishment = true
            }));
});

builder.Services
    .AddOptions<StripeOptions>()
    .Bind(builder.Configuration.GetSection(StripeOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddSingleton<Stripe.Checkout.SessionService>();

var stripeOptions = builder.Configuration.GetSection(StripeOptions.SectionName).Get<StripeOptions>()!;
Stripe.StripeConfiguration.ApiKey = stripeOptions.SecretKey;

builder.Services.AddDbContext<ReloraDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres"));
});

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Relora.Items.Application.Marker.MediatRAssemblyMarker).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(Relora.Bids.Application.Marker.MediatRAssemblyMarker).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(Relora.Auctions.Application.Marker.MediatRAssemblyMarker).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(Relora.Orders.Application.Marker.MediatRAssemblyMarker).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(Relora.Identity.Application.Handlers.LoginCommandHandler).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(GetAdminDashboardData).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(Relora.Support.Application.Marker.MediatRAssemblyMarker).Assembly);
});

builder.Services.AddOptions<EmailOptions>()
    .Bind(builder.Configuration.GetSection(EmailOptions.SectionName))
    .ValidateDataAnnotations();

builder.Services.AddScoped<IEmailSender, EmailSender>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins(corsOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddAuthorization();
builder.Services.AddScoped<IAuctionRepository, AuctionRepository>();
builder.Services.AddScoped<IBidRepository, BidRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<ILotRepository, LotRepository>();
builder.Services.AddScoped<ILotCatalogRepository, LotCatalogRepository>();
builder.Services.AddScoped<IPendingLotMediaUploadRepository, PendingLotMediaUploadRepository>();
builder.Services.AddScoped<IPendingLotProofDocumentUploadRepository, PendingLotProofDocumentUploadRepository>();
builder.Services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
builder.Services.AddScoped<ICookieFactory, CookieFactory>();
builder.Services.AddScoped<IOrderRepository, OrdersRepository>();
builder.Services.AddScoped<ITransactionRunner, TransactionRunner>();
builder.Services.AddScoped<IOrderDisputeRepository, OrderDisputeRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<Relora.Payments.Application.Service.PaymentService>();
builder.Services.AddScoped<IPaymentRefundService, StripePaymentRefundService>();
builder.Services.AddSingleton<IClock, SystemClock>();
builder.Services.AddScoped<ISellerPaymentAccountRepository, SellerPaymentAccountRepository>();
builder.Services.AddScoped<ISupportRequestRepository, SupportRequestRepository>();

builder.Services.AddHostedService<AuctionAutoStopBackgroundService>();
builder.Services.AddHostedService<OrderPaymentExpirationBackgroundService>();
builder.Services.AddHostedService<SellerPayoutBackgroundService>();
builder.Services.AddHostedService<PendingLotMediaCleanupBackgroundService>();

builder.Services.AddScoped<IMediaUploader, MediaUploader>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseCors("RealtimeCors");
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapReloraRealtime();
app.MapControllers();

app.Run();
