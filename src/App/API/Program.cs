using API.Authentication;
using API.ExampleProviders;
using API.Installers;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(c => c.Filters.Add<ApiKeyAuthFilter>());
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDatabase();
builder.Services.AddMessageBus();
builder.Services.AddMockBank();
builder.Services.AddServices();

builder.Services.AddSwaggerExamplesFromAssemblyOf<CardPaymentExampleProvider>();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo {Title = "Checkout.com challenge", Version = "v1"});
    options.ExampleFilters();
    options.AddSecurityDefinition("ApiKeyAuth", new OpenApiSecurityScheme()
    {
        Type = SecuritySchemeType.ApiKey,
        Description = "API key to access the payment gateway",
        Name = AuthConstants.ApiKeyHeader,
        In = ParameterLocation.Header,
        Scheme = "ApiKeyScheme",
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme()
            {
                In = ParameterLocation.Header,
                Reference = new OpenApiReference()
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiKeyAuth"
                }
            },
            new List<string>()
        }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();