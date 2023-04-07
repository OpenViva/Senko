import type {
  AutocompleteInteraction,
  ChatInputCommandInteraction,
  TextChannel,
} from "discord.js";

import env from "../env.js";
import { Command, RegisterBehavior } from "@sapphire/framework";
import { ApplyOptions } from "@sapphire/decorators";

@ApplyOptions<Command.Options>({
  name: "faq",
  description: "Get answers to your questions!`",
})
export class PingCommand extends Command {
  public override registerApplicationCommands(registry: Command.Registry) {
    registry.registerChatInputCommand(
      (builder) =>
        builder
          .setName(this.name)
          .setDescription(this.description)
          .addStringOption((builder) =>
            builder
              .setName("question")
              .setDescription("The Question to answer!")
              .setAutocomplete(true)
              .setRequired(true)
          ),
      {
        behaviorWhenNotIdentical: RegisterBehavior.Overwrite,
      }
    );
  }

  public override async autocompleteRun(interaction: AutocompleteInteraction) {
    const partialQuestion = interaction.options.getFocused().toString();
    let questions = await this.container.client.db("faq").select();

    questions = questions.filter((question) =>
      question.question.startsWith(partialQuestion)
    );

    interaction.respond(
      questions.map(({ question }) => ({ name: question, value: question }))
    );
  }

  public override async chatInputRun(interaction: ChatInputCommandInteraction) {
    this.container.logger.info("Someone asked a question");
    const question = interaction.options.getString("question", true);

    await interaction.deferReply();

    const answer = await this.container.client
      .db("faq")
      .select("answer")
      .where("question", question)
      .first();
    if (answer) return interaction.editReply(answer.answer);
    const channel = (await this.container.client.channels.fetch(
      env.UNANSWERED_CHANNEL
    )) as TextChannel;

    await channel.send(`<@${interaction.user.id}> Asked: "${question}"`);
    await interaction.editReply(
      `We dont have an answer for "${question}", but don't worry, we forwarded it to the dev team!`
    );
    return;
  }
}
