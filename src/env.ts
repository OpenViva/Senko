import "dotenv/config.js";

import type { PresenceStatusData } from "discord.js";

import { ActivityType } from "discord.js";
import env from "env-var";

export default {
  UNANSWERED_CHANNEL: env.get("UNANSWERED_CHANNEL").required().asString(),
  WELCOME_CHANNEL: env.get("WELCOME_CHANNEL").required().asString(),
  WEBSITE_URL: env.get("WEBSITE_URL").required().asString(),
  DISCORD_TOKEN: env.get("DISCORD_TOKEN").required().asString(),
  ACTIVITY_TYPE: (env.get("ACTIVITY_TYPE").asString() ??
    ActivityType.Playing) as
    | ActivityType.Playing
    | ActivityType.Streaming
    | ActivityType.Listening
    | ActivityType.Watching
    | ActivityType.Competing,
  ACTIVITY_STATUS: (env.get("ACTIVITY_STATUS").asString() ??
    "online") as PresenceStatusData,
  ACTIVITY_NAME: env.get("ACTIVITY_NAME").asString() ?? "Uhh",
  KOBOLD_KEY: env.get("KOBOLD_KEY").asString() ?? "0000000000",
  KOBOLD_MESSAGE_LENGTH:
    env.get("KOBOLD_MESSAGE_LENGTH").asIntPositive() ?? Infinity,
  KOBOLD_TEMPERATURE: env.get("KOBOLD_TEMPERATURE").asString() ?? "0.62",
  CHATBOT_CHANNEL: env.get("CHATBOT_CHANNEL").required().asString(),
  CHATBOT_PERSONA:
    env.get("CHATBOT_PERSONA").asString() ?? "A friendly AI chatbot.",
  CHATBOT_HELLO:
    env.get("CHATBOT_HELLO").asString() ??
    "Hey there! How can I help you today?",
  CHATBOT_MEMORY: env.get("CHATBOT_MEMORY").asIntPositive() ?? Infinity,
  CHATBOT_LIMIT: env.get("CHATBOT_LIMIT").asIntPositive() ?? Infinity,
  CHATBOT_FILTER: env.get("CHATBOT_FILTER").asString() ?? "sex",
  // A comma separated list of models to use
  CHATBOT_MODELS:
    env.get("CHATBOT_MODELS").asString() ??
    "PygmalionAI/pygmalion-6b,PygmalionAI/pygmalion-7b",
  CHATBOT_LIMITER: env.get("CHATBOT_LIMITER").asString() ?? "<CLEAR>",
  CHATBOT_REACTION: env.get("CHATBOT_REACTION").asString() ?? "⌛",
};
