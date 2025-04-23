using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserService.DataAccess;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using UserService.Business;
using UserService.Middleware;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using UserService.Business.Profiles;
using UserService.CommunicationTypes;
using System.Security.Claims;
using UserService.CommunicationTypes.Grpc.GrpcClient;

var builder = WebApplication.CreateBuilder(args);

// Dynamic mode for testing
var environment = builder.Environment.IsProduction();

builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("UserServiceConn")));

// Add Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Configure JWT and authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
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

// Add grpc
builder.Services.AddSingleton<GrpcMajorClientService>();
builder.Services.AddSingleton<GrpcBatchClientService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();

// Add CORS:

var corsPolicy = "AllowSpecificOrigins";

builder.Services.AddCors(options => {
    options.AddPolicy(corsPolicy, builder =>
    {
        builder.AllowAnyOrigin()
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

//Config DI
builder.Services.AddRepositories();
builder.Services.AddServices();
builder.Services.AddCommunicationTypes();

// Config automapper
builder.Services.AddSingleton<AutoMapper.IConfigurationProvider>(new MapperConfiguration(cfg =>
{
    cfg.AddProfile<UserProfile>();
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

//Seed data
await PrepDb.PrepPopulationAsync(app, environment);

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

// use the exception middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.Run();
