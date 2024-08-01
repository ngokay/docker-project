using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using WebApplication5.Filter;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc;
using WebApplication5.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebApplication5.Redis;
using WebApplication5.CQRS.Repositories.Accounts;
using MediatR;
using System.Reflection;
using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;
using WebApplication5.RabbitMQ;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

// add cross origin

const string policyName = "CorsPolicy";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: policyName, builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// add services api version
builder.Services
    .AddApiVersioning(o => {
        o.ReportApiVersions = true;
        o.AssumeDefaultVersionWhenUnspecified = true;
        o.DefaultApiVersion = new ApiVersion(1, 0);
    })
    .AddVersionedApiExplorer(options =>
    {
        // Add the versioned API explorer, which also adds IApiVersionDescriptionProvider service
        // note: the specified format code will format the version as "'v'major[.minor][-status]"
        options.GroupNameFormat = "'v'VVV";
        //options.SubstituteApiVersionInUrl = true;

    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSwaggerGen(option =>
{
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});
builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();
// add mediatR
builder.Services.AddMediatR(Assembly.GetExecutingAssembly());
// add dependency injection
builder.Services.AddSingleton(sp => new ConnectionFactory() { Uri = new Uri(builder.Configuration.GetConnectionString("RabbitMQ")), DispatchConsumersAsync = true });
builder.Services.AddSingleton<ConnectionRabbitMQHepper>();
builder.Services.AddSingleton<ConnectionRabbitMQHepperExtension>();
builder.Services.AddSingleton<IClassA, ClassA>();
builder.Services.AddTransient<IClassB, ClassB>();
builder.Services.AddTransient<IRedisCacheService, RedisCacheService>();
builder.Services.AddSingleton<ISendMessage, SendMessage>();
builder.Services.AddSingleton<IAccounts, Accounts>();

builder.Services
        .AddAuthentication(JwtBearerDefaults.AuthenticationScheme) //check the HTTP Header's Authorization has the JWT Bearer Token
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                // varify Issuer
                ValidateIssuer = true,
                ValidIssuer = builder.Configuration.GetValue<string>("jwt:issuer"),

                // 📌 Important!!! audience need to be set to false
                // Because the default is true. 
                // So if you didn't set this prop when generate token, then the api will always return a check token error
                ValidateAudience = false,

                ValidateLifetime = true,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("jwt:secretKey")))
            };
        });


ConfigureLogging();
builder.Host.UseSerilog();

var app = builder.Build();

var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions.Reverse())
        {
            options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                description.GroupName.ToUpperInvariant());
        }
    });
//}


app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// add cross origin

app.UseCors(policyName);

app.MapControllers();

// add middleware
app.UseMiddleware<WebApplication5.Middleware.MiddlewareExcusion>();
app.UseMiddleware<WebApplication5.Middleware.MiddlewareCheckRevockToken>();
app.UseMiddleware<WebApplication5.Middleware.MiddlewareAuthorized>();



app.Run();


void ConfigureLogging()
{
    var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
    var configuration = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile(
            $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json",
            optional: true)
        .Build();

    Log.Logger = new LoggerConfiguration()
        .Enrich.FromLogContext()
        .Enrich.WithExceptionDetails()
        .WriteTo.Debug()
        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {CorrelationId} {Level:u3} {ClientIp} ] {Message} (at {Caller}){NewLine}{Exception}")
        .WriteTo.Elasticsearch(ConfigureElasticSink(configuration, environment))
        .Enrich.WithProperty("Environment", environment)
        .ReadFrom.Configuration(configuration)
        .CreateLogger();
}

ElasticsearchSinkOptions ConfigureElasticSink(IConfigurationRoot configuration, string environment)
{
#pragma warning disable CS8604 // Possible null reference argument.
    return new ElasticsearchSinkOptions(new Uri(configuration["ElasticConfiguration:Uri"]))
#pragma warning restore CS8604 // Possible null reference argument.
    {
        AutoRegisterTemplate = true,
        IndexFormat = $"docker-test-{DateTime.UtcNow:yyyy-MM}"
    };
}