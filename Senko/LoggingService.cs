using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Net;
using System.Text.RegularExpressions;
using System.IO;
using System.Reflection;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Senko
{
	public class LoggingService
	{
		private readonly DiscordSocketClient _client;
		private readonly CommandService _commands;


		public LoggingService(DiscordSocketClient discord, CommandService commands)
		{
			_client = discord;
			_commands = commands;

			_client.Log += LogDiscord;
			_commands.Log += LogCommand;
		}

		public Task LogGeneral(string message, LogGeneralSeverity severity = LogGeneralSeverity.Info)
		{
			
			switch (severity)
			{
				case LogGeneralSeverity.Info:
					Console.ForegroundColor = ConsoleColor.Gray;
					message.Insert(0, $"[INFO] {DateTime.Now}| ");
					break;
				case LogGeneralSeverity.Warning:
					Console.ForegroundColor = ConsoleColor.Yellow;
					message.Insert(0, $"[WARN] {DateTime.Now}| ");
					break;
				case LogGeneralSeverity.Error:
					Console.ForegroundColor = ConsoleColor.Red;
					message.Insert(0, $"[ERROR] {DateTime.Now}| ");
					break;
				case LogGeneralSeverity.Fatal:
					Console.ForegroundColor = ConsoleColor.DarkMagenta;
					message.Insert(0, $"[FATAL] {DateTime.Now}| ");
					break;
			}

			Console.WriteLine(message);
			Console.ForegroundColor = ConsoleColor.Gray;
			return Task.CompletedTask;
		}

		public Task LogCommand(LogMessage message)
		{
			// Return an error message for async commands
			if (message.Exception is CommandException command)
			{
				var _ = command.Context.Channel.SendMessageAsync($"Error: {command.Message}");
			}

			LogDiscord(message);
			return Task.CompletedTask;
		}

		public Task LogDiscord(LogMessage message)
		{
			switch (message.Severity)
			{
				case LogSeverity.Critical:
				case LogSeverity.Error:
					Console.ForegroundColor = ConsoleColor.Red;
					break;
				case LogSeverity.Debug:
				case LogSeverity.Verbose:
					Console.ForegroundColor = ConsoleColor.Gray;
					break;
				case LogSeverity.Warning:
					Console.ForegroundColor = ConsoleColor.Yellow;
					break;
				case LogSeverity.Info:
					Console.ForegroundColor = ConsoleColor.Green;
					break;
				default:
					break;
			}

			Console.WriteLine(message.ToString());
			Console.ResetColor();

			return Task.CompletedTask;
		}

		public enum LogGeneralSeverity { Info, Warning, Error, Fatal }
	}
}
