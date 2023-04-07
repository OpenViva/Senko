import "dotenv/config.js";

import type { PresenceStatusData } from "discord.js";

import { ActivityType } from "discord.js";
import env from "env-var";

export default {
  UNANSWERED_CHANNEL: env.get("UNANSWERED_CHANNEL").required().asString(),
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
};
