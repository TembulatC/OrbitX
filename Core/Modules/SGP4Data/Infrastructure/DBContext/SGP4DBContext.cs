using Core.Modules.SGP4Data.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Modules.SGP4Data.Infrastructure.DBContext
{
    public class SGP4DBContext(DbContextOptions<SGP4DBContext> options) : DbContext(options)
    {
        // Таблица TLE в контексте баллистического модуля
        public DbSet<SatelliteTLE> SatellitesTLE { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Настраиваем маппинг для модели SatelliteTLE
            modelBuilder.Entity<SatelliteTLE>(entity =>
            {
                // Жёстко указываем имя таблицы, чтобы оба контекста смотрели в одно место!
                entity.ToTable("Satellites");

                entity.HasKey(s => s.NoradId);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
