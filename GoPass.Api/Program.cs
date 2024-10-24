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
using GoPass.Domain.DTOs.Request.NotificationDTOs;
using GoPass.API.Middlewares;
using GoPass.Infrastructure.UnitOfWork;
using GoPass.Application.ServiceFacade;
using GoPass.Application.Utilities.Mappers;
using GoPass.Application.Services.Validations.Interfaces;
using GoPass.Application.Services.Validations.Classes;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

#region Services Area
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers()
            .AddJsonOptions(options => {options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;});

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

builder.Services.AddScoped<IUserValidationService, UserValidationService>();

// Services
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IReventaService, ReventaService>();
builder.Services.AddScoped<IEntradaService, EntradaService>();
builder.Services.AddScoped<ITicketMasterService, TicketSimulatorService>();
builder.Services.AddScoped<IAesGcmCryptoService, AesGcmCryptoService>();
builder.Services.AddScoped<ITemplateService, TemplateService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IServiceFacade, ServiceFacade>();

builder.Services.AddSingleton<IVonageSmsService, VonageSmsService>();
// Services

builder.Services.AddScoped<ICustomAutoMapper, CustomAutoMapper>();

builder.Services.AddSingleton<GoPass.Application.Notifications.Interfaces.ISubject<string>, Subject<string>>();

builder.Services.AddTransient<GoPass.Application.Notifications.Interfaces.IObserver<NotificationEmailRequestDto>, BuyerEmailNotificationObserver>();
builder.Services.AddTransient<GoPass.Application.Notifications.Interfaces.IObserver<NotificationEmailRequestDto>, SellerEmailNotificationObserver>();

// Repositories
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IReventaRepository, ReventaRepository>();
builder.Services.AddScoped<IEntradaRepository, EntradaRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IHistorialCompraVentaRepository, HistorialCompraVentaRepository>();
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

app.UseUserMiddlewares();

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();
app.MapControllers();

#endregion Middlewares Area

app.Run();
