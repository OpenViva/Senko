import type { ChatInputCommandInteraction } from "discord.js";

import { isMessageInstance } from "@sapphire/discord.js-utilities";
import { Command, RegisterBehavior } from "@sapphire/framework";
import { ApplyOptions } from "@sapphire/decorators";

@ApplyOptions<Command.Options>({
  name: "ping",
  description: "pingy pongy",
})
export class PingCommand extends Command {
  public override registerApplicationCommands(registry: Command.Registry) {
    registry.registerChatInputCommand(
      (builder) => builder.setName(this.name).setDescription(this.description),
      {
        behaviorWhenNotIdentical: RegisterBehavior.Overwrite,
      }
    );
  }

  public override async chatInputRun(interaction: ChatInputCommandInteraction) {
    this.container.logger.info("yay we got pingy");
    const msg = await interaction.reply({
      content: `Ping?`,
      ephemeral: true,
      fetchReply: true,
    });

    if (isMessageInstance(msg)) {
      const diff = msg.createdTimestamp - interaction.createdTimestamp;
      const ping = Math.round(this.container.client.ws.ping);
      return interaction.editReply(
        `Pong! (Round trip took: ${diff}ms. Heartbeat: ${ping}ms.)`
      );
    }

    return interaction.editReply("Failed to retrieve ping :(");
  }
}
