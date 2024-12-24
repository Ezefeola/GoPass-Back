using FluentValidation;
using GoPass.Application.Services.Classes;
using GoPass.Application.Services.Interfaces;
using GoPass.Infrastructure.Repositories.Classes;
using GoPass.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using System.Text;
using GoPass.Infrastructure.Data;
using System.Reflection;
using GoPass.Application.Notifications.Classes;
using GoPass.Infrastructure.UnitOfWork;
using GoPass.Application.Facades.ServiceFacade;
using GoPass.Application.Utilities.Mappers;
using GoPass.Application.Services.Validations.Interfaces;
using GoPass.Application.Services.Validations.Classes;
using GoPass.API.Middlewares;
using GoPass.Application.Utilities.Assemblers.Interfaces;
using GoPass.Application.Utilities.Assemblers.Classes;
using GoPass.ExternalIntegrations.Payments.Stripe;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

#region Services Area
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers()
            .AddJsonOptions(options => { options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles; });

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "GoPass API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Introduce el token JWT en el formato: Bearer {tu token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[] {}
        }
    });
});

builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseSqlServer(connectionString)
);

builder.Services.AddAuthentication(options =>
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
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});

builder.Services.AddAuthorization();

builder.Services.AddValidatorsFromAssembly(Assembly.Load("GoPass.Application"));
builder.Services.AddFluentValidationAutoValidation();

var allowedOrigin = builder.Configuration.GetValue<string>("allowedOrigins")!;

builder.Services.AddCors(opciones =>
{
    opciones.AddDefaultPolicy(configuracion =>
    {
        configuracion.WithOrigins(allowedOrigin).AllowAnyHeader().AllowAnyMethod();
    });

    opciones.AddPolicy("free", configuracion =>
    {
        configuracion.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

builder.Services.AddHttpClient<IGopassHttpClientService, GopassHttpClientService>(client =>
{
    //client.BaseAddress = new Uri("https://localhost:7292/api/");
    client.BaseAddress = new Uri("http://localhost:5149/api/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddHttpContextAccessor();

//Validations
builder.Services.AddScoped<IUserValidationService, UserValidationService>();
//Validations

builder.Services.AddScoped<StripeService>();
// Services
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IResaleService, ResaleService>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<IResaleTicketTransactionService, ResaleTicketTransactionService>();
builder.Services.AddScoped<IAesGcmCryptoService, AesGcmCryptoService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IServiceFacade, ServiceFacade>();

//Singleton ???? No sabemos que pasa y si alguno rompe la app generando un bug sin devolver una Exception
builder.Services.AddSingleton<ITemplateService, TemplateService>();
builder.Services.AddSingleton<IEmailService, EmailService>();
builder.Services.AddSingleton<NotificationQueue>();
builder.Services.AddSingleton<GoPass.Application.Notifications.Interfaces.ISubject<string>, Subject<string>>();
builder.Services.AddSingleton<EmailNotificationBackgroundService>();
builder.Services.AddHostedService(provider => provider.GetRequiredService<EmailNotificationBackgroundService>());
//Singleton ???? No sabemos que pasa y si alguno rompe la app generando un bug sin devolver una Exception
// Services

// Assemblers
builder.Services.AddScoped<ITicketResaleHistoryAssembler, TicketResaleHistoryAssembler>();

// Assemblers


builder.Services.AddScoped<ISmappfter, Smappfter>();

// Repositories
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IResaleRepository, ResaleRepository>();
builder.Services.AddScoped<ITicketRepository, TicketRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<ITicketResaleHistoryRepository, TicketResaleHistoryRepository>();
// Repositories
#endregion Services Area

var app = builder.Build();

#region Middlewares Area

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors();

app.UseMiddleware<TimingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();
app.MapControllers();

#endregion Middlewares Area

app.Run();
