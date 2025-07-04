using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using CourseService.DataAccess;
using EnrollmentService.Middleware;
using EnrollmentService.CommunicationTypes;
using EnrollmentService.CommunicationTypes.Grpc.GrpcClient;
using EnrollmentService.CommunicationTypes.Grpc.GrpcServer;
using EnrollmentService.DataAccess.Repositories;
using EnrollmentService.Business.Services;
using EnrollmentService.Business.Mappings;
using EnrollmentService.Utils.DistributedLock;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Dynamic mode for testing
var environment = builder.Environment.IsProduction();

builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("EnrollmentServiceConn")));

// Configure JWT and authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key),
            RoleClaimType = ClaimTypes.Role
        };
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();

//Add cors
var corsPolicy = "AllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: corsPolicy, policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

//Add Swagger
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer {token}'"
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
            new List<string>()
        }
    });
});

// Add grpc
builder.Services.AddSingleton<GrpcAcademicClassClientService>();
builder.Services.AddSingleton<GrpcStudentClientService>();

// Add Redis with fallback
builder.Services.AddSingleton<IConnectionMultiplexer>(provider =>
{
    try
    {
        var connectionString = builder.Configuration.GetConnectionString("RedisConnection") ?? "localhost:6379";
        var configurationOptions = ConfigurationOptions.Parse(connectionString);
        configurationOptions.AbortOnConnectFail = false;
        configurationOptions.ConnectRetry = 3;
        configurationOptions.ConnectTimeout = 5000;
        configurationOptions.SyncTimeout = 5000;
        
        return ConnectionMultiplexer.Connect(configurationOptions);
    }
    catch (Exception ex)
    {
        var logger = provider.GetService<ILogger<Program>>();
        logger?.LogError(ex, "Failed to connect to Redis. Fallback lock service will be used.");
        return null!; // Return null to trigger fallback
    }
});

// Add Distributed Lock Service with fallback
builder.Services.AddScoped<IDistributedLockService>(provider =>
{
    try
    {
        var redis = provider.GetService<IConnectionMultiplexer>();
        if (redis != null && redis.IsConnected)
        {
            var logger = provider.GetRequiredService<ILogger<RedisDistributedLockService>>();
            return new RedisDistributedLockService(redis, logger);
        }
        else
        {
            var logger = provider.GetRequiredService<ILogger<FallbackDistributedLockService>>();
            logger.LogWarning("Redis not available, using fallback distributed lock service");
            return new FallbackDistributedLockService(logger);
        }
    }
    catch (Exception ex)
    {
        var logger = provider.GetService<ILogger<Program>>();
        logger?.LogError(ex, "Error setting up distributed lock service, using fallback");
        var fallbackLogger = provider.GetRequiredService<ILogger<FallbackDistributedLockService>>();
        return new FallbackDistributedLockService(fallbackLogger);
    }
});

//Config DI
//Register repositories
builder.Services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
builder.Services.AddScoped<IExamRepository, ExamRepository>();
//Register services
builder.Services.AddScoped<IEnrollmentService, EnrollmentSvc>();
builder.Services.AddScoped<IExamService, ExamService>();

// Add Communication Types (Kafka, etc.)
//builder.Services.AddHostedService<KafkaConsumerService>();
builder.Services.AddCommunicationTypes();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Config automapper
builder.Services.AddSingleton<AutoMapper.IConfigurationProvider>(new MapperConfiguration(cfg =>
{
    cfg.AddProfile<EnrollmentMappingProfile>();
    cfg.AddProfile<ExamMappingProfile>();
}));
builder.Services.AddScoped<IMapper, Mapper>();

// Config validation middleware
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(x => x.Value.Errors.Count > 0)
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList()
            );

        var errorMessages = errors.SelectMany(e => e.Value).ToList();

        var response = new ApiResponse<List<string>>(false, null, errorMessages);
        return new BadRequestObjectResult(response);
    };
});

// Add GRPC:
builder.Services.AddGrpc();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors(corsPolicy);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

// Map GRPC services
app.MapGrpcService<GrpcEnrollmentServerService>();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.Run();
