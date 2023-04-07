import env from "./env.js";

import { GatewayIntentBits, Partials } from "discord.js";
import { SapphireClient } from "@sapphire/framework";

import db from "./db.js";

const client = new SapphireClient({
  intents: [
    GatewayIntentBits.Guilds,
    GatewayIntentBits.GuildMessages,
    GatewayIntentBits.GuildMessageReactions,
    GatewayIntentBits.DirectMessageReactions,
    GatewayIntentBits.MessageContent,
    GatewayIntentBits.GuildMembers,
    GatewayIntentBits.GuildPresences,
  ],
  partials: [Partials.Reaction],
});

client.db = db;

client.login(env.DISCORD_TOKEN);
