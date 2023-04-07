import knex from "knex";

const db = knex({
  client: "sqlite3",
  useNullAsDefault: true,
  connection: {
    filename: "./data.db",
  },
});

if (!(await db.schema.hasTable("faq")))
  await db.schema.createTable("faq", (table) => {
    table.string("question").unique();
    table.string("answer");
  });

if (!(await db.schema.hasTable("cursedPeople")))
  await db.schema.createTable("cursedPeople", (table) =>
    table.string("id").unique()
  );

export default db;
