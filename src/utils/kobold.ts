interface JobCreateOptions {
  prompt: string;
  params: {
    n: number;
    frmtadsnsp?: boolean;
    frmtrmblln?: boolean;
    frmtrmspch?: boolean;
    frmttriminc?: boolean;
    max_context_length: number;
    max_length: number;
    rep_pen: number;
    rep_pen_range: number;
    rep_pen_slope: number;
    singleline?: boolean;
    soft_prompt?: string;
    temperature: number;
    tfs: number;
    top_a: number;
    top_k: number;
    top_p: number;
    typical: number;
    sampler_order: number[];
  };
  softprompts?: string[];
  trusted_workers?: boolean;
  nsfw?: boolean;
  workers: string[];
  models: string[];
}

interface JobCreateResponse {
  id: string;
}

export interface JobStatusResponse {
  generations: {
    worker_id: string;
    worker_name: string;
    model: string;
    text: string;
  }[];
  finished: number;
  processing: number;
  restarted: number;
  waiting: number;
  done: boolean;
  faulted: boolean;
  wait_time: number;
  queue_position: number;
  kudos: number;
  is_possible: boolean;
}

export interface KoboldError {
  message: string;
}

function sleep(milliseconds: number) {
  return new Promise<void>((resolve) => setTimeout(resolve, milliseconds));
}

function getMillisToSleep(retryHeader: string) {
  let millisToSleep = Math.round(parseFloat(retryHeader) * 1000);
  if (isNaN(millisToSleep))
    millisToSleep = Math.max(
      0,
      new Date(retryHeader).getMilliseconds() - Date.now()
    );
  return millisToSleep;
}

async function fetchAndRetryIfNecessary(
  callAPIFn: () => Promise<Response>
): Promise<Response> {
  const response = await callAPIFn();

  if (response.status == 429) {
    const retryAfter = response.headers.get("retry-after");
    const millisToSleep = retryAfter ? getMillisToSleep(retryAfter) : 0;
    await sleep(millisToSleep);
    return fetchAndRetryIfNecessary(callAPIFn);
  }
  return response;
}

export class KoboldAIHorde {
  apiKey: string;
  defaultOptions: JobCreateOptions;
  constructor(apiKey?: string, options?: Partial<JobCreateOptions>) {
    this.apiKey = apiKey || "0000000000";
    this.defaultOptions = {
      prompt: "",
      models: [],
      params: {
        max_context_length: 1024,
        max_length: 40,
        n: 1,
        rep_pen: 1.08,
        rep_pen_range: 1024,
        rep_pen_slope: 0.7,
        sampler_order: [6, 0, 1, 2, 3, 4, 5],
        temperature: 0.62,
        tfs: 1,
        top_a: 0,
        top_k: 0,
        top_p: 0.9,
        typical: 1,
        singleline: true,
      },
      workers: [],
      ...options,
    };
  }

  async createJob(text: string): Promise<string> {
    const res = await fetchAndRetryIfNecessary(() =>
      fetch("https://stablehorde.net/api/v2/generate/text/async", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          apiKey: this.apiKey,
        },
        body: JSON.stringify({
          ...this.defaultOptions,
          prompt: text,
        } as JobCreateOptions),
      })
    );

    if (res.status != 202) {
      const json: KoboldError = await res.json();
      throw new Error(json.message);
    }

    const json: JobCreateResponse = await res.json();
    return json.id;
  }

  async getJob(id: string): Promise<JobStatusResponse> {
    const res = await fetchAndRetryIfNecessary(() =>
      fetch(`https://stablehorde.net/api/v2/generate/text/status/${id}`, {
        headers: {
          "Content-Type": "application/json",
        },
      })
    );

    if (res.status != 200) {
      const json: KoboldError = await res.json();
      throw new Error(json.message);
    }

    const json: JobStatusResponse = await res.json();
    return json;
  }

  async cancelJob(id: string): Promise<JobStatusResponse> {
    const res = await fetchAndRetryIfNecessary(() =>
      fetch(`https://stablehorde.net/api/v2/generate/text/status/${id}`, {
        method: "DELETE",
        headers: {
          "Content-Type": "application/json",
        },
      })
    );

    if (res.status != 200) {
      const json: KoboldError = await res.json();
      throw new Error(json.message);
    }

    const json: JobStatusResponse = await res.json();
    return json;
  }
}
