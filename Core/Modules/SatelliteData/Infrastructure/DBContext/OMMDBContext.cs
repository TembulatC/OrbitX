using System;
using Microsoft.EntityFrameworkCore;
using Core.Modules.SatelliteData.Domain.Models;

namespace Core.Modules.SatelliteData.Infrastructure.DBContext
{
    public class OMMDBContext(DbContextOptions<OMMDBContext> options) : DbContext(options)
    {
        // Главная таблица
        public DbSet<Satellite> Satellites { get; set; } // Создание главной таблицы

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Указываем NoradId как первичный ключ
            modelBuilder.Entity<Satellite>().HasKey(s => s.NORAD_CAT_ID);

            // Индекс для быстрого поиска по категориям (SpaceX, Couper и т.д.)
            modelBuilder.Entity<Satellite>().HasIndex(s => s.Category);

            base.OnModelCreating(modelBuilder);
        }
    }
}
