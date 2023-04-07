import {
  ActionRowBuilder,
  ButtonBuilder,
  ButtonStyle,
  ChatInputCommandInteraction,
} from "discord.js";
import { Command, RegisterBehavior } from "@sapphire/framework";
import { ApplyOptions } from "@sapphire/decorators";

@ApplyOptions<Command.Options>({
  name: "pat",
  description: "Pat Senko",
})
export class PatCommand extends Command {
  public override registerApplicationCommands(registry: Command.Registry) {
    registry.registerChatInputCommand(
      (builder) => builder.setName(this.name).setDescription(this.description),
      {
        behaviorWhenNotIdentical: RegisterBehavior.Overwrite,
      }
    );
  }

  public override async chatInputRun(interaction: ChatInputCommandInteraction) {
    const PatButton = new ButtonBuilder()
      .setCustomId("DoPat")
      .setLabel("Pat")
      .setStyle(ButtonStyle.Success);

    const DontPatButton = new ButtonBuilder()
      .setCustomId("DontPat")
      .setLabel("Don't Pat")
      .setStyle(ButtonStyle.Danger);

    const actionRow = new ActionRowBuilder<ButtonBuilder>().addComponents(
      PatButton,
      DontPatButton
    );

    await interaction.reply({
      content: `Will you pat Senko?`,
      components: [actionRow],
      fetchReply: true,
    });
  }
}
