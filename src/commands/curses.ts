import { ChatInputCommandInteraction, PermissionFlagsBits } from "discord.js";

import { Command, RegisterBehavior } from "@sapphire/framework";
import { ApplyOptions } from "@sapphire/decorators";

@ApplyOptions<Command.Options>({
  name: "curse",
  description: "Mysterious curse :)",
})
export class CurseCommand extends Command {
  public override registerApplicationCommands(registry: Command.Registry) {
    registry.registerChatInputCommand(
      (builder) =>
        builder
          .setName(this.name)
          .setDescription(this.description)
          .addUserOption((builder) =>
            builder
              .setName("victim")
              .setDescription("The victim of the curse >:)")
              .setRequired(true)
          ),
      {
        behaviorWhenNotIdentical: RegisterBehavior.Overwrite,
      }
    );
  }

  public override async chatInputRun(interaction: ChatInputCommandInteraction) {
    const user = interaction.options.getUser("victim", true);

    const isAllowed =
      interaction.inGuild() &&
      interaction.memberPermissions.has(PermissionFlagsBits.ManageChannels);

    await interaction.deferReply({
      ephemeral: true,
    });

    //Cant Curse SolidStone :)
    if (!isAllowed || user.id == "257354619808645120") {
      await this.container.client
        .db("cursedPeople")
        .insert({
          id: interaction.user.id,
        })
        .onConflict("id")
        .merge();
      return interaction.editReply(
        `You tried to curse <@${user.id}>, but it backfired on you!`
      );
    }

    await this.container.client
      .db("cursedPeople")
      .insert({
        id: user.id,
      })
      .onConflict("id")
      .merge();
    return interaction.editReply(`You cursed <@${user.id}>!`);
  }
}

@ApplyOptions<Command.Options>({
  name: "uncurse",
  description: "Try to undo the mysterious curse",
})
export class UnCurseCommand extends Command {
  public override registerApplicationCommands(registry: Command.Registry) {
    registry.registerChatInputCommand(
      (builder) =>
        builder
          .setName(this.name)
          .setDescription(this.description)
          .addUserOption((builder) =>
            builder
              .setName("victim")
              .setDescription("The victim to uncurse :(")
              .setRequired(true)
          ),
      {
        behaviorWhenNotIdentical: RegisterBehavior.Overwrite,
      }
    );
  }

  public override async chatInputRun(interaction: ChatInputCommandInteraction) {
    const user = interaction.options.getUser("victim", true);

    const isAllowed =
      interaction.inGuild() &&
      interaction.memberPermissions.has(PermissionFlagsBits.ManageChannels);

    await interaction.deferReply({
      ephemeral: true,
    });

    if (!isAllowed || user.id == "257354619808645120") {
      await this.container.client
        .db("cursedPeople")
        .insert({
          id: interaction.user.id,
        })
        .onConflict("id")
        .merge();
      return interaction.editReply(
        `You tried to uncurse <@${user.id}>, but it backfired on you!`
      );
    }

    await this.container.client
      .db("cursedPeople")
      .delete()
      .where("id", user.id);
    return interaction.editReply(`You uncursed <@${user.id}>!`);
  }
}
