using Microsoft.EntityFrameworkCore;
using RfidAccessSystem.Data;
using RfidAccessSystem.Services;
using RfidAccessSystem.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Scoped Services
builder.Services.AddScoped<IAccessService, AccessService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ILogService, LogService>();
builder.Services.AddScoped<IReportService, ReportService>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

// Middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<ApiKeyAuthenticationMiddleware>();

//app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();
app.UseDefaultFiles();
app.UseStaticFiles();

// Seed Database
using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        context.Database.EnsureCreated();
        DbInitializer.Initialize(context);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error initializing database: {ex.Message}");
        Console.WriteLine($"Stack Trace: {ex.StackTrace}");
    }
}

Console.WriteLine("Starting application...");
app.Urls.Add("http://localhost:5000");
app.Run();

public class DbInitializer
{
    public static void Initialize(ApplicationDbContext context)
    {
        // Check if already has data
        if (context.Users.Any())
            return;

        // Seed Users
        var users = new[]
        {
            new { UID = "A1B2C3D4", FullName = "John Doe", Email = "john@example.com", Role = "Employee", Department = "IT", IsActive = true },
            new { UID = "E5F6G7H8", FullName = "Jane Smith", Email = "jane@example.com", Role = "Employee", Department = "HR", IsActive = true },
            new { UID = "I9J0K1L2", FullName = "Bob Johnson", Email = "bob@example.com", Role = "Visitor", Department = "Admin", IsActive = true }
        };

        foreach (var u in users)
        {
            context.Users.Add(new RfidAccessSystem.Models.Entities.User
            {
                UID = u.UID,
                FullName = u.FullName,
                Email = u.Email,
                Role = u.Role,
                Department = u.Department,
                IsActive = u.IsActive,
                CreatedAt = DateTime.Now
            });
        }

        context.SaveChanges();

        // Seed API Keys
        context.ApiKeys.Add(new RfidAccessSystem.Models.Entities.ApiKey
        {
            ClientName = "Frontend App",
            KeyValue = "RFID_ABC123XYZ789",
            IsActive = true,
            CreatedAt = DateTime.Now
        });

        context.SaveChanges();
    }
}
