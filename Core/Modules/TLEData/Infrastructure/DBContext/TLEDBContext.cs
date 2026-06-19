using System;
using Microsoft.EntityFrameworkCore;
using Core.Modules.TLEData.Domain.Models;

namespace Core.Modules.TLEData.Infrastructure.DBContext
{
    public class TLEDBContext(DbContextOptions<TLEDBContext> options) : DbContext(options)
    {
        // Главная таблица
        public DbSet<Satellite> Satellites { get; set; } // Создание главной таблицы

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Указываем NoradId как первичный ключ
            modelBuilder.Entity<Satellite>().HasKey(s => s.NoradId);

            // Ограничиваем длину TLE строк (стандарт ~70 символов)
            modelBuilder.Entity<Satellite>().Property(s => s.TLELine1).HasMaxLength(70);
            modelBuilder.Entity<Satellite>().Property(s => s.TLELine2).HasMaxLength(70);

            // Индекс для быстрого поиска по категориям (SpaceX, Couper и т.д.)
            modelBuilder.Entity<Satellite>().HasIndex(s => s.Category);

            base.OnModelCreating(modelBuilder);
        }
    }
}
