import type { JobStatusResponse } from "../utils/kobold.js";

import { ApplyOptions } from "@sapphire/decorators";
import { Listener } from "@sapphire/framework";
import { ChannelType, Message, PermissionFlagsBits } from "discord.js";
import env from "../env.js";
import { KoboldAIHorde } from "../utils/kobold.js";
import emojiRegex from "emoji-regex";

const models = env.CHATBOT_MODELS.split(",");
const memoryTimeLimit = env.CHATBOT_MEMORY * 60000;
const memoryLengthLimit = Math.min(env.CHATBOT_LIMIT, 100);
const isReactionEmoji = emojiRegex().test(env.CHATBOT_REACTION);

//List of bad words
const wordsthatmightgetusbanned: string[] = [
  "nazi",
  "hitler",
  "nigger",
  "nigga",
  "loli",
  "vagina",
  "sex",
  "child ",
];

let jobRequestCancel: (() => void) | undefined;
const horde = new KoboldAIHorde(env.KOBOLD_KEY, {
  models,
});

async function replaceAsync(
  str: string,
  regex: RegExp,
  asyncFn: (match: string, ...args: unknown[]) => Promise<string>
) {
  const promises: Promise<string>[] = [];
  str.replace(regex, (match, ...args) => {
    const promise = asyncFn(match, ...args);
    promises.push(promise);
    return match;
  });
  const data = await Promise.all(promises);
  return str.replace(regex, () => data.shift() || "");
}

@ApplyOptions<Listener.Options>({
  name: "chatbot",
  event: "messageCreate",
})
export class ChatbotListener extends Listener {
  async parseUserInput(message: Message) {
    return (
      (await replaceAsync(
        // Make all emojis :emoji: instead of <:emoji:id>
        message.content
          // Make it single-line
          .replaceAll("\n", " ")
          // Make all emojis :emoji:
          .replaceAll(
            /<(?:(?<animated>a)?:(?<name>\w{2,32}):)?(?<id>\d{17,21})>/g,
            (...args) => `:${args[2]}:`
          ),
        /<@!?(?<id>\d{17,20})>/g,
        // Make all user mentions @User instead of <@id>
        async (...args) => {
          const id = args[1] as string;

          // Fetch the user
          const user = await this.container.client.users.fetch(id);

          // If the user is not found, return a generic @
          if (!user) return "@User";

          // If the user is found, return their username
          return `@${user.username}`;
        }
      )) +
      // Add all attachments to the end
      message.attachments.map((attachment) => ` ${attachment.url}`).join("")
    );
  }

  async createPrompt(
    name: string,
    persona: string,
    hello: string,
    memory: Message[]
  ) {
    const helloName = memory.find((message) => message.author.username !== name)
      ?.author.username;

    // If we have a persona, add it to the prompt
    let prompt = persona ? `${name}'s Persona: ${persona}\n` : "";

    // The docs say to add this as a delimiter
    prompt += "<START>\n";

    // If we have a hello message, add it to the prompt
    !hello || (prompt += `${helloName || "User"}: Hello ${name}!\n`);
    !hello || (prompt += `${name}: ${hello}\n`);

    // Add all the messages in the memory to the prompt
    prompt += (
      await Promise.all(
        memory.map(
          async (message) =>
            `${message.author.username}: ${await this.parseUserInput(message)}`
        )
      )
    ).join("\n");

    // Add the chat bot's name to the prompt
    prompt += `\n${name}:`;
    return prompt;
  }

  private parseInput(message: string) {
    // The AI likes to impersonate the user, remember to check for that
    const lines = message.trim().split("\n");

    // The first line is always the bot's response
    const botLine = lines.splice(0, 1)[0] || "...";

    // Get all lines that start with the bot's name
    let foundImpersonation = false;
    const botLines = lines
      .filter((line) => {
        if (foundImpersonation) return false;
        if (line.startsWith(`${this.name}: `)) return true;
        foundImpersonation = true;
        return false;
      })
      .map((line) => line.replace(`${this.name}: `, "").trim());

    return [botLine, ...botLines];
  }

  public async run(message: Message) {
    if (message.channelId != env.CHATBOT_CHANNEL) return;
    if (message.channel.type == ChannelType.DM) return;

    // Do not run if the bot does not have the required permissions, we do not need to worry if we're in a DM
    if (
      !message.channel
        .permissionsFor(this.container.client.user?.id || "")
        ?.has([
          PermissionFlagsBits.SendMessages,
          PermissionFlagsBits.ReadMessageHistory,
        ])
    )
      return;

    // If the message is the limiter, react to it
    if (
      message.content == env.CHATBOT_LIMITER &&
      (!isReactionEmoji ||
        message.channel
          .permissionsFor(this.container.client.user?.id || "")
          ?.has(PermissionFlagsBits.AddReactions))
    ) {
      jobRequestCancel?.();

      return isReactionEmoji
        ? message.react(env.CHATBOT_REACTION)
        : message.reply(env.CHATBOT_REACTION);
    }

    // Do not run if the message is from a webhook or the bot
    if (message.author.id == this.container.client.user?.id) return;

    // Get the chatbot's configs, with defaults
    const chatbotName = this.container.client.user?.username || "Robot";
    const chatbotPersona = env.CHATBOT_PERSONA;
    const chatbotHello = env.CHATBOT_HELLO;

    // Get message reference (reply)
    const reference = message.reference ? await message.fetchReference() : null;

    // If we have not been mentioned, stop
    if (
      !jobRequestCancel &&
      !message.mentions.has(this.container.client.user?.id || "") &&
      reference?.author.id != this.container.client.user?.id
    )
      return;

    // Start typing
    await message.channel.sendTyping();

    let messages = [
      ...(
        await message.channel.messages.fetch({
          limit: memoryLengthLimit,
        })
      ).values(),
    ];

    // Filter the messages that our bot should not remember
    messages = messages.filter(
      (m) => Date.now() - m.createdAt.getTime() <= memoryTimeLimit
    );

    // If any messages are the limiter, we need to remove all previous messages
    const limiterIndex = messages.findIndex(
      (m) => m.content == env.CHATBOT_LIMITER
    );
    if (limiterIndex >= 0) messages = messages.slice(0, limiterIndex);

    // createPrompt expects the most recent message to be last,
    // Currently the messages are in reverse order
    messages = messages.reverse();

    // Make sure that a non-bot message is the first message
    const firstMessageIndex = messages.findIndex(
      (m) => !(m.webhookId || m.author.id == this.container.client.id)
    );
    if (firstMessageIndex >= 0) messages = messages.slice(firstMessageIndex);

    // Construct the prompt
    const prompt = await this.createPrompt(
      chatbotName,
      chatbotPersona,
      chatbotHello,
      messages
    );

    // Cancel the current job if it exists
    jobRequestCancel?.();

    // Make sure we catch any cancel requests
    let cancelled = false;
    jobRequestCancel = () => (cancelled = true);

    // Create a new job
    const jobId = await horde.createJob(prompt);

    // Cancel the job if we received a cancel request
    if (cancelled) {
      await horde.cancelJob(jobId);
      return;
    }

    // Now that the job is created, we can make the cancel function cancel the job
    jobRequestCancel = () => {
      cancelled = true;
      horde.cancelJob(jobId);
    };

    // Get the response
    let job: JobStatusResponse | undefined;
    while (!job?.done) {
      // Wait a bit before checking again
      await new Promise((resolve) => setTimeout(resolve, 1500));

      // Continue typing
      await message.channel.sendTyping();

      // Return if we received a cancel request
      if (cancelled) return;

      // Get the job
      job = await horde.getJob(jobId);

      // If the job failed, return
      if (!job.is_possible || job.faulted) return;
    }

    // Delete the cancel function if we finished without a cancel request
    if (!cancelled) jobRequestCancel = undefined;

    const botMessages = this.parseInput(job.generations[0]?.text || "...");
    // Send the messages
    for (const botMessage of botMessages) {
      for (const badword of wordsthatmightgetusbanned) {
        //Filter the bad messages
        if (botMessage.toLowerCase().includes(badword.toLowerCase())) {
          await message.channel.send("[Redacted]");
          console.log(`Senko tried to say [${botMessage}] but was denied`);
          return;
        }
      }
      await message.channel.send(botMessage);
    }
    return;
  }
}
