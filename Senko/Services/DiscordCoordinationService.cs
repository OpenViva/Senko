using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Senko.Services
{
	public class DiscordCoordinationService
	{
		private readonly DiscordSocketClient Client;
		private readonly SlashCommandInterationService SlashCommandService;
		private readonly ComponentInteractionService ComponentInteractionService;
		private readonly LoggingService LoggingService;

		public DiscordCoordinationService(DiscordSocketClient client, SlashCommandInterationService slashCommandService, ComponentInteractionService componentInteractionService, LoggingService loggingService)
		{
			Client = client;
			SlashCommandService = slashCommandService;
			ComponentInteractionService = componentInteractionService;
			LoggingService = loggingService;


			Client.Ready += OnReady;
		}

		private async Task OnReady()
		{
			await Client.SetGameAsync("I'm being helpful!");

			await LoggingService.LogGeneral("Startup Complete");
			await LoggingService.LogGeneral($"Logged in as {Client.CurrentUser.Username}");
		}
	}
}
