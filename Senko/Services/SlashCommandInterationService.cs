using Data;
using Data.DbModels;
using Discord;
using Discord.Net;
using Discord.WebSocket;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Senko.Services
{
	public class SlashCommandInterationService
	{
		private readonly SenkoDbContext DbContext;
		private readonly DiscordSocketClient Client;
		private readonly LoggingService LoggingService;
		public SlashCommandInterationService(SenkoDbContext dbContext, DiscordSocketClient client, LoggingService logger)
		{
			DbContext = dbContext;
			Client = client;
			LoggingService = logger;
			client.GuildAvailable += Client_GuildAvailable;
			//client.Ready += Client_Ready;
			client.SlashCommandExecuted += Client_SlashCommandExecuted;
		}

		

		private async Task Client_SlashCommandExecuted(SocketSlashCommand arg)
		{
			
			switch (arg.Data.Name)
			{
				case "faq":
					await Faq(arg);
					break;
				case "pat":
					await Pat(arg);
					break;
				default:
					await LoggingService.LogGeneral($"An unknown slashcommand was received: {arg.CommandName}");
					break;
			}
		}

		private async Task Client_GuildAvailable(SocketGuild arg)
		{
			SlashCommandProperties[] commands = GetCommands();
			try
			{
				await arg.DeleteApplicationCommandsAsync();
				await arg.BulkOverwriteApplicationCommandAsync(commands);
			}
			catch (HttpException e)
			{
				await LoggingService.LogGeneral($"Error while adding commands", LoggingService.LogGeneralSeverity.Error);
				await LoggingService.LogGeneral(e.ToString(), LoggingService.LogGeneralSeverity.Fatal);
				await LoggingService.LogGeneral(Newtonsoft.Json.JsonConvert.SerializeObject(e.Errors, Newtonsoft.Json.Formatting.Indented), LoggingService.LogGeneralSeverity.Fatal);
				Environment.Exit(1);
			}
		}

		private SlashCommandProperties[] GetCommands()
		{
			List<SlashCommandBuilder> commands = new List<SlashCommandBuilder>();

			#region /Faq
			SlashCommandBuilder faqBuilder = new SlashCommandBuilder();
			faqBuilder.WithName("faq")
				.WithDescription("Opens the FAQ menu");
			commands.Add(faqBuilder);
			#endregion

			#region /Pat
			SlashCommandBuilder patBuilder = new SlashCommandBuilder();
			patBuilder.WithName("pat")
				.WithDescription("Pat the Fox?");
			commands.Add(patBuilder);

			#endregion

			List<SlashCommandProperties> builtCommands = new List<SlashCommandProperties>();
			foreach (SlashCommandBuilder builder in commands)
			{
				builtCommands.Add(builder.Build());
			}
			return builtCommands.ToArray();
		}

		public async Task Faq(SocketSlashCommand command)
		{
			await command.DeferAsync();
			ComponentModel model = new ComponentModel();
			model.CustomId = Guid.NewGuid();
			SelectMenuBuilder menuBuilder = new SelectMenuBuilder();
			menuBuilder.WithPlaceholder("Select an option");
			menuBuilder.WithCustomId(model.CustomId.ToString());
			menuBuilder.AddOption("What happened to Viva? / What is OpenViva?", "about", emote: new Emoji("❓"));
			menuBuilder.AddOption("How do I download?", "download", emote: new Emoji("⏬"));
			menuBuilder.AddOption("How do I find the player log", "log", emote: new Emoji("🗒"));
			menuBuilder.AddOption("Game isn't being detected in SteamVR", "runtime");
			menuBuilder.AddOption("What is the Viva Web address?", "website", emote: new Emoji("🌐"));
			menuBuilder.AddOption("Where can I find the Patreon?", "donate", emote: new Emoji("💰"));
			menuBuilder.AddOption("How do I use the .rar file?", "rar", emote: new Emoji("📇"));
			menuBuilder.AddOption("How do I bind my VR Controls?", "vr-bindings", emote: new Emoji("🎮"));
			menuBuilder.AddOption("Why is the character model invisible?", "invisible", emote: new Emoji("👀"));
			menuBuilder.AddOption("How do I play on the Oculus Quest?", "quest", emote: new Emoji("🗿"));
			menuBuilder.AddOption("The game isn't downloading!", "not-downloading", emote: new Emoji("🔻"));
			menuBuilder.AddOption("Where do I find new characters?", "cards", emote: new Emoji("🔧"));

			ComponentBuilder componentBuilder = new ComponentBuilder();
			componentBuilder.WithSelectMenu(menuBuilder, 0);

			var msg = await command.FollowupAsync("Select an FAQ option:", components: componentBuilder.Build());

			model.ChannelId = command.Channel.Id;
			model.MessageId = msg.Id;
			model.SenkoComponentType = "FAQ-Menu";
			model.OwnerId = command.User.Id;

			DbContext.Components.Add(model);
			await DbContext.SaveChangesAsync();
		}

		public async Task Pat(SocketSlashCommand command)
        {
			await command.DeferAsync();
			ComponentModel model = new ComponentModel();
			model.CustomId = Guid.NewGuid();
			ButtonBuilder Pat = new ButtonBuilder()
			{
				Label = "Pat",
				CustomId = "DoPat",
				Style = ButtonStyle.Success,
			};

			ButtonBuilder dontPat = new ButtonBuilder()
			{
				Label = "Dont Pat",
				CustomId = "DontPat",
				Style = ButtonStyle.Danger,
			};

			ComponentBuilder componentBuilder = new ComponentBuilder();
			componentBuilder.WithButton(Pat);
			componentBuilder.WithButton(dontPat);

			var msg = await command.FollowupAsync("Will you pat Senko?", components: componentBuilder.Build());
			
			//I have no idea how this works
			model.ChannelId = command.Channel.Id;
			model.MessageId = msg.Id;
			model.SenkoComponentType = "SenkoPat";
			model.OwnerId = command.User.Id;

			DbContext.Components.Add(model);
			await DbContext.SaveChangesAsync();
		}

	}

}
