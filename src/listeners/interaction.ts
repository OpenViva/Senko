import type { Interaction } from "discord.js";

import { EmbedBuilder } from "@discordjs/builders";
import { ApplyOptions } from "@sapphire/decorators";
import { Listener } from "@sapphire/framework";

console.log("Initalized Interaction Listener");

@ApplyOptions<Listener.Options>({
  name: "interaction",
  event: "interactionCreate",
})
export class InteractionListener extends Listener {
  public async run(interaction: Interaction) {
    if (!interaction.isButton()) return;

    const authorName = interaction.user.tag;

    let imageUrl = "";
    let description = "";
    if (interaction.customId === "DoPat") {
      description = "You have patted Senko! \nThe Fox is now happy!";
      imageUrl =
        "https://cdn.discordapp.com/attachments/976660916831662113/1003077839882551356/senko-pat.gif";
    } else if (interaction.customId === "DontPat") {
      description = "You did not pat Senko. \nThe Fox is now sad how dare you!";
      imageUrl =
        "https://cdn.discordapp.com/attachments/976660916831662113/1003078352535552040/sad-senko.gif";
    } else {
      return;
    }

    const embed = {
      author: {
        name: authorName,
      },
      description: description,
      image: {
        url: imageUrl,
      },
      color: 0x00ff00,
    };

    await interaction.reply({
      embeds: [new EmbedBuilder(embed).toJSON()],
      ephemeral: true,
    });
  }
}
