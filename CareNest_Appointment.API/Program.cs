using CareNest_Appointment.API.Middleware;
using CareNest_Appointment.Application.Common;
using CareNest_Appointment.Application.Common.Options;
using CareNest_Appointment.Application.Features.Commands.Create;
using CareNest_Appointment.Application.Features.Commands.Delete;
using CareNest_Appointment.Application.Features.Commands.Update;
using CareNest_Appointment.Application.Features.Commands.UpdateTotalAmount;
using CareNest_Appointment.Application.Features.Queries.GetAllPaging;
using CareNest_Appointment.Application.Features.Queries.GetById;
using CareNest_Appointment.Application.Interfaces.CQRS;
using CareNest_Appointment.Application.Interfaces.CQRS.Commands;
using CareNest_Appointment.Application.Interfaces.CQRS.Queries;
using CareNest_Appointment.Application.Interfaces.Services;
using CareNest_Appointment.Application.Interfaces.UOW;
using CareNest_Appointment.Application.UseCases;
using CareNest_Appointment.Domain.Repositories;
using CareNest_Appointment.Infrastructure.Persistences.Configuration;
using CareNest_Appointment.Infrastructure.Persistences.Database;
using CareNest_Appointment.Infrastructure.Persistences.Repository;
using CareNest_Appointment.Infrastructure.Services;
using CareNest_Appointment.Infrastructure.UOW;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
// Lấy DatabaseSettings từ configuration
DatabaseSettings dbSettings = builder.Configuration.GetSection("DatabaseSettings").Get<DatabaseSettings>()!;
dbSettings.Display();
string connectionString = dbSettings?.GetConnectionString();


// Đăng ký DbContext với PostgreSQL
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(5),
            errorCodesToAdd: null);
    }));

builder.Services.AddTransient<DatabaseSeeder>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// Đăng ký service thêm chú thích cho api
builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

    //ADD JWT BEARER SECURITY DEFINITION
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Nhập token theo định dạng: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        //Type = SecuritySchemeType.ApiKey,
        Type = SecuritySchemeType.Http,//ko cần thêm token phía trước
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                In = ParameterLocation.Header,
                Name = "Bearer",
                Scheme = "Bearer"
            },
            new List<string>()
        }
    });
});

// Đăng ký các repository
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
//command
builder.Services.AddScoped<ICommandHandler<CreateCommand, AppointmentResponse>, CreateCommandHandler>();
builder.Services.AddScoped<ICommandHandler<UpdateCommand, AppointmentResponse>, UpdateCommandHandler>();
builder.Services.AddScoped<ICommandHandler<DeleteCommand>, DeleteCommandHandler>();
builder.Services.AddScoped<ICommandHandler<UpdateTotalAmountCommand, AppointmentResponse>, UpdateTotalAmountCommandHandler>();
//query
builder.Services.AddScoped<IQueryHandler<GetAllPagingQuery, PageResult<AppointmentResponse>>, GetAllPagingQueryHandler>();
builder.Services.AddScoped<IQueryHandler<GetByIdQuery, AppointmentResponse>, GetByIdQueryHandler>();

builder.Services.Configure<RouteOptions>(options =>
{
    options.LowercaseUrls = true;
});

builder.Services.Configure<APIServiceOption>(builder.Configuration.GetSection("APIService"));

builder.Services.AddHttpClient();

builder.Services.AddScoped<IShopService, ShopService>();
builder.Services.AddScoped<IAPIService, APIService>();
builder.Services.AddScoped<IAppointmentDetailService, AppointmentDetailService>();
builder.Services.AddScoped<IAuthorizeService, AuthorizeService>();
builder.Services.AddScoped<IEmailService, EmailService>();

// Add authentication services
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,  // Tạm thời tắt validate issuer
        ValidateAudience = false, // Tạm thời tắt validate audience
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]))
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            if (context.Request.Headers.ContainsKey("Authorization"))
            {
                var bearer = context.Request.Headers["Authorization"].ToString();
                Console.WriteLine($"Authorization Header: {bearer}");
                if (bearer.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    context.Token = bearer.Substring("Bearer ".Length).Trim();
                    Console.WriteLine($"Extracted Token: {context.Token}");
                }
            }
            else
            {
                Console.WriteLine("No Authorization Header Found");
            }
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            var claims = context.Principal?.Claims;
            if (claims != null)
            {
                Console.WriteLine("Token Claims:");
                foreach (var claim in claims)
                {
                    Console.WriteLine($"Type: {claim.Type}, Value: {claim.Value}");
                }
            }
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"Authentication failed: {context.Exception.Message}");
            return Task.CompletedTask;
        }
    };
});


builder.Services.AddScoped<IUseCaseDispatcher, UseCaseDispatcher>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder
                .SetIsOriginAllowed(_ => true)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
    context.Database.Migrate();
}

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
app.UseRouting();
app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();