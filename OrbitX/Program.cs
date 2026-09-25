using Core.Modules.SatelliteData.Application.Interfaces;
using Core.Modules.SatelliteData.Application.Services;
using Core.Modules.SatelliteData.Domain.Interfaces;
using Core.Modules.SatelliteData.Infrastructure.DBContext;
using Core.Modules.SatelliteData.Infrastructure.HttpClients;
using Core.Modules.SatelliteData.Infrastructure.Repositories;
using Core.Modules.SGP4Data.Application.Interfaces;
using Core.Modules.SGP4Data.Application.Services;
using Core.Modules.SGP4Data.Domain.Interfaces;
using Core.Modules.SGP4Data.Infrastructure.DBContext;
using Core.Modules.SGP4Data.Infrastructure.Repositories;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using OrbitX.BackgroundWorkers;
using OrbitX.BackgroundWorkers.Helper;
using OrbitX.SignalRHubs;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace OrbitX
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Регистрируем политику CORS
            var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();
          
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("OrbitxCorsPolicy", policy =>
                {
                    policy.WithOrigins(allowedOrigins ?? new[] { "http://localhost" }) // fallback на случай отсутствия конфига
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials(); // Критически важно для сокетов SignalR
                });
            });

            builder.Services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                options.KnownProxies.Clear();
                options.KnownNetworks.Clear();
            });

            var customTheme = new AnsiConsoleTheme(new Dictionary<ConsoleThemeStyle, string>
            {
                [ConsoleThemeStyle.Text] = "\x1b[37m",             // Обычный текст - белый
                [ConsoleThemeStyle.SecondaryText] = "\x1b[90m",    // Скобочки и контекст - серый

                [ConsoleThemeStyle.LevelInformation] = "\x1b[36m", // INFO - Cyan (Голубой)
                [ConsoleThemeStyle.LevelWarning] = "\x1b[33m",     // WARN - Желтый
                [ConsoleThemeStyle.LevelError] = "\x1b[31m",       // ERRR - Красный
                [ConsoleThemeStyle.LevelFatal] = "\x1b[35m"        // FTAL - Пурпурный
            });

            Log.Logger = new LoggerConfiguration()
                 .ReadFrom.Configuration(builder.Configuration)
                 .Enrich.FromLogContext()
                 .WriteTo.Console(
                    theme: customTheme, 
                    outputTemplate: "[{Timestamp:HH:mm:ss}] [{Level:u4}] [{SourceContext}] {Message:lj}{NewLine}{Exception}",
                    applyThemeToRedirectedOutput: true
                 )
                 .Filter.ByExcluding("RequestSource = 'Worker or Special' and @l in ['Information', 'Debug', 'Verbose']")
                 .CreateLogger();

            Log.Information("Start OrbitX");

            // Передаем управление логированием хоста в руки Serilog
            builder.Host.UseSerilog();

            // Подключение PostgreSQL
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<OMMDBContext>(options => options.UseNpgsql(connectionString));
            builder.Services.AddDbContext<SGP4DBContext>(options => options.UseNpgsql(connectionString));

            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Регистрируем HttpClient и сам класс клиента
            builder.Services.AddHttpClient<HttpSatellitesData>();

            // Добавление сервисов и репозиториев для модуля SatelliteData
            builder.Services.AddScoped<ISatellitesDataRepository, SatellitesDataRepository>();
            builder.Services.AddScoped<ISatellitesGetDataRepository, SatellitesGetDataRepository>();
            builder.Services.AddScoped<ISatellitesService, SatellitesDataService>();
            builder.Services.AddScoped<ISatellitesGetService, SatellitesGetDataService>();
            builder.Services.AddSingleton<SatellitesParserService>();

            // Добавление сервисов и репозиториев для модуля SGP4
            builder.Services.AddScoped<ISatelliteSGPRepository, SatelliteSGPRepository>();
            builder.Services.AddScoped<ISatelliteSGPServices, SatelliteSGP4Service>();

            // Регистрируем сам класс воркера как Singleton, чтобы DI мог найти его для конструктора Хаба
            builder.Services.AddSingleton<SatelliteBackgroundWorker>();
            // Класс для загрузки спутников всех категорий в бд через воркер
            builder.Services.AddScoped<SatelliteOMMDownloader>();
            // Говорим .NET Core использовать этот же самый Singleton-экземпляр в качестве фонового Hosted-сервиса
            builder.Services.AddHostedService<SatelliteBackgroundWorker>(provider =>
                provider.GetRequiredService<SatelliteBackgroundWorker>());

            // Добавляем инфраструктуру веб-сокетов SignalR
            builder.Services.AddSignalR();

            // Включаем поддержку IMemoryCache
            builder.Services.AddMemoryCache();

            var app = builder.Build();

            // Блок автомиграции
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<OMMDBContext>();

                // Эта команда смотрит на папку Migrations в Core 
                // и применяет их к базе в Docker, если они еще не применены.
                await context.Database.MigrateAsync();
            }

            app.UseForwardedHeaders();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // app.UseHttpsRedirection();

            app.UseCors("OrbitxCorsPolicy"); // Активация CORS
            app.UseAuthorization();
            app.MapControllers();

            // Выделяем адрес для SignalR
            app.MapHub<SignalRHub>("/ws/satellite");

            await app.RunAsync();
        }
    }
}
