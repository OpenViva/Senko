import { Message, PermissionFlagsBits } from "discord.js";
import { ApplyOptions } from "@sapphire/decorators";
import { Listener } from "@sapphire/framework";
import Uwuifier from "uwuifier";

const uwuifier = new Uwuifier();

console.log("Initalized Curse Listener");

@ApplyOptions<Listener.Options>({
  name: "curse",
  event: "messageCreate",
})
export class CurseListener extends Listener {
  public async run(message: Message) {
    if (message.channel.isDMBased() || message.channel.isThread()) return;
    if (
      !message.channel
        .permissionsFor(this.container.client.id ?? "")
        ?.has([
          PermissionFlagsBits.ManageMessages,
          PermissionFlagsBits.ManageWebhooks,
        ])
    )
      return;
    const isCursed = !!(await this.container.client
      .db("cursedPeople")
      .select()
      .where("id", message.author.id)
      .first());
    if (!isCursed) return;
    const { content, author, member, attachments, tts } = message;

    const avatarURL = member?.displayAvatarURL() || author.avatarURL() || "";
    const username = member?.nickname || author.username;

    const webhooks = await message.channel.fetchWebhooks();
    let webhook = webhooks.find((webhook) => webhook.token);
    if (!webhook)
      webhook = await message.channel.createWebhook({
        name: this.container.client.user?.username ?? "Senko",
      });

    await webhook.send({ 
      avatarURL, 
      username: uwuifier.uwuifyWords(username), 
      content: uwuifier.uwuifySentence(content), 
      files: Array.from(attachments.values()),
      allowedMentions: { parse: [] },
      tts,
      });
    await message.delete();
  }
}
