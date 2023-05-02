import { ApplyOptions } from "@sapphire/decorators";
import { Listener } from "@sapphire/framework";
import { TextChannel, EmbedBuilder, User, GuildMember } from "discord.js";
import welcomeMessages from "../welcome.json" assert { type: "json" };
import env from "../env.js";

console.log("Initalized Welcome Listener");

@ApplyOptions<Listener.Options>({
  name: "welcome",
  event: "guildMemberAdd",
})
export class WelcomeListener extends Listener {
  public async run(user: GuildMember) {
    if (env.WELCOME_MEMBERS) this.sendWelcomeMessage(user.user);
  }

  public async sendWelcomeMessage(user: User): Promise<void> {
    const channel = this.container.client.channels.cache.get(
      env.WELCOME_CHANNEL
    ) as TextChannel;
    await new Promise((resolve) => setTimeout(resolve, 500));
    const msg = welcomeMessages[this.getRandomInt(welcomeMessages.length)];
    const embed = new EmbedBuilder()
      .setColor(0x343498)
      .setTitle("User Get!")
      .setDescription(msg?.message.replace("<@>", `**${user.username}**`) ?? "")
      .setThumbnail(user?.displayAvatarURL())
      .setTimestamp()
      .setFooter({
        text: "Use 'faq' before asking any questions",
        iconURL: this.container.client.user?.avatarURL() as string,
      });
    if (msg?.image) embed.setImage(msg?.image);

    const message = await channel.send({ embeds: [embed] });
    await message.react("👋");
  }

  private getRandomInt(max: number): number {
    return Math.floor(Math.random() * max);
  }
}
