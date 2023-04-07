import type { Knex } from "knex";

declare module "knex/types/tables" {
  interface Question {
    question: string;
    answer: string;
  }

  interface CursedPerson {
    id: string;
  }

  interface Tables {
    faq: Question;
    cursedPeople: CursedPerson;
  }
}

declare module "discord.js" {
  interface Client {
    db: Knex;
  }
}
