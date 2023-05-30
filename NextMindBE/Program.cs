using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NextMindBE;
using NextMindBE.Data;
using NextMindBE.Interfaces.Repostory;
using NextMindBE.Interfaces.Service;
using NextMindBE.Model;
using NextMindBE.Repositories;
using NextMindBE.Services;
using Swashbuckle.AspNetCore.Filters;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(options =>
{
    //options.JsonSerializerOptions.Converters.Add(new ByteArrayConverter());
});

builder.Services.AddDbContext<ApplicationDbContext>(
    options =>
    {
        options.UseMySQL(StartupHelper.GetConnectionString(builder.Configuration)) ;
    });

builder.Services.AddResponseCaching();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "Standard Authorization header using the Bearer scheme (\"bearer {token}\")",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

// Repositories
builder.Services.AddScoped<ISessionRepository, SessionRepository>();

// Services
builder.Services.AddScoped<ICipher, CipherService>();
builder.Services.AddScoped<ISessionService, SessionService>();
builder.Services.AddScoped<IValidatorService, ValidatorService>();
builder.Services.AddScoped<IProcessingService, ProcessingService>();

// Validators
builder.Services.AddScoped<IValidator<SensorData>, SensorDataValidator>();
builder.Services.AddScoped<IValidator<float>, PulseDataValidator>();


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            // ValidIssuer = builder.Configuration["Jwt:Issuer"],
            // ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:Token"]))
        };
    });


var app = builder.Build();

app.UseCors(
    options => options.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()
);


using (var scope = app.Services.CreateScope())
{
    Console.WriteLine("Applying migrations.");
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
    Console.WriteLine("Migrations applied.");
}
// Configure the HTTP request pipeline.
// Sif (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseResponseCaching();

// app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

PingTimerManager.StartTimer();

app.Run();

