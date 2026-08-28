import { getCurrentWindow } from '@tauri-apps/api/window';
import { createApp } from 'vue';
import App from '@/App.vue';

import router from './router';
import '@/css/styles.css';

import { isRunningInTauri, setPageLoader } from './utils/Utils.ts';
import { api } from "./services/Api.ts";

const app = createApp(App);

if (isRunningInTauri)
  await getCurrentWindow().show();

const connected = await api.connect()

app.use(router);
app.mount('#app');

if ('scrollRestoration' in history)
  history.scrollRestoration = 'manual';

document.documentElement.style.backgroundColor = '#2a2a2a';

if (connected)
  setPageLoader(false);