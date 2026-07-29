using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QRQueueDecanat.Data;
using QRQueueDecanat.ExceptionHandling;
using QRQueueDecanat.Extensions;
using QRQueueDecanat.Entities;
using QRQueueDecanat.Hubs;
using QRQueueDecanat.Interfaces;
using QRQueueDecanat.Services;

var FrontendCorsPolicy = "FrontendCors";
var builder = WebApplication.CreateBuilder(args);
var queueTimeZoneId = builder.Configuration["Queue:TimeZoneId"]
    ?? "Asia/Yekaterinburg";
var queueTimeZone =
    TimeZoneInfo.FindSystemTimeZoneById(queueTimeZoneId);
builder.Services.AddSignalR();
builder.Services.AddSingleton<TimeZoneInfo>(queueTimeZone);
builder.Services.AddScoped<IOperatorTicketsService, OperatorTicketsService>();
builder.Services.AddScoped<IOperatorSessionService, OperatorSessionService>();
builder.Services.AddScoped<IOperatorSettingsService, OperatorSettingsService>();
builder.Services.AddScoped<IPanelService, PanelService>();
builder.Services.AddScoped<IServiceCatalogService, ServiceCatalogService>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<IQueueNotifier, QueueNotifier>();
builder.Services.AddProblemDetails(); 
builder.Services.AddExceptionHandler<AppExceptionHandler>();
builder.Services.AddApplicationAuthentication(builder.Configuration);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options
        .UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
        .UseSnakeCaseNamingConvention()
);
builder.Services.AddCors(options =>
{
    options.AddPolicy(FrontendCorsPolicy,
        policy =>
        {
            policy.WithOrigins("http://localhost:5173")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

var app = builder.Build();
app.UseExceptionHandler();
app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    await using var scope = app.Services.CreateAsyncScope();
    var context = scope.ServiceProvider
        .GetRequiredService<ApplicationDbContext>();
    var passwordHasher = scope.ServiceProvider
        .GetRequiredService<
            IPasswordHasher<AppUser>
        >();
    await context.Database.MigrateAsync();
    await DbInitialiser.SeedAsync(
        context,
        passwordHasher
    );

    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors(FrontendCorsPolicy);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<QueueHub>("/hubs/queue");
app.Run();
