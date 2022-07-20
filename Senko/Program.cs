using Data;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Senko.Services;
using Microsoft.EntityFrameworkCore;

namespace Senko
{
	internal class Program
	{
		public static DiscordSocketClient _client;
		private IServiceProvider _services;
		public static Config Config;
		public static SqlMode AppSqlMode { get; set; }
		public enum SqlMode { Sqlite, MariaDb }
		private DiscordCoordinationService DiscordCoordinationService;

		static void Main(string[] args)
		{
			Console.WriteLine($"Directories.MySqlConfigPath = {Directories.MySqlConfigPath}");
			Console.WriteLine($"Directories.Config = {Directories.Config}");
			Console.WriteLine($"Directories.Appdata = {Directories.AppData}");
			Console.WriteLine($"MySql Connection String = {MySqlConfig.FromConfigFile().GetConnectionString().Remove(MySqlConfig.FromConfigFile().GetConnectionString().IndexOf("Pwd=") + 4)}");
			Console.WriteLine();

			try
			{
				new Program().MainAsync(args).GetAwaiter().GetResult();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				Console.WriteLine(e.StackTrace);
				throw;
			}
		}

		private async Task MainAsync(string[] args)
		{
			try
			{
				await Log(new LogMessage(LogSeverity.Info, "Startup", "Initializing bot..."));

				await Log(new LogMessage(LogSeverity.Info, "Startup", "Setting up service providers..."));
				Config = new Config().GetConfigFromFile();

				_client = new DiscordSocketClient(new DiscordSocketConfig
				{
					ConnectionTimeout = 8000,
					HandlerTimeout = 3000,
					MessageCacheSize = 25,
					LogLevel = LogSeverity.Verbose,
					GatewayIntents = GatewayIntents.All
				});
				_services = BuildServices();

				var dbContext = _services.GetRequiredService<SenkoDbContext>();
				this.DiscordCoordinationService = _services.GetRequiredService<DiscordCoordinationService>();

				//apply new database migrations on startup
				var migrations = await dbContext.Database.GetPendingMigrationsAsync();
				if (migrations.Count() > 0)
				{
					Console.WriteLine("Applying database migrations...");
					await dbContext.Database.MigrateAsync();
					Console.WriteLine("Done.");
				}

				await Log(new LogMessage(LogSeverity.Info, "Startup", "Done!"));

				await Log(new LogMessage(LogSeverity.Info, "Startup", "Logging in..."));

				await _client.LoginAsync(TokenType.Bot, Config.Token);

				await _client.StartAsync();

				await Task.Delay(-1);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				Console.WriteLine("===============================");
				Console.WriteLine(e.StackTrace);

				Environment.Exit(1);
			}
		}

		

		public static Task Log(LogMessage msg) => Task.Run(() => Console.WriteLine(msg.ToString()));

		private IServiceProvider BuildServices()
			=> new ServiceCollection()
				.AddSingleton(_client)
				.AddEntityFrameworkMySql()
				.AddSingleton<CommandService>()
				.AddSingleton<LoggingService>()
				.AddDbContext<SenkoDbContext>()
				.AddSingleton<SlashCommandInterationService>()
				.AddSingleton<ComponentInteractionService>()
				.AddSingleton<DiscordCoordinationService>()
				.BuildServiceProvider();
	}
}