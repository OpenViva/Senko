import type {
  AutocompleteInteraction,
  ChatInputCommandInteraction,
} from "discord.js";

import { RegisterBehavior } from "@sapphire/framework";
import { ApplyOptions } from "@sapphire/decorators";
import { Subcommand } from "@sapphire/plugin-subcommands";

@ApplyOptions<Subcommand.Options>({
  name: "faqadmin",
  description: "Check our FAQ",
  subcommands: [
    {
      name: "set",
      chatInputRun: "set",
    },
    {
      name: "remove",
      chatInputRun: "remove",
    },
  ],
})
export class FaqAdminCommand extends Subcommand {
  public override registerApplicationCommands(registry: Subcommand.Registry) {
    registry.registerChatInputCommand(
      (builder) =>
        builder
          .setName(this.name)
          .setDescription(this.description)
          .addSubcommand((builder) =>
            builder
              .setName("set")
              .setDescription("Set a new FAQ")
              .addStringOption((builder) =>
                builder
                  .setName("question")
                  .setDescription("The FAQ question")
                  .setAutocomplete(true)
                  .setRequired(true)
              )
              .addStringOption((builder) =>
                builder
                  .setName("answer")
                  .setDescription("The FAQ answer")
                  .setRequired(true)
              )
          )
          .addSubcommand((builder) =>
            builder
              .setName("remove")
              .setDescription("Remove a FAQ")
              .addStringOption((builder) =>
                builder
                  .setName("question")
                  .setDescription("The FAQ question")
                  .setAutocomplete(true)
                  .setRequired(true)
              )
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

  public async set(interaction: ChatInputCommandInteraction) {
    this.container.logger.info("Someone Set a FAQ");
    const question = interaction.options.getString("question", true);
    const answer = interaction.options.getString("answer", true);
    await interaction.deferReply({
      ephemeral: true,
    });

    await this.container.client
      .db("faq")
      .insert({
        question,
        answer,
      })
      .onConflict("question")
      .merge();

    await interaction.editReply("You added a question/answer to the FAQ");
    return;
  }

  public async remove(interaction: ChatInputCommandInteraction) {
    this.container.logger.info("Someone Removed a FAQ");
    const question = interaction.options.getString("question", true);
    await interaction.deferReply({
      ephemeral: true,
    });

    await this.container.client.db("faq").delete().where("question", question);

    await interaction.editReply("You removed a question/answer from the FAQ");
    return;
  }
}
