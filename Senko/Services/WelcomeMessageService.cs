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
    public class WelcomeMessageService
    {
        private DiscordSocketClient Client;
        private SenkoDbContext DbContext;

        public WelcomeMessageService(DiscordSocketClient client, SenkoDbContext dbContext)
        {
            Client = client;
            DbContext = dbContext;

            client.UserJoined += SendWelcomeMessageAsync;
        }

        //THE MESSAGES AND IMAGES MUST BE IN ORDER
        private String[] WelcomeMessages = new String[]
        {
            "Welcome to OpenViva <@>",
            "hey, <@>",
            "Please refrain from drowning lolis in the onsen, <@>",
            "You'd best be watchin that mouth, <@>",
            "<@>, Welcome to OpenViva",
            "And Remember <@>, no cringe!",
            "Welcome to the autism house <@>",
            "Don't forget to pet your loli everyday <@>",
            "Dont worry <@>, everyones retarded here!",
            "Attention <@>: If you or a loved one has been diagnosed with Mesothelioma you may to be entitled to financial compensation. Mesothelioma is a rare cancer linked to asbestos exposure. Exposure to asbestos in they Navy, shipyards, mills, heating, construction or the automotive industries may put you at risk. Please don't wait, call 1-800-99 LAW USA today for a free legal consultation and financial information packet. Mesothelioma patients call now! 1-800-99 LAW USA",
            "ahh!, <@> you scared me!",
            "<@> lives in a yellow submarine, a yellow submarine, yes a yellow submarine.",
            "<@>, dont be a diaperpole.",
            "Start working out already, <@>",
            "<@> watch out, the power of god and anime is on my side",
            "Hello, <@>. Run while you still can.",
            "Welcome to Foxify's foster home for retards <@>",
            "Hey, <@>. Just so you know...",
            "Do you think twitter was a mistake? <@>",
            "Oh, its just <@>",
            "Get the fuck out, <@>, no one invited you. Yo who the hell didn't set a time limit on the invite?",
            "Hey <@>, are you lonely, retarded and NEET? Welcome home.",
            "lmao hide the drugs <@> is here",
            "<@>",
            "Don't cry anymore <@>, your waifu is real now.",
            "Pepper your angus, <@>, the lolis are revolting.",
            "<@>, assume the position.",
            "I've got bad news for you, <@>",
            "Pants down <@>",
            "Welcome, <@> Please tell us all about your home address and hard drive contents.",
            "It seems you are worthy of my full strength, <@>",
            "<@> As long as there are too primates left alive, somebody is going to want someone dead",
            "<@> go the fuck away you little shit",
            "Hoi <@>! There is an epic neko party happening right nyow! Join them if you want to have a nyice time!",
            "<@>? What are you doing here onii-chan?",
            "Oh look who showed up, <@>",
            "H-hey, <@>",
            "We got you on 42 different counts of loli abuse <@>. You may deliver your testimony before the jury",
            "<@> has come for a mega bonking!",
            "Oh hey, <@>! Welcome to the bondage party!",
            "You picked a bad time to get lost, <@>",
            "<@> joined!",
            "<@> welcome to mf rice fields",
            "<@>, hold that thought-",
            "Hello, <@>. Remember you are here forever.",
            "Welcome to the watchlist, <@>",
            "<@> The Senate will decide your fate",
            "Welcome to OpenViva, <@>! No Fentanyl allowed.",
            "By joining this server, you agree to being put on a government watch list, <@>",
            "<@> please, no cringe anymore...",
            "<@> forgot to say 'in minecraft'",
            "<@>, dont be a koomquat.",
            "Are you sure you can extract RARs <@>",
            "Hey, <@>! I found this weird machine, can you step\ninside and see what will happen?\nPlease, I really want to find out!",
            "Fun's over, <@> is here",
            "Why hello there <@>",
            "Hey <@>! Check fucking #📔-rules",
            "We'll now begin the push up section <@>.\nReady? Begin.\nDown, up 1\nDown, up 2\nDown, up 3\nDown, up 4\nDown, up 5",
            "Hey <@>, don't mind the feet goblins.",
            "Quick, <@> say something funny",
            "Gee,,, i wonder who could be behind <@>",
            "<@>, this is required listening. https://youtu.be/miXRKvbTFfM",
            "<@> Shinobu needs your help to break into the doughnut vault! The password is your social security number! Make sure you type it into #password",
            "It's not like I want <@> in this server or anything, b-baka!",
            "<@> is the antichrist",
            "I greet you <@>",
            "Who invited <@>?!?!",
            "<@> glows like the sun.",
            "GUYYS <@> IS HERE, you know what to do...",
            "<@> Don't put memes in general or i'll kick your ass",
            "<@> came to the wrong house fool! AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA",
            "Welcome <@>!\nThis is not your feet pic server, remember that.",
            "TFW <@> joined",
            "Don't worry, <@>, you're in good company. We're all retards here.",
            "<@> dont be a brendan.",
            "Where are you hiding <@>? Now, don't be shy~...",
            "Welcome to the league of Discord Mods, <@>",
            "Remember <@>, if your 3d printer jizzes plastic into the wrong shape, you're a felon.",
            "<@>, I'm trying to sneak around, but I'm dummy thicc and the clap of my ass cheeks keeps alerting the guards!",
            "How based are you <@>",
            "Hey <@>, don't drill the third hole.",
            "Another cringelord <@> has shown up!",
            "Oh, its just <@>",
            "Oh so you like anime <@>? Name every anime."
        };
        public Dictionary<int, string> _associatedImageLinks = new Dictionary<int, string>
        {
            {0, ""},
            {1, "https://cdn.discordapp.com/attachments/850561678982774814/850563302110003200/ActingSus.jpg?size=4096" },
            {2, ""},
            {3, "https://cdn.discordapp.com/attachments/850561678982774814/850568299342725150/Screenshot_2021-05-27_19.03.25.png?size=4096" },
            {4, "https://cdn.discordapp.com/attachments/850561678982774814/850574804645314590/image0-14.jpg?size=4096" },
            {5, ""},
            {6, ""},
            {7, ""},
            {8, ""},
            {9, ""},
            {10, ""},
            {11, ""},
            {12, ""},
            {13, "https://cdn.discordapp.com/attachments/850561678982774814/850568007046791178/14617893942523.png?size=4096"},
            {14, ""},
            {15, ""},
            {16, ""},
            {17, "https://cdn.discordapp.com/attachments/850561678982774814/850577233621155850/5b2.png?size=4096"},
            {18, ""},
            {19, ""},
            {20, ""},
            {21, ""},
            {22, ""},
            {23, ""},
            {24, ""},
            {25, "https://cdn.discordapp.com/attachments/850561678982774814/850575907932209162/Hw3xj64.png?size=4096"},
            {26, "https://cdn.discordapp.com/attachments/850561678982774814/850575192900370472/image0.jpg?size=4096"},
            {27, "https://cdn.discordapp.com/attachments/850561678982774814/850571063234527273/Screenshot_2021-04-13_03.43.17.png?size=4096"},
            {28, "https://cdn.discordapp.com/attachments/850561678982774814/850567600375463936/yH4YpuQ-2.gif?size=4096" },
            {29, "https://cdn.discordapp.com/attachments/850561678982774814/850569975801315328/GettyImages-1184836915.png?size=4096" },
            {30, "https://cdn.discordapp.com/attachments/850561678982774814/850573688146362368/1576833891023.jpg?size=4096" },
            {31, "https://cdn.discordapp.com/attachments/850561678982774814/850574095796011028/Screenshot_2021-01-19_05.51.30.png?size=4096" },
            {32, "https://cdn.discordapp.com/attachments/850561678982774814/850577818970095656/image0.gif?size=4096" },
            {33, "https://media.discordapp.net/attachments/826973115765489684/849484602799030293/tenor1.gif" },
            {34, "https://cdn.discordapp.com/attachments/850561678982774814/850572070374998106/Screenshot_2021-03-21_16.57.47.png?size=4096" },
            {35, "https://cdn.discordapp.com/attachments/850561678982774814/850581274955612191/IMG_20200626_192228.jpg?size=4096"},
            {36, "https://media.discordapp.net/attachments/585066060722470937/830013940729511946/can_i_have_credit_card_info_pls.gif"},
            {37, "https://cdn.discordapp.com/attachments/850561678982774814/850584689046847498/paperslap.gif?size=4096"},
            {38, "https://media.discordapp.net/attachments/817476126016143393/834973708749373450/1534586662341.gif"},
            {39, "https://cdn.discordapp.com/attachments/850561678982774814/850576785585340456/336.png?size=4096"},
            {40, "https://cdn.discordapp.com/attachments/850561678982774814/850576435415744592/image0.jpg?size=4096"},
            {41, "https://cdn.discordapp.com/attachments/850561678982774814/850572283080081428/586365824869335069.gif?size=4096"},
            {42, "https://cdn.discordapp.com/attachments/850561678982774814/850565579818795018/Screenshot_2021-05-28_22.26.29.png?size=4096"},
            {43, "https://media.discordapp.net/attachments/357317690815152128/769146791303708732/funny_shark.gif"}
        };

        public async Task SendWelcomeMessageAsync(SocketGuildUser user)
        {
            var channel = Client.GetChannel(837081902631878697) as SocketTextChannel;
            await Task.Delay(500);
            Random rnd = new Random();
            var msgIndex = rnd.Next(0, WelcomeMessages.Length);
            var embed = new EmbedBuilder();
            embed.WithColor(Color.Blue);
            embed.WithTitle("User Get!");
            embed.WithDescription(this.WelcomeMessages[msgIndex].Replace("<@>", $"**{user.Username}**"));
            if (this._associatedImageLinks.ContainsKey(msgIndex)) embed.WithImageUrl(_associatedImageLinks[msgIndex]);
            embed.WithThumbnailUrl(user.GetAvatarUrl());
            embed.WithCurrentTimestamp();
            embed.WithFooter("Use 'faq' before asking any questions", Program._client.CurrentUser.GetAvatarUrl());
            var emb = await channel.SendMessageAsync(embed: embed.Build());
            await emb.AddReactionAsync(new Emoji("👋"));
        }

    }
}
