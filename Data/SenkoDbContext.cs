using System;
using System.IO;
using System.Reflection;
using Data.DbModels;
using Microsoft.EntityFrameworkCore;
using Data;

namespace Data
{
	public class SenkoDbContext : DbContext
	{
		public DbSet<ComponentModel> Components { get; set; }

		private DatabaseType databaseType;

		public SenkoDbContext(DatabaseType type = DatabaseType.MariaDb)
		{
			this.databaseType = type;
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			switch (this.databaseType)
			{
				case DatabaseType.MariaDb:
					optionsBuilder.UseMySql(MySqlConfig.FromConfigFile().GetConnectionString(), new MariaDbServerVersion("10.4.12"));
					break;
				case DatabaseType.Sqlite:
					if (!File.Exists("Resources\\Data\\KyaruData.db"))
					{
						Directory.CreateDirectory("Resources\\Data");
						File.Create("Resources\\Data\\KyaruData.db");
					}
					optionsBuilder.UseSqlite("Data Source=Resources\\Data\\KyaruData.db;Cache=Shared");
					break;
				case DatabaseType.InMemory:
					throw new NotImplementedException();
				default: 
					throw new ArgumentException("Unsupported database type");
			}
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<ComponentModel>(entity =>
			{
				entity.HasKey(e => e.CustomId);
			});
		}

		public enum DatabaseType { MariaDb, Sqlite, InMemory};
	}
}