import { client } from "./client/client.gen";

export default function clientSetup() {
  client.setConfig({
    baseUrl: import.meta.env.VITE_API_URL,
  });
}
