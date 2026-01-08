using Serilog;
using System.Net;
using System.Text;
using FluentValidation;
using Microsoft.OpenApi.Models;
using Serilog.Sinks.MSSqlServer;
using FluentValidation.AspNetCore;
using OrderSystem.API.Middlewares;
using Microsoft.IdentityModel.Tokens;
using OrderSystem.Infrastructure.Data;
using OrderSystem.Application.Services;
using OrderSystem.Application.Settings;
using OrderSystem.Application.Interfaces;
using OrderSystem.Application.Validators;
using OrderSystem.Application.CQRS.Handlers;
using OrderSystem.Infrastructure.Repositories;
using OrderSystem.Infrastructure.BackgroundJobs;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
Log.Logger = new LoggerConfiguration()
    .WriteTo.File(
        path: "Logs/log-.txt",
        rollingInterval: RollingInterval.Day)
    .WriteTo.MSSqlServer(
        connectionString: builder.Configuration.GetConnectionString("DefaultConnection"),
        sinkOptions: new MSSqlServerSinkOptions
        {
            TableName = "Logs",
            AutoCreateSqlTable = false
        })
    .CreateLogger();
builder.Host.UseSerilog();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtKey = builder.Configuration["Jwt:Key"];
    if (string.IsNullOrEmpty(jwtKey))
    {
        throw new InvalidOperationException("JWT Key is missing in configuration!");
    }
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.Zero
    };
});
builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid token."
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<DapperContext>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IPaymentLogRepository, PaymentLogRepository>();
builder.Services.AddScoped<ISessionRepository, SessionRepository>();
builder.Services.AddScoped<ILogRepository, LogRepositories>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ISubCategoryRepository, SubCategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IOrderItemRepository, OrderItemRepository>();
builder.Services.AddHostedService<PaymentRetryBackgroundService>();
builder.Services.Configure<StripeSettings>(
        builder.Configuration.GetSection("Stripe")
);
builder.Services
    .AddOptions<StripeSettings>()
    .Bind(builder.Configuration.GetSection("Stripe"))
    .Validate(
        settings => !string.IsNullOrWhiteSpace(settings.SecretKey),
        "Stripe SecretKey is missing or empty"
    )
    .ValidateOnStart();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<LoginValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<ProcessPaymentValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateCategoryValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateSubCategoryValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateProductValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<AddToCartValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<GetOrderValidator>();
// https://jsonplaceholder.typicode.com/posts
builder.Services.AddHttpClient("ExternalApi", client =>
{
    client.BaseAddress = new Uri("http://jsonplaceholder.typicode.com/");
    client.Timeout = TimeSpan.FromSeconds(30);
})
    .ConfigurePrimaryHttpMessageHandler(() =>
        new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        }
    );
builder.Services.AddMediatR(r =>
{
    r.RegisterServicesFromAssembly(typeof(GetOrderByIdHandler).Assembly);
    r.RegisterServicesFromAssemblies(typeof(ProcessPaymentHandler).Assembly);
    r.RegisterServicesFromAssembly(typeof(LoginUserHandler).Assembly);
    r.RegisterServicesFromAssembly(typeof(LogoutUserHandler).Assembly);
    r.RegisterServicesFromAssembly(typeof(ProcessStripePaymentHandler).Assembly);
    r.RegisterServicesFromAssembly(typeof(RegisterUserHandler).Assembly);
    r.RegisterServicesFromAssembly(typeof(CreateCategoryHandler).Assembly);
    r.RegisterServicesFromAssembly(typeof(CreateSubCategoryHandler).Assembly);
    r.RegisterServicesFromAssembly(typeof(CreateProductHandler).Assembly);
    r.RegisterServicesFromAssembly(typeof(AddToCartHandler).Assembly);
    r.RegisterServicesFromAssembly(typeof(CheckoutHandler).Assembly);
});
// Force TLS 1.2 for all outgoing HTTPS requests
ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
/*
app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<SessionValidationMiddleware>();
app.UseMiddleware<UnifiedMiddleware>();
*/
app.UseMiddleware<CustomExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<UnifiedMiddleware>();
app.MapControllers();
app.Run();