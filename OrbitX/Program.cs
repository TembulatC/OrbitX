using Core.Modules.SGP4Data.Application.Interfaces;
using Core.Modules.SGP4Data.Application.Services;
using Core.Modules.SGP4Data.Domain.Interfaces.Repositories;
using Core.Modules.SGP4Data.Domain.Interfaces.Services;
using Core.Modules.SGP4Data.Infrastructure.DBContext;
using Core.Modules.SGP4Data.Infrastructure.Repositories;
using Core.Modules.TLEData.Application.Services;
using Core.Modules.TLEData.Domain.Interfaces.Repositories;
using Core.Modules.TLEData.Domain.Interfaces.Services;
using Core.Modules.TLEData.Infrastructure.DBContext;
using Core.Modules.TLEData.Infrastructure.HttpClients;
using Core.Modules.TLEData.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace OrbitX
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Подключение PostgreSQL
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<TLEDBContext>(options => options.UseNpgsql(connectionString));
            builder.Services.AddDbContext<SGP4DBContext>(options => options.UseNpgsql(connectionString));

            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Регистрируем HttpClient и сам класс клиента
            builder.Services.AddHttpClient<HttpSatellitesData>();

            // Добавление сервисов и репозиториев для модуля TLEData
            builder.Services.AddScoped<ISatellitesDataRepository, SatellitesDataRepository>();
            builder.Services.AddScoped<ISatellitesService, SatellitesDataService>();

            // Добавление сервисов и репозиториев для модуля SGP4
            builder.Services.AddScoped<ISatelliteSGPRepository, SatelliteSGPRepository>();
            builder.Services.AddScoped<ISatelliteSGPServices, SatelliteSGP4Service>();
            builder.Services.AddScoped<ISGP, SatelliteSGP4Service>();

            var app = builder.Build();

            // Блок автомиграции
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var context = services.GetRequiredService<TLEDBContext>();

                // Эта команда смотрит на папку Migrations в Core 
                // и применяет их к базе в Docker, если они еще не применены.
                context.Database.Migrate();
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
