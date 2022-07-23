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
		private readonly WelcomeMessageService WelcomeMessageService;

		public DiscordCoordinationService(DiscordSocketClient client, SlashCommandInterationService slashCommandService, ComponentInteractionService componentInteractionService, LoggingService loggingService, WelcomeMessageService welcomeMessageService)
        {
            Client = client;
            SlashCommandService = slashCommandService;
            ComponentInteractionService = componentInteractionService;
            LoggingService = loggingService;
			WelcomeMessageService = welcomeMessageService;

			Client.Ready += OnReady;  
        }

        private async Task OnReady()
		{
			await Client.SetGameAsync("Senko's here to help!");

			await LoggingService.LogGeneral("Startup Complete");
			await LoggingService.LogGeneral($"Logged in as {Client.CurrentUser.Username}");
		}
	}
}
