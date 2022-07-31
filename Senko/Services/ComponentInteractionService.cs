using Data;
using Data.DbModels;
using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Senko.Services
{
	public class ComponentInteractionService
	{
		private DiscordSocketClient Client;
		private SenkoDbContext DbContext;

		private Discord.Color VivaBlueColor = new Discord.Color(68, 78, 127);
		private Discord.Color VivaTanColor = new Discord.Color(245, 201, 118);

		public ComponentInteractionService(DiscordSocketClient client, SenkoDbContext dbContext)
		{
			Client = client;
			DbContext = dbContext;

			client.SelectMenuExecuted += Client_SelectMenuExecuted;
			client.ButtonExecuted += Client_ButtonExecuted;
		}

		private async Task Client_ButtonExecuted(SocketMessageComponent arg)
		{
			await arg.DeferAsync();

			//ComponentModel model = DbContext.Components.Find(Guid.Parse(arg.Data.CustomId));

			//if (arg.User.Id != model.OwnerId)
			//{
			//	await arg.RespondAsync("You cannot control a component generated for another user.", ephemeral: true);
			//	return;
			//}

			EmbedBuilder builder = new EmbedBuilder();

			switch (arg.Data.CustomId)
			{
				case "DoPat":
					builder.WithAuthor(arg.User);
					builder.WithDescription("You have patted Senko! \n:)");
					builder.WithImageUrl("https://cdn.discordapp.com/attachments/976660916831662113/1003077839882551356/senko-pat.gif");
					builder.WithColor(Color.Green);
					await arg.FollowupAsync(embed: builder.Build(), ephemeral: true);
					break;
				case "DontPat":
					builder.WithAuthor(arg.User);
					builder.WithDescription("You have failed to pat Senko. \nThe Fox is now sad :(");
					builder.WithImageUrl("https://cdn.discordapp.com/attachments/976660916831662113/1003078352535552040/sad-senko.gif");
					builder.WithColor(Color.DarkRed);
					await arg.FollowupAsync(embed: builder.Build(), ephemeral: true);
					break;
			}
			
			
		}

		private async Task Client_SelectMenuExecuted(SocketMessageComponent arg)
		{
			await arg.DeferAsync();

			ComponentModel model = DbContext.Components.Find(Guid.Parse(arg.Data.CustomId));

			EmbedBuilder builder = new EmbedBuilder();

			switch (model.SenkoComponentType)
			{
				case "FAQ-Menu":

					if (arg.User.Id != model.OwnerId)
					{
						await arg.RespondAsync("**Unauthorized**: This faq menu belongs to another user.", ephemeral: true);
						return;
					}

					switch (arg.Data.Values.FirstOrDefault())
					{
						case "about":
							builder.WithAuthor(arg.User);
							builder.WithTitle("What happened to Viva? / What is OpenViva?");
							builder.WithDescription("Original Viva created by Sir Hal has been abandoned in March 2022. Thanks to original author's generosity we've received its source code and started OpenViva Project. Viva is a FOSS VR and non-VR compatible game where you can interact with your very own AI anime character!");
							builder.AddField("__**Website**__", "Official OpenViva website can be found [here](https://viva-project.org/)");
							builder.AddField("__**Github**__", "OpenViva Source Code can be found on our [Github](https://github.com/OpenViva/OpenViva/releases)");
							builder.AddField("__**Itch.io**__", "Legacy Viva versions can be found on [Itch.io](https://shinobuproject.itch.io/game)");
							builder.WithColor(VivaBlueColor);
							builder.WithUrl("https://viva-project.org/");
							builder.WithCurrentTimestamp();
							builder.WithFooter("/Help for commands");
							break;

						case "download":
							builder.WithAuthor(arg.User);
							builder.WithTitle("How to download OpenViva");
							builder.WithDescription("OpenViva releases are free, if you were forced to pay for a release, you were scammed");
							builder.AddField("__**Website**__", "Official OpenViva Releases can be found on our [Website](https://viva-project.org/download)");
							builder.AddField("__**Github**__", "Source code and alternative downloads can be found on [Github](https://github.com/OpenViva/OpenViva/releases)");
							builder.AddField("__**Itch.io**__", "Legacy versions can be found on [Itch.io](https://shinobuproject.itch.io/game)");
							builder.WithColor(VivaBlueColor);
							builder.WithUrl("https://viva-project.org/download");
							builder.WithCurrentTimestamp();
							builder.WithFooter("/Help for commands");
							break;

						case "log":
							builder.WithAuthor(arg.User);
							builder.WithTitle("How do I find the player log");
							builder.WithDescription("Player logs should be stored in **C:/Users/USERNAMEHERE/AppData/LocalLow/OpenViva/VivaProject**");
							builder.AddField("NOTE:","Make sure to replace **USERNAMEHERE** with your pc username.");
							builder.WithColor(VivaTanColor);
							builder.WithUrl("https://viva-project.org/");
							builder.WithCurrentTimestamp();
							builder.WithFooter("/Help for commands");
							break;

						case "runtime":
							builder.WithAuthor(arg.User);
							builder.WithTitle("Game isn't being detected in SteamVR");
							builder.WithDescription("This may be due to SteamVR not being set as your active OpenXR Runtime");
							builder.AddField("__**How To:**__", "Go to your SteamVR settings and enable advanced settings\n Go to Developer settings and click **SET STEAMVR AS OPENXR RUNTIME** if it is not already set.");
							builder.WithColor(VivaBlueColor);
							builder.WithCurrentTimestamp();
							builder.WithFooter("/Help for commands");
							builder.WithImageUrl("https://cdn.discordapp.com/attachments/322830540191432705/1002223309909413998/OpenXRRuntime.png?size=4096");
							break;

						case "website":
							builder.WithAuthor(arg.User);
							builder.WithTitle("Visit our web page!");
							builder.WithDescription("[Viva-Project.org](https://viva-project.org/)\n" +
								"Instructions on how to install your character/clothing cards are included in the .rar file.");
							builder.AddField("__**Download Character Cards**__", "[Viva-Project.org/Cards/Characters](https://viva-project.org/assets?category=character)");
							builder.AddField("__**Download Clothing Cards**__", "[Viva-Project.org/Cards/Clothes](https://viva-project.org/assets?category=clothing)");
							builder.AddField("__**Requesting Cards**__", "You can request cards in OpenViva Discord in #characters-requests channel");
							builder.WithColor(VivaTanColor);
							builder.WithUrl("https://viva-project.org/");
							builder.WithCurrentTimestamp();
							builder.WithFooter("/Help for commands");
							break;

						case "donate":
							builder.WithAuthor(arg.User);
							builder.WithTitle("Support us");
							builder.WithDescription("We currently do not have any donation services set up however you can still support us!\n" +
								"You can support us by sharing with your friends, boosting the server, finding bugs, making pull requests and leaving a star on github!");
							builder.WithColor(VivaBlueColor);
							builder.WithCurrentTimestamp();
							builder.WithFooter("/Help for commands");
							break;

						case "rar":
							builder.WithAuthor(arg.User);
							builder.WithTitle("How to Extract Viva from a RAR file");
							builder.WithDescription("Windows does not natively support RAR extractions, but a program called \"7-Zip\" can help! \n*Note: Viva is not supported on x86 systems*");
							builder.AddField("__**Step 1: Download 7-Zip**__", "7-zip (7z) is a free and open source archive tool. Download the x64bit version [here](https://www.7-zip.org/)");
							builder.AddField("__**Step 2: Download Viva if you haven't already**__", "If you do not know where to download Viva, type */Download* for a link to our download page");
							builder.AddField("__**Step 3: Extract**__", "Find your downloaded .rar file. It's probably in your /Downloads directory. Right-click the .rar file. If 7-Zip is properly installed you should see a menu option. \n\n" +
								"Select the option that says \"7-Zip\". A second menu should pop out. Select the option *Extract to /Viva....*.");
							builder.AddField("__**Step 4: Launch the game**__", "You should now have a directory called \"Viva...\" in your /Downloads directory. Within that directory is an .exe named *viva.exe*. Run the exe to start the game.\n" +
								"*Note: Future releases may be shipped as a 7-zip self extracting archives. In that case, you only need to run the .exe and it will extract itself.");
							builder.WithColor(VivaTanColor);
							builder.WithCurrentTimestamp();
							builder.WithFooter("/Help for commands");
							builder.WithImageUrl("https://viva-project.org/static/faq/viva-project-7z.png");
							break;

						case "vr-bindings":
							builder.WithAuthor(arg.User);
							builder.WithTitle("How to Bind your VR Controls");
							builder.WithDescription("Open the Menu Book (press tab or p, depending on version), then \"Switch to VR\", then follow the instructions in the following image");
							builder.WithColor(VivaTanColor);
							builder.WithCurrentTimestamp();
							builder.WithFooter("/Help for commands");
							builder.WithImageUrl("https://viva-project.org/static/faq/Viva-Project-Bindings.png");
							break;

						case "invisible":
							builder.WithAuthor(arg.User);
							builder.WithTitle("Invisible Characters/Models");
							builder.WithDescription("If you are seeing something like the following image, it is likely because you do not meet the minimum requirements to run Viva Project.");
							builder.AddField("Minimum Requirements", "View the minimum system requirements [here](https://support.humblebundle.com/hc/en-us/articles/360035266854-Humble-Unity-Bundle-2019-System-Requirements-)");
							builder.WithColor(VivaTanColor);
							builder.WithCurrentTimestamp();
							builder.WithFooter("/Help for commands");
							builder.WithImageUrl("https://viva-project.org/static/faq/outdated-gc.png");
							break;

						case "black-screen":
							builder.WithAuthor(arg.User);
							builder.WithTitle("Black screen in VR");
							builder.WithDescription("If you are seeing a black screen in your VR headset, but the game is rendering properly on desktop, you need to press 'p', then select 'Switch to VR'");
							builder.AddField("Note:", "You must have SteamVR installed");
							builder.WithColor(VivaBlueColor);
							builder.WithCurrentTimestamp();
							builder.WithFooter(GetFooter());
							break;

						case "quest":
							builder.WithAuthor(arg.User);
							builder.WithTitle("Oculus Quest");
							builder.WithDescription("Oculus Quest support may be added in the future");
							builder.AddField("OpenViva", "You must link your quest to your PC to play in vr on your Quest.");
							builder.WithColor(VivaBlueColor);
							builder.WithCurrentTimestamp();
							builder.WithFooter(GetFooter());
							break;

						case "not-downloading":
							builder.WithAuthor(arg.User);
							builder.WithTitle("I cannot download the game");
							builder.WithDescription("If you cannot download the game from our website, try the following workaround(s)");
							builder.AddField("Use a different browser", "Some browsers are just broken. Not much we can do about that.\nTry downloading from our [Github](https://github.com/OpenViva/OpenViva/releases).\nTry asking in our discord server.");
							builder.WithColor(VivaBlueColor);
							builder.WithCurrentTimestamp();
							builder.WithFooter(GetFooter());
							break;

						case "cards":
							builder.WithAuthor(arg.User);
							builder.WithTitle("Cards");
							builder.WithDescription("Character, clothing, and skin cards are currently the only way to add content into OpenViva");
							builder.AddField("Cards", "The method for creating cards in viva 0.8 is the same method we use in OpenViva so cards should be compatible.");
							builder.WithColor(VivaTanColor);
							builder.WithCurrentTimestamp();
							builder.WithFooter(GetFooter());
							break;
					}

					await arg.Message.ModifyAsync(x =>
					{
						x.Components = null;
						x.Content = "";
						x.Embed = builder.Build();
					});
					break;

			}

			DbContext.Components.Remove(model);
			await DbContext.SaveChangesAsync();
		}

		private EmbedFooterBuilder GetFooter()
		{
			EmbedFooterBuilder builder = new EmbedFooterBuilder();
			builder.IconUrl = Program._client.CurrentUser.GetAvatarUrl();
			builder.Text = "Type /help for a list of commands";
			return builder;
		}
	}
}
