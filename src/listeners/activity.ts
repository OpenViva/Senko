import env from "../env.js";

import { ApplyOptions } from "@sapphire/decorators";
import { Listener } from "@sapphire/framework";

console.log("Initalized Activity Listener");

@ApplyOptions<Listener.Options>({
  name: "activity",
  event: "ready",
  once: true,
})
export class ActivityListener extends Listener {
  public async run() {
    this.setPresence();
    setInterval(() => this.setPresence(), 30000);
  }

  public setPresence() {
    this.container.client.user?.setPresence({
      status: env.ACTIVITY_STATUS,
      activities: [
        {
          type: env.ACTIVITY_TYPE,
          name: env.ACTIVITY_NAME,
        },
      ],
    });
  }
}
